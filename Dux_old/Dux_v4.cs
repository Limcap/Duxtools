using DataUtils.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUtils.DuxDataStructure_v4
{
	[DebuggerDisplay( "{Preview(), nq}" )]
	public class Dux // Dictionary Uninodular XML-like
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

		public enum ValueKind { Single, Multiple, ArrayString, ArrayDux };
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
			if (Value is DuxString) throw new Exception("Tentativa de obtenção de chave em um Dux do tipo String.");
			var index = (Value as DuxArray).FindIndex(n => n.Key == key);
			//var index = (Value as DuxArray)._list.FindIndex(n => n.Key == key);
			return index == -1 ? new DuxNull() : (Value as DuxArray)[index];
		}

		public void Add(string key, string value, params char[] mods)
		{
			if (Value is DuxString) {
				var d = new DuxArray();
				d.Add( new Dux(null, Value) );
				d.Add( new Dux(null, value) );
				Value = d;
				//throw new Exception( "Impossível adicionar ao string ao Dux, pois ele não é do tipo DuxArray." );
			}
			(Value as DuxArray).Add(new Dux(key, new DuxString(value), mods));
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
			if( Value is DuxString ) {
				return $"\"{Key}\" : \"{Value}\"";
			}
			else {
				return $"\"{Key}\" : {Value}";
			}
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
	}
















	[DebuggerDisplay("{Preview()}")]
	public class DuxString : DuxValue
	{
		public DuxString( string val )
		{
			_val = val;
		}

		private string _val;

		public override string ToString()
		{
			return _val;
		}

		public string Preview()
		{
			return $"\"{_val}\"";
		}

		public static implicit operator DuxString( string value ) { return new DuxString( value ); }
		public static implicit operator string( DuxString value ) { return value._val; }
	}
















	[DebuggerDisplay( "{Preview(), nq}" )]
	public class DuxArray : List<Dux>, DuxValue
	{
		public DuxArray(int capacity = 10) : base(10) {}
		public DuxArray(string value) : this( new Dux( null, new DuxString( value ) ) ) { }
		public DuxArray(Dux value) : base() { Add(value); }
		public DuxArray(List<Dux> values) : base(values) {}

		public bool IsArray {
			get { foreach (var dux in this) if (!string.IsNullOrEmpty(dux.Key.Trim())) return false; return true; }
		}

		public string AsObjectString() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Count; i++) {
				var dux = this[i];
				sb.Append($"\"{dux.Key}\" : \"{dux.Value}\"");
				if (i < Count - 1) sb.Append(", ");
			}
			return $"{{{sb}}}";
		}

		public string AsArrayString() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Count; i++) {
				var dux = this[i];
				sb.Append($"\"{dux.Value}\"");
				if (i < Count - 1) sb.Append(", ");
			}
			return $"[{sb}]";
		}

		public override string ToString()
		{
			if (IsArray) return AsArrayString();
			return AsObjectString();
		}

		public string Preview()
		{
			return $"DuxArray({Count})";
		}
	}


















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
			var entries = new List<Dux>(jsn.Count);
			if (jsn.Count == 0) return new DuxString(jsn.AsString);
			else foreach (var item in jsn) entries.Add(DuxFrom(item.Key, item.Value));
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
			if (value is DuxString) node = value.ToString();
			else {
				if ((value as DuxArray).IsArray) node = new JSONArray();
				else node = new JSONObject();
				foreach (var entry in (value as DuxArray)) node.Add( ToJson( entry ) );
			};
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
