using System.ComponentModel;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using rules_conversion_tool.Parsers;
using YamlDotNet.Serialization;

namespace rules_conversion_tool;

public enum ParserType
{
    GoodBad,
    Youtube,
    Aside,
    Frontmatter,
    Twitter,
    Email,
    Greybox,
}

public enum ParseContent
{
    CompleteFile, // SELECT ONLY THIS OPTION TO PARSE ALL OPTIONS
    BlurbOnly,
    BodyOnly,
}

public struct BodyMarkdown
{
    public string? OutPath { get; set; }
    public string? Content { get; set; }
}

public struct BlurbMarkdown
{
    public string? OutPath { get; set; }
    public string? Content { get; set; }
}

public struct MarkdownProperties
{
    public string? OutDir { get; set; }
    public string? Frontmatter { get; set; }
    public BodyMarkdown? Body { get; set; }
    public BlurbMarkdown? Blurb { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class ConversionSettings : CommandSettings
{
    [CommandArgument(0, "<FILE_OR_DIRECTORY>")]
    [Description("Path to the markdown file or directory.")]
    public string? Path { get; set; }

    [CommandArgument(1, "<OUTPUT_DIR>")]
    [Description("Output path, folder will be filled")]
    public string? Output { get; set; }

    public required List<ParseContent> SelectedContent { get; set; }

    // Add a new property to store the selected parsers
    public required List<ParserType> SelectedParsers { get; set; }
}

public class ConversionCommand : Command<ConversionSettings>
{
    public override int Execute(CommandContext context, ConversionSettings settings)
    {
        // Prompt the user for parser selection
        settings.SelectedContent = AnsiConsole.Prompt(
            new MultiSelectionPrompt<ParseContent>()
                .Title("Which [green]content section[/] would you like to parse?")
                .Required()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more content sections)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a content section, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(Enum.GetValues<ParseContent>()));

        settings.SelectedParsers = AnsiConsole.Prompt(
            new MultiSelectionPrompt<ParserType>()
                .Title("Which [green]parsers[/] would you like to use?")
                .Required()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more parsers)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a parser, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(Enum.GetValues<ParserType>()));

        // Now, process the files using the selected parsers
        if (Directory.Exists(settings.Path))
        {
            foreach (var file in Directory.GetFiles(settings.Path, "rule.md", SearchOption.AllDirectories))
            {
                Console.WriteLine($"[DEBUG] parsing {file}");
                Parse(file, settings);
            }
        }
        else if (File.Exists(settings.Path))
        {
            Parse(settings.Path, settings);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error:[/] The specified path does not exist.");
            return -1;
        }

        return 0;
    }

    private static void Parse(string filePath, ConversionSettings settings)
    {
        var markdown = File.ReadAllText(filePath);
        AnsiConsole.MarkupLine($"[green]File:[/] {filePath}");
        AnsiConsole.MarkupLine($"[green]Selected Content:[/] {string.Join(", ", settings.SelectedContent!)}");
        AnsiConsole.MarkupLine($"[green]Selected Options:[/] {string.Join(", ", settings.SelectedParsers!)}");

        var markdownProperties = SpiltFrontMatterFromMarkdown(markdown, settings);
        if (markdownProperties is null) throw new Exception("No Markdown Properties Found");

        var blurbContent = markdownProperties?.Blurb?.Content;
        var bodyContent = markdownProperties?.Body?.Content;

        // Select Content 
        foreach (var parseContent in settings.SelectedContent)
        {
            switch (parseContent)
            {
                case ParseContent.CompleteFile:
                    // Handle Blurb
                    if (string.IsNullOrEmpty(blurbContent)) AnsiConsole.MarkupLine($"[red]No Blurb Content Found[/]");
                    var blurb1 = ParseParserContent(blurbContent, settings.SelectedParsers!, markdownProperties!);
                    File.WriteAllText(markdownProperties.Value.Blurb.Value.OutPath, blurb1);

                    // Handle Body
                    if (string.IsNullOrEmpty(bodyContent)) AnsiConsole.MarkupLine($"[red]No Blurb Content Found[/]");
                    var body1 = ParseParserContent(GetBodyContent(markdown), settings.SelectedParsers!, markdownProperties!);
                    File.WriteAllText(markdownProperties.Value.Body.Value.OutPath, body1);

                    // Handle Frontmatter 
                    string frontmatterContent = string.Empty;
                    if (string.IsNullOrEmpty(markdownProperties.Value.Frontmatter)) AnsiConsole.MarkupLine($"[red]No Frontmatter Found[/]");
                    // Append the Frontmatter to the Body content
                    if (!string.IsNullOrEmpty(markdownProperties.Value.Frontmatter))
                    {
                        frontmatterContent = markdownProperties.Value.Frontmatter;
                        var combinedContent = frontmatterContent + Environment.NewLine + body1;
                        File.WriteAllText(markdownProperties.Value.Body.Value.OutPath, combinedContent);
                    }

                    break;
                case ParseContent.BlurbOnly:
                    if (string.IsNullOrEmpty(blurbContent)) throw new Exception("No Blurb Content Found");
                    var blurb = ParseParserContent(blurbContent, settings.SelectedParsers!, markdownProperties!);
                    File.WriteAllText(markdownProperties.Value.Blurb.Value.OutPath, blurb);
                    break;
                case ParseContent.BodyOnly:
                    if (string.IsNullOrEmpty(bodyContent)) throw new Exception("No Body Content Found");
                    var body = ParseParserContent(GetBodyContent(markdown), settings.SelectedParsers!,
                        markdownProperties!);
                    File.WriteAllText(markdownProperties.Value.Body.Value.OutPath, body);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static string GetBlurbContent(string markdown)
    {
        // Find the first "<!--endintro-->" and return everything before it
        var index = markdown.IndexOf("<!--endintro-->");
        if (index <= 0) return string.Empty;
        return markdown[..index].Trim();
    }

    public static string GetBodyContent(string markdown)
    {
        // Anything after the first "<!--endintro-->" is the body!
        var index = markdown.IndexOf("<!--endintro-->");
        if (index != -1)
        {
            return markdown.Substring(index + "<!--endintro-->".Length).Trim();
        }

        return string.Empty;
    }

    private static string ParseParserContent(string markdown, List<ParserType> parsers,
        MarkdownProperties? markdownProperties)
    {
        // if (string.IsNullOrEmpty(markdown)) throw new Exception("Markdown was empty");
        if (string.IsNullOrEmpty(markdown)) return "";

        if (markdownProperties is null) throw new Exception("No Markdown Properties Found");

        var content = markdown;
        foreach (var parser in parsers)
        {
            switch (parser)
            {
                case ParserType.GoodBad:
                    content = GoodBad.Parse(content);
                    break;
                case ParserType.Aside:
                    content = Aside.Parse(content);
                    break;
                case ParserType.Youtube:
                    content = Youtube.Parse(content);
                    break;
                case ParserType.Twitter:
                    content = Twitter.Parse(content);
                    break;
                case ParserType.Greybox:
                    content = Greybox.Parse(content);
                    break;
                case ParserType.Email:
                    content = Email.Parse(content);
                    break;
                case ParserType.Frontmatter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return content;
    }

    private static (string, string) CreateIndexMdoc(Dictionary<string, object> metadata, ConversionSettings settings)
    {
        var outputDir = Path.Combine(settings.Output, metadata["uri"].ToString() ?? string.Empty);
        Directory.CreateDirectory(outputDir);
        var outputPath = Path.Combine(outputDir, "index.mdoc");
        return (outputDir, outputPath);
    }

    private static (string, string) CreateBlurbMdoc(Dictionary<string, object> metadata, ConversionSettings settings)
    {
        try
        {
            var outputDir = Path.Combine(settings.Output, metadata["uri"].ToString() ?? string.Empty);
            Directory.CreateDirectory(outputDir);
            var outputPath = Path.Combine(outputDir, "blurb.mdoc");
            return (outputDir, outputPath);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {e.Message}");
        }

        return (String.Empty, String.Empty);
    }

    private static string GetContentWithoutFrontmatter(string markdown, YamlFrontMatterBlock frontMatterBlock)
    {
        // Convert the frontmatter block lines to a single string
        var frontmatter = string.Join(Environment.NewLine, frontMatterBlock.Lines);

        // Add the delimiters to the frontmatter string
        frontmatter = $"---{Environment.NewLine}{frontmatter}{Environment.NewLine}---";

        // Remove the frontmatter from the original markdown
        var contentWithoutFrontmatter = markdown.Replace(frontmatter, "").Trim();

        return contentWithoutFrontmatter;
    }

    private static MarkdownProperties? SpiltFrontMatterFromMarkdown(string markdown, ConversionSettings settings)
    {
        AnsiConsole.MarkupLine($"[blue]Setting up parser[/]");
        var pipeline = new MarkdownPipelineBuilder().UseYamlFrontMatter().Build();
        var document = Markdown.Parse(markdown, pipeline);

        AnsiConsole.MarkupLine($"[blue]Splitting frontmatter from body[/]");

        var frontMatterBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        if (frontMatterBlock == null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] No YAML front matter found.");
            return null;
        }

        AnsiConsole.MarkupLine($"[green]Passed YamlFrontMatterBlocks[/]");
        var frontmatter = frontMatterBlock.Lines.ToString();
        if (string.IsNullOrEmpty(frontmatter))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Empty YAML front matter.");
            return null;
        }

        AnsiConsole.MarkupLine($"[green]Passed frontmatter[/]");
        var deserializer = new DeserializerBuilder().Build();
        var metadata = deserializer.Deserialize<Dictionary<string, object>>(frontmatter);
        if (!metadata.ContainsKey("uri"))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Invalid or missing 'uri' in metadata.");
            return null;
        }

        AnsiConsole.MarkupLine($"[green]Passed metadata[/]");

        AnsiConsole.MarkupLine($"[blue]Removing frontmatter[/]");
        var justContent = GetContentWithoutFrontmatter(markdown, frontMatterBlock);

        AnsiConsole.MarkupLine($"[blue]Splitting Blurb [/]");
        var blurb = GetBlurbContent(justContent);
        var (_, blurbOutPath) = CreateBlurbMdoc(metadata, settings);

        AnsiConsole.MarkupLine($"[blue]Splitting Body [/]");
        var body = GetBodyContent(justContent);
        var (_, bodyOutPath) = CreateIndexMdoc(metadata, settings);

        return new MarkdownProperties()
        {
            Blurb = new BlurbMarkdown
            {
                Content = blurb,
                OutPath = blurbOutPath
            },
            Body = new BodyMarkdown
            {
                Content = body,
                OutPath = bodyOutPath
            },
            Frontmatter = Frontmatter.Parse(metadata),
            Metadata = metadata
        };
    }
}
