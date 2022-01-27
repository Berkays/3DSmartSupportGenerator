public record struct MetricValue(double value, MetricUnit unit = MetricUnit.Centimeter)
{
    public MetricValue ConvertToUnit(MetricUnit newUnit)
    {
        if (unit == newUnit)
            return this;
        else
            return new(this.value * Math.Pow(10, (int)this.unit - (int)newUnit), newUnit);
    }

    public static MetricValue operator +(MetricValue v0, double v1)
    {
        return new MetricValue(v0.value + v1, v0.unit);
    }
    public static MetricValue operator -(MetricValue v0, double v1)
    {
        return new MetricValue(v0.value - v1, v0.unit);
    }
    public static MetricValue operator -(MetricValue v)
    {
        return new MetricValue(-v.value, v.unit);
    }
    public static MetricValue operator *(MetricValue v0, double v1)
    {
        return new MetricValue(v0.value * v1, v0.unit);
    }
    public static MetricValue operator /(MetricValue v0, double v1)
    {
        return new MetricValue(v0.value / v1, v0.unit);
    }

    public static implicit operator MetricValue(double v)
    {
        return new MetricValue(v, MetricUnit.Milimeter);
    }
    public static explicit operator double(MetricValue v)
    {
        return v.ConvertToUnit(MetricUnit.Milimeter).value;
    }
}