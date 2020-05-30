using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Exocortex.Windows.Forms {

	public class ExocortexResources {

		//--------------------------------------------------------------------------------

		static public bool		IsStream( string fileName ) {
			string fullName = GetFullResourceName( fileName );
			Assembly localAssembly = typeof( ExocortexResources ).Assembly;
			Stream stream = localAssembly.GetManifestResourceStream( fullName );
			if( stream == null ) {
				return	false;
			}
			stream.Close();
			return	true;
		}

		static public Stream	GetStream( string fileName ) {
			string fullName = GetFullResourceName( fileName );
			Assembly localAssembly = typeof( ExocortexResources ).Assembly;
			Stream stream = localAssembly.GetManifestResourceStream( fullName );
			if( stream == null ) {
				Debug.WriteLine( "Can not find resource: '" + fullName + "'  ('" + fileName + "')" );
				OutputAvailableResources();
				throw new ResourceNotFoundException( "Can not find resource '" + fullName + "'" );
			}
			return stream;
		}
		
		static public Icon		GetIcon( string fileName ) {
			Icon icon = null;

			fileName = "Icons." + fileName;
			
			if( IsResourceCached( fileName ) ) {
				//Debug.Write( "!" );
				icon = (Icon) GetCachedResource( fileName );
			}
			else {
				//Debug.Write( "." );
				icon = new Icon( GetStream( fileName ) );
				AddResourceToCache( fileName, icon );
			}

			return icon;
		}

		static public Bitmap	GetBitmap( string fileName ) {
			Bitmap bitmap = null;

			fileName = "Bitmaps." + fileName;
			
			if( IsResourceCached( fileName ) ) {
				//Debug.Write( "!" );
				bitmap = (Bitmap) GetCachedResource( fileName );
			}
			else {
				//Debug.Write( "." );
				bitmap = new Bitmap( GetStream( fileName ) );
				AddResourceToCache( fileName, bitmap );
			}

			return bitmap;
		}

		//--------------------------------------------------------------------------------

		static protected Hashtable _resourceCache = new Hashtable();
		static protected void AddResourceToCache( string fileName, object resource ) {
			Debug.Assert( _resourceCache.Contains( fileName ) == false );
			_resourceCache.Add( fileName, resource );
		}
		static protected bool IsResourceCached( string fileName ) {
			return	_resourceCache.Contains( fileName );
		}
		static protected object	GetCachedResource( string fileName ) {
			Debug.Assert( _resourceCache.Contains( fileName ) == true );
			return	_resourceCache[ fileName ];
		}

		static protected void ClearResourceCache() {
			_resourceCache.Clear();
		}

		
		//--------------------------------------------------------------------------------

		static protected string	GetFullResourceName( string fileName ) {
			return "Exocortex.Windows.Forms." + fileName;
		}

		static protected void OutputAvailableResources() {
			Debug.WriteLine( "Available resources:" );
			Debug.Indent();
			Assembly localAssembly = typeof( ExocortexResources ).Assembly;
			string[] resourceNames = localAssembly.GetManifestResourceNames();
			Array.Sort( resourceNames );
			foreach( string name in resourceNames ) {
				Debug.WriteLine( "'" + name + "'" );
			}
			Debug.Unindent();
		}

		//--------------------------------------------------------------------------------

	}
}
