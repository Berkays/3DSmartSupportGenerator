using System.Numerics;

public class OverhangProcessor
{
    public MetricValue Step { get; init; }
    public double MinAngle { get; init; }
    private DMesh3 Mesh { get; init; } = null!;
    private DMeshAABBTree3 Spatial { get; init; }
    private static readonly Vector3d AngleVector = Vector3d.AxisX + Vector3d.AxisY;
    private const double MAX_ANGLE = 90.0;
    private const double DEG2RAD = Math.PI / 180.0;

    public OverhangProcessor(DMesh3 mesh, MetricValue step, double minAngle = 45.0)
    {
        if (mesh is null)
            throw new ArgumentNullException("Mesh cannot be null");

        this.Mesh = mesh;
        this.Step = step;
        this.MinAngle = minAngle;

        this.Spatial = new DMeshAABBTree3(this.Mesh);
        Spatial.Build();
    }

    public OverhangTopology FindOverhangs()
    {
        Vector3d maxBounds = Mesh.CachedBounds.Max;

        double _step = this.Step.value;

        int X_STEP_COUNT = (int)(maxBounds.x / _step);
        int Y_STEP_COUNT = (int)(maxBounds.y / _step);

        byte[,] Intersections = new byte[X_STEP_COUNT, Y_STEP_COUNT];
        double[,] Distances = new double[X_STEP_COUNT, Y_STEP_COUNT];
        double[,] Angles = new double[X_STEP_COUNT, Y_STEP_COUNT];
        Vector3d[,] Points = new Vector3d[X_STEP_COUNT, Y_STEP_COUNT];

        ValueTuple<int, int>[] indices = new ValueTuple<int, int>[X_STEP_COUNT * Y_STEP_COUNT];

        for (int x = 0; x < X_STEP_COUNT; x++)
        {
            for (int y = 0; y < Y_STEP_COUNT; y++)
                indices[(x * Y_STEP_COUNT) + y] = (x, y);
        }

        Action<int> fn = new Action<int>((int i) =>
        {
            var (a, b) = indices[i];
            double x = a * _step;
            double y = b * _step;

            Vector3d origin = new Vector3d(x, y, 0);
            Ray3d ray = new Ray3d(origin, Vector3d.AxisZ);
            var hit = Spatial.FindNearestHitTriangle(ray);
            if (hit != DMesh3.InvalidID)
            {
                IntrRay3Triangle3 intr = MeshQueries.TriangleIntersection(Mesh, hit, ray);
                Vector3d hitVector = ray.PointAt(intr.RayParameter);

                var overhangAngle = intr.Triangle.Normal.AngleD(AngleVector);
                double hit_dist = origin.Distance(hitVector);

                if (hit_dist > 0.001 && overhangAngle >= MinAngle && overhangAngle < MAX_ANGLE)
                {
                    Intersections[a, b] = 1;
                    Distances[a, b] = hit_dist;
                    Angles[a, b] = overhangAngle * DEG2RAD;
                    Points[a, b] = hitVector;
                }
            }
        });

        Parallel.For(0, indices.Length, fn);

        return new OverhangTopology(Intersections, Distances, Angles, Points);
    }
}