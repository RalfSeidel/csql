/**
** @file
** @brief Demonstration of the __EVAL macro.
*/
#define BIT0 1
#define BIT1 __EVAL( BIT0 << 1 )
#define BIT2 __EVAL( BIT0 << 2 )
#define BIT3 __EVAL( BIT0 << 3 )

BIT0   // Expands to 1
BIT1   // Expands to 2
BIT2   // Expands to 4
BIT3   // Expands to 8
__EVAL( 1 << 8 ) // Expands to 256


__EVAL( 'A' + 1 )
