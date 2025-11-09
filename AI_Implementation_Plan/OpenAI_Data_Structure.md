# OpenAI Data Structure for Enhanced Conversations

This document outlines the structured data format used when sending messages to the OpenAI Chat Completions API, focusing on how we enable the AI to better understand conversational context and speaker identity.

## Example JSON Structure

Here's an example of the `messages` array that will be sent in the API request body:

```json
{
  "model": "gpt-4o-mini",
  "messages": [
    {
      "role": "system",
      "content": "You are Airy, a helpful AI assistant in a multi-user Discord channel. Pay attention to the 'name' attribute on each user message to understand who is speaking. Address users by their name when it feels natural. Use Discord markdown format for your responses.

      Channel Summary: [Insert ChannelConversation.ConversationSummary here]

      User Opinions:
      - Alice: [Insert Alice's ChatUser.AiOpinion here]
      - Bob: [Insert Bob's ChatUser.AiOpinion here]"
    },
    {
      "role": "user",
      "name": "Alice",
      "content": "Hey Airy, how are you doing today?"
    },
    {
      "role": "assistant",
      "content": "I'm doing well, Alice! Thanks for asking. How can I assist you?"
    },
    {
      "role": "user",
      "name": "Bob",
      "content": "Airy, can you tell me about the weather?"
    },
    {
      "role": "assistant",
      "content": "the weather is good and sunny today."
    },
    {
      "role": "user",
      "name": "Alice",
      "content": "Oh, I was just about to ask that too, Bob!"
    }
  ],
  "max_tokens": 800
}
```

## Section Explanations

### `model`
*   **Purpose:** Specifies the particular OpenAI language model to use for the completion.
*   **Example:** `"gpt-4o-mini"`
*   **Our Use:** We will continue to use the configured model name, typically `gpt-4o-mini`.

### `messages` (Array of Message Objects)
*   **Purpose:** This is the core of the conversational input. It's an array of message objects, each representing a turn in the conversation. The order of messages is crucial as it dictates the conversational flow and context for the AI.

#### Message Object Fields:

*   **`role`**
    *   **Purpose:** Defines the speaker of the message. This is critical for the AI to understand the conversational turn-taking.
    *   **Possible Values:**
        *   `"system"`: Provides initial instructions, context, or personality definition for the AI. This is where our `systemPrompt` goes.
        *   `"user"`: Represents a message from a human user.
        *   `"assistant"`: Represents a message from the AI itself (Airy). These are included in the history to maintain the AI's own context.
    *   **Our Use:** We map our `ChatRole` enum (`System`, `User`, `Assistant`) directly to these lowercase string values.

*   **`content`**
    *   **Purpose:** The actual text content of the message.
    *   **Our Use:** This will be the `Context` property of our `Message` domain object.

*   **`name`** (Optional, but crucial for our goal)
    *   **Purpose:** An optional field that provides a name for the participant in a multi-turn conversation. When provided for `"user"` messages, it helps the AI distinguish between different human speakers.
    *   **Constraints:** Must be a string containing only a-z, A-Z, 0-9, dashes, and underscores, with a maximum length of 64 characters.
    *   **Our Use:** We will include this field for all `"user"` role messages, using a sanitized version of the Discord user's `UserName`. This directly addresses the requirement for the AI to "understand who said what."

### `max_tokens`
*   **Purpose:** The maximum number of tokens to generate in the chat completion. The total length of input tokens and generated tokens is limited by the model's context window.
*   **Example:** `800`
*   **Our Use:** We will continue to use a configured value, currently `800`.
