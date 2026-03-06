# Demandas API

A REST API for managing legal cases (demandas), built as part of a hands-on learning journey from Power Apps to full-stack web development.

### The learning path

| #  | Project                                                              | Status         |
| -- | -------------------------------------------------------------------- | -------------- |
| 1  | **Contactos CLI** — C# fundamentals via a CLI contact manager | ✅ Done        |
| 2  | **Demandas API** — ASP.NET Core Web API (this repo)           | 🔨 In progress |
| 3  | Demandas DB — Entity Framework Core + Dapper with SQL Server        | ⬜             |
| 4  | Mi Primera Página — HTML/CSS/JS/React/TypeScript                   | ⬜             |
| 5  | Full Stack Demandas — Connect React to the API                      | ⬜             |
| 6  | Autenticación Microsoft — Entra ID SSO                             | ⬜             |
| 7  | Visor de Documentos — NAS file serving + SharePoint via Graph API   | ⬜             |
| 8  | Correos y Trabajos — Hangfire background jobs + email automation    | ⬜             |
| 9  | Acuerdos de Pago — Complex feature end-to-end                       | ⬜             |
| 10 | Despliegue — Production deployment with IIS/nginx + SSL + CI/CD     | ⬜             |

## What this project covers

- ASP.NET Core Web API with controller-based routing
- RESTful endpoint design (GET, POST, PUT, PATCH, DELETE)
- DTOs for separating API contracts from internal models
- Filtering, search, and pagination via query parameters
- Swagger/OpenAPI for interactive API documentation
- Proper HTTP status codes (200, 201, 204, 400, 404)

Data is stored in-memory for now — Project 3 connects this to SQL Server.

## Endpoints

| Method | Route                         | Description                                                                |
| ------ | ----------------------------- | -------------------------------------------------------------------------- |
| GET    | `/api/demandas`             | List all (supports `?estado=`, `?search=`, `?page=`, `?pageSize=`) |
| GET    | `/api/demandas/{id}`        | Get by ID                                                                  |
| POST   | `/api/demandas`             | Create a new demanda                                                       |
| PUT    | `/api/demandas/{id}`        | Full update                                                                |
| PATCH  | `/api/demandas/{id}/estado` | Update status only                                                         |
| DELETE | `/api/demandas/{id}`        | Delete                                                                     |

## Running locally

**Prerequisites:** [.NET 9 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/YOUR_USERNAME/demandas-api.git
cd demandas-api/DemandasApi
dotnet run
```

Open `http://localhost:5xxx/swagger` (check terminal output for the exact port).

### Example request

```bash
curl -X POST http://localhost:5xxx/api/demandas \
  -H "Content-Type: application/json" \
  -d '{
    "expediente": "2024-001",
    "juzgado": "Juzgado de Primera Instancia nº3 de Madrid",
    "estado": "Activo",
    "demandante": "Banco Nacional S.A.",
    "demandado": "Juan García López",
    "responsable": "María Fernández",
    "importeReclamado": 15000.50
  }'
```
