# Project Architecture & Technology Specification

This document details the finalized architecture and technology stack for the Keycloak-integrated web application, incorporating best practices and refinements over the initial requirements.

## 1. Backend Architecture

### Core Technologies
- **ASP.NET Core**: Enterprise-level API framework providing high performance.
- **PostgreSQL & PgAdmin4**: Robust, open-source relational database. PgAdmin4 used for local management.
- **Keycloak**: Industry-standard OpenID Connect and OAuth 2.0 identity provider for Authentication & Authorization.

### Architectural Patterns
- **Onion Architecture**: Ensures core domain logic is isolated and independent of infrastructure, UI, or databases.
- **CQRS (Command Query Responsibility Segregation) & MediatR**: Clean separation of read and write operations.
  - **Commands (Write)**: Handled via MediatR. Uses a standard Repository pattern to fetch and save Domain Entities.
  - **Queries (Read)**: Handled via MediatR. Bypasses Repositories entirely. Dapper queries directly map database rows to flat Read Models (DTOs) for maximum performance.
- **Dapper**: Lightweight micro-ORM used across the application.
  - *Note*: We explicitly avoid a rigid "Unit of Work" and "Generic Repository" over Dapper, as Dapper does not feature change tracking. Instead, `IDbTransaction` management is handled within MediatR Pipeline Behaviors for commands that require transactions.
- **Fluent Dependency Injection**: DI registrations are encapsulated in extension methods per layer (e.g., `AddInfrastructure`, `AddApplication`) for clean configuration.
- **CORS Configuration**: The `.NET` API explicitly configures Cross-Origin Resource Sharing (CORS) to safely accept frontend requests (e.g., from `http://localhost:3000`) and prevent preflight errors.
- **Clean Boilerplate**: No default weather forecast controllers or unnecessary boilerplate code.

## 2. Frontend Architecture

### Core Technologies
- **React & Next.js**: Modern web application framework leveraging the App Router and React Server Components for native data fetching, server-side rendering, and built-in caching (`fetch` API), avoiding heavy client-side state libraries.
- **Auth.js (formerly NextAuth.js)**: Integrated with the Keycloak provider to securely handle OIDC flows, manage sessions, and automatically refresh tokens via HttpOnly cookies. It also robustly extracts Keycloak `realm_access.roles` by securely decoding the JWT `access_token` within the NextAuth callback.
- **Global Navigation System**: A persistent `NavBar` component injected into the Next.js Layout dynamically renders secure application routes (`/products`, `/users`) depending strictly on the user's active session roles.
- **Federated Logout**: Logouts are explicitly federated back to Keycloak using `id_token_hint` parameters, guaranteeing that ending a Next.js session natively terminates the upstream Keycloak session.

### UI & Styling
- **Shadcn UI & Tailwind CSS**: Chosen over Material UI to achieve a highly customized, premium "modern UI" that avoids the default Google Material aesthetic. Shadcn provides accessible, headless components that we can heavily tailor.
- **Responsive Design**: The UI is built mobile-first using Tailwind's responsive breakpoints (`sm:`, `md:`, `lg:`). Layouts adapt gracefully across devices (e.g., stacked buttons and single-column forms on mobile, side-by-side elements on larger screens).

## 3. Role-Based Access Control (RBAC)
- **Keycloak Role Definition**: `admin`, `moderator`, and `user` roles are defined at the realm level. New registrations automatically receive the `user` role.
- **Backend Mapping**: `.NET` extracts `realm_access` JSON payload from the Keycloak JWT during the `OnTokenValidated` event and seamlessly maps it to standard `ClaimTypes.Role`. This enables native `[Authorize(Roles="admin")]` functionality without heavy custom authorization handlers.
- **Frontend Mapping**: NextAuth extracts `profile.realm_access.roles` on login and attaches it to the Next.js `session` object.
- **Product States**: Products support an `IsListed` boolean state, which determines visibility or availability.
- **Granular Permissions**:
  - `Admin`: Can View, Create, Delete, and Toggle Product Status. Can view the Users dashboard and modify user roles.
  - `Moderator`: Can View and Create Products.
  - `User`: Can View Products only.
- **Role Management UI**: A dedicated `/users` dashboard allows `admin` users to securely assign and revoke Keycloak roles (e.g., granting someone `moderator` access) through a seamless UI. The `.NET` API dynamically resolves internal Keycloak Role UUIDs behind the scenes to execute these role mappings.

### Clean Code Practices
  - Extensive use of reusable components.
  - Arrow functions with implicit returns used where appropriate (primarily for presentational components without complex hooks).
  - No unnecessary boilerplate code.

## 3. Identity & User Registration

- **Custom Registration Flow**: Rather than using Keycloak's native login-screen registration, the application implements a fully custom registration flow across both the frontend and backend.
- **Frontend Registration**: A dedicated Next.js registration page using Shadcn UI captures user details and submits them to the `.NET` API.
- **Backend Integration**: The `.NET` API utilizes a Keycloak service account (`backend-client` with Client Credentials grant) to authenticate securely. It leverages the Keycloak Admin REST API to programmatically create the user and configure credentials.
- **Mandatory Email Verification**: New users are created with `emailVerified = false` and assigned the `VERIFY_EMAIL` required action. Upon creation, the `.NET` API instantly triggers Keycloak's `execute-actions-email` endpoint (including a `redirect_uri` back to the application), ensuring the verification email is immediately dispatched via Keycloak's SMTP transport.

## 4. Infrastructure & Development Workflow

- **Docker Containerization**: The local development environment uses Docker to orchestrate PostgreSQL, PgAdmin4, and Keycloak.
- **Local SMTP Testing**: A local **Mailpit** Docker container is strictly bridged into the Keycloak network. Keycloak's SMTP settings are mapped to Mailpit, safely catching all outgoing verification and identity emails so developers can preview them natively without configuring external SMTP providers.
