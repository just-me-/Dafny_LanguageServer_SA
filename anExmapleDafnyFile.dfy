method Demo(x: int, y: int) returns (more: int, less: int)
   //requires 0 < y 
   ensures less < x < more
{
   more := x + y;
   less := x - y
   assert 1 == 2;
}