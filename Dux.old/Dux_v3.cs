using DataUtils.SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUtils.DuxDataStructure_V3
{
	public class Dux // Dictionary Uninodular XML-like
	{
		public Dux() { }


		public Dux(string key, DuxValue value, params char[] mods)
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
				var selectedDux = Get(key);
				value.Key = key;
				if (selectedDux is DuxNull) Value.Add(value);
				else selectedDux.Update(value.Value, value.Mods);
			}
		}


		private Dux Get(string key)
		{
			if( !Value.IsArray ) throw new Exception("Impossivel buscar chave em string");
			var index = Value.FindIndex(n => n.Key == key);
			return index == -1 ? new DuxNull() : Value[index];
		}


		public void Add(string key, string value, params char[] mods)
		{
			Value.Add(new Dux(key, value, mods));
		}


		public void Add(Dux dux)
		{
			Value.Add(dux);
		}


		public void Update(DuxValue value = null, List<char> mods = null)
		{
			if (value != null) Value = value;
			if (mods != null) Mods = mods;
		}


		public override string ToString()
		{
			string valuetext = Value.IsArray ? $"{Key}" : $"\"{Value}\"";
			return $"\"{Key}\" : {valuetext}";
		}


		public static implicit operator Dux(string value) { return new Dux(null, value); }
	}
















	public class DuxNull : Dux
	{
		public DuxNull() : base() { }
		public override string Key { get => null; }
		public override DuxValue Value { get => null; set { } }
	}













	public class DuxValue : List<Dux>
	{
		public DuxValue(int capacity = 10) : base(capacity) { }
		public DuxValue(List<Dux> values) : base(values) { }
		public DuxValue(Dux value) : base() { Add(value); }

		private StringBuilder sb = new StringBuilder();

		public bool IsArray {
			get { foreach (var dux in this) if (dux.Key != null) return false; return true; }
		}

		public bool IsSingleString { get => AsSingleString != null; }

		public string AsSingleString {
			get {
				if (Count == 1 && string.IsNullOrEmpty(this[0].Key))
					return this[0].Value.ToString();
				else return null;
			} 
		}

		public string AsObjectString {
			get {
				sb.Clear();
				for (int i = 0; i < Count; i++) {
					var dux = this[i];
					sb.Append($"\"{dux.Key}\" : \"{dux.Value}\"");
					if (i < Count - 1) sb.Append(", ");
				}
				return $"{{{sb}}}";
			}
		}

		public string AsArrayString {
			get {
				sb.Clear();
				for (int i = 0; i < Count; i++) {
					var dux = this[i];
					sb.Append($"\"{dux.Value}\"");
					if (i < Count - 1) sb.Append(", ");
				}
				return $"{{{sb}}}";
			}
		}

		public override string ToString()
		{
			if (IsSingleString) return AsSingleString;
			if (IsArray) return AsArrayString;
			return AsObjectString; 
		}

		public static implicit operator DuxValue(string value) { return new DuxValue(value); }
		public static implicit operator string(DuxValue dux) { return dux; }
		public static implicit operator DuxValue(Dux v) { return new DuxValue(v); }
	}
















	//public class DuxStringValue : DuxValue
	//{
	//	public DuxStringValue(string value) { _value = value; }
	//	public string _value;
	//	public static implicit operator DuxStringValue(string value) { return new DuxStringValue(value); }
	//	public static implicit operator string(DuxStringValue value) { return value._value; }

	//	public override string ToString() { return _value; }
	//}
















	//public class DuxObjectValue : List<Dux>, DuxValue
	//{
	//	public DuxObjectValue(int capacity = 10) : base(capacity) { }
	//	public DuxObjectValue(List<Dux> values) : base(values) { }
	//	public DuxObjectValue(Dux value) : base() { Add(value); }

	//	public Dux.ValueKind Kind => Count > 1 && string.IsNullOrEmpty(_entries[0].Key) ? Dux.ValueKind.ArrayString : Dux.ValueKind.ArrayDux;


	//	public static implicit operator DuxObjectValue(Dux v) { return new DuxObjectValue(v); i }

	//	public override string ToString()
	//	{
	//		List<string> temp = new List<string>(_entries.Count);
	//		foreach (var dux in _entries) {
	//			temp.Add($"\"{dux.Key}\" : \"{dux.Value}\"");
	//		}
	//		return $"{{{string.Join(",", temp)}}}";
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
			var entries = new List<Dux>(jsn.Count);
			if (jsn.Count == 0) return new DuxValue(string.Empty);
			else foreach (var item in jsn) entries.Add(DuxFrom(item.Key, item.Value));
			return new DuxValue(entries);
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
			if (value.IsSingleString) node = value.AsSingleString;
			else {
				node = new JSONObject();
				foreach (var entry in value) node.Add(ToJson(entry));
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
			if (dux.Value.IsSingleString && int.TryParse(dux.Value.AsSingleString, out int intval)) return intval;
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
