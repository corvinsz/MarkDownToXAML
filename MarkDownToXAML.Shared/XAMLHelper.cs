using System.Windows;
using System.Windows.Media;

namespace MarkDownToXAML.Shared;

public static class XAMLHelper
{
	public static T LoadXaml<T>(string xaml)
	{
		using var stringReader = new System.IO.StringReader(xaml);
		using var xmlReader = System.Xml.XmlReader.Create(stringReader);
		return (T)System.Windows.Markup.XamlReader.Load(xmlReader);
	}

	public static T? FindChild<T>(this DependencyObject parent, string childName)
		   where T : DependencyObject
	{
		// Confirm parent and childName are valid. 
		if (parent == null) return null;

		T? foundChild = null;

		int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < childrenCount; i++)
		{
			var child = VisualTreeHelper.GetChild(parent, i);
			// If the child is not of the request child type child
			T? childType = child as T;
			if (childType is null)
			{
				// recursively drill down the tree
				foundChild = FindChild<T>(child, childName);

				// If the child is found, break so we do not overwrite the found child. 
				if (foundChild is not null) break;
			}
			else if (!string.IsNullOrEmpty(childName))
			{
				// If the child's name is set for search
				if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
				{
					// if the child's name is of the request name
					foundChild = (T)child;
					break;
				}
			}
			else
			{
				// child element found.
				foundChild = (T)child;
				break;
			}
		}

		return foundChild;
	}
	public static T? FindChild<T>(this DependencyObject depObj) where T : DependencyObject
	{
		if (depObj is null) return null;

		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
		{
			var child = VisualTreeHelper.GetChild(depObj, i);

			var result = (child as T) ?? FindChild<T>(child);
			if (result is not null) return result;
		}
		return null;
	}
	/// <summary>
	/// Finds the fist Parent of a given item in the visual tree. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="current"></param>
	/// <returns></returns>
	public static T? FindParent<T>(DependencyObject current) where T : DependencyObject
	{
		do
		{
			if (current is T ancestor)
			{
				return ancestor;
			}
			current = VisualTreeHelper.GetParent(current);
		} while (current is not null);

		return null;
	}
}
