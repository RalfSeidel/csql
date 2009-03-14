/*
** @file 
** @brief Check if a string comparison is recognized 
** (Note that the maximal length for strings in conditional 
** expressions is four characters).
**
** See Test script ErrC2015.h for an example of the behaviour if
** the defined string is longer than four characters.
*/
#define STRING 'ABCD'

#if STRING == 'ABCD'
OK
#endif