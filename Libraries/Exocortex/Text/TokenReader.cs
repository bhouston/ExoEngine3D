// Exocortex Technologies
// http://www.exocortex.org
// Copyright (c) 2001, 2002 Ben Houston (ben@exocortex.org).  All Rights Reserved.


using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Exocortex.Text {

	public class TokenReader {

		protected StreamReader _sr;
		protected StringTokenizer _tokenizer;

		protected String	_token;
		protected String	_line;
		
		public TokenReader( string strFileName, char cDelimiter ) {
			_sr			= new StreamReader( strFileName );
			_tokenizer	= new StringTokenizer();
			_line		= _sr.ReadLine();
			_tokenizer.SetBuffer( _line );
			_token		= null;
			GetToken();
		}

		public string	LookAhead {
			get	{	return	_token;	}
		}

		public string GetToken() {

			string temp = _token;
			_token		= _tokenizer.GetToken();

			while( _token == null && _line != null ) {
				_line = _sr.ReadLine();
				_tokenizer.SetBuffer( _line );
				_token = _tokenizer.GetToken();
			}

			return	temp;
		}

		public void Close() {
			_sr.Close();
		}

	}

}
