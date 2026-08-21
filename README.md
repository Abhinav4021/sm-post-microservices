# 🚀 Social Media Post Microservices

### CQRS • Event Sourcing • Event-Driven Architecture • Apache Kafka • MongoDB • SQL Server

An event-driven Social Media Post backend built with **C#, ASP.NET Core, CQRS, Event Sourcing, Apache Kafka, MongoDB, SQL Server, Entity Framework Core, and the Mediator/Dispatcher pattern**.

The project demonstrates how to separate **write operations from read operations**, persist domain events as the source of truth, communicate asynchronously through Kafka, and maintain an independent SQL Server read model.

> 🚧 **Project Status:** Continuously evolving learning and reference implementation.
>
> The architecture and implementation are being improved incrementally with a focus on understanding distributed systems, CQRS, Event Sourcing, messaging, and microservice design.

---

## 📑 Table of Contents

- [🎯 Project Overview](#-project-overview)
- [🏗️ Architecture](#️-architecture)
- [🔄 End-to-End Flow](#-end-to-end-flow)
- [🧠 CQRS](#-cqrs)
- [📜 Event Sourcing](#-event-sourcing)
- [📡 Event-Driven Communication](#-event-driven-communication)
- [🎯 Command Side](#-command-side)
- [🔎 Query Side](#-query-side)
- [🧩 Mediator / Dispatcher Pattern](#-mediator--dispatcher-pattern)
- [📦 Domain Events](#-domain-events)
- [🗂️ Project Structure](#️-project-structure)
- [🛠️ Technology Stack](#️-technology-stack)
- [⚙️ Prerequisites](#️-prerequisites)
- [🐳 Infrastructure Setup](#-infrastructure-setup)
- [🔧 Configuration](#-configuration)
- [▶️ Running the Application](#️-running-the-application)
- [📘 Swagger](#-swagger)
- [📡 API Reference](#-api-reference)
- [⏱️ Eventual Consistency](#️-eventual-consistency)
- [🔐 Concurrency Handling](#-concurrency-handling)
- [⚠️ Failure Scenarios](#️-failure-scenarios)
- [🧠 Key Design Decisions](#-key-design-decisions)
- [🎨 Architecture Assets](#-architecture-assets)
- [🚧 Current Limitations](#-current-limitations)
- [🛣️ Roadmap](#️-roadmap)
- [🎓 Learning Objectives](#-learning-objectives)
- [🤝 Contributing](#-contributing)

---

# 🎯 Project Overview

This project implements a Social Media Post backend using an **event-driven microservices architecture**.

Instead of following a traditional CRUD architecture where reads and writes operate directly against the same database, the system separates responsibilities using **CQRS**.

The architecture consists primarily of:

| Component | Responsibility |
|---|---|
| `Post.Cmd` | Handles commands and write operations |
| `Post.Query` | Handles queries and read operations |
| `Post.Common` | Shared DTOs and domain event contracts |
| `CQRS.Core` | Reusable CQRS/Event Sourcing infrastructure |
| MongoDB | Event Store |
| Apache Kafka | Event/message transport |
| SQL Server | Query/read model |

At a high level:

```text
                    ┌─────────────────────┐
                    │       Client        │
                    └──────────┬──────────┘
                               │
                    ┌──────────┴──────────┐
                    │                     │
                 Commands              Queries
                    │                     │
                    ▼                     ▼
            ┌──────────────┐      ┌──────────────┐
            │  Post.Cmd    │      │  Post.Query  │
            │  WRITE SIDE  │      │   READ SIDE  │
            └──────┬───────┘      └──────▲───────┘
                   │                     │
                   ▼                     │
            ┌──────────────┐             │
            │ PostAggregate│             │
            └──────┬───────┘             │
                   │                     │
              Domain Events              │
                   │                     │
                   ▼                     │
            ┌──────────────┐             │
            │   MongoDB    │             │
            │ Event Store  │             │
            └──────┬───────┘             │
                   │                     │
                   ▼                     │
            ┌──────────────┐             │
            │    Kafka     │─────────────┘
            │ Event Broker │
            └──────────────┘
                   │
                   ▼
            ┌──────────────┐
            │ SQL Server   │
            │ Read Model   │
            └──────────────┘


## 🏗️ Architecture

<p align="center">
  <img src="Assets/Architecture%20Overview.png" alt="Architecture Overview" width="850" />
</p>

### Architectural Responsibilities

**Architecture Overview**

The project separates command and query responsibilities and connects the two sides asynchronously through Apache Kafka.

**Architecture Overview:**



**Architectural Responsibilities:**
**Command Side**
The command side is responsible for changing application state.

HTTP Request
     │
     ▼
Post.Cmd.Api
     │
     ▼
Command
     │
     ▼
Command Dispatcher
     │
     ▼
Command Handler
     │
     ▼
PostAggregate
     │
     ▼
Domain Event
     │
     ├──────────────► MongoDB Event Store
     │
     ▼
Kafka Producer

**Query Side**
The query side is responsible for retrieving data.

Kafka
  │
  ▼
Kafka Consumer
  │
  ▼
Event Handler
  │
  ▼
SQL Server Read Model
  │
  ▼
Query Repository
  │
  ▼
Query Handler
  │
  ▼
Query Dispatcher
  │
  ▼
Post.Query.Api

This separation is the foundation of the CQRS architecture.

**🔄 End-to-End Flow**
Let's follow a Create Post request through the entire system.
