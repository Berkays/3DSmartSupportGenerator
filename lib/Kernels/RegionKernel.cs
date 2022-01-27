public class RegionKernel : StaticAbstractKernel<byte, RegionContainer>
{
    public RegionKernel(int size) : base(size) { }

    public override RegionContainer Compute(OverhangTopology topology)
    {
        Span<byte> window = stackalloc byte[this.length];
        Span<byte> one = stackalloc byte[this.length];
        Span<ValueTuple<int, int>> regionSegment = stackalloc ValueTuple<int, int>[this.length];
        one.Fill(1);

        var data = topology.Intersections;
        var xRank = data.GetLength(0);
        var yRank = data.GetLength(1);

        byte[,] regions = new byte[xRank, yRank];
        List<(int, int)> regionIndices = new List<(int, int)>(128);

        for (int offsetX = this.offset; offsetX < xRank - this.offset; offsetX++)
        {
            for (int offsetY = this.offset; offsetY < yRank - this.offset; offsetY++)
            {
                window.Fill(0);
                regionSegment.Fill((0, 0));

                for (int i = 0; i < this.size; i++)
                {
                    for (int j = 0; j < this.size; j++)
                    {
                        int x = offsetX + i - this.offset;
                        int y = offsetY + j - this.offset;

                        byte cellValue = data[x, y];
                        if (cellValue == 0)
                        {
                            i = this.size;
                            break;
                        }

                        int index = j + (i * this.size);
                        window[index] = cellValue;
                        regionSegment[index] = (x, y);
                    }
                }

                if (window.SequenceEqual(one))
                {
                    for (int i = 0; i < this.length; i++)
                    {
                        regionIndices.Add((regionSegment[i].Item1, regionSegment[i].Item2));
                        regions[regionSegment[i].Item1, regionSegment[i].Item2] = 1;
                    }
                }
            }
        }

        return new(regions, regionIndices.ToArray());
    }
}