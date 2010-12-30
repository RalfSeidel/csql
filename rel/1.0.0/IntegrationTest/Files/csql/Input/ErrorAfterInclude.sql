-- Test case for the bug 5562 
print 'Error after include test'
#include "include\Print1.h"
#errordemo 'error after include'
go