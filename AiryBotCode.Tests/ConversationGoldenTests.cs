using AiryBotCode.Application.Conversation;
using System.Runtime.CompilerServices;
using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ConversationGoldenTests
    {
        private static readonly LogLine[] Room =
        [
            new(1, "KiraZ", "anyone listen to trance audios before bed?"),
            LogLine.Placeholder(2),
            new(3, "JunoMx", "yeah they knock me out\nevery time"),
        ];

        private static readonly CheckLine[] Recent =
        [
            new("KiraZ", "anyone listen to trance audios before bed?"),
            new("Airy", "Slow ones work best for me."),
            new("JunoMx", new string('z', 320)),
        ];

        public static TheoryData<string> Names() => new()
        {
            "agent-alone", "agent-context-pictures-about", "agent-join", "agent-join-pictures-about",
            "check-join", "check-join-empty", "check-follow-up",
        };

        private static string Render(string name) => name switch
        {
            "agent-alone" => AgentMessage.Reply("Airy", "events", "Evil (42)", "hello", []),
            "agent-context-pictures-about" => AgentMessage.Reply("Airy", "events", "Evil (42)", "Do you take that fox? :evil_merc:", Room,
                [":evil_merc:", "[gif: cat dance]"], aboutYou: true),
            "agent-join" => AgentMessage.JoinIn("Airy", "airy-talk", "JunoMx (7)", Room),
            "agent-join-pictures-about" => AgentMessage.JoinIn("Airy", "airy-talk", "JunoMx (7)", Room, ["[sticker wave]"], aboutYou: true),
            "check-join" => JoinCheck.Body(new CheckRequest("Airy", "airy-talk", "Evil", 405, "Isn't that right foxy fox airy", Recent)),
            "check-join-empty" => JoinCheck.Body(new CheckRequest("Airy", "airy-talk", "Evil", 405, "gm", [])),
            "check-follow-up" => JoinCheck.Body(new CheckRequest("Airy", "airy-talk", "Evil", 405, "Good bot", Recent, FollowUp: true)),
            _ => throw new ArgumentOutOfRangeException(nameof(name)),
        };

        [Theory]
        [MemberData(nameof(Names))]
        public void What_the_agent_and_the_decider_read_does_not_change(string name)
        {
            var path = Path.Combine(Folder(), "Golden", name + ".txt");
            var actual = Render(name);
            if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1") File.WriteAllText(path, actual);
            Assert.Equal(File.ReadAllText(path), actual);
        }

        private static string Folder([CallerFilePath] string file = "") => Path.GetDirectoryName(file)!;
    }
}
