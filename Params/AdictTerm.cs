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

	public class AdictTerm : Dux
	{
		//public ParamList _group;
		public string Key { get; set; }
		public override string Value { get; set; }
		private readonly bool isValueContainer;

		public List<char> mods;






		public AdictTerm( string key, string value, params char[] mods ) :
		{
			this.mods = new List<char>( mods );
			SetData( key, value );
			SetMods( mods );
		}




		public AdictTerm SetData( string key, string value )
		{
			this.Key = key;
			this.Value = value;
			return this;
		}




		public AdictTerm SetMods( char[] mods )
		{
			this.mods.Clear();
			foreach (var m in mods) this.mods.Add( m );
			return this;
		}




		public AdictTerm UnsetMod( params char[] properties )
		{
			foreach (var p in properties)
				if (this.mods.Contains( p ))
					this.mods.Remove( p );
			return this;
		}




		public AdictTerm ToggleMod( char m )
		{
			if (mods.Contains( m )) mods.Remove( m );
			else mods.Add( m );
			return this;
		}




		public bool HasMod( char p )
		{
			return mods.Contains( p );
		}




		public override string ToString()
		{
			//var propstring = this.properties.Aggregate( "", ( agg, it ) => agg += it );
			var propstring = string.Join( "", mods );
			return $"{propstring} | {Key} : {Value}";
		}




		//public bool valueIsInt { get => !string.IsNullOrEmpty( value ) && value.All( c => char.IsDigit( c ) ); }
		//public bool valueIsBool { get => value.Trim().ToLower().IsIn( "true", "false" ); }

		//public int Int( int defaultVal = 0 )
		//{
		//	return defaultVal;
		//}
		//public bool Bool( bool defaultVal = false )
		//{
		//	return defaultVal;
		//}
		//public string String( string defaultVal = "", bool defaultIfEmpty = false )
		//{
		//	return defaultVal;
		//}


		public int? Int( params int[] asNull )
		{
			if (HasMod( Adict.Mod.DISABLED ) || !int.TryParse( Value, out int intval ) || asNull.Contains( intval ))
				return null;
			else return intval;
		}




		public bool? Bool()
		{
			var v = Value.Trim().ToLower();
			if (v.IsIn( "0", "false" )) return false;
			if (HasMod( Adict.Mod.DISABLED )) return null;
			if (v.IsIn( "1", "true" )) return true;
			return null;
		}




		public string String( bool nullIfEmpty = false )
		{
			if (Value.Trim().Length == 0) return null;
			else return Value;
		}




		public AdictTerm OrNull { get => Value is null ? null : this; }




















		public static implicit operator string( AdictTerm value )
		{
			return value.Value;
		}
		public static implicit operator AdictTerm( string value )
		{
			return new AdictTerm( null, value );
		}


		public static implicit operator int( AdictTerm aterm )
		{
			if (!int.TryParse( aterm.Value, out int intval )) return 0;
			else return intval;
		}
		public static implicit operator AdictTerm( int value )
		{
			return new AdictTerm( null, value.ToString() );
		}


		public static implicit operator bool( AdictTerm aterm )
		{
			var val = aterm.Value.Trim().Trim( '0' ).ToLower();
			if (val == "" || val == "false") return false; else return true;
		}
		public static implicit operator AdictTerm( bool value )
		{
			return new AdictTerm( null, value.ToString().ToLower() );
		}
	}

	//public class AdictTermValue
	//{
	//	public string Value { get; set; }

	//	public static implicit operator string(AdictTermValue value)
	//	{
	//		return value.Value;
	//	}

	//	public static implicit operator int(AdictTermValue aterm)
	//	{
	//		if (!int.TryParse(aterm.Value, out int intval)) return 0;
	//		else return intval;
	//	}

	//	public static implicit operator AdictTermValue(int value)
	//	{
	//		return new AdictTermValue() { Value = value.ToString() };
	//	}
	//}

}
