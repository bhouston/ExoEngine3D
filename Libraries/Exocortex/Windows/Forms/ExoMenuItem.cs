using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Exocortex.Windows.Forms {

	/// <summary>
	/// Summary description for ExoMenuItem.
	/// </summary>
	public class ExoMenuItem : MenuItem {
		static Color window;
		static Color bgColor;
		static Color stripeColor;
		static Color selectionColor;
		static Color borderColor;
		
		static int iconSize = SystemInformation.SmallIconSize.Width + 5;
		static int itemHeight;
		static bool doColorUpdate = false;
		string shortcuttext = "";
		Bitmap icon				= null;
		Bitmap iconDark			= null;
		Bitmap iconUnselected	= null;
		static int BITMAP_SIZE = 16;
		static int STRIPE_WIDTH = iconSize + 2;
		
		// constructors
		// ---------------------------------------------------
		public ExoMenuItem() : base() {
			OwnerDraw = true;	UpdateColors();
		}	

		public ExoMenuItem(string name, EventHandler handler, Shortcut shortcut) : this(name, handler) {
			OwnerDraw = true;	this.Shortcut = shortcut;	UpdateColors();
		}
		
		public ExoMenuItem(string name, EventHandler handler) : base(name, handler) {
			OwnerDraw = true;	UpdateColors();
		}	

		public ExoMenuItem(string name ) : base(name) {
			OwnerDraw = true;	UpdateColors();
		}	

		public Bitmap Icon {
			get {
				return icon;
			}
			set {
				icon = value;
				if( icon != null ) {
					ExoControlUtils.SetBitmapTransparency( icon, Color.Magenta );
					iconDark = ExoControlUtils.CreateDisabledBitmap( icon, 192, 64 );
					iconUnselected = ExoControlUtils.CreateContrastedBitmap( icon, 0.8f );
				}
				else {
					iconDark = null;
					iconUnselected = null;
				}
			}
		}

		public string ShortcutText {
			get {
				return shortcuttext;
			}
			set {
				shortcuttext = value;
			}
		}

		static public void UpdateMenuColors() {            
			doColorUpdate = true;
		}

		private void UpdateColors() {
			window = SystemColors.Window;
			bgColor  = SystemColors.ControlLightLight;
			stripeColor = SystemColors.Control;
			selectionColor  = SystemColors.Highlight;
			borderColor = SystemColors.Highlight;

			int wa = (int)window.A;
			int wr = (int)window.R;
			int wg = (int)window.G;
			int wb = (int)window.B;

			int mna = (int)bgColor.A;
			int mnr = (int)bgColor.R;
			int mng = (int)bgColor.G;
			int mnb = (int)bgColor.B;

			int sta = (int)stripeColor.A;
			int str = (int)stripeColor.R;
			int stg = (int)stripeColor.G;
			int stb = (int)stripeColor.B;

			int sla = (int)selectionColor.A;
			int slr = (int)selectionColor.R;
			int slg = (int)selectionColor.G;
			int slb = (int)selectionColor.B;

			bgColor = Color.FromArgb(wr-(((wr-mnr)*2)/5), wg-(((wg-mng)*2)/5), wb-(((wb-mnb)*2)/5));
			stripeColor = Color.FromArgb(wr-(((wr-str)*4)/5), wg-(((wg-stg)*4)/5), wb-(((wb-stb)*4)/5));
			selectionColor = Color.FromArgb(wr-(((wr-slr)*2)/5), wg-(((wg-slg)*2)/5), wb-(((wb-slb)*2)/5));
		}

		private void DoUpdateMenuColors() {
			UpdateColors();			
			doColorUpdate = false;
		}

		// overrides
		// ---------------------------------------------------------
		protected override void OnMeasureItem(MeasureItemEventArgs e) {
			base.OnMeasureItem(e);
			
			// measure shortcut text
			if (Shortcut != Shortcut.None) {
				string text = "";
				int    key  = (int)Shortcut;
				int    ch   = key & 0xFF;
				if (((int)Keys.Control & key) > 0)
					text += "Ctrl+";
				if (((int)Keys.Shift & key) > 0)
					text += "Shift+";
				if (((int)Keys.Alt & key) > 0)
					text += "Alt+";
				
				if (ch >= (int)Shortcut.F1 && ch <= (int)Shortcut.F12)
					text += "F" + (ch - (int)Shortcut.F1 + 1);
				else {
					if ( Shortcut == Shortcut.Del) {
						text += "Del";
					}
					else {
						text += (char)ch;
					}
				}
				shortcuttext = text;
			} 
			
			if (Text == "-") {
				e.ItemHeight = 3;
				e.ItemWidth  = 4;
				return;
			}
				
			bool topLevel = ( this.Parent == this.Parent.GetMainMenu() );
			string tempShortcutText = shortcuttext;
			if ( topLevel ) {
				tempShortcutText = "";
			}
			int textwidth = (int)(e.Graphics.MeasureString(Text + tempShortcutText, SystemInformation.MenuFont).Width);
			int extraHeight = 3;
			e.ItemHeight  = SystemInformation.MenuHeight + extraHeight;
			if ( topLevel )
				e.ItemWidth  = textwidth - 5; 
			else
				e.ItemWidth   = Math.Max(160, textwidth + 50);

			// save menu item heihgt for later use
			itemHeight = e.ItemHeight;
			
		}
		
		protected override void OnDrawItem(DrawItemEventArgs e) {
			if ( doColorUpdate) {
				DoUpdateMenuColors();
			}
			
			//base.OnDrawItem(e);

			Graphics  g      = e.Graphics;
			Rectangle bounds = e.Bounds;
			bool selected = (e.State & DrawItemState.Selected) > 0;
			bool toplevel = (Parent == Parent.GetMainMenu());
			bool hasicon  = Icon != null;
			bool enabled = Enabled;
			
			DrawBackground(g, bounds, e.State, toplevel, hasicon, enabled);
			if (hasicon)
				DrawIcon(g, Icon, bounds, selected, Enabled, Checked);
			else
				if (Checked)
				DrawCheckmark(g, bounds, selected);
			
			if (Text == "-") {
				DrawSeparator(g, bounds);
			} 
			else {
				DrawMenuText(g, bounds, Text, shortcuttext, Enabled, toplevel, e.State);
			}
		}

		Bitmap bitmapCheckmark = null;

		public Color	CreateColor( int a, int r, int g, int b ) {
			return	Color.FromArgb(
				Math.Min( 255, Math.Max( 0, a ) ),
				Math.Min( 255, Math.Max( 0, r ) ),
				Math.Min( 255, Math.Max( 0, g ) ),
				Math.Min( 255, Math.Max( 0, b ) ) );
		}

		public Color	ScaleColor( Color c, float scale ) {
			return	CreateColor( c.A, (int)( c.R * scale ), (int)( c.G * scale ), (int)( c.B * scale ) );
		}


		public void DrawCheckmark(Graphics g, Rectangle bounds, bool selected) {
			if( bitmapCheckmark == null ) {
				bitmapCheckmark = ExocortexResources.GetIcon( "menuCheckmark.ico" ).ToBitmap();
				ExoControlUtils.SetBitmapTransparency( bitmapCheckmark, Color.Magenta );
			}
			int checkTop = bounds.Top + (itemHeight - BITMAP_SIZE)/2;
			int checkLeft = bounds.Left + ( STRIPE_WIDTH - BITMAP_SIZE)/2;
			if( selected ) {
				Color darker = ScaleColor( selectionColor, 0.8f ); //.A, (int)(selectionColor.R * 0.8), (int)(selectionColor.G * 0.8), (int)(selectionColor.B * 0.8) );
				g.FillRectangle( new SolidBrush(darker), checkLeft-1, checkTop-1, BITMAP_SIZE+3, BITMAP_SIZE+3 );
			}
			else {
				Color lighter = ScaleColor( selectionColor, 1.2f ); //CreateColor( selectionColor.A, (int)(selectionColor.R * 1.2), (int)(selectionColor.G * 1.2), (int)(selectionColor.B * 1.2) );
				g.FillRectangle( new SolidBrush(lighter), checkLeft-1, checkTop-1, BITMAP_SIZE+3, BITMAP_SIZE+3 );
			}
			g.DrawImage( bitmapCheckmark, checkLeft, checkTop );
			g.DrawRectangle(new Pen(borderColor), checkLeft-2, checkTop-2, BITMAP_SIZE+3, BITMAP_SIZE+3);
		}

		
		public void DrawIcon(Graphics g, Image icon, Rectangle bounds, bool selected, bool enabled, bool ischecked) {
			// make icon transparent
			int iconTop = bounds.Top + (itemHeight - BITMAP_SIZE)/2;
			int iconLeft = bounds.Left + ( STRIPE_WIDTH - BITMAP_SIZE)/2;
			if (enabled) {
				if (selected) {
					g.DrawImage( iconDark, iconLeft + 1, iconTop + 1 );
					g.DrawImage( icon, iconLeft - 1, iconTop - 1 );
				} 
				else {
					g.DrawImage( iconUnselected, iconLeft, iconTop );
				}
			} 
			else {
				g.DrawImage( iconDark, iconLeft, iconTop );
			}
		}
	
		public void DrawSeparator(Graphics g, Rectangle bounds) {
			int y = bounds.Y + bounds.Height / 2;
			g.DrawLine(new Pen(SystemColors.ControlDark), bounds.X + iconSize + 7, y, bounds.X + bounds.Width - 2, y);
		}
		
		public void DrawBackground(Graphics g, Rectangle bounds, DrawItemState state, bool toplevel, bool hasicon, bool enabled) {
			bool selected = (state & DrawItemState.Selected) > 0;
			
			if (selected || ((state & DrawItemState.HotLight) > 0)) {
				if (toplevel && selected) { 
				    // draw toplevel, selected menuitem
					bounds.Inflate(-1, 0);
					g.FillRectangle(new SolidBrush(stripeColor), bounds);
					Border3DSide borders = Border3DSide.Top | Border3DSide.Left | Border3DSide.Right;
					if( this.MenuItems.Count == 0 ) {
						borders |= Border3DSide.Bottom;
					}
					ControlPaint.DrawBorder3D(g, bounds.Left, bounds.Top, bounds.Width, 
						bounds.Height, Border3DStyle.Flat, borders );
					;
				} 
				else { 
				    // draw menuitem, selected OR toplevel, hotlighted
					if ( enabled ) {
						g.FillRectangle(new SolidBrush(selectionColor), bounds);
						g.DrawRectangle(new Pen(borderColor), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
					}
					else {
						// if disabled draw menuitem unselected
						g.FillRectangle(new SolidBrush(stripeColor), bounds);
						bounds.X += STRIPE_WIDTH;
						bounds.Width -= STRIPE_WIDTH;
						g.FillRectangle(new SolidBrush(bgColor), bounds);
					}

				}
			} 
			else {
				if (!toplevel) { 
				    // draw menuitem, unselected
					g.FillRectangle(new SolidBrush(stripeColor), bounds);
					bounds.X += STRIPE_WIDTH;
					bounds.Width -= STRIPE_WIDTH;
					g.FillRectangle(new SolidBrush(bgColor), bounds);
				} 
				else {
					// draw toplevel, unselected menuitem
					g.FillRectangle(SystemBrushes.Control, bounds);
				}
			}
		}

		public void DrawMenuText(Graphics g, Rectangle bounds, string text, string shortcut, bool enabled, bool toplevel, DrawItemState state ) {
			StringFormat stringformat = new StringFormat();
			stringformat.HotkeyPrefix = ((state & DrawItemState.NoAccelerator) > 0) ? HotkeyPrefix.Hide : HotkeyPrefix.Show;
		
			// if 3D background happens to be black, as it is the case when
			// using a high contrast color theme, then make sure text is white
			bool highContrast = false;
			bool whiteHighContrast = false;
			if ( SystemColors.Control.ToArgb() == Color.FromArgb(255,0,0,0).ToArgb() ) highContrast = true;
			if ( SystemColors.Control.ToArgb() == Color.FromArgb(255,255,255,255).ToArgb() ) whiteHighContrast = true;

			// if menu is a top level, extract the ampersand that indicates the shortcut character
			// so that the menu text is centered
			string textTopMenu = text;
			if ( toplevel ) {
				int index = text.IndexOf("&");
				if ( index != -1 ) {
					// remove it
					text = text.Remove(index,1);
				}
			}
			
			int textwidth = (int)(g.MeasureString(text, SystemInformation.MenuFont).Width);
			int x = toplevel ? bounds.Left + (bounds.Width - textwidth) / 2: bounds.Left + iconSize + 10;
			int topGap = 4;
			if ( toplevel ) topGap = 2;
			int y = bounds.Top + topGap;
			Brush brush = null;
			
			if (!enabled)
				brush = new SolidBrush(Color.FromArgb(120, SystemColors.MenuText));
			else if ( highContrast ) 
				brush = new SolidBrush(Color.FromArgb(255, SystemColors.MenuText));
			else 
				brush = new SolidBrush(Color.Black);

			if ( whiteHighContrast && ( (state & DrawItemState.HotLight) > 0 
				|| ( (state & DrawItemState.Selected) > 0 && !toplevel )) )
				brush = new SolidBrush(Color.FromArgb(255, Color.White));
			
			if ( toplevel ) text = textTopMenu;
			g.DrawString(text, SystemInformation.MenuFont, brush, x, y, stringformat);
					
			// don't draw the shortcut for top level menus
			// in case there was actually one
			if ( !toplevel ) {
				// draw shortcut right aligned
				stringformat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				g.DrawString(shortcut, SystemInformation.MenuFont, brush, bounds.Width - 10 , bounds.Top + topGap, stringformat);
			}
		}


	}

}