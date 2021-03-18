using DataUtils;
using DataUtils.DuxDataStructure;
using DataUtils.SimpleJSON;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Teste {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		protected override void OnStartup( StartupEventArgs e ) {
			base.OnStartup( e );
			//var jsn = JSON.Parse(GZip.DecodeFromFile("data/Web.ejs",Encoding.UTF8));
			//var dux =	Dux.FromJSONNode(null, jsn);
			var jsn = GZip.DecodeFromFile( "data/Web.ejs", Encoding.UTF8 );
			var dux = Dux.FromJson( jsn );
			dux[0]["menino"] = new DuxIndexedList( "joao" ).Add( new DuxValue( "CHAVE", new DuxIndexedList( "VALOR" ).Add( 1 ).Add(2) ) );
			var item = dux[0]["menino"];
			dux[0]["menino"].SetContent( 2 );
			var duxstr = dux.ToString();
			//var newjsn = Dux.ToJSONNode( dux );
			var newjsn2 = Dux.ToSimpleJson( dux );

			//var cs = newjsn.ToString( 2 );
			var ds = newjsn2.Value.ToString( 2 );

			GZip.EncodeToFile( "data/Web.ejs", duxstr, Encoding.UTF8 );
			//File.WriteAllText( "testeDux.txt", ds );
		}
	}
}
