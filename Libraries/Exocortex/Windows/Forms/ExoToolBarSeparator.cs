using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using Exocortex.Imaging;

namespace Exocortex.Windows.Forms {

	public class ExoToolBarSeparator : Label {

		public ExoToolBarSeparator() {
			this.Text = "";
			this.Size = new Size( 3, 22 );
			this.Layout += new LayoutEventHandler( this.OnLayout );
		}

		private void OnLayout( object sender, System.Windows.Forms.LayoutEventArgs le ) {
			this.SetBounds( this.Location.X, this.Location.Y, 3, 22, BoundsSpecified.Width );   
		}

		protected override void OnPaint( PaintEventArgs pe ) {
			int width	= this.Size.Width;
			int height	= this.Size.Height;
			Graphics gc = pe.Graphics;

			Color clr = this.BackColor;
			Pen pen = new Pen( Color.FromArgb( (int)( 0.8 * clr.R ), (int)( 0.8 * clr.G ), (int)( 0.8 * clr.B ) ) );
			gc.DrawLine( pen, 1, 1, 1, height - 1 );
			
		}

	}

}
