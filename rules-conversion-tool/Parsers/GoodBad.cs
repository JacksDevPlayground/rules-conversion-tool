using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace rules_conversion_tool.Parsers;

public class GoodBad
{
    private enum GoodBadType
    {
        Good,
        Bad,
        Ok
    }

    public static string Parse(string markdown)
    {
        var parsedMarkdown = new StringBuilder(markdown);
        var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
        var replacements = new Dictionary<string, string>();

        foreach (var block in document)
        {
            if (block is not CustomContainer customContainer) continue;

            var containerType = customContainer.Info;
            if (string.IsNullOrEmpty(containerType)) continue;

            if (Enum.TryParse(containerType, true, out GoodBadType type))
            {
                string figureText = string.Empty;
                string imageUrl = string.Empty;

                var contentInside = customContainer.Count > 0
                    ? markdown.Substring(customContainer[0].Span.Start,
                        customContainer[^1].Span.End - customContainer[0].Span.Start + 1).Trim()
                    : string.Empty;

                // Check for LinkInline objects (media links)
                var linkInline = customContainer.Descendants<LinkInline>().FirstOrDefault();
                if (linkInline != null && linkInline.FirstChild is LiteralInline literal)
                {
                    figureText = literal.Content.ToString();
                    imageUrl = linkInline.Url;
                }
                
                if (!string.IsNullOrEmpty(contentInside) && string.IsNullOrEmpty(figureText))
                {
                    if (contentInside.StartsWith("Figure:"))
                    {
                        figureText = contentInside.Split('\n').FirstOrDefault() ?? string.Empty;
                        contentInside = ""; // Clear the contentInside as it's only the figure text
                    }
                }
                
                var transformedContent = string.IsNullOrEmpty(imageUrl)
                    ? (string.IsNullOrEmpty(contentInside)
                        ? $"{{% GoodBadComponent type=\"{containerType.ToLower()}\" figureText=\"{figureText}\" /%}}"
                        : $"{{% GoodBadComponent type=\"{containerType.ToLower()}\" figureText=\"{figureText}\" %}}\n{contentInside}\n{{% /GoodBadComponent %}}")
                    : $"{{% GoodBadImageComponent type=\"{containerType.ToLower()}\" image=\"{imageUrl}\" figureText=\"{figureText}\" /%}}";

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