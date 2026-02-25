# Real-Time Financial Monitor (MVP)

## Overview

This project implements a Real-Time Financial Monitoring system.

The system receives transaction data via HTTP, processes it, stores it persistently, and instantly broadcasts updates to connected clients through WebSockets.

The goal of this MVP is to demonstrate:

- Clean architecture
- Concurrency-safe design
- Real-time communication
- Responsive UI under burst load
- Testable and maintainable backend logic
- Cloud-ready thinking

---

## Project Structure

```
/FinancialMonitor          -> Backend (.NET 8, Minimal API, SignalR)
/financial-monitor-client  -> Frontend (React + TypeScript)
/FinancialMonitor.Tests    -> Unit Tests (xUnit + Moq)
/README.md
```

---

## Architecture

### Backend

- ASP.NET Core 8 Minimal API
- Entity Framework Core
- SQLite (persistent storage)
- SignalR for real-time updates
- Clean separation: Routes / Services / Data / Modules / DTO

Responsibilities:

- Accept transaction ingestion via HTTP POST
- Store transactions safely
- Broadcast updates to all connected clients
- Handle concurrency safely
- Provide retrieval endpoint for dashboard reload

---

### Frontend

- React + TypeScript
- Redux Toolkit for state management
- SignalR client for real-time communication

Two distinct routes:

- `/add` → Transaction Simulator
- `/monitor` → Live Dashboard

The UI updates in real-time without refresh.

---

## Data Model

Each transaction follows this structure:

```json
{
  "id": "guid",
  "amount": 100,
  "currency": "USD",
  "status": "Pending | Completed | Failed",
  "createdAt": "UTC timestamp"
}
```

Status is assigned server-side.

---

## Data Flow

1. Client sends POST `/transactions`
2. Backend:
   - Validates input
   - Assigns status
   - Stores in SQLite
   - Commits transaction
   - Broadcasts via SignalR
3. Connected clients receive the update instantly
4. Dashboard updates without reload
5. On page refresh, dashboard fetches persisted transactions from DB

---

## Concurrency & Thread Safety

The system safely handles concurrent ingestion and multiple WebSocket connections:

- No shared static collections
- EF Core ensures transactional integrity
- Explicit database transaction used in service layer
- SignalR broadcasting is thread-safe
- Concurrency covered by unit tests (parallel task execution)

Race conditions are avoided during reads and writes.

---

## Performance Considerations

The UI remains responsive even under burst load (100+ rapid transactions).

Techniques applied:

- Lightweight state updates
- No heavy computations inside render
- DOM growth bounded to 1000 latest transactions
- Stress-tested with 100 concurrent POST calls
- Verified using browser performance tools

The dashboard behaves as a real-time stream window, not an unbounded historical grid.

---

## Unit Testing

The backend is fully unit-tested using:

- xUnit
- EF Core InMemory provider
- Moq (for SignalR mocking)

Covered scenarios:

- Transaction persistence
- Status assignment validity
- Real-time broadcast verification
- Concurrent ingestion safety
- Ordering by CreatedAt

Run tests with:

```
dotnet test
```

All tests are isolated and independent.

---

## Why SQLite?

SQLite was selected for:

- Simplicity
- Zero infrastructure setup
- Persistent storage
- Easy transition to production-grade database

The architecture allows switching to PostgreSQL / SQL Server without structural changes.

---

## Cloud-Ready Architecture

If deployed to multiple replicas (pods), WebSocket broadcasting must be synchronized.

Problem:

Clients connected to Pod A would not receive events from Pod B.

Production-ready solution:

- SignalR Redis Backplane
- Distributed message broker (Kafka / RabbitMQ)
- Shared distributed cache layer

The current architecture supports this extension without major refactoring.

---

## How to Run

### Backend

```
dotnet run
```

Swagger is available in Development mode.

---

### Frontend

```
npm install
npm run dev
```

---

## Design Principles Applied

- Separation of Concerns
- Dependency Injection
- Single Responsibility
- Thread-safe design
- Testability
- Cloud-aware architecture
- Minimal but extensible structure

---

## Summary

This MVP demonstrates:

- Real-time data ingestion and broadcasting
- Concurrency-safe backend logic
- Responsive frontend under burst traffic
- Clean architecture
- Production-oriented thinking

The system is designed to be scalable, testable, and cloud-ready while remaining lightweight and focused.