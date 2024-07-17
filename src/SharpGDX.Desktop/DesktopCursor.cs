using SharpGDX.Utils;
using SharpGDX.Graphics;
using System.Runtime.InteropServices;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpGDX.Shims;

namespace SharpGDX.Desktop
{
	public class DesktopCursor : ICursor
	{
		static readonly Array<DesktopCursor> cursors = new ();
		static readonly Map<ICursor.SystemCursor, IntPtr> systemCursors = new();

		private static int inputModeBeforeNoneCursor = -1;

		readonly DesktopWindow window;
		Pixmap pixmapCopy;
		Image? glfwImage;
		internal unsafe readonly OpenTK.Windowing.GraphicsLibraryFramework.Cursor* glfwCursor;

		internal unsafe DesktopCursor(DesktopWindow window, Pixmap pixmap, int xHotspot, int yHotspot)
		{
			this.window = window;
			if (pixmap.getFormat() != Pixmap.Format.RGBA8888)
			{
				throw new GdxRuntimeException("Cursor image pixmap is not in RGBA8888 format.");
			}

			if ((pixmap.getWidth() & (pixmap.getWidth() - 1)) != 0)
			{
				throw new GdxRuntimeException(
					"Cursor image pixmap width of " + pixmap.getWidth() + " is not a power-of-two greater than zero.");
			}

			if ((pixmap.getHeight() & (pixmap.getHeight() - 1)) != 0)
			{
				throw new GdxRuntimeException(
					"Cursor image pixmap height of " + pixmap.getHeight() +
					" is not a power-of-two greater than zero.");
			}

			if (xHotspot < 0 || xHotspot >= pixmap.getWidth())
			{
				throw new GdxRuntimeException(
					"xHotspot coordinate of " + xHotspot + " is not within image width bounds: [0, " +
					pixmap.getWidth() + ").");
			}

			if (yHotspot < 0 || yHotspot >= pixmap.getHeight())
			{
				throw new GdxRuntimeException(
					"yHotspot coordinate of " + yHotspot + " is not within image height bounds: [0, " +
					pixmap.getHeight() + ").");
			}

			this.pixmapCopy = new Pixmap(pixmap.getWidth(), pixmap.getHeight(), Pixmap.Format.RGBA8888);
			this.pixmapCopy.setBlending(Pixmap.Blending.None);
			this.pixmapCopy.drawPixmap(pixmap, 0, 0);

			// TODO: Verify
			var imageDataHandle = GCHandle.Alloc(pixmapCopy.getPixels().array(), GCHandleType.Pinned);

			glfwImage = new Image()
			{
				Width = (pixmapCopy.getWidth()),
				Height = (pixmapCopy.getHeight()),
				Pixels = (byte*)imageDataHandle.AddrOfPinnedObject()
			};

			imageDataHandle.Free();

			glfwCursor = GLFW.CreateCursor(glfwImage.Value, xHotspot, yHotspot);
			cursors.add(this);
		}

		public unsafe void dispose()
		{
			if (pixmapCopy == null)
			{
				throw new GdxRuntimeException("Cursor already disposed");
			}

			cursors.removeValue(this, true);
			pixmapCopy.dispose();
			pixmapCopy = null;

			glfwImage = null;
			GLFW.DestroyCursor(glfwCursor);
		}

		internal static void dispose(DesktopWindow window)
		{
			for (int i = cursors.size - 1; i >= 0; i--)
			{
				DesktopCursor cursor = cursors.get(i);
				if (cursor.window.Equals(window))
				{
					cursors.removeIndex(i).dispose();
				}
			}
		}

		internal static unsafe void disposeSystemCursors()
		{
			foreach (long systemCursor in systemCursors.values())
			{
				// TODO: Verify cast. -RP
				GLFW.DestroyCursor((OpenTK.Windowing.GraphicsLibraryFramework.Cursor*)systemCursor);
			}

			systemCursors.clear();
		}

		internal static unsafe void setSystemCursor(Window* windowHandle, ICursor.SystemCursor systemCursor)
		{
			if (systemCursor == ICursor.SystemCursor.None)
			{
				if (inputModeBeforeNoneCursor == -1)
					inputModeBeforeNoneCursor = (int)GLFW.GetInputMode(windowHandle, CursorStateAttribute.Cursor);
				GLFW.SetInputMode(windowHandle, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
				return;
			}
			else if (inputModeBeforeNoneCursor != -1)
			{
				GLFW.SetInputMode(windowHandle, CursorStateAttribute.Cursor,
					(CursorModeValue)inputModeBeforeNoneCursor);
				inputModeBeforeNoneCursor = -1;
			}

			// TODO: Verify cast. -RP
			OpenTK.Windowing.GraphicsLibraryFramework.Cursor* glfwCursor =
				(OpenTK.Windowing.GraphicsLibraryFramework.Cursor*)systemCursors.get(systemCursor);

			if (glfwCursor == null)
			{
				OpenTK.Windowing.GraphicsLibraryFramework.Cursor* handle;
				if (systemCursor == ICursor.SystemCursor.Arrow)
				{
					handle = GLFW.CreateStandardCursor(CursorShape.Arrow);
				}
				else if (systemCursor == ICursor.SystemCursor.Crosshair)
				{
					handle = GLFW.CreateStandardCursor(CursorShape.Crosshair);
				}
				else if (systemCursor == ICursor.SystemCursor.Hand)
				{
					handle = GLFW.CreateStandardCursor(CursorShape.Hand);
				}
				else if (systemCursor == ICursor.SystemCursor.HorizontalResize)
				{
					handle = GLFW.CreateStandardCursor(CursorShape.HResize);
				}
				else if (systemCursor == ICursor.SystemCursor.VerticalResize)
				{
					handle = GLFW.CreateStandardCursor(CursorShape.VResize);
				}
				else if (systemCursor == ICursor.SystemCursor.Ibeam)
				{
					handle = GLFW.CreateStandardCursor(CursorShape.IBeam);
				}
				else if (systemCursor == ICursor.SystemCursor.NWSEResize)
				{
					//handle = GLFW.CreateStandardCursor(GLFW.GLFW_RESIZE_NWSE_CURSOR);
					throw new NotImplementedException();
				}
				else if (systemCursor == ICursor.SystemCursor.NESWResize)
				{
					//handle = GLFW.CreateStandardCursor(GLFW.GLFW_RESIZE_NESW_CURSOR);
					throw new NotImplementedException();
				}
				else if (systemCursor == ICursor.SystemCursor.AllResize)
				{
					//handle = GLFW.CreateStandardCursor(GLFW.GLFW_RESIZE_ALL_CURSOR);
					throw new NotImplementedException();
				}
				else if (systemCursor == ICursor.SystemCursor.NotAllowed)
				{
					// TODO: handle = GLFW.CreateStandardCursor(GLFW.GLFW_NOT_ALLOWED_CURSOR);
					throw new NotImplementedException();
				}
				else
				{
					throw new GdxRuntimeException("Unknown system cursor " + systemCursor);
				}

				if (handle == null)
				{
					return;
				}

				glfwCursor = handle;

				// TODO: Verify cast. -RP
				systemCursors.put(systemCursor, (IntPtr)glfwCursor);
			}

			GLFW.SetCursor(windowHandle, glfwCursor);
		}
	}
}