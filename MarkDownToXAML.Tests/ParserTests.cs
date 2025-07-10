using MarkDownToXAML.Shared;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MarkDownToXAML.Tests;

public class ParserTests
{
    private static StackPanel ParseToStackPanel(string markdown)
    {
        string parsedXAML = MarkDownToXAML.MarkDownParser.Parse(markdown);
        return MarkDownToXAML.Shared.XAMLHelper.LoadXaml<StackPanel>(parsedXAML);
    }

    [WpfTheory]
    [InlineData("# Hello World1", "Hello World1", 32)]
    [InlineData("## Hello World2", "Hello World2", 24)]
    [InlineData("### Hello World3", "Hello World3", 18)]
    public void Parse_ParsesHeader_ToTextBlock(string markdownText, string expectedText, int expectedFontSize)
    {
        // Arrange
        FontWeight expectdFontWeight = FontWeights.Bold;

        // Act
        StackPanel stackPanel = ParseToStackPanel(markdownText);

        // Assert
        TextBlock tb = stackPanel.FindChild<TextBlock>();
        Assert.Equal(expectedText, tb.Text);
        Assert.Equal(expectedFontSize, tb.FontSize);
        Assert.Equal(expectdFontWeight, tb.FontWeight);
    }

    [WpfTheory]
    [InlineData("- [ ] To Do1", "To Do1", false)]
    [InlineData("- [x] To Do2", "To Do2", true)]
    [InlineData("- [X] To Do3", "To Do3", true)]
    public void Parse_ParsesSquareBrackets_ToCheckBox(string markdownText, string expectedCheckBoxText, bool expectedIsChecked)
    {
        // Arrange

        // Act
        StackPanel stackPanel = ParseToStackPanel(markdownText);

        // Assert
        CheckBox cb = stackPanel.FindChild<CheckBox>();
        Assert.Equal(expectedCheckBoxText, cb.Content);
        Assert.Equal(expectedIsChecked, cb.IsChecked);
    }

    [WpfTheory]
    [InlineData("**some bold text**", "some bold text")]
    public void Parse_ParsesDoubleAsertisks_ToBoldText(string markdownText, string expectedText)
    {
        // Arrange

        // Act
        StackPanel stackPanel = ParseToStackPanel(markdownText);

        // Assert
        TextBlock textBlock = stackPanel.FindChild<TextBlock>();
        var firstRun = (Run)textBlock.Inlines.First();

        Assert.Equal(expectedText, firstRun.Text);
        Assert.Equal(FontWeights.Bold, firstRun.FontWeight);
    }


    [WpfTheory]
    [InlineData("*some italic text*", "some italic text")]
    public void Parse_ParsesSingleAsertisk_ToItalicText(string markdownText, string expectedText)
    {
        // Arrange

        // Act
        StackPanel stackPanel = ParseToStackPanel(markdownText);

        // Assert
        TextBlock textBlock = stackPanel.FindChild<TextBlock>();
        var firstRun = (Run)textBlock.Inlines.First();

        Assert.Equal(expectedText, firstRun.Text);
        Assert.Equal(FontStyles.Italic, firstRun.FontStyle);
    }

    [WpfTheory]
    [InlineData("- Some ToDo Item", "Some ToDo Item")]
    [InlineData("- Item2", "Item2")]
    public void Parse_ParsesListItem_ToTextBlockWithBulletPoint(string markdownText, string expectedText)
    {
        // Arrange

        // Act
        StackPanel stackPanel = ParseToStackPanel(markdownText);

        // Assert
        TextBlock textBlock = stackPanel.FindChild<TextBlock>();

        Assert.StartsWith(SymbolConstants.BulletPoint, textBlock.Text);
        Assert.Contains(expectedText, textBlock.Text);
    }

    [WpfFact]
    public void Parse_ParsesMultipleListItems_ToMultipleTextBlocksWithBulletPoints()
    {
        // Arrange
        string markdownText = """
            - Item 1
            - Item 2
            - Item 3
            """;

        // Act
        StackPanel stackPanel = ParseToStackPanel(markdownText);

        // Assert
        Assert.Equal(3, stackPanel.Children.Count);

        TextBlock textBlock1 = (TextBlock)stackPanel.Children[0];
        Assert.StartsWith(SymbolConstants.BulletPoint, textBlock1.Text);
        Assert.EndsWith("Item 1", textBlock1.Text);

        TextBlock textBlock2 = (TextBlock)stackPanel.Children[1];
        Assert.StartsWith(SymbolConstants.BulletPoint, textBlock2.Text);
        Assert.EndsWith("Item 2", textBlock2.Text);

        TextBlock textBlock3 = (TextBlock)stackPanel.Children[2];
        Assert.StartsWith(SymbolConstants.BulletPoint, textBlock3.Text);
        Assert.EndsWith("Item 3", textBlock3.Text);
    }
}