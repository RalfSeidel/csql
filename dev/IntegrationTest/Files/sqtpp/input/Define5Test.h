/*
** @file 
** @brief Define and use Macro (recursive definition).
*/
#define A B
#define B A

A // A --> B --> A (don't expand)
B // B --> A --> B (don't expand)
