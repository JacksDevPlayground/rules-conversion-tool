using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace rules_conversion_tool.Parsers;

public partial class Twitter
{
    public static string Parse(string markdown)
    {
        // Clone the markdown
            var parsedMarkdown = new StringBuilder(markdown);

            // Parse the cloned markdown for YouTube blocks
            var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().Build();
            var document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
            List<(string, string)> replacements = new List<(string, string)>();
            
            for (var i = 0; i < document.Count; i++)
            {
                var block = document[i];
                if (block is ParagraphBlock { Inline.FirstChild: CodeInline codeInline } &&
                    FindTwitterBlock(codeInline))
                {
                    var tweetId = ExtractTweetId(codeInline.Content);
                    
                    var transformedBlock =
                        $"{{% TweetComponent #{tweetId}  /%}}";
                    
                    //{% TweetComponent #abc123 /%}
                    replacements.Add(($"`{codeInline.Content}`", transformedBlock));
                }
            }

            // Now, replace the matches in the original markdown
            return replacements.Aggregate(markdown, (current, r) => current.Replace(r.Item1, r.Item2));
        }
        
        private static bool FindTwitterBlock(CodeInline block)
        {
            return block.Content.StartsWith("oembed:") && block.Content.Contains("twitter.com");
        }

        public static string? ExtractTweetId(string youtubeLink)
        {
            // Extract the actual URL from the youtubeLink
            var match = HtmlMatchRegex().Match(youtubeLink);
            if (!match.Success) return null;

            var url = match.Value.Trim('`'); // Ensure to trim any backticks

            if (!url.Contains("twitter.com")) return null;
            
            const string tweetIdPattern = @"status/(\d+)";
            var tweetMatch = TwitterRegex().Match(url);
            return tweetMatch.Success ? tweetMatch.Groups[1].Value : null;
        }

    [GeneratedRegex("status/(\\d+)")]
    private static partial Regex TwitterRegex();
    [GeneratedRegex(@"https?://[^\s]+")]
    private static partial Regex HtmlMatchRegex();
}