# Evently Modular Monolith

## 📋 Resumen Ejecutivo

**Evently** es una aplicación de gestión de eventos construida con .NET 9 que implementa una **arquitectura monolítica modular**. Este enfoque combina las ventajas de un monolito (simplicidad de despliegue, transacciones locales) con la modularidad de los microservicios (separación de dominios, mantenibilidad, escalabilidad futura).

### Características Principales
- **Framework**: .NET 9 con ASP.NET Core Minimal APIs
- **Base de Datos**: PostgreSQL con Entity Framework Core
- **Patrones**: CQRS, Domain-Driven Design (DDD), Clean Architecture
- **Mensajería**: MediatR para comunicación interna, MassTransit para eventos de integración
- **Logging**: Serilog con Seq
- **Containerización**: Docker y Docker Compose

## 🏗️ Arquitectura General

### Estructura Modular

El sistema está dividido en tres módulos principales de negocio:

```
src/
├── Modules/
│   ├── Events/        # Gestión de eventos
│   ├── Ticketing/     # Sistema de tickets
│   └── Users/         # Gestión de usuarios
├── Common/            # Código compartido
└── API/              # API Gateway / Host
```

### Capas por Módulo

Cada módulo sigue una arquitectura limpia (Clean Architecture) con 4 capas bien definidas:

```
Módulo/
├── Domain/           # Entidades, Value Objects, Domain Events
├── Application/      # Casos de uso, Commands, Queries, Handlers
├── Infrastructure/   # Implementaciones técnicas (DB, servicios externos)
└── Presentation/     # Endpoints HTTP, DTOs de entrada/salida
```

## 🔍 Análisis Detallado por Capa

### 1. **Capa de Dominio (Domain)**

La capa más interna que contiene la lógica de negocio pura:

- **Entidades**: Clases que representan conceptos del negocio (Event, Category, TicketType, User)
- **Value Objects**: Objetos inmutables sin identidad propia
- **Domain Events**: Eventos que ocurren en el dominio (EventPublished, TicketTypeCreated)
- **Interfaces de Repositorios**: Contratos para persistencia (IEventRepository, ICategoryRepository)
- **Errores de Dominio**: Definición de errores específicos del negocio

**Ejemplo de Entidad**:
```csharp
public sealed class Event : Entity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public EventStatus Status { get; private set; }
    
    public static Result<Event> Create(Category category, string title, ...)
    {
        // Lógica de validación y creación
    }
    
    public Result Publish()
    {
        // Lógica de negocio para publicar
    }
}
```

### 2. **Capa de Aplicación (Application)**

Orquesta los casos de uso del sistema:

- **Commands/Queries**: Implementación del patrón CQRS
  - Commands: CreateEventCommand, PublishEventCommand, CancelEventCommand
  - Queries: GetEventQuery, SearchEventsQuery
- **Handlers**: Procesadores de comandos y consultas usando MediatR
- **Validators**: Validación de entrada con FluentValidation
- **DTOs**: Objetos de transferencia entre capas
- **Behaviors**: Pipeline behaviors para cross-cutting concerns (logging, validación)

**Patrón CQRS implementado**:
```csharp
// Command
public record PublishEventCommand(Guid EventId) : ICommand;

// Handler
public class PublishEventCommandHandler : ICommandHandler<PublishEventCommand>
{
    public async Task<Result> Handle(PublishEventCommand command, CancellationToken ct)
    {
        // Lógica del caso de uso
    }
}
```

### 3. **Capa de Infraestructura (Infrastructure)**

Implementaciones técnicas y acceso a recursos externos:

- **DbContext**: Configuración de Entity Framework Core
- **Repositories**: Implementación concreta de los repositorios
- **Migrations**: Migraciones de base de datos
- **Configuración de Entidades**: Mapeo OR/M con Fluent API
- **Servicios de Integración**: Publicación de eventos con MassTransit
- **Interceptors**: Para auditoría y funcionalidades transversales

**Características técnicas**:
- Uso del patrón Outbox para garantizar consistencia en eventos
- Interceptores para publicación automática de domain events
- Configuración modular de DbContext por módulo

### 4. **Capa de Presentación (Presentation)**

Expone la funcionalidad a través de HTTP:

- **Endpoints**: Minimal APIs organizadas por feature
- **Request/Response DTOs**: Contratos de la API
- **Mapeo**: Conversión entre DTOs y comandos/queries
- **Validación**: Validación de entrada en endpoints

**Ejemplo de Endpoint**:
```csharp
app.MapPut("events/{id}/publish", async (Guid id, ISender sender) =>
{
    Result result = await sender.Send(new PublishEventCommand(id));
    return result.Match(Results.NoContent, ApiResults.Problem);
});
```

## 🔄 Flujo de Comunicación

### Flujo típico de una petición:

1. **Cliente HTTP** → Realiza petición a la API
2. **Minimal API Endpoint** → Recibe y valida la petición
3. **MediatR** → Enruta el comando/query al handler apropiado
4. **Application Handler** → Ejecuta la lógica del caso de uso
5. **Domain** → Aplica reglas de negocio
6. **Repository** → Persiste cambios si es necesario
7. **Domain Events** → Se publican eventos del dominio
8. **Response** → Se devuelve resultado al cliente

### Comunicación entre Módulos

Los módulos se comunican mediante:
- **Integration Events**: Eventos publicados a través de MassTransit
- **Shared Kernel**: Código común en la capa Common
- **No hay referencias directas entre módulos** para mantener el bajo acoplamiento

## 🛠️ Tecnologías y Patrones

### Stack Tecnológico
- **.NET 9**: Framework principal
- **PostgreSQL**: Base de datos relacional
- **Entity Framework Core**: ORM
- **MediatR**: Implementación del patrón Mediator
- **FluentValidation**: Validación de datos
- **MassTransit**: Bus de mensajes para eventos de integración
- **Serilog + Seq**: Logging estructurado
- **Docker**: Containerización

### Patrones Implementados
- **Domain-Driven Design (DDD)**: Modelado del dominio
- **CQRS**: Separación de comandos y consultas
- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Outbox Pattern**: Garantía de entrega de eventos
- **Result Pattern**: Manejo explícito de errores sin excepciones
- **Specification Pattern**: Para queries complejas

## 🚀 Ventajas de esta Arquitectura

1. **Modularidad**: Cada módulo es independiente y puede evolucionar por separado
2. **Escalabilidad**: Fácil transición a microservicios si es necesario
3. **Mantenibilidad**: Separación clara de responsabilidades
4. **Testabilidad**: Cada capa puede testearse de forma aislada
5. **Simplicidad operacional**: Un solo despliegue, una sola base de datos
6. **Transacciones ACID**: Al ser un monolito, mantiene transacciones locales
7. **Desarrollo rápido**: No hay overhead de comunicación entre servicios

## 📦 Estructura de Proyectos

```
Evently.sln
├── src/
│   ├── API/
│   │   └── Evently.Api                    # Host principal de la aplicación
│   ├── Common/
│   │   ├── Evently.Common.Domain          # Clases base del dominio
│   │   ├── Evently.Common.Application     # Utilidades de aplicación
│   │   ├── Evently.Common.Infrastructure  # Configuración base de infra
│   │   └── Evently.Common.Presentation    # Utilidades para endpoints
│   └── Modules/
│       ├── Events/
│       │   ├── *.Domain
│       │   ├── *.Application
│       │   ├── *.Infrastructure
│       │   └── *.Presentation
│       ├── Ticketing/
│       │   └── [misma estructura]
│       └── Users/
│           └── [misma estructura]
└── docker-compose.yml                      # Orquestación de contenedores
```

## 🔧 Configuración y Extensibilidad

### Configuración Modular
Cada módulo tiene su propia configuración:
- `modules.events.json`
- `modules.ticketing.json`
- `modules.users.json`

### Registro de Módulos
```csharp
builder.Services.AddEventsModule(configuration);
builder.Services.AddUsersModule(configuration);
builder.Services.AddTicketingModule(configuration);
```

### Pipeline de MediatR
Comportamientos configurables para:
- Logging de comandos/queries
- Validación automática
- Gestión de transacciones
- Manejo de errores

## 🐳 Infraestructura con Docker

El proyecto incluye configuración completa de Docker:

- **API**: Contenedor principal de la aplicación
- **PostgreSQL**: Base de datos
- **Seq**: Servidor de logs

## 📊 Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────┐
│                   Cliente HTTP                      │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│              API Gateway (Minimal APIs)             │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│                    MediatR                          │
└──────┬─────────────────┬─────────────────┬──────────┘
       │                 │                 │
┌──────▼───────┐  ┌──────▼───────┐  ┌──────▼───────┐
│  Events      │  │ Ticketing    │  │    Users     │
│  Module      │  │  Module      │  │    Module    │
├──────────────┤  ├──────────────┤  ├──────────────┤
│Presentation  │  │Presentation  │  │Presentation  │
├──────────────┤  ├──────────────┤  ├──────────────┤
│Application   │  │Application   │  │Application   │
├──────────────┤  ├──────────────┤  ├──────────────┤
│  Domain      │  │  Domain      │  │  Domain      │
├──────────────┤  ├──────────────┤  ├──────────────┤
│Infrastructure│  │Infrastructure│  │Infrastructure│
└─────┬────────┘  └─────┬────────┘  └─────┬────────┘
      │                 │                 │
┌─────▼─────────────────▼─────────────────▼┐
│            PostgreSQL Database           │
└──────────────────────────────────────────┘
```

## 🎯 Casos de Uso Principales

### Módulo de Eventos
- Crear evento
- Publicar evento
- Cancelar evento
- Reprogramar evento
- Buscar eventos
- Gestionar categorías
- Definir tipos de tickets

### Módulo de Ticketing
- Comprar tickets
- Validar disponibilidad
- Gestionar inventario
- Procesar pagos

### Módulo de Usuarios
- Registro de usuarios
- Autenticación
- Gestión de perfiles
- Autorización

## 🔐 Consideraciones de Seguridad

- Validación en múltiples capas
- Uso del patrón Result para evitar excepciones
- Sanitización de inputs con FluentValidation
- Logging estructurado para auditoría
- Segregación de responsabilidades por módulo

## 🚦 Como iniciar el proyecto

1. Instalar .NET 9 SDK desde https://dotnet.microsoft.com/en-us/download/dotnet/9.0
2. Instalar PostgreSQL desde https://www.postgresql.org/download/
3. Instalar Docker desde https://www.docker.com/get-started/
4. Clonar el repositorio del proyecto
5. Escribir en la terminal:

```bash
docker-compose up
```

5.1. Si falla la imagen usad los siguientes comandos:
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up
```

6. Url de la API: https://localhost:5001
6.1. Si usais Swagger: https://localhost:5001/swagger/index.html
