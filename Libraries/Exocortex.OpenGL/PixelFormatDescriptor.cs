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
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
//using System.Windows.Forms;

namespace Exocortex.OpenGL
{
	public enum PixelFlag : uint {
		PFD_DRAW_TO_WINDOW = 4,
		PFD_DRAW_TO_BITMAP = 8,
		PFD_SUPPORT_GDI = 16,
		PFD_SUPPORT_OPENGL = 32,
		PFD_DOUBLEBUFFER = 1
	};
	public enum PixelType : byte {
		PFD_TYPE_RGBA = 0,
		PFD_TYPE_COLORINDEX = 1
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct PIXELFORMATDESCRIPTOR
	{
		public ushort nSize;
		public ushort nVersion;
		public PixelFlag dwFlags;
		public PixelType iPixelType;
		public byte cColorBits;
		public byte cRedBits;
		public byte cRedShift;
		public byte cGreenBits;
		public byte cGreenShift;
		public byte cBlueBits;
		public byte cBlueShift;
		public byte cAlphaBits;
		public byte cAlphaShift;
		public byte cAccumBits;
		public byte cAccumRedBits;
		public byte cAccumGreenBits;
		public byte cAccumBlueBits;
		public byte cAccumAlphaBits;
		public byte cDepthBits;
		public byte cStencilBits;
		public byte cAuxBuffers;
		public byte iLayerType;
		public byte bReserved;
		public uint dwLayerMask;
		public uint dwVisibleMask;
		public uint dwDamageMask;
		public override string ToString() 
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("PIXELFORMATDESCRIPTOR {");
			sb.AppendFormat("\n\t nSize = {0}", nSize); 
			sb.AppendFormat("\n\t nVersion = {0}", nVersion); 
			sb.AppendFormat("\n\t dwFlags = {0}", dwFlags); 
			sb.AppendFormat("\n\t iPixelType = {0}", iPixelType); 
			sb.AppendFormat("\n\t cColorBits = {0}", cColorBits); 
			sb.AppendFormat("\n\t cRedBits = {0}", cRedBits); 
			sb.AppendFormat("\n\t cRedShift = {0}", cRedShift); 
			sb.AppendFormat("\n\t cGreenBits = {0}", cGreenBits); 
			sb.AppendFormat("\n\t cGreenShift = {0}", cGreenShift); 
			sb.AppendFormat("\n\t cBlueBits = {0}", cBlueBits); 
			sb.AppendFormat("\n\t cBlueShift = {0}", cBlueShift); 
			sb.AppendFormat("\n\t cAlphaBits = {0}", cAlphaBits); 
			sb.AppendFormat("\n\t cAlphaShift = {0}", cAlphaShift); 
			sb.AppendFormat("\n\t cAccumBits = {0}", cAccumBits); 
			sb.AppendFormat("\n\t cAccumRedBits = {0}", cAccumRedBits); 
			sb.AppendFormat("\n\t cAccumGreenBits = {0}", cAccumGreenBits); 
			sb.AppendFormat("\n\t cAccumBlueBits = {0}", cAccumBlueBits); 
			sb.AppendFormat("\n\t cAccumAlphaBits = {0}", cAccumAlphaBits); 
			sb.AppendFormat("\n\t cDepthBits = {0}", cDepthBits); 
			sb.AppendFormat("\n\t cStencilBits = {0}", cStencilBits); 
			sb.AppendFormat("\n\t cAuxBuffers = {0}", cAuxBuffers); 
			sb.AppendFormat("\n\t iLayerType = {0}", iLayerType); 
			sb.AppendFormat("\n\t bReserved = {0}", bReserved); 
			sb.AppendFormat("\n\t dwLayerMask = {0}", dwLayerMask); 
			sb.AppendFormat("\n\t dwVisibleMask = {0}", dwVisibleMask); 
			sb.AppendFormat("\n\t dwDamageMask = {0}", dwDamageMask); 
			sb.Append("\n}");
			return sb.ToString();
		}
		public unsafe PIXELFORMATDESCRIPTOR(PixelFlag flags, byte cDepth, byte zDepth)
		{
			nSize           = (ushort) sizeof(PIXELFORMATDESCRIPTOR);
			nVersion        = 1;
			dwFlags         = flags;
			iPixelType      = PixelType.PFD_TYPE_RGBA; // RGBA type
			cColorBits      = cDepth; // 24-bit color depth
			cRedBits        = 0;
			cRedShift       = 0;
			cGreenBits      = 0;
			cGreenShift     = 0;
			cBlueBits       = 0;
			cBlueShift      = 0;
			cAlphaBits      = 0;
			cAlphaShift     = 0;
			cAccumBits      = 0;
			cAccumRedBits   = 0;
			cAccumGreenBits = 0;
			cAccumBlueBits  = 0;
			cAccumAlphaBits = 0;
			cDepthBits      = zDepth;
			cStencilBits    = 0;
			cAuxBuffers     = 0;
			iLayerType      = 0;
			bReserved       = 0;
			dwLayerMask     = 0;
			dwVisibleMask   = 0;
			dwDamageMask    = 0;
		}
	}
}
