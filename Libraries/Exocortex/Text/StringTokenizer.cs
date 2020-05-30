// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.


using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Exocortex;

namespace Exocortex.Text {

	public class StringTokenizer {

		static public string[]	ToTokens( string strBuffer ) {
			StringTokenizer st = new StringTokenizer();
			st.SetBuffer( strBuffer );

			ArrayList v = new ArrayList();
			while( st.LookAhead != null ) {
				v.Add( st.GetToken() );
			}

			return (string[]) v.ToArray( typeof( string ) );
		}

		protected string _buffer = null;
		protected string _token  = null;
		protected char   _delimiter = ' ';

		public StringTokenizer() {
		}

		public void SetBuffer( string buffer ) {
			_buffer = buffer;
			_token = ExtractNextToken();
		}

		public string	LookAhead {
			get {	return	_token;	}
		}

		public string	GetToken() {
			string str = _token;
			_token = ExtractNextToken();
			return	str;
		}

		protected string	ExtractNextToken() {
			if( _buffer == null ) {
				return null;
			}

			int bufferLength = _buffer.Length;

			// get start position
			int tokenStart = 0;
			while( tokenStart < bufferLength && _buffer[tokenStart] == _delimiter ) {
				tokenStart++;
			}

			if( tokenStart >= bufferLength ) {
				_buffer = null;
				return null;
			}

			// get end position
			int tokenEnd = tokenStart + 1;
			if( _buffer[tokenStart] == '"' ) {
				tokenStart++;
				while( tokenEnd < bufferLength && _buffer[tokenEnd] != '"' ) {
					tokenEnd++;
				}
			}
			else {
				while( tokenEnd < bufferLength && _buffer[tokenEnd] != _delimiter ) {
					tokenEnd++;
				}
			}

			// remove first token from buffer
			string token = _buffer.Substring( tokenStart, tokenEnd - tokenStart );
			if( ( tokenEnd + 1 ) < bufferLength ) {
				_buffer = _buffer.Substring( tokenEnd + 1, bufferLength - ( tokenEnd + 1 ) );
			}
			else {
				_buffer = null;
			}

			return token;
		}
	}


}
