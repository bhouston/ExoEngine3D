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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Data;
using System.Threading;

namespace Exocortex.Diagnostics
{
	/// <summary>
	/// Summary description for ErrorLog.
	/// </summary>
	public class BugTracking {
		
		//--------------------------------------------------------------------------------------------

		static public void OnThreadException( object o, ThreadExceptionEventArgs e ) {
			BugTracking.LogException( e.Exception );
		}

		static public void LogException( Exception e ) {
			string fileName = BugTracking.WriteExceptionRecord( e );
			System.Windows.Forms.MessageBox.Show( null,
				"A fatal error occured.  Please send the following bug report to bugs@exocortex.org:\n" + 
				fileName,
				Application.ProductName,
				MessageBoxButtons.OK,
				MessageBoxIcon.Error );
		}
		//--------------------------------------------------------------------------------------------

		protected class LogEntry {
			public string		Text;
			public DateTime	DateTime;
			public LogEntry( string text ) {
				this.Text		= text;
				this.DateTime	= DateTime.Now;
			}
			public override string	ToString() {
				return	"[" + this.DateTime.ToLocalTime() + "] " + this.Text;
			}
		}

		static protected ArrayList	s_logEntries	= new ArrayList();

		static public void Log( string text ) {
			s_logEntries.Add( new LogEntry( text ) );
		}

		//--------------------------------------------------------------------------------------------

		static protected Hashtable	s_htWatches = new Hashtable();

		static public void	SetWatch( string name, object value ) {
			Debug.Assert( name != null );
			if( s_htWatches.Contains( name ) ) {
				s_htWatches.Remove( name );
			}

			s_htWatches.Add( name, ( value != null ) ? new WeakReference( value ) : null );
			Debug.Assert( s_htWatches.Contains( name ) == true );
		}

		static public void	RemoveWatch( string name ) {
			Debug.Assert( name != null );
			Debug.Assert( s_htWatches.Contains( name ) == true );
			
			s_htWatches.Remove( name );
		}

		static public string	GetWatchString( string name ) {
			Debug.Assert( name != null );
			Debug.Assert( s_htWatches.Contains( name ) == true );
			
			WeakReference wr = (WeakReference) s_htWatches[ name ];
			if( wr != null ) {
				if( wr.IsAlive == true ) {
					object value = wr.Target;
					return	value.ToString() + " [" + value.GetType().Name + "]";
				}
				else {
					return	"(garbageCollected)";
				}
			}
			else {
				return	"(null)";
			}
		}

		//--------------------------------------------------------------------------------------------

		static public string WriteExceptionRecord( Exception e ) {
			try {
				string	fileName = Path.Combine( Application.StartupPath, "error.log" );
				StreamWriter sw = new StreamWriter( fileName, true );
			
				sw.WriteLine( "ErrorLog.WriteExceptionLog() >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" );

				try {
					DateTime now = DateTime.Now;
					sw.WriteLine( "CurrentDate: " + now.ToLongDateString() );
					sw.WriteLine( "CurrentTime: " + now.ToLongTimeString() );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-CurrentDateTime>" + ex.ToString() );
				}
				
				try {
					sw.WriteLine( "Exception.Type: " + e.GetType().Name );
					sw.WriteLine( "Exception.Message: " + e.Message );
					sw.WriteLine( "Exception.Source: " + e.Source );
					sw.WriteLine( "Exception.HelpLink: " + e.HelpLink );
					sw.WriteLine( "Exception.TargetSite: " + e.TargetSite );
					sw.WriteLine( "Exception.StackTrace: " );
					sw.WriteLine( e.StackTrace );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-Exception>" + ex.ToString() );
				}
		
				try {
					sw.WriteLine( "GC.TotalMemory: " + GC.GetTotalMemory( false ) );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-GC>" + ex.ToString() );
				}
		
				try {
					sw.WriteLine( "Application.ProductName: " + Application.ProductName );
					sw.WriteLine( "Application.ProductVersion: " + Application.ProductVersion );
					sw.WriteLine( "Application.StartupPath: " + Application.StartupPath );
					sw.WriteLine( "Application.ExecutablePath: " + Application.ExecutablePath );
					sw.WriteLine( "Application.CurrentDirectory: " + Directory.GetCurrentDirectory() );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-Application>" + ex.ToString() );
				}
		
				try {
					Assembly execAssembly = Assembly.GetExecutingAssembly();
					sw.WriteLine( "ExecutingAssembly.CodeBase: " + execAssembly.CodeBase );
					sw.WriteLine( "ExecutingAssembly.Location: " + execAssembly.Location );
					sw.WriteLine( "ExecutingAssembly.GlobalAssemblyCache: " + execAssembly.GlobalAssemblyCache );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-ExecutingAssembly>" + ex.ToString() );
				}

				try {
					foreach( string watchName in s_htWatches.Keys ) {
						sw.WriteLine( "Watch[" + watchName + "]: " + GetWatchString( watchName ) );
					}
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-Watches>" + ex.ToString() );
				}

				try {
					foreach( LogEntry logEntry in s_logEntries ) {
						sw.WriteLine( "LogEntry" + logEntry.ToString() );
					}
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-LogEntry>" + ex.ToString() );
				}
				
				try {
					Assembly selfAssembly = Assembly.GetAssembly( typeof( BugTracking ) );
					sw.WriteLine( "CurrentAssembly.CodeBase: " + selfAssembly.CodeBase );
					sw.WriteLine( "CurrentAssembly.Location: " + selfAssembly.Location );
					sw.WriteLine( "CurrentAssembly.GlobalAssemblyCache: " + selfAssembly.GlobalAssemblyCache );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-CurrentAssembly>" + ex.ToString() );
				}
				
				try {
					Thread thread = Thread.CurrentThread;
					sw.WriteLine( "CurrentThread.Name: " + thread.Name );
					sw.WriteLine( "CurrentThread.Priority: " + thread.Priority );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-CurrentThread>" + ex.ToString() );
				}

				try {
					Process process = Process.GetCurrentProcess();
					sw.WriteLine( "CurrentProcess.Name: " + process.ProcessName );
					sw.WriteLine( "CurrentProcess.MachineName: " + process.MachineName );
					sw.WriteLine( "CurrentProcess.MainModule: " + process.MainModule );
					sw.WriteLine( "CurrentProcess.StartDate: " + process.StartTime.ToLongDateString() );
					sw.WriteLine( "CurrentProcess.StartTime: " + process.StartTime.ToLongTimeString() );
					sw.WriteLine( "CurrentProcess.UserProcessorTime: " + process.UserProcessorTime );
					sw.WriteLine( "CurrentProcess.TotalProcessorTime: " + process.TotalProcessorTime );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-CurrentProcess>" + ex.ToString() );
				}

				try {
					OperatingSystem os = Environment.OSVersion;
					sw.WriteLine( "Environment.OSVersion.Platform: " + os.Platform );
					sw.WriteLine( "Environment.OSVersion.Version: " + os.Version );
	
					Version ver = Environment.Version;
					sw.WriteLine( "Environment.Version.Major: " + ver.Major );
					sw.WriteLine( "Environment.Version.Minor: " + ver.Minor );
					sw.WriteLine( "Environment.Version.Revision: " + ver.Revision );
					sw.WriteLine( "Environment.Version.Build: " + ver.Build );

					sw.WriteLine( "Environment.UserName: " + Environment.UserName );
					sw.WriteLine( "Environment.SystemDirectory: " + Environment.SystemDirectory );
					sw.WriteLine( "Environment.TickCount: " + Environment.TickCount );
					sw.WriteLine( "Environment.CommandLine: " + Environment.CommandLine );
					string[] args = Environment.GetCommandLineArgs();
					if( args != null ) {
						for( int i = 0; i < args.Length; i ++ ) {
							sw.WriteLine( "Environment.CommandLineArgs[" + i + "]: " + args[i] );
						}
					}
					sw.WriteLine( "Environment.StackTrace: " );
					sw.WriteLine( Environment.StackTrace );
					sw.Flush();
				}
				catch( Exception ex ) {
					sw.WriteLine( "<error-Environment>" + ex.ToString() );
				}

				sw.WriteLine( "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<" );
				sw.Close();
				
				return	fileName;
			}
			catch( Exception ) {
				return	"-- error writing log file --";
			}
		}
	}
}
