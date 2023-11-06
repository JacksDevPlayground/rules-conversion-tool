using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace rules_conversion_tool.Parsers
{
    public static class Youtube
    {
        public static string Parse(string markdown)
        {
            // Clone the markdown
            var parsedMarkdown = new StringBuilder(markdown);

            // Parse the cloned markdown for YouTube blocks
            var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().Build();
            var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
            List<(string, string)> replacements = new List<(string, string)>();
            
            for (int i = 0; i < document.Count; i++)
            {
                var block = document[i];
                if (block is ParagraphBlock { Inline.FirstChild: CodeInline codeInline } &&
                    FindYoutubeBlock(codeInline))
                {
                    var videoId = ExtractVideoId(codeInline.Content);
                    var figureText = string.Empty;
                    if (i < document.Count - 1 && document[i + 1] is ParagraphBlock nextBlock)
                    {
                        if (nextBlock.Inline.FirstChild is EmphasisInline emphasisInline)
                        {
                            StringBuilder emphasisContent = new StringBuilder();
                            foreach (var inline in emphasisInline)
                            {
                                emphasisContent.Append(inline);
                            }

                            figureText = emphasisContent.ToString();
                        }
                        else
                        {
                            figureText = string.Empty;
                        }
                        replacements.Add(($"**{figureText}**", string.Empty));
                        i++; // Skip the next block since we've processed it
                    }

                    figureText = figureText.Replace("\n", " ").Replace("\r", "").Trim().Trim('*').Trim();
                    var transformedBlock =
                        $"{{% YoutubeVideoComponent videoId=\"{videoId}\" figureText=\"{figureText}\"  /%}}";
                    replacements.Add(($"`{codeInline.Content}`", transformedBlock));
                }
            }

            // Now, replace the matches in the original markdown
            return replacements.Aggregate(markdown, (current, r) => current.Replace(r.Item1, r.Item2));
        }
        
        private static bool FindYoutubeBlock(CodeInline block)
        {
            return block.Content.StartsWith("youtube:");
        }

        private static string? ExtractVideoId(string youtubeLink)
        {
            // Extract the actual URL from the youtubeLink
            var match = Regex.Match(youtubeLink, @"https?://[^\s]+");
            if (!match.Success) return null;

            var url = match.Value.Trim('`'); // Ensure to trim any backticks

            if (url.Contains("youtu.be"))
            {
                return url[(url.LastIndexOf('/') + 1)..];
            }
            else if (url.Contains("youtube.com"))
            {
                var uri = new Uri(url);
                var query = uri.Query;
                var queryParams = HttpUtility.ParseQueryString(query);
                return queryParams["v"];
            }

            return null;
        }
    }
}