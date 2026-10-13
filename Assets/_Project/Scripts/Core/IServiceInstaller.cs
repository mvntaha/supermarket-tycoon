namespace Stockwell.Core
{
    /// <summary>
    /// Implemented by components that register services. Bootstrap calls these explicitly
    /// after clearing the registry, so the order services appear in is readable in one
    /// place rather than depending on Unity's component execution order.
    /// </summary>
    public interface IServiceInstaller
    {
        void Install();
    }
}
