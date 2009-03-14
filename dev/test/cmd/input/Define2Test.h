/*
** @file 
** @brief Define and use Macro with empty argument list.
*/
#define MACRO() X

// Expecting  MACRO
MACRO
// Expecting X
MACRO()
// Expecting X
MACRO ()
// Expecting X
MACRO(
)
// Expecting X
MACRO (
)
// Expecting X
MACRO /* comment1, (), # */ ()
// Expecting X
MACRO /* comment2, (), # */ ( /*

                         */ )
