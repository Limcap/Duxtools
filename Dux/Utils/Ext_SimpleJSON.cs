using Limcap.Dux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limacap.Extensions.SimpleJSON {

	public static class Ext_SimpleJson {

		public static string Preview( this JSONNode arr, int limit = 0, bool oneline = false ) {
			StringBuilder sb = new StringBuilder();
			limit = limit <= 0 ? arr.Count : limit;
			int i = 1;
			foreach (var child in arr) {
				string childStr;
				if (child.Value.GetType() == typeof( JSONObject )) {
					childStr = child.Value.AsObject.Preview( limit, oneline );
					sb.Append( "\"" + child.Key + "\":" + childStr + (oneline ? ", " : ",\n") );
				}
				else if (child.Value.GetType() == typeof( JSONArray )) {
					childStr = child.Value.AsObject.Preview( limit, oneline );
					if (i <= limit) sb.Append( childStr + (oneline ? ", " : "\n") );
				}
				else {
					childStr = child.Value.Value;
					sb.Append( "\"" + childStr + "\"" );
				}
			}
			return sb.ToString();
		}
	}
}
