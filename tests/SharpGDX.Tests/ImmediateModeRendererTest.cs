//using SharpGDX.Tests.Utils;
//using SharpGDX.Utils;
//using SharpGDX.Scenes.Scene2D;
//using SharpGDX.Scenes.Scene2D.Utils;
//using SharpGDX.Scenes.Scene2D.UI;
//using SharpGDX.Graphics;
//using SharpGDX.Graphics.G2D;
//using SharpGDX.Utils.Viewports;
//using SharpGDX.Shims;
//using SharpGDX.Mathematics;
//using SharpGDX.Graphics.GLUtils;
//
//namespace SharpGDX.Tests;
//
//public class ImmediateModeRendererTest : GdxTest {
//	Matrix4 projMatrix = new Matrix4();
//	ImmediateModeRenderer renderer;
//	Texture texture;
//
//	public override void Dispose () {
//		texture.Dispose();
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		Gdx.gl.glEnable(GL20.GL_TEXTURE_2D);
//		texture.bind();
//		renderer.begin(projMatrix, GL20.GL_TRIANGLES);
//		renderer.texCoord(0, 0);
//		renderer.color(1, 0, 0, 1);
//		renderer.vertex(-0.5f, -0.5f, 0);
//		renderer.texCoord(1, 0);
//		renderer.color(0, 1, 0, 1);
//		renderer.vertex(0.5f, -0.5f, 0);
//		renderer.texCoord(0.5f, 1);
//		renderer.color(0, 0, 1, 1);
//		renderer.vertex(0f, 0.5f, 0);
//		renderer.end();
//	}
//
//	public override void Create () {
//		renderer = new ImmediateModeRenderer20(false, true, 1);
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//	}
//}
