method MultipleReturns(tomsInputVar: int, tomsInputVar2: int) returns (tomsOutputVar: int)
   //requires 0 < y            //einkommentieren -> fehler postcondition geht weg
   ensures tomsOutputVar > 0
{
   tomsOutputVar := -tomsInputVar+tomsInputVar2; 
}

