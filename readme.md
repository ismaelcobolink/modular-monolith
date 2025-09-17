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

## Resumen

-   El módulo de eventos es independiente y desacoplado.
-   La comunicación entre capas se realiza mediante MediatR.
-   La persistencia se gestiona con Entity Framework Core.
-   Los endpoints son minimalistas y fáciles de mantener.

## Como iniciar el proyecto
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
    docker-compose 
    ```
6. Url de la API: https://localhost:5001
6.1. Si usais Swagger: https://localhost:5001/swagger/index.html
