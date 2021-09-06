using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limcap.Duxtools {
	/// <summary>
	/// Methods for converting from and to SimpleJSON
	/// </summary>
	public abstract partial class Dux {

		public static class Import {
			/// <summary>
			/// O check integrity ainda não esta funcionando direito. pobservei um caso que deu erro, entao adicionei
			/// um parametro checkIntegrity=false como padrao para nao fazer essa validação por enquanto.
			/// </summary>
			/// <param name="json"></param>
			/// <param name="checkIntegrity"></param>
			/// <returns></returns>
			public static Dux FromJson( string json, bool checkIntegrity=false ) {
				var jsn = JSON.Parse( json );
				if( checkIntegrity && !CheckJsonImportIntegrity( json, jsn ))
					throw new CorruptedJsonException( jsn );
				else return FromJSONNode( null, jsn );
			}

			public static Dux FromJSONNode( string key, JSONNode jsn ) {
				Dux dux;
				if (jsn is JSONObject jso) dux = FromJSONObject( key, jso );
				else if (jsn is JSONArray jsa) dux = FromJSONArray( key, jsa );
				else dux = FromJSONValue( key, jsn );
				return dux;
			}

			public static Dux FromJSONObject( string key, JSONObject jso ) {
				var dux = new DuxNamedList( RemovePropsFromKey( key ), null, GetPropsFromKey( key ) );
				foreach (var item in jso)
					dux.Add( FromJSONNode( item.Key, item.Value ) );
				return dux;
			}

			public static Dux FromJSONArray( string key, JSONArray jsa ) {
				var dux = new DuxIndexedList( RemovePropsFromKey( key ), null, GetPropsFromKey( key ) );
				foreach (var item in jsa)
					dux.Add( FromJSONNode( item.Key, item.Value ) );
				return dux;
			}

			public static Dux FromJSONValue( string keyWithProps, JSONNode jsn ) {
				var s = jsn.AsString;
				var key = RemovePropsFromKey( keyWithProps );
				var props = GetPropsFromKey( keyWithProps );
				Dux dux = null;
				if (jsn is JSONNumber) {
					if (int.TryParse( s, out int i )) dux = new DuxValue( key, i, props );
					if (long.TryParse( s, out long l )) dux = new DuxValue( key, l, props );
					else if (float.TryParse( s, out float f )) dux = new DuxValue( key, f, props );
					else if (double.TryParse( s, out double d )) dux = new DuxValue( key, d, props );
				}
				else if (jsn is JSONBool) dux = new DuxValue( key, jsn.AsBool, props );
				else dux = new DuxValue( key, s, props );
				return dux;
			}

			public static char[] GetPropsFromKey( string keyWithPropps ) {
				if (keyWithPropps is null) return null;
				List<char> propsList = new List<char>();
				int propsEndIndex = keyWithPropps.IndexOf( '|' );
				string propsString = propsEndIndex == -1 ? string.Empty : keyWithPropps.Substring( 0, propsEndIndex );
				foreach (var prop in propsString) propsList.Add( prop );
				return propsList.ToArray();
			}
			public static string RemovePropsFromKey( string keyWithPropps ) {
				if (keyWithPropps is null) return null;
				int propsEndIndex = keyWithPropps.IndexOf( '|' );
				if (propsEndIndex == -1) return keyWithPropps;
				return keyWithPropps.Substring( propsEndIndex + 1 );
			}

			public static bool CheckJsonImportIntegrity( string source, JSONNode imported ) {
				// O JSON.Parse as vezes emite exceção se a string JSON estiver errada, como por exemplo a 
				// exceção "Too many closing brackets", porém em alguns outros casos ele faz um parse incorreto, por
				// exemplo se tiver aspas ou opening brackets errados. Portanto fazemos um catch para os casos em que
				// emite exceção mas também verificamos o resultado do Parse caso dê certo.
				// Como o JSON.Parse faz um parse parcial se o json estiver errado, é necessário verificar se
				// o parse foi feito certo para não danificar a string do item.value
				var valueText = source.Replace( " ", "" ).Replace( "\n", "" ).Replace( "\r", "" ).Replace( ",}", "}" );
				var objectText = imported.ToString().Replace( " ", "" );
				return valueText.Length == objectText.Length;
			}

			public class CorruptedJsonException : Exception {
				public JSONNode jsn;
				public CorruptedJsonException( JSONNode jsn )
				: base( "O JSON importado está corrompido." ) {
					this.jsn = jsn;
				}
			}
		}








		public static class Export {
			public static KeyValuePair<string, JSONNode> ToSimpleJson( Dux dux, bool includeProps = false ) {
				//var prefix = (includeProps && dux.Props != null) ? string.Join( "", dux.Props ) + '|' : "";
				//var key = $"{prefix}{dux.Key}";
				JSONNode node;
				if (dux is DuxValue) {
					node = new JSONString( dux.Content.ToString() );
				}
				else if (dux is DuxIndexedList a) {
					node = new JSONArray();
					foreach (var item in a.Children)
						node.Add( ToSimpleJson( item ) );
				}
				else if (dux is DuxNamedList g) {
					node = new JSONObject();
					foreach (var item in g.Children)
						node.Add( GetKey( item, includeProps ), ToSimpleJson( item ) );
				}
				else node = dux.ToString();
				return new KeyValuePair<string, JSONNode>( GetKey( dux, includeProps ), node );
			}

			public static string GetKey( Dux dux, bool includeProps = true ) {
				var prefix = (includeProps && dux.Props != null && dux.Props.Count > 0) ? string.Join( "", dux.Props ) + '|' : "";
				var key = $"{prefix}{dux.Key}";
				return key;
			}
		}
	}
}
