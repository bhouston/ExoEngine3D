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
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using Exocortex;
using Exocortex.Collections;
using Exocortex.Geometry3D;
using Exocortex.OpenGL;

namespace ExoEngine.Geometry {

	public class Camera {

		//---------------------------------------------------------------------------------

		public Camera() {
			this.SetCamera( new Matrix3D() );
			this.SetClipDistances( 10, 1000 );
			this.SetViewport( 5, 5 );
		}
		
		//---------------------------------------------------------------------------------

		protected Matrix3D	_xfrm;
		protected Matrix3D	_xfrmInverse;
		protected Vector3D		_translation;		
		protected Vector3D		_forwardAxis;	// forward = + z
		protected Vector3D		_upAxis;		// up = + y
		protected Vector3D		_rightAxis;		// right = - x

		public Matrix3D	Transform {
			get {	return	_xfrm;	}
		}
		public Matrix3D	TransformInverse {
			get {	return	_xfrmInverse;	}
		}
		public Vector3D		Translation {
			get {	return	_translation;	}
		}
		public Vector3D		ForwardAxis {
			get {	return	_forwardAxis;	}
		}
		public Vector3D		UpAxis {
			get {	return	_upAxis;	}
		}
		public Vector3D		RightAxis {
			get {	return	_rightAxis;	}
		}

		public	void	SetCamera( Matrix3D xfrm ) {
			_xfrmInverse	= xfrm;
			_xfrm			= _xfrmInverse.GetInverse();
			_translation	= _xfrmInverse.ExtractTranslation();
			xfrm.ExtractBasis(
				out _rightAxis,		// x axis
				out _upAxis,		// y axis
				out _forwardAxis	// z axis
				);
			_rightAxis = -_rightAxis;
			_bFrustumPlanesDirty = true;
		}

		//---------------------------------------------------------------------------------

		protected float	_viewportWidth;
		protected float	_viewportHeight;

		public float	ViewportWidth {
			get {	return	_viewportWidth;	}
		}
		public float	ViewportHeight {
			get {	return	_viewportHeight;	}
		}

		public void	SetViewport( float viewportWidth, float viewportHeight ) {
			_viewportWidth	= viewportWidth;
			_viewportHeight	= viewportHeight;
			_bFrustumPlanesDirty = true;
		}

		//---------------------------------------------------------------------------------

		protected float	_nearClipDistance;
		protected float	_farClipDistance;

		public float	NearClipDistance {
			get {	return	_nearClipDistance;	}
		}
		public float	FarClipDistance {
			get {	return	_farClipDistance;	}
		}

		public void	SetClipDistances( float nearClipDistance, float farClipDistance ) {
			_nearClipDistance	= nearClipDistance;
			_farClipDistance	= farClipDistance;
			_bFrustumPlanesDirty = true;
		}

		//---------------------------------------------------------------------------------

		protected bool	_bFrustumPlanesDirty = true;

		protected Plane3D	_nearPlane;
		protected Plane3D	_farPlane;
		protected Plane3D	_topPlane;
		protected Plane3D	_bottomPlane;
		protected Plane3D	_rightPlane;
		protected Plane3D	_leftPlane;

		public Plane3D	NearPlane {
			get {
				SyncFrustumPlanes();
				return	_nearPlane;
			}
		}
		public Plane3D	FarPlane {
			get {
				SyncFrustumPlanes();
				return	_farPlane;
			}
		}
		public Plane3D	TopPlane {
			get {
				SyncFrustumPlanes();
				return	_topPlane;
			}
		}
		public Plane3D	BottomPlane {
			get {
				SyncFrustumPlanes();
				return	_bottomPlane;
			}
		}
		public Plane3D	RightPlane {
			get {
				SyncFrustumPlanes();
				return	_rightPlane;
			}
		}
		public Plane3D	LeftPlane {
			get {
				SyncFrustumPlanes();
				return	_leftPlane;
			}
		}

		protected void	SyncFrustumPlanes() {
			if( _bFrustumPlanesDirty == false ) {
				return;
			}

			Vector3D nearClipTranslation	= _translation + _forwardAxis * _nearClipDistance;
			Vector3D farClipTranslation	= _translation + _forwardAxis * _nearClipDistance;

			_topPlane = Plane3D.FromNormalAndPoint( _forwardAxis, _translation + nearClipTranslation );
			_farPlane = Plane3D.FromNormalAndPoint( _forwardAxis, _translation + farClipTranslation );

			Vector3D viewportUpTranslation		= _upAxis * _viewportHeight / 2; 
			Vector3D viewportRightTranslation	= _rightAxis * _viewportWidth / 2; 

			Vector3D topNormal = Vector3D.Cross( _rightAxis, nearClipTranslation + viewportUpTranslation ).GetUnit();
			_topPlane = Plane3D.FromNormalAndPoint( topNormal, _translation + farClipTranslation );

			Vector3D bottomNormal = Vector3D.Cross( - _rightAxis, nearClipTranslation - viewportUpTranslation ).GetUnit();
			_bottomPlane = Plane3D.FromNormalAndPoint( bottomNormal, _translation + farClipTranslation );

			Vector3D rightNormal = Vector3D.Cross( - _upAxis, nearClipTranslation + viewportRightTranslation ).GetUnit();
			_rightPlane = Plane3D.FromNormalAndPoint( rightNormal, _translation + farClipTranslation );

			Vector3D leftNormal = Vector3D.Cross( _upAxis, nearClipTranslation - viewportRightTranslation ).GetUnit();
			_leftPlane = Plane3D.FromNormalAndPoint( leftNormal, _translation + farClipTranslation );

			_bFrustumPlanesDirty = false;
		}

		//---------------------------------------------------------------------------------

	}
}
