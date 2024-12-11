using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Environments;

/** @author Xoppa */
public class DirectionalShadowLight : DirectionalLight , ShadowMap, IDisposable {
	protected FrameBuffer fbo;
	protected Camera cam;
	protected float halfDepth;
	protected float halfHeight;
	protected readonly Vector3 tmpV = new Vector3();
	protected readonly TextureDescriptor textureDesc;

	public DirectionalShadowLight (int shadowMapWidth, int shadowMapHeight, float shadowViewportWidth, float shadowViewportHeight,
		float shadowNear, float shadowFar) {
		fbo = new FrameBuffer(Pixmap.Format.RGBA8888, shadowMapWidth, shadowMapHeight, true);
		cam = new OrthographicCamera(shadowViewportWidth, shadowViewportHeight);
		cam.near = shadowNear;
		cam.far = shadowFar;
		halfHeight = shadowViewportHeight * 0.5f;
		halfDepth = shadowNear + 0.5f * (shadowFar - shadowNear);
		textureDesc = new TextureDescriptor();
		textureDesc.minFilter = textureDesc.magFilter = Texture.TextureFilter.Nearest;
		textureDesc.uWrap = textureDesc.vWrap = Texture.TextureWrap.ClampToEdge;
	}

	public void update (Camera camera) {
		update(tmpV.Set(camera.direction).scl(halfHeight), camera.direction);
	}

	public void update (Vector3 center, Vector3 forward) {
		cam.position.Set(direction).scl(-halfDepth).add(center);
		cam.direction.Set(direction).nor();
		cam.normalizeUp();
		cam.update();
	}

	public void begin ( Camera camera) {
		update(camera);
		begin();
	}

	public void begin ( Vector3 center,  Vector3 forward) {
		update(center, forward);
		begin();
	}

	public void begin () {
		 int w = fbo.getWidth();
		 int h = fbo.getHeight();
		fbo.begin();
		GDX.GL.glViewport(0, 0, w, h);
        GDX.GL.glClearColor(1, 1, 1, 1);
        GDX.GL.glClear(IGL20.GL_COLOR_BUFFER_BIT | IGL20.GL_DEPTH_BUFFER_BIT);
        GDX.GL.glEnable(IGL20.GL_SCISSOR_TEST);
        GDX.GL.glScissor(1, 1, w - 2, h - 2);
	}

	public void end () {
        GDX.GL.glDisable(IGL20.GL_SCISSOR_TEST);
		fbo.end();
	}

	public FrameBuffer getFrameBuffer () {
		return fbo;
	}

	public Camera getCamera () {
		return cam;
	}

	public Matrix4 getProjViewTrans () {
		return cam.Combined;
	}

	public TextureDescriptor getDepthMap () {
		textureDesc.texture = fbo.getColorBufferTexture();
		return textureDesc;
	}

	public void Dispose () {
		if (fbo != null) fbo.Dispose();
		fbo = null;
	}
}
