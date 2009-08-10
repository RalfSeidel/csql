/**
** @file
** @author Ralf Seidel
** @brief Std includes  / First include in every file for pre compiled header optimisation.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_STDAFX_H
#define SQTPP_STDAFX_H
#if _MSC_VER > 10
#pragma once
#endif

#include <string>
#include <iostream>
#include <sstream>
#include <fstream>
#include <vector>
#include <list>
#include <set>
#include <map>
#include <stack>
#include <stdexcept>
#include <locale>
#include <cassert>
#include <malloc.h>
#include <stdlib.h>
#include <wchar.h>
#if defined( _MANAGED )
#include <vcclr.h>
#using <mscorlib.dll>
#endif

typedef std::vector<std::wstring> StringArray;
typedef std::set<std::wstring>    StringSet;
typedef std::map<std::wstring, std::wstring> StringDictionary;

// This function or variable may be unsafe - don't bother
#pragma warning( disable: 4996 )

using namespace std;

#ifdef _DEBUG
//#define new DEBUG_NEW
#endif



#endif // SQTPP_STDAFX_H



