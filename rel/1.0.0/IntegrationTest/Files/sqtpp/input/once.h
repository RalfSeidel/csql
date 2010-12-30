/**
** @file
** @brief Include some files two times the preprocessor should include only once.
*/
// first time --> must include
#include "include\once_explicite.h"
#include "include\once_implicite.h"

// second time --> dont include
#include "include\once_explicite.h"
#include "include\once_implicite.h"
