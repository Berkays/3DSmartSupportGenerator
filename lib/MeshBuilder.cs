public static class MeshBuilder
{
    private static Vector3d CentimeterScaleVector = new Vector3d(0.1, 0.1, 0.1);
    private static Vector3d MeterScaleVector = new Vector3d(0.01, 0.01, 0.01);
    public static DMesh3 CreateMesh(Stream stream, MetricUnit unit = MetricUnit.Milimeter)
    {
        if (stream is null)
            throw new ArgumentNullException("Stream cannot be null");

        var stlReader = new STLReader();
        var meshBuilder = new DMesh3Builder();

        using BinaryReader br = new(stream);
        var result = stlReader.Read(br, ReadOptions.Defaults, meshBuilder);

        if (result.code != IOCode.Ok)
            throw new ArgumentException("File is not a valid STL");

        var mesh = meshBuilder.Meshes[0];

        // Move to origin
        MeshTransforms.Translate(mesh, -mesh.CachedBounds.Min);

        // Scale unit
        if (unit == MetricUnit.Centimeter)
            MeshTransforms.Scale(mesh, CentimeterScaleVector, Vector3d.Zero);
        else if (unit == MetricUnit.Meter)
            MeshTransforms.Scale(mesh, MeterScaleVector, Vector3d.Zero);

        return mesh;
    }

    public static void SaveMesh(Stream stream, MetricUnit unit = MetricUnit.Centimeter)
    {
    }
}