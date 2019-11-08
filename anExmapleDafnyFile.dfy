method MultipleReturns(tomsInputVar: int, tomsInputVar2: int) returns (tomsOutputVar: int, tomsOutputVar2: int)
   //requires 0 < y            //einkommentieren -> fehler postcondition geht weg
   ensures tomsOutputVar > 0
   ensures tomsOutputVar2 > 0
{
   tomsOutputVar := x + y;
   tomsOutputVar2 := x - y;
   assert x == 1;              //auskommentieren -> assertion violation geht weg
   // bruder := 1;
   
}

