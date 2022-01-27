public record class OverhangTopology(byte[,] Intersections, double[,] Distances, double[,] Angles, Vector3d[,] Points)
{
    public ValueTuple<int, int> Dimensions => (Intersections.GetLength(0), Intersections.GetLength(1));
    public ValueTuple<byte, double, double, Vector3d> this[int x, int y]
    {
        get
        {
            return new ValueTuple<byte, double, double, Vector3d>(
            this.Intersections[x, y],
            this.Distances[x, y],
            this.Angles[x, y],
            this.Points[x, y]);
        }
    }
}