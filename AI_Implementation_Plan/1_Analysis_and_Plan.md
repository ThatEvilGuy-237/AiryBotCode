# Phase 1: Analysis and Plan

This document outlines the current state of the AI conversational feature and the proposed plan for its enhancement. We have successfully refactored the architecture to align with Clean Architecture principles, moving orchestration logic to the Application layer and modernizing the OpenAI API interaction.

## Current Implementation Analysis

The conversational feature is now handled by `TalkToAiry.cs` (Application Layer) which orchestrates the flow using `IConversationManagerService` (Application Layer) for database interactions and `OpenAIClient.cs` (Application Layer) for AI communication.

**Key Improvements:**
- **Stateful:** The system now uses the database entities (`ChatUser`, `Message`, `ChannelConversation`) to store and retrieve conversation history, giving the AI memory.
- **Efficient API Usage:** `OpenAIClient` now sends structured messages to the OpenAI API, adhering to best practices for conversational models.
- **Clean Architecture:** Orchestration logic has been moved to the Application layer, improving separation of concerns.
- **`AiOpinion` Field:** The `ChatUser` entity still contains an `AiOpinion` string property, which is perfect for the user summary feature.

## Proposed Enhancement Plan

We will implement the features in phases to incrementally build towards a stateful, intelligent AI.

### Task 1: Refactor `OpenAIClient` for Modern API Standards - **COMPLETED**
- **Goal:** Improve the quality of AI responses and adhere to OpenAI best practices.
- **Action:** Modified `OpenAIClient.SendMessageAsync` to send a JSON array of message objects, each with a `role` (`system`, `user`, `assistant`) and `content`, instead of a single flattened string.

**Expected JSON Structure for OpenAI API `messages` parameter:**
```json
[
  {
    "role": "system",
    "content": "You are a helpful assistant."
  },
  {
    "role": "user",
    "content": "Who won the world series in 2020?"
  },
  {
    "role": "assistant",
    "content": "The Los Angeles Dodgers won the World Series in 2020."
  },
  {
    "role": "user",
    "content": "Where was it played?"
  }
]
```

### Task 2: Implement Database Persistence - **COMPLETED**
- **Goal:** Give the AI memory of conversations.
- **Action:**
    1.  Introduced `IConversationManagerService` and `ConversationManagerService` in the Application layer.
    2.  `TalkToAiry.ProcessMessageAsync` now uses `IConversationManagerService` to:
        a.  Find or create the `ChatUser` in the database.
        b.  Find or create the `ChannelConversation` for the current channel.
        c.  **Save** the user's message to the database.
        d.  **Retrieve** recent message history from the database to build the prompt context.
        e.  After getting the AI's reply, **save** its message to the database.
    3.  `TalkToAiryAction.cs` was simplified to a thin wrapper calling `TalkToAiry.ProcessMessageAsync`.

### Task 3: Implement the "AI Opinion" (User Summary) Feature - **COMPLETED**
- **Goal:** Create long-term, persistent "memory" about a user.
- **Action:**
    1.  Created `SummarizeUserCommand.cs` and `SummarizeUserAction.cs`.
    2.  Registered the new command and action.
    3.  Added `GenerateAndSaveUserSummaryAsync` to `IConversationManagerService` and its implementation in `ConversationManagerService.cs`.
    4.  Added `GetMessagesByUserIdAsync` to `IMessageRepository` and its implementation in `MessageRepository.cs`.
    5.  Updated `SummarizeUserCommand.cs` to use `IConversationManagerService` to generate and save the summary.

### Task 4: Utilize the "AI Opinion" in Conversations - **COMPLETED**
- **Goal:** Make conversations context-aware and personalized.
- **Action:**
    1.  Modified `TalkToAiry.ProcessMessageAsync` to check if the `ChatUser` has a non-empty `AiOpinion`.
    2.  If it exists, the `AiOpinion` is now prepended to the system prompt sent to the AI, making conversations more personalized.

## Architectural Refinement: Splitting ConversationManagerService - **COMPLETED**
- **Motivation:** To adhere more closely to the Single Responsibility Principle (SRP) and improve maintainability, reusability, and separation of concerns.
- **Action:** The monolithic `ConversationManagerService` was refactored into three specialized services:
    1.  **`IChannelConversationService` / `ChannelConversationService`:** Responsible for managing `ChannelConversation` entities (getting or creating conversations).
    2.  **`IChatUserService` / `ChatUserService`:** Responsible for managing `ChatUser` entities (getting or creating users, and updating their `AiOpinion`).
    3.  **`IMessageService` / `MessageService`:** Responsible for managing `Message` entities (retrieving history, managing history size, saving messages, and retrieving messages by user ID).
- **Outcome:** `ConversationManagerService` now acts as an orchestrator, injecting and utilizing these three new services to perform its tasks, rather than directly interacting with repositories. This significantly cleans up the service layer and enhances modularity.

## Repository Layer Refinement - **COMPLETED**
- **Motivation:** To make the repositories more expressive and efficient for the specific needs of the AI interaction features.
- **Actions:**
    1.  **`IMessageRepository`:**
        - Added `GetAndPruneConversationHistoryAsync` to centralize the logic for retrieving conversation history and managing its size in a single, efficient database operation.
        - Added `GetUserMessageHistoryForSummaryAsync` to retrieve only the text content of a user's recent messages, reducing the amount of data pulled from the database for generating AI summaries.
    2.  **File Structure:** Corrected the location of `IChannelConversationService.cs` and updated all `using` statements across the project to reflect the new, more organized `Interfaces/Repository` and `Interfaces/Service` folder structure.
- **Outcome:** The service layer now relies on more specialized and efficient repository methods, leading to cleaner service code and better data access patterns.

We will proceed with **Task 3** next.
