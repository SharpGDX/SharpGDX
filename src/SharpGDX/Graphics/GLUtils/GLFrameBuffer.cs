using SharpGDX.Utils;
using SharpGDX.Shims;
using System.Text;

namespace SharpGDX.Graphics.GLUtils;

/**
 * <p>
 * Encapsulates OpenGL ES 2.0 frame buffer objects. This is a simple helper class which should cover most FBO uses. It will
 * automatically create a gltexture for the color attachment and a renderbuffer for the depth buffer. You can get a hold of the
 * gltexture by {@link GLFrameBuffer#getColorBufferTexture()}. This class will only work with OpenGL ES 2.0.
 * </p>
 *
 * <p>
 * FrameBuffers are managed. In case of an OpenGL context loss, which only happens on Android when a user switches to another
 * application or receives an incoming call, the framebuffer will be automatically recreated.
 * </p>
 *
 * <p>
 * A FrameBuffer must be disposed if it is no longer needed
 * </p>
 *
 * @author mzechner, realitix */
public abstract class GLFrameBuffer<T > : IDisposable 
where T : GLTexture
{
	/** the frame buffers **/
	protected readonly static Map<IApplication, Array<GLFrameBuffer<T>>> buffers = new ();

	protected readonly static int GL_DEPTH24_STENCIL8_OES = 0x88F0;

	/** the color buffer texture **/
	protected Array<T> textureAttachments = new Array<T>();

	/** the default framebuffer handle, a.k.a screen. */
	protected static int defaultFramebufferHandle;
	/** true if we have polled for the default handle already. */
	protected static bool defaultFramebufferHandleInitialized = false;

	/** the framebuffer handle **/
	protected int framebufferHandle;
	/** the depthbuffer render object handle **/
	protected int depthbufferHandle;
	/** the stencilbuffer render object handle **/
	protected int stencilbufferHandle;
	/** the depth stencil packed render buffer object handle **/
	protected int depthStencilPackedBufferHandle;
	/** if has depth stencil packed buffer **/
	protected bool hasDepthStencilPackedBuffer;
	/** the colorbuffer render object handles **/
	protected readonly IntArray colorBufferHandles = new IntArray();

	/** if multiple texture attachments are present **/
	protected bool isMRT;

	protected GLFrameBufferBuilder<GLFrameBuffer<GLTexture>> bufferBuilder;

	private IntBuffer defaultDrawBuffers;

	private protected GLFrameBuffer () {
	}

	/** Creates a GLFrameBuffer from the specifications provided by bufferBuilder **/
	protected GLFrameBuffer (GLFrameBufferBuilder<GLFrameBuffer<GLTexture>> bufferBuilder) {
		this.bufferBuilder = bufferBuilder;
		build();
	}

	/** Convenience method to return the first Texture attachment present in the fbo **/
	public T getColorBufferTexture () {
		return textureAttachments.first();
	}

	/** Return the Texture attachments attached to the fbo **/
	public Array<T> getTextureAttachments () {
		return textureAttachments;
	}

	/** Override this method in a derived class to set up the backing texture as you like. */
	protected abstract T createTexture (FrameBufferTextureAttachmentSpec attachmentSpec);

	/** Override this method in a derived class to dispose the backing texture as you like. */
	protected abstract void disposeColorTexture (T colorTexture);

	/** Override this method in a derived class to attach the backing texture to the GL framebuffer object. */
	protected abstract void attachFrameBufferColorTexture (T texture);

	protected void build () {
		IGL20 gl = Gdx.GL20;

		checkValidBuilder();

		// iOS uses a different framebuffer handle! (not necessarily 0)
		if (!defaultFramebufferHandleInitialized) {
			defaultFramebufferHandleInitialized = true;
			if (Gdx.App.GetType() == IApplication.ApplicationType.IOS) {
				IntBuffer intbuf = ByteBuffer.allocateDirect(16 * sizeof(int) / 8).order(ByteOrder.nativeOrder()).asIntBuffer();
				gl.glGetIntegerv(IGL20.GL_FRAMEBUFFER_BINDING, intbuf);
				defaultFramebufferHandle = intbuf.get(0);
			} else {
				defaultFramebufferHandle = 0;
			}
		}

		framebufferHandle = gl.glGenFramebuffer();
		gl.glBindFramebuffer(IGL20.GL_FRAMEBUFFER, framebufferHandle);

		int width = bufferBuilder.width;
		int height = bufferBuilder.height;

		if (bufferBuilder.hasDepthRenderBuffer) {
			depthbufferHandle = gl.glGenRenderbuffer();
			gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, depthbufferHandle);
			if (bufferBuilder.samples > 0) {
				Gdx.GL31.glRenderbufferStorageMultisample(IGL20.GL_RENDERBUFFER, bufferBuilder.samples,
					bufferBuilder.depthRenderBufferSpec.internalFormat, width, height);
			} else {
				gl.glRenderbufferStorage(IGL20.GL_RENDERBUFFER, bufferBuilder.depthRenderBufferSpec.internalFormat, width, height);
			}
		}

		if (bufferBuilder.hasStencilRenderBuffer) {
			stencilbufferHandle = gl.glGenRenderbuffer();
			gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, stencilbufferHandle);
			if (bufferBuilder.samples > 0) {
				Gdx.GL31.glRenderbufferStorageMultisample(IGL20.GL_RENDERBUFFER, bufferBuilder.samples,
					bufferBuilder.stencilRenderBufferSpec.internalFormat, width, height);
			} else {
				gl.glRenderbufferStorage(IGL20.GL_RENDERBUFFER, bufferBuilder.stencilRenderBufferSpec.internalFormat, width, height);
			}
		}

		if (bufferBuilder.hasPackedStencilDepthRenderBuffer) {
			depthStencilPackedBufferHandle = gl.glGenRenderbuffer();
			gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, depthStencilPackedBufferHandle);
			if (bufferBuilder.samples > 0) {
				Gdx.GL31.glRenderbufferStorageMultisample(IGL20.GL_RENDERBUFFER, bufferBuilder.samples,
					bufferBuilder.packedStencilDepthRenderBufferSpec.internalFormat, width, height);
			} else {
				gl.glRenderbufferStorage(IGL20.GL_RENDERBUFFER, bufferBuilder.packedStencilDepthRenderBufferSpec.internalFormat, width,
					height);
			}
			hasDepthStencilPackedBuffer = true;
		}

		isMRT = bufferBuilder.textureAttachmentSpecs.size > 1;
		int colorAttachmentCounter = 0;
		if (isMRT) {
			foreach (FrameBufferTextureAttachmentSpec attachmentSpec in bufferBuilder.textureAttachmentSpecs) {
				T texture = createTexture(attachmentSpec);
				textureAttachments.add(texture);
				if (attachmentSpec.isColorTexture()) {
					gl.glFramebufferTexture2D(IGL20.GL_FRAMEBUFFER, IGL30.GL_COLOR_ATTACHMENT0 + colorAttachmentCounter,
						IGL30.GL_TEXTURE_2D, texture.getTextureObjectHandle(), 0);
					colorAttachmentCounter++;
				} else if (attachmentSpec.isDepth) {
					gl.glFramebufferTexture2D(IGL20.GL_FRAMEBUFFER, IGL20.GL_DEPTH_ATTACHMENT, IGL20.GL_TEXTURE_2D,
						texture.getTextureObjectHandle(), 0);
				} else if (attachmentSpec.isStencil) {
					gl.glFramebufferTexture2D(IGL20.GL_FRAMEBUFFER, IGL20.GL_STENCIL_ATTACHMENT, IGL20.GL_TEXTURE_2D,
						texture.getTextureObjectHandle(), 0);
				}
			}
		} else if (bufferBuilder.textureAttachmentSpecs.size > 0) {
			T texture = createTexture(bufferBuilder.textureAttachmentSpecs.first());
			textureAttachments.add(texture);
			gl.glBindTexture(texture.glTarget, texture.getTextureObjectHandle());
		}

		foreach (FrameBufferRenderBufferAttachmentSpec colorBufferSpec in bufferBuilder.colorRenderBufferSpecs) {
			int colorbufferHandle = gl.glGenRenderbuffer();
			gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, colorbufferHandle);
			if (bufferBuilder.samples > 0) {
				Gdx.GL31.glRenderbufferStorageMultisample(IGL20.GL_RENDERBUFFER, bufferBuilder.samples, colorBufferSpec.internalFormat,
					width, height);
			} else {
				gl.glRenderbufferStorage(IGL20.GL_RENDERBUFFER, colorBufferSpec.internalFormat, width, height);
			}
			Gdx.GL.glFramebufferRenderbuffer(IGL20.GL_FRAMEBUFFER, IGL20.GL_COLOR_ATTACHMENT0 + colorAttachmentCounter,
				IGL20.GL_RENDERBUFFER, colorbufferHandle);
			colorBufferHandles.add(colorbufferHandle);
			colorAttachmentCounter++;
		}

		if (isMRT || bufferBuilder.samples > 0) {
			defaultDrawBuffers = BufferUtils.newIntBuffer(colorAttachmentCounter);
			for (int i = 0; i < colorAttachmentCounter; i++) {
				defaultDrawBuffers.put(IGL30.GL_COLOR_ATTACHMENT0 + i);
			}
			((Shims.Buffer)defaultDrawBuffers).position(0);
			Gdx.GL30.glDrawBuffers(colorAttachmentCounter, defaultDrawBuffers);
		} else if (bufferBuilder.textureAttachmentSpecs.size > 0) {
			attachFrameBufferColorTexture(textureAttachments.first());
		}

		if (bufferBuilder.hasDepthRenderBuffer) {
			gl.glFramebufferRenderbuffer(IGL20.GL_FRAMEBUFFER, IGL20.GL_DEPTH_ATTACHMENT, IGL20.GL_RENDERBUFFER, depthbufferHandle);
		}

		if (bufferBuilder.hasStencilRenderBuffer) {
			gl.glFramebufferRenderbuffer(IGL20.GL_FRAMEBUFFER, IGL20.GL_STENCIL_ATTACHMENT, IGL20.GL_RENDERBUFFER, stencilbufferHandle);
		}

		if (bufferBuilder.hasPackedStencilDepthRenderBuffer) {
			gl.glFramebufferRenderbuffer(IGL20.GL_FRAMEBUFFER, IGL30.GL_DEPTH_STENCIL_ATTACHMENT, IGL20.GL_RENDERBUFFER,
				depthStencilPackedBufferHandle);
		}

		gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, 0);
		foreach (T texture in textureAttachments) {
			gl.glBindTexture(texture.glTarget, 0);
		}

		int result = gl.glCheckFramebufferStatus(IGL20.GL_FRAMEBUFFER);

		if (result == IGL20.GL_FRAMEBUFFER_UNSUPPORTED && bufferBuilder.hasDepthRenderBuffer && bufferBuilder.hasStencilRenderBuffer
			&& (Gdx.Graphics.supportsExtension("GL_OES_packed_depth_stencil")
				|| Gdx.Graphics.supportsExtension("GL_EXT_packed_depth_stencil"))) {
			if (bufferBuilder.hasDepthRenderBuffer) {
				gl.glDeleteRenderbuffer(depthbufferHandle);
				depthbufferHandle = 0;
			}
			if (bufferBuilder.hasStencilRenderBuffer) {
				gl.glDeleteRenderbuffer(stencilbufferHandle);
				stencilbufferHandle = 0;
			}
			if (bufferBuilder.hasPackedStencilDepthRenderBuffer) {
				gl.glDeleteRenderbuffer(depthStencilPackedBufferHandle);
				depthStencilPackedBufferHandle = 0;
			}

			depthStencilPackedBufferHandle = gl.glGenRenderbuffer();
			hasDepthStencilPackedBuffer = true;
			gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, depthStencilPackedBufferHandle);
			if (bufferBuilder.samples > 0) {
				Gdx.GL31.glRenderbufferStorageMultisample(IGL20.GL_RENDERBUFFER, bufferBuilder.samples, GL_DEPTH24_STENCIL8_OES, width,
					height);
			} else {
				gl.glRenderbufferStorage(IGL20.GL_RENDERBUFFER, GL_DEPTH24_STENCIL8_OES, width, height);
			}
			gl.glBindRenderbuffer(IGL20.GL_RENDERBUFFER, 0);

			gl.glFramebufferRenderbuffer(IGL20.GL_FRAMEBUFFER, IGL20.GL_DEPTH_ATTACHMENT, IGL20.GL_RENDERBUFFER,
				depthStencilPackedBufferHandle);
			gl.glFramebufferRenderbuffer(IGL20.GL_FRAMEBUFFER, IGL20.GL_STENCIL_ATTACHMENT, IGL20.GL_RENDERBUFFER,
				depthStencilPackedBufferHandle);
			result = gl.glCheckFramebufferStatus(IGL20.GL_FRAMEBUFFER);
		}

		gl.glBindFramebuffer(IGL20.GL_FRAMEBUFFER, defaultFramebufferHandle);

		if (result != IGL20.GL_FRAMEBUFFER_COMPLETE) {
			foreach (T texture in textureAttachments) {
				disposeColorTexture(texture);
			}

			if (hasDepthStencilPackedBuffer) {
				gl.glDeleteBuffer(depthStencilPackedBufferHandle);
			} else {
				if (bufferBuilder.hasDepthRenderBuffer) gl.glDeleteRenderbuffer(depthbufferHandle);
				if (bufferBuilder.hasStencilRenderBuffer) gl.glDeleteRenderbuffer(stencilbufferHandle);
			}

			gl.glDeleteFramebuffer(framebufferHandle);

			if (result == IGL20.GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT)
				throw new IllegalStateException("Frame buffer couldn't be constructed: incomplete attachment");
			if (result == IGL20.GL_FRAMEBUFFER_INCOMPLETE_DIMENSIONS)
				throw new IllegalStateException("Frame buffer couldn't be constructed: incomplete dimensions");
			if (result == IGL20.GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT)
				throw new IllegalStateException("Frame buffer couldn't be constructed: missing attachment");
			if (result == IGL20.GL_FRAMEBUFFER_UNSUPPORTED)
				throw new IllegalStateException("Frame buffer couldn't be constructed: unsupported combination of formats");
			if (result == IGL31.GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE)
				throw new IllegalStateException("Frame buffer couldn't be constructed: multisample mismatch");
			throw new IllegalStateException("Frame buffer couldn't be constructed: unknown error " + result);
		}

		addManagedFrameBuffer(Gdx.App, this);
	}

	private void checkValidBuilder () {

		if (bufferBuilder.samples > 0 && !Gdx.Graphics.isGL31Available()) {
			throw new GdxRuntimeException("Framebuffer multisample requires GLES 3.1+");
		}
		if (bufferBuilder.samples > 0 && bufferBuilder.textureAttachmentSpecs.size > 0) {
			throw new GdxRuntimeException("Framebuffer multisample with texture attachments not yet supported");
		}

		bool runningGL30 = Gdx.Graphics.isGL30Available();

		if (!runningGL30) {
			bool supportsPackedDepthStencil = Gdx.Graphics.supportsExtension("GL_OES_packed_depth_stencil")
				|| Gdx.Graphics.supportsExtension("GL_EXT_packed_depth_stencil");

			if (bufferBuilder.hasPackedStencilDepthRenderBuffer && !supportsPackedDepthStencil) {
				throw new GdxRuntimeException("Packed Stencil/Render render buffers are not available on GLES 2.0");
			}
			if (bufferBuilder.textureAttachmentSpecs.size > 1) {
				throw new GdxRuntimeException("Multiple render targets not available on GLES 2.0");
			}
			foreach (FrameBufferTextureAttachmentSpec spec in bufferBuilder.textureAttachmentSpecs) {
				if (spec.isDepth) throw new GdxRuntimeException("Depth texture FrameBuffer Attachment not available on GLES 2.0");
				if (spec.isStencil) throw new GdxRuntimeException("Stencil texture FrameBuffer Attachment not available on GLES 2.0");
				if (spec.isFloat) {
					if (!Gdx.Graphics.supportsExtension("OES_texture_float")) {
						throw new GdxRuntimeException("Float texture FrameBuffer Attachment not available on GLES 2.0");
					}
				}
			}
		}

		if (bufferBuilder.hasPackedStencilDepthRenderBuffer) {
			if (bufferBuilder.hasDepthRenderBuffer || bufferBuilder.hasStencilRenderBuffer) throw new GdxRuntimeException(
				"Frame buffer couldn't be constructed: packed stencil depth buffer cannot be specified together with separated depth or stencil buffer");
		}
	}

	/** Releases all resources associated with the FrameBuffer. */
	public void Dispose () {
		IGL20 gl = Gdx.GL20;

		foreach (T texture in textureAttachments) {
			disposeColorTexture(texture);
		}

		gl.glDeleteRenderbuffer(depthStencilPackedBufferHandle);
		gl.glDeleteRenderbuffer(depthbufferHandle);
		gl.glDeleteRenderbuffer(stencilbufferHandle);

		gl.glDeleteFramebuffer(framebufferHandle);

		if (buffers.get(Gdx.App) != null) buffers.get(Gdx.App).removeValue(this, true);
	}

	/** Makes the frame buffer current so everything gets drawn to it. */
	public virtual void bind () {
		Gdx.GL20.glBindFramebuffer(IGL20.GL_FRAMEBUFFER, framebufferHandle);
	}

	/** Unbinds the framebuffer, all drawing will be performed to the normal framebuffer from here on. */
	public static void unbind () {
		Gdx.GL20.glBindFramebuffer(IGL20.GL_FRAMEBUFFER, defaultFramebufferHandle);
	}

	/** Binds the frame buffer and sets the viewport accordingly, so everything gets drawn to it. */
	public void begin () {
		bind();
		setFrameBufferViewport();
	}

	/** Sets viewport to the dimensions of framebuffer. Called by {@link #begin()}. */
	protected void setFrameBufferViewport () {
		Gdx.GL20.glViewport(0, 0, bufferBuilder.width, bufferBuilder.height);
	}

	/** Unbinds the framebuffer, all drawing will be performed to the normal framebuffer from here on. */
	public void end () {
		end(0, 0, Gdx.Graphics.getBackBufferWidth(), Gdx.Graphics.getBackBufferHeight());
	}

	/** Unbinds the framebuffer and sets viewport sizes, all drawing will be performed to the normal framebuffer from here on.
	 *
	 * @param x the x-axis position of the viewport in pixels
	 * @param y the y-asis position of the viewport in pixels
	 * @param width the width of the viewport in pixels
	 * @param height the height of the viewport in pixels */
	public void end (int x, int y, int width, int height) {
		unbind();
		Gdx.GL20.glViewport(x, y, width, height);
	}

	static readonly IntBuffer singleInt = BufferUtils.newIntBuffer(1);

	/** Transfer pixels from this frame buffer to the destination frame buffer. Usually used when using multisample, it resolves
	 * samples from this multisample FBO to a non-multisample as destination in order to be used as textures. This is a convenient
	 * method that automatically choose which of stencil, depth, and colors buffers attachment to be copied.
	 * @param destination the destination of the copy. */
	public void transfer (GLFrameBuffer<T> destination) {

		int copyBits = 0;
		foreach (FrameBufferTextureAttachmentSpec attachment in destination.bufferBuilder.textureAttachmentSpecs) {
			if (attachment.isDepth && (bufferBuilder.hasDepthRenderBuffer || bufferBuilder.hasPackedStencilDepthRenderBuffer)) {
				copyBits |= IGL20.GL_DEPTH_BUFFER_BIT;
			} else if (attachment.isStencil
				&& (bufferBuilder.hasStencilRenderBuffer || bufferBuilder.hasPackedStencilDepthRenderBuffer)) {
				copyBits |= IGL20.GL_STENCIL_BUFFER_BIT;
			} else if (colorBufferHandles.size > 0) {
				copyBits |= IGL20.GL_COLOR_BUFFER_BIT;
			}
		}

		transfer(destination, copyBits);
	}

	/** Transfer pixels from this frame buffer to the destination frame buffer. Usually used when using multisample, it resolves
	 * samples from this multisample FBO to a non-multisample as destination in order to be used as textures.
	 * @param destination the destination of the copy (should be same size as this frame buffer).
	 * @param copyBits combination of GL20.GL_COLOR_BUFFER_BIT, GL20.GL_STENCIL_BUFFER_BIT, and GL20.GL_DEPTH_BUFFER_BIT. When
	 *           GL20.GL_COLOR_BUFFER_BIT is present, every color buffers will be copied to each corresponding color texture
	 *           buffers in the destination framebuffer. */
	public void transfer (GLFrameBuffer<T> destination, int copyBits) {

		if (destination.getWidth() != getWidth() || destination.getHeight() != getHeight()) {
			throw new IllegalArgumentException("source and destination frame buffers must have same size.");
		}

		Gdx.GL.glBindFramebuffer(IGL30.GL_READ_FRAMEBUFFER, framebufferHandle);
		Gdx.GL.glBindFramebuffer(IGL30.GL_DRAW_FRAMEBUFFER, destination.framebufferHandle);

		int colorBufferIndex = 0;
		int attachmentIndex = 0;
		foreach (FrameBufferTextureAttachmentSpec attachment in destination.bufferBuilder.textureAttachmentSpecs) {
			if (attachment.isColorTexture()) {
				Gdx.GL30.glReadBuffer(IGL30.GL_COLOR_ATTACHMENT0 + colorBufferIndex);

				singleInt.clear();
				singleInt.put(IGL30.GL_COLOR_ATTACHMENT0 + attachmentIndex);
				singleInt.flip();
				Gdx.GL30.glDrawBuffers(1, singleInt);

				Gdx.GL30.glBlitFramebuffer(0, 0, getWidth(), getHeight(), 0, 0, destination.getWidth(), destination.getHeight(),
					copyBits, IGL20.GL_NEAREST);

				copyBits = IGL20.GL_COLOR_BUFFER_BIT;
				colorBufferIndex++;
			}
			attachmentIndex++;
		}
		// case of depth and/or stencil only
		if (copyBits != IGL20.GL_COLOR_BUFFER_BIT) {
			Gdx.GL30.glBlitFramebuffer(0, 0, getWidth(), getHeight(), 0, 0, destination.getWidth(), destination.getHeight(),
				copyBits, IGL20.GL_NEAREST);
		}

		// restore draw buffers for destination (in case of MRT only)
		if (destination.defaultDrawBuffers != null) {
			Gdx.GL30.glDrawBuffers(destination.defaultDrawBuffers.limit(), destination.defaultDrawBuffers);
		}

		Gdx.GL.glBindFramebuffer(IGL30.GL_READ_FRAMEBUFFER, 0);
		Gdx.GL.glBindFramebuffer(IGL30.GL_DRAW_FRAMEBUFFER, 0);
	}

	/** @return The OpenGL handle of the framebuffer (see {@link GL20#glGenFramebuffer()}) */
	public int getFramebufferHandle () {
		return framebufferHandle;
	}

	/** @return The OpenGL handle of the (optional) depth buffer (see {@link GL20#glGenRenderbuffer()}). May return 0 even if depth
	 *         buffer enabled */
	public int getDepthBufferHandle () {
		return depthbufferHandle;
	}

	/** @param n index of the color buffer as added to the frame buffer builder.
	 * @return The OpenGL handle of a color buffer (see {@link GL20#glGenRenderbuffer()}). **/
	public int getColorBufferHandle (int n) {
		return colorBufferHandles.get(n);
	}

	/** @return The OpenGL handle of the (optional) stencil buffer (see {@link GL20#glGenRenderbuffer()}). May return 0 even if
	 *         stencil buffer enabled */
	public int getStencilBufferHandle () {
		return stencilbufferHandle;
	}

	/** @return The OpenGL handle of the packed depth & stencil buffer (GL_DEPTH24_STENCIL8_OES) or 0 if not used. **/
	protected int getDepthStencilPackedBuffer () {
		return depthStencilPackedBufferHandle;
	}

	/** @return the height of the framebuffer in pixels */
	public int getHeight () {
		return bufferBuilder.height;
	}

	/** @return the width of the framebuffer in pixels */
	public int getWidth () {
		return bufferBuilder.width;
	}

	private static void addManagedFrameBuffer (IApplication app, GLFrameBuffer<T> frameBuffer) {
		var managedResources = buffers.get(app);
		if (managedResources == null) managedResources = new ();
		managedResources.add(frameBuffer);
		buffers.put(app, managedResources);
	}

	/** Invalidates all frame buffers. This can be used when the OpenGL context is lost to rebuild all managed frame buffers. This
	 * assumes that the texture attached to this buffer has already been rebuild! Use with care. */
	public static void invalidateAllFrameBuffers (IApplication app) {
		if (Gdx.GL20 == null) return;

		var bufferArray = buffers.get(app);
		if (bufferArray == null) return;
		for (int i = 0; i < bufferArray.size; i++) {
			bufferArray.get(i).build();
		}
	}

	public static void clearAllFrameBuffers (IApplication app) {
		buffers.remove(app);
	}

	public static StringBuilder getManagedStatus (StringBuilder builder) {
		builder.Append("Managed buffers/app: { ");
		foreach (IApplication app in buffers.keySet()) {
			builder.Append(buffers.get(app).size);
			builder.Append(" ");
		}
		builder.Append("}");
		return builder;
	}

	public static String getManagedStatus () {
		return getManagedStatus(new StringBuilder()).ToString();
	}



	
}

// TODO: Is this class supposed to be publicly available?
public class FrameBufferTextureAttachmentSpec
{
    internal int internalFormat, format, type;
    internal bool isFloat, isGpuOnly;
    internal bool isDepth;
    internal bool isStencil;

    public FrameBufferTextureAttachmentSpec(int internalformat, int format, int type)
    {
        this.internalFormat = internalformat;
        this.format = format;
        this.type = type;
    }

    public bool isColorTexture()
    {
        return !isDepth && !isStencil;
    }
}

// TODO: Is this class supposed to be publicly available?
public class FrameBufferRenderBufferAttachmentSpec
{
    internal int internalFormat;

    public FrameBufferRenderBufferAttachmentSpec(int internalFormat)
    {
        this.internalFormat = internalFormat;
    }
}

public abstract class GLFrameBufferBuilder<U>
    where U : GLFrameBuffer<GLTexture>
{
    internal protected int width, height, samples;

    internal protected Array<FrameBufferTextureAttachmentSpec> textureAttachmentSpecs = new Array<FrameBufferTextureAttachmentSpec>();
    internal protected Array<FrameBufferRenderBufferAttachmentSpec> colorRenderBufferSpecs = new Array<FrameBufferRenderBufferAttachmentSpec>();

    internal protected FrameBufferRenderBufferAttachmentSpec stencilRenderBufferSpec;
    internal protected FrameBufferRenderBufferAttachmentSpec depthRenderBufferSpec;
    internal protected FrameBufferRenderBufferAttachmentSpec packedStencilDepthRenderBufferSpec;

    internal protected bool hasStencilRenderBuffer;
    internal protected bool hasDepthRenderBuffer;
    internal protected bool hasPackedStencilDepthRenderBuffer;

    public GLFrameBufferBuilder(int width, int height)
    : this(width, height, 0)
    {

    }

    public GLFrameBufferBuilder(int width, int height, int samples)
    {
        this.width = width;
        this.height = height;
        this.samples = samples;
    }

    public GLFrameBufferBuilder<U> addColorTextureAttachment(int internalFormat, int format, int type)
    {
        textureAttachmentSpecs.add(new FrameBufferTextureAttachmentSpec(internalFormat, format, type));
        return this;
    }

    public GLFrameBufferBuilder<U> addBasicColorTextureAttachment(Pixmap.Format format)
    {
        int glFormat = Pixmap.FormatUtils.toGlFormat(format);
        int glType = Pixmap.FormatUtils.toGlType(format);
        return addColorTextureAttachment(glFormat, glFormat, glType);
    }

    public GLFrameBufferBuilder<U> addFloatAttachment(int internalFormat, int format, int type, bool gpuOnly)
    {
        FrameBufferTextureAttachmentSpec spec = new FrameBufferTextureAttachmentSpec(internalFormat, format, type);
        spec.isFloat = true;
        spec.isGpuOnly = gpuOnly;
        textureAttachmentSpecs.add(spec);
        return this;
    }

    public GLFrameBufferBuilder<U> addDepthTextureAttachment(int internalFormat, int type)
    {
        FrameBufferTextureAttachmentSpec spec = new FrameBufferTextureAttachmentSpec(internalFormat, IGL30.GL_DEPTH_COMPONENT,
            type);
        spec.isDepth = true;
        textureAttachmentSpecs.add(spec);
        return this;
    }

    public GLFrameBufferBuilder<U> addStencilTextureAttachment(int internalFormat, int type)
    {
        FrameBufferTextureAttachmentSpec spec = new FrameBufferTextureAttachmentSpec(internalFormat, IGL30.GL_STENCIL_ATTACHMENT,
            type);
        spec.isStencil = true;
        textureAttachmentSpecs.add(spec);
        return this;
    }

    public GLFrameBufferBuilder<U> addDepthRenderBuffer(int internalFormat)
    {
        depthRenderBufferSpec = new FrameBufferRenderBufferAttachmentSpec(internalFormat);
        hasDepthRenderBuffer = true;
        return this;
    }

    public GLFrameBufferBuilder<U> addColorRenderBuffer(int internalFormat)
    {
        colorRenderBufferSpecs.add(new FrameBufferRenderBufferAttachmentSpec(internalFormat));
        return this;
    }

    public GLFrameBufferBuilder<U> addStencilRenderBuffer(int internalFormat)
    {
        stencilRenderBufferSpec = new FrameBufferRenderBufferAttachmentSpec(internalFormat);
        hasStencilRenderBuffer = true;
        return this;
    }

    public GLFrameBufferBuilder<U> addStencilDepthPackedRenderBuffer(int internalFormat)
    {
        packedStencilDepthRenderBufferSpec = new FrameBufferRenderBufferAttachmentSpec(internalFormat);
        hasPackedStencilDepthRenderBuffer = true;
        return this;
    }

    public GLFrameBufferBuilder<U> addBasicDepthRenderBuffer()
    {
        return addDepthRenderBuffer(IGL20.GL_DEPTH_COMPONENT16);
    }

    public GLFrameBufferBuilder<U> addBasicStencilRenderBuffer()
    {
        return addStencilRenderBuffer(IGL20.GL_STENCIL_INDEX8);
    }

    public GLFrameBufferBuilder<U> addBasicStencilDepthPackedRenderBuffer()
    {
        return addStencilDepthPackedRenderBuffer(IGL30.GL_DEPTH24_STENCIL8);
    }

    public abstract object build();
}

public class FrameBufferBuilder : GLFrameBufferBuilder<GLFrameBuffer<GLTexture>>
{
    public FrameBufferBuilder(int width, int height)
    : base(width, height)
    {

    }

    public FrameBufferBuilder(int width, int height, int samples)
    : base(width, height, samples)
    {

    }

    public override FrameBuffer build()
    {
        return new FrameBuffer(this);
    }
}

public class FloatFrameBufferBuilder : GLFrameBufferBuilder<GLFrameBuffer<GLTexture>>
{
    public FloatFrameBufferBuilder(int width, int height)
    : base(width, height)
    {

    }

    public FloatFrameBufferBuilder(int width, int height, int samples)
    : base(width, height, samples)
    {

    }

    public override FloatFrameBuffer build()
    {
        return new FloatFrameBuffer(this);
    }
}

public class FrameBufferCubemapBuilder : GLFrameBufferBuilder<GLFrameBuffer<GLTexture>>
{
    public FrameBufferCubemapBuilder(int width, int height)
    : base(width, height)
    {

    }

    public FrameBufferCubemapBuilder(int width, int height, int samples)
    : base(width, height, samples)
    {

    }

    public override FrameBufferCubemap build()
    {
        return new FrameBufferCubemap(this);
    }
}
