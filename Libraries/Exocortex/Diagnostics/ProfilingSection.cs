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
using System.Text;
using System.IO;

namespace Exocortex.Diagnostics
{
	/// <summary>
	/// Summary description for TraceSection.
	/// </summary>
	public class ProfilingSection {

		//------------------------------------------------------------------------------

		static	protected ArrayList	s_sections = new ArrayList();

		static public string GetStatsString() {
			System.GC.Collect();

			StringWriter sw = new StringWriter();
			sw.WriteLine( "ProfilingSection Statistics" );
			string prefix = "\t";
			ArrayList newSections = new ArrayList();
			foreach( WeakReference wr in s_sections ) {
				if( wr.IsAlive ) {
					ProfilingSection ps = (ProfilingSection) wr.Target;
					sw.WriteLine( prefix + ps.ToString() );
					newSections.Add( wr );
				}
			}
			s_sections = newSections;
			return	sw.ToString();
		}

		static public void	ResetAll() {
			ArrayList newSections = new ArrayList();
			foreach( WeakReference wr in s_sections ) {
				if( wr.IsAlive ) {
					ProfilingSection ps = (ProfilingSection) wr.Target;
					ps.Reset();
					newSections.Add( wr );
				}
			}
			s_sections = newSections;
		}

		//------------------------------------------------------------------------------

		public ProfilingSection() {
			s_sections.Add( new WeakReference( this ) );
		}
		public ProfilingSection( string name ) {
			Debug.Assert( name != null );
			_name = name;
			s_sections.Add( new WeakReference( this ) );
		}

		//------------------------------------------------------------------------------

		public	void	Begin() {
			Debug.Assert( _inSection == false );
			_inSection = true;
			_beginTime = DateTime.Now;
		}

		public	void	End() {
			_timeSpan += ( DateTime.Now - _beginTime );
			Debug.Assert( _inSection == true );
			_inSection = false;
			_count ++;
		}

		//------------------------------------------------------------------------------
		
		public void		Reset() {
			Debug.Assert( _inSection == false );
			_count		= 0;
			_timeSpan	= TimeSpan.Zero;
		}

		//------------------------------------------------------------------------------

		protected string _name = "";
		public string	Name {
			get	{	return	_name;	}
			set {	_name = value;	}
		}

		protected int _count = 0;
		public int	Count {
			get	{	return	_count;	}
		}

		protected bool _inSection = false;
		public bool	InSection {
			get	{	return	_inSection;	}
		}

		protected DateTime	_beginTime = DateTime.Now;

		protected TimeSpan	_timeSpan = TimeSpan.Zero;
		public TimeSpan	TimeSpan {
			get	{	return	_timeSpan;	}
		}

		public TimeSpan	Elapsed {
			get {
				Debug.Assert( _inSection == true );
				return	DateTime.Now - _beginTime;
			}
		}

		public double	AverageMilliseconds {
			get	{
				if( _count == 0 ) {
					return	0;
				}
				return	_timeSpan.TotalMilliseconds / _count;
			}
		}
		public double	AverageSeconds {
			get	{
				if( _count == 0 ) {
					return	0;
				}
				return	_timeSpan.TotalSeconds / _count;
			}
		}

		public double	CountPerSecond {
			get	{
				if( _timeSpan.TotalSeconds == 0 ) {
					return	0;
				}
				return	_count / _timeSpan.TotalSeconds;
			}
		}

		//------------------------------------------------------------------------------

		public override string	ToString() {
			return	"[" + this.Name + "(" + this.Count + ") avg = " + this.AverageMilliseconds + " msec / " + this.CountPerSecond + " cps ]";
		}
		
		//------------------------------------------------------------------------------

	}
}
