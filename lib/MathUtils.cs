public static class MathUtils
{
    public static double Sigmoid(double x)
    {
        return Math.Clamp(1.0 / (Math.Exp(-x) + 1), 0, 1);
    }

    public static void MinMaxScale2D(double[,] data)
    {
        double min = Double.MaxValue;
        double max = Double.MinValue;

        int xBound = data.GetLength(0);
        int yBound = data.GetLength(1);
        // Span<double> rowBuffer = stackalloc double[xBound];

        for (int i = 0; i < xBound; i++)
        {
            for (int j = 0; j < yBound; j++)
            {
                double val = data[i, j];
                if (val < min)
                    min = val;
                if (val > max)
                    max = val;
            }
        }

        double sub = 1.0 / max - min;

        for (int i = 0; i < xBound; i++)
        {
            for (int j = 0; j < yBound; j++)
                data[i, j] = (data[i, j] - min) * sub;
        }
    }
}