using SharpGDX.Graphics;
using SharpGDX.Utils;
using SharpGDX.Shims;
using File = SharpGDX.Shims.File;

namespace SharpGDX.Tests.Utils;

/** List of GdxTest classes. To be used by the test launchers. If you write your own test, add it in here!
 * 
 * @author badlogicgames@gmail.com */
public class GdxTests
{
	public static readonly List<Type> tests = new(){
        // @off
//        typeof(IssueTest),
//typeof(AccelerometerTest),
//typeof(ActionSequenceTest),
//typeof(ActionTest),
//typeof(Affine2Test),
//typeof(AlphaTest),
//typeof(Animation3DTest),
//typeof(AnimationTest),
//typeof(AnisotropyTest),
//typeof(AnnotationTest),
//typeof(AssetManagerTest),
//typeof(AtlasIssueTest),
//typeof(AudioChangeDeviceTest),
//typeof(AudioDeviceTest),
//typeof(AudioRecorderTest),
//typeof(AudioSoundAndMusicIsolationTest),
//typeof(Basic3DSceneTest),
//typeof(Basic3DTest),
//typeof(Benchmark3DTest),
//typeof(BigMeshTest),
//typeof(BitmapFontAlignmentTest),
//typeof(BitmapFontDistanceFieldTest),
//typeof(BitmapFontFlipTest),
//typeof(BitmapFontMetricsTest),
//typeof(BitmapFontTest),
//typeof(BitmapFontAtlasRegionTest),
//typeof(BlitTest),
//typeof(Box2DTest),
//typeof(Box2DTestCollection),
//typeof(Bresenham2Test),
//typeof(BufferUtilsTest),
//typeof(BulletTestCollection),
//typeof(ClipboardTest),
//typeof(CollectionsTest),
//typeof(CollisionPlaygroundTest),
//typeof(ColorTest),
//typeof(ContainerTest),
//typeof(CoordinatesTest),
//typeof(CpuSpriteBatchTest),
//typeof(CullTest),
//typeof(CursorTest),
//typeof(DecalTest),
//typeof(DefaultTextureBinderTest),
//typeof(DelaunayTriangulatorTest),
//typeof(DeltaTimeTest),
//typeof(DirtyRenderingTest),
//typeof(DisplayModeTest),
//typeof(DownloadTest),
//typeof(DragAndDropTest),
//typeof(ETC1Test),
////typeof(EarClippingTriangulatorTest),
//typeof(EdgeDetectionTest),
//typeof(ExitTest),
//typeof(ExternalMusicTest),
//typeof(FilesTest),
//typeof(FilterPerformanceTest),
//typeof(FloatTextureTest),
//typeof(FogTest),
//typeof(FrameBufferCubemapTest),
//typeof(FrameBufferTest),
//typeof(FramebufferToTextureTest),
//typeof(FullscreenTest),
//typeof(Gdx2DTest),
//typeof(GestureDetectorTest),
//typeof(GL30Texture3DTest),
//typeof(GLES30Test),
//typeof(GL31IndirectDrawingIndexedTest),
//typeof(GL31IndirectDrawingNonIndexedTest),
//typeof(GL31FrameBufferMultisampleMRTTest),
//typeof(GL31FrameBufferMultisampleTest),
//typeof(GL31ProgramIntrospectionTest),
//typeof(GL32AdvancedBlendingTest),
//typeof(GL32DebugControlTest),
//typeof(GL32MultipleRenderTargetsBlendingTest),
//typeof(GL32OffsetElementsTest),
//typeof(GlTexImage2D),
//typeof(GLProfilerErrorTest),
//typeof(GroupCullingTest),
//typeof(GroupFadeTest),
//typeof(GroupTest),
//typeof(HeightMapTest),
//typeof(HelloTriangle),
//typeof(HexagonalTiledMapTest),
//typeof(I18NMessageTest),
//typeof(I18NSimpleMessageTest),
//typeof(ImageScaleTest),
//typeof(ImageTest),
//typeof(ImmediateModeRendererTest),
//typeof(IndexBufferObjectShaderTest),
//typeof(InputTest),
//typeof(InstancedRenderingTest),
//typeof(IntegerBitmapFontTest),
//typeof(InterpolationTest),
//typeof(IntersectorOverlapConvexPolygonsTest),
//typeof(InverseKinematicsTest),
//typeof(IsometricTileTest),
//typeof(KinematicBodyTest),
//typeof(KTXTest),
//typeof(LabelScaleTest),
//typeof(LabelTest),
//typeof(LifeCycleTest),
//typeof(LightsTest),
//typeof(MaterialTest),
//typeof(MaterialEmissiveTest),
//typeof(MatrixJNITest),
//typeof(MeshBuilderTest),
//typeof(MeshShaderTest),
//typeof(MeshWithCustomAttributesTest),
//typeof(MipMapTest),
//typeof(ModelTest),
//typeof(ModelCacheTest),
//typeof(ModelInstancedRenderingTest),
//typeof(MoveSpriteExample),
//typeof(MultipleRenderTargetTest),
//typeof(MultitouchTest),
//typeof(MusicTest),
//typeof(NetAPITest),
//typeof(NinePatchTest),
typeof(NoncontinuousRenderingTest)//,
//typeof(NonPowerOfTwoTest),
//typeof(OctreeTest),
//typeof(OnscreenKeyboardTest),
//typeof(NativeInputTest),
//typeof(OrientedBoundingBoxTest),
//typeof(PathTest),
//typeof(ParallaxTest),
//typeof(ParticleControllerInfluencerSingleTest),
//typeof(ParticleControllerTest),
//typeof(ParticleEmitterTest),
//typeof(ParticleEmittersTest),
//typeof(ParticleEmitterChangeSpriteTest),
//typeof(PixelBufferObjectTest),
//typeof(PixelsPerInchTest),
//typeof(PixmapBlendingTest),
//typeof(PixmapPackerTest),
//typeof(PixmapPackerIOTest),
//typeof(PixmapTest),
//typeof(PolarAccelerationTest),
//typeof(PolygonRegionTest),
//typeof(PolygonSpriteTest),
//typeof(PreferencesTest),
//typeof(ProjectTest),
//typeof(ProjectiveTextureTest),
//typeof(ReflectionTest),
//typeof(ReflectionCorrectnessTest),
//typeof(RotationTest),
//typeof(RunnablePostTest),
//typeof(Scene2dTest),
//typeof(ScrollPane2Test),
//typeof(ScrollPaneScrollBarsTest),
//typeof(ScrollPaneTest),
//typeof(ScrollPaneTextAreaTest),
//typeof(ScrollPaneWithDynamicScrolling),
//typeof(SelectTest),
//typeof(SensorTest),
//typeof(ShaderCollectionTest),
//typeof(ShaderMultitextureTest),
//typeof(ShaderTest),
//typeof(ShadowMappingTest),
//typeof(ShapeRendererTest),
//typeof(ShapeRendererAlphaTest),
//typeof(SimpleAnimationTest),
//typeof(SimpleDecalTest),
//typeof(SimpleStageCullingTest),
//typeof(SimpleVertexShader),
//typeof(SkeletonTest),
//typeof(SoftKeyboardTest),
//typeof(SortedSpriteTest),
//typeof(SoundTest),
//typeof(SpriteBatchRotationTest),
//typeof(SpriteBatchPerformanceTest),
//typeof(SpriteBatchShaderTest),
//typeof(SpriteBatchTest),
//typeof(SpriteCacheOffsetTest),
//typeof(SpriteCacheTest),
//typeof(StageDebugTest),
//typeof(StagePerformanceTest),
//typeof(StageTest),
//typeof(SuperKoalio),
//typeof(SystemCursorTest),
//typeof(TableLayoutTest),
//typeof(TableTest),
//typeof(TangentialAccelerationTest),
//typeof(TextAreaTest),
//typeof(TextAreaTest2),
//typeof(TextAreaTest3),
//typeof(TextButtonTest),
//typeof(TextInputDialogTest),
//typeof(TextureAtlasTest),
//typeof(TextureArrayTest),
//typeof(TextureDataTest),
//typeof(TextureDownloadTest),
//typeof(TextureFormatTest),
//typeof(TextureRegion3DTest),
//typeof(TideMapAssetManagerTest),
//typeof(TideMapDirectLoaderTest),
//typeof(TiledDrawableTest),
//typeof(TileTest),
//typeof(TiledMapAnimationLoadingTest),
//typeof(TiledMapAssetManagerTest),
//typeof(TiledMapGroupLayerTest),
//typeof(TiledMapAtlasAssetManagerTest),
//typeof(TiledMapDirectLoaderTest),
//typeof(TiledMapModifiedExternalTilesetTest),
//typeof(TiledMapObjectLoadingTest),
//typeof(TiledMapObjectPropertyTest),
//typeof(TiledMapBench),
//typeof(TiledMapLayerOffsetTest),
//typeof(TimerTest),
//typeof(TimeUtilsTest),
//typeof(TouchpadTest),
//typeof(TreeTest),
//typeof(UISimpleTest),
//typeof(UITest),
//typeof(UniformBufferObjectsTest),
//typeof(UtfFontTest),
//typeof(VBOWithVAOPerformanceTest),
//typeof(Vector2dTest),
//typeof(VertexArrayTest),
//typeof(VertexBufferObjectShaderTest),
//typeof(VibratorTest),
//typeof(ViewportTest1),
//typeof(ViewportTest2),
//typeof(ViewportTest3),
//typeof(YDownTest),
//typeof(FreeTypeFontLoaderTest),
//typeof(FreeTypeDisposeTest),
//typeof(FreeTypeMetricsTest),
//typeof(FreeTypeIncrementalTest),
//typeof(FreeTypePackTest),
//typeof(FreeTypeAtlasTest),
//typeof(FreeTypeTest),
//typeof(InternationalFontsTest),
//typeof(PngTest),
//typeof(JsonTest),
//typeof(QuadTreeFloatTest),
//typeof(QuadTreeFloatNearestTest)

    // @on

    // SoundTouchTest.class, Mpg123Test.class, WavTest.class, FreeTypeTest.class,
    // VorbisTest.class
    };

	static readonly ObjectMap<String, String> obfuscatedToOriginal = new ();
	static readonly ObjectMap<String, String> originalToObfuscated = new ();
	static GdxTests() {
		InputStream mappingInput = new InputStream(System.IO.File.OpenRead("/mapping.txt"));
		if (mappingInput != null) {
			BufferedReader reader = null;
			try {
				reader = new BufferedReader(new InputStreamReader(mappingInput), 512);
				while (true) {
					String line = reader.readLine();
					if (line == null) break;
					if (line.StartsWith("    ")) continue;
					String[] split = line.Replace(":", "").Split(" -> ");
	String original = split[0];
					if (original.IndexOf('.') != -1) original = original.Substring(original.LastIndexOf('.') + 1);
					originalToObfuscated.put(original, split[1]);
					obfuscatedToOriginal.put(split[1], original);
				}
reader.close();
			} catch (Exception ex) {
				Console.WriteLine("GdxTests: Error reading mapping file: mapping.txt");
// TODO: ??? ex.printStackTrace();
			} finally {
	StreamUtils.closeQuietly(reader);
}
		}
	}

	public static List<String> getNames()
{
	List<String> names = new List<String>(tests.Count);
	foreach (Type clazz in tests)
			names.Add(obfuscatedToOriginal.get(clazz.Name, clazz.Name));

    names.Sort();
return names;
	}

    public static Type forName(String name)
    {
        name = originalToObfuscated.get(name, name);
        foreach (Type clazz in tests)
            if (clazz.Name.Equals(name))
                return clazz;
        return null;
    }

    public static GdxTest? newTest(String testName)
{
	testName = originalToObfuscated.get(testName, testName);
    try
    {
        return Activator.CreateInstance(Type.GetType(testName)) as GdxTest;
    }
    //catch (InstantiationException e)
    //{
    //	e.printStackTrace();
    //}
    //catch (IllegalAccessException e)
    //{
    //	e.printStackTrace();
    //}
    catch (Exception e)
    {
			// TODO: Get rid of this exception
            var s = e;
    }
	return null;
}
}
