/*
** @file 
** @brief Define and use Macro without arguments.
*/
#define MACRO X

// Expecting X
MACRO
// Expecting X()
MACRO()
// Expecting X  ()
MACRO /* comment1, (), # */ ()
// Expecting X  ( NEW_LINE NEW_LINE )
MACRO /* comment2, (), # */ ( /*

                         */ )
