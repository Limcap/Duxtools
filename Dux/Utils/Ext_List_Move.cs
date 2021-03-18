using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Limcap.Extensions.List
{
	public static class Ext_List_Move
	{
		public static void MoveUp<T>( this List<T> list, T item )
		{
			list.Move( item, list.IndexOf( item ) - 1 );
		}
		public static void MoveDown<T>( this List<T> list, T item )
		{
			list.Move( item, list.IndexOf( item ) + 1 );
		}
		public static void Move<T>( this List<T> list, T item, int newIndex )
		{
			int oldIndex = list.IndexOf( item );
			if (item == null || list.Count == 0 || oldIndex < 0) return;
			newIndex = newIndex < 0 ? 0 : newIndex >= list.Count ? list.Count - 1 : newIndex;
			list.RemoveAt( oldIndex );
			list.Insert( newIndex, item );
		}








		public static void MoveUp<T>( this List<T> list, int index )
		{
			list.Move( index, index - 1 );
		}
		public static void MoveDown<T>( this List<T> list, int index )
		{
			list.Move( index, index + 1 );
		}
		public static void Move<T>( this List<T> list, int oldIndex, int newIndex )
		{
			if (list.Count == 0 || oldIndex >= list.Count || oldIndex < 0) return;
			newIndex = newIndex < 0 ? 0 : newIndex >= list.Count ? list.Count - 1 : newIndex;
			var item = list[oldIndex];
			list.RemoveAt( oldIndex );
			list.Insert( newIndex, item );
		}








		public static void MoveUp( this UIElementCollection list, int index )
		{
			list.Move( index, index - 1 );
		}
		public static void MoveDown( this UIElementCollection list, int index )
		{
			list.Move( index, index + 1 );
		}
		public static void Move( this UIElementCollection list, int itemIndex, int newIndex )
		{
			if (list.Count == 0 || itemIndex < 0 || itemIndex >= list.Count || newIndex < 0 || newIndex >= list.Count) return;
			var item = list[itemIndex];
			int oldIndex = list.IndexOf( item );
			if (oldIndex == -1) return;
			list.RemoveAt( oldIndex );
			list.Insert( newIndex, item );
		}








		public static void Move( this UIElementCollection list, UIElement item, int newIndex )
		{
			if (item == null || list.Count == 0 || newIndex < 0 || newIndex >= list.Count) return;
			int oldIndex = list.IndexOf( item );
			if (oldIndex == -1) return;
			list.RemoveAt( oldIndex );
			list.Insert( newIndex, item );
		}




		//public static List<T> ToList<T>( this UIElementCollection collection )
		//{
		//	return collection.Cast<T>().ToList();
		//}
	}
}
