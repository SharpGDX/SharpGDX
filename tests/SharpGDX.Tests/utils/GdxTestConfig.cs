namespace SharpGDX.Tests.Utils;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GdxTestConfig : Attribute
{
    public bool RequireGL30 { get; set; } = false;
    public bool RequireGL31 { get; set; } = false;
    public bool RequireGL32 { get; set; } = false;
    public bool OnlyGL20 { get; set; } = false;
}
