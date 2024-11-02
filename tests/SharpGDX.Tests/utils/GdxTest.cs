namespace SharpGDX.Tests.Utils;

public abstract class GdxTest : InputAdapter, IApplicationListener
{
    public virtual void Create()
    {
    }

    public virtual void Dispose()
    {
    }

    public virtual void Pause()
    {
    }

    public virtual void Render()
    {
    }

    public virtual void Resize(int width, int height)
    {
    }

    public virtual void Resume()
    {
    }
}