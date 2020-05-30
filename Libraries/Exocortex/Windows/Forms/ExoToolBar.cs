using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Exocortex.Windows.Forms {

	public class ExoToolBar : System.Windows.Forms.Control {

		static public readonly int	 sToolBarHeight	= 24;

		public ExoToolBar() {
			this.Layout	+= new LayoutEventHandler( this.OnLayout );
			this.Paint	+= new PaintEventHandler( this.OnPaint );
			Color c = SystemColors.Control;
			float r = 1.05f;
			this.BackColor = Color.FromArgb( (int)( c.R * r ), (int)( c.G * r ), (int)( c.B * r ) );
			this.ResizeRedraw = true;
		}

		protected ToolTip _toolTip = null;
		public void SetupButtons() {
			this.SetupButtons( 0, false );
		}

		public void AddSeparator() {
			this.Controls.Add( new ExoToolBarSeparator() );
		}
			
		public void SetupButtons( int spacing, bool outlines ) {
			Debug.Assert( _toolTip == null );

			// Create the ToolTip and associate with the Form container.
			_toolTip = new ToolTip();

			// Set up the delays for the ToolTip.
			_toolTip.AutoPopDelay = 5000;
			_toolTip.InitialDelay = 700;
			_toolTip.ReshowDelay = 500;
			// Force the ToolTip text to be displayed whether or not the form is active.
			_toolTip.ShowAlways = true;
         
			// Set up the ToolTip text for the Button and Checkbox.
			int x = 2;
			int y = 1;
			foreach( Control control in this.Controls ) {
				if( ! ( control is ComboBox ) ) {
					control.BackColor = this.BackColor;
				}
				control.Location = new Point( x, y );
				control.Height = sToolBarHeight - 2;
				x += control.Width + spacing;
				if( control is ButtonClient ) {
					ButtonClient bc = (ButtonClient) control;
					string text = bc.CommandButton.TooltipText;
					if( text == "" ) {
						text = bc.CommandButton.Text;
					}
					if( bc.CommandButton.Shortcut != Shortcut.None ) {
						text += " (" + bc.CommandButton.Shortcut.ToString() + ")";
					}
					bc.Outline = outlines;
					_toolTip.SetToolTip( control, text );
				}
			}
		}

		/*protected Margins	_margins	= null;
		public Margins	Margins {
			get	{	return	_margins;	}
		}

		protected void OnMarginsChanged( Margins newMargins ) {
			this.Invalidate();
		}		*/

		protected void OnLayout( object sender, System.Windows.Forms.LayoutEventArgs e) {
			int height = ExoToolBar.sToolBarHeight;// + this._margins.Top + this._margins.Bottom;
			this.SetBounds( 0, 0, 0, height, BoundsSpecified.Height );
		}

		protected void OnPaint( object sender, PaintEventArgs paintEvent ) {
			Rectangle rect		= new Rectangle( 0, 0, this.Width, this.Height );
			//Rectangle rectInner	= this.Margins.GetInnerRect( rectOuter );
/*			Point[] ptsInner  = new Point[] {
												new Point( rectInner.Left + 1,	rectInner.Top + 0 ),
												new Point( rectInner.Left + 0,	rectInner.Top + 1 ),
												new Point( rectInner.Left + 0,	rectInner.Bottom - 3 ),
												new Point( rectInner.Left + 1,	rectInner.Bottom - 0 ),
												new Point( rectInner.Right - 1,	rectInner.Bottom - 0 ),
												new Point( rectInner.Right - 0,	rectInner.Bottom - 3 ),
												new Point( rectInner.Right - 0,	rectInner.Top + 1 ),
												new Point( rectInner.Right - 1,	rectInner.Top + 0 ) };*/

			Graphics gc = paintEvent.Graphics;
			gc.Clear( SystemColors.Control );
			SolidBrush brushDark = new SolidBrush( this.BackColor );
			Pen penDark = new Pen( this.BackColor );
			gc.FillRectangle( brushDark, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2 );
			gc.DrawLine( penDark, rect.X, rect.Y + 2, rect.X, rect.Y + rect.Height - 3 ); 
			gc.DrawLine( penDark, rect.X + rect.Width - 1, rect.Y + 2, rect.X + rect.Width - 1, rect.Y + rect.Height - 3 ); 
		}
	}
}
