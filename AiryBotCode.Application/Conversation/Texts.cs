using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace AiryBotCode.Application.Conversation
{
    public static class Texts
    {
        private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> Files = new();

        public static IReadOnlyDictionary<string, string> Sections(string file) => Files.GetOrAdd(file, Read);

        public static string Section(string file, string key) => Sections(file)[key];

        public static string Say(string file, string key, params (string Name, string Value)[] values) =>
            Fill(Section(file, key), values.ToDictionary(v => v.Name, v => v.Value));

        public static string Fill(string template, IReadOnlyDictionary<string, string> values) =>
            Regex.Replace(template, @"\{\{(\w+)\}\}", m => values.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value).Trim();

        public static string Flat(string text, int max)
        {
            var flat = text.Replace('\n', ' ').Trim();
            return flat.Length <= max ? flat : flat[..max] + "…";
        }

        private static IReadOnlyDictionary<string, string> Read(string file)
        {
            using var stream = typeof(Texts).Assembly.GetManifestResourceStream($"AiryBotCode.Texts.{file}.md")
                ?? throw new InvalidOperationException($"Text {file}.md is not embedded.");
            using var reader = new StreamReader(stream);
            var sections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string? current = null;
            var buffer = new StringBuilder();
            foreach (var line in reader.ReadToEnd().Split('\n'))
            {
                if (line.StartsWith("## "))
                {
                    if (current is not null) sections[current] = buffer.ToString().Trim();
                    current = line[3..].Trim();
                    buffer.Clear();
                }
                else if (current is not null) buffer.AppendLine(line.TrimEnd('\r'));
            }
            if (current is not null) sections[current] = buffer.ToString().Trim();
            return sections;
        }
    }
}
