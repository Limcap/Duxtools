using DataUtils.SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUtils.DuxDataStructure_V2
{
	public class Dux // Dictionary Uninodular XML-like
	{
		public Dux() { }


		public Dux(string key, DuxObjectValue value, params char[] mods)
		{
			Key = key;
			Value = value;
			Mods = mods?.ToList();
		}


		public Dux(string key, DuxStringValue value, params char[] mods)
		{
			Key = key;
			Value = value;
			Mods = mods?.ToList();
		}



		public enum ValueKind { Single, Multiple, ArrayString, ArrayDux };
		public virtual string Key { get; set; }
		public List<char> Mods;
		public virtual DuxValue Value { get; set; }




		public Dux this[string key] {
			get {
				return Get(key);
			}
			set {
				var dux = Get(key);
				value.Key = key;
				if (dux is DuxNull) {
					if (this.Value is DuxObjectValue obj) { obj.Add(value); }
					else throw new Exception("Impossivel adicionar chave a string");
				}
				else {
					dux.Value = value.Value;
					dux.Mods = value.Mods;
				}
			}
		}


		private Dux
			Get(string key)
		{
			if (Value is DuxObjectValue obj) {
				var index = obj._entries.FindIndex(n => n.Key == key);
				return index == -1 ? new DuxNull() : obj._entries[index];
			}
			else throw new Exception("Impossivel buscar chave em string");
		}




		public void Add(string key, string value, params char[] mods)
		{
			Add(new Dux(key, value, mods));
		}


		public void Add(Dux dux)
		{
			if (Value is DuxObjectValue obj) obj.Add(dux);
			else throw new Exception("Impossível adicionar um Dux a uma valor do tipo DuxStringValue");
		}




		public static implicit operator Dux(DuxObjectValue value) { return new Dux(null, value); }
		public static implicit operator Dux(string value) { return new Dux(null,value); }

		

		public override string ToString()
		{
			string valuetext = Value is DuxObjectValue ? $"{Value}" : $"\"{Value}\"";
			return $"\"{Key}\" : {valuetext}";
		}
	}
















	public class DuxNull : Dux
	{
		public DuxNull() : base() { }
		public override string Key { get => null; }
		public override DuxValue Value { get => null; set { } }
	}













	public interface DuxValue {}
	public class DuxStringValue : DuxValue
	{
		public DuxStringValue( string value ) { _value = value; }
		public string _value;
		public static implicit operator DuxStringValue( string value ) { return new DuxStringValue(value); }
		public static implicit operator string( DuxStringValue value ) { return value._value; }
		
		public override string ToString() { return _value;	}
	}
















	public partial class DuxObjectValue : List<Dux>, DuxValue
	{
		public DuxObjectValue(int capacity = 10) { _entries = new List<Dux>(capacity); }
		public DuxObjectValue(List<Dux> entries) { _entries = entries.ToList(); AddRange(entries); }
		public DuxObjectValue(Dux entry)	{ _entries = new List<Dux>() { entry }; Add(entry);  }

		public List<Dux> _entries;

		public Dux.ValueKind Kind => _entries.Count > 1 && string.IsNullOrEmpty(_entries[0].Key)? Dux.ValueKind.ArrayString : Dux.ValueKind.ArrayDux;

		public static implicit operator DuxObjectValue(Dux v) { return new DuxObjectValue(v); }

		public override string ToString() {
			List<string> temp = new List<string>(_entries.Count);
			foreach( var dux in _entries) {
				temp.Add($"\"{dux.Key}\" : \"{dux.Value}\"");
			}
			return $"{{{string.Join(",", temp)}}}";
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
			if( value is DuxObjectValue)
				return new Dux(key, (DuxObjectValue)value, mods.ToArray());
			return new Dux(key, (DuxStringValue)value, mods.ToArray());
		}

		public static DuxValue DuxValueFrom(JSONNode jsn)
		{
			var entries = new List<Dux>(jsn.Count);
			if (jsn.Count == 0) return new DuxStringValue( jsn.AsString );
			else foreach (var item in jsn) entries.Add(DuxFrom(item.Key, item.Value));
			return new DuxObjectValue(entries);
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


		public static JSONNode ToJson(DuxValue d)
		{
			JSONNode node;
			if (d is DuxObjectValue obj) {
				node = new JSONObject();
				foreach (var entry in obj._entries) node.Add(ToJson(entry));
			}
			else node = (d as DuxStringValue)._value;
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
			if( dux.Value is DuxStringValue val && int.TryParse(val, out int intval) ) return intval;
			if (defaultValue.HasValue) return defaultValue.Value;
			else throw new InvalidCastException("Impossível converter valor do Dux em inteiro", new Exception($"Dux: {dux}"));
		}

		public static string String(this Dux dux, string defaultValue = null)
		{
			if( dux.Value != null ) return dux.Value.ToString();
			if (defaultValue != null) return defaultValue;
			else throw new InvalidCastException("Impossível converter valor de Dux em String", new Exception($"Dux: {dux}"));
		}

		public static bool Exists(this Dux dux)
		{
			return !(dux is DuxNull);
		}
	}
}
