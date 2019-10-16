
method Find(a: array<int>, key: int) returns (index: int)
// ensures 1==2
   ensures 0 <= index ==> index < a.Length && a[index] == key
   ensures index < 0 ==> forall k :: 0 <= k < a.Length ==> a[k] != key
{
   index := 0;
   while index < a.Length  
      invariant 0 <= index <= a.Length
      invariant forall k :: 0 <= k < index ==> a[k] != key
          //für alle k zwischen 0 bis index gilt...
      decreases a.Length - index
   {
      if a[index] == key { return; }
      index := index + 1;
   }
   index := -1;
}