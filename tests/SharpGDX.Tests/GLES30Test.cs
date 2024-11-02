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
//public class GLES30Test : GdxTest {
//
//	SpriteBatch batch;
//	Texture texture;
//	ShaderProgram shaderProgram;
//
//	public override void Create () {
//		Gdx.app.log("GLES30Test", "GL_VERSION = " + Gdx.gl.glGetString(GL20.GL_VERSION));
//		batch = new SpriteBatch();
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"));
//		shaderProgram = new ShaderProgram(Gdx.files.@internal("data/shaders/gles30sprite.vert"),
//			Gdx.files.@internal("data/shaders/gles30sprite.frag"));
//		Gdx.app.log("GLES30Test", shaderProgram.getLog());
//		if (shaderProgram.isCompiled()) {
//			Gdx.app.log("GLES30Test", "Shader compiled");
//			batch.setShader(shaderProgram);
//		}
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0, 0, 0, 1);
//
//		batch.begin();
//		batch.draw(texture, 0, 0, Gdx.graphics.getWidth() / 2f, Gdx.graphics.getHeight() / 2f);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		batch.Dispose();
//		shaderProgram.Dispose();
//	}
//}
