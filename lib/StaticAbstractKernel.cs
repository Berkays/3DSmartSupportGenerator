public abstract class StaticAbstractKernel<T, K>
{
    protected int size { get; init; }
    protected int offset { get; init; }
    protected int length => this.size * this.size;

    public StaticAbstractKernel(int size)
    {
        this.size = size;
        this.offset = (size - 1) / 2;
    }
    public virtual K Compute(OverhangTopology topology)
    {
        throw new NotImplementedException();
    }
    public virtual K Compute(T data)
    {
        throw new NotImplementedException();
    }
    public virtual K Compute(OverhangTopology topology, T data)
    {
        throw new NotImplementedException();
    }
}