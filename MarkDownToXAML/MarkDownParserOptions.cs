using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkDownToXAML;

public class MarkDownParserOptions
{
	public static readonly MarkDownParserOptions Default = new();

	public string ImageFolder { get; set; } = ".";
}
