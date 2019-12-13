method MultipleReturns(inp1: int, inp2: int) returns (more: int, less: int)
   //ensures less < inp1 < more
{
   more := inp1 + inp2;
   less := inp1 - inp2;
}


method Demo(x: int, y: int) returns (more: int, less: int)
   requires 0 < y 
   ensures less < x < more
{
   more := x + y;
   less := x - y;
   // assert x == 1;
}

class C {
   constructor () { }
   method m() {
      // do somethin g
   }
}


method Main() {
   var a := 1+2;
   var acc := new C();
   acc.m(); 
}
