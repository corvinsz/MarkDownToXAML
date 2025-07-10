using System.Text.RegularExpressions;
using System.Text;

namespace MarkDownToXAML;

public static class MarkDownParser
{
	private const string _separator = """  <Border BorderThickness="0,1,0,0" BorderBrush="White" Opacity="0.1" />""";

	public static string Parse(string markdown, MarkDownParserOptions? options = null)
	{
		options ??= MarkDownParserOptions.Default;

		var xamlBuilder = new StringBuilder();
		xamlBuilder.Append("""<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>\n""");

		// Split markdown into lines
		var lines = markdown.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

		foreach (string line in lines)
		{
			// Headers
			if (line.StartsWith("### "))
			{
				xamlBuilder.AppendLine($"""  <TextBlock FontSize="18" FontWeight="Bold">{EscapeXaml(TrimMarkdownSyntax(line, "###"))}</TextBlock>""");
				xamlBuilder.AppendLine(_separator);
			}
			else if (line.StartsWith("## "))
			{
				xamlBuilder.AppendLine($"""  <TextBlock FontSize="24" FontWeight="Bold">{EscapeXaml(TrimMarkdownSyntax(line, "##"))}</TextBlock>""");
				xamlBuilder.AppendLine(_separator);
			}
			else if (line.StartsWith("# "))
			{
				xamlBuilder.AppendLine($"""  <TextBlock FontSize="32" FontWeight="Bold">{EscapeXaml(TrimMarkdownSyntax(line, "#"))}</TextBlock>""");
				xamlBuilder.AppendLine(_separator);
			}
			// Checkboxes
			else if (line.StartsWith("- [ ]"))
			{
				xamlBuilder.AppendLine($"""  <CheckBox Content="{EscapeXaml(TrimMarkdownSyntax(line, "- [ ]"))}" IsChecked="False" />""");
			}
			else if (line.StartsWith("- [x]") || line.StartsWith("- [X]"))
			{
				xamlBuilder.AppendLine($"""  <CheckBox Content="{EscapeXaml(TrimMarkdownSyntax(line, "- [x]", "- [X]"))}" IsChecked="True" />""");
			}
			// Images
			else if (line.StartsWith("!["))
			{
				xamlBuilder.AppendLine(ParseImage(line, options));
			}
			// Bold
			else if (line.Contains("**"))
			{
				xamlBuilder.AppendLine($"""  <TextBlock>{ParseInlineElements(line)}</TextBlock>""");
			}
			// Italic
			else if (line.Contains("*"))
			{
				xamlBuilder.AppendLine($"""  <TextBlock>{ParseInlineElements(line)}</TextBlock>""");
			}
			// Regular lists
			else if (line.StartsWith("- "))
			{
				xamlBuilder.AppendLine($"""  <TextBlock Text="• {EscapeXaml(TrimMarkdownSyntax(line, "-"))}" />""");
			}
			// Regular paragraph
			else
			{
				xamlBuilder.AppendLine($"""  <TextBlock>{EscapeXaml(line)}</TextBlock>""");
			}
		}

        xamlBuilder.Append("</StackPanel>");
        return xamlBuilder.ToString();
    }

    private static string TrimMarkdownSyntax(string line, params string[] syntax)
    {
        foreach (var syn in syntax)
        {
            if (line.StartsWith(syn))
            {
                return line.Substring(syn.Length).Trim();
            }
        }
        return line.Trim();
    }

    private static string EscapeXaml(string text)
    {
        return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

	private static string ParseInlineElements(string line)
	{
		// Bold **text**
		line = Regex.Replace(line, @"\*\*(.*?)\*\*", "<Run FontWeight=\"Bold\">$1</Run>");
		// Italic *text*
		line = Regex.Replace(line, @"\*(.*?)\*", "<Run FontStyle=\"Italic\">$1</Run>");
		return line;
	}

	private static string ParseImage(string line, MarkDownParserOptions options)
	{
		// Match the Markdown image syntax ![alt text](image-url)
		var match = Regex.Match(line, @"!\[(.*?)\]\((.*?)\)");
		if (match.Success)
		{
			string altText = EscapeXaml(match.Groups[1].Value);
			string imageUrl = EscapeXaml(match.Groups[2].Value);

			if (options.ImageFolder is not null)
			{
				imageUrl = Path.Combine(options.ImageFolder, imageUrl);
			}

			if (File.Exists(imageUrl) == false)
			{
				return $"""<TextBlock Text="{altText}" FontStyle="Italic" FontSize="12" Foreground="Gray" />""";
			}

			// Return an Image XAML element
			return $"""<Image Source="{imageUrl}" Stretch="Uniform" Height="200" Width="400" />""";
		}

		// If no image syntax found, throw exception
		throw new InvalidDataException($"Invalid image syntax in line: {line}. Basic markdown syntax is expected.");
	}
}
