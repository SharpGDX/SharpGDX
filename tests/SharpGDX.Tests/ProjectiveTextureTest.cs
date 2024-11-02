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
//public class ProjectiveTextureTest : GdxTest {
//
//	PerspectiveCamera cam;
//	PerspectiveCamera projector;
//	Texture texture;
//	Mesh plane;
//	Matrix4 planeTrans = new Matrix4();
//	Matrix4 cubeTrans = new Matrix4();
//	Matrix4 modelNormal = new Matrix4();
//	ShaderProgram projTexShader;
//	Stage ui;
//	Skin skin;
//	InputMultiplexer multiplexer = new InputMultiplexer();
//	PerspectiveCamController controller;
//	ImmediateModeRenderer20 renderer;
//
//	float angle = 0;
//	private SelectBox camera;
//	private Label fps;
//
//	public override void Create () {
//		setupScene();
//		setupUI();
//		setupShaders();
//
//		multiplexer.AddProcessor(ui);
//		multiplexer.AddProcessor(controller);
//		Gdx.input.setInputProcessor(multiplexer);
//
//		// renderer = new ImmediateModeRenderer20(false, true, 0);
//	}
//
//	public void setupScene () {
//		plane = new Mesh(true, 4, 6, new VertexAttribute(Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE),
//			new VertexAttribute(Usage.Normal, 3, ShaderProgram.NORMAL_ATTRIBUTE));
//		plane.setVertices(new float[] {-10, -1, 10, 0, 1, 0, 10, -1, 10, 0, 1, 0, 10, -1, -10, 0, 1, 0, -10, -1, -10, 0, 1, 0});
//		plane.setIndices(new short[] {3, 2, 1, 1, 0, 3});
//
//		texture = new Texture(Gdx.files.@internal("data/badlogic.jpg"), Format.RGB565, true);
//		texture.setFilter(Texture.TextureFilter.MipMap, Texture.TextureFilter.Nearest);
//
//		cam = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		cam.position.set(0, 5, 10);
//		cam.lookAt(0, 0, 0);
//		cam.update();
//		controller = new PerspectiveCamController(cam);
//
//		projector = new PerspectiveCamera(67, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
//		projector.position.set(2, 3, 2);
//		projector.lookAt(0, 0, 0);
//		projector.normalizeUp();
//		projector.update();
//	}
//
//	public void setupUI () {
//		ui = new Stage();
//		skin = new Skin(Gdx.files.@internal("data/uiskin.json"));
//		TextButton reload = new TextButton("Reload Shaders", skin.get(TextButtonStyle.class));
//		camera = new SelectBox(skin.get(SelectBoxStyle.class));
//		camera.setItems("Camera", "Light");
//		fps = new Label("fps: ", skin.get(LabelStyle.class));
//
//		Table table = new Table();
//		table.setFillParent(true);
//		table.top().padTop(15);
//		table.add(reload).spaceRight(5);
//		table.add(camera).spaceRight(5);
//		table.add(fps);
//		ui.addActor(table);
//
//		reload.addListener(new ClickListener() {
//			public void clicked (InputEvent event, float x, float y) {
//				ShaderProgram prog = new ShaderProgram(Gdx.files.@internal("data/shaders/projtex-vert.glsl").readString(),
//					Gdx.files.@internal("data/shaders/projtex-frag.glsl").readString());
//				if (prog.isCompiled() == false) {
//					Gdx.app.log("GLSL ERROR", "Couldn't reload shaders:\n" + prog.getLog());
//				} else {
//					projTexShader.Dispose();
//					projTexShader = prog;
//				}
//			}
//		});
//	}
//
//	public void setupShaders () {
//		ShaderProgram.pedantic = false;
//		projTexShader = new ShaderProgram(Gdx.files.@internal("data/shaders/projtex-vert.glsl").readString(),
//			Gdx.files.@internal("data/shaders/projtex-frag.glsl").readString());
//		if (!projTexShader.isCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + projTexShader.getLog());
//	}
//
//	public override void Render () {
//		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
//		Gdx.gl.glEnable(GL20.GL_DEPTH_TEST);
//
//		angle += Gdx.graphics.getDeltaTime() * 20.0f;
//		cubeTrans.setToRotation(Vector3.Y, angle);
//
//		cam.update();
//		projector.update();
//
//		texture.bind();
//		projTexShader.bind();
//
//		if (camera.getSelectedIndex() == 0) {
//			renderMesh(projTexShader, cam.combined, projector.combined, planeTrans, plane, Color.WHITE);
//			/*
//			 * TODO: Fix method rendering renderMesh(projTexShader, cam.combined, projector.combined, cubeTrans, cube, Color.WHITE);
//			 */
//		} else {
//			renderMesh(projTexShader, projector.combined, projector.combined, planeTrans, plane, Color.WHITE);
//			/*
//			 * TODO: Fix method rendering renderMesh(projTexShader, projector.combined, projector.combined, cubeTrans, cube,
//			 * Color.WHITE);
//			 */
//		}
//
//		fps.setText("fps: " + Gdx.graphics.getFramesPerSecond());
//		ui.act();
//		ui.draw();
//	}
//
//	Vector3 position = new Vector3();
//
//	private void renderMesh (ShaderProgram shader, Matrix4 cam, Matrix4 projector, Matrix4 model, Mesh mesh, Color color) {
//		position.set(this.projector.position);
//		modelNormal.set(model).toNormalMatrix();
//
//		shader.setUniformMatrix("u_camera", cam);
//		shader.setUniformMatrix("u_projector", projector);
//		shader.setUniformf("u_projectorPos", position.x, position.y, position.z);
//		shader.setUniformMatrix("u_model", model);
//		shader.setUniformMatrix("u_modelNormal", modelNormal);
//		shader.setUniformf("u_color", color.r, color.g, color.b);
//		shader.setUniformi("u_texture", 0);
//		mesh.render(shader, GL20.GL_TRIANGLES);
//	}
//
//	public override void Dispose () {
//		texture.Dispose();
//		plane.Dispose();
//		projTexShader.Dispose();
//		ui.Dispose();
//		skin.Dispose();
//		// renderer.Dispose();
//	}
//}
