# Phase 1: Analysis and Plan

This document outlines the current state of the AI conversational feature and the proposed plan for its enhancement.

## Current Implementation Analysis

The conversational feature is primarily handled by `TalkToAiry.cs` and `OpenAIClient.cs`.

**Workflow:**
1.  A user triggers an interaction that calls the `TalkToAiry.Conversation` method.
2.  `TalkToAiry` holds a detailed, hard-coded **system prompt** that defines the AI's personality ("Airy").
3.  It maintains a conversation history in a temporary, in-memory `List<Message>`. This list is re-created for each interaction and is not persisted.
4.  A new `OpenAIClient` is instantiated for every single message.
5.  The `OpenAIClient.BuildPrompt` method concatenates the entire conversation history (including the system prompt) into a **single large block of text**.
6.  This single text block is sent to the OpenAI API as a "system" message. This is a legacy approach that underutilizes the capabilities of modern chat models.
7.  The AI's response is received and sent back to the Discord channel.

**Key Observations:**
- **Stateless:** The system has no long-term memory. It does not use the database entities (`ChatUser`, `Message`, `ChannelConversation`) that have been prepared.
- **Inefficient API Usage:** The entire conversation history is flattened into one string and sent as a single system prompt. The standard and more effective method is to send a structured list of messages, each with a designated role (`system`, `user`, `assistant`).
- **`AiOpinion` Field:** The `ChatUser` entity already contains an `AiOpinion` string property, which is perfect for the user summary feature.

## Proposed Enhancement Plan

We will implement the features in phases to incrementally build towards a stateful, intelligent AI.

### Task 1: Refactor `OpenAIClient` for Modern API Standards
- **Goal:** Improve the quality of AI responses and adhere to OpenAI best practices.
- **Action:** Modify `OpenAIClient.SendMessageAsync` to send a JSON array of message objects, each with a `role` (`system`, `user`, `assistant`) and `content`, instead of a single flattened string.

### Task 2: Implement Database Persistence
- **Goal:** Give the AI memory of conversations.
- **Action:**
    1.  Inject database repositories (`IChatUserRepository`, `IMessageRepository`, etc.) into `TalkToAiry` using dependency injection.
    2.  When a conversation occurs:
        a.  Find or create the `ChatUser` in the database.
        b.  Find or create the `ChannelConversation` for the current channel.
        c.  **Save** the user's message to the database.
        d.  **Retrieve** recent message history from the database to build the prompt context.
        e.  After getting the AI's reply, **save** its message to the database.

### Task 3: Implement the "AI Opinion" (User Summary) Feature
- **Goal:** Create long-term, persistent "memory" about a user.
- **Action:**
    1.  Create a new slash command (e.g., `/summarize-user <user>`).
    2.  This command will retrieve the message history of the specified user.
    3.  It will call the `OpenAIClient` with a specific prompt to generate a concise summary of the user's personality, interests, and tone.
    4.  The generated summary will be saved to the `AiOpinion` field in the corresponding `ChatUser` entity.

### Task 4: Utilize the "AI Opinion" in Conversations
- **Goal:** Make conversations context-aware and personalized.
- **Action:**
    1.  In `TalkToAiry`, when preparing the prompt for the AI, check if the `ChatUser` has a non-empty `AiOpinion`.
    2.  If it exists, prepend it to the main system prompt. For example: *"You are talking to {UserName}. Here is a brief summary of them: {AiOpinion}. Keep this in mind during your conversation."*

We will proceed with **Task 1** first.
