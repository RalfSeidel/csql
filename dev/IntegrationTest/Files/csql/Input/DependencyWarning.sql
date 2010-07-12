-- Create a procedure that references an object that does not exist.
-- The SQL Server will emit a warning (severity == )
-- Check that the context (file name/line number) is emitted.
if exists ( select 1 from sys.objects where object_id = object_id( 'dbo.pDependencyWarningTest' ) )
	drop procedure dbo.pDependencyWarningTest
go

create procedure dbo.pDependencyWarningTest
as
begin
	exec _NoExistingProcedure_
end
go



if exists ( select 1 from sys.objects where object_id = object_id( 'dbo.pDependencyWarningTest' ) )
	drop procedure dbo.pDependencyWarningTest
go
