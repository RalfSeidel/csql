/**
** @file 
** @brief Define and use nested macros.
*/
#define SIMPLE           SIMPLE
#define PARAM(a)         a
#define NESTED_SIMPLE    SIMPLE
#define NESTED_PARAM     PARAM(SIMPLE)
#define NESTED_PARAM2(a) PARAM(a)
#define NESTED_PARAM3(a) NESTED_PARAM2(a)

// SIMPLE --> SIMPLE
SIMPLE
// PARAM(SIMPLE) --> SIMPLE
PARAM(SIMPLE)

// NESTED_PARAM  --> SIMPLE
NESTED_PARAM 

// NESTED_PARAM2(SIMPLE) -- > SIMPLE
NESTED_PARAM2(SIMPLE)

// NESTED_PARAM3(SIMPLE) --> SIMPLE
NESTED_PARAM3(SIMPLE)

// NESTED_PARAM2(x) --> x
NESTED_PARAM2(x)

// NESTED_PARAM3(x) --> x
NESTED_PARAM3(x)

// NESTED_PARAM3(SIMPLE) --> SIMPLE
NESTED_PARAM3(SIMPLE)
