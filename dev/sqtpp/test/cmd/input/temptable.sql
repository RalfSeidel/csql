/**
** @file
** @brief Repro for a problem found by melanie. 
** Transact SQL Scripts containing lines which are starting with a temporary
** table name were not processed correctly.
*/
insert into
	#temp ( id, value )
values
	( 1, 'A' )
