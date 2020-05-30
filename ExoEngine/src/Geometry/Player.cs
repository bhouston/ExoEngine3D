/*
 * BSD Licence:
 * Copyright (c) 2001, Ben Houston [ ben@exocortex.org ]
 * Exocortex Technologies [ www.exocortex.org ]
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the 
 * documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

using Exocortex;
using Exocortex.Geometry3D;

namespace ExoEngine.Geometry {

	public class Player {

		//---------------------------------------------------------------------------------

		public Player() {
		}

		//---------------------------------------------------------------------------------

		public void Reset( World world ) {
			this._world = world;

			// rise body up to height
			Vector3D feetLocation = _translationBody - (this.World.LocalUpAxis * _bodyHeight);
			Vector3D raiseDelta = MoveTo( _translationBody, feetLocation ) - feetLocation;
			_translationBody += raiseDelta;
		}
		
		//---------------------------------------------------------------------------------

		protected World _world = null;
		public World World {
			get	{	return	_world;		}
		}

		//---------------------------------------------------------------------------------

		protected ArrayList _vKeys = new ArrayList();

		protected float		_forwardVelocity	= 0;
		protected float		_rightVelocity		= 0;

		protected float		_rotationVelocity		= 0;
		protected float		_rotationLookVelocity	= 0;

		protected void UpdateVelocities() {
			bool keyUp		= _vKeys.Contains( Keys.Up );
			bool keyDown	= _vKeys.Contains( Keys.Down );
			bool keyLeft	= _vKeys.Contains( Keys.Left );
			bool keyRight	= _vKeys.Contains( Keys.Right );
			bool keyAlt		= _vKeys.Contains( Keys.Menu );
			bool keyCtrl	= _vKeys.Contains( Keys.ControlKey );

			/*Vector3D forward	= this.World.LocalForwardAxis * 25;
			Vector3D right	= this.World.LocalRightAxis * 25;
			Vector3D up		= this.World.LocalUpAxis * 25;*/

			_forwardVelocity		= 0;
			_rightVelocity			= 0;
			_rotationVelocity		= 0;
			_rotationLookVelocity	= 0;

			//_xfrmMovement.SetToIdentity();

			if( ! keyCtrl && ! keyAlt ) {
				if( keyUp ) {
					_forwardVelocity += 1;
				}
				if( keyDown ) {
					_forwardVelocity -= 1;
				}
				if( keyRight ) {
					_rotationVelocity -= 0.005f;
				}
				if( keyLeft ) {
					_rotationVelocity += 0.005f;
				}
			}
			else if( keyCtrl && ! keyAlt ) {
				/*if( keyUp ) {
					_xfrmMovement.Translate( -up );
				}
				if( keyDown ) {
					_xfrmMovement.Translate( up );
				}  */
				if( keyRight ) {
					_rightVelocity += 1;
				}
				if( keyLeft ) {
					_rightVelocity -= 1;
				}
			}
			else if( ! keyCtrl && keyAlt ) {
				if( keyUp ) {
					_rotationLookVelocity -= 0.005f;
				}
				if( keyDown ) {
					_rotationLookVelocity += 0.005f;
				}
				/*if( keyRight ) {
					_xfrmMovement.Rotate( forward, -0.005f );
				}
				if( keyLeft ) {
					_xfrmMovement.Rotate( forward, 0.005f );
				}*/
			}
		}

		public void OnKeyDown( Keys key ) {
			if( _vKeys.Contains( key ) == false ) {
				_vKeys.Add( key );
			}
			UpdateVelocities();
		}
		public void OnKeyUp( Keys key ) {
			_vKeys.Remove( key );
			UpdateVelocities();
		}

		//---------------------------------------------------------------------------------

		protected DateTime	_timeLastUpdate	= DateTime.Now;
		public	void	Advance() {
			
	
			if( _rotationLookVelocity == 0 && _rotationVelocity == 0 && _forwardVelocity == 0 && _rightVelocity == 0 ) {
				_timeLastUpdate	= DateTime.Now;
				return;
			}
			
			DateTime now = DateTime.Now;
			TimeSpan span = now - _timeLastUpdate;
			float seconds = Math.Min( span.Milliseconds / 1000.0f, 0.5f ) * 20;
			_timeLastUpdate = now;

			Vector3D localUp		 = this.World.LocalUpAxis;
			Vector3D localRight	 = this.World.LocalRightAxis;
			Vector3D localForward = this.World.LocalForwardAxis;

			// updating rotations are easy...
			_rotationBody += seconds * ( _rotationVelocity * 30 );
			_rotationLook = Math2.Clamp( _rotationLook + seconds * _rotationLookVelocity * 30, -1.4f, 1.4f );


			Matrix3D xfrm = Matrix3D.FromRotation( localUp, _rotationBody );

			Vector3D localVelocity =
				seconds * _forwardVelocity * localForward * 35 + 
				seconds * _rightVelocity   * localRight   * 35;

			Vector3D absoluteVelocity = xfrm * localVelocity;
			
			Vector3D absoluteUp		 = xfrm * this.World.LocalUpAxis;
			Vector3D absoluteRight	 = xfrm * this.World.LocalRightAxis;
			Vector3D absoluteForward	 = xfrm * this.World.LocalForwardAxis;

			//Debug.WriteLine( "Velocity: " + velocityBody );
			//Debug.WriteLine( "TimeStep Velocity: " + incrementalMovement );


			//Vector3D extentCenter	= xfrm * ( localForward * _bodyRadius );
			//Vector3D extentTop		= localUp * _bodyRadius;
			//Vector3D extentBottom	= - localUp * ( _bodyHeight - _liftSize );

			//Debug.WriteLine( "Old Position: " + _translationBody );
			if( absoluteVelocity.GetMagnitude() > 0 ) {
				Vector3D moveDirection = absoluteVelocity.GetUnit();
				Vector3D centerDelta	= SmartStep( _translationBody, moveDirection, _bodyRadius, absoluteVelocity.GetMagnitude() ); //+ extentCenter, _translationBody + extentCenter + absoluteVelocity ) - ( _translationBody + extentCenter ) - forwardStep;
				Vector3D topDelta	= SmartStep( _translationBody + absoluteUp * _bodyRadius, moveDirection, _bodyRadius, absoluteVelocity.GetMagnitude() ); //MoveTo( _translationBody + extentTop, _translationBody + extentTop + absoluteVelocity ) - ( _translationBody + extentTop )    - forwardStep;
				Vector3D bottomDelta	= SmartStep( _translationBody - absoluteUp * ( _bodyHeight - _liftSize ), moveDirection, _bodyRadius, absoluteVelocity.GetMagnitude() ); // MoveTo( _translationBody + extentBottom, _translationBody + extentBottom + absoluteVelocity ) - ( _translationBody + extentBottom ) - forwardStep;

				_translationBody += Vector3D.Min( Vector3D.Min( centerDelta, topDelta ), bottomDelta );
			}

			_translationBody += SmartStep( _translationBody, absoluteRight, _bodyRadius, 0 ); 
			_translationBody += SmartStep( _translationBody, -absoluteRight, _bodyRadius, 0 ); 
			
			_translationBody += SmartStep( _translationBody, -absoluteUp, _bodyHeight, _liftSize ); 
		}

		public Vector3D SmartStep( Vector3D origin, Vector3D direction, float buffer, float move ) {
			return	MoveTo( origin, origin + ( direction * ( buffer + move ) ) ) - ( origin + ( buffer * direction )) - direction;
		}

		public	Vector3D	GetLiftStep( Vector3D origin, Vector3D lift, Vector3D translation ) {
			Vector3D stepA = MoveTo( origin, origin + lift );
			Vector3D stepB = MoveTo( stepA, stepA + translation );
			return MoveTo( stepB, stepB - lift * 2 );
		}

		public	Vector3D	MoveTo( Vector3D start, Vector3D end ) {
			Face face = this.World.BSPTreeRoot.GetCollision( start, end );
			if( face == null ) {
				return	end;
			}
			//Debug.WriteLine( id );
			return	face.Plane.GetIntersection( start, end );
		}

		//---------------------------------------------------------------------------------

		protected float	_liftSize	= 85;
		public float	LiftSize {
			get	{	return	_liftSize;	}
			set	{	_liftSize = value;	}
		}

		protected float	_bodyRadius	= 100;
		public float	BodyRadius {
			get	{	return	_bodyRadius;	}
			set	{	_bodyRadius = value;	}
		}

		protected float	_bodyHeight	= 150;
		public float	BodyHeight {
			get	{	return	_bodyHeight;	}
			set	{	_bodyHeight = value;	}
		}

		//---------------------------------------------------------------------------------

		protected Vector3D	_translationBody	= Vector3D.Origin;
		public Vector3D	TranslationBody {
			get	{	return	_translationBody;	}
			set	{	_translationBody = value;	}
		}

		protected float	_rotationBody	= 0;
		public float	RotationBody {
			get	{	return	_rotationBody;	}
			set	{	_rotationBody = value;	}
		}

		public Matrix3D	GetBodyFrameOfReference() {
			Matrix3D	xfrm = new Matrix3D();
			xfrm.Translate( _translationBody );
			xfrm.Rotate( this.World.LocalUpAxis, _rotationBody );
			xfrm.Rotate( this.World.LocalRightAxis, (float)( Math2.PI / 2 ) );
			return	xfrm;
		}

		//---------------------------------------------------------------------------------

		protected float	_rotationLook	= 0;
		public float	RotationLook {
			get	{	return	_rotationLook;	}
			set	{	_rotationLook = value;	}
		}
	
		public Matrix3D	GetLookFrameOfReference() {
			Matrix3D	xfrm = new Matrix3D();
			xfrm.Translate( _translationBody );
			xfrm.Rotate( this.World.LocalUpAxis, _rotationBody );
			xfrm.Rotate( this.World.LocalRightAxis, (float)( Math2.PI / 2 ) + _rotationLook );
			return	xfrm;
		}

		//---------------------------------------------------------------------------------
	}

}
