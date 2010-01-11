/*!
** \file DllInit.cpp
** \brief Implementation der Klasse DllInit.
**
** Design and Implementation 2003 by Heinrich & Seidel Wuppertal
**
** Projekt:  GRP Automat\n
** Erstellt: Dienstag, 11. November 2003 17:23:46\n
** Autor:    RS
*/
#include "stdafx.h"
#include "DllInit.h"
#using <mscorlib.dll>
using namespace System;
using namespace System::Reflection;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;


bool CrtDllInit::Initialize()
{
	BOOL bResult = true;

//	AfxSetAllocStop( 121 );

	pin_ptr<LONG> pInitCount = &(m_nInitCount);
	if ( ::InterlockedIncrement( pInitCount ) == 1 ) {
		CrtDllInit^  pDummy    = gcnew CrtDllInit();
		Type^        pThisType = pDummy->GetType();
		Assembly^    pAssembly = Assembly::GetAssembly( pThisType );
		//std::wstring sPath( pAssembly->Location );

		//m_hModule      = ::LoadLibrary( sPath );

		//::MessageBox( NULL, "DLL Init", "DLL Init", MB_OK );

#		ifdef _DEBUG
		//::AfxEnableMemoryTracking( false );
#		endif
		try {
			//bResult = _CRT_INIT( m_hApplication, DLL_PROCESS_ATTACH, NULL ) != FALSE;
			//__crt_dll_initialize();
			if ( bResult ) {
//				bResult = AfxWinInit( m_hModule, NULL, NULL, 0 ) != FALSE;
			}
		} catch( System::Exception^ e ) {
			if ( e != nullptr ) {
				Console::WriteLine(e->Message);
			}
			bResult = false;
		}	
#		ifdef _DEBUG
//		::AfxEnableMemoryTracking( true );
#		endif

		if ( !bResult ) {
			//_CRT_INIT( m_hApplication, DLL_PROCESS_DETACH, NULL );
			//__crt_dll_terminate();
		}

	}

	return bResult != FALSE;
}

bool CrtDllInit::Cleanup()
{
	BOOL bResult = true;

	pin_ptr<LONG> pInitCount = &(m_nInitCount);
	if ( ::InterlockedDecrement( pInitCount ) == 0 ) {
		//::MessageBox( NULL, "DLL Terminate", "DLL Terminate", MB_OK );
		try {
			if ( m_hModule != NULL ) {
				::FreeLibrary( m_hModule );
				m_hModule = NULL;
			}
			// __crt_dll_initialize will crash if __crt_dll_terminate
			// has ever been called.
			//bResult = __crt_dll_terminate();
		}
		catch( System::Exception^ e ) {
			if ( e != nullptr ) {
				Console::WriteLine(e->Message);
			}
			bResult = false;
		}
	}
	return bResult != FALSE;
}
