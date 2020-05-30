

#include "stdafx.h"



//=================================================================================
//=================================================================================
#pragma comment (lib, "Shlwapi.lib")
#include "Shlwapi.h"


static char glpLogFile[256];

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH: {
		//::CreateNamedPipe( "
		GetModuleFileName( (HMODULE) hModule, glpLogFile, 255 );
		PathRemoveFileSpec( glpLogFile );
		PathAppend( glpLogFile, "ExocortexNative.log" );

		log( "START---------------------------------------------\n" );
		log( "Build Info: %s %s\n", __DATE__, __TIME__ );
		}
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		log( "END-----------------------------------------------\n" );
		break;
	}
    return TRUE;
}

//=================================================================================
//=================================================================================

/*HANDLE hPipe	= NULL;

void	setupDebugPipe() {
   LPTSTR	lpszPipename = "\\\\.\\pipe\\ExocortexNative.Log"; 

   hPipe = CreateNamedPipe( 
          lpszPipename,             // pipe name 
          PIPE_ACCESS_DUPLEX,       // read/write access 
          PIPE_TYPE_MESSAGE |       // message type pipe 
          PIPE_READMODE_MESSAGE |   // message-read mode 
          PIPE_WAIT,                // blocking mode 
          PIPE_UNLIMITED_INSTANCES, // max. instances  
          BUFSIZE,                  // output buffer size 
          BUFSIZE,                  // input buffer size 
          PIPE_TIMEOUT,             // client time-out 
          NULL);                    // no security attribute 

   if ( hPipe == INVALID_HANDLE_VALUE ) {
	   log( "CreatePipe failed: (hPipe == INVALID_HANDLE_VALUE )" );
	   hPipe = NULL;
   }
}	*/

void	log( char *szFormatString, ... ) {
	//static char[1024] szBuffer;

	/*va_list params;
	va_start( params, szFormatString );
	vsprintf( szBuffer, szFormatString, params );*/

	FILE *file = fopen( glpLogFile, "a" );
	if( file != NULL ) {
		va_list params;
		va_start( params, szFormatString );
		vfprintf( file, szFormatString, params );
		fflush( file );
		fclose( file );
	}

/*	if( hPipe != NULL ) {
		int cbWritten;
		WriteFile( 
         hPipe,        // handle to pipe 
         szBuffer,      // buffer to write from 
         strlen( szBuffer ) + 1, // number of bytes to write 
         &cbWritten,   // number of bytes written 
         NULL );        // not overlapped I/O 
	}  */
}

void	log_assert( bool bExpression, char *szFile, int iLine ) {
	if( bExpression == false ) {
		log( "_ASSERT failed: %s, %i\n", szFile, iLine );

		LPVOID lpMsgBuf;
		FormatMessage( 
				FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
				NULL,
				GetLastError(),
				MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
				(LPTSTR) &lpMsgBuf,
				0,
				NULL 
			);
		if( lpMsgBuf != NULL ) {
			log( "Extended Info: %s\n", lpMsgBuf );
			LocalFree( lpMsgBuf );
		}
	}
}

void	log_error( char *szFile, int iLine ) {
	log( "_ERROR: %s, %i\n", szFile, iLine );

	LPVOID lpMsgBuf;
	FormatMessage( 
			FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,
			GetLastError(),
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
			(LPTSTR) &lpMsgBuf,
			0,
			NULL 
		);
	if( lpMsgBuf != NULL ) {
		log( "Extended Info: %s\n", lpMsgBuf );
		LocalFree( lpMsgBuf );
	}
}

//=================================================================================
//=================================================================================
