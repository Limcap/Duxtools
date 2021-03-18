using DataUtils.BasicExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataUtils //Dform
{
	public static partial class Params
	{
		public class ParamItemView : DockPanel
		{
			public readonly ParamItem paramItem;
			public readonly TextBlock keyBox;
			public readonly ValueBox valueBox;

			public event Action<ParamItemView> OnFocus;
			public event Action<ParamItemView> OnChange;
			public event Action<ParamItemView> Setup;

			public object customRef;




			public ParamItemView( char kind, string key, string value, object customRef=null )
			: this( new ParamItem( key, value, kind ), customRef )
			{ }




			public ParamItemView( ParamItem paramItem, object customRef=null )
			{
				this.paramItem = paramItem;
				this.customRef = customRef;

				keyBox = new TextBlock() { Text = paramItem.Key };
				var kind = paramItem.mods is null || paramItem.mods.Count == 0 ? 't' : paramItem.mods[0];
				valueBox = new ValueBox( kind, paramItem.Value );
				valueBox.OnFocus += ( vb ) => {
					OnFocus?.Invoke( this );
				};
				valueBox.OnValueChange += ( vb ) => {
					paramItem.Value = vb.Value;
					OnChange?.Invoke( this );
				};

				Children.Add( keyBox );
				Children.Add( valueBox );

				SetupStyle();
				Setup?.Invoke( this );
				Update();
			}




			private void SetupStyle()
			{
				LastChildFill = true;
				Margin = itemMargin;

				keyBox.Margin = keyMargin;
				keyBox.Width = 150;

				valueBox.MaxHeight = 300;
			}




			public void SetToolTip( string tooltip )
			{
				throw new NotImplementedException();
			}




			public void SetValueBoxBackground( SolidColorBrush solidColorBrush )
			{
				throw new NotImplementedException();
			}




			public void SetOnValueChange( Action onChange )
			{
				throw new NotImplementedException();
			}




			public ParamItemView Update()
			{
				// Atualiza os dados dos componentes da view
				keyBox.Text = paramItem.Key;
				valueBox.Value = paramItem.Value;

				// Atualizada o estilo dos componentes da view
				keyBox.TextDecorations = paramItem.HasMod( Params.Mod.DISABLED ) ? TextDecorations.Strikethrough : null;
				//valueBox.IsEnabled = !dataItem.HasMod( ParamList.Mod.DISABLED );
				valueBox.Opacity = paramItem.HasMod( Params.Mod.DISABLED ) ? 0.5 : 1;

				return this;
			}




			public ParamItemView SetFocus()
			{
				try {
					//textBox.Focus();
					valueBox.Focus();
					OnFocus?.Invoke( this );
				}
				catch { }
				return this;
			}




			public ParamItemView SetData( string key = null, string value=null )
			{
				paramItem.SetData( key, value );
				return this;
			}




			public bool HasMod( char p )
			{
				return paramItem.HasMod( p );
			}




			public ParamItemView SetMods( params char[] mods )
			{
				paramItem.SetMods( mods );
				return this;
			}




			public ParamItemView UnsetMod( params char[] mods )
			{
				paramItem.UnsetMod( mods );
				return this;
			}




			public ParamItemView ToggleMod( char m )
			{
				paramItem.ToggleMod( m );
				return this;
			}




			public override string ToString()
			{
				return paramItem.ToString();
			}
		}
	}
}
