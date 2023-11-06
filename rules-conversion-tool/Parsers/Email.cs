using System.Text;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Syntax;

namespace rules_conversion_tool.Parsers;

public class Email
{
    public static string Parse(string markdown)
    {
        StringBuilder emailComponent = new StringBuilder("");

        StringBuilder parsedMarkdown = new StringBuilder(markdown);
        MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseCustomContainers().UseMediaLinks().Build();
        MarkdownDocument document = Markdown.Parse(parsedMarkdown.ToString(), pipeline);
        Dictionary<string, string> replacements = new Dictionary<string, string>();
        
        foreach (Block block in document)
        {
            // Only get custom blocks
            if (block is not CustomContainer customContainer) continue;

            // Only get custom blocks that are of type "email-template"
            string containerType = customContainer.Info;
            if (string.IsNullOrEmpty(containerType) || containerType != "email-template") continue;
            
            Console.WriteLine($"[DEBUG] customContainer.Count {customContainer.Count}");

            foreach (Block container in customContainer)
            {
                switch (container)
                {
                    // Get "email-template" content
                    case ParagraphBlock:
                        string to = "";
                        string cc = "";
                        string bcc = "";
                        string subject = "";
                        
                        foreach (string line in markdown.Substring(container.Span.Start, container.Span.End - container.Span.Start).Split("\n"))
                        {
                            if (string.IsNullOrEmpty(to) && line.Contains("To"))
                            {
                                to = line.Split("|")[2].Trim();
                                continue;
                            } 
                            
                            if (string.IsNullOrEmpty(cc) && line.Contains("Cc")) 
                            {
                                cc = line.Split("|")[2].Trim();
                                continue;
                            } 
                            
                            if (string.IsNullOrEmpty(bcc) && line.Contains("Bcc"))
                            { 
                                bcc = line.Split("|")[2].Trim();
                                continue;
                            } 
                            
                            if (string.IsNullOrEmpty(subject) && line.Contains("Subject"))
                            {
                                subject = line.Split("|")[2].Trim();
                            }
                        }
                        
                        emailComponent.Append($"{{% EmailTemplateComponent to=\"{to}\" cc=\"{cc}\" bcc=\"{bcc}\" subject=\"{subject}\" %}}\n");
                        break;
                    case CustomContainer c:
                        if (c.Info == "email-content") 
                        {
                            emailComponent.Append(markdown.Substring(c[0].Span.Start, c[^1].Span.End - c[0].Span.Start + 1).Replace("::: email-content", "").Trim());
                            emailComponent.Append("\n{% /EmailTemplateComponent %}");
                        }
                        break;
                }
            }

            int start = block.Span.Start;
            int end = block.Span.End;
            string originalContent = markdown.Substring(start, end - start + 5);
            replacements.Add(originalContent, emailComponent.ToString());
        }

        // Apply all replacements to the original markdown
        return replacements.Aggregate(markdown, (current, pair)
            => current.Replace(pair.Key, pair.Value));
    }
}