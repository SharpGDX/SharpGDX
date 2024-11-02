//namespace SharpGDX.Tests.Desktop;
//using OpenTK.Windowing.GraphicsLibraryFramework;
//using SharpGDX.Shims;

//public class GlfwTest {
//	private static long windowHandle;
//	private static GLFWErrorCallback errorCallback = GLFWErrorCallback.createPrint(System.err);

//	public static void main (String[] argv) {
//		GLFW.SetErrorCallback(errorCallback);
//		if (!GLFW.Init()) {
//			Console.WriteLine("Couldn't initialize GLFW");
//			System.exit(-1);
//		}

//		GLFW.DefaultWindowHints();
//        GLFW.WindowHint(GLFW_VISIBLE, GLFW_FALSE);

//		// fullscreen, not current resolution, fails
//		Buffer modes = GLFW.GetVideoModes(glfwGetPrimaryMonitor());
//		for (int i = 0; i < modes.limit(); i++) {
//			Console.WriteLine(modes.get(i).width() + "x" + modes.get(i).height());
//		}
//		GLFWVidMode mode = modes.get(7);
//		Console.WriteLine("Mode: " + mode.width() + "x" + mode.height());
//		windowHandle = GLFW.CreateWindow(mode.width(), mode.height(), "Test", GLFW.GetPrimaryMonitor(), 0);
//		if (windowHandle == 0) {
//			throw new RuntimeException("Couldn't create window");
//		}
//        GLFW.MakeContextCurrent(windowHandle);
//		GL.createCapabilities();
//        GLFW.SwapInterval(1);
//        GLFW.ShowWindow(windowHandle);

//		IntBuffer tmp = BufferUtils.createIntBuffer(1);
//		IntBuffer tmp2 = BufferUtils.createIntBuffer(1);

//		int fbWidth = 0;
//		int fbHeight = 0;

//		while (!GLFW.WindowShouldClose(windowHandle)) {
//            GLFW.GetFramebufferSize(windowHandle, tmp, tmp2);
//			if (fbWidth != tmp.get(0) || fbHeight != tmp2.get(0)) {
//				fbWidth = tmp.get(0);
//				fbHeight = tmp2.get(0);
//				Console.WriteLine("Framebuffer: " + tmp.get(0) + "x" + tmp2.get(0));
//// GL11.glViewport(0, 0, tmp.get(0) * 2, tmp2.get(0) * 2);
//			}
//			GL11.glClear(GL11.GL_COLOR_BUFFER_BIT);
//			GL11.glBegin(GL11.GL_TRIANGLES);
//			GL11.glVertex2f(-1f, -1f);
//			GL11.glVertex2f(1f, -1f);
//			GL11.glVertex2f(0, 1f);
//			GL11.glEnd();
//            GLFW.SwapBuffers(windowHandle);
//            GLFW.PollEvents();
//		}

//        GLFW.DestroyWindow(windowHandle);
//        GLFW.Terminate();
//	}
//}
