using System;
using System.Drawing;


namespace Exocortex.Windows.Forms {

	public delegate void MarginsChangedEventHandler( Margins margins );
	
	public class Margins {

		protected event MarginsChangedEventHandler Changed;

		public Margins( MarginsChangedEventHandler eventHandler ) {
			this.Changed = eventHandler;
		}

		public Rectangle	GetInnerRect( Rectangle rect ) {
			return	new Rectangle(
				rect.X + this.Left,
				rect.Y + this.Top,
				Math.Max( 0, rect.Width - ( this.Left + this.Right ) ),
				Math.Max( 0, rect.Height - ( this.Top + this.Bottom ) ) );
		}

		public Rectangle	GetOutterRect( Rectangle rect ) {
			return	new Rectangle(
				rect.X - this.Left,
				rect.Y - this.Top,
				rect.Width + ( this.Left + this.Right ),
				rect.Height + ( this.Top + this.Bottom ) );
		}

		public	void	Set( int left, int top, int right, int bottom ) {
			bool changed = false;
			changed |= UpdateMargin( ref _left, left );
			changed |= UpdateMargin( ref _top, top );
			changed |= UpdateMargin( ref _right, right );
			changed |= UpdateMargin( ref _bottom, bottom );
			if( changed ) {
				this.Changed( this );
			}
		}

		protected	bool	UpdateMargin( ref int margin, int newValue ) {
			int temp = Math.Max( newValue, 0 );
			if( margin != temp ) {
				margin = temp;
				return true;
			}
			return false;
		}

		protected int	_top		= 0;
		public	int	Top	{
			get	{	return	_top;	}
			set	{	
				if( UpdateMargin( ref _top, value ) ) {
					this.Changed( this );
				}
			}
		}

		protected int	_bottom		= 0;
		public	int	Bottom	{
			get	{	return	_bottom;	}
			set	{
				if( UpdateMargin( ref _bottom, value ) ) {
					this.Changed( this );
				}
			}
		}

		protected int	_left		= 0;
		public	int	Left	{
			get	{	return	_left;	}
			set	{
				if( UpdateMargin( ref _left, value ) ) {
					this.Changed( this );
				}
			}
		}
		
		protected int	_right		= 0;
		public	int	Right	{
			get	{	return	_right;	}
			set	{
				if( UpdateMargin( ref _right, value ) ) {
					this.Changed( this );
				}
			}
		}
	}
}
