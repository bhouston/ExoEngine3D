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
using System.Threading;
using System.Reflection;
using System.Text;

namespace Exocortex {

	public class Debug2 {

		//==================================================================

		// Prevent from begin created
		private Debug2() {
		}

		//==================================================================

		static private bool	_showTime = true;
		static public bool ShowTime {
			get	{	return	_showTime;	}
			set	{	_showTime = value;	}
		}
		
		static private bool	_showMemory = true;
		static public bool ShowMemory {
			get	{	return	_showMemory;	}
			set	{	_showMemory = value;	}
		}

		//==================================================================

		[Conditional("DEBUG")]
		static public void TraceFunction() {
			StackTrace st = new StackTrace( 1, true );
			StackFrame frame = st.GetFrame( 0 );
			MethodBase method = frame.GetMethod();
			// C:\Development\ExoEngine\Geometry\SkyBox.cs(36):
			Debug.WriteLine( "[function trace] " + method.DeclaringType + "." + method.Name );
			Debug.WriteLine( frame.GetFileName() + "(" + frame.GetFileLineNumber() + ")" );
		}

		[Conditional("DEBUG")]
		static public void TraceFunction( string id ) {
			StackTrace st = new StackTrace( 1, true );
			StackFrame frame = st.GetFrame( 0 );
			MethodBase method = frame.GetMethod();
			// C:\Development\ExoEngine\Geometry\SkyBox.cs(36):
			Debug.WriteLine( "[function trace:" + id + "] " + method.DeclaringType + "." + method.Name );
			Debug.WriteLine( frame.GetFileName() + "(" + frame.GetFileLineNumber() + ")" );
		}

		[Conditional("DEBUG")]
		static public void TraceFunction( int id ) {
			StackTrace st = new StackTrace( 1, true );
			StackFrame frame = st.GetFrame( 0 );
			MethodBase method = frame.GetMethod();
			// C:\Development\ExoEngine\Geometry\SkyBox.cs(36):
			Debug.WriteLine( "[function trace:" + id + "] " + method.DeclaringType + "." + method.Name );
			Debug.WriteLine( frame.GetFileName() + "(" + frame.GetFileLineNumber() + ")" );
		}

		[Conditional("DEBUG")]
		static public void TraceThread() {
			Thread currentThread = Thread.CurrentThread;
			if( currentThread.Name == null || currentThread.Name.Length == 0 ) {
				currentThread.Name = new Random().Next( 1000 ).ToString();
			}
			Debug.WriteLine( "[thread trace] CurrentThread.Name = '" + currentThread.Name + "'" );
			//Debug.WriteLine( frame.GetFileName() + "(" + frame.GetFileLineNumber() + ")" );
		}

		[Conditional("DEBUG")]
		static public void TraceStack() {
			Debug.WriteLine( "[stack trace]" );
			StackTrace st = new StackTrace( 1, true );
			for( int i = 0; i < st.FrameCount; i ++ ) {
				StackFrame frame = st.GetFrame( i );
				MethodBase method = frame.GetMethod();
				Debug.WriteLine( "   " + method.DeclaringType + "." + method.Name + "   " + frame.GetFileName() + " line " + frame.GetFileLineNumber() );
			}
		}

		[Conditional("DEBUG")]
		static public void TraceStack( int maximumDepth ) {
			Debug.WriteLine( "[stack trace (maximumDepth=" + maximumDepth + ")]" );
			StackTrace st = new StackTrace( 1, true );
			for( int i = 0; i < Math.Min( st.FrameCount, maximumDepth ); i ++ ) {
				StackFrame frame = st.GetFrame( i );
				MethodBase method = frame.GetMethod();
				Debug.WriteLine( "   " + method.DeclaringType + "." + method.Name + "   " + frame.GetFileName() + " line " + frame.GetFileLineNumber() );
			}
		}

		[Conditional("DEBUG")]
		static public void	WriteVar( string variableName, object variable ) {
			StringBuilder stringBuilder = new StringBuilder( 50 );
			
			stringBuilder.Append( variableName );
			stringBuilder.Append( '[' );
			if( variable != null ) {
				stringBuilder.Append( variable.GetType().Name );
			}
			else {
				stringBuilder.Append( "(unknown)" );
			}
			stringBuilder.Append( "] = " );
			if( variable != null ) {
				stringBuilder.Append( variable.ToString() );
			}
			else {
				stringBuilder.Append( "(null)" );
			}

			Debug.WriteLine( stringBuilder.ToString() );
		}

		[Conditional("DEBUG")]
		static public void	WriteArray( string arrayName, Array array ) {
			StringBuilder stringBuilder = new StringBuilder( 100 );
			stringBuilder.Append( arrayName );
			stringBuilder.Append( '[' );
			if( array != null ) {
				stringBuilder.Append( array.GetType().GetElementType().Name );
			}
			else {
				stringBuilder.Append( "(unknown)" );
			}
			stringBuilder.Append( "] = " );
			if( array != null ) {
				stringBuilder.Append( "( " );
				for( int i = 0; i < array.Length; i ++ ) {
					stringBuilder.Append( array.GetValue( i ).ToString() );
					stringBuilder.Append( ' ' );
				}
				stringBuilder.Append( ')' );				
			}
			else {
				stringBuilder.Append( "(null)" );
			}
		
			Debug.WriteLine( stringBuilder.ToString() );
		}

		[Conditional("DEBUG")]
		static public void Collect() {
			Debug2.Push();
			System.GC.Collect();
			Debug2.Pop();
		}
			
		[Conditional("DEBUG")]
		static public void WriteSeparator() {
			Debug.WriteLine( "-------------------------------------------------------------------------------" );
		}
		[Conditional("DEBUG")]
		static public void WriteLine() {
			Debug.WriteLine( "" );
		}
		[Conditional("DEBUG")]
		static public void WriteLine( string s ) {
			Debug.WriteLine( s );
		}
		[Conditional("DEBUG")]
		static public void WriteLines( string[] ss ) {
			foreach( string s in ss ) {
				Debug.WriteLine( s );
			}
		}
		[Conditional("DEBUG")]
		static public void Write( string s ) {
			Debug.Write( s );
		}
		[Conditional("DEBUG")]
		static public void Writes( string[] ss ) {
			foreach( string s in ss ) {
				Debug.Write( s );
			}
		}

		//==================================================================

		protected class DebugFrame {
			protected DateTime	_datetimeBegin;
			protected String	_name;
			protected long		_memoryBegin;

			public DebugFrame( string name ) {
				_name = name;
				_memoryBegin = System.GC.GetTotalMemory( false );
				_datetimeBegin = System.DateTime.Now;
			}
			public float	GetElapsedMillisecs() {
				TimeSpan timespan = System.DateTime.Now - _datetimeBegin;
				return	(float) timespan.TotalMilliseconds;
			}
			public long		GetMemoryDelta() {
				long memoryDelta = System.GC.GetTotalMemory( false ) - _memoryBegin;
				return	memoryDelta;
			}
		}

		static protected Stack	_stackDebugFrames = new Stack();

		//==================================================================

		[Conditional("DEBUG")]
		static public void Push() {
			StackTrace st = new StackTrace( 1, true );
			StackFrame frame = st.GetFrame( 0 );
			MethodBase method = frame.GetMethod();

			string strFrameName = method.DeclaringType + "." + method.Name;
			Debug2.Push( strFrameName );
		}

		[Conditional("DEBUG")]
		static public void Push( String strName ) {
			lock( _stackDebugFrames ) {
				Debug.WriteLine( strName + " {" );
				Debug.Indent();
				//Debug.Flush();
				DebugFrame frame = new DebugFrame( strName );
				_stackDebugFrames.Push( frame );
			}
		}

		[Conditional("DEBUG")]
		static public void Pop() {
			lock( _stackDebugFrames ) {
				Debug.Assert( _stackDebugFrames.Count > 0 );
				DebugFrame frame = (DebugFrame) _stackDebugFrames.Pop();

				float fMilliseconds = 0;
				if( _showTime ) {
					fMilliseconds = frame.GetElapsedMillisecs();
				}
				long memoryDelta = 0;
				if( _showMemory ) {
					memoryDelta = frame.GetMemoryDelta();
				}
				
				Debug.Unindent();

				StringBuilder stringBuilder = new StringBuilder( 50 );
				stringBuilder.Append( '}' );

				if( _showTime || _showMemory ) {
					stringBuilder.Append( "  [" );
				}

				if( _showTime ) {
					stringBuilder.Append( " " );
					if( fMilliseconds < 1000 ) {
						stringBuilder.Append( fMilliseconds );
						stringBuilder.Append( " ms" );
					}
					else if( fMilliseconds < 60000 ) {
						stringBuilder.Append( fMilliseconds/1000 );
						stringBuilder.Append( " secs" );
					}
					else {
						stringBuilder.Append( fMilliseconds/60000 );
						stringBuilder.Append( " mins" );
					}
					stringBuilder.Append( " " );
				}

				if( _showMemory ) {
					stringBuilder.Append( " " );
					if( Math.Abs( memoryDelta ) < 1000 ) {
						stringBuilder.Append( memoryDelta );
						stringBuilder.Append( " Bytes" );
					}
					else if( Math.Abs( memoryDelta ) < 1000000 ) {
						stringBuilder.Append( ((int)memoryDelta/10)/100f );
						stringBuilder.Append( " KB" );
					}
					else {
						stringBuilder.Append( ((int)memoryDelta/10000)/100f );
						stringBuilder.Append( " MB" );
					}
					stringBuilder.Append( " " );
				}

				if( _showTime || _showMemory ) {
					stringBuilder.Append( "]" );
				}

				Debug.WriteLine( stringBuilder.ToString() );
				//Debug.Flush();
			}
		}

		//==================================================================

	}
}
