using Limcap.Extensions.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Limcap.Duxtools {
	public partial class DuxView {

		public class ListBox : StackPanel {
			public DuxNamedList duxList { get; private set; }
			public event Action<KeyValueBox> OnItemSelected;
			//public new List<KeyValueBox> Children { get => base.Children.Cast<KeyValueBox>().ToList(); }

			public KeyValueBox SelectedItem { get; protected set; }
			public int SelectedIndex { get => Children.IndexOf( SelectedItem ); }








			public ListBox( DuxNamedList datamodel = null, Func<UIElement[]> customLoader = null ) {
				Margin = new Thickness( 5, 5, 5, 0 );
				duxList = datamodel ?? new DuxNamedList( null );
				CustomLoader = customLoader;
				Reload();
			}








			public void Reload() {
				base.Children.Clear();
				var elements = CustomLoader is null ? DefaultLoader() : CustomLoader();
				AddMany( elements );
			}








			/// <summary>
			/// Can be set by the user to tell this class how to rebuild its children.
			/// <remarks>
			/// When <see cref="duxList"/> is updated in a way that the instences contained in <see cref="Children"/> are
			/// replaced, the view needs to be rebuilt. If this field is set, it will be used to generate the children.
			/// If it is not set, the default constructor will be used.
			/// </remarks>
			/// </summary>
			public Func<UIElement[]> CustomLoader;








			//[MethodImpl( MethodImplOptions.Synchronized )]
			/// <summary>
			/// Constrói todos os elementos (<see cref="KeyValueBox"/>) da view (<see cref="ListBox"/>),
			/// baseado no modelo de dados (<see cref="DuxNamedList"/>) definido no campo <see cref="duxList"/>.
			/// É executado imetiatamente ao construir o objeto. Pode ser chamado posteriormente se houver necessidade
			/// de reconstruir o objeto inteiro.
			/// </summary>
			private UIElement[] DefaultLoader() {
				var elements = new List<UIElement>();
				StackPanel parentPanel = null;
				StackPanel currentPanel = null;
				foreach (Dux dux in duxList.Children) {
					if (!(dux is DuxValue duxval)) continue;
					if (duxval.HasProp( '{' ) ) {
						StartGroup( duxval.Key, duxval );
					}
					else if (duxval.HasProp( '}' ) ) {
						EndGroup();
					}
					else if (duxval.HasProp('-') ) {
						Add( NewTitle( duxval.Key ) );
						//Add( new Separator() { Margin = new Thickness( 5 ) } );
					}
					else if (duxval.Key is null || duxval.Key.Trim() == "") {
						Add( new Border() { Margin = new Thickness( 5 ) } );
					}
					else {
						var valueView = new KeyValueBox( duxval );
						valueView.OnFocus += this.DefineSelectedItem;
						Add( valueView );
					}
				}
				return elements.ToArray();

				void Add( UIElement item ) {
					if (currentPanel is null) elements.Add( item );
					else currentPanel.Children.Add( item );
				}

				void StartGroup( string header, string tooltip ) {
					var group = new StackPanel() { Orientation = Orientation.Vertical };
					group.Children.Add( NewTitle( header ) );
					var panel = new StackPanel() { Margin = new Thickness( 20, 0, 0, 10 ) };
					group.Children.Add( panel );
					if (tooltip != null && tooltip.Trim().Length > 0) group.ToolTip = tooltip;
					Add( group );
					parentPanel = currentPanel; currentPanel = panel;
				}

				void EndGroup() {
					currentPanel = parentPanel;
					parentPanel = null;
				}

				Panel NewTitle( string titleText ) {
					var titlePanel = new DockPanel() {
						LastChildFill = true,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						//Orientation = Orientation.Horizontal,
						//Background = new SolidColorBrush( Color.FromArgb( 15, 0, 0, 0 ) ),
						Margin = new Thickness( 0, elements.Count == 0 ? 0 : 20, 0, 10 )
					};
					if (!string.IsNullOrEmpty( titleText )) {
						var label = new Label() {
							Content = titleText,
							FontWeight = FontWeight.FromOpenTypeWeight( 600 ),
							Padding = new Thickness( 0,0,5,0 )
						};
						DockPanel.SetDock( label, Dock.Left );
						label.FontSize *= 1.25;
						titlePanel.Children.Add( label );
					}
					var line = new Separator() {
						BorderThickness = new Thickness( 2 ),
						BorderBrush = Brushes.DarkGray,
						Height = 2,
						Margin = new Thickness( 0, 2, 0, 0 )
					};
					titlePanel.Children.Add( line );
					DockPanel.SetDock( line, Dock.Right );
					return titlePanel;
				}
			}








			public void Refresh() {
				if (duxList.Children is null) return;
				foreach (UIElement item in base.Children) {
					if (!(item is KeyValueBox duxval)) continue;
					duxval.Update();
				}
			}






			public void DefineSelectedItem( KeyValueBox item ) {
				SelectedItem = item;
				OnItemSelected?.Invoke( item );
			}
			public void DefineSelectedItem( int index ) {
				DefineSelectedItem( Children[index] as KeyValueBox );
			}




			/// <summary>
			/// Adiciona um novo item à View e à estrutura de dados deste ParamList.
			/// </summary>
			public void Add( string key, string value, params char[] props ) {
				Add( new DuxValue( key, value, props ) );
			}




			public void Add( Dux node ) {
				if (node is DuxValue duxValue) {
					var valueView = new KeyValueBox( duxValue );
					valueView.OnFocus += ( o ) => DefineSelectedItem( o );
					duxList.Add( duxValue );
					base.Children.Add( valueView );
				}
				else base.Children.Add( new Separator() );
			}




			public void Add( UIElement element ) {
				if (element is KeyValueBox valueView) {
					valueView.OnFocus += ( o ) => DefineSelectedItem( o );
					if (!base.Children.Contains( valueView )) base.Children.Add( valueView );
					if (!duxList.Contains( valueView.dux )) duxList.Add( valueView.dux );
				}
				else {
					base.Children.Add( element );
					duxList.Add( DuxNull.Singleton );
				}

			}




			public void AddMany( params UIElement[] es ) {
				foreach (var e in es) Add( e );
			}




			public void AddFrom( DuxCollection dataModel, bool ignoreNonExistent, params string[] keys ) {
				foreach (var key in keys) {
					var item = dataModel[key];
					if (!(item is DuxNull)) Add( item );
					else if (!ignoreNonExistent) throw new NullReferenceException( $"A chave {key} não existe na ParamList." );
				}
			}




			public ListBox Remove( KeyValueBox viewitem ) {
				duxList.Remove( viewitem.dux );
				base.Children.Remove( viewitem );
				return this;
			}




			public ListBox MoveSelectedUp() {
				int index = Children.IndexOf( SelectedItem );
				base.Children.MoveUp( index );

				// Para que a sincronização das duas listas fique mais certeira, em vez de invocar também chamar o
				// MoveUp no dataModel, chamamos o Move, especificando qual item e qual index, pois assim tem-se
				// certeza de que está movendo o item certo.
				//_dataModel.MoveUp( index );
				if( SelectedItem != null )
					duxList.Move( SelectedItem.dux, index - 1 );
				return this;
			}




			public ListBox MoveSelectedDown() {
				var index = Children.IndexOf( SelectedItem );
				base.Children.MoveDown( index );

				// Para que a sincronização das duas listas fique mais certeira, em vez de invocar também chamar o
				// MoveDown no dataModel, chamamos o Move, especificando qual item e qual index, pois assim tem-se
				// certeza de que está movendo o item certo.
				//_dataModel.MoveDown( index );
				duxList.Move( SelectedItem.dux, index + 1 );
				return this;
			}
		}
	}
}