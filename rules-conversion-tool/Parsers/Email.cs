using System.Text;
using Markdig;
using Markdig.Extensions.CustomContainers;

namespace rules_conversion_tool.Parsers;

public class Email
{
    public static string Parse(string markdown)
    {
        // Clone the markdown
        var parsedMarkdown = new StringBuilder(markdown);

        // Parse the cloned markdown for greybox blocks
        var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
        var replacements = new Dictionary<string, string>();
        foreach (var block in document)
        {
            if (block is not CustomContainer customContainer) continue;

            // Only get greybox containers
            var containerType = customContainer.Info;
            AnsiConsole.MarkupLine($"[cyan]Container type: {containerType}[/]");
            if (string.IsNullOrEmpty(containerType) || containerType != "email-template") continue;
            AnsiConsole.MarkupLine($"[yellow]Container type: {containerType}[/]");
            AnsiConsole.MarkupLine($"[yellow]Container Count: {customContainer.Count}[/]");
            // Extract content inside the CustomContainer
            var contentInside = customContainer.Count > 0
                ? markdown.Substring(customContainer[0].Span.Start,
                    customContainer[^1].Span.End - customContainer[0].Span.Start + 1).Trim()
                : string.Empty;
            AnsiConsole.MarkupLine($"[yellow]ContentInside: {contentInside}[/]");
        }

        // Apply all replacements to the original markdown
        return replacements.Aggregate(markdown, (current, pair) 
            => current.Replace(pair.Key, pair.Value));
    }
}