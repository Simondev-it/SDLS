using System.Text.RegularExpressions;

namespace SDLS.Services.Utilities;

public static class HtmlContentParser
{
    private static readonly Regex ImgSrcRegex = new(
        "<img\\b[^>]*?\\bsrc\\s*=\\s*(?:\"(?<src>[^\"]+)\"|'(?<src>[^']+)'|(?<src>[^\\s>]+))",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static List<string> ExtractImageUrls(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return new List<string>();

        var result = new List<string>();

        foreach (Match match in ImgSrcRegex.Matches(html))
        {
            var raw = match.Groups["src"].Value;
            var normalized = NormalizeUrl(raw);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                result.Add(normalized);
            }
        }

        return result
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string? NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var trimmed = url.Trim();
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            return null;

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            return null;

        var builder = new UriBuilder(uri)
        {
            Query = string.Empty,
            Fragment = string.Empty
        };

        return builder.Uri.AbsoluteUri.TrimEnd('/');
    }

    public static string ResolveImageNameFromUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return "lesson-image";

        var fileName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(fileName))
            return "lesson-image";

        return fileName;
    }
}
