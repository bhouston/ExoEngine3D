using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Exocortex;
using Exocortex.Diagnostics;

namespace Exocortex.Windows.Forms
{
	public class CommandControls {

		//----------------------------------------------------------------------------

		protected ArrayList	_vCommandControls = new ArrayList();

		public CommandControls() {
		}

		//----------------------------------------------------------------------------
		
		public CommandControl this[ int index ] {
			get { return (CommandControl) _vCommandControls[index]; }
		}

		public int Count {
			get { return _vCommandControls.Count; }
		}

		public void	Add( CommandControl cc ) {
			Debug.Assert( _vCommandControls.Contains( cc ) == false );
			_vCommandControls.Add( cc );
		}
		public bool Contains( CommandControl cc ) {
			return _vCommandControls.Contains( cc );
		}
		public void Remove( CommandControl cc ) {
			Debug.Assert( _vCommandControls.Contains( cc ) == true );
			_vCommandControls.Remove( cc );
		}

		//----------------------------------------------------------------------------

		public CommandControl Find( Type type ) {
			for( int i = 0; i < _vCommandControls.Count; i ++ ) {
				CommandControl cc = (CommandControl) _vCommandControls[i];
				if( cc.GetType() == type ) {
					return cc;
				}
			}
			return null;
		}

		public int ExecutionCount	= 0;
		public bool IsCommandExecuting {
			get	{	return	( this.ExecutionCount > 0 );	}
		}

		public bool Execute( Type type ) {
			return	this.Execute( type, null );
		}

		public bool Execute( Type type, params object[] list ) {
			CommandControl cc = Find( type );
			if( cc == null ) {
				throw new ArgumentException( "no CommandControl of name '" + type + "' found", "type" );
			}
			if( ! ( cc is CommandButton ) ) {
				throw new ArgumentException( "CommandControl '" + type + "' is not a CommandButton", "type" );
			}

			CommandButton cb = (CommandButton) cc;
			cb.Update();
			if( cb.Enabled == true ) {
				cb.Params = list;
				//BugTracking.SetWatch( "CommandControls.LastExecute", cb );
				return cb.Execute();
			}
			return	false;
		}

		//----------------------------------------------------------------------------
	
		public CommandControl Request( Type type ) {
			CommandControl cc = this.Find( type );
			if( cc != null ) {
				return cc;
			}
			Debug.Assert( type.IsSubclassOf( typeof( CommandControl ) ) == true );
			
			ConstructorInfo ci = type.GetConstructor( new Type[0] );
			Debug.Assert( ci != null );
			
			cc = (CommandControl) ci.Invoke( null );
			cc.CommandControls = this;
			Debug.Assert( cc != null );
			Debug.Assert( cc.GetType() == type );

			this.Add( cc );

			return cc;
		}

		//----------------------------------------------------------------------------

		public MenuItem	CreateMenuItem( Type type ) {
			CommandControl cc = Request( type );
			if( cc != null && cc is CommandButton ) {
				return new MenuItemClient( (CommandButton) cc );
			}
			return null;
		}

		public Button	CreateButton( Type type ) {
			CommandControl cc = Request( type );
			if( cc != null && cc is CommandButton ) {
				return new ButtonClient( (CommandButton) cc );
			}
			return null;
		}

		//----------------------------------------------------------------------------

		public void UpdateAll() {
			for( int i = 0; i < _vCommandControls.Count; i ++ ) {
				CommandControl cc = (CommandControl) _vCommandControls[i];
				cc.Update();
			}
		}

		//----------------------------------------------------------------------------

	}


}
