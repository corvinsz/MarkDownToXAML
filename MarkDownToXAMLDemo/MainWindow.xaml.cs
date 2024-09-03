using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkDownToXAMLDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		string markDown = File.ReadAllText(@"..\..\..\MDFiles\Example1.md");

		string xaml = MarkDownToXAML.MarkDownParser.Parse(markDown);

		svContent.Content = LoadXaml<StackPanel>(xaml);

		static T LoadXaml<T>(string xaml)
		{
			using (var stringReader = new System.IO.StringReader(xaml))
			using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
				return (T)System.Windows.Markup.XamlReader.Load(xmlReader);
		}
	}
}