/**
** Recursive definition of macros with the concat operator.
*/ 
#define LOOP_1(proc,p1) \
	proc##(p1)

// Expecting:
//		myProc( @P1 )
LOOP_1( myProc, @P1 )

#define LOOP_2(proc,p1,p2) \
	LOOP_1(proc,p1) \
	LOOP_1(proc,p2) 

// Expecting: 
//		myProc( @P1 )
//		myProc( @P2 )
LOOP_2( myProc, @P1, @P2 )

#define LOOP_3(proc,p1,p2,p3) \
	LOOP_2(proc,p1,p2) \
	LOOP_1(proc,p3) 


// Expecting: 
//		myProc( @P1 )
//		myProc( @P2 )
//		myProc( @P3 )
LOOP_3( myProc, @P1, @P2, @P3 )

go
