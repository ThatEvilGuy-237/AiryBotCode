# Project Overview: Airy Discord Bot

This project is a C# Discord bot named "Airy," designed with a clean architecture approach. It leverages Discord.Net for Discord API interactions, Entity Framework Core for database management (configured for PostgreSQL), and Microsoft's Dependency Injection for robust service and command handling. The bot is capable of managing various Discord interactions, including slash commands, button presses, and form submissions.

## Architecture and Key Components

The project is structured into the following layers, adhering to clean architecture principles:

*   **AiryBotCode.Domain**: Contains core domain entities (e.g., `ChatUser`, `Message`) and business logic.
*   **AiryBotCode.Application**: Houses application-specific commands (e.g., `TimeoutCommand`, `UserlogsCommand`) and services (e.g., `UserService`, `LogService`). This layer orchestrates interactions between the UI/Bot and the domain.
*   **AiryBotCode.Infrastructure**: Provides implementations for interfaces defined in the Domain and Application layers, including database persistence (Entity Framework Core), Discord event handlers (`SlashCommandHandler`, `ButtonPressHandler`), and external service integrations.
*   **AiryBotCode.Bot**: The entry point of the application, responsible for bot startup, configuration loading, and dependency injection setup.

**Key Files and Their Roles:**

*   **`Program.cs` (Bot layer)**: The application's entry point, responsible for building the configuration, setting up the dependency injection container, and initiating the `AiryDevBot`.
*   **`appsettings.json` (Bot layer)**: Stores configuration settings for the database (PostgreSQL connection details), OpenAI API key, and various bot-specific parameters such as the Discord token, admin role IDs, and log channel IDs.
*   **`BotConfiguration.cs` (Infrastructure layer)**: Defines the structure for bot-related settings, facilitating strong-typed access to configuration values.
*   **`ServiceRegistration.cs` (Bot layer)**: Manages the registration of core bot services and integrates registrations from the Infrastructure layer.
*   **`RegisterApplication.cs` (Application layer)**: Registers application-specific commands and services into the dependency injection container. Commands are typically registered with a `Scoped` lifetime.
*   **`RegisterInfrastructure.cs` (Infrastructure layer)**: Registers infrastructure services, including the `AIDbContext` (for database access), various Discord event handlers, and repositories. It also integrates application service registrations.
*   **`UserlogsCommand.cs` (Application layer)**: An example of a slash command implementation. It defines the command's metadata (name, description, options) and encapsulates the logic for handling slash command interactions, button presses, and form submissions related to user logging.
*   **`SlashCommandHandler.cs` (Infrastructure layer)**: Responsible for dynamically registering slash commands with Discord and dispatching incoming slash command interactions to the appropriate `ISlashAction` implementations.
*   **`ChatUser.cs` (Domain layer)**: A core domain entity representing a chat user within the bot's context, including properties like `Id`, `UserName`, `Role`, and `Messages`.

## Building and Running

This project is a standard .NET 8.0 application.

*   **Build the project:**
    ```bash
    dotnet build
    ```
*   **Run the bot (from the root directory):**
    ```bash
    dotnet run --project AiryBotCode.Bot
    ```
*   **Docker:** The presence of `Dockerfile` and `docker-compose.yml` indicates support for containerized deployment using Docker.

## Development Conventions

*   **Clean Architecture:** The project strictly adheres to clean architecture principles, separating concerns into distinct layers (Domain, Application, Infrastructure, Bot).
*   **Dependency Injection:** Microsoft's Dependency Injection framework is extensively used for managing the lifecycle and dependencies of services and commands throughout the application.
*   **Command/Action Logic Encapsulation:** As per the `README.md`, each command or action (e.g., `UserlogsCommand`) is designed to contain the complete logic for its specific interaction, promoting maintainability and clarity.
*   **Event Handling:** Discord events are processed through dedicated event classes (e.g., `UserlogsEvent`) that implement specific interfaces (`ISlashEvent`, `IButtonEvent`, `IFormEvent`). These events are then registered within their respective handlers and the `RegisterInfrastructure.cs` file.
*   **Configuration Management:** Application settings are managed via `appsettings.json` and can be overridden by environment variables, providing flexibility for different deployment environments.
