using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkDownToXAMLDemo;
public static class XAMLHelper
{
    public static T LoadXaml<T>(string xaml)
    {
        using (var stringReader = new System.IO.StringReader(xaml))
        using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
            return (T)System.Windows.Markup.XamlReader.Load(xmlReader);
    }
}
