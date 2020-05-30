// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.


using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Exocortex;
using Exocortex.Collections;
using Exocortex.Mathematics;
using Exocortex.Geometry3D;

using ExoEngine.Geometry;

namespace ExoEngine.BSPTree {

	public class BSPProfiler {

		//----------------------------------------------------------------------------------
		
		public BSPProfiler( Plane3D plane, Faces faces ) {
			_plane = plane;
			_faces = faces;
		}

		//----------------------------------------------------------------------------------

		protected Plane3D	_plane	= Plane3D.Zero;
		protected Faces		_faces	= null;

		public Plane3D	Plane {
			get {   return _plane;	}
		}

		//----------------------------------------------------------------------------------

		protected void CalculateCounts() {

			_negativeCount	= 0;
			_coplanarCount	= 0;
			_positiveCount	= 0;
			_cutCount		= 0;

			_currentStatistic = Statistic.Counts;

			foreach( Face face in _faces ) {
				ProcessFace( face );
			}
		}

		protected void CalculateFaces() {

			_negativeFaces	= new Faces();
			_coplanarFaces	= new Faces();
			_positiveFaces	= new Faces();

			_currentStatistic = Statistic.Faces;

			foreach( Face face in _faces ) {
				ProcessFace( face );
			}
		}
		
		//----------------------------------------------------------------------------------

		protected void ProcessFace( Face face ) {
			if( face.Points.Count < 3 ) {
				return;
			}
			
			Exocortex.Geometry3D.Sign sign = _plane.GetSign( face );
			//Debug.WriteLine( "plane.GetSign( face ) = " + sign.ToString() );
			switch( sign ) {

			case Exocortex.Geometry3D.Sign.Negative:
				RecordNegativeFace( face );
				break;

			case Exocortex.Geometry3D.Sign.Zero:
				if( Vector3D.Dot( _plane.Normal, face.GetNormal() ) > 0 ) {
					RecordCoplanarFace( face );
				}
				else {
					RecordNegativeFace( face );
				}
				break;

			case Exocortex.Geometry3D.Sign.Positive:
				RecordPositiveFace( face );
				break;

			case Exocortex.Geometry3D.Sign.Mixed:
				RecordCutFace( face );
				Face faceA = (Face) face.Clone();
				_plane.ClipPolygon( faceA );
				if( _plane.GetSign( faceA ) != Exocortex.Geometry3D.Sign.Mixed ) {
					ProcessFace( faceA );
				}
				Face faceB = (Face) face.Clone();
				_plane.GetFlipped().ClipPolygon( faceB );
				if( _plane.GetSign( faceB ) != Exocortex.Geometry3D.Sign.Mixed ) {
					ProcessFace( faceB );
				}
				break;

			default:
				Debug.Assert( false );
				break;
			}
		}
		
		//-------------------------------------------------------------------------

		protected enum Statistic {
			None,
			Counts,
			Faces
		}

		protected Statistic	_currentStatistic = Statistic.None;

		protected void RecordCoplanarFace( Face face ) {
			switch( _currentStatistic ) {
			case Statistic.Counts:
				_coplanarCount ++;
				break;
			case Statistic.Faces:
				_coplanarFaces.Add( face );
				break;
			default:
				Debug.Assert( false );
				break;
			}
		}
		protected void RecordPositiveFace( Face face ) {
			switch( _currentStatistic ) {
			case Statistic.Counts:
				_positiveCount ++;
				break;
			case Statistic.Faces:
				_positiveFaces.Add( face );
				break;
			default:
				Debug.Assert( false );
				break;
			}
		}
		protected void RecordNegativeFace( Face face ) {
			switch( _currentStatistic ) {
			case Statistic.Counts:
				_negativeCount ++;
				break;
			case Statistic.Faces:
				_negativeFaces.Add( face );
				break;
			default:
				Debug.Assert( false );
				break;
			}
		}
		protected void RecordCutFace( Face face ) {
			switch( _currentStatistic ) {
			case Statistic.Counts:
				_cutCount ++;
				break;
			case Statistic.Faces:
				break;
			default:
				Debug.Assert( false );
				break;
			}
		}

		//----------------------------------------------------------------------------------

		protected int	_coplanarCount	= -1;
		protected int	_positiveCount	= -1;
		protected int	_negativeCount	= -1;
		protected int	_cutCount		= -1;

		public int	CoplanarCount {
			get {
				if( _coplanarCount == -1 ) {
					CalculateCounts();
					Debug.Assert( _coplanarCount != -1 );
				}
				return _coplanarCount;
			}
		}
		public int	PositiveCount {
			get {
				if( _positiveCount == -1 ) {
					CalculateCounts();
					Debug.Assert( _positiveCount != -1 );
				}
				return _positiveCount;	
			}
		}
		public int	NegativeCount {
			get {
				if( _negativeCount == -1 ) {
					CalculateCounts();
					Debug.Assert( _negativeCount != -1 );
				}
				return _negativeCount;	
			}
		}
		public int	CutCount {
			get {
				if( _cutCount == -1 ) {
					CalculateCounts();
					Debug.Assert( _cutCount != -1 );
				}
				return _cutCount;		
			}
		}

		//----------------------------------------------------------------------------------

		protected int	_cutPenalty			= 5;
		protected int	_unbalancedPenalty	= 1;

		public int	CutPenalty {
			get {	return	_cutPenalty;	}
			set	{	_cutPenalty = value;	}
		}
		public int	UnbalancedPenalty {
			get {	return	_unbalancedPenalty;	}
			set	{	_unbalancedPenalty = value;	}
		}

		public int	GetCost() {
			return	this.CutCount * _cutPenalty + Math.Abs( this.PositiveCount - this.NegativeCount ) * _unbalancedPenalty;
		}

		//----------------------------------------------------------------------------------

		protected Faces	_coplanarFaces	= null;
		protected Faces	_positiveFaces	= null;
		protected Faces	_negativeFaces	= null;

		public Faces CoplanarFaces {
			get {
				if( _coplanarFaces == null ) {
					CalculateFaces();
					Debug.Assert( _coplanarFaces != null );
				}
				return _coplanarFaces;
			}
		}
		public Faces PositiveFaces {
			get {
				if( _positiveFaces == null ) {
					CalculateFaces();
					Debug.Assert( _positiveFaces != null );
				}
				return _positiveFaces;
			}
		}
		public Faces NegativeFaces {
			get {
				if( _negativeFaces == null ) {
					CalculateFaces();
					Debug.Assert( _negativeFaces != null );
				}
				return _negativeFaces;
			}
		}


		/*public int  CoplanarCount = 0;
		public int	PositiveCount = 0;
		public int	NegativeCount = 0;
			
		public void CountFaces() {
			foreach( Face f in this.Faces ) {
				CountFace( f );
			}
		}
		protected void CountFace( Face face ) {
			if( face.Points.Count < 3 ) {
				return;
			}
			Sign sign = this.Plane.GetSign( face );
			if( sign == Sign.Zero ) {
				if( Vector3D.Dot( this.Plane.Normal, face.GetNormal() ) > 0 ) {
					this.CoplanarCount ++;
				}
				else {
					this.NegativeCount ++;
				}
			}
			else if( sign == Sign.Positive ) {
				this.PositiveCount ++;
			} 
			else if( sign == Sign.Negative ) {
				this.NegativeCount ++;
			}
			else if( sign == Sign.Mixed ) {
				this.Cuts ++;
				Face faceA = (Face) face.Clone();
				faceA.ClipByPlane( this.Plane );
				faceA.Optimize();
				if( this.Plane.GetSign( faceA ) != Sign.Mixed ) {
					CountFace( faceA );
				}
				Face faceB = (Face) face.Clone();
				faceB.ClipByPlane( this.Plane.GetFlipped() );
				faceB.Optimize();
				if( this.Plane.GetSign( faceB ) != Sign.Mixed ) {
					CountFace( faceB );
				}
			}
		}*/

	}
}
