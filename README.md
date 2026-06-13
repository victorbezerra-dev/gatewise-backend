# GateWise Backend

Licensed under the GNU Affero General Public License v3.0 or later (AGPL-3.0-or-later). See [LICENSE](LICENSE).

Sistema de controle de acesso físico multi-tenant. Organizações independentes (empresas, condomínios, universidades) gerenciam seus próprios espaços e membros. Autentica usuários via Keycloak, verifica assinaturas RSA de dispositivos móveis, envia comandos de desbloqueio para fechaduras ESP32 via MQTT e entrega confirmações em tempo real via WebSocket (SignalR).

## Hardware Image 
<img width="400" height="500" alt="WhatsApp Image 2026-06-12 at 18 38 12" src="https://github.com/user-attachments/assets/92ba7797-2ed0-443d-a2c5-ecff2587b592" />

## Architecture

```
Mobile App / Web Panel ──(JWT + RSA)──► API (ASP.NET Core 9) ──(RSA + MQTT)──► ESP32 (door lock)
                                                 │                                      │
                                           Keycloak (OIDC)               (RSA confirmation)
                                                 │                                      │
                                            RabbitMQ ◄──── SPI Provider (Java) ────────┘
                                                 │
                                 ┌───────────────┴───────────────┐
                          User Events Consumer             Outbox Worker
                          (sync users → PostgreSQL)        (publish pending events)
```

**Services:**

| Service | Technology | Description |
|---|---|---|
| `backend/` | C# / ASP.NET Core 9 | Main REST API |
| `keycloak/` | Keycloak 26 | Identity Provider with custom login theme |
| `spiprovider/` | Java / Maven | Keycloak SPI for university IDP (matricula auth) |
| `sync-user-events-message-queue/` | Node.js | RabbitMQ consumer — syncs users to PostgreSQL |
| `event-sync-outbox-worker/` | Node.js | Outbox worker — publishes pending events to RabbitMQ |
| `esp32-simulator/` | Node.js | Development simulator for ESP32 door lock |

**Backend projects:**
- `GateWise.Api` — Controllers, authentication, middleware
- `GateWise.Core` — Entities, DTOs, interfaces, enums
- `GateWise.Infrastructure` — Repositories, services, EF Core, SignalR hub

## Domain Model

```
Organization (IsInstitutional flag for pre-provisioned orgs like IFRO)
 ├── OrganizationMember (userId + role: owner | manager | member)
 ├── OrganizationInvite (code-based or email-based invite)
 └── Space
      ├── AccessGrant  (user permission request — pending | granted | rejected)
      └── AccessLog    (cryptographic audit trail of every access attempt)
```

**Roles within an organization:**

| Role | Can do |
|---|---|
| `owner` | Full control — manage members, spaces, invites |
| `manager` | Create/edit spaces, approve/reject access grants |
| `member` | View spaces, request access |

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9 / ASP.NET Core |
| Database | PostgreSQL 15 + Entity Framework Core 9 |
| Auth | Keycloak 26 (JWT Bearer / OIDC) |
| Messaging | MQTT (MQTTnet) + RabbitMQ 3.12 |
| Real-time | SignalR |
| Cryptography | RSA-2048 / SHA-256 |
| Containers | Docker + Docker Compose |

## Prerequisites

- [Docker](https://www.docker.com/) and Docker Compose
- A devtunnel or public URL pointing to your Keycloak instance (required for mobile clients)
- RSA key files for the backend and ESP32 (see [Cryptographic Keys](#cryptographic-keys))

## Getting Started

### 1. Configure environment variables

```bash
cp deploy/.env.example deploy/.env
```

Edit `deploy/.env` and fill in all values — especially `KC_HOSTNAME` with your devtunnel URL:

```env
KC_HOSTNAME=https://your-tunnel-url.brs.devtunnels.ms
KEYCLOAK_ADMIN_PASSWORD=your_password
KC_DB_PASSWORD=your_password
GATEWISE_DB_PASSWORD=your_password
RABBITMQ_PASS=your_password
```

### 2. Add cryptographic keys

Place the following PEM files inside `deploy/secrets/`:

```
secrets/
├── private_backend.pem   # Backend RSA private key (signs MQTT commands)
└── public_esp32.pem      # ESP32 RSA public key (verifies door confirmations)
```

### 3. Start the services

```bash
cd deploy
docker compose -f docker-compose.base.yml -f docker-compose.dev.yml up
```

| Service | URL |
|---|---|
| API (Swagger) | http://localhost:5353/swagger |
| Keycloak Admin | http://localhost:8080/admin |
| RabbitMQ Management | http://localhost:15672 |

**Production:**

```bash
docker compose -f docker-compose.base.yml -f docker-compose.prod.yml up
```

### 4. Apply database migrations

```bash
# via Docker
docker compose run api --migrate

# or locally
dotnet run --project backend/GateWise.Api -- --migrate
```

## Environment Variables

| Variable | Description |
|---|---|
| `KC_HOSTNAME` | Public Keycloak URL (devtunnel or domain). Used as JWT issuer. |
| `KEYCLOAK_ADMIN` | Keycloak admin username |
| `KEYCLOAK_ADMIN_PASSWORD` | Keycloak admin password |
| `KC_DB_NAME` | Keycloak internal database name |
| `KC_DB_USER` | Keycloak database user |
| `KC_DB_PASSWORD` | Keycloak database password |
| `GATEWISE_DB_NAME` | GateWise application database name |
| `GATEWISE_DB_USER` | GateWise database user |
| `GATEWISE_DB_PASSWORD` | GateWise database password |
| `RABBITMQ_USER` | RabbitMQ user |
| `RABBITMQ_PASS` | RabbitMQ password |
| `API_URL` | API base URL used by the Keycloak SPI provider |
| `API_AUTH_KEY` | Auth key for SPI-to-API communication |
| `IFRO_ORG_ID` | *(sync worker)* Organization ID for IFRO. When set, every synced user is automatically added as a member of this org. |

## User Registration Flow

### Self-service (Play Store / Web)

```
1. User registers or logs in via Keycloak → JWT
2. App calls GET /api/users/me
       └─ Backend upserts user from JWT claims
       └─ Auto-accepts any pending email invites
       └─ Returns user profile + organizations with roles
3. If organizations is empty → app shows onboarding:
       ├─ "Create organization" → POST /api/organizations (user becomes owner)
       └─ "Join with code"     → POST /api/organizations/join { code }
```

### Institutional (IFRO)

```
1. User logs in with matricula via Keycloak SPI → JWT
2. RabbitMQ sync worker creates user in DB and links to IFRO org (IFRO_ORG_ID)
3. App calls GET /api/users/me → user already has org membership
```

## API Endpoints

### Organizations

| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/organizations` | admin | List all organizations |
| `GET` | `/api/organizations/{id}` | member | Get organization by ID |
| `POST` | `/api/organizations` | authenticated | Create organization (caller becomes owner) |
| `PUT` | `/api/organizations/{id}` | owner, manager, admin | Update organization |
| `DELETE` | `/api/organizations/{id}` | admin | Delete organization |
| `GET` | `/api/organizations/{id}/members` | owner, manager, admin | List members |
| `DELETE` | `/api/organizations/{id}/members/{memberId}` | owner, manager, admin | Remove member |
| `GET` | `/api/organizations/{id}/invites` | owner, manager, admin | List invites |
| `POST` | `/api/organizations/{id}/invites` | owner, manager, admin | Create invite (returns code) |
| `DELETE` | `/api/organizations/{id}/invites/{inviteId}` | owner, manager, admin | Revoke invite |
| `POST` | `/api/organizations/join` | authenticated | Join org with invite code |

### Spaces

| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/spaces` | authenticated | List spaces (admin sees all; members see their org's) |
| `GET` | `/api/spaces/{id}` | member | Get space by ID |
| `POST` | `/api/spaces` | owner, manager, admin | Create space |
| `PUT` | `/api/spaces/{id}` | owner, manager, admin | Update space |
| `DELETE` | `/api/spaces/{id}` | owner, manager, admin | Delete space |
| `POST` | `/api/spaces/{id}/open` | authenticated | Request door access (mobile app) |
| `POST` | `/api/spaces/access-confirmation` | — | ESP32 door confirmation |

### Users

| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/users` | authenticated | List all users |
| `GET` | `/api/users/me` | authenticated | Upsert + get current user profile with organizations |
| `PATCH` | `/api/users/me/public-key` | authenticated | Register device RSA public key |
| `GET` | `/api/users/{id}` | — | Get user by ID |
| `PATCH` | `/api/users/{id}` | authenticated | Update user |
| `DELETE` | `/api/users/{id}` | — | Delete user |

### Access Grants

| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/accessGrants` | admin, manager | List grants (scoped to manager's org) |
| `GET` | `/api/accessGrants/{id}` | admin, manager | Get grant by ID |
| `POST` | `/api/accessGrants/request-access` | member | Request space access |
| `PUT` | `/api/accessGrants/{id}/review` | admin, manager | Approve or reject request |
| `DELETE` | `/api/accessGrants/{id}` | admin, manager | Delete grant |
| `GET` | `/api/accessGrants/user/{userId}` | authenticated | Get grants for a user |

### Space Managers

| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/spaceManagers` | admin | List all space managers |
| `GET` | `/api/spaceManagers/{id}` | admin | Get by ID |
| `POST` | `/api/spaceManagers` | admin | Assign manager to space |
| `DELETE` | `/api/spaceManagers/{id}` | admin | Remove manager |

### WebSocket

| Endpoint | Description |
|---|---|
| `ws://.../accessconfirmationhub` | SignalR hub — real-time access result notifications |

## Access Flow

```
1. Mobile app sends  { timestamp, signature }  to POST /api/spaces/{id}/open
       └─ API verifies user is a member of the space's org
       └─ API verifies signature using the device's registered public key

2. API publishes MQTT command to broker.hivemq.com
       └─ Payload: { commandId, timestamp, signature (signed by backend private key) }

3. ESP32 receives the command, verifies the backend signature, opens the lock
       └─ Sends confirmation: { commandId, timestamp, signature (signed by ESP32 key) }

4. API receives confirmation at POST /api/spaces/access-confirmation
       └─ Verifies ESP32 signature
       └─ Updates access log status
       └─ Broadcasts result via SignalR to connected clients
```

## Cryptographic Keys

The system uses RSA-2048 with SHA-256 for three distinct key pairs:

| Key | Held by | Used for |
|---|---|---|
| Device private key | Mobile app | Signing access requests |
| Backend private key | API server | Signing MQTT commands sent to ESP32 |
| ESP32 private key | ESP32 device | Signing door confirmation messages |

Each party verifies the other's signature using the corresponding public key before trusting any message.

## ESP32 Simulator

To test the unlock flow without physical hardware:

```bash
cd esp32-simulator
npm install
node esp32-simulator.js
```

The simulator connects to `broker.hivemq.com:1883`, subscribes to the `command/open-lock` topic, verifies the backend RSA signature, and sends the door confirmation to `https://gatewiseiot.com/api/spaces/access-confirmation`.

## Project Structure

```
GateWise-Backend/
├── backend/
│   ├── GateWise.Api/
│   │   ├── Controllers/       # HTTP endpoints
│   │   ├── Extensions/        # Auth, authorization, OpenAPI setup
│   │   └── Authorization/     # Keycloak claims transformer, custom policies
│   ├── GateWise.Core/
│   │   ├── Entities/          # Domain models (Organization, Space, User, ...)
│   │   ├── DTOs/              # Request/response contracts
│   │   ├── Interfaces/        # Repository and service contracts
│   │   └── Enums/             # Status and role enumerations
│   └── GateWise.Infrastructure/
│       ├── Repositories/      # EF Core data access
│       ├── Services/          # Business logic (access control, MQTT, RSA)
│       ├── Persistence/       # DbContext and entity configurations
│       ├── Migrations/        # EF Core migrations
│       └── WebSocket/         # SignalR hub
├── keycloak/                  # Custom theme and startup scripts
├── spiprovider/               # Java SPI for university IDP (matricula auth)
├── sync-user-events-message-queue/   # Node.js RabbitMQ consumer
├── event-sync-outbox-worker/         # Node.js outbox processor
├── esp32-simulator/           # Node.js ESP32 dev simulator
└── deploy/
    ├── docker-compose.base.yml
    ├── docker-compose.dev.yml
    ├── docker-compose.prod.yml
    ├── secrets/               # RSA key files (not tracked)
    └── .env.example
```

## License

GateWise Backend is free software licensed under the GNU Affero General Public License v3.0 or later (`AGPL-3.0-or-later`).

If you modify this software and make it available for users to interact with over a network, you must provide access to the Corresponding Source of your modified version as required by the AGPLv3. See [LICENSE](LICENSE) for the full license text.
