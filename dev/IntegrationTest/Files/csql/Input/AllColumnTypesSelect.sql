use tempdb
go


if not exists ( select 1 from sysusers where name = 'test' )
	exec sp_addrole 'test'
go


if exists ( select 1 from sysobjects where name = 'ColumnTypes' and type = 'U' and uid = user_id( 'test' ) )
	drop table test.ColumnTypes
go



create table test.ColumnTypes (
	c_id                int   identity  not null,
	c_bit               bit                 null,
--	c_id                numeric(5)  identity not null ,  -- Numeric for Sybase
--	c_bit               bit             not null,        -- Sybase erlaubt nicht bit null zu sein.
	c_tinyint           tinyint             null,
	c_smallint          smallint            null,
	c_int               int                 null,
	c_bigint            bigint              null,
	c_real              real                null,
	c_float             float               null,
	c_money             money               null,
	c_numeric           numeric(10,5)       null,
	c_datetime          datetime            null,
	c_varchar           varchar(5)          null,
	c_text              text                null,
	c_varbinary         varbinary(1000)     null,
	c_image             image               null,
	c_guid              uniqueidentifier    null,
	c_variant           sql_variant         null,
	c_timestamp         timestamp,
	
	constraint ColumnTypesPK primary key ( c_id )
)
go

grant select, insert, update, delete on test.ColumnTypes to public
go

declare @guid uniqueidentifier
set @guid = cast( '{9b7f1c49-7b08-4f78-bbb2-aeeb3d640fd1}' as uniqueidentifier )

insert into test.ColumnTypes ( c_bit ) values ( 0 )
insert into test.ColumnTypes ( c_bit ) values ( 1 )

insert into test.ColumnTypes ( c_tinyint ) values ( 0 )
insert into test.ColumnTypes ( c_tinyint ) values ( 255 )

insert into test.ColumnTypes ( c_smallint ) values ( 0 )
insert into test.ColumnTypes ( c_smallint ) values ( 32767 )

insert into test.ColumnTypes ( c_int ) values ( 0 )
insert into test.ColumnTypes ( c_int ) values ( 2147483647 )

insert into test.ColumnTypes ( c_bigint ) values ( 0 )
insert into test.ColumnTypes ( c_bigint ) values ( 2147483647 )

insert into test.ColumnTypes ( c_real ) values ( -3.40E+38 )
insert into test.ColumnTypes ( c_real ) values ( -1.18E-38 )
insert into test.ColumnTypes ( c_real ) values ( 0 )
insert into test.ColumnTypes ( c_real ) values ( 1.18E-38 )
insert into test.ColumnTypes ( c_real ) values ( 3.40E+38 )

insert into test.ColumnTypes ( c_float ) values ( -1.79E+308)
insert into test.ColumnTypes ( c_float ) values ( -2.23E-308 )
insert into test.ColumnTypes ( c_float ) values ( 0)
insert into test.ColumnTypes ( c_float ) values ( 2.23E-308 )
insert into test.ColumnTypes ( c_float ) values ( 1.79E+308)

insert into test.ColumnTypes ( c_numeric ) values ( -99999.99999)
insert into test.ColumnTypes ( c_numeric ) values ( -00000.00001)
insert into test.ColumnTypes ( c_numeric ) values ( 0)
insert into test.ColumnTypes ( c_numeric ) values ( 00000.00001)
insert into test.ColumnTypes ( c_numeric ) values ( 99999.99999)

insert into test.ColumnTypes ( c_money ) values ( -922337203685477.5808 )
insert into test.ColumnTypes ( c_money ) values ( -00000.0001 )
insert into test.ColumnTypes ( c_money ) values ( 0 )
insert into test.ColumnTypes ( c_money ) values ( 00000.0001 )
insert into test.ColumnTypes ( c_money ) values ( 922337203685477.5807 )

insert into test.ColumnTypes ( c_datetime ) values ( '20000101' )
insert into test.ColumnTypes ( c_datetime ) values ( '18991231' )
insert into test.ColumnTypes ( c_datetime ) values ( '19000101' )
insert into test.ColumnTypes ( c_datetime ) values ( '20000615' )
insert into test.ColumnTypes ( c_datetime ) values ( '20100601 06:00:00.000' )
insert into test.ColumnTypes ( c_datetime ) values ( '20100702 06:05:00.000' )
insert into test.ColumnTypes ( c_datetime ) values ( '20110803 12:15:44.000' )
insert into test.ColumnTypes ( c_datetime ) values ( '20120905 23:30:59.006' )

insert into test.ColumnTypes ( c_varchar ) values ( '' )
insert into test.ColumnTypes ( c_varchar ) values ( 'a' )
insert into test.ColumnTypes ( c_varchar ) values ( 'A' )
insert into test.ColumnTypes ( c_varchar ) values ( 'zzzzz' )
insert into test.ColumnTypes ( c_varchar ) values ( 'ZZZZZ' )

insert into test.ColumnTypes ( c_text ) values ( '' )
insert into test.ColumnTypes ( c_text ) values ( 'a' )
insert into test.ColumnTypes ( c_text ) values ( 'A' )
insert into test.ColumnTypes ( c_text ) values ( 'zzzzz' )
insert into test.ColumnTypes ( c_text ) values ( 'ZZZZZ' )
insert into test.ColumnTypes ( c_text ) values ( '0123456789_abcdefghijklmnopqrstuvwxyz_ABCDEFGHIJKLMNOPQRSTUVWXYZ' )

insert into test.ColumnTypes ( c_guid ) values ( '{00000000-0000-0000-0000-000000000000}' )
insert into test.ColumnTypes ( c_guid ) values ( @guid )

insert into test.ColumnTypes ( c_varbinary ) values( 0x00 )
insert into test.ColumnTypes ( c_varbinary ) values( 0x0102030405060708090A0B0C0E0F10 )

insert into test.ColumnTypes ( c_variant ) values( 0 )
insert into test.ColumnTypes ( c_variant ) values( 1 )
insert into test.ColumnTypes ( c_variant ) values( 'a' )
insert into test.ColumnTypes ( c_variant ) values( 'b' )
insert into test.ColumnTypes ( c_variant ) values( 0x00 )
insert into test.ColumnTypes ( c_variant ) values( 0x01 )
insert into test.ColumnTypes ( c_variant ) values( @guid )
go

select * from test.ColumnTypes where c_guid is not null
go

select * from test.ColumnTypes where c_datetime is not null
go


select * from test.ColumnTypes
go

if exists ( select 1 from sysobjects where name = 'ColumnTypes' and type = 'U' and uid = user_id( 'test' ) )
	drop table test.ColumnTypes
go

