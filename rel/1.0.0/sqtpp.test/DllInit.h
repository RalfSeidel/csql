/*!
** \file DllInit.h
** \brief Enthält die Deklaration der Klasse DllInit.
**  
** Design and Implementation 2003 by Heinrich & Seidel Wuppertal
**
** Projekt:  SqtLib\n
** Erstellt: Dienstag, 11. November 2003 17:23:06\n
** Autor:    RS
*/
#ifndef CRTDLLINIT_SQTPP_TEST_H
#define CRTDLLINIT_SQTPP_TEST_H
#pragma once

/*
** \brief Initialisierungsklasse.
**
** Diese Klasse ist erforderlich um die C/C++ Runtime Bibliotheken zu initialisieren.
*/ 
public ref class CrtDllInit sealed 
{
private:
	static LONG    m_nInitCount   = 0;
	static HMODULE m_hModule      = NULL;
public:
	[Microsoft::VisualStudio::TestTools::UnitTesting::AssemblyInitialize]
	static bool Initialize();
	[Microsoft::VisualStudio::TestTools::UnitTesting::AssemblyCleanup]
	static bool Cleanup();
};



#endif  // ifndef CRTDLLINIT_SQTPP_TEST_H
