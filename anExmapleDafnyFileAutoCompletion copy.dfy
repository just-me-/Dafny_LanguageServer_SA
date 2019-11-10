//include "anIncludeeDafny.dfy"  

method MultipleReturns(x: int, y: int) returns (more: int, less: int)
   requires 0 < y 
   ensures less < x < more
{
   more := x + y;
   less := x - y;
   // assert x == 1;              //auskommentieren -> assertion violation geht weg
   var a := 1; 
   var aa := 1; 
   
}

class C {
   constructor () {
      var c := 1;
   }

   method m() {
      var b := 1+2;
      var c := b+1; 
   }
  }

/* xD wie macht man ne vererbung 2Do... 
class B : C {
   constructor()
}
*/

method Main() {
   var a := 1+2;
   print "a is ";
   print a; 
   var acc2 := new C();
   var acc3 := new C();
}

