/*
** @file 
** @brief Check \#if directive.
*/
#if 1
OK
#else 
ERROR
#endif

#if 1 == 1
OK
#else 
ERROR
#endif


#if 1 == 0
ERROR
#else 
OK
#endif
