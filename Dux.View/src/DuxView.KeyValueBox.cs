using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Limcap.Duxtools {

	public static partial class DuxView {
		public class KeyValueBox : DockPanel {

			public readonly Dux dux;
			public readonly TextBlock keyBox;
			public readonly ValueBox valueBox;

			public event Action<KeyValueBox> OnFocus;
			public event Action<KeyValueBox> OnChange;
			public event Action<KeyValueBox> Setup;

			public object customRef;




			public KeyValueBox( char kind, string key, string value, object customRef = null )
			: this( new DuxValue( key, value, kind ), customRef ) { }




			public KeyValueBox( DuxValue duxValue, object customRef = null ) {
				this.dux = duxValue;
				this.customRef = customRef;

				keyBox = new TextBlock() { Text = duxValue.Key };
				var kind = duxValue.Props is null || duxValue.Props.Count == 0 ? 't' : duxValue.Props[0];
				valueBox = new ValueBox( kind, duxValue.AsString() );
				valueBox.OnFocus += ( vb ) => {
					OnFocus?.Invoke( this );
				};
				valueBox.OnValueChange += ( vb ) => {
					duxValue.SetContent( vb.Value );
					OnChange?.Invoke( this );
				};

				Children.Add( keyBox );
				Children.Add( valueBox );

				SetupStyle();
				Setup?.Invoke( this );
				Update();
			}




			private void SetupStyle() {
				LastChildFill = true;
				Margin = itemMargin;

				keyBox.Margin = keyMargin;
				keyBox.Width = 150;

				valueBox.MaxHeight = 300;
			}




			public void SetToolTip( string tooltip ) {
				throw new NotImplementedException();
			}




			public void SetValueBoxBackground( SolidColorBrush solidColorBrush ) {
				throw new NotImplementedException();
			}




			public void SetOnValueChange( Action onChange ) {
				throw new NotImplementedException();
			}




			public KeyValueBox Update() {
				// Atualiza os dados dos componentes da view
				keyBox.Text = dux.Key;
				valueBox.Value = dux.AsString();

				// Atualizada o estilo dos componentes da view
				keyBox.TextDecorations = dux.Props.Contains( Mod.DISABLED ) ? TextDecorations.Strikethrough : null;
				//valueBox.IsEnabled = !dataItem.HasMod( ParamList.Mod.DISABLED );
				valueBox.Opacity = dux.Props.Contains( Mod.DISABLED ) ? 0.5 : 1;

				return this;
			}




			public KeyValueBox SetFocus() {
				try {
					//textBox.Focus();
					valueBox.Focus();
					OnFocus?.Invoke( this );
				}
				catch { }
				return this;
			}




			public KeyValueBox SetData( string key = null, string value = null ) {
				dux.SetKey( key ).SetContent( value );
				return this;
			}




			public bool HasMod( char p ) {
				return dux.Props.Contains( p );
			}




			public KeyValueBox SetProps( params char[] props ) {
				dux.Props = props.ToList();
				return this;
			}




			public KeyValueBox UnsetMod( params char[] props ) {
				foreach (var p in props)
					if (dux.Props.Contains( p ))
						dux.Props.Remove( p );
				return this;
			}




			public KeyValueBox ToggleMod( char m ) {
				if (dux.Props.Contains( m )) dux.Props.Remove( m );
				else dux.Props.Add( m );
				return this;
			}




			public override string ToString() {
				return dux.ToString();
			}
		}
	}
}