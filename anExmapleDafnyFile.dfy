method MultipleReturns(tomsInputVar: int, tomsInputVar2: int) returns (tomsOutputVar: int, tomsOutputVar2: int)
   //requires 0 < y            //einkommentieren -> fehler postcondition geht weg
   ensures tomsOutputVar > 0
   ensures tomsOutputVar2 > 0
{
   tomsOutputVar := tomsInputVar;
   tomsOutputVar2 :=  tomsInputVar2;
}

