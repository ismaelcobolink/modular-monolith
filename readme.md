# Arquitectura y Funcionamiento del Módulo de Eventos

## Arquitectura General

El módulo de eventos está diseñado siguiendo una arquitectura modular y en capas, basada en buenas prácticas de .NET:

-   **Domain:** Entidades y lógica de negocio (`Event`, `Category`, `TicketType`).
-   **Application:** Casos de uso, comandos y consultas (por ejemplo, `PublishEventCommand`).
-   **Infrastructure:** Implementaciones técnicas (repositorios, acceso a datos, servicios de infraestructura).
-   **Presentation:** Endpoints HTTP expuestos como Minimal APIs.

## Detalles Técnicos

-   **Framework:** .NET 9
-   **Persistencia:** Entity Framework Core con PostgreSQL
-   **Mediación:** MediatR para comandos y consultas
-   **Validación:** FluentValidation
-   **Inyección de dependencias:** `IServiceCollection`
-   **API:** ASP.NET Core Minimal APIs

## Flujo de Comunicación

1. **Exposición de Endpoints**

    - Los endpoints HTTP se definen en la capa de presentación.
    - Ejemplo: Publicar un evento
        ```csharp
        app.MapPut("events/{id}/publish", async (Guid id, ISender sender) =>
        {
            Result result = await sender.Send(new PublishEventCommand(id));
            return result.Match(Results.NoContent, ApiResults.ApiResults.Problem);
        });
        ```

2. **Mediación de Comandos**

    - Se utiliza MediatR para enviar comandos y consultas desacoplando la API de la lógica de negocio.

3. **Lógica de Negocio**

    - Los handlers de comandos procesan la lógica y utilizan los repositorios para acceder a los datos.

4. **Persistencia**

    - Los repositorios implementan el acceso a datos usando Entity Framework Core.

5. **Respuesta**
    - El resultado se transforma en una respuesta HTTP adecuada.

## Diagrama de Flujo

flowchart TD A[HTTP Request] --> B[Endpoint Presentation] B --> C[MediatR Command/Query] C --> D[Application Handler] D --> E[Domain/Infrastructure] E --> F[Base de datos]

## Resumen

-   El módulo de eventos es independiente y desacoplado.
-   La comunicación entre capas se realiza mediante MediatR.
-   La persistencia se gestiona con Entity Framework Core.
-   Los endpoints son minimalistas y fáciles de mantener.

¿Necesitas que detalle algún flujo específico o alguna parte del código?
