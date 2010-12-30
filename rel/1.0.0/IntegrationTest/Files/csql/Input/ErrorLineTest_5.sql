-- Test to check line number synchronisation
-- The script contains code that is interpreted as a name of a stored procedure
-- The SQL Server will raise an exception with line number 1. Check that csql reports line 5.

#errordemo 