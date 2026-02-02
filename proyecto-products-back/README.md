# Sistema de Gestión de Inventarios - Backend

API RESTful con .NET 8, Clean Architecture, CQRS y MongoDB.

## Arquitectura

```
src/
├── InventorySystem.Domain/         # Entidades e Interfaces
├── InventorySystem.Application/    # CQRS, DTOs, Validaciones
├── InventorySystem.Infrastructure/ # MongoDB, JWT, Servicios
└── InventorySystem.API/            # Controllers
```

## Stack

- .NET 8
- MongoDB
- MediatR + FluentValidation
- JWT + BCrypt
- QuestPDF
- xUnit + Moq

## Ejecución

```bash
cd src/InventorySystem.API
dotnet restore
dotnet run
```

## Docker

```bash
docker-compose up -d
```

API: `http://localhost:5000`

## Roles

- **Administrator**: Acceso completo
- **Employee**: Solo lectura + reportar stock bajo

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | /api/auth/register | Registro |
| POST | /api/auth/login | Login |
| GET | /api/products | Listar productos |
| POST | /api/products | Crear producto |
| GET | /api/reports/low-stock/pdf | Reporte PDF |
