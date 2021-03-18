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
	public class AdictSpacer : Dux
	{
		public AdictSpacer() : base( null, null )
		{
		}

		public override string Key { get; } = "SPACER";
		public override string ToString()
		{
			return "----------";
		}
	}
}
