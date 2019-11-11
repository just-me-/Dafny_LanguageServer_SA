method MultipleReturns(x: int, y: int) returns (more: int, less: int)
   requires 0 < y 
   ensures less < x < more
{
   more := x + y;
   less := x - y;
   // assert x == 1;              //auskommentieren -> assertion violation geht weg
   var a := 1; 
   var aa := 1;
   assert x == 1;
   
}