/*
 * BSD Licence:
 * Copyright (c) 2001, Lloyd Dupont (lloyd@galador.net)
 * <ORGANIZATION> 
 * All rights reserved.
 * 
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
using System.Text;
using System.Runtime.InteropServices;

namespace Exocortex.OpenGL
{
	public class GLU
	{
		public const uint GLU_EXT_object_space_tess = 1;
		public const uint GLU_EXT_nurbs_tessellator = 1;
		public const uint GLU_FALSE = 0;
		public const uint GLU_TRUE = 1;
		public const uint GLU_VERSION_1_1 = 1;
		public const uint GLU_VERSION_1_2 = 1;
		public const uint GLU_VERSION = 100800;
		public const uint GLU_EXTENSIONS = 100801;
		public const uint GLU_INVALID_ENUM = 100900;
		public const uint GLU_INVALID_VALUE = 100901;
		public const uint GLU_OUT_OF_MEMORY = 100902;
		public const uint GLU_INCOMPATIBLE_GL_VERSION = 100903;
		public const uint GLU_INVALID_OPERATION = 100904;
		public const uint GLU_OUTLINE_POLYGON = 100240;
		public const uint GLU_OUTLINE_PATCH = 100241;
		public const uint GLU_ERROR = 100103;
		public const uint GLU_NURBS_ERROR1 = 100251;
		public const uint GLU_NURBS_ERROR2 = 100252;
		public const uint GLU_NURBS_ERROR3 = 100253;
		public const uint GLU_NURBS_ERROR4 = 100254;
		public const uint GLU_NURBS_ERROR5 = 100255;
		public const uint GLU_NURBS_ERROR6 = 100256;
		public const uint GLU_NURBS_ERROR7 = 100257;
		public const uint GLU_NURBS_ERROR8 = 100258;
		public const uint GLU_NURBS_ERROR9 = 100259;
		public const uint GLU_NURBS_ERROR10 = 100260;
		public const uint GLU_NURBS_ERROR11 = 100261;
		public const uint GLU_NURBS_ERROR12 = 100262;
		public const uint GLU_NURBS_ERROR13 = 100263;
		public const uint GLU_NURBS_ERROR14 = 100264;
		public const uint GLU_NURBS_ERROR15 = 100265;
		public const uint GLU_NURBS_ERROR16 = 100266;
		public const uint GLU_NURBS_ERROR17 = 100267;
		public const uint GLU_NURBS_ERROR18 = 100268;
		public const uint GLU_NURBS_ERROR19 = 100269;
		public const uint GLU_NURBS_ERROR20 = 100270;
		public const uint GLU_NURBS_ERROR21 = 100271;
		public const uint GLU_NURBS_ERROR22 = 100272;
		public const uint GLU_NURBS_ERROR23 = 100273;
		public const uint GLU_NURBS_ERROR24 = 100274;
		public const uint GLU_NURBS_ERROR25 = 100275;
		public const uint GLU_NURBS_ERROR26 = 100276;
		public const uint GLU_NURBS_ERROR27 = 100277;
		public const uint GLU_NURBS_ERROR28 = 100278;
		public const uint GLU_NURBS_ERROR29 = 100279;
		public const uint GLU_NURBS_ERROR30 = 100280;
		public const uint GLU_NURBS_ERROR31 = 100281;
		public const uint GLU_NURBS_ERROR32 = 100282;
		public const uint GLU_NURBS_ERROR33 = 100283;
		public const uint GLU_NURBS_ERROR34 = 100284;
		public const uint GLU_NURBS_ERROR35 = 100285;
		public const uint GLU_NURBS_ERROR36 = 100286;
		public const uint GLU_NURBS_ERROR37 = 100287;
		public const uint GLU_AUTO_LOAD_MATRIX = 100200;
		public const uint GLU_CULLING = 100201;
		public const uint GLU_SAMPLING_TOLERANCE = 100203;
		public const uint GLU_DISPLAY_MODE = 100204;
		public const uint GLU_PARAMETRIC_TOLERANCE = 100202;
		public const uint GLU_SAMPLING_METHOD = 100205;
		public const uint GLU_U_STEP = 100206;
		public const uint GLU_V_STEP = 100207;
		public const uint GLU_OBJECT_PARAMETRIC_ERROR_EXT = 100208;
		public const uint GLU_OBJECT_PATH_LENGTH_EXT = 100209;
		public const uint GLU_PATH_LENGTH = 100215;
		public const uint GLU_PARAMETRIC_ERROR = 100216;
		public const uint GLU_DOMAIN_DISTANCE = 100217;
		public const uint GLU_MAP1_TRIM_2 = 100210;
		public const uint GLU_MAP1_TRIM_3 = 100211;
		public const uint GLU_POINT = 100010;
		public const uint GLU_LINE = 100011;
		public const uint GLU_FILL = 100012;
		public const uint GLU_SILHOUETTE = 100013;
		public const uint GLU_SMOOTH = 100000;
		public const uint GLU_FLAT = 100001;
		public const uint GLU_NONE = 100002;
		public const uint GLU_OUTSIDE = 100020;
		public const uint GLU_INSIDE = 100021;
		public const uint GLU_TESS_BEGIN = 100100;
		public const uint GLU_BEGIN = 100100;
		public const uint GLU_TESS_VERTEX = 100101;
		public const uint GLU_VERTEX = 100101;
		public const uint GLU_TESS_END = 100102;
		public const uint GLU_END = 100102;
		public const uint GLU_TESS_ERROR = 100103;
		public const uint GLU_TESS_EDGE_FLAG = 100104;
		public const uint GLU_EDGE_FLAG = 100104;
		public const uint GLU_TESS_COMBINE = 100105;
		public const uint GLU_TESS_BEGIN_DATA = 100106;
		public const uint GLU_TESS_VERTEX_DATA = 100107;
		public const uint GLU_TESS_END_DATA = 100108;
		public const uint GLU_TESS_ERROR_DATA = 100109;
		public const uint GLU_TESS_EDGE_FLAG_DATA = 100110;
		public const uint GLU_TESS_COMBINE_DATA = 100111;
		public const uint GLU_NURBS_MODE_EXT = 100160;
		public const uint GLU_NURBS_TESSELLATOR_EXT = 100161;
		public const uint GLU_NURBS_RENDERER_EXT = 100162;
		public const uint GLU_NURBS_BEGIN_EXT = 100164;
		public const uint GLU_NURBS_VERTEX_EXT = 100165;
		public const uint GLU_NURBS_NORMAL_EXT = 100166;
		public const uint GLU_NURBS_COLOR_EXT = 100167;
		public const uint GLU_NURBS_TEX_COORD_EXT = 100168;
		public const uint GLU_NURBS_END_EXT = 100169;
		public const uint GLU_NURBS_BEGIN_DATA_EXT = 100170;
		public const uint GLU_NURBS_VERTEX_DATA_EXT = 100171;
		public const uint GLU_NURBS_NORMAL_DATA_EXT = 100172;
		public const uint GLU_NURBS_COLOR_DATA_EXT = 100173;
		public const uint GLU_NURBS_TEX_COORD_DATA_EXT = 100174;
		public const uint GLU_NURBS_END_DATA_EXT = 100175;
		public const uint GLU_CW = 100120;
		public const uint GLU_CCW = 100121;
		public const uint GLU_INTERIOR = 100122;
		public const uint GLU_EXTERIOR = 100123;
		public const uint GLU_UNKNOWN = 100124;
		public const uint GLU_TESS_WINDING_RULE = 100140;
		public const uint GLU_TESS_BOUNDARY_ONLY = 100141;
		public const uint GLU_TESS_TOLERANCE = 100142;
		public const uint GLU_TESS_ERROR1 = 100151;
		public const uint GLU_TESS_ERROR2 = 100152;
		public const uint GLU_TESS_ERROR3 = 100153;
		public const uint GLU_TESS_ERROR4 = 100154;
		public const uint GLU_TESS_ERROR5 = 100155;
		public const uint GLU_TESS_ERROR6 = 100156;
		public const uint GLU_TESS_ERROR7 = 100157;
		public const uint GLU_TESS_ERROR8 = 100158;
		public const uint GLU_TESS_MISSING_BEGIN_POLYGON = 100151;
		public const uint GLU_TESS_MISSING_BEGIN_CONTOUR = 100152;
		public const uint GLU_TESS_MISSING_END_POLYGON = 100153;
		public const uint GLU_TESS_MISSING_END_CONTOUR = 100154;
		public const uint GLU_TESS_COORD_TOO_LARGE = 100155;
		public const uint GLU_TESS_NEED_COMBINE_CALLBACK = 100156;
		public const uint GLU_TESS_WINDING_ODD = 100130;
		public const uint GLU_TESS_WINDING_NONZERO = 100131;
		public const uint GLU_TESS_WINDING_POSITIVE = 100132;
		public const uint GLU_TESS_WINDING_NEGATIVE = 100133;
		public const uint GLU_TESS_WINDING_ABS_GEQ_TWO = 100134;
		public const double GLU_TESS_MAX_COORD = 1.0e150;
		
		[DllImport("glu32")]
		public unsafe static extern void gluBeginCurve (IntPtr nurb);
		[DllImport("glu32")]
		public static extern void gluBeginPolygon (IntPtr tess);
		[DllImport("glu32")]
		public unsafe static extern void gluBeginSurface (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern void gluBeginTrim (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern int gluBuild1DMipmaps (uint target, int component, int width, uint format, uint type, void *data);
		[DllImport("glu32")]
		public unsafe static extern int gluBuild2DMipmaps (uint target, int component, int width, int height, uint format, uint type, void *data);
		[DllImport("glu32")]
		public unsafe static extern void gluCylinder (IntPtr quad, double myBase, double top, double height, int slices, int stacks);
		[DllImport("glu32")]
		public unsafe static extern void gluDeleteNurbsRenderer (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern void gluDeleteNurbsTessellatorEXT (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern void gluDeleteQuadric (IntPtr quad);
		[DllImport("glu32")]
		public unsafe static extern void gluDeleteTess (IntPtr tess);
		[DllImport("glu32")]
		public unsafe static extern void gluDisk (IntPtr quad, double inner, double outer, int slices, int loops);
		[DllImport("glu32")]
		public unsafe static extern void gluEndCurve (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern void gluEndPolygon (IntPtr tess);
		[DllImport("glu32")]
		public unsafe static extern void gluEndSurface (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern void gluEndTrim (IntPtr nurb);
		[DllImport("glu32")]
		public unsafe static extern byte * gluErrorString (uint error);
		[DllImport("glu32")]
		public unsafe static extern void gluGetNurbsProperty (IntPtr nurb, uint property, float* data);
		[DllImport("glu32")]
		public unsafe static extern byte * gluGetString (uint name);
		[DllImport("glu32")]
		public unsafe static extern void gluGetTessProperty (IntPtr tess, uint which, double* data);
		[DllImport("glu32")]
		public unsafe static extern void gluLoadSamplingMatrices (IntPtr nurb,  float *model, float *perspective, int *view);
		[DllImport("glu32")]
		public unsafe static extern void gluLookAt (double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ, double upX, double upY, double upZ);
		[DllImport("glu32")]
		public unsafe static extern IntPtr gluNewNurbsRenderer ();
		[DllImport("glu32")]
		public unsafe static extern IntPtr gluNewNurbsTessellatorEXT ();
		[DllImport("glu32")]
		public unsafe static extern IntPtr gluNewQuadric ();
		[DllImport("glu32")]
		public unsafe static extern IntPtr gluNewTess ();
		[DllImport("glu32")]
		public unsafe static extern void gluNextContour (IntPtr tess, uint type);
//		[DllImport("glu32")]
//		public unsafe static extern void gluNurbsCallback (IntPtr nurb, uint which, void (CALLBACK *CallBackFunc)());
		[DllImport("glu32")]
		public unsafe static extern void gluNurbsCallbackDataEXT (IntPtr nurb, void* userData);
		[DllImport("glu32")]
		public unsafe static extern void gluNurbsCurve (IntPtr nurb, int knotCount, float *knots, int stride, float *control, int order, uint type);
		[DllImport("glu32")]
		public unsafe static extern void gluNurbsProperty (IntPtr nurb, uint property, float value);
		[DllImport("glu32")]
		public unsafe static extern void gluNurbsSurface (IntPtr nurb, int sKnotCount, float* sKnots, int tKnotCount, float* tKnots, int sStride, int tStride, float* control, int sOrder, int tOrder, uint type);
		[DllImport("glu32")]
		public unsafe static extern void gluOrtho2D (double left, double right, double bottom, double top);
		[DllImport("glu32")]
		public unsafe static extern void gluPartialDisk (IntPtr quad, double inner, double outer, int slices, int loops, double start, double sweep);
		[DllImport("glu32")]
		public unsafe static extern void gluPerspective (double fovy, double aspect, double zNear, double zFar);
		[DllImport("glu32")]
		public unsafe static extern void gluPickMatrix (double x, double y, double delX, double delY, int *viewport);
		[DllImport("glu32")]
		public unsafe static extern int gluProject (double objX, double objY, double objZ, double *model, double *proj, int *view, double* winX, double* winY, double* winZ);
		[DllImport("glu32")]
		public unsafe static extern void gluPwlCurve (IntPtr nurb, int count, float* data, int stride, uint type);
//		[DllImport("glu32")]
//		public unsafe static extern void gluQuadricCallback (IntPtr quad, uint which, void (CALLBACK *CallBackFunc)());
		[DllImport("glu32")]
		public unsafe static extern void gluQuadricDrawStyle (IntPtr quad, uint draw);
		[DllImport("glu32")]
		public unsafe static extern void gluQuadricNormals (IntPtr quad, uint normal);
		[DllImport("glu32")]
		public unsafe static extern void gluQuadricOrientation (IntPtr quad, uint orientation);
		[DllImport("glu32")]
		public unsafe static extern void gluQuadricTexture (IntPtr quad, byte texture);
		[DllImport("glu32")]
		public unsafe static extern int gluScaleImage (uint format, int wIn, int hIn, uint typeIn, void *dataIn, int wOut, int hOut, uint typeOut, void* dataOut);
		[DllImport("glu32")]
		public unsafe static extern void gluSphere (IntPtr quad, double radius, int slices, int stacks);
		[DllImport("glu32")]
		public unsafe static extern void gluTessBeginContour (IntPtr tess);
		[DllImport("glu32")]
		public unsafe static extern void gluTessBeginPolygon (IntPtr tess, void* data);
//		[DllImport("glu32")]
//		public unsafe static extern void gluTessCallback (IntPtr tess, uint which, void (CALLBACK *CallBackFunc)());
		[DllImport("glu32")]
		public unsafe static extern void gluTessEndContour (IntPtr tess);
		[DllImport("glu32")]
		public unsafe static extern void gluTessEndPolygon (IntPtr tess);
		[DllImport("glu32")]
		public unsafe static extern void gluTessNormal (IntPtr tess, double valueX, double valueY, double valueZ);
		[DllImport("glu32")]
		public unsafe static extern void gluTessProperty (IntPtr tess, uint which, double data);
		[DllImport("glu32")]
		public unsafe static extern void gluTessVertex (IntPtr tess, double *location, void* data);
		[DllImport("glu32")]
		public unsafe static extern int gluUnProject (double winX, double winY, double winZ, double *model, double *proj, int *view, double* objX, double* objY, double* objZ);
		
		// hand added function
		[DllImport("glu32")]
		public static extern int gluBuild2DMipmaps(uint target, int component, int width, int height, uint format, uint type, IntPtr data);
	}
}
