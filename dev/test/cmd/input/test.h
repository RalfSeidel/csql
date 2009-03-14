__FILE__
#define PARAM(a) a
#define FILE ""## __FILE__ ##""
FILE
PARAM
PARAM (b)

#define EMPTY
#define SIMPLE SIMPLE
#define MULTILINE MULTI \
	              LINE
#define EXPRESSION (1 + 2) * 3
#define PARAM(a) a
#define STRING( a ) #a
#define STRING2( a ) #@a
#define GLUE( a, b ) a ## b
#define GLUE2( a, b ) a##b
#define NESTED_SIMPLE SIMPLE
#define NESTED_PARAM     PARAM(SIMPLE)
#define NESTED_PARAM2(a) PARAM(a)



EMPTY
#ifdef EMPTY
SIMPLE
EXPRESSION
MULTILINE
NESTED_SIMPLE
NESTED_PARAM
NESTED_PARAM2(SIMPLE)
PARAM("x")
STRING(x""Y)
STRING(__FILE__)
STRING2('x')
GLUE(x, y)
GLUE2(x, y)

#else 
#error "should not happend"
#endif

#ifndef EMPTY
#error "should not happend"
#else
OK
#endif


#include "../input/once.h"
#include "once.h"

