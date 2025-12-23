namespace EasyToolKit.Inspector.Editor
{
    public interface IResolver : IHandler
    {
        /// <summary>
        /// Gets or sets the <see cref="IElement"/> that this resolver is associated with.
        /// </summary>
        IElement Element { get; set; }
    }
}
