# TasksAPI — Learning Project

## What this is

A simple task list REST API built with ASP.NET Core (.NET 10) and PostgreSQL. The purpose is to learn how web APIs work by building one from scratch — understanding controllers, routing, DTOs, dependency injection, services, repositories, Dapper, and middleware.

This project intentionally uses the same patterns as a larger production API (BPA-API) but stripped down to one entity (Task) so each concept can be learned in isolation.

## Tech stack

* **Framework:** ASP.NET Core Web API (.NET 10)
* **Database:** PostgreSQL 17 (installed via Homebrew on Mac)
* **ORM:** Dapper (lightweight — you write your own SQL)
* **DB Driver:** Npgsql (the PostgreSQL equivalent of Microsoft.Data.SqlClient)

## Database

PostgreSQL running locally. Connection string in `appsettings.json`:

```
Host=localhost;Database=tasksdemo;Username=YOUR_MAC_USERNAME;
```

Single table:

```sql
CREATE TABLE tasks (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    is_complete BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

## Project structure

Everything lives in one project (no layers) to keep things simple:

```
TasksAPI/
├── Controllers/
│   └── TasksController.cs       ← HTTP endpoints (routes + status codes)
├── DTOs/
│   └── TaskDtos.cs              ← What the client sends and receives (not the DB entity)
├── Models/
│   └── TaskItem.cs              ← The entity (maps 1:1 to the database table)
├── Repositories/
│   ├── ITaskRepository.cs       ← Interface (contract — "what" the repo can do)
│   └── TaskRepository.cs        ← Implementation (the "how" — actual SQL via Dapper)
├── Services/
│   ├── ITaskService.cs          ← Interface
│   └── TaskService.cs           ← Business logic, maps entities ↔ DTOs
├── Middleware/
│   └── RequestLoggingMiddleware.cs ← Runs on every request (before + after)
├── Program.cs                   ← Wires everything together (DI + middleware pipeline)
└── appsettings.json             ← Configuration (connection string, logging)
```

## How the request flows

Every HTTP request follows this exact path:

```
Client (Postman)
  → Middleware (logging, auth, etc.)
    → Controller (parses HTTP, calls service)
      → Service (business logic, maps DTOs ↔ entities)
        → Repository (SQL via Dapper)
          → PostgreSQL database
        ← Returns entity
      ← Returns DTO
    ← Returns HTTP response (status code + JSON)
  ← Middleware (can modify response)
Client receives JSON
```

## Key concepts to understand

### 1. Entity vs DTO

* **Entity** (`TaskItem`): mirrors the database table exactly. Never sent to the client.
* **DTO** (`CreateTaskDto`, `TaskResponseDto`): what the client sees. You control the shape. This decouples your API contract from your database schema — you can change one without breaking the other.

### 2. Repository pattern

* **Interface** (`ITaskRepository`): defines what operations exist (GetAll, Create, Delete, etc.) without saying how they work. This is a contract.
* **Implementation** (`TaskRepository`): the actual SQL queries via Dapper. If you swapped PostgreSQL for MySQL tomorrow, you'd only change this file.

### 3. Service layer

Sits between the controller and repository. Handles business logic and DTO ↔ entity mapping. The controller doesn't know about the database, and the repository doesn't know about DTOs.

### 4. Dependency injection (DI)

In `Program.cs`, you register services:

```csharp
builder.Services.AddScoped<ITaskRepository>(_ => new TaskRepository(connectionString));
builder.Services.AddScoped<ITaskService, TaskService>();
```

This tells ASP.NET: "When a class asks for `ITaskRepository` in its constructor, give it a `TaskRepository`." The classes never create their own dependencies — they receive them. This makes testing and swapping implementations easy.

**Lifetimes:**

* `AddScoped` = one instance per HTTP request (most common for DB work)
* `AddSingleton` = one instance for the entire app lifetime
* `AddTransient` = new instance every time it's requested

### 5. Middleware

Code that runs on every request, before and after the controller. Think of it as a pipeline:

```
Request → Middleware1 → Middleware2 → Controller → Middleware2 → Middleware1 → Response
```

Each middleware calls `await _next(context)` to pass control to the next one. Code before that call runs on the way in, code after runs on the way out.

### 6. Controllers and routing

* `[Route("api/[controller]")]` on the class → base URL is `/api/tasks`
* `[HttpGet]` → GET /api/tasks
* `[HttpGet("{id:int}")]` → GET /api/tasks/5
* `[HttpPost]` → POST /api/tasks
* `[FromBody]` → parse the request body as JSON into a DTO

### 7. HTTP status codes

* `Ok(data)` → 200 (here's your data)
* `Created(url, data)` → 201 (created something, here's where it lives)
* `NoContent()` → 204 (success, nothing to return)
* `BadRequest(error)` → 400 (you sent something wrong)
* `NotFound()` → 404 (doesn't exist)
* `Unauthorized()` → 401 (who are you?)

### 8. Dapper

A micro-ORM. You write raw SQL, Dapper maps the results to C# objects:

```csharp
var tasks = await db.QueryAsync<TaskItem>("SELECT * FROM tasks");
```

It maps column names to property names automatically (case-insensitive). Parameters use `@Name` syntax to prevent SQL injection.

## API endpoints

| Method | URL                        | Body                | Returns    | Description     |
| ------ | -------------------------- | ------------------- | ---------- | --------------- |
| GET    | `/api/tasks`             | —                  | 200 + list | Get all tasks   |
| GET    | `/api/tasks/{id}`        | —                  | 200 or 404 | Get one task    |
| POST   | `/api/tasks`             | `{"title":"..."}` | 201 + task | Create a task   |
| PUT    | `/api/tasks/{id}/toggle` | —                  | 204 or 404 | Toggle complete |
| DELETE | `/api/tasks/{id}`        | —                  | 204 or 404 | Delete a task   |

## Key differences from SQL Server (for reference)

| Concept          | SQL Server                   | PostgreSQL           |
| ---------------- | ---------------------------- | -------------------- |
| Auto-increment   | `INT IDENTITY(1,1)`        | `SERIAL`           |
| Boolean          | `BIT`                      | `BOOLEAN`          |
| Get inserted ID  | `OUTPUT INSERTED.Id`       | `RETURNING id`     |
| String type      | `NVARCHAR(200)`            | `VARCHAR(200)`     |
| Current time     | `GETUTCDATE()`             | `NOW()`            |
| DB driver (C#)   | `Microsoft.Data.SqlClient` | `Npgsql`           |
| Connection class | `SqlConnection`            | `NpgsqlConnection` |
| True/False       | `1`/`0`                  | `TRUE`/`FALSE`   |

## Build order (suggested learning path)

Build these one at a time, running `dotnet build` after each step:

1. **Models/TaskItem.cs** — understand how an entity maps to a table
2. **DTOs/TaskDtos.cs** — understand why we don't expose entities directly
3. **Repositories/ITaskRepository.cs** — define the contract
4. **Repositories/TaskRepository.cs** — implement with Dapper + PostgreSQL
5. **Services/ITaskService.cs** — define the service contract
6. **Services/TaskService.cs** — implement mapping + business logic
7. **Controllers/TasksController.cs** — HTTP endpoints + status codes
8. **Middleware/RequestLoggingMiddleware.cs** — understand the pipeline
9. **Program.cs** — wire everything together with DI
10. **Test in Postman** — see it all work end to end

## Exercises to deepen understanding

After building the base project, try these:

1. **Add a `DueDate` field** — requires DB migration, entity change, DTO change, and new query logic
2. **Add filtering** — `GET /api/tasks?complete=true` to filter by status
3. **Add an `UpdateTitle` endpoint** — `PUT /api/tasks/{id}` with a body
4. **Add error handling middleware** — catch exceptions globally and return clean JSON errors
5. **Add a second entity (Tags)** — many-to-many relationship with tasks, new controller, new repository
