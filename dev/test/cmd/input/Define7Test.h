/*
** @file 
** @brief Define and use Macros
*/
#define A 1 + 2
// The brace does not follow immediatly after the macro name
// --> No parameters
#define B (A) << 1
#define C(U) U
#define D (

// Expected: 1 + 2
A
// Expected: (1 + 2) << 1
B
// X
C(X)
// /
D
