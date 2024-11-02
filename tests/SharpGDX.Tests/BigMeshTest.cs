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
//public class BigMeshTest : GdxTest {
//
//	/** copied from {@link ModelBatch} */
//	protected static class RenderablePool : FlushablePool<Renderable> {
//		@Override
//		protected Renderable newObject () {
//			return new Renderable();
//		}
//
//		@Override
//		public Renderable obtain () {
//			Renderable renderable = super.obtain();
//			renderable.environment = null;
//			renderable.material = null;
//			renderable.meshPart.set("", null, 0, 0, 0);
//			renderable.shader = null;
//			renderable.userData = null;
//			return renderable;
//		}
//	}
//
//	private Camera camera;
//	private ModelBatch batch;
//	private final Array<RenderableProvider> renderableProviders = new Array<RenderableProvider>();
//
//	public override void Create () {
//
//		batch = new ModelBatch();
//
//		camera = new PerspectiveCamera();
//		camera.position.set(0, 1, 1).scl(3);
//		camera.up.set(Vector3.Y);
//		camera.lookAt(Vector3.Zero);
//		camera.near = .1f;
//		camera.far = 100f;
//
//		final Material material = new Material();
//		material.set(ColorAttribute.createDiffuse(Color.ORANGE));
//
//		final VertexAttributes attributes = new VertexAttributes(VertexAttribute.Position(), VertexAttribute.Normal(),
//			VertexAttribute.TexCoords(0));
//		final long attributesMask = attributes.getMask();
//
//		ModelBuilder mb = new ModelBuilder();
//
//		mb.begin();
//		MeshPartBuilder mpb = mb.part("ellipse", GL20.GL_TRIANGLES, attributes, material);
//
//		// create an ellipse with 64k vertices
//		float width = 1;
//		float height = 1;
//		float angleFrom = 0;
//		float angleTo = 360;
//		int divisions = (1 << 16) - 2; // ellipse: vertices count = divisions + 2
//		EllipseShapeBuilder.build(mpb, width, height, 0, 0, divisions, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, angleFrom, angleTo);
//
//		Model model = mb.end();
//
//		// Test few method relying on short indices
//		ModelInstance modelInstance = new ModelInstance(model);
//		Console.WriteLine(modelInstance.calculateBoundingBox(new BoundingBox()));
//		Mesh mesh = model.nodes.first().parts.first().meshPart.mesh;
//		Console.WriteLine(mesh.calculateRadius(0f, 0f, 0f, 0, mesh.getNumIndices(), new Matrix4()));
//
//		// model cache (simple)
//		modelInstance.transform.setTranslation(-2, 0, 0);
//		ModelCache modelCache = new ModelCache(new ModelCache.Sorter(), new ModelCache.SimpleMeshPool());
//		modelCache.begin();
//		modelCache.add(modelInstance);
//		modelCache.end();
//
//		// model cache (tight)
//		modelInstance.transform.setTranslation(2, 0, 0);
//		ModelCache modelCacheTight = new ModelCache(new ModelCache.Sorter(), new ModelCache.TightMeshPool());
//		modelCacheTight.begin();
//		modelCacheTight.add(modelInstance);
//		modelCacheTight.end();
//
//		modelInstance.transform.setTranslation(0, 0, 0);
//
//		renderableProviders.add(modelInstance);
//		renderableProviders.add(modelCache);
//		renderableProviders.add(modelCacheTight);
//
//		trace(modelInstance, "Model base");
//		trace(modelCache, "Model cache (simple)");
//		trace(modelCacheTight, "Model cache (tight)");
//	}
//
//	private void trace (RenderableProvider rp, String label) {
//		Array<Renderable> renderables = new Array<Renderable>();
//		Pool<Renderable> pool = new RenderablePool();
//		rp.getRenderables(renderables, pool);
//		Console.WriteLine(label + ":");
//		Console.WriteLine("- renderables: " + renderables.size);
//		for (Renderable r : renderables) {
//			Mesh mesh = r.meshPart.mesh;
//			Console.WriteLine("-- renderable [" + String.valueOf(r.meshPart.id) + "]: ");
//			Console.WriteLine("-- mesh part offset: " + r.meshPart.offset);
//			Console.WriteLine("-- mesh part size: " + r.meshPart.size);
//			Console.WriteLine("-- mesh num vertices: " + mesh.getNumVertices());
//			Console.WriteLine("-- mesh num indices: " + mesh.getNumIndices());
//		}
//	}
//
//	public override void Render () {
//		ScreenUtils.clear(0, 0, 0, 0, true);
//
//		camera.viewportWidth = Gdx.graphics.getWidth();
//		camera.viewportHeight = Gdx.graphics.getHeight();
//		camera.update();
//
//		batch.begin(camera);
//		batch.render(renderableProviders);
//		batch.end();
//	}
//
//}
