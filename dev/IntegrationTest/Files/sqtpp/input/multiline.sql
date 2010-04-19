#define CSQL_DROP_PROCEDURE(X) \
if exists (select 1 from sysobjects where type ='P' and id = object_id(#X) ) \
begin \
    print '' \
    print 'dropping procedure ' + #X \
    print '' \
    drop procedure X \
end \

CSQL_DROP_PROCEDURE( MyProc )

__LINE__ // should be 12
