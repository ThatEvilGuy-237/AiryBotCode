# Airy Discord Bot

<div align="center">
  <img src="https://via.placeholder.com/150" alt="Airy Bot Logo" width="150"/>
</div>

<p align="center">
  <strong>A modern, extensible Discord bot built with C# and Clean Architecture.</strong>
  <br />
  <br />
</p>

---

## About The Project

Airy is a custom-built Discord bot developed in C# using the Discord.Net library. It was created to provide a robust and maintainable alternative to common bot platforms. The project is designed with a clean architecture approach, ensuring a clear separation of concerns, which makes it easy to extend and contribute to.

---

## Features

-   **User Moderation**: Timeout and un-timeout users.
-   **Logging**: Create and manage detailed, editable user logs.
-   **Reminders**: Set reminders for users.
-   **AI Integration**: Features conversational capabilities through AI services.
-   **Extensible Command System**: Easily add new slash commands, button interactions, and forms.

---

## Architecture

This project follows the principles of **Clean Architecture** to create a scalable and maintainable application. The codebase is divided into four main layers:

-   **`AiryBotCode.Domain`**: Contains the core business logic, entities (e.g., `ChatUser`, `Message`), and domain-level interfaces. It has no dependencies on other layers.
-   **`AiryBotCode.Application`**: Orchestrates the application's use cases and commands (e.g., `UserlogsCommand`). It depends on the Domain layer but not on the Infrastructure or Bot layers.
-   **`AiryBotCode.Infrastructure`**: Contains implementations for external concerns, such as database access (Entity Framework Core), Discord API interactions, and event handlers (`SlashCommandHandler`, `ButtonPressHandler`).
-   **`Bots`**: The presentation layer and entry point of the application. This layer contains the executable bot projects (e.g., `AiryBotCode.AiryDevBot`) and is responsible for dependency injection, configuration, and starting the bot.

---

## Project Structure

Here is a high-level overview of the project's folder structure:

```
AiryBotCode/
├── Bots/
│   ├── AiryBotCode.AiryBot/         # Main bot executable
│   └── AiryBotCode.AiryDevBot/      # Development bot executable
├── AiryBotCode.Application/         # Application logic, commands, services
├── AiryBotCode.Domain/              # Core domain entities and business logic
├── AiryBotCode.Infrastructure/      # Database, external services, event handlers
├── AiryBotCode.sln                  # Visual Studio Solution file
└── README.md
```

---

## Getting Started

Follow these steps to get a local copy up and running.

### Prerequisites

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Docker](https://www.docker.com/products/docker-desktop/) (for running a PostgreSQL database)
-   A Discord Bot Token ([how to create one](https://discord.com/developers/docs/topics/oauth2#bots))

### Installation & Configuration

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/your-repository/AiryBotCode.git
    cd AiryBotCode
    ```

2.  **Set up the database:**
    The project is configured to use a PostgreSQL database. You can easily start one using the provided `docker-compose.yml` file:
    ```sh
    docker-compose up -d
    ```

3.  **Configure the bot:**
    Navigate to the `Bots/AiryBotCode.AiryDevBot` directory and create an `appsettings.json` file. You can copy the example file to get started:
    ```sh
    cp appsettings.example.json appsettings.json
    ```
    Now, edit `appsettings.json` with your specific credentials and settings.

    **Example `appsettings.json`:**
    ```json
    {
      "Database": {
        "Host": "localhost",
        "Port": 5432,
        "Name": "airy_db",
        "User": "airy_user",
        "Password": "your_password_here"
      },
      "OpenAI": {
        "ApiKey": "YOUR_OPENAI_API_KEY"
      },
      "Bots": {
        "Name": "DevAiryBot",
        "Enabled": true,
        "Token": "YOUR_DISCORD_BOT_TOKEN",
        "AdminRoleIds": [ "YOUR_ADMIN_ROLE_ID" ],
        "LogChannelId": "YOUR_LOG_CHANNEL_ID",
        "BotId": "YOUR_BOT_ID"
      }
    }
    ```
    **Note:** It is highly recommended to use environment variables or .NET user secrets for sensitive data in production environments instead of hardcoding them in `appsettings.json`.

### Building and Running

You can run the bot directly from the root directory of the project.

1.  **Build the solution:**
    ```sh
    dotnet build
    ```

2.  **Run the development bot:**
    ```sh
    dotnet run --project Bots/AiryBotCode.AiryDevBot
    ```

---

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

When adding a new feature (like a command), please adhere to the existing architectural patterns.

### Adding a New Command

1.  **Create a Command Class**: In the `AiryBotCode.Application` layer (under `Comands/SlashCommands`), create a new class for your command (e.g., `MyNewCommand.cs`). This class should contain all the logic for the command's lifecycle, from the initial slash command to any subsequent button presses or form submissions.
2.  **Implement the Action**: In `AiryBotCode.Infrastructure` (under `Activitys`), create an `Action` class (e.g., `MyNewAction.cs`) that implements the required interfaces (`ISlashAction`, `IButtonAction`, etc.).
3.  **Register Dependencies**:
    -   Register your new command and any related services in `RegisterApplication.cs`.
    -   Register your action/event handler in `RegisterInfrastructure.cs`.
4.  **Test**: Run the bot and test your new command thoroughly.

This structure keeps related logic encapsulated and ensures the project remains organized and maintainable.