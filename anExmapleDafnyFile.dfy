method MultipleReturns(tomsInputVar: int, tomsInputVar2: int) returns (tomsOutputVar: int, tomsOutputVar2: int)
   //requires 0 < y            //einkommentieren -> fehler postcondition geht weg
   ensures tomsOutputVar > 0
   ensures tomsOutputVar2 > 0
{
   more := x + y;
   less := x - y;
   // assert x == 1;              //auskommentieren -> assertion violation geht weg
   // bruder := 1;
   var a := 1; 
   var aa := 1; 
   
}

class C {
   constructor ()

   method m() 
  }

method Main() {
   var a := 1+2;
   print "a is ";
   print a;
   var acc := new C();
   // acc.
   
}
