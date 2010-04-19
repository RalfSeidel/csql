/*
** @file 
** @brief Check \#ifndef MACRO.
*/
#ifndef MACRO
OK
#else 
#error MACRO not defined.
#endif
#define MACRO
#ifndef MACRO
#error MACRO is defined.
#else 
OK
#endif
