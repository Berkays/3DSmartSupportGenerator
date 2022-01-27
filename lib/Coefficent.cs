using System.Diagnostics;

public record struct Coefficent
{
    public double value { get; init; }
    public CoefficentRelation relation { get; init; }

    public Coefficent(double value, CoefficentRelation coefficentRelation)
    {
        Debug.Assert(value >= 0 && value <= 1);
        this.value = value;
        this.relation = coefficentRelation;
    }
    public double Apply(double v) =>
    relation switch
    {
        CoefficentRelation.Proportional => this.value * v,
        CoefficentRelation.InvProportional => v / this.value,
        _ => throw new ArgumentException()
    };

}