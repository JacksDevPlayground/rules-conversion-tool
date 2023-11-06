using System.Text;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;

namespace rules_conversion_tool.Parsers;

public class Greybox
{
    public static string Parse(string markdown)
    {
        // Clone the markdown
        var parsedMarkdown = new StringBuilder(markdown);

        // Parse the cloned markdown for greybox blocks
        var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
        List<(string, string)> replacements = new List<(string, string)>();
        foreach (var block in document)
        {
            if (block is not CustomContainer customContainer) continue;

            // Only get greybox containers
            var containerType = customContainer.Info;
            if (string.IsNullOrEmpty(containerType) || containerType != "greybox") continue;

            // Extract content inside the CustomContainer
            var contentInside = customContainer.Count > 0
                ? markdown.Substring(customContainer[0].Span.Start,
                    customContainer[^1].Span.End - customContainer[0].Span.Start + 1).Trim()
                : string.Empty;

            var transformedContent = $"{{% GreyboxComponent %}}\n{contentInside}\n{{% /GreyboxComponent %}}";

            // Replace the original content with the transformed content
            var start = block.Span.Start;
            var end = block.Span.End;
            var originalContent = markdown.Substring(start, end - start + 1);
            replacements.Add((originalContent, transformedContent));
        }

        // Apply all replacements to the original markdown
        return replacements.Aggregate(markdown, (current, r) => current.Replace(r.Item1, r.Item2));
    }
}