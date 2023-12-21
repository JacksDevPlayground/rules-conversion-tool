using System.Text;
using Markdig;

namespace rules_conversion_tool.Parsers;

public class Image
{
    public static string Parse(string markdown)
    {
        // Clone the markdown
        var parsedMarkdown = new StringBuilder(markdown);

        // Parse the cloned markdown for greybox blocks
        var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);

        return markdown;
    }
}