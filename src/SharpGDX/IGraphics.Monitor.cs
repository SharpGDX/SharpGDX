namespace SharpGDX;

public partial interface IGraphics
{
    /// <summary>
    ///     Describes a monitor.
    /// </summary>
    public class Monitor
    {
        public readonly string Name;
        public readonly int VirtualX;
        public readonly int VirtualY;

        protected Monitor(int virtualX, int virtualY, string name)
        {
            VirtualX = virtualX;
            VirtualY = virtualY;
            Name = name;
        }
    }
}