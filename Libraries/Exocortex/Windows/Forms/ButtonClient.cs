using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using Exocortex.Imaging;

namespace Exocortex.Windows.Forms {

	public class ButtonClient  : Button {

		protected Bitmap _bitmap			= null;
		protected Bitmap _bitmapDark		= null;
		protected Bitmap _bitmapUnselected	= null;

		public ButtonClient( CommandButton commandButton ) {
			_commandButton = commandButton;
			_commandButton.Changed += new CommandControlChangedEventHandler( this.ClientUpdate );
			this.Dock = DockStyle.None;
			Debug.Assert( _commandButton.Icon != null, "CommandButton has null icon: " + _commandButton.GetType() );
			_bitmap		= new Bitmap( 16, 16, PixelFormat.Format32bppArgb );
			BitmapUtils.CopyBitmapData( _commandButton.Icon.ToBitmap(), _bitmap );
			BitmapUtils.SetTransparency( _bitmap, Color.Magenta );
			_bitmapDark = ExoControlUtils.CreateDisabledBitmap( _bitmap, 192, 64 );
			_bitmapUnselected = ExoControlUtils.CreateContrastedBitmap( _bitmap, 0.8f );

			this.Size = new Size( 22, 22 );
			this.Layout += new LayoutEventHandler( this.OnLayout );
		}

		public bool Outline = false;

		private void OnLayout( object sender, System.Windows.Forms.LayoutEventArgs le ) {
			this.SetBounds( this.Location.X, this.Location.Y, 22, 22, BoundsSpecified.Size );   
		}

		protected	bool _mouseInside = false;
		protected override void OnMouseEnter( EventArgs e ) {
			_mouseInside = true;
			this.Invalidate();
			this.Update();
		}
		protected override void OnMouseLeave( EventArgs e ) {
			_mouseInside = false;
			this.Invalidate();
			this.Update();
		}

		protected	bool _mouseDown = false;
		protected override void OnMouseDown( MouseEventArgs e ) {
			_mouseDown = true;
			this.Invalidate();
			this.Update();
		}
		protected override void OnMouseUp( MouseEventArgs e ) {
			if( _mouseInside == true ) {
				this.PerformClick();
			}
			_mouseDown = false;
			this.Invalidate();
			this.Update();
		}

		protected override void OnPaint( PaintEventArgs pe ) {
			ButtonBase buttonBase = this;

			int width	= this.Size.Width;
			int height	= this.Size.Height;
			Graphics gc = pe.Graphics;

			if( this.Enabled == false ) {
				gc.Clear( this.BackColor );
				if( this.Outline ) {
					Color clr = this.BackColor;
					Pen pen = new Pen( Color.FromArgb( (int)( 0.8 * clr.R ), (int)( 0.8 * clr.G ), (int)( 0.8 * clr.B ) ) );
					gc.DrawRectangle( pen, 0, 0, width - 1, height - 1 );
				}
				gc.DrawImage( _bitmapDark, 3, 3 );
				return;
			}

			if( _mouseInside == false ) {
				gc.Clear( this.BackColor );
				if( this.Outline ) {
					Color clr = this.BackColor;
					Pen pen = new Pen( Color.FromArgb( (int)( 0.8 * clr.R ), (int)( 0.8 * clr.G ), (int)( 0.8 * clr.B ) ) );
					gc.DrawRectangle( pen, 0, 0, width - 1, height - 1 );
				}
				gc.DrawImage( _bitmapUnselected, 3, 3 );
				return;
			}

			if( _mouseInside == true && _mouseDown == false ) {
				gc.Clear( this.BackColor );
				gc.DrawImage( _bitmapDark, 4, 4 );
				SolidBrush brush = new SolidBrush( Color.FromArgb( 64, SystemColors.ActiveCaption ) );
				gc.FillRectangle( brush, 0, 0, width - 1, height - 1 );
				Pen pen = new Pen( SystemColors.ActiveCaption );
				gc.DrawRectangle( pen, 0, 0, width - 1, height - 1 );
				gc.DrawImage( _bitmap, 2, 2 );
				return;
			}

			if( _mouseInside == true && _mouseDown == true ) {
				gc.Clear( this.BackColor );
				SolidBrush brush = new SolidBrush( Color.FromArgb( 96, SystemColors.ActiveCaption ) );
				gc.FillRectangle( brush, 0, 0, width - 1, height - 1 );
				Pen pen = new Pen( SystemColors.ActiveCaption );
				gc.DrawRectangle( pen, 0, 0, width - 1, height - 1 );
				gc.DrawImage( _bitmap, 3, 3 );
				return;
			}

		}

		protected override void Dispose( bool disposing ) {
			_commandButton.Changed -= new CommandControlChangedEventHandler( this.ClientUpdate );
			base.Dispose( disposing );
		}

		public void ClientUpdate( CommandControl commandControl ) {
			Debug.Assert( commandControl == _commandButton );
			this.Enabled	= _commandButton.Enabled;
		}

		protected override void OnClick( EventArgs e ) {
			if( _mouseInside == true && _mouseDown == true ) {
				_commandButton.Execute();
			}
			//base.OnClick( e );
		}

		protected CommandButton	_commandButton = null;
		public CommandButton	CommandButton {
			get {	return	_commandButton;	}
		}

	}

}
