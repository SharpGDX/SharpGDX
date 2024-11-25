using SharpGDX.Graphics;
using SharpGDX.Graphics.G2D;
using SharpGDX.Scenes.Scene2D;
using SharpGDX.Scenes.Scene2D.Utils;
using SharpGDX.Scenes.Scene2D.UI;
using SharpGDX.Utils.Viewports;
using SharpGDX.Mathematics;
using SharpGDX.Tests.Utils;
using SharpGDX.Graphics.Profiling;

namespace SharpGDX.Tests.Utils;

public class GdxTestWrapper : IApplicationListener
{
	private IApplicationListener app;
	private bool logGLErrors;

	public GdxTestWrapper(IApplicationListener delegates, bool logGLErrors)
	: base()
	{

		this.app = delegates;
		this.logGLErrors = logGLErrors;
	}

	public void Create()
	{
		if (logGLErrors)
		{
			GDX.App.Log("GLProfiler", "profiler enabled");
			GLProfiler profiler = new GLProfiler(GDX.Graphics);
			profiler.setListener(new GLErrorListener());
		profiler.enable();
	}
	app.Create();
	}

    private class GLErrorListener : IGLErrorListener
    {
        public void onError(int error)
        {
            GDX.App.Error("GLProfiler", "error " + error);
        }
    }

public void Resize(int width, int height)
{
	app.Resize(width, height);
}

public void Render()
{
	app.Render();
}

public void Pause()
{
	app.Pause();
}

public void Resume()
{
	app.Resume();
}

public void Dispose()
{
	app.Dispose();
}

}
