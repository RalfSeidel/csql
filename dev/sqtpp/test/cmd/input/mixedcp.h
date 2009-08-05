// This is a file includes two other files with different code pages (cp1252 and UTF-8).
// The test verifies that the resulting output does not contain any BOM flags in the middle
// of the output file.
mixedcp_1252
#include "include/mixedcp_1252.h"

mixedcp_utf8
#include "include/mixedcp_utf8.h"

mixedcp_utf16
#include "include/mixedcp_utf16.h"

