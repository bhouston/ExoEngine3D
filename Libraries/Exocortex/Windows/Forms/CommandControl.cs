using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

using Exocortex;
using Exocortex.Collections;

namespace Exocortex.Windows.Forms {

	//================================================================================
	//================================================================================
	
	/*public interface CommandControlClient {
		void	ClientUpdate();		
	}

	public class CommandControlClients : CollectionBase2 {
		
		public virtual void Add( CommandControlClient o ){
			this.List.Add( o );        
		}
		public virtual CommandControlClient this[int index]	{
			get { return (CommandControlClient) this.List[index];	}
			set { this.List[index] = value;				}
		}

	}*/

	//================================================================================
	//================================================================================

	// A delegate type for hooking up change notifications.
	public delegate void CommandControlChangedEventHandler( CommandControl commandControl );
		
	public abstract class CommandControl {

		public CommandControl() {
		}

		public CommandControls	CommandControls = null;

		// An event that clients can use to be notified whenever the
		// elements of the list change.
		public event CommandControlChangedEventHandler Changed;

		// Invoke the Changed event; called whenever list changes
		protected virtual void OnChanged() {
			if( Changed != null ) {
				Changed( this );
			}
		}

		protected Shortcut _shortcut = Shortcut.None;
		public Shortcut Shortcut {
			get {	return	_shortcut;	}
			set {	_shortcut = value;	}
		}

		protected string _text = "n/a";
		public string Text {
			get {	return	_text;	}
			set {	_text = value;	}
		}

		protected string _tooltipText = "";
		public string TooltipText {
			get {	return	_tooltipText;	}
			set {	_tooltipText = value;	}
		}

		protected bool _enabled = true;
		public bool Enabled {
			get {	return	_enabled;	}
			set {	_enabled = value;	}
		}

		public abstract void Update();

	}


}
