if exists ( select 1 from sys.objects where object_id = object_id('dbo.SelectTest') )
	drop table dbo.SelectTest
go

create table dbo.SelectTest (
	c_id                int   identity  not null,
	c_bit               bit                 null,
--	c_id                numeric(5)  identity not null ,  -- Numeric für Sybase
--	c_bit               bit             not null,         -- Sybase erlaubt nicht bit null zu sein.
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
	c_varbinary         varbinary(5)        null,
	c_image             image               null,
	c_guid              uniqueidentifier    null,
	c_variant           sql_variant         null,
	--c_timestamp         rowverion,
)
go

insert into dbo.SelectTest ( c_bit ) values ( 0 )
insert into dbo.SelectTest ( c_bit ) values ( 1 )

insert into dbo.SelectTest ( c_tinyint ) values ( 0 )
insert into dbo.SelectTest ( c_tinyint ) values ( 255 )

insert into dbo.SelectTest ( c_smallint ) values ( 0 )
insert into dbo.SelectTest ( c_smallint ) values ( 32767 )

insert into dbo.SelectTest ( c_int ) values ( 0 )
insert into dbo.SelectTest ( c_int ) values ( 2147483647 )

insert into dbo.SelectTest ( c_bigint ) values ( 0 )
insert into dbo.SelectTest ( c_bigint ) values ( 2147483647 )

insert into dbo.SelectTest ( c_real ) values ( -3.40E+38 )
insert into dbo.SelectTest ( c_real ) values ( -1.18E-38 )
insert into dbo.SelectTest ( c_real ) values ( 0 )
insert into dbo.SelectTest ( c_real ) values ( 1.18E-38 )
insert into dbo.SelectTest ( c_real ) values ( 3.40E+38 )

insert into dbo.SelectTest ( c_float ) values ( -1.79E+308)
insert into dbo.SelectTest ( c_float ) values ( -2.23E-308 )
insert into dbo.SelectTest ( c_float ) values ( 0)
insert into dbo.SelectTest ( c_float ) values ( 2.23E-308 )
insert into dbo.SelectTest ( c_float ) values ( 1.79E+308)

insert into dbo.SelectTest ( c_numeric ) values ( -99999.99999)
insert into dbo.SelectTest ( c_numeric ) values ( -00000.00001)
insert into dbo.SelectTest ( c_numeric ) values ( 0)
insert into dbo.SelectTest ( c_numeric ) values ( 00000.00001)
insert into dbo.SelectTest ( c_numeric ) values ( 99999.99999)

insert into dbo.SelectTest ( c_money ) values ( -922337203685477.5808 )
insert into dbo.SelectTest ( c_money ) values ( -00000.0001 )
insert into dbo.SelectTest ( c_money ) values ( 0 )
insert into dbo.SelectTest ( c_money ) values ( 00000.0001 )
insert into dbo.SelectTest ( c_money ) values ( 922337203685477.5807 )

insert into dbo.SelectTest ( c_datetime ) values ( '18991231' )
insert into dbo.SelectTest ( c_datetime ) values ( '19000101' )
insert into dbo.SelectTest ( c_datetime ) values ( '20000615' )
insert into dbo.SelectTest ( c_datetime ) values ( '23450101 11:55:00.333' )

insert into dbo.SelectTest ( c_varchar ) values ( '' )
insert into dbo.SelectTest ( c_varchar ) values ( 'a' )
insert into dbo.SelectTest ( c_varchar ) values ( 'A' )
insert into dbo.SelectTest ( c_varchar ) values ( 'zzzzz' )
insert into dbo.SelectTest ( c_varchar ) values ( 'ZZZZZ' )

insert into dbo.SelectTest ( c_text ) values ( '' )
insert into dbo.SelectTest ( c_text ) values ( 'a' )
insert into dbo.SelectTest ( c_text ) values ( 'A' )
insert into dbo.SelectTest ( c_text ) values ( 'zzzzz' )
insert into dbo.SelectTest ( c_text ) values ( 'ZZZZZ' )

insert into dbo.SelectTest ( c_guid ) values ( '{00000000-0000-0000-0000-000000000000}' )
insert into dbo.SelectTest ( c_guid ) values ( '{9B7F1C49-7B08-4f78-BBB2-AEEB3D640FD1}' )


insert into dbo.SelectTest ( c_variant ) values( 0 )
insert into dbo.SelectTest ( c_variant ) values( 1 )
insert into dbo.SelectTest ( c_variant ) values( 'a' )
insert into dbo.SelectTest ( c_variant ) values( 'b' )
insert into dbo.SelectTest ( c_variant ) values( 0x00 )
insert into dbo.SelectTest ( c_variant ) values( 0x01 )
insert into dbo.SelectTest ( c_variant ) values( cast( '{9B7F1C49-7B08-4f78-BBB2-AEEB3D640FD1}' as uniqueidentifier ) )
go

select * from dbo.SelectTest where 0 = 1
go

select * from dbo.SelectTest
go

if exists ( select 1 from sys.objects where object_id = object_id('dbo.SelectTest') )
	drop table dbo.SelectTest
go
