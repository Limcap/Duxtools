using DataUtils.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUtils.DuxDataStructure_v5
{
	/*
	public interface DuxVal
	{
		string AsString { get; set; }
		DuxRegularNode AsDux { get; set; }
		DuxListNode AsDuxList { get; set; }
	}


	public abstract class Dux2
	{
		public abstract string Key { get; set; }
		public abstract char[] Props { get; set; }
		public abstract string Val { get; set; }
		public abstract Dux2 ValNode { get; set; }
		public abstract DuxListNode ValNodes { get; set; }
		public virtual Dux2 this[string key] {
			get => throw new NullReferenceException();
			set => throw new NullReferenceException();
		}

		public abstract override string ToString();

		public static implicit operator string( Dux2 d ) { return d.ToString(); }
		public static implicit operator Dux2( string v ) { return new DuxValueNode( v ); }
	}




	public class DuxNullNode : Dux2
	{
		public DuxNullNode() { }

		public override string Key {
			get => null;
			set => throw new NullReferenceException();
		}
		public override char[] Props {
			get => null;
			set => throw new NullReferenceException();
		}
		public override Dux2 Val {
			get => null;
			set => throw new NullReferenceException();
		}

		public override string ToString()
		{
			return "NULL";
		}
	}




	public class DuxValueNode : Dux2, DuxVal
	{
		public DuxValueNode( string val ) { _val = val; }

		public override string Key {
			get => null;
			set => throw new NullReferenceException();
		}
		public override char[] Props {
			get => null;
			set => throw new NullReferenceException();
		}
		public override Dux2 Val { get => this; set { } }

		public string AsString { get => _val; }
		public DuxRegularNode AsDux { get => null; }
		public DuxListNode AsDuxList { get => null; }

		private string _val;
		public override string ToString()
		{
			return $"\"{_val}\"";
		}

		public static implicit operator string( DuxValueNode d ) { return d._val; }
		public static implicit operator DuxValueNode( string v ) { return new DuxValueNode( v ); }
	}




	public class DuxRegularNode : Dux2
	{
		public DuxRegularNode( string key, Dux2 val )
		{
			Key = key;
			Val = val;
		}
		public override string Key { get; set; }
		public override char[] Props { get; set; }
		//public new Dux2 Val { get; set; }
		public override Dux2 Val { get; set; }

		public override string ToString()
		{
			return $"\"{Key}\" : {Val}";
		}
	}




	public class DuxListNode : Dux2
	{
		public DuxListNode( string key, Dux2 value )
		{
			Key = key;
			Children = new List<Dux2>();
			Children.Add( value );
		}
		public override string Key { get; set; }
		public override char[] Props { get; set; }
		public override Dux2 Val { get => ToString(); set { } }
		public List<Dux2> Children { get; set; }

		public Dux2 this[string key] {
			get => Get( key );
			set {
				var selectedDux = Get( key );
				if (selectedDux is DuxNullNode) Children.Add( value );
				selectedDux.Val = value.Val;
				selectedDux.Props = value.Props;
			}
		}

		private Dux2 Get( string key )
		{
			var index = Children.FindIndex( d => d.Key == key );
			return index == -1 ? new DuxNullNode() : Children[index];
		}
	}
	*/






















	[DebuggerDisplay( "{Preview(), nq}" )]
	public class Dux : DuxValue // Dictionary Uninodular XML-like
	{
		public Dux() { }

		public Dux( string key, DuxValue value, params char[] mods )
		{
			Key = key; Value = value; Mods = mods.ToList();
		}

		public Dux( string key, DuxString value, params char[] mods )
		{
			Key = key; Value = new DuxString( value ); Mods = mods.ToList();
		}

		public Dux( string key, DuxArray value, params char[] mods )
		{
			Key = key; Value = value; Mods = mods.ToList();
		}
		public Dux( string key, Dux value, params char[] mods )
		{
			Key = key; Value = value; Mods = mods.ToList();
		}

		//public enum ValueKind { Single, Multiple, ArrayString, ArrayDux };
		public virtual string Key { get; set; }
		public List<char> Mods { get; set; }
		public virtual DuxValue Value { get; set; }
		
		public Dux this[string key] {
			get {
				return Get(key);
			}
			set {
				var selectedDux = Get(key);
				value.Key = key;
				// Se o método Get não deu erro é pq o tipo do Value é Array então não precisa refazer a verificação.
				if (selectedDux is DuxNull) (Value as DuxArray).Add(value);
				else selectedDux.Update(value.Value, value.Mods);
			}
		}

		private Dux Get(string key)
		{
			if (!(Value is DuxArray)) throw new Exception("Não é possível buscar um subitem de um Dux com valor único.");
			var index = (Value as DuxArray).FindIndex(n => n is Dux d && d.Key == key );
			//var index = (Value as DuxArray)._list.FindIndex(n => n.Key == key);
			return index == -1 ? new DuxNull() : (Value as DuxArray)[index] as Dux;
		}

		public void Add( params string[] value )
		{
			if (Value is DuxString) {
				var d = new DuxArray();
				d.Add( Value );
				foreach( var str in value ) d.Add( new DuxString( str ) );
				Value = d;
			}
			foreach( var v in value ) (Value as DuxArray).Add( new DuxString( v ) );
		}

		public void Add(string key, string value, params char[] mods)
		{
			if (Value is DuxString) {
				var d = new DuxArray();
				d.Add( new Dux(null, Value) );
				d.Add( new Dux(null, value) );
				Value = d;
			}
			(Value as DuxArray).Add( new Dux( key, new DuxString( value ), mods ) );
		}

		public void Add(Dux dux)
		{
			if (Value is DuxString) throw new Exception( "Impossível adicionar ao valor do Dux, pois ele não é do tipo DuxArray." );
			(Value as DuxArray).Add(dux);
		}

		public void Update(DuxValue value = null, List<char> mods = null)
		{
			if (value != null) Value = value;
			if (mods != null) Mods = mods;
		}

		public void Update(string value = null, List<char> mods = null)
		{
			if (value != null) Value = new DuxString(value);
			if (mods != null) Mods = mods;
		}

		public override string ToString()
		{
			return $"\"{Key}\" : {Value.ToNotation()}";
		}

		public string ToNotation()
		{
			return $"\"{Key}\" : {Value.ToNotation()}";
		}

		public string Preview()
		{
			return $"\"{Key}\" : {Value.Preview()}";
		}
	}










	


	public class DuxNull : Dux
	{
		public DuxNull() : base() { }
		public override string Key { get => null; }
		public override DuxValue Value { get => null; set { } }
	}









	public interface DuxValue
	{
		string Preview();
		string ToNotation();
	}











	[DebuggerDisplay("{Preview(), nq}")]
	public class DuxString : DuxValue
	{
		public DuxString( string val )
		{
			_string = val;
		}

		public string _string;

		public override string ToString() { return _string; }
		public string ToNotation()	{ return $"\"{_string}\""; }
		public string Preview() { return ToNotation(); }

		public static implicit operator DuxString( string val ) { return new DuxString( val ); }
		public static implicit operator string( DuxString val ) { return val._string; }
	}




	[DebuggerDisplay( "{Preview()}" )]
	public class DuxAny : DuxValue
	{
		public DuxAny( object val ) { Val = val;}
		public object Val { get; set; }
		public override string ToString() { return Val.ToString(); }
		public string ToNotation() {
			if (Val is int || Val is double || Val is bool || Val is float ) return Val.ToString();
			else return $"\"{ToString()}\"";
		}
		public string Preview()	{ return ToNotation(); }
	}











	[DebuggerDisplay( "{Preview(), nq}" )]
	public class DuxArray : DuxValue
	{
		public List<DuxValue> Items = new List<DuxValue>();
		public DuxArray(int capacity = 10) { Items = new List<DuxValue>(capacity); }
		//public DuxArray(int capacity = 10) : base(10) {}
		public DuxArray(DuxValue value) { Items = new List<DuxValue>(10);  Items.Add( value ); }
		//public DuxArray(DuxValue value) : base() { Add( value ); }
		public DuxArray(List<DuxValue> values) { Items = new List<DuxValue>(values); }
		//public DuxArray(List<DuxValue> values) : base(values) { }



		//public bool IsArray {
		//	get { foreach (var dux in this) if (!string.IsNullOrEmpty(dux.Key.Trim())) return false; return true; }
		//}
		//public bool UseUniqueKeys;

		public string AsObjectString() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Items.Count; i++) {
				var dux = Items[i];
				var key = dux is Dux d? d.Key : string.Empty;
				sb.Append($"\"{key}\" : \"{Items[i].ToNotation()}\"");
				if (i < Items.Count - 1) sb.Append(", ");
			}
			return $"{{{sb}}}";
		}

		public string ToNotation() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Items.Count; i++) {
				sb.Append($"{Items[i].ToNotation()}");
				if (i < Items.Count - 1) sb.Append(", ");
			}
			return $"[{sb}]";
		}

		public override string ToString()
		{
			return ToNotation();
		}

		public string Preview()
		{
			return $"DuxArray({Items.Count})";
		}



		//public int Count => Items.Count;
		public int FindIndex(Predicate<DuxValue> p) => Items.FindIndex(p);
		//public DuxValue Find(Predicate<DuxValue> p) => Items.Find(p);
		public void Add(DuxValue v) => Items.Add(v);
		public DuxValue this[int index] => Items[index];
		//object IEnumerator.Current => Items.GetEnumerator().Current;
		//public DuxValue Current => Items.GetEnumerator().Current;
		//public IEnumerator GetEnumerator() => Items.GetEnumerator();
		//public bool MoveNext() => Items.GetEnumerator().MoveNext();
		//public void Reset() => GetEnumerator().Reset();
		//void IDisposable.Dispose() => Items.GetEnumerator().Dispose();
	}












	//public class DuxDictionary : Dictionary<string,Dux>, DuxValue
	//{
	//	public DuxDictionary( int capacity = 10 ) : base( 10 ) { }
	//	public DuxDictionary( string value ) : this( new Dux( null, new DuxString( value ) ) ) { }
	//	public DuxDictionary( Dux value ) : base() { Add( value.Key, value ); }
	//	public DuxDictionary( Dictionary<string, Dux> values ) : base( values ) { }

	//	public override string ToString()
	//	{
	//		StringBuilder sb = new StringBuilder();
	//		var array = this.ToArray();
	//		for (int i = 0; i < Count; i++) {
	//			var dux = array[i];
	//			sb.Append( $"\"{dux.Key}\" : \"{dux.Value}\"" );
	//			if (i < Count - 1) sb.Append( ", " );
	//		}
	//		return $"{{{sb}}}";
	//	}

	//	public string Preview()
	//	{
	//		return $"DuxArray({Count})";
	//	}
	//}


















	/// <summary>
	/// Metodos para
	/// </summary>
	public static class DuxImporter
	{
		public static Dux DuxFrom(JSONNode jsn)
		{
			return DuxFrom(null, jsn);
		}
		public static Dux DuxFrom(string modsAndKey, JSONNode jsn)
		{
			var mods = ModsFrom(modsAndKey);
			string key = KeyFrom(modsAndKey, jsn);
			var value = DuxValueFrom(jsn);
			return new Dux(key, value, mods.ToArray());
		}

		public static DuxValue DuxValueFrom(JSONNode jsn)
		{
			var entries = new List<DuxValue>(jsn.Count);
			if (jsn.Count == 0) return new DuxString(jsn.AsString);
			else
				foreach (var item in jsn)
					if (string.IsNullOrEmpty(item.Key)) entries.Add( DuxValueFrom(item.Value));
					else entries.Add(DuxFrom(item.Key, item.Value));
			return new DuxArray(entries);
		}

		public static char[] ModsFrom(string modsAndKey)
		{
			List<char> propsList = new List<char>();
			if (modsAndKey != null) {
				int propsEndIndex = modsAndKey.IndexOf('|');
				string propsString = propsEndIndex == -1 ? string.Empty : modsAndKey.Substring(0, propsEndIndex);
				foreach (var prop in propsString) propsList.Add(prop);
			}
			return propsList.ToArray();
		}

		public static string KeyFrom(string modsAndKey, JSONNode jsn)
		{
			if (modsAndKey is null) return string.Empty;
			int propsEndIndex = modsAndKey.IndexOf('|');
			string key = jsn is JSONArray ? null
				: modsAndKey.Length > propsEndIndex + 1 ? modsAndKey.Substring(propsEndIndex + 1) : string.Empty;
			return key;
		}
	}
















	public static class DuxExporter
	{
		public static JSONNode ToJson(Dux d)
		{
			JSONNode node;
			node = new KeyValuePair<string, JSONNode>(d.Key, ToJson(d.Value));
			return node;
		}


		public static JSONNode ToJson(DuxValue value)
		{
			JSONNode node;
			if (value is DuxString)
				node = value.ToString();
			else if (value is DuxArray a) {
				node = new JSONArray();
				foreach (DuxValue entry in a.Items) node.Add(ToJson(entry));
			}
			else if (value is Dux d) {
				node = new KeyValuePair<string, JSONNode>(d.Key, ToJson(d.Value));
			}
			else
				node = JSONNull.CreateOrGet();
			//else {
			//	if ((value as DuxArray).IsArray) node = new JSONArray();
			//	else node = new JSONObject();
			//	foreach (var entry in (value as DuxArray)) node.Add( ToJson( entry ) );
			//};
			return node;
		}
	}
















	public static class DuxExtensions
	{
		public static bool Bool(this Dux dux, bool? defaultValue = null)
		{
			var value = dux.Value?.ToString().Trim().Trim('0').ToLower();
			if (value == "" || value == "false") return false;
			if (value == "1" || value == "true") return true;
			if (defaultValue.HasValue) return defaultValue.Value;
			else throw new InvalidCastException("Impossível converter valor do Dux em Boolean", new Exception($"Dux: {dux}"));
		}

		public static int Int(this Dux dux, int? defaultValue = null)
		{
			if (dux.Value is DuxString val && int.TryParse(val, out int intval)) return intval;
			if (defaultValue.HasValue) return defaultValue.Value;
			else throw new InvalidCastException("Impossível converter valor do Dux em inteiro", new Exception($"Dux: {dux}"));
		}

		public static string String(this Dux dux, string defaultValue = null)
		{
			if (dux.Value != null) return dux.Value.ToString();
			if (defaultValue != null) return defaultValue;
			else throw new InvalidCastException("Impossível converter valor de Dux em String", new Exception($"Dux: {dux}"));
		}

		public static bool Exists(this Dux dux)
		{
			return !(dux is DuxNull);
		}
	}
}
