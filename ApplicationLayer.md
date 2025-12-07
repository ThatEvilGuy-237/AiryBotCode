# Folder Structure: Application Layer

This document outlines the proposed folder structure for the `AiryBotCode.Application` layer. The goal is to improve readability, maintainability, and scalability by organizing files based on their features and responsibilities.

## Proposed Structure

```
AiryBotCode.Application/
├── Commands/
│   ├── Conversational/
│   └── Slash/
├── DTOs/
├── Features/
│   ├── Giveaway/
│   ├── Logging/
│   ├── Moderation/
│   ├── Reminders/
│   └── Verification/
├── Interfaces/
│   ├── ICommand.cs
│   ├── IService.cs
│   └── ...
├── Services/
│   ├── AIService/
│   ├── ButtonEncryptionService.cs
│   ├── DiscordService.cs
│   └── ...
├── ConfigurationReader.cs
├── IConfigurationReader.cs
└── RegisterApplication.cs
```

## Key Changes and Rationale

1.  **`Commands` Folder**:
    *   The folder `Comands` has been renamed to `Commands` to fix the typo.
    *   This folder will now primarily contain command-related interfaces and base classes, rather than concrete command implementations.

2.  **`Features` Folder**:
    *   A new `Features` folder is introduced to group all the logic related to a specific feature. This is a move towards a more vertical slice architecture.
    *   Each feature folder (e.g., `Moderation`, `Logging`) will contain the commands, handlers, and any other related logic for that feature.
    *   For example, the `Moderation` folder might contain `TimeoutCommand.cs`, `BanCommand.cs`, etc.

3.  **`Services` Folder**:
    *   The `Services` folder will contain services that are shared across different features, such as `DiscordService.cs` or `ButtonEncryptionService.cs`.

4.  **`Interfaces` Folder**:
    *   The `Interfaces` folder is kept for application-level interfaces.

This new structure will make it easier to find and work with related files, and it will provide a clear organization for adding new features in the future.
