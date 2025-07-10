using CommunityToolkit.Mvvm.ComponentModel;
using MarkDownToXAML;
using System.IO;
using System.Windows;
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
		MarkDownParserOptions options = new MarkDownParserOptions
		{
			ImageFolder = @"..\..\..\Images"
		};

		try
		{
			string xaml = MarkDownToXAML.MarkDownParser.Parse(value, options);

			GeneratedXAML = MarkDownToXAML.Shared.XAMLHelper.LoadXaml<StackPanel>(xaml);
			GeneratedXAMLCode = xaml;
			IsMarkdownValid = true;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString());
			IsMarkdownValid = false;
		}
	}

	[ObservableProperty]
	private object _generatedXAML;

	[ObservableProperty]
	private string _generatedXAMLCode;
}
