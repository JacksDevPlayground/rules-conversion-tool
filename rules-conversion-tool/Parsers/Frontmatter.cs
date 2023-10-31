using System.Text;

namespace rules_conversion_tool.Parsers;

public static class Frontmatter
{
    private static List<Dictionary<string, object>> SafeCastToListOfDictionaries(object obj)
    {
        if (obj is List<object> list)
        {
            return list.OfType<Dictionary<string, object>>().ToList();
        }

        return new List<Dictionary<string, object>>();
    }

    public static string Parse(Dictionary<string, object> metadata)
    {
        var sb = new StringBuilder();
        sb.AppendLine("---");
        sb.AppendLine($"title: {metadata["title"]}");
        sb.AppendLine($"identifier: {Guid.NewGuid().ToString()[..20]}");
        if (metadata.TryGetValue("created", out var value))
        {
            sb.AppendLine($"dateCreated: '{value}'");
        }
        else
        {
            sb.AppendLine($"dateCreated: '{DateTime.UtcNow:O}'");
        }

        sb.AppendLine($"lastUpdated: '{DateTime.UtcNow:O}'");

        if (metadata.ContainsKey("authors") && SafeCastToListOfDictionaries(metadata["authors"]).Count > 0)
        {
            var authors = SafeCastToListOfDictionaries(metadata["authors"]);
            sb.AppendLine("acknowledgements:");
            foreach (var author in authors)
            {
                sb.AppendLine($"  - name: {author["title"]}");
                sb.AppendLine($"    url: {author["url"]}");
            }
        }

        // Handle 'related' section
        if (metadata.ContainsKey("related") && metadata["related"] is List<object> { Count: > 0 } relatedList)
        {
            sb.AppendLine("related:");
            foreach (var relatedItem in relatedList)
            {
                sb.AppendLine($"  - {relatedItem}");
            }
        }
        else
        {
            sb.AppendLine("related: []");
        }

        // Handle 'redirects' section
        if (metadata.ContainsKey("redirects") && metadata["redirects"] is List<object> { Count: > 0 } redirectsList)
        {
            sb.AppendLine("redirects:");
            foreach (var redirect in redirectsList)
            {
                sb.AppendLine($"  - {redirect}");
            }
        }
        else
        {
            sb.AppendLine("redirects: []");
        }

        sb.AppendLine("---");
        return sb.ToString();
    }
}