using DataUtils.BasicExtensions;
using DataUtils.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataUtils
{
	public static partial class Params
	{
		public class Spacer : ParamListNode
		{
			public string Key { get; } = null;
			public override string ToString()
			{
				return "----------";
			}
		}
	}
}
