public record class RegionContainer(byte[,] data, (int, int)[] indices)
{
    public byte this[int x, int y] => data[x, y];
}
/*
    (0,0)                  (0, Y)
          ------------------
          ------------------
          ------------------
          ------------------
          ------------------
    (X, 0)                 (X, Y)
*/