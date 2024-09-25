using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace MarkDownToXAMLDemo;
public partial class MainWindowViewModel : ObservableObject
{
	public MainWindowViewModel()
	{
		MarkdownText = File.ReadAllText(@"..\..\..\MDFiles\Example1.md");
	}

	[ObservableProperty]
	private bool _isMarkdownValid;

	[ObservableProperty]
	private string _markdownText;

	partial void OnMarkdownTextChanged(string value)
	{
		try
		{
			string xaml = MarkDownToXAML.MarkDownParser.Parse(value);

			GeneratedXAML = MarkDownToXAML.Shared.XAMLHelper.LoadXaml<StackPanel>(xaml);
			GeneratedXAMLCode = xaml;
			IsMarkdownValid = true;
		}
		catch (Exception ex)
		{
			IsMarkdownValid = false;
		}
	}

	[ObservableProperty]
	private object _generatedXAML;

	[ObservableProperty]
	private string _generatedXAMLCode;
}
