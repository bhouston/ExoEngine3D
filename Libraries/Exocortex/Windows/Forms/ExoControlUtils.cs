using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Exocortex.Windows.Forms
{
	/// <summary>
	/// Summary description for ExoControlUtils.
	/// </summary>
	public class ExoControlUtils {

		static unsafe public void	SetBitmapTransparency( Bitmap bitmap, Color clrTrans ) {
			BitmapData bd = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

			int *rgba = (int *) bd.Scan0.ToPointer();
			
			for( int i = 0; i < ( bitmap.Width * bitmap.Height ); i ++ ) {
				Color clr = Color.FromArgb( rgba[i] );
				if( clr.R == clrTrans.R && clr.G == clrTrans.G && clr.B == clrTrans.B ) {
					rgba[i] = 0;
				}
			}

			bitmap.UnlockBits( bd );
		}

		static unsafe public void	CopyBitmapData( Bitmap bitmapSource, Bitmap bitmapDest ) {
			Debug.Assert( bitmapSource.Width == bitmapDest.Width );
			Debug.Assert( bitmapSource.Height == bitmapDest.Height );

			int		width	= bitmapSource.Width;
			int		height	= bitmapSource.Height;
			int		size	= width * height;

			BitmapData bdSource = bitmapSource.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
			BitmapData bdDest = bitmapDest.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

			int *rgbaSource = (int *) bdSource.Scan0.ToPointer();
			int *rgbaDest = (int *) bdDest.Scan0.ToPointer();
			for( int i = 0; i < size; i ++ ) {
				rgbaDest[i] = rgbaSource[i];
			}

			bitmapSource.UnlockBits( bdSource );
			bitmapDest.UnlockBits( bdDest );
		}
		
		static unsafe public Bitmap CreateDisabledBitmap( Bitmap original, int threshold, int disabledIntensity ) {
			Bitmap		disabledBitmap = new Bitmap( original );
			BitmapData	bd = disabledBitmap.LockBits( new Rectangle( 0, 0, disabledBitmap.Width, disabledBitmap.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

			int transparentRGBA = Color.Transparent.ToArgb();
			int disabledRGBA	= Color.FromArgb( disabledIntensity, 0, 0, 0 ).ToArgb();

			threshold *= 3;

			int *rgba = (int *) bd.Scan0.ToPointer();

			for( int i = 0; i < ( disabledBitmap.Width * disabledBitmap.Height ); i ++ ) {
				Color clr = Color.FromArgb( rgba[i] );
				if( clr.A > 0 && ((int)clr.R + (int)clr.G + (int)clr.B) < threshold ) {
					rgba[i] = disabledRGBA;
				}
				else {
					rgba[i] = transparentRGBA;
				}
			}

			disabledBitmap.UnlockBits( bd );

			return	disabledBitmap;
		}

		static unsafe public Bitmap CreateContrastedBitmap( Bitmap original, float contrast ) {
			Bitmap		lightenedBitmap = new Bitmap( original );
			BitmapData	bd = lightenedBitmap.LockBits( new Rectangle( 0, 0, lightenedBitmap.Width, lightenedBitmap.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

			int *rgba = (int *) bd.Scan0.ToPointer();

			for( int i = 0; i < ( lightenedBitmap.Width * lightenedBitmap.Height ); i ++ ) {
				Color clr = Color.FromArgb( rgba[i] );
				rgba[i] = Color.FromArgb( clr.A,
					Math2.Clamp( (int)( (clr.R - 255 ) * contrast + 255 ), 0, 255 ),
					Math2.Clamp( (int)( (clr.G - 255 ) * contrast + 255 ), 0, 255 ),
					Math2.Clamp( (int)( (clr.B - 255 ) * contrast + 255 ), 0, 255 ) ).ToArgb();
			}

			lightenedBitmap.UnlockBits( bd );

			return	lightenedBitmap;
		}
		
		static public string	ShortcutToString( Shortcut shortcut ) {
			if( shortcut == Shortcut.None ) {
				return	"";
			}

			string text = "";
			int    key  = (int)shortcut;
			int    ch   = key & 0xFF;

			if( ((int)Keys.Control & key ) != 0) {
				text += "Ctrl+";
			}
			if( ((int)Keys.Shift & key ) != 0) {
				text += "Shift+";
			}
			if( ((int)Keys.Alt & key ) != 0) {
				text += "Alt+";
			}
				
			if( (int)Shortcut.F1 <= ch && ch <= (int)Shortcut.F12 ) {
				text += "F" + (ch - (int)Shortcut.F1 + 1);
			}
			else if( shortcut == Shortcut.Del ) {
				text += "Del";
			}
			else {
				text += (char)ch;
			}

			return	text;
		} 
	}
}
