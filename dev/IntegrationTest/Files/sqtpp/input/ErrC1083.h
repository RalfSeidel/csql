-- Test case for the bug 6725
print 'Before include'
#include "include\NonExisistingInclude.h"
print 'After include'
go