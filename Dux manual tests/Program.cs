using Limcap.Duxtools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dux_manual_tests {
	class Program {
		static void Main( string[] args ) {

			var d = new DuxNamedList( "root" );
			d.Add( new DuxValue( "first", "hello", '#' ) );
			var j = Dux.Export.ToSimpleJson( d );

		}
	}
}
