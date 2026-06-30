# 🔐 Keycloak Identity & OAuth 2.0 Web App

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![Next.js](https://img.shields.io/badge/Next.js-16-000000?style=for-the-badge&logo=next.js)
![Keycloak](https://img.shields.io/badge/Keycloak-24.0-0097CE?style=for-the-badge&logo=keycloak)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?style=for-the-badge&logo=postgresql)

Welcome to the **Keycloak Web App**! This repository is a comprehensive, production-ready full-stack application designed specifically to practice and demonstrate advanced **OpenID Connect (OIDC)** and **OAuth 2.0** protocols using Keycloak. 

By acting as a secure bridge between a modern React frontend and a highly-structured `.NET` API, this project demonstrates how to handle authentication, role-based access control (RBAC), custom identity flows, and secure token management.

---

## 🎯 Purpose of the Project

The primary goal of this application is to move beyond basic "Sign In with Keycloak" tutorials and implement a **real-world identity architecture**. Throughout this project, we've accomplished:
- 🛡️ **Custom Registration & Password Resets:** Built bespoke Next.js UI flows that communicate with the `.NET` API, which securely acts as a gatekeeper to execute highly-privileged Keycloak Admin REST API commands on the user's behalf.
- 🔑 **Token & Role Mapping:** Successfully mapped internal Keycloak `realm_access` roles to native `.NET` claims (`[Authorize(Roles="admin")]`) and NextAuth session properties for seamless RBAC across the entire stack.
- 🎨 **Native Theming:** Injected a beautiful, custom CSS/FreeMarker login theme directly into the Keycloak Docker container without relying on heavy third-party React compilers.
- ⚡ **Streamlined SSO:** Bypassed default NextAuth prompt screens to provide a seamless, one-click Single Sign-On (SSO) experience.
- 🚪 **Federated Logouts:** Implemented robust federated logouts utilizing the `id_token_hint`, ensuring that destroying the Next.js session natively terminates the upstream Keycloak session as well.
- ⚠️ **Resilient Error Handling:** Built resilient frontend API interceptors that detect `401 Unauthorized` responses (when a `.NET` token expires) and gracefully drop the user back into the Keycloak OAuth flow without breaking the UI.

---

## 🛠️ Technology Stack

### 💻 Frontend
- **Framework:** [Next.js 16](https://nextjs.org/) (App Router & React Server Components)
- **Authentication:** [Auth.js (NextAuth)](https://next-auth.js.org/) for OIDC flows and secure HttpOnly session cookie management.
- **UI & Styling:** [Tailwind CSS](https://tailwindcss.com/) and [Shadcn UI](https://ui.shadcn.com/) for a premium, accessible, headless component design.

### ⚙️ Backend
- **Framework:** [ASP.NET Core 10](https://dotnet.microsoft.com/) Web API.
- **Architecture:** Onion Architecture & CQRS (Command Query Responsibility Segregation).
- **Mediator Pattern:** [MediatR](https://github.com/jbogard/MediatR) for decoupling requests from their handlers.
- **Database Access:** [Dapper](https://github.com/DapperLib/Dapper) (Micro-ORM) for ultra-fast, raw SQL execution.
- **Identity Provider:** [Keycloak 24](https://www.keycloak.org/) managed locally via Docker.

---

## ✨ Architectural Highlights & Design Patterns

What makes this application shine isn't just the features, but *how* they are implemented under the hood:

- 🏗️ **CQRS & MediatR:** Write operations (Commands) and read operations (Queries) are strictly segregated. Queries bypass heavy repositories entirely, using Dapper to map directly to flat DTOs for maximum read performance.
- 🧩 **Fluent Dependency Injection:** Services are registered cleanly using the Fluent pattern (e.g., `services.AddInfrastructure()`, `services.AddApplication()`), keeping `Program.cs` exceptionally clean.
- 🚀 **Asynchronous Parallelization:** Resolved N+1 query bottlenecks when talking to Keycloak's API by utilizing `Task.WhenAll` to aggregate user roles concurrently.
- 🛡️ **The `KeycloakAuthHeaderHandler`:** A custom `.NET` `DelegatingHandler` that intercepts outgoing HTTP requests (like fetching a role from Keycloak) and automatically attaches the highly-privileged Admin `access_token` to the headers, utilizing `IMemoryCache` to prevent redundant token requests.
- 🔒 **Security First:** The frontend *never* holds the Keycloak Client Secret. The `.NET` API acts as a secure Policy Enforcement Point (PEP), ensuring users cannot enumerate emails during password resets or bypass role-assignment rules.
- ⚛️ **Modern React Hydration:** Fixed complex server/client hydration issues by adopting the `render` prop paradigm, ensuring headless component libraries (like Radix and BaseUI) compile cleanly in production Next.js App Router environments.

---

## 🚦 What the App Can & Cannot Do

### ✅ What it CAN do:
- Complete end-to-end user authentication via Keycloak.
- Custom user registration that triggers Keycloak's mandatory `VERIFY_EMAIL` workflow.
- Secure "Forgot Password" UI that safely queues a Keycloak reset email.
- Dashboard for `admin` users to assign and revoke roles (`moderator`, `admin`) to other users.
- Protect frontend routes and backend endpoints based strictly on decoded JWT Keycloak roles.

### ❌ What it CANNOT do:
- It is not an e-commerce platform. While there is a `/products` page used to demonstrate RBAC (Admins can view/edit, Users can only view), it does not include checkout, billing, or real inventory management.
- It does not store passwords. All credential management is strictly offloaded to Keycloak.

---

## 🚀 Getting Started

Follow these steps to get the application running on your local machine.

### 1. Prerequisites
- [Docker & Docker Compose](https://www.docker.com/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)

### 2. Spin up the Infrastructure
Start the PostgreSQL database, PgAdmin, and the Keycloak Server:
```bash
docker-compose up -d
```
*(Note: A local Mailpit container is also recommended for catching SMTP emails sent by Keycloak during registration/password resets).*

### 3. Configure Keycloak
1. Navigate to `http://localhost:8080` and log in with the admin credentials defined in your `docker-compose.yml` (`admin` / `admin`).
2. Create a new Realm (e.g., `webapp-realm`).
3. Create two Clients:
   - **Frontend Client** (Public): For Next.js to authenticate users.
   - **Backend Client** (Confidential): Enable "Service Accounts Roles" (Client Credentials grant) and assign it the `realm-management` roles necessary to query users and trigger emails.
4. Set up the `admin`, `moderator`, and `user` Realm Roles.

### 4. Run the .NET API
Open a terminal in the root directory and navigate to the API project:
```bash
cd KeycloakWebApp.Api
dotnet run
```
The API will start (usually on `http://localhost:5212`).

### 5. Run the Next.js Frontend
Open a new terminal and navigate to the frontend directory:
```bash
cd frontend
npm install
npm run dev
```
Navigate to `http://localhost:3000` in your browser. You're ready to go! 🎉
