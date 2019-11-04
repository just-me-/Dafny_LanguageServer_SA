method MultipleReturns(tomsInputVar: int) returns (tomsOutputVar: int)
   //requires 0 < y            //einkommentieren -> fehler postcondition geht weg
   ensures tomsOutputVar > 0
{
   return tomsInputVar;
}

