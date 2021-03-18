using DataUtils.SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUtils.DuxDataStructure_V1
{
	public partial class Dux // Dictionary Uninodular XML-like
	{
		public Dux(string key, DuxValue value, params char[] mods)
		{
			Key = key;
			Value = value;
			Mods = mods?.ToList();
		}


		public Dux(string key, DuxValueEntry value, params char[] mods)
		: this(key, new DuxValue(value), mods)
		{}


		public virtual string Key { get; set; }
		public List<char> Mods { get; set; }
		public virtual DuxValue Value { get; set; }


		public static implicit operator Dux(DuxValue value)
		{
			return new Dux(null, value);
		}


		public Dux this[string key] {
			get => Get(key);
			set {
				var dux = Get(key);
				if (dux is DuxNull) Value.Add(new Dux(key, value.Value));
				else dux.Value = value.Value;
			}
		}


		private Dux Get(string key)
		{
			//if (Value.Kind != ValueKind.Multiple)
			//	throw new ArrayTypeMismatchException(
			//		$"Não é possível buscar uma entrada no Dux pois ele não é do tipo {ValueKind.Multiple}");
			var index = Value._entries.FindIndex(n => n.dux.Key == key);
			return index == -1 ? new DuxNull() : Value._entries[index].dux;
		}


		public void Add(string key, string value, params char[] mods)
		{
			Add(new Dux(key, value, mods));
		}


		public void Add(Dux dux)
		{
			Value.Add(dux);
		}

		//public void AddSpacer()
		//{
		//	_children.Add( new AdictSpacer() );
		//}

		public enum ValueKind { Single, Multiple, SingleDux, DuxArray };
	}














	//public partial class DuxValue
	//{
	//	public DuxValue( string value ) { _string = value; Kind = Dux.ValueKind.SingleString; }
	//	public DuxValue( params string[] values ) { _manyStrings = values.ToList(); Kind = Dux.ValueKind.StringArray; }
	//	public DuxValue( List<string> values ) { _manyStrings = values; Kind = Dux.ValueKind.StringArray; }
	//	public DuxValue( Dux value ) { _dux = value; Kind = Dux.ValueKind.SingleDux; }
	//	public DuxValue( List<Dux> values ) { _manyDux = values; Kind = Dux.ValueKind.DuxArray; }
	//	public DuxValue( params Dux[] values ) { _manyDux = values.ToList(); Kind = Dux.ValueKind.DuxArray; }
	//	public DuxValue( Dux.ValueKind kind, int capacity = 10 )
	//	{
	//		Kind = kind;
	//		if (kind == Dux.ValueKind.DuxArray) _manyDux = new List<Dux>( capacity );
	//		else if (kind == Dux.ValueKind.StringArray) _manyStrings = new List<string>( capacity );
	//		else if (kind == Dux.ValueKind.SingleDux) _dux = new DuxNull();
	//		else _string = string.Empty;
	//	}

	//	private string _string;
	//	private Dux _dux;
	//	private List<DuxValue> _array;

	//	private List<string> _manyStrings;
	//	private List<Dux> _manyDux;
	//	public readonly Dux.ValueKind Kind;
	//	//public Dux.ValueKind Kind {
	//	//	get => _duxArray != null ? Dux.ValueKind.DuxArray
	//	//		: _stringArray != null ? Dux.ValueKind.StringArray
	//	//		: _singleDux != null ? Dux.ValueKind.SingleDux
	//	//		: Dux.ValueKind.SingleString;
	//	//}


	//	public void Add( Dux dux )
	//	{
	//		if (Kind == Dux.ValueKind.DuxArray) _manyDux.Add( dux );
	//		else throw new ArrayTypeMismatchException( $"Tentativa de adicionar um Dux a outro Dux que não é do tipo {Dux.ValueKind.DuxArray}. Tipo: {Kind}" );
	//	}

	//	public void Add( string str )
	//	{
	//		if (Kind == Dux.ValueKind.StringArray) _manyStrings.Add( str );
	//		else throw new ArrayTypeMismatchException( $"Tentativa de adicionar um String a um Dux que não é do tipo {Dux.ValueKind.StringArray}. Tipo: {Kind}" );
	//	}








	//	public static implicit operator DuxValue( List<Dux> value )
	//	{
	//		return new DuxValue( value );
	//	}

	//	public static implicit operator DuxValue( Dux[] value )
	//	{
	//		return new DuxValue( value );
	//	}

	//	public static implicit operator DuxValue( List<string> value )
	//	{
	//		return new DuxValue( value );
	//	}

	//	public static implicit operator DuxValue( string[] value )
	//	{
	//		return new DuxValue( value );
	//	}

	//	public static implicit operator DuxValue( string value )
	//	{
	//		return new DuxValue( value );
	//	}

	//	public static implicit operator DuxValue( int value )
	//	{
	//		return new DuxValue( value.ToString() );
	//	}

	//	public static implicit operator DuxValue( bool value )
	//	{
	//		return new DuxValue( value.ToString().ToLower() );
	//	}

	//	public static implicit operator string( DuxValue dux )
	//	{
	//		return dux.ToString();
	//	}

	//	public static explicit operator int( DuxValue dux )
	//	{
	//		return int.Parse( dux.ToString() );
	//	}

	//	public static explicit operator bool( DuxValue dux )
	//	{
	//		var value = dux.ToString().Trim().Trim( '0' ).ToLower();
	//		if (value == "" || value == "false") return false;
	//		if (value == "1" || value == "true") return true;
	//		else throw new InvalidCastException( "Impossível converter Dux em boolean", new Exception( $"Dux não é boolean: {dux}" ) );
	//	}

	//	public override string ToString()
	//	{
	//		return ToJson().ToString();
	//	}
	//}
















	public class DuxNull : Dux
	{
		public DuxNull() : base(null, null) { }
		public override string Key { get => null; }
		public override DuxValue Value { get => null; set { } }
	}
















	public partial class DuxValue
	{
		public DuxValue(int capacity = 10)
		{
			_entries = new List<DuxValueEntry>(capacity);
		}

		public DuxValue(List<DuxValueEntry> entries)
		{
			_entries = entries.ToList();
		}

		public DuxValue(DuxValueEntry entry)
		{
			_entries = new List<DuxValueEntry>() { entry };
		}

		public List<DuxValueEntry> _entries;
		public Dux.ValueKind Kind => _entries.Count <= 1
				 ? Dux.ValueKind.Single
				 : Dux.ValueKind.Multiple;




		public static implicit operator DuxValue(List<DuxValueEntry> v)
		{
			return new DuxValue(v);
		}
		public static implicit operator DuxValue(DuxValueEntry v)
		{
			return new DuxValue(v);
		}




		public void Add(DuxValueEntry entry)
		{
			_entries.Add(entry);
		}




		//public JSONNode ToJSON()
		//{
		//	return DuxExporter.ToJson(this);
		//}
		//public override string ToString()
		//{
		//	return ToJSON().ToString();
		//}
	}
















	public class DuxValueEntry
	{
		public DuxValueEntry(string str) { this.str = str; this.dux = null; }
		public DuxValueEntry(Dux dux) { this.str = null; this.dux = dux; }

		public string str;
		public Dux dux;

		public static implicit operator DuxValueEntry(Dux dux)
		{
			return new DuxValueEntry(dux);
		}
		public static implicit operator DuxValueEntry(string value)
		{
			return new DuxValueEntry(value);
		}

		public static implicit operator DuxValueEntry(int value)
		{
			return new DuxValueEntry(value.ToString());
		}

		public static implicit operator DuxValueEntry(bool value)
		{
			return new DuxValueEntry(value.ToString().ToLower());
		}

		public static implicit operator string(DuxValueEntry entry)
		{
			return entry.ToString();
		}

		public static explicit operator int(DuxValueEntry entry)
		{
			return int.Parse(entry.ToString());
		}

		public static explicit operator bool(DuxValueEntry entry)
		{
			var value = entry.ToString().Trim().Trim('0').ToLower();
			if (value == "" || value == "false") return false;
			if (value == "1" || value == "true") return true;
			else throw new InvalidCastException("Impossível converter DuxValueEntry em boolean", new Exception($"Dux não é boolean: {entry}"));
		}
	}
















	/// <summary>
	/// Metodos para
	/// </summary>
	public static class DuxImporter
	{
		public static Dux DuxFrom(string modsAndKey, JSONNode jsn)
		{
			var mods = ModsFrom(modsAndKey);
			string key = KeyFrom(modsAndKey, jsn);
			var value = DuxValueFrom(jsn);
			return new Dux(key, value, mods.ToArray());
		}

		public static DuxValue DuxValueFrom(JSONNode jsn)
		{
			var entries = new List<DuxValueEntry>(jsn.Count);
			if (jsn.Count == 0) entries.Add(jsn.AsString);
			else foreach (var item in jsn) entries.Add(DuxValueEntryFrom(item.Key, item.Value));
			var a = entries.ToString();
			return new DuxValue(entries);
		}

		public static DuxValueEntry DuxValueEntryFrom(string key, JSONNode jsn)
		{
			dynamic entry;
			if (jsn is JSONObject || jsn is JSONArray || !string.IsNullOrEmpty(key))
				entry = DuxFrom(key, jsn);
			else
				entry = jsn.AsString;
			return new DuxValueEntry(entry);
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
			if (d._entries.Count == 0) node = string.Empty;
			else if (d._entries.Count == 1) node = ToJson(d._entries[0]);
			else node = ToJson(d._entries);
			return node;
		}


		public static JSONNode ToJson(DuxValueEntry e)
		{
			JSONNode node;
			if (e.dux is null) node = e.str;
			else node = ToJson(e.dux);
			return node;
		}
	}
}
