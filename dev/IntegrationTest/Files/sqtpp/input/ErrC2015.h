/*
** @file 
** @brief Check if a string comparison in a conditional with
** a string longer than four characters emitts the expected
** warning.
*/
#define STRING 'ABCDE'

#if STRING == 'ABCDE'
STRING
#endif