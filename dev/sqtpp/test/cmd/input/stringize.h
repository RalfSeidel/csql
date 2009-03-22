/**
** @file
** @brief File for the stringize operator (\#\#).
*/
#define B X
#define STRINGIZE(A) #A
#define CHARIZE(A)   #@A

STRINGIZE( B )
STRINGIZE( "B" )
STRINGIZE( 'B' )

CHARIZE( B )
CHARIZE( "B" )
CHARIZE( 'B' )