/*
** @file 
** @brief Check \#if directive.
*/
#define ZERO 0
#define ONE  1
#define TWO  2
#define OOPS 

#if ZERO
ERROR
#else 
OK
#endif

#if ONE 
OK
#else 
ERROR
#endif

#if TWO
OK
#else 
ERROR
#endif

#if OOPS
ERROR
#else 
OK
#endif


#if defined OOPS
OK
#else 
ERROR
#endif


#if defined( OOPS )
OK
#else 
ERROR
#endif

#if defined FUCK
ERROR
#else
OK
#endif

#if defined (FUCK )
ERROR
#else
OK
#endif
