/**
** @file
** @brief File for the concatenate operator (\#\#).
*/
// Outside of a macro the concate operator has no effect
DONT ## CONCATE


// In a macro without parameters the concat operator works
#define CONCATE A ## B

// Expected: AB
CONCATE

#define A C
#define B D

// Expected: CD
CONCATE

#undef A
#undef B

#define CONCATE_ARGS( DO, CONCATE ) DO ## CONCATE

// Expected: AB
CONCATE_ARGS( A, B )
// Expected: CONCATEC
CONCATE_ARGS( CONCATE, C )
// Expected: CONCATE_ARGSC
CONCATE_ARGS( CONCATE_ARGS, C )
//Expected: ABC
CONCATE_ARGS( CONCATE_ARGS( A, B ), C )

#define A C
#define B D
// Expected: CD
CONCATE_ARGS( A, B )
#undef A
#undef B

#undef CONCATE_ARGS 
#define CONCATE_ARGS( DO, CONCATE ) DO##CONCATE

// Expected: AB
CONCATE
// Expected: AB
CONCATE_ARGS( A, B )
// Expected: CONCATEC
CONCATE_ARGS( CONCATE, C )
// Expected: CONCATE_ARGSC
CONCATE_ARGS( CONCATE_ARGS, C )
// Expected: ABC
CONCATE_ARGS( CONCATE_ARGS( A, B ), C )

#define A C
#define B D
// Expected: CD
CONCATE_ARGS( A, B )
#undef A
#undef B
