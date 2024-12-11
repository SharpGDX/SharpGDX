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
//
//namespace SharpGDX.Tests;
//
//public class CustomShaderSpriteBatchTest : GdxTest {
//	SpriteBatch batch;
//	ShaderProgram shader;
//	Texture texture;
//
//	public override void Create () {
//		batch = new SpriteBatch(10);
//		ShaderProgram.pedantic = false;
//		shader = new ShaderProgram(Gdx.files.@internal("data/shaders/batch.vert").readString(),
//			Gdx.files.@internal("data/shaders/batch.frag").readString());
//		batch.setShader(shader);
//		texture = new Texture("data/badlogic.jpg");
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//		batch.begin();
//		batch.draw(texture, 0, 0);
//		batch.end();
//	}
//
//	public override void Dispose () {
//		batch.Dispose();
//		shader.Dispose();
//		texture.Dispose();
//	}
//}
