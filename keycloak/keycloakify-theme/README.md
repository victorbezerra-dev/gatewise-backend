# GateWise Keycloakify Theme

This directory contains the local Keycloakify project for the GateWise Keycloak login theme.

The source theme is intentionally kept as a **native Keycloak login theme** under:

```text
src/keycloak-theme/login
```

This preserves the same static theme already available at:

```text
../themes/gatewise
```

while allowing Keycloakify to package it as provider JARs.

## Commands

Install dependencies from this directory:

```bash
npm install
```

Validate the Vite project only:

```bash
npm run build:vite
```

Build the Vite app and package the Keycloak theme as JAR(s):

```bash
npm run build
```

> `keycloakify build` requires Apache Maven available in `PATH` because Keycloakify uses it to package provider JARs.
> On Windows, install Maven with Chocolatey, SDKMAN, Scoop, or manually from Apache Maven, then reopen the terminal:
>
> ```powershell
> choco install maven
> mvn -version
> ```

The Keycloakify output directory is configured as:

```text
../keycloakify-theme-build
```

After a successful build, use the generated JAR(s) from that directory as Keycloak providers, or keep mounting the static theme from `../themes/gatewise` during development.

## Current validation status

- `npm install`: OK. The project now uses the official npm package `keycloakify@11.15.3`.
- `npm run build:vite`: OK.
- `npm run build`: Vite build passes, then stops at `keycloakify build` if Maven is not installed with: `Apache Maven required`.

## Important configuration

- Theme name: `gatewise`
- Keycloakify package: `keycloakify@11.15.3` from npm
- Account theme: disabled with `accountThemeImplementation: "none"`
- Locales: `en,pt-BR`