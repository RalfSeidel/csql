/*
** @file 
** @brief Define and use Macro with one argument.
*/
#define MACRO(U) U

MACRO
//MACRO()
MACRO (X)
MACRO(X)
MACRO ( X )
MACRO /* comment1, (), # */ (X)
MACRO /* comment2, (), # */ ( X /*
							   
							   */ )
MACRO /* comment3, (), # */ ( /*
							   
							   */ X)
