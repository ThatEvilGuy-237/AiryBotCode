using Docker.DotNet;
using Docker.DotNet.Models;
using System.Diagnostics;

namespace AiryBotCode.Launcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting bot launcher...");

            var dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

            var botsDir = new DirectoryInfo("Bots");
            if (!botsDir.Exists)
            {
                Console.WriteLine("Bots directory not found!");
                return;
            }

            var botProjects = botsDir.GetDirectories();
            var imageName = "airy-bot-image";
            var templateFile = "BotTemplate.dockerfile";

            Console.WriteLine($"Building base image '{imageName}' from '{templateFile}'...");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"build -t {imageName} -f {templateFile} .",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                Console.WriteLine("Image build failed:");
                Console.WriteLine(await process.StandardError.ReadToEndAsync());
                return;
            }

            Console.WriteLine("Image build complete.");

            foreach (var project in botProjects)
            {
                var botName = project.Name.ToLower();
                var projectPath = $"Bots/{project.Name}";

                Console.WriteLine($"Checking for existing container for bot: {botName}");

                var existingContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
                var existingContainer = existingContainers.FirstOrDefault(c => c.Names.Contains($"/{botName}"));

                if (existingContainer != null)
                {
                    Console.WriteLine($"Removing existing container: {existingContainer.ID}");
                    await dockerClient.Containers.RemoveContainerAsync(existingContainer.ID, new ContainerRemoveParameters { Force = true });
                }

                Console.WriteLine($"Starting container for bot: {botName}");

                var container = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = imageName,
                    Name = botName,
                    HostConfig = new HostConfig
                    {
                        RestartPolicy = new RestartPolicy
                        {
                            Name = RestartPolicyKind.Always
                        }
                    },
                    Cmd = new List<string> { "dotnet", "run", "--project", projectPath, "--working-dir", projectPath }
                });

                await dockerClient.Containers.StartContainerAsync(container.ID, null);

                Console.WriteLine($"Container for {botName} started with ID: {container.ID}");
            }

            Console.WriteLine("All bots started. Launcher finished.");
        }
    }
}