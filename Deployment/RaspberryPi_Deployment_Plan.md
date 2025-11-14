# Raspberry Pi Deployment Plan

This document outlines the plan for deploying the Airy Discord Bot to a Raspberry Pi using a containerized launcher.

## Chosen Architecture: Launcher-in-a-Container

The core of this strategy is a single "Launcher" application that runs inside its own Docker container. This launcher is given access to the host's Docker service, allowing it to start, stop, and manage a separate container for each individual bot.

---

### Pros and Cons

| Pros                                                              | Cons                                                                      |
| ----------------------------------------------------------------- | ------------------------------------------------------------------------- |
| **Clean Host System**                                             | **Initial Complexity**                                                    |
| Only Docker needs to be installed on the Raspberry Pi. No .NET SDK or other dependencies are needed on the host. | The initial setup of the Dockerfile and docker-compose is more complex than a simple script. |
| **Perfect Environment Consistency**                               | **Security Consideration**                                                |
| The launcher and all bots run in defined, identical container environments, eliminating "it works on my machine" issues. | Giving a container access to the host's Docker socket is powerful and requires trust in the container's code. |
| **Scalability & Ease of Use**                                     | **Resource Overhead**                                                     |
| Adding a new bot is as simple as adding its project folder. The user never has to touch Dockerfiles or build scripts. | Each bot runs in its own container, which has a small amount of resource overhead compared to running all bots in one process. |
| **High Portability**                                              |                                                                           |
| The entire system can be moved to any machine running Docker (x86 or ARM) and started with a single `docker-compose up` command. |                                                                           |

---

## Step-by-Step Implementation Plan

### Step 1: Create the New Launcher Project
- We will create a new .NET Console Application project named `AiryBotCode.Launcher`.
- This project will be placed in a new top-level `Launcher` directory.
- We will add a reference to a Docker management library, such as `Docker.DotNet`, to allow the C# code to interact with the Docker API.

### Step 2: Create a Generic "Bot" Dockerfile
- A single `Dockerfile` will be created. This is the master template for all bots.
- **Build Stage:** This stage will copy the *entire* solution source code, restore all NuGet packages, and build every project (`dotnet build`).
- **Final Stage:** This stage will contain the final published output of the entire solution. It will be configured with an entrypoint that can launch a specific bot project based on an environment variable or command-line argument.
- **Cross-Platform Build:** The Dockerfile will be configured to build for the `linux/arm64` architecture required by the Raspberry Pi.

### Step 3: Implement the Launcher's Logic
The C# code within the `AiryBotCode.Launcher` project will perform the following actions on startup:
1.  **Scan for Bots:** It will scan the `/Bots` directory to get a list of all available bot projects.
2.  **Check Running Containers:** It will query the Docker API to see which bot containers are currently running.
3.  **Reconcile State:**
    - It will start a new container for any bot project that doesn't have a corresponding running container.
    - It will stop and remove any running bot containers that no longer have a corresponding project in the `Bots` folder.
4.  **Container Configuration:** When starting a bot container, the launcher will:
    - Use the generic "bot" image created in Step 2.
    - Mount the specific `appsettings.json` from the bot's project directory into the container.
    - Pass an environment variable (e.g., `BOT_PROJECT_NAME`) telling the container which bot `.csproj` to run.

### Step 4: Create the `docker-compose.yml` File
- This file will define the **Launcher service**. It will *not* define services for the individual bots.
- **Image:** It will point to the generic bot image and instruct Docker to build it using our `Dockerfile`.
- **Volume Mounts:**
    - It will mount the host's Docker socket (`/var/run/docker.sock`) into the launcher container, giving it control over Docker.
    - It will mount the solution's `/Bots` directory into the container so the launcher can scan for bot projects.
- **Restart Policy:** It will be set to `always` to ensure the launcher (and by extension, all the bots it manages) restarts automatically if the Raspberry Pi reboots.

### Step 5: End-User Workflow
Once the setup is complete, managing bots becomes incredibly simple for the end-user:
- **To Add a Bot:** The user creates a new bot project inside the `Bots` directory. They then run `docker-compose up --build` on the Raspberry Pi. The launcher will automatically find and start the new bot in its own container.
- **To Remove a Bot:** The user deletes the bot's project folder from the `Bots` directory. The next time the launcher runs, it will automatically stop and remove the corresponding container.