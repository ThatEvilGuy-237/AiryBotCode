# AiryBotCode

## Project Overview

This is a fun side project I created in C# because I enjoy working with Discord bots. Instead of using pre-made blocks or strange custom languages often provided by bot platforms, I decided to build my own Discord bot called **Airy**. The project has since been refactored into a **clean architecture-inspired** structure.

---

## Key Tips Before Contributing

When working on this project, there are a few important things to know that are currently the standards and saves you time lol.

### Command & Action Logic

Every **command or action** (for example: `UserlogsCommand`) should contain the full logic for that specific interaction. These interactions can happen in sequence.  
For example:

1. A slash command (`/userlogs`) triggers an action.
2. It sends a message with a button.
3. The button interaction is handled.
4. A form is created and must also be handled.

All of this logic should **remain inside the same command class** (`UserlogsCommand`) in the Aplication layer. This way your action 'chain' stays clear and maintainable. You can create services to make it more oginzed and easier to read. For example:
- Banning a user
- Deleting multiple messages
- Logging actions

Then register all your **commands** and **services** in `RegisterApplication.cs` (check console for error messages if you forgot any).


## Event Handling

Next, create an **Event** class for your command or action (for example: `UserlogsEvent`). This class handles Discord events such as:

- `SendMessage`
- `JoinServer`
- `ButtonPress`
- `SlashCommand`
- `FormInteraction`

There are some interfaces already in place to support these. Each event class should implement the necessary interfaces that you need for your command to fully work.  
For example, `UserlogsEvent.cs` look like this:
```csharp
public class UserlogsEvent : EvilEvent, ISlashEvent, IButtonEvent, IFormEvent, IClientAccess
{
    // Functon from the interfaces
}
```

Each interaction type (e.g. *slash, button, form, etc.*), or even specific data that not all classes need (like *client*), should be handled within this specific class.

After that:

- Add the event class to the appropriate handler constructor in your `Events/...Handler.cs` file.
- Register it in `RegisterInfrastructure.cs`.


**Once everything is wired up, run and test your code!**

*(dont forget to set your evn file to your bot and check the AiryDevBot.cs for evenes that you want to use for your bot)*

---
## Current Features
- Time out a user and create userlog
- Untime out a user
- Creating editable userlogs
- 
---
## TODO
- Logchannel (make better settings/env file handeling)
- Remind a user of somthing (with punshments? *hehehe*)

---
## FUTURE UPGRADES AND IDEAS
- [ ] Expend the readabilty and services
- [ ] Better way of using ENV
- [ ] Better naming for command and events
- [ ] Database per server insted of ENV file [low-priority]
- [ ] ~~Tests?? XD~~

---
# Current ENV file:
- DISCORD_AIRYBOT_TOKEN=[DISCORD_TOKEN]
- ADMINROLEID=[ADMIN_IN_SERVER]
- EVILID=[BOT_OWNER_ID]
