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
using Exocortex;
using Exocortex.Geometry3D;

namespace ExoEngine.Geometry
{
	/// <summary>
	/// Summary description for FaceUtils.
	/// </summary>
	public class FaceUtils {

		static public void GetExtents( Vector3D[] points, out Vector3D ptMin, out Vector3D ptMax ) {
			ptMin = new Vector3D( float.MaxValue, float.MaxValue, float.MaxValue );
			ptMax = new Vector3D( -float.MaxValue, -float.MaxValue, -float.MaxValue );
			foreach( Vector3D pt in points ) {
				ptMin = Vector3D.MinXYZ( ptMin, pt );
				ptMax = Vector3D.MaxXYZ( ptMax, pt );
			}
		}

		static public void GetMidPoint( Faces faces, out Vector3D ptMidPoint ) {
			Vector3D ptMin, ptMax;
			FaceUtils.GetExtents( faces, out ptMin, out ptMax );
			ptMidPoint	= ( ptMin + ptMax ) * 0.5f;
		}

		static public void GetExtents( Faces faces, out Vector3D ptMin, out Vector3D ptMax ) {
			ptMin = new Vector3D( float.MaxValue, float.MaxValue, float.MaxValue );
			ptMax = new Vector3D( -float.MaxValue, -float.MaxValue, -float.MaxValue );
			foreach( Face face in faces ) {
				foreach( Vector3D pt in face.Points ) {
					ptMin = Vector3D.MinXYZ( ptMin, pt );
					ptMax = Vector3D.MaxXYZ( ptMax, pt );
				}
			}
		}

		static public void OptimizeFaces( Faces faces ) {
			int removalCount = 0;
			Faces facesOptimized = new Faces();
			facesOptimized.AddRange( faces );
			for( int i = 0; i < faces.Count; i ++ ) {
				bool bNotRemoved = true;
				Face face = faces[i];
				for( int j = 0; j < facesOptimized.Count && bNotRemoved == true; j ++ ) {
					Face facePossibleContainer = facesOptimized[j];
					if( facePossibleContainer == face ) {
						continue;
					}
					if( Vector3D.Dot( facePossibleContainer.GetNormal(), face.GetNormal() ) < -0.9999 ) {
						Vector3D midPoint = face.GetMidPoint();
						if( facePossibleContainer.IsContained( midPoint ) == true &&
							facePossibleContainer.GetPlane().GetSign( midPoint ) == 0 ) {
							int containedPoints = 0;
							for( int p = 0; p < face.Points.Count; p ++ ) {
								if( facePossibleContainer.IsContained( face.Points[p] ) ) {
									containedPoints ++;
								}
							}
							if( containedPoints == face.Points.Count ) {
								facesOptimized.Remove( face );
								//Debug.Write( "." );
								bNotRemoved = false;
								removalCount ++;
							}
						}
					}
				}
			}
			faces.Clear();
			faces.AddRange( facesOptimized );
		}
	}
}
