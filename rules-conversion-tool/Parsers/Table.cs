using System.Text;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;

namespace rules_conversion_tool.Parsers;

public class Table
{
    public static string Parse(string markdown)
    {
        // Clone the markdown
        var parsedMarkdown = new StringBuilder(markdown);

        // Parse the cloned markdown for YouTube blocks
        var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
        List<(string, string)> replacements = new List<(string, string)>();
        foreach (var block in document)
        {
            if (block is not CustomContainer customContainer) continue;

            //! Only get email-template containers
            var containerType = customContainer.Info;
            if (string.IsNullOrEmpty(containerType) || containerType != "email-template") continue;
            AnsiConsole.MarkupLine($"[yellow]Container type: {containerType}[/]");

            var contentBuilder = new StringBuilder();
            foreach (var childBlock in customContainer)
            {
                if (childBlock is not ParagraphBlock paragraphBlock) continue;
                if (paragraphBlock.Inline == null) continue;
                foreach (var inline in paragraphBlock.Inline)
                {
                    contentBuilder.Append(inline);
                }
            }

            AnsiConsole.MarkupLine($"[cyan]Content inside CustomContainer: {contentBuilder}[/]");
        }

        return string.Empty;
    }
}