global using g3;
using System.Text;

using var fs = new FileStream(@"/workspaces/stl/tower.stl", FileMode.Open);
var mesh = MeshBuilder.CreateMesh(fs, MetricUnit.Milimeter); /// Unit in given STL file

MetricValue RayResolution = new(0.2, MetricUnit.Milimeter);
var overhang = new OverhangProcessor(mesh, RayResolution);
var topology = overhang.FindOverhangs();

var regionProcessor = new RegionProcessor(RayResolution);
var regionContainer = regionProcessor.ReduceTopology(topology);

var supportKernel = new SupportKernel(topology.Dimensions.Item1);

var final = supportKernel.Compute(topology, regionContainer);

var (xRank, yRank) = topology.Dimensions;
var sb = new StringBuilder();
for (int i = 0; i < xRank; i++)
{
    for (int j = 0; j < yRank; j++)
    {
        sb.Append(topology[i, j].Item1);
        sb.Append(",");
    }
    sb.AppendLine();
}

File.WriteAllText("/workspaces/stl/test_top.txt", sb.ToString());
sb.Clear();

for (int i = 0; i < xRank; i++)
{
    for (int j = 0; j < yRank; j++)
    {
        sb.Append(regionContainer[i, j]);
        sb.Append(",");
    }
    sb.AppendLine();
}
File.WriteAllText("/workspaces/stl/test_red.txt", sb.ToString());
sb.Clear();

for (int i = 0; i < xRank; i++)
{
    for (int j = 0; j < yRank; j++)
    {
        double val = final[i, j];
        if (val > 0.5)
            sb.Append(1);
        else
            sb.Append(0);

        sb.Append(",");
    }
    sb.AppendLine();
}
File.WriteAllText("/workspaces/stl/test_out.txt", sb.ToString());
sb.Clear();