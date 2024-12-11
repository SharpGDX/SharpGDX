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
//public class MipMapTest : GdxTest {
//	PerspectiveCamera camera;
//	PerspectiveCamController controller;
//	Mesh mesh;
//	Texture textureHW;
//	Texture textureSW;
//	Texture currTexture;
//	ShaderProgram shader;
//	Stage ui;
//	Skin skin;
//	InputMultiplexer multiplexer;
//	SelectBox<String> minFilter;
//	SelectBox<String> magFilter;
//	CheckBox hwMipMap;
//
//	public override void Create () {
//		camera = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		camera.position.set(0, 1.5f, 1.5f);
//		camera.lookAt(0, 0, 0);
//		camera.update();
//		controller = new PerspectiveCamController(camera);
//
//		mesh = new Mesh(true, 4, 4, new VertexAttribute(Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE),
//			new VertexAttribute(Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE));
//		mesh.setVertices(new float[] {-1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, -1, 1, 0, -1, 0, -1, 0, 0,});
//		mesh.setIndices(new short[] {0, 1, 2, 3});
//
//		shader = new ShaderProgram(Gdx.files.@internal("data/shaders/flattex-vert.glsl").readString(),
//			Gdx.files.@internal("data/shaders/flattex-frag.glsl").readString());
//		if (!shader.isCompiled()) throw new GdxRuntimeException("shader error: " + shader.getLog());
//
//		textureHW = new Texture(Gdx.files.@internal("data/badlogic.jpg"), Format.RGB565, true);
//		MipMapGenerator.setUseHardwareMipMap(false);
//		textureSW = new Texture(Gdx.files.@internal("data/badlogic.jpg"), Format.RGB565, true);
//		currTexture = textureHW;
//
//		createUI();
//
//		multiplexer = new InputMultiplexer();
//		Gdx.input.setInputProcessor(multiplexer);
//		multiplexer.addProcessor(ui);
//		multiplexer.addProcessor(controller);
//	}
//
//	private void createUI () {
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		ui = new Stage();
//
//		String[] filters = new String[TextureFilter.values().length];
//		int idx = 0;
//		for (TextureFilter filter : TextureFilter.values()) {
//			filters[idx++] = filter.toString();
//		}
//		hwMipMap = new CheckBox("Hardware Mips", skin);
//		minFilter = new SelectBox(skin);
//		minFilter.setItems(filters);
//		magFilter = new SelectBox(skin.get(SelectBoxStyle.class));
//		magFilter.setItems("Nearest", "Linear");
//
//		Table table = new Table();
//		table.setSize(ui.getWidth(), 30);
//		table.setY(ui.getHeight() - 30);
//		table.add(hwMipMap).spaceRight(5);
//		table.add(new Label("Min Filter", skin)).spaceRight(5);
//		table.add(minFilter).spaceRight(5);
//		table.add(new Label("Mag Filter", skin)).spaceRight(5);
//		table.add(magFilter);
//
//		ui.addActor(table);
//	}
//
//	public override void Render () {
//		GDX.GL.glClear(GL20.GL_COLOR_BUFFER_BIT);
//
//		camera.update();
//
//		currTexture = hwMipMap.isChecked() ? textureHW : textureSW;
//		currTexture.bind();
//		currTexture.setFilter(Texture.TextureFilter.valueOf(minFilter.getSelected()), Texture.TextureFilter.valueOf(magFilter.getSelected()));
//
//		shader.bind();
//		shader.setUniformMatrix("u_projTrans", camera.combined);
//		shader.setUniformi("s_texture", 0);
//		mesh.render(shader, GL20.GL_TRIANGLE_FAN);
//
//		ui.act();
//		ui.draw();
//	}
//
//	public override void Dispose () {
//		shader.Dispose();
//		textureHW.Dispose();
//		textureSW.Dispose();
//		mesh.Dispose();
//		ui.Dispose();
//		skin.Dispose();
//	}
//}
