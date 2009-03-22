









if exists (select 1 from sysobjects where type ='P' and id = object_id('MyProc') ) 
begin 
    print '' 
    print 'dropping procedure ' + 'MyProc' 
    print '' 
    drop procedure MyProc 
end 


12 
