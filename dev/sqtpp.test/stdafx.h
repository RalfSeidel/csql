#pragma once
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
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
#include <malloc.h>
#include <stdlib.h>
#include <wchar.h>

#if defined( _MANAGED )
#include <vcclr.h>
#using <mscorlib.dll>
#endif

using namespace std;

typedef std::vector<std::wstring> StringArray;
typedef std::set<std::wstring>    StringSet;
typedef std::map<std::wstring, std::wstring> StringDictionary;


