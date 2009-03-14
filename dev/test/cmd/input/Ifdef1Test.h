/*
** @file 
** @brief Check \#ifdef MACRO.
*/
#ifdef MACRO
#error MACRO not defined.
#else 
OK
#endif
#define MACRO
#ifdef MACRO
OK
#else 
#error MACRO is defined.
#endif
