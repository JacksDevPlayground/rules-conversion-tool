using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace rules_conversion_tool.Parsers;

public static partial class Frontmatter
{
    public class Author
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

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
        var uid = UUID().Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "");
        sb.AppendLine($"identifier: {uid}");
        if (metadata.TryGetValue("created", out var value))
        {
            sb.AppendLine($"dateCreated: '{value}'");
        }
        else
        {
            sb.AppendLine($"dateCreated: '{DateTime.UtcNow:O}'");
        }

        sb.AppendLine($"lastUpdated: '{DateTime.UtcNow:O}'");

        if (metadata.TryGetValue("authors", out var authorsObj) && authorsObj is List<object> authorsList)
        {
            if (authorsList.Count > 0)
            {
                sb.AppendLine("acknowledgements:");
                foreach (var authorObj in authorsList)
                {
                    if (authorObj is Dictionary<object, object> authorDict)
                    {
                        var name = authorDict.ContainsKey("title") ? authorDict["title"]?.ToString() : string.Empty;
                        var url = authorDict.ContainsKey("url") ? authorDict["url"]?.ToString() : string.Empty;

                        sb.AppendLine($"  - name: {name}");
                        sb.AppendLine($"    url: {url}");
                    }
                }
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

        if (metadata.TryGetValue("archivedreason", out var archivedReason) && archivedReason is not null)
        {
            sb.AppendLine($"archive: {archivedReason}");
        }

        sb.AppendLine("---");
        return sb.ToString();
    }

    [GeneratedRegex("[/+=]")]
    private static partial Regex UUID();
}