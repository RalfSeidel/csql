/*
** @file 
** @brief Test processing macro definitions (macro with several arguments).
*/
#define MACRO(A,B,C) C B A
MACRO
MACRO(1,2,3)
MACRO ( 1 /*comment*/, 2, 3 )
MACRO /*comment*/ ( 1, 2, 3 )
