namespace MarkDownToXAML.Shared;

public static class XAMLHelper
{
    public static T LoadXaml<T>(string xaml)
    {
        using (var stringReader = new System.IO.StringReader(xaml))
        using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
            return (T)System.Windows.Markup.XamlReader.Load(xmlReader);
    }
}