
use tempdb
go

if exists ( select 1 from sys.objects where object_id = object_id( 'dbo.pPrintTest' ) )
	drop procedure dbo.pPrintTest
go

create procedure dbo.pPrintTest
as
begin
	declare @ProcName sysname;
	declare @MsgText nvarchar(100)
	set @ProcName = object_name( @@procid )
	set @MsgText  = 'Hello world from procedure ' + @ProcName + char(13) + char(10) + 'Hello World!'
	print @MsgText
end
go

exec dbo.pPrintTest
go


if exists ( select 1 from sys.objects where object_id = object_id( 'dbo.pPrintTest' ) )
	drop procedure dbo.pPrintTest
go
