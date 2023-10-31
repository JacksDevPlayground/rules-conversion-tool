using System.Text;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace rules_conversion_tool.Parsers;

public class Aside
{
    private enum AsideType
    {
        Warning,
        Info,
        Todo,
        China,
        Codeauditor,
        Highlight
    }

    public static string Parse(string markdown)
    {
        // Clone the markdown
        var parsedMarkdown = new StringBuilder(markdown);

        // Parse the cloned markdown for YouTube blocks
        var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
        var replacements = new Dictionary<string, string>();
        foreach (var block in document)
        {
            if (block is not CustomContainer customContainer) continue;

            // Extract the word after :::
            var containerType = customContainer.Info;
            if (string.IsNullOrEmpty(containerType)) continue;
            if (Enum.TryParse(containerType, true, out AsideType type))
            {
                // Extract content inside the CustomContainer
                var contentInside = customContainer.Count > 0
                    ? markdown.Substring(customContainer[0].Span.Start,
                        customContainer[^1].Span.End - customContainer[0].Span.Start + 1).Trim()
                    : string.Empty;

                var transformedContent = string.IsNullOrEmpty(contentInside)
                    ? $"{{% AsideComponent type=\"{containerType.ToLower()}\" /%}}"
                    : $"{{% AsideComponent type=\"{containerType.ToLower()}\" %}}\n{contentInside}\n{{% /AsideComponent %}}";

                // Replace the original content with the transformed content
                var start = block.Span.Start;
                var end = block.Span.End;
                var originalContent = markdown.Substring(start, end - start + 1);
                replacements.Add(originalContent, transformedContent);
            }
        }
        
        markdown = replacements.Aggregate(markdown, (current, pair) => current.Replace(pair.Key, pair.Value));
        return markdown;
    }
}