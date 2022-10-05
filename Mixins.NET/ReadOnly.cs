namespace Mixins
{
    /// <summary>
    /// Makes your instance ReadOnly
    /// </summary>
    public interface IReadOnly : IMixin
    {
        bool IsReadOnly { get; set; }
    }
}