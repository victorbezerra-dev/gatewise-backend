# GateWise Backend

REST API for physical lab access control. Authenticates users via Keycloak, verifies RSA-signed requests from mobile devices, sends unlock commands to ESP32 devices over MQTT, and delivers real-time access results via WebSocket (SignalR).

## Architecture

```
Mobile App ──► API (ASP.NET Core 9) ──► MQTT Broker ──► ESP32 (door lock)
                │                                              │
                │ ◄────────────── Confirmation (MQTT) ────────┘
                │
                ▼
          SignalR Hub ──► Mobile App (real-time result)
```

**Projects:**
- `GateWise.Api` — Controllers, authentication, middleware
- `GateWise.Core` — Entities, DTOs, interfaces
- `GateWise.Infrastructure` — Repositories, services, EF Core, SignalR hub

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9 / ASP.NET Core |
| Database | PostgreSQL 15 + Entity Framework Core 9 |
| Auth | Keycloak (JWT Bearer) |
| Messaging | MQTT (MQTTnet) |
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

Place the following PEM files inside `backend/GateWise.Api/Keys/`:

```
Keys/
├── private_backend.pem   # Backend RSA private key (signs MQTT commands)
└── public_esp32.pem      # ESP32 RSA public key (verifies door confirmations)
```

### 3. Start the services

```bash
cd deploy
docker compose -f docker-compose.base.yml -f docker-compose.dev.yml up
```

The API will be available at `http://localhost:5353`.  
Swagger UI: `http://localhost:5353/swagger`

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

## API Endpoints

### Labs
| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/labs` | admin | List all labs |
| `GET` | `/api/labs/{id}` | admin | Get lab by ID |
| `POST` | `/api/labs` | admin | Create lab |
| `PUT` | `/api/labs/{id}` | admin | Update lab |
| `DELETE` | `/api/labs/{id}` | admin | Delete lab |
| `POST` | `/api/labs/{id}/open` | authenticated | Request door access |
| `POST` | `/api/labs/access-confirmation` | — | ESP32 access confirmation |

### Users
| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/users` | authenticated | List all users |
| `GET` | `/api/users/me` | authenticated | Get current user profile |
| `PATCH` | `/api/users/me/public-key` | authenticated | Register device RSA public key |
| `GET` | `/api/users/{id}` | — | Get user by ID |
| `PATCH` | `/api/users/{id}` | authenticated | Update user |
| `DELETE` | `/api/users/{id}` | — | Delete user |

### Access Grants
| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/accessGrants` | admin, professor | List all access grants |
| `GET` | `/api/accessGrants/{id}` | admin, professor | Get grant by ID |
| `POST` | `/api/accessGrants/request-access` | authenticated | Request lab access |
| `PUT` | `/api/accessGrants/{id}/review` | admin, professor | Approve or reject request |
| `DELETE` | `/api/accessGrants/{id}` | admin, professor | Delete grant |
| `GET` | `/api/accessGrants/user/{userId}` | authenticated | Get grants for a user |

### Lab Access Managers
| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/labAccessManagers` | admin | List all managers |
| `POST` | `/api/labAccessManagers` | admin | Assign manager to lab |
| `DELETE` | `/api/labAccessManagers/{id}` | admin | Remove manager |

### WebSocket
| Endpoint | Description |
|---|---|
| `ws://.../accessconfirmationhub` | SignalR hub — real-time access result notifications |

## Access Flow

```
1. Mobile app sends  { timestamp, signature }  to POST /api/labs/{id}/open
       └─ API verifies signature using the device's registered public key

2. API publishes MQTT command to the broker
       └─ Payload: { commandId, timestamp, signature (signed by backend private key) }

3. ESP32 receives the command, verifies the backend signature, opens the lock
       └─ Sends confirmation: { commandId, timestamp, signature (signed by ESP32 key) }

4. API receives confirmation at POST /api/labs/access-confirmation
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

## Project Structure

```
backend/
├── GateWise.Api/
│   ├── Controllers/       # HTTP endpoints
│   ├── Extensions/        # Auth, authorization, Swagger setup
│   └── Authorization/     # Keycloak claims transformer, custom policies
├── GateWise.Core/
│   ├── Entities/          # Domain models
│   ├── DTOs/              # Request/response contracts
│   ├── Interfaces/        # Repository and service contracts
│   └── Enums/             # Status and type enumerations
└── GateWise.Infrastructure/
    ├── Repositories/      # EF Core data access
    ├── Services/          # Business logic (access control, MQTT, RSA)
    ├── Persistence/       # DbContext and entity configurations
    ├── Migrations/        # EF Core migrations
    └── WebSocket/         # SignalR hub

deploy/
├── docker-compose.base.yml   # Keycloak + Keycloak DB
├── docker-compose.dev.yml    # Full dev stack
└── .env.example              # Environment variable template
```
