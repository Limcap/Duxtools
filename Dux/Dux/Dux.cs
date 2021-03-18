using Limcap.Extensions.List;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Limcap.Dux {

	/// <summary>Composite Object For JSON and XML Data - COJAX</summary>
	/// <version>1.0</version>
	/// <author>LimãoCapeta</author>
	[DebuggerDisplay( "{Preview(), nq}" )]
	public abstract partial class Dux {
		//
		// constructors
		public Dux( string key, object value, params char[] props ) {
			_Key = FixKey( key ); Props = props?.ToList(); _Content = value;
		}
		//
		// key stuff
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		protected string _Key;
		public virtual string Key {
			get => _Key; //private set => _Key = FixKey( value );
		}
		public virtual Dux SetKey( string k ) {
			_Key = FixKey( k ); return this;
		}
		protected virtual string FixKey( string k ) {
			return string.IsNullOrWhiteSpace( k ) ? null : k.Trim();
		}
		public virtual List<char> Props { get; set; }
		//
		// content stuff
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		protected object _Content;
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public virtual object Content { get => _Content; set => _Content = value; }
		public virtual Dux SetContent( object c ) { _Content = c; return this; }
		public abstract void UpdateChildren( Dux otherDux, bool includeNewItems = false );
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public Dux Child { get => _Content is Dux d ? d : null; }
		[DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
		public List<Dux> Children { get => _Content as List<Dux>; }
		//public List<Dux> Children { get => _Content is List<Dux> c ? c : null; }
		//
		// list stuff
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public virtual int Count { get => _Content is IEnumerable<Dux> n ? n.Count() : _Content is Dux ? 0 : -1; }
		public abstract int FindIndex( Predicate<Dux> p );
		public abstract Dux this[int i] { get; set; }
		public abstract Dux this[string k] { get; set; }
		public abstract Dux Add( Dux d );
		public abstract Dux AddRange( List<Dux> ds );
		public abstract Dux AddAndGet( Dux d );
		public virtual Dux CreateIndexedListIfNull() { return this; }
		public virtual Dux AddIfNull( object value ) { return this; }
		//
		// to string suff
		public override string ToString() { return ToString( false ); }
		public abstract string ToString( bool showKey );
		public virtual string Preview() { return ToString( Key != null ); }
		public abstract bool IsEmpty();
		//
		// conversions stuff
		public static implicit operator Dux( string o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( bool o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( int o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( byte o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( short o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( long o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( float o ) { return new DuxValue( null, o, null ); }
		public static implicit operator Dux( double o ) { return new DuxValue( null, o, null ); }
		public static implicit operator string( Dux o ) { return o.ToString( false ); }
		//
		// debug info stuff
#if DEBUG
		// exibe o Type desse Dux. 
		protected Info_Dux_Type infoType => new Info_Dux_Type( this );
#endif
	}












	[DebuggerDisplay( "{Preview(), nq}" )]
	public class DuxNull : Dux {
		//
		// constructor
		public DuxNull() : base( null, null, null ) { }
		public DuxNull( Dux source, string key ) : base( null, null, null ) { _source = source; _absentKey = key; }
		//
		// content stuff
		private Dux _source;
		private string _absentKey;
		public static readonly DuxNull Singleton = new DuxNull();
		public override List<char> Props {
			get => null;
			set { }
		}
		public override Dux SetKey( string k ) {
			throw new DuxOperationNotAllowed( this, "Dux.SetKey" );
		}
		public override object Content {
			set => SetContent( value );
		}
		public override Dux SetContent( object c ) {
			throw new DuxOperationNotAllowed( this, "Dux.Content.set" );
		}
		public override void UpdateChildren( Dux otherDux, bool includeNewItems = false ) {
			throw new DuxOperationNotAllowed( this, "Dux.UpdateItems" );
		}
		public override Dux CreateIndexedListIfNull() {
			return _source.AddAndGet( new DuxIndexedList( _absentKey ) );
		}
		public override Dux AddIfNull( object value ) {
			if (value is DuxValue d) {
				if (string.IsNullOrWhiteSpace( d.Key )) d.SetKey( _absentKey );
				return _source.AddAndGet( d );
			}
			if (value is DuxCollection c) return _source.AddAndGet( c.SetKey( _absentKey ) );
			else return _source.AddAndGet( new DuxValue( _absentKey, value ) );
		}
		//
		// list stuff
		public override int FindIndex( Predicate<Dux> p ) { return -1; }
		public override Dux this[string k] {
			get => Singleton;
			set => throw new DuxOperationNotAllowed( this, "Dux[string].set" );
		}
		public override Dux this[int i] {
			get => Singleton;
			set => throw new DuxOperationNotAllowed( this, "Dux[index].set" );
		}
		public override Dux Add( Dux d ) { throw new DuxOperationNotAllowed( this, "Dux.Add" ); }
		public override Dux AddRange( List<Dux> ds ) { throw new DuxOperationNotAllowed( this, "Dux.AddRange" ); }
		public override Dux AddAndGet( Dux ds ) { throw new DuxOperationNotAllowed( this, "Dux.AddAndGet" ); }
		//
		// to string stuff
		public override string ToString( bool showKey ) { return null; }
		public override bool IsEmpty() { return true; }
	}












	[DebuggerDisplay( "{Preview(), nq}" )]
	public class DuxValue : Dux {
		//
		// constructors
		public DuxValue( string key, object content, params char[] props ) : base( key, content, props ) {
			_Content = FixContent( content );
		}
		//
		// content stuff
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public bool IsPrimitive {
			get => _Content is bool || _Content is byte || _Content is short || _Content is int || _Content is long || _Content is double || _Content is float;
		}
		private object FixContent( object c ) {
			return c is Dux d ? d.ToString( true ) : c;//.Replace( "\"", "\\\"" )
		}
		public override object Content {
			set => SetContent( value );
		}
		public override Dux SetContent( object c ) {
			_Content = FixContent( c ); return this;
		}
		public override void UpdateChildren( Dux otherDux, bool includeNewItems = false ) {
			SetContent( otherDux.Content );
		}
		//
		// list stuff
		public override Dux this[int i] {
			get => throw new DuxOperationNotAllowed( this, "Dux[int].get" );
			set => throw new DuxOperationNotAllowed( this, "Dux[int].set" );
		}
		public override Dux this[string k] {
			get => throw new DuxOperationNotAllowed( this, "Dux[string].get" );
			set => throw new DuxOperationNotAllowed( this, "Dux[string].set" );
		}
		public override int FindIndex( Predicate<Dux> p ) { throw new DuxOperationNotAllowed( this, "Dux.FindIndex" ); }
		public override Dux Add( Dux d ) { throw new DuxOperationNotAllowed( this, "Dux.Add" ); }
		public override Dux AddRange( List<Dux> ds ) { throw new DuxOperationNotAllowed( this, "Dux.AddRange" ); }
		public override Dux AddAndGet( Dux ds ) { throw new DuxOperationNotAllowed( this, "Dux.AddAndGet" ); }
		//
		// to string stuff
		public override string Preview() { return ToString( true ); }
		public override string ToString( bool showKey ) {
			showKey = showKey && Key != null;
			var k = showKey ? $"\"{Key}\" : " : "";
			if (IsPrimitive) return $"{k}{Content.ToString().ToLower()}";
			return showKey ? $"{k}\"{Content}\"" : $"{Content}";
		}
		public override bool IsEmpty() { return _Content is null || (_Content is string s && string.IsNullOrEmpty( s )); }
		//
		// debug info stuff
#if DEBUG
		[DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
		protected Info_DuxValue_Content infoContent => new Info_DuxValue_Content( this );
#endif
	}












	[DebuggerDisplay( "{Preview(), nq}" )]
	public abstract class DuxCollection : Dux, IEnumerable<Dux> {
		//
		// conostructors
		public DuxCollection( string key, params char[] props ) : this( key, null, props ) { }
		public DuxCollection( string key, List<Dux> value, params char[] props ) : base( key, value, props ) {
			if (_Content is null) _Content = new List<Dux>( 10 );
		}
		//
		// content stuff
		public override object Content {
			set => SetContent( value );
		}
		public override Dux SetContent( object c ) {
			Children.Clear();
			if (c is Dux d) Add( d );
			if (c is List<Dux> l) _Content = l;
			else Add( new DuxValue( null, c ) );
			return this;
			//throw new DuxOperationNotAllowed( this, "Dux.Content.set" );
		}
		//
		// list stuff
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public override Dux this[int i] {
			get => Children[i];
			set => ReplaceItemOfIndex( i, value );
		}
		public override Dux this[string k] {
			get => GetItemByKey( k );
			set { if (GetItemByKey( k ) is DuxNull) Add( value ); else ReplaceItemOfKey( k, value.SetKey( k ) ); }
		}
		private void ReplaceItemOfKey( string key, Dux newDux ) {
			int i = Children.FindIndex( x => x.Key == key );
			if (i > -1) ReplaceItemOfIndex( i, newDux );
		}
		private void ReplaceItemOfIndex( int index, Dux newDux ) {
			Children.RemoveAt( index );
			Children.Insert( index, newDux );
		}
		private Dux GetItemByKey( string k ) {
			foreach (var item in Children) if (item.Key == k) return item; return new DuxNull( this, k );
		}
		public override int FindIndex( Predicate<Dux> p ) {
			return Children.FindIndex( p );
		}
		public override Dux Add( Dux d ) {
			Children.Add( d ); return this;
		}
		public override Dux AddRange( List<Dux> ds ) {
			Children.AddRange( ds ); return this;
		}
		public override Dux AddAndGet( Dux d ) {
			Children.Add( d ); return d;
		}
		public void Remove( Dux item ) {
			if (Children.Contains( item )) Children.Remove( item );
		}
		public void RemoveAt( int index ) {
			Children.RemoveAt( index );
		}
		public void Clear() {
			Children.Clear();
		}
		public void MoveUp( int index ) {
			Children.MoveUp( index );
		}
		public void MoveDown( int index ) {
			Children.MoveDown( index );
		}
		public void Move( Dux item, int index ) {
			Children.Move( item, index );
		}
		//
		// to string stuff
		public override string ToString( bool showKey ) {
			showKey = showKey && Key != null;
			var thisKeyStr = showKey ? $"\"{Key}\" : " : "";
			var contentShowKeys = Count > 0 && Children[0].Key != null && !(this is DuxIndexedList);
			var thisContentStr = new StringBuilder();
			for (int i = 0; i < Children.Count; i++) {
				var item = Children[i];
				var itemKeyStr = contentShowKeys ? $"\"{item.Key}\":" : "";
				var itemContentStr = item is DuxValue d
					? d.IsPrimitive ? $"{item.Content}" : $"\"{item.Content}\""
					: item.ToString( false );
				var itemStr = $"{itemKeyStr}{itemContentStr}";
				var comma = i < Children.Count - 1 ? ", " : "";
				thisContentStr.Append( itemStr + comma );
			}
			return $"{thisKeyStr}{thisContentStr}";
		}
		public override bool IsEmpty() { return Children.Count == 0; }
		//
		// debug info stuff
		public override string Preview() {
			var quant = $"{ Children.Count } • "; //→•
			return $"{quant}{ToString()}";
		}
		//
		// enumerator stuff
		public IEnumerator<Dux> GetEnumerator() {
			return Children.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Children.GetEnumerator();
		}
		//
		// debug info stufff
#if DEBUG
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		protected Info_DuxCollection_Content infoContent => new Info_DuxCollection_Content( this );
#endif
	}












	[DebuggerDisplay( "{Preview(), nq}" )]
	public class DuxIndexedList : DuxCollection {
		//
		// constructors
		public DuxIndexedList( string key = null, params char[] props ) : base( key, null, props ) { }
		public DuxIndexedList( string key, List<Dux> value, params char[] props ) : base( key, value, props ) { }
		//
		// content stuff
		public override void UpdateChildren( Dux otherDux, bool includeNewChildren = false ) {
			if (!includeNewChildren) Children.Clear();
			if (otherDux is DuxValue) Children.Add( new DuxValue( null, otherDux ) );
			else if (otherDux is DuxCollection otherDuxCol) foreach (Dux otherDuxChild in otherDuxCol) Children.Add( otherDuxChild );
		}
		//
		// to string stuff
		public override string ToString( bool showKey ) {
			showKey = showKey && Key != null;
			var k = showKey ? $"\"{Key}\" : " : "";
			return $"{k}[ {ItemsToString()} ]";
		}
		//
		// debug info stuff
		public override string Preview() {
			var k = Key is null ? "" : $"\"{Key}\" : ";
			var quant = $"{ Children.Count }•"; //→•
			return $"{k}[{quant} {ItemsToString()} ]";
		}

		private string ItemsToString() {
			var thisContentStr = new StringBuilder();
			for (int i = 0; i < Children.Count; i++) {
				var item = Children[i];
				var noKey = string.IsNullOrWhiteSpace( item.Key );
				var itemKeyStr = noKey ? "" : $"{item.Key}:";
				var itemContentStr = item is DuxValue ? item.Content.ToString() : item.ToString( false );
				var itemStr = $"{itemKeyStr}{itemContentStr}";
				if (!noKey && item is DuxValue d && !d.IsPrimitive) itemStr = Quote( itemStr );
				var comma = i < Children.Count - 1 ? ", " : "";
				thisContentStr.Append( itemStr + comma );
			}
			return thisContentStr.ToString();
		}

		private string Quote( string text ) { return $"\"{text.Replace( "\"", "\\\"" )}\""; }
	}












	[DebuggerDisplay( "{Preview(), nq}" )]
	public class DuxNamedList : DuxCollection {
		//
		// constructors
		public DuxNamedList( string key = null, params char[] props ) : base( key, null, props ) { }
		public DuxNamedList( string key, List<Dux> value, params char[] props ) : base( key, value, props ) { }
		//
		// content stuff
		public override void UpdateChildren( Dux otherDux, bool includeNewItems = false ) {
			//Children.Clear();
			if (otherDux is DuxCollection otherDuxCol)
				foreach (Dux otherDuxChild in otherDuxCol) {
					if (string.IsNullOrWhiteSpace( otherDuxChild.Key )) continue;
					var thisChild = this[otherDuxChild.Key];
					if (thisChild.Exists()) thisChild.UpdateChildren( otherDuxChild, includeNewItems );
					else if (includeNewItems) thisChild.AddIfNull( otherDuxChild );
				}
		}
		//
		// to string stuff
		public override string ToString( bool showKey ) {
			showKey = showKey && Key != null;
			var k = showKey ? $"\"{Key}\" : " : "";
			return $"{k}{{ {base.ToString( false )} }}";
		}
		//
		// debug info members
		public override string Preview() {
			var k = Key != null ? $"\"{Key}\" : " : "";
			var quant = $"{ Children.Count }•"; //→•
			return $"{k}{{{quant} {base.ToString( false )} }}";
		}
	}

























	public static class DuxExtensions {

		public static string AsString( this Dux dux, string defaultVal = "", bool useDefaultIfWhiteSpaceEmpty = false ) {
			if (dux is DuxNull || (useDefaultIfWhiteSpaceEmpty && string.IsNullOrWhiteSpace( dux.ToString() ))) return defaultVal;
			return dux.ToString();
		}

		public static bool AsBool( this Dux dux, bool defaultVal = default ) {
			if (dux is DuxNull) return defaultVal;
			if (dux.Content is bool b) return b;
			var str = dux.Content.ToString().Trim().Trim( '0' ).ToLower();
			if (str == "true" || str == "1") return true;
			if (str == "false" || str == "") return false;
			return defaultVal;
		}

		public static int AsInt( this Dux dux, int? defaultVal = default ) {
			if (!(dux is DuxNull)) {
				if (dux.Content is int i) return i;
				var str = dux.Content.ToString().Trim();
				if (int.TryParse( str, out int intval )) return intval;
				//else if (defaultVal.HasValue) return defaultVal.Value;
			}
			if (defaultVal.HasValue) return defaultVal.Value;
			throw new InvalidCastException( "Impossível converter valor do Dux em inteiro", new Exception( $"Dux: {dux}" ) );
		}

		public static bool Exists( this Dux dux ) {
			return !(dux is DuxNull);
		}
	}










	public abstract partial class Dux {
		public class ForbiddenTypeOfContent : Exception {
			public readonly Dux dux;
			public readonly object content;
			public ForbiddenTypeOfContent( Dux dux, object content ) :
			base( $"Member 'Content' of type '{dux.GetType().Name}' can not be of type '{content.GetType().Name}'" ) {
				this.dux = dux; this.content = content;
			}
		}
		public class DuxIsNotACollectionException : Exception {
			public readonly Dux dux;
			public DuxIsNotACollectionException( Dux dux ) :
			base( $"A call to a method exclusive to DuxCollection was made on a Dux of type '{dux.GetType().Name}'" ) {
				this.dux = dux;
			}
		}
		public class DuxOperationNotAllowed : Exception {
			public readonly Dux dux;
			public DuxOperationNotAllowed( Dux dux, string operation ) :
			base( $"Operação '{operation}' não permitida em Dux de tipo '{dux.GetType().Name}'" ) {
				this.dux = dux;
			}
		}
	}








#if DEBUG
	/// <summary>
	/// Classes de informação para janela de análise de objetos na depuração.
	/// </summary>
	public abstract partial class Dux {

		[DebuggerDisplay( "{Value,nq}", Name = "{Name,nq}" )]
		protected class Info_Dux_Type {
			public Info_Dux_Type( Dux d ) { dux = d; }
			[DebuggerBrowsable( DebuggerBrowsableState.Never )]
			Dux dux;
			[DebuggerBrowsable( DebuggerBrowsableState.Never )]
			protected string Value {
				get {
					var duxType = dux.GetType().Name;
					var contentType = dux is DuxCollection ? "List<Dux>" : dux.Content.GetType().Name;
					return $"{duxType} → {contentType}";
				}
			}
			[DebuggerBrowsable( DebuggerBrowsableState.Never )]
			protected string Name => "Type";
		}


		[DebuggerDisplay( "{Content,nq}", Name = "Content (view)" )]
		protected class Info_DuxCollection_Content {
			public Info_DuxCollection_Content( Dux d ) { dux = d; }
			[DebuggerBrowsable( DebuggerBrowsableState.Never )]
			Dux dux;
			[DebuggerBrowsable( DebuggerBrowsableState.Never )]
			public object Content { get => dux.ToString( false ); set { } }
		}


		protected class Info_DuxValue_Content {
			public Info_DuxValue_Content( Dux d ) { dux = d; }
			[DebuggerBrowsable( DebuggerBrowsableState.Never )]
			Dux dux;
			public object Content { get => dux.Content; set => dux.SetContent( value ); }
		}
	}
#endif
}
