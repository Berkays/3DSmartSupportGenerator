public class SupportKernel : StaticAbstractKernel<RegionContainer, double[,]>
{
    public Coefficent CfSupportHeight = new(0.2, CoefficentRelation.InvProportional);
    public Coefficent CfAngle = new(0.65, CoefficentRelation.Proportional);
    public Coefficent CfCorner = new(0.4, CoefficentRelation.Proportional);
    public Coefficent CfPlanarSupport = new(0.2, CoefficentRelation.Proportional);
    public Coefficent CfOverhangVolume = new(0.25, CoefficentRelation.Proportional);
    public Coefficent CfSupportCountMultiplier = new(0.1, CoefficentRelation.Proportional);

    public SupportKernel(int size) : base(size)
    {
    }

    public override double[,] Compute(OverhangTopology topology, RegionContainer regionContainer)
    {
        // Angle Window
        Span<double> angle_window = stackalloc double[this.length];

        // Get height distance matrix window
        Span<double> distance_window = stackalloc double[this.length];

        // TODO: Get overhang volume matrix window
        // Span<double> height_window = stackalloc double[this.length];
        // TODO: Calculate corner matrix window

        var data = regionContainer.data;
        var xRank = regionContainer.data.GetLength(0);
        var yRank = regionContainer.data.GetLength(1);

        Span<double> window = stackalloc double[this.length];
        Span<double> zero = stackalloc double[this.length];
        zero.Fill(0);

        MathUtils.MinMaxScale2D(topology.Angles);
        MathUtils.MinMaxScale2D(topology.Distances);

        double[,] final = new double[xRank, yRank];

        for (int offsetX = this.offset; offsetX < xRank - this.offset; offsetX++)
        {
            for (int offsetY = this.offset; offsetY < yRank - this.offset; offsetY++)
            {
                window.Fill(0);

                for (int i = 0; i < this.size; i++)
                {
                    for (int j = 0; j < this.size; j++)
                    {
                        int x = offsetX + i - this.offset;
                        int y = offsetY + j - this.offset;

                        int index = j + (i * this.size);
                        window[index] = (double)regionContainer[x, y];

                        angle_window[index] = MathUtils.Sigmoid(CfAngle.Apply(topology.Angles[x, y]));
                        distance_window[index] = MathUtils.Sigmoid(CfSupportHeight.Apply(topology.Distances[x, y]));
                    }
                }

                // Window not all zero
                if (window.SequenceEqual(zero))
                    continue;

                for (int i = 0; i < this.size; i++)
                {
                    for (int j = 0; j < this.size; j++)
                    {
                        int x = offsetX + i - this.offset;
                        int y = offsetY + j - this.offset;

                        int index = j + (i * this.size);
                        double val = window[index] * angle_window[index] * distance_window[index];
                        final[x, y] = val;
                    }
                }

            }
        }

        return final;
    }


}