using DataUtils.BasicExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataUtils//Dform
{
	public static partial class Params
	{
		public class ParamListView : StackPanel //RawList<ParamListGUI.Item>
		{
			public Adict _theParamList { get; private set; }
			public event Action<ParamItemView> OnItemSelected;

			//public new List<Item> Children { get; } = new List<Item>();
			//private List<Item> Children => Children.Cast<Item>().ToList();
			public new List<ParamItemView> Children { get => base.Children.Cast<ParamItemView>().ToList(); }

			public ParamItemView SelectedItem { get; protected set; }
			public int SelectedIndex { get => Children.IndexOf( SelectedItem ); }




			public ParamListView( Adict datamodel = null )
			{
				_theParamList = datamodel ?? new Adict();
				Margin = new Thickness( 5, 5, 5, 0 );
				Refresh();
			}




			//[MethodImpl( MethodImplOptions.Synchronized )]
			/// <summary>
			/// Constrói todos os elementos (<see cref="ParamItemView"/>) da view (<see cref="Params"/>),
			/// baseado no modelo de dados (<see cref="Params"/>) definido no campo <see cref="_theParamList"/>.
			/// É executado imetiatamente ao construir o objeto. Pode ser chamado posteriormente se houver necessidade
			/// de reconstruir o objeto inteiro.
			/// </summary>
			public void Refresh()
			{
				base.Children.Clear();
				foreach (Dux n in _theParamList) Add( n );
			}




			public void DefineSelectedItem( ParamItemView item )
			{
				SelectedItem = item;
				OnItemSelected?.Invoke( item );
			}
			public void DefineSelectedItem( int index )
			{
				DefineSelectedItem( Children[index] );
			}




			/// <summary>
			/// Adiciona um novo item à View e à estrutura de dados deste ParamList.
			/// </summary>
			public void Add( string key, string value, params char[] mods )
			{
				Add( new AdictTerm( key, value, mods ) );
			}




			public void Add( Dux node )
			{
				if( node is AdictTerm leaf ) {
					var leafGui = new ParamItemView( leaf );
					leafGui.OnFocus += (o) => DefineSelectedItem( o );
					_theParamList.Add( leaf );
					base.Children.Add( leafGui );
				}
				else base.Children.Add( new Separator() );
			}




			public void Add( UIElement element )
			{
				if( element is ParamItemView leafGui) {
					leafGui.OnFocus += ( o ) => DefineSelectedItem( o );
					if (!base.Children.Contains( leafGui )) base.Children.Add( leafGui );
					if (!_theParamList.Contains( leafGui.paramItem )) _theParamList.Add( leafGui.paramItem );
				}
				else {
					base.Children.Add( element );
					_theParamList.AddSpacer();
				}

			}




			public void AddMany( params UIElement[] es )
			{
				foreach (var e in es) Add( e );
			}




			public void AddFrom( Adict dataModel, bool ignoreNonExistent, params string[] keys )
			{
				foreach (var key in keys) {
					var item = dataModel.GetItem( key );
					if (item != null) Add( item );
					else if (!ignoreNonExistent) throw new NullReferenceException( $"A chave {key} não existe na ParamList." );
				}
			}




			public ParamListView Remove( ParamItemView viewitem )
			{
				_theParamList.Remove( viewitem.paramItem );
				base.Children.Remove( viewitem );
				return this;
			}




			public ParamListView MoveSelectedUp()
			{
				int index = Children.IndexOf( SelectedItem );
				base.Children.MoveUp( index );

				// Para que a sincronização das duas listas fique mais certeira, em vez de invocar também chamar o
				// MoveUp no dataModel, chamamos o Move, especificando qual item e qual index, pois assim tem-se
				// certeza de que está movendo o item certo.
				//_dataModel.MoveUp( index );
				_theParamList.Move( SelectedItem.paramItem, index - 1 );
				return this;
			}




			public ParamListView MoveSelectedDown()
			{
				var index = Children.IndexOf( SelectedItem );
				base.Children.MoveDown( index );

				// Para que a sincronização das duas listas fique mais certeira, em vez de invocar também chamar o
				// MoveDown no dataModel, chamamos o Move, especificando qual item e qual index, pois assim tem-se
				// certeza de que está movendo o item certo.
				//_dataModel.MoveDown( index );
				_theParamList.Move( SelectedItem.paramItem, index + 1 );
				return this;
			}
		}
	}
}
