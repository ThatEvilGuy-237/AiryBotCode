# AI Implementation Analysis: The `TalkToAiry` Feature

This document provides a comprehensive analysis of the `TalkToAiry` conversational AI feature within the Airy Bot. It details the architecture, core components, and the step-by-step workflow of how the bot processes and responds to user messages.

## 1. System Overview

The `TalkToAiry` feature allows users to have stateful, context-aware conversations with the bot in a Discord channel. When a user mentions the bot, it processes the message, retrieves relevant conversation history and user-specific information, sends this context to an external AI service (OpenAI), and then relays the AI's response back to the channel. The system is designed to maintain a memory of conversations within a channel and even stores summaries of user personalities to make interactions more personalized.

## 2. Core Components

The feature is implemented across the Domain, Application, and Infrastructure layers, following Clean Architecture principles.

### Domain Layer

These are the core business objects that model the conversational data.

-   **`ChatUser.cs`**: Represents a participant in a conversation.
    -   `Id`: The user's Discord ID.
    -   `UserName`: The user's display name.
    -   `Role`: An enum (`System`, `User`, `Assistant`, `Owner`) defining the user's role in the conversation. This is crucial for structuring the AI prompt.
    -   `AiOpinion`: A string field used to store a long-term, AI-generated summary of the user's personality and behavior.

-   **`Message.cs`**: Represents a single message in a conversation.
    -   `UserId`: Foreign key linking to the `ChatUser` who sent the message.
    -   `Context`: The actual text content of the message.
    -   `ChannelConversationId`: Foreign key linking the message to a specific channel's conversation history.

-   **`ChannelConversation.cs`**: Represents the ongoing conversation within a specific Discord channel.
    -   `ChannelId`: The ID of the Discord channel.
    -   `ConversationSummary`: An AI-generated summary of the channel's conversation topics.
    -   `Messages`: A collection of all messages exchanged in that channel's conversation.

### Application Layer

This layer orchestrates the business logic and interaction between the domain and infrastructure.

-   **`TalkToAiry.cs`**: The central orchestrator for handling an incoming message. It uses other services to process the message, get an AI response, and save the conversation.

-   **`IConversationManagerService.cs` / `ConversationManagerService.cs`**: A high-level service responsible for managing the entire conversation context.
    -   `GetOrCreateConversationContextAsync`: Gathers all necessary data (users, channel info, message history) and constructs a detailed system prompt.
    -   `SaveMessagesAsync`: Persists the user's new message and the AI's response to the database.
    -   `GenerateAndSaveUserSummaryAsync`: Creates and stores the `AiOpinion` for a `ChatUser`.

-   **`OpenAIClient.cs`**: A dedicated client for communicating with the OpenAI API.
    -   `SendMessageAsync`: Takes a list of `Message` objects, formats them into the JSON structure required by the OpenAI API (including roles and names), sends the request, and returns the AI's text response.

-   **`ConversationContext.cs`**: A Data Transfer Object (DTO) used to pass all relevant conversational data between services. It holds the `ChannelConversation`, all relevant `ChatUser` objects (author, system, AI), the recent message history, and the dynamically generated system prompt.

### Infrastructure Layer

This layer handles external concerns, such as receiving events from Discord and interacting with the database.

-   **`TalkToAiryAction.cs`**: Acts as the bridge between a Discord event and the application logic. It implements `IMessageAction`, allowing it to be triggered when a new message is received in the server. Its primary role is to call the `TalkToAiry.ProcessMessageAsync` method.

## 3. Detailed Workflow

Here is the step-by-step process from a user sending a message to receiving a response:

1.  **Message Received**: A user sends a message in a Discord channel that mentions the bot (e.g., `@AiryBot Hello!`).

2.  **Event Handling**: The bot's `MessageSendHandler` detects the new message and, seeing that it's relevant to the AI, invokes the `HandleMessageReceivedAsync` method in `TalkToAiryAction.cs`.

3.  **Application Logic Triggered**: `TalkToAiryAction` calls `TalkToAiry.ProcessMessageAsync`, passing in the `SocketMessage` object from Discord.

4.  **Context Aggregation**: `TalkToAiry.ProcessMessageAsync` calls `_conversationManagerService.GetOrCreateConversationContextAsync`. This is a critical step where the service:
    a.  Retrieves or creates the `ChannelConversation` for the current channel from the database.
    b.  Retrieves or creates `ChatUser` entities for the message author, the "System" persona, and the "Assistant" (AI) persona.
    c.  Retrieves the recent message history for the channel.
    d.  Constructs a dynamic `SystemPrompt`. This prompt instructs the AI on its persona ("You are Airy...") and includes any available summaries of the channel's conversation (`ConversationSummary`) and long-term opinions on the users involved (`AiOpinion`).

5.  **Prompt Construction**: Back in `TalkToAiry`, a final list of messages is assembled for the AI. This list includes:
    a.  The dynamic system prompt.
    b.  The recent message history.
    c.  The new message from the user.

6.  **AI Communication**: The complete message list is passed to `_openAIClient.SendMessageAsync`.
    a.  The client transforms the list into a JSON array, correctly assigning `role` (`system`, `user`, `assistant`) and `name` to each message.
    b.  It sends this payload to the OpenAI `chat/completions` API endpoint.

7.  **AI Response**: The OpenAI service processes the context and generates a response, which is sent back to the `OpenAIClient`. The client parses the response and returns the content as a string.

8.  **Persistence**: `TalkToAiry` now has the AI's response. It calls `_conversationManagerService.SaveMessagesAsync`, which saves both the original user message and the AI's response message to the database, linking them to the current `ChannelConversation`.

9.  **Send to Discord**: Finally, `TalkToAiry` sends the AI's response text back to the original Discord channel for the user to see.