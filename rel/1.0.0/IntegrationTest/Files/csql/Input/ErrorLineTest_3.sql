-- Repro for the issue 3823.
-- Check the line number informating printed in the output.
#define MULTI_LINE_MACRO \
	print 'Macro Line 1' \
	print 'Macro Line 2' \
	print 'Macro Line 3'

raiserror 50000 'ErrorLineTest_3.sql - 8'
go

MULTI_LINE_MACRO
raiserror 50000 'ErrorLineTest_3.sql - 12'
go

raiserror 50000 'ErrorLineTest_3.sql - 15'
go
