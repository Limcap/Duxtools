using DataUtils.BasicExtensions;
using DataUtils.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataUtils
{
	public partial class Adict : AdictBase
	{
		
		public string Key { get; set; }
		public override string Value { get => this.ToString(); set { } }


		public Adict(int capacity, string key = null) : this(key, capacity) { }
		public Adict(string key = null, int capacity = 10)
		{
			Key = key;
		}




		public void Add(string key, string value, params char[] props)
		{
			//_list.Add( new ParamItem( key, value, props ) { _group = this } );
			_list.Add(new AdictTerm(key, value, props));
		}
		public void Add(Dux n)
		{
			if (!_list.Contains(n)) _list.Add(n);
		}
		public void Add(AdictTerm n)
		{
			//n._group = this;
			if (!_list.Contains(n)) _list.Add(n);
		}
		public void AddSpacer()
		{
			_list.Add(new AdictSpacer());
		}




		public void Replace(string key, string value)
		{
			foreach (var item in _list)
				if (item is Adict.AdictTerm leaf && leaf.Key == key)
					leaf.Value = value;
		}













		public Adict GetList(string key)
		{
			foreach (Dux anode in _list) if (anode is Adict adict)
					if (anode.Key == key) return adict;
			return null;
		}




		public AdictTerm GetItem(string key)
		{
			foreach (Dux node in _list) if (node is AdictTerm item)
					if (item.Key == key) return item;
			return null;
		}




		public bool HasItem(string key)
		{
			return GetItem(key) != null || GetList(key) != null;
		}




		public string GetString(string key, string defaultValue = "", bool useDefaultIfEmpty = false)
		{
			var item = GetItem(key);
			return item is null || item.HasMod(Mod.DISABLED) || (item.Value.Trim() == "" && useDefaultIfEmpty)
				? defaultValue : item.Value;
		}




		public bool GetBool(string key, bool defaultValue = false)
		{
			var item = GetItem(key);
			if (item is null || item.HasMod(Mod.DISABLED)) return defaultValue;
			var v = item.Value.Trim().ToLower();
			return v.IsIn("0", "false") ? false : v.IsIn("1", "true") ? true : defaultValue;
		}




		public int GetInt(string key, int defaultValue = 0)
		{
			var item = GetItem(key);
			if (item is null || item.HasMod(Mod.DISABLED)) return defaultValue;
			if (!int.TryParse(item.Value, out int integer)) return defaultValue;
			else return integer;
		}




		public override string ToString()
		{
			return (Key is null ? "" : $"\"{Key}\" : ") + ToJso().ToString();
		}





















		public Dux this[string key] {
			get => GetItem(key);
			set {
				var term = GetItem(key);
				if (term is null) throw new Exception($"Chave não existente no Adict: '{key}'");
				if (value.Key is null) {
					term.Value = value;
				}
				else {
					var index = _list.FindIndex(i => i == term);
					if (index > -1) {
						_list.RemoveAt(index);
						_list.Insert(index, value);
					}
				}
			}
		}
		//public AdictTerm this[string key] {
		//	get => GetItem(key);
		//	set {
		//		var term = GetItem(key);
		//		if (term is null) throw new Exception($"Chave não existente no Adict: '{key}'");
		//		if (value.Key is null) {
		//			term.Value = value;
		//		}
		//		else {
		//			var index = _list.FindIndex(i => i == term);
		//			if (index > -1) {
		//				_list.RemoveAt(index);
		//				_list.Insert(index, value);
		//			}
		//		}
		//	}
		//}

		public Dux Get(string key)
		{
			foreach (Dux node in _list)
				if (node.Key == key) return node;
			return new Adict.Null();
		}





















		public static Adict FromJso(JSONObject jso, string nodeKey = null)
		{
			var p = new Adict(jso.Count, nodeKey);
			p._list.Clear();
			foreach (var item in jso) {
				//for (int i = 0; i < jso.Count; i++) {
				List<char> propsList = new List<char>();
				int propsEndIndex = item.Key.IndexOf('|');
				string propsString = propsEndIndex == -1 ? string.Empty : item.Key.Substring(0, propsEndIndex);
				foreach (var prop in propsString) propsList.Add(prop);

				string key = item.Key.Length > propsEndIndex + 1 ? item.Key.Substring(propsEndIndex + 1) : string.Empty;

				if (item.Value is JSONObject jso2) {
					p.Add(Adict.FromJso(jso2, key));
				}
				else {
					string value = item.Value is JSONString ? item.Value.AsString : item.Value.ToString(2);
					p.Add(new AdictTerm(key, value, propsList.ToArray()));
				}
				//bool isObject = !(item.Value is JSONString);
				//string value = !isObject ? item.Value.AsString : item.Value.ToString( 2 );
				//p.Add( new ParamItem( key, value, propsList.ToArray() ) );
			}
			return p;
		}








		public JSONObject ToJso(bool includeProps = true)
		{
			this["lll"] = 1.ToString();
			var jso = new JSONObject();
			var invalidJSONfields = new List<string>();
			foreach (Dux node in this) {
				if (node is Adict list) {
					jso.Add(list.Key, list.ToJso());
				}
				else if (node is AdictTerm item) {
					var prefix = includeProps ? string.Join("", item.mods) + '|' : "";
					var key = $"{prefix}{item.Key}";

					// O JSON.Parse as vezes emite exceção se a string JSON estiver errada, como por exemplo a 
					// exceção "Too many closing brackets", porém em alguns outros casos ele faz um parse incorreto, por
					// exemplo se tiver aspas ou opening brackets errados. Portanto fazemos um catch para os casos em que
					// emite exceção mas também verificamos o resultado do Parse caso dê certo.
					// Como o JSON.Parse faz um parse parcial se o json estiver errado, é necessário verificar se
					// o parse foi feito certo para não danificar a string do item.value
					JSONNode valueAsObject;
					bool objectIsCorrectlyParsed;
					try {
						valueAsObject = JSON.Parse(item.Value);
						var valueText = item.Value.Replace(" ", "").Replace("\n", "").Replace("\r", "");
						var objectText = valueAsObject.ToString().Replace(" ", "");
						objectIsCorrectlyParsed = valueText.Length == objectText.Length;
					}
					catch {
						valueAsObject = new JSONObject();
						objectIsCorrectlyParsed = false;
					}

					// Adiciona ao jso, dependendo se o parse deu certo ou não
					if (valueAsObject.Count > 0) {
						if (!objectIsCorrectlyParsed) {
							invalidJSONfields.Add(key);
							jso.Add(key, item.Value);
						}
						else jso.Add(key, valueAsObject);
					}
					else jso.Add(key, item.Value);
				}
			}

			if (invalidJSONfields.Count > 0)
				throw new InvalidJSONFieldsException(invalidJSONfields.ToArray(), jso);
			return jso;
		}








		public void UpdateFromJso(JSONObject jso)
		{
			foreach (var item in jso) {
				if (HasItem(item.Key)) {
					if (item.Value is JSONObject jso2 && GetList(item.Key) is Adict pl) {
						pl.UpdateFromJso(jso2);
					}
					else if (item.Value is JSONString && GetItem(item.Key) is AdictTerm pi) {
						pi.Value = item.Value;
					}
				}
				else if (item.Value is JSONString)
					Add(item.Key, item.Value);
				else if (item.Value is JSONObject jso2)
					Add(FromJso(jso2, item.Key));
			}
		}
	}

}
