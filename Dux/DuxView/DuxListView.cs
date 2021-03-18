using Limcap.Extensions.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Limcap.Dux {

	public static partial class DuxView {

		public class DuxListView : StackPanel
		{
			public DuxNamedList duxList { get; private set; }
			public event Action<DuxValueView> OnItemSelected;
			public new List<DuxValueView> Children { get => base.Children.Cast<DuxValueView>().ToList(); }

			public DuxValueView SelectedItem { get; protected set; }
			public int SelectedIndex { get => Children.IndexOf( SelectedItem ); }








			public DuxListView( DuxNamedList datamodel = null, Func<UIElement[]> customLoader = null ) {
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
			/// Constrói todos os elementos (<see cref="DuxValueView"/>) da view (<see cref="DuxListView"/>),
			/// baseado no modelo de dados (<see cref="DuxNamedList"/>) definido no campo <see cref="duxList"/>.
			/// É executado imetiatamente ao construir o objeto. Pode ser chamado posteriormente se houver necessidade
			/// de reconstruir o objeto inteiro.
			/// </summary>
			private UIElement[] DefaultLoader() {
				var elements = new List<UIElement>();
				//if (duxList.Children is null) return elements;
				foreach (Dux dux in duxList.Children) {
					if (!(dux is DuxValue duxval)) continue;
					var valueView = new DuxValueView( duxval );
					valueView.OnFocus += DefineSelectedItem;
					elements.Add( valueView );
					//base.Children.Add( valueView );
				}
				return elements.ToArray();
			}








			public void Refresh() {
				if (duxList.Children is null) return;
				foreach (UIElement item in base.Children) {
					if (!(item is DuxValueView duxval)) continue;
					duxval.Update();
				}
			}






			public void DefineSelectedItem( DuxValueView item ) {
				SelectedItem = item;
				OnItemSelected?.Invoke( item );
			}
			public void DefineSelectedItem( int index ) {
				DefineSelectedItem( Children[index] );
			}




			/// <summary>
			/// Adiciona um novo item à View e à estrutura de dados deste ParamList.
			/// </summary>
			public void Add( string key, string value, params char[] props ) {
				Add( new DuxValue( key, value, props ) );
			}




			public void Add( Dux node ) {
				if (node is DuxValue duxValue) {
					var valueView = new DuxValueView( duxValue );
					valueView.OnFocus += ( o ) => DefineSelectedItem( o );
					duxList.Add( duxValue );
					base.Children.Add( valueView );
				}
				else base.Children.Add( new Separator() );
			}




			public void Add( UIElement element ) {
				if (element is DuxValueView valueView) {
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




			public DuxListView Remove( DuxValueView viewitem ) {
				duxList.Remove( viewitem.dux );
				base.Children.Remove( viewitem );
				return this;
			}




			public DuxListView MoveSelectedUp() {
				int index = Children.IndexOf( SelectedItem );
				base.Children.MoveUp( index );

				// Para que a sincronização das duas listas fique mais certeira, em vez de invocar também chamar o
				// MoveUp no dataModel, chamamos o Move, especificando qual item e qual index, pois assim tem-se
				// certeza de que está movendo o item certo.
				//_dataModel.MoveUp( index );
				duxList.Move( SelectedItem.dux, index - 1 );
				return this;
			}




			public DuxListView MoveSelectedDown() {
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
