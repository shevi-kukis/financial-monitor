# Financial Monitor – Real-Time Transaction System

## Overview

This project implements a real-time transaction monitoring system.

The system simulates financial transactions, processes them asynchronously,
and updates all connected clients in real time using SignalR.

The UI remains responsive even under high event frequency.

---

## Architecture

### Backend
- .NET 8 Minimal API
- Entity Framework Core
- SQLite database
- SignalR for real-time updates
- Background processing for transaction lifecycle
- Dockerized API

### Frontend
- React 19
- TypeScript
- Redux Toolkit
- Vite
- SignalR client
- Client-side filtering
- Animated UI transitions
- Dockerized with Nginx

---

## Transaction Lifecycle

1. A transaction is created with status **Pending**
2. It is stored in SQLite
3. It is immediately broadcast to all clients
4. A background process simulates processing
5. After a delay, status changes to **Completed** or **Failed**
6. The update is broadcast again via SignalR

This demonstrates real-time state transitions.

---

## Performance Considerations

- UI uses CSS-based animations (GPU-friendly)
- No blocking operations in render
- Redux prevents duplicate transactions
- Transaction list capped at 1000 entries
- Background updates use a new DI scope to avoid DbContext concurrency issues

---

## Concurrency Handling

- Each background update creates a new service scope
- No shared DbContext across async tasks
- Updates are idempotent (client updates by ID, not append-only)
- Supports multiple concurrent clients

---

## Running with Docker

From the root folder:

```
docker compose up --build
```

Backend:
```
http://localhost:8080/swagger
```

Frontend:
```
http://localhost:5173
http://localhost:5174
```

---

## Development Mode

Backend:
```
dotnet run
```

Frontend:
```
npm run dev
```

---

## Health Endpoint

```
GET /health
```

---

## Possible Future Improvements

- Horizontal scaling with distributed SignalR backplane
- Persistent database container
- Authentication & authorization
- Integration tests
- Monitoring & structured logging

---

## Author Notes

This project focuses on:

- Clean architecture
- Real-time systems
- Concurrency safety
- Performance under burst load