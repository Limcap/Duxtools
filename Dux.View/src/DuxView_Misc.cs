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

namespace Limcap.Duxtools {

	public partial class DuxView {
		public static readonly Thickness keyMargin = new Thickness( 0, 0, 10, 0 );
		public static readonly Thickness itemMargin = new Thickness( 0, 0, 0, 2 );
		public static readonly SolidColorBrush colorSystemField = new SolidColorBrush( Color.FromRgb( 255, 250, 225 ) );
		public static readonly SolidColorBrush colorImmutable = new SolidColorBrush( Color.FromRgb( 220, 45, 45 ) );
		public static readonly SolidColorBrush colorInvisible = new SolidColorBrush( Color.FromRgb( 165, 198, 195 ) ); // Brushes.LightGray;
		public static readonly SolidColorBrush colorDisabled = Brushes.Goldenrod; //new SolidColorBrush( Color.FromRgb( 150, 150, 150 ) );



		public static class Mod {
			public const char DISABLED = '!';
			public const char PASSWORD = '*';
			public const char CHECKBOX = '#';
			public const char RADIO = '@';
		}
	}
}
