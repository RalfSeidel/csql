/**
** @file
** @brief Sample and test of the quote operator:
*/
#define STD_STRINGIFICATION( E ) #E
#define SQT_STRINGIFICATION( E ) __QUOTE( E )
#define MY_DATABASE   sqtdb
#define MY_DATABASE_S STD_STRINGIFICATION( MY_DATABASE )
#define MY_DATABASE_Q SQT_STRINGIFICATION( MY_DATABASE )
// MY_DATABASE --> sqtdb
MY_DATABASE
// MY_DATABASE_S --> 'MY_DATABASE'
MY_DATABASE_S
// MY_DATABASE_Q --> 'sqtdb'
MY_DATABASE_Q
