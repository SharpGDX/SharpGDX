using SharpGDX.Utils;

namespace SharpGDX.Graphics.G3D.Utils;

public abstract class BaseShaderProvider : ShaderProvider
{
    protected Array<Shader> shaders = new Array<Shader>();

    public Shader getShader(Renderable renderable)
    {
        Shader? suggestedShader = renderable.shader;
        if (suggestedShader != null && suggestedShader.canRender(renderable)) return suggestedShader;
        foreach (Shader shader in shaders)
        {
            if (shader.canRender(renderable)) return shader;
        }

        {

            Shader shader = createShader(renderable);
            if (!shader.canRender(renderable))
                throw new GdxRuntimeException("unable to provide a shader for this renderable");
            shader.init();
            shaders.Add(shader);
            return shader;
        }
    }

    protected abstract Shader createShader(Renderable renderable);

    public void Dispose()
    {
        foreach (Shader shader in shaders)
        {
            shader.Dispose();
        }

        shaders.clear();
    }
}