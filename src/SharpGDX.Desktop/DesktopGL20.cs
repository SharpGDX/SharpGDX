using SharpGDX.Graphics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using SharpGDX.Shims;
using SharpGDX.Utils;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Desktop
{
	internal class DesktopGL20 : GL20
	{
		private ByteBuffer buffer = null;
		private FloatBuffer floatBuffer = null;
		private IntBuffer intBuffer = null;

		private void ensureBufferCapacity(int numBytes)
		{
			if (buffer == null || buffer.capacity() < numBytes)
			{
				buffer = BufferUtils.newByteBuffer(numBytes);
				floatBuffer = buffer.asFloatBuffer();
				intBuffer = buffer.asIntBuffer();
			}
		}

		private FloatBuffer toFloatBuffer(float[] v, int offset, int count)
		{
			ensureBufferCapacity(count << 2);
			((Buffer)floatBuffer).clear();
			((Buffer)floatBuffer).limit(count);
			floatBuffer.put(v, offset, count);
			((Buffer)floatBuffer).position(0);
			return floatBuffer;
		}

		private IntBuffer toIntBuffer(int[] v, int offset, int count)
		{
			ensureBufferCapacity(count << 2);
			((Buffer)intBuffer).clear();
			((Buffer)intBuffer).limit(count);
			intBuffer.put(v, offset, count);
			((Buffer)intBuffer).position(0);
			return intBuffer;
		}

		public void glActiveTexture(int texture)
		{
			GL.ActiveTexture((TextureUnit)texture);
		}

		public void glAttachShader(int program, int shader)
		{
			GL.AttachShader(program, shader);
		}

		public void glBindAttribLocation(int program, int index, String name)
		{
			GL.BindAttribLocation(program, index, name);
		}

		public void glBindBuffer(int target, int buffer)
		{
			GL.BindBuffer((BufferTarget)target, buffer);
		}

		public void glBindFramebuffer(int target, int framebuffer)
		{
			throw new NotImplementedException();
			//	EXTFramebufferObject.glBindFramebufferEXT(target, framebuffer);
		}

		public void glBindRenderbuffer(int target, int renderbuffer)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glBindRenderbufferEXT(target, renderbuffer);
		}

		public void glBindTexture(int target, int texture)
		{
			GL.BindTexture((TextureTarget)target, texture);
		}

		public void glBlendColor(float red, float green, float blue, float alpha)
		{
			GL.BlendColor(red, green, blue, alpha);
		}

		public void glBlendEquation(int mode)
		{
			GL.BlendEquation((BlendEquationMode)mode);
		}

		public void glBlendEquationSeparate(int modeRGB, int modeAlpha)
		{
			GL.BlendEquationSeparate((BlendEquationMode)modeRGB, (BlendEquationMode)modeAlpha);
		}

		public void glBlendFunc(int sfactor, int dfactor)
		{
			GL.BlendFunc((BlendingFactor)sfactor, (BlendingFactor)dfactor);
		}

		public void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha)
		{
			GL.BlendFuncSeparate((BlendingFactorSrc)srcRGB, (BlendingFactorDest)dstRGB, (BlendingFactorSrc)srcAlpha, (BlendingFactorDest)dstAlpha);
		}

		public void glBufferData(int target, int size, Buffer data, int usage)
		{
			if (data == null)
			{
				// TODO: Not sure that this will work.
				GL.BufferData((BufferTarget)target, size, IntPtr.Zero, (BufferUsageHint)usage);
			}
			else if (data is ByteBuffer)
				GL.BufferData((BufferTarget)target, size, ((ByteBuffer)data).array(), (BufferUsageHint)usage);
			else if (data is IntBuffer)
				GL.BufferData((BufferTarget)target, size, ((IntBuffer)data).array(), (BufferUsageHint)usage);
			else if (data is FloatBuffer)
				GL.BufferData((BufferTarget)target, size, ((FloatBuffer)data).array(), (BufferUsageHint)usage);
			else if (data is DoubleBuffer)
				GL.BufferData((BufferTarget)target, size, ((DoubleBuffer)data).array(), (BufferUsageHint)usage);
			else if (data is ShortBuffer) //
				GL.BufferData((BufferTarget)target, size, ((ShortBuffer)data).array(), (BufferUsageHint)usage);
		}

		public void glBufferSubData(int target, int offset, int size, Buffer data)
		{
			throw new NotImplementedException();
			// //if (data == null)
			//	throw new GdxRuntimeException("Using null for the data not possible, blame LWJGL");
			//else if (data is ByteBuffer)
			//	GL.glBufferSubData(target, offset, (ByteBuffer)data);
			//else if (data is IntBuffer)
			//	GL.glBufferSubData(target, offset, (IntBuffer)data);
			//else if (data is FloatBuffer)
			//	GL.glBufferSubData(target, offset, (FloatBuffer)data);
			//else if (data is DoubleBuffer)
			//	GL.glBufferSubData(target, offset, (DoubleBuffer)data);
			//else if (data is ShortBuffer) //
			//	GL.glBufferSubData(target, offset, (ShortBuffer)data);
		}

		public int glCheckFramebufferStatus(int target)
		{
			throw new NotImplementedException();
			//return EXTFramebufferObject.glCheckFramebufferStatusEXT(target);
		}

		public void glClear(int mask)
		{
			GL.Clear((ClearBufferMask)mask);
		}

		public void glClearColor(float red, float green, float blue, float alpha)
		{
			GL.ClearColor(red, green, blue, alpha);
		}

		public void glClearDepthf(float depth)
		{
			GL.ClearDepth(depth);
		}

		public void glClearStencil(int s)
		{
			GL.ClearStencil(s);
		}

		public void glColorMask(bool red, bool green, bool blue, bool alpha)
		{
			GL.ColorMask(red, green, blue, alpha);
		}

		public void glCompileShader(int shader)
		{
			GL.CompileShader(shader);
		}

		public void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border,
			int imageSize, Buffer data)
		{
			if (data is ByteBuffer)
			{
				throw new NotImplementedException();
				//	GL.glCompressedTexImage2D(target, level, internalformat, width, height, border, (ByteBuffer)data);
			}
			else
			{
				throw new GdxRuntimeException("Can't use " + data.GetType().Name +
				                              " with this method. Use ByteBuffer instead.");
			}
		}

		public void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height,
			int format,
			int imageSize, Buffer data)
		{
			throw new GdxRuntimeException("not implemented");
		}

		public void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height,
			int border)
		{
			GL.CopyTexImage2D((TextureTarget)target, level, (InternalFormat)internalformat, x, y, width, height, border);
		}

		public void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width,
			int height)
		{
			GL.CopyTexSubImage2D((TextureTarget)target, level, xoffset, yoffset, x, y, width, height);
		}

		public int glCreateProgram()
		{
			return GL.CreateProgram();
		}

		public int glCreateShader(int type)
		{
			return GL.CreateShader((ShaderType)type);
		}

		public void glCullFace(int mode)
		{
			GL.CullFace((CullFaceMode)mode);
		}

		public void glDeleteBuffers(int n, IntBuffer buffers)
		{
			// TODO: If it should be writing something back into the buffers array, it needs to be updated.
			GL.DeleteBuffers(n, buffers.array());
		}

		public void glDeleteBuffer(int buffer)
		{
			GL.DeleteBuffer(buffer);
		}

		public void glDeleteFramebuffers(int n, IntBuffer framebuffers)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glDeleteFramebuffersEXT(framebuffers);
		}

		public void glDeleteFramebuffer(int framebuffer)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glDeleteFramebuffersEXT(framebuffer);
		}

		public void glDeleteProgram(int program)
		{
			GL.DeleteProgram(program);
		}

		public void glDeleteRenderbuffers(int n, IntBuffer renderbuffers)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glDeleteRenderbuffersEXT(renderbuffers);
		}

		public void glDeleteRenderbuffer(int renderbuffer)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glDeleteRenderbuffersEXT(renderbuffer);
		}

		public void glDeleteShader(int shader)
		{
			GL.DeleteShader(shader);
		}

		public void glDeleteTextures(int n, IntBuffer textures)
		{
			// TODO: If it should be writing something back into the buffers array, it needs to be updated.
			GL.DeleteTextures(n, textures.array());
		}

		public void glDeleteTexture(int texture)
		{
			throw new NotImplementedException();
			//GL.glDeleteTextures(texture);
		}

		public void glDepthFunc(int func)
		{
			GL.DepthFunc((DepthFunction)func);
		}

		public void glDepthMask(bool flag)
		{
			GL.DepthMask(flag);
		}

		public void glDepthRangef(float zNear, float zFar)
		{
			GL.DepthRange(zNear, zFar);
		}

		public void glDetachShader(int program, int shader)
		{	
			GL.DetachShader(program, shader);
		}

		public void glDisable(int cap)
		{
			GL.Disable((EnableCap)cap);
		}

		public void glDisableVertexAttribArray(int index)
		{
			GL.DisableVertexAttribArray(index);
		}

		public void glDrawArrays(int mode, int first, int count)
		{
			GL.DrawArrays((PrimitiveType)mode, first, count);
		}

		public void glDrawElements(int mode, int count, int type, Buffer indices)
		{
			GCHandle bufferHandle;

			// TODO: Should this be in a try/finally?
			if (indices is ShortBuffer && type == GL20.GL_UNSIGNED_SHORT)
			{
				ShortBuffer sb = (ShortBuffer)indices;
				int position = sb.position();
				int oldLimit = sb.limit();
				sb.limit(position + count);

				bufferHandle = GCHandle.Alloc(sb.array(), GCHandleType.Pinned);

				// TODO: GL.glDrawElements(mode, sb.remaining(), GL11.GL_UNSIGNED_SHORT, bufferHandle.AddrOfPinnedObject());
				GL.DrawElements((PrimitiveType)mode, count, (DrawElementsType)type, sb.array());
				sb.limit(oldLimit);
			}
			else if (indices is ByteBuffer && type == GL20.GL_UNSIGNED_SHORT)
			{
				ShortBuffer sb = ((ByteBuffer)indices).asShortBuffer();
				int position = sb.position();
				int oldLimit = sb.limit();
				sb.limit(position + count);

				bufferHandle = GCHandle.Alloc(sb.array(), GCHandleType.Pinned);
				GL.DrawElements((PrimitiveType)mode, sb.remaining(), DrawElementsType.UnsignedShort, bufferHandle.AddrOfPinnedObject());
				sb.limit(oldLimit);
			}
			else if (indices is ByteBuffer && type == GL20.GL_UNSIGNED_BYTE)
			{
				ByteBuffer bb = (ByteBuffer)indices;
				int position = bb.position();
				int oldLimit = bb.limit();
				bb.limit(position + count);

				bufferHandle = GCHandle.Alloc(bb.array(), GCHandleType.Pinned);
				GL.DrawElements((PrimitiveType)mode, bb.remaining(), DrawElementsType.UnsignedByte, bufferHandle.AddrOfPinnedObject());
				bb.limit(oldLimit);
			}
			else
				throw new GdxRuntimeException("Can't use " + indices.GetType().Name
														   + " with this method. Use ShortBuffer or ByteBuffer instead. Blame LWJGL");

			bufferHandle.Free();
		}

		public void glEnable(int cap)
		{
			GL.Enable((EnableCap)cap);
		}

		public void glEnableVertexAttribArray(int index)
		{
			GL.EnableVertexAttribArray(index);
		}

		public void glFinish()
		{
			GL.Finish();
		}

		public void glFlush()
		{
				GL.Flush();
		}

		public void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer)
		{
			throw new NotImplementedException();
			//	EXTFramebufferObject.glFramebufferRenderbufferEXT(target, attachment, renderbuffertarget, renderbuffer);
		}

		public void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level)
		{
			throw new NotImplementedException();
			//	EXTFramebufferObject.glFramebufferTexture2DEXT(target, attachment, textarget, texture, level);
		}

		public void glFrontFace(int mode)
		{
			GL.FrontFace((FrontFaceDirection)mode);
		}

		public void glGenBuffers(int n, IntBuffer buffers)
		{
			throw new NotImplementedException();
			//GL.glGenBuffers(buffers);
		}

		public int glGenBuffer()
		{
			return GL.GenBuffer();
		}

		public void glGenFramebuffers(int n, IntBuffer framebuffers)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glGenFramebuffersEXT(framebuffers);
		}

		public int glGenFramebuffer()
		{
			throw new NotImplementedException();
			//return EXTFramebufferObject.glGenFramebuffersEXT();
		}

		public void glGenRenderbuffers(int n, IntBuffer renderbuffers)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glGenRenderbuffersEXT(renderbuffers);
		}

		public int glGenRenderbuffer()
		{
			throw new NotImplementedException();
			//return EXTFramebufferObject.glGenRenderbuffersEXT();
		}

		public void glGenTextures(int n, IntBuffer textures)
		{
			// TODO: If it should be writing something back into the buffers array, it needs to be updated.
			GL.GenTextures(textures.remaining(), textures.array());
		}

		public int glGenTexture()
		{
			return GL.GenTexture();
		}

		public void glGenerateMipmap(int target)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glGenerateMipmapEXT(target);
		}

		public String glGetActiveAttrib(int program, int index, IntBuffer size, IntBuffer type)
		{
			var s =GL.GetActiveAttrib(program, index,out int l, out var a);

			return s;
		}

		public String glGetActiveUniform(int program, int index, IntBuffer size, IntBuffer type)
		{
			var s =GL.GetActiveUniform(program, index, out int l, out ActiveUniformType a);

			return s;
		}

		public void glGetAttachedShaders(int program, int maxcount, Buffer count, IntBuffer shaders)
		{
			throw new NotImplementedException();
			//GL.glGetAttachedShaders(program, (IntBuffer)count, shaders);
		}

		public int glGetAttribLocation(int program, String name)
		{
			var results= GL.GetAttribLocation(program, name);

			return results;
		}

		public void glGetBooleanv(int pname, Buffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetBooleanv(pname, (ByteBuffer)@params);
		}

		public void glGetBufferParameteriv(int target, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetBufferParameteriv(target, pname, @params);
		}

		public int glGetError()
		{
			var results= GL.GetError();
			
			return (int)results;
		}

		public void glGetFloatv(int pname, FloatBuffer @params)
		{
			@params.put(0, GL.GetFloat((GetPName)pname));
		}

		public void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glGetFramebufferAttachmentParameterivEXT(target, attachment, pname, @params);
		}

		public void glGetIntegerv(int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetIntegerv(pname, @params);
		}

		public String glGetProgramInfoLog(int program)
		{
			throw new NotImplementedException();
			// //ByteBuffer buffer = ByteBuffer.allocateDirect(1024 * 10);
			//buffer.order(ByteOrder.nativeOrder());
			//ByteBuffer tmp = ByteBuffer.allocateDirect(4);
			//tmp.order(ByteOrder.nativeOrder());
			//IntBuffer intBuffer = tmp.asIntBuffer();

			//GL.glGetProgramInfoLog(program, intBuffer, buffer);
			//int numBytes = intBuffer.get(0);
			//byte[] bytes = new byte[numBytes];
			//buffer.get(bytes);
			//return new String(bytes);
		}

		public void glGetProgramiv(int program, int pname, IntBuffer @params)
		{
			// TODO: I'm not sure why passing @params.array() won't work in this case, but this is a workaround. -RP
			var buffer = new int[@params.limit()];
			GL.GetProgram(program, (GetProgramParameterName)pname, buffer);
			@params.put(buffer);
		}

		public void glGetRenderbufferParameteriv(int target, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glGetRenderbufferParameterivEXT(target, pname, @params);
		}

		public String glGetShaderInfoLog(int shader)
		{
			GL.GetShaderInfoLog(shader, out string logInfo);

			return logInfo;
		}

		public void glGetShaderPrecisionFormat(int shadertype, int precisiontype, IntBuffer range, IntBuffer precision)
		{
			throw new UnsupportedOperationException("unsupported, won't implement");
		}

		public void glGetShaderiv(int shader, int pname, IntBuffer @params)
		{
			// TODO: I'm not sure why passing @params.array() won't work in this case, but this is a workaround. -RP
			var intBuffer = new int[@params.limit()];
			GL.GetShader(shader, (ShaderParameter)pname, intBuffer);
			@params.put(intBuffer);
		}

		public String glGetString(int name)
		{
			var results = GL.GetString((StringName)name);

			return results;
		}

		public void glGetTexParameterfv(int target, int pname, FloatBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetTexParameterfv(target, pname, @params);
		}

		public void glGetTexParameteriv(int target, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetTexParameteriv(target, pname, @params);
		}

		public int glGetUniformLocation(int program, String name)
		{
			var results= GL.GetUniformLocation(program, name);

			return results;
		}

		public void glGetUniformfv(int program, int location, FloatBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetUniformfv(program, location, @params);
		}

		public void glGetUniformiv(int program, int location, IntBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetUniformiv(program, location, @params);
		}

		public void glGetVertexAttribPointerv(int index, int pname, Buffer pointer)
		{
			throw new UnsupportedOperationException("unsupported, won't implement");
		}

		public void glGetVertexAttribfv(int index, int pname, FloatBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetVertexAttribfv(index, pname, @params);
		}

		public void glGetVertexAttribiv(int index, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glGetVertexAttribiv(index, pname, @params);
		}

		public void glHint(int target, int mode)
		{
			throw new NotImplementedException();
			//	GL.glHint(target, mode);
		}

		public bool glIsBuffer(int buffer)
		{
			throw new NotImplementedException();
			//return GL.glIsBuffer(buffer);
		}

		public bool glIsEnabled(int cap)
		{
			throw new NotImplementedException();
			//return GL.glIsEnabled(cap);
		}

		public bool glIsFramebuffer(int framebuffer)
		{
			throw new NotImplementedException();
			//return EXTFramebufferObject.glIsFramebufferEXT(framebuffer);
		}

		public bool glIsProgram(int program)
		{
			throw new NotImplementedException();
			//return GL.glIsProgram(program);
		}

		public bool glIsRenderbuffer(int renderbuffer)
		{
			throw new NotImplementedException();
			//return EXTFramebufferObject.glIsRenderbufferEXT(renderbuffer);
		}

		public bool glIsShader(int shader)
		{
			throw new NotImplementedException();
			//return GL.glIsShader(shader);
		}

		public bool glIsTexture(int texture)
		{
			throw new NotImplementedException();
			//return GL.glIsTexture(texture);
		}

		public void glLineWidth(float width)
		{
			throw new NotImplementedException();
			//GL.glLineWidth(width);
		}

		public void glLinkProgram(int program)
		{
			GL.LinkProgram(program);
		}

		public void glPixelStorei(int pname, int param)
		{
			GL.PixelStore((PixelStoreParameter)pname, param);
		}

		public void glPolygonOffset(float factor, float units)
		{
			GL.PolygonOffset(factor, units);
		}

		public void glReadPixels(int x, int y, int width, int height, int format, int type, Buffer pixels)
		{
			throw new NotImplementedException();
			//if (pixels is ByteBuffer)
			//	GL.glReadPixels(x, y, width, height, format, type, (ByteBuffer)pixels);
			//else if (pixels is ShortBuffer)
			//	GL.glReadPixels(x, y, width, height, format, type, (ShortBuffer)pixels);
			//else if (pixels is IntBuffer)
			//	GL.glReadPixels(x, y, width, height, format, type, (IntBuffer)pixels);
			//else if (pixels is FloatBuffer)
			//	GL.glReadPixels(x, y, width, height, format, type, (FloatBuffer)pixels);
			//else
			//	throw new GdxRuntimeException("Can't use " + pixels.GetType().Name
			//		+ " with this method. Use ByteBuffer, ShortBuffer, IntBuffer or FloatBuffer instead. Blame LWJGL");
		}

		public void glReleaseShaderCompiler()
		{
			// nothing to do here
		}

		public void glRenderbufferStorage(int target, int internalformat, int width, int height)
		{
			throw new NotImplementedException();
			//EXTFramebufferObject.glRenderbufferStorageEXT(target, internalformat, width, height);
		}

		public void glSampleCoverage(float value, bool invert)
		{
			throw new NotImplementedException();
			//GL.glSampleCoverage(value, invert);
		}

		public void glScissor(int x, int y, int width, int height)
		{
			throw new NotImplementedException();
			//GL.glScissor(x, y, width, height);
		}

		public void glShaderBinary(int n, IntBuffer shaders, int binaryformat, Buffer binary, int length)
		{
			throw new UnsupportedOperationException("unsupported, won't implement");
		}

		public void glShaderSource(int shader, String @string)
		{
			// TODO: Verify
			var length = @string.Length;
			//GL.glShaderSource(shader, 1, new string[] { @string }, ref length);
			GL.ShaderSource(shader,  @string); //, new string[] { @string }, new int[@string.Length]);
			
		}

		public void glStencilFunc(int func, int @ref, int mask)
		{
			throw new NotImplementedException();
			//	GL.glStencilFunc(func, @ref, mask);
		}

		public void glStencilFuncSeparate(int face, int func, int @ref, int mask)
		{
			throw new NotImplementedException();
			//GL.glStencilFuncSeparate(face, func, @ref, mask);
		}

		public void glStencilMask(int mask)
		{
			throw new NotImplementedException();
			//	GL.glStencilMask(mask);
		}

		public void glStencilMaskSeparate(int face, int mask)
		{
			throw new NotImplementedException();
			//GL20.glStencilMaskSeparate(face, mask);
		}

		public void glStencilOp(int fail, int zfail, int zpass)
		{
			throw new NotImplementedException();
			//GL.glStencilOp(fail, zfail, zpass);
		}

		public void glStencilOpSeparate(int face, int fail, int zfail, int zpass)
		{
			throw new NotImplementedException();
			//	GL.glStencilOpSeparate(face, fail, zfail, zpass);
		}

		public void glTexImage2D<T>
		(
			int target,
			int level,
			int internalFormat,
			int width, int height,
			int border,
			int format,
			int type,
			T[] pixels
		)
			where T : struct
		{
			GL.TexImage2D
			(
				(TextureTarget)target,
				level,
				(PixelInternalFormat)internalFormat,
				width,
				height,
				border,
				(PixelFormat)format,
				(PixelType)type, pixels
			);
		}

		public void glTexImage2D(int target, int level, int internalformat, int width, int height, int border,
			int format, int type,
			Buffer pixels)
		{
			GCHandle pixelHandle;

			if (pixels == null)
			{
				GL.TexImage2D((TextureTarget)target, level, (PixelInternalFormat)internalformat, width, height, border, (PixelFormat)format, (PixelType)type,
					(pixelHandle = GCHandle.Alloc(((ByteBuffer?)null)?.array(), GCHandleType.Pinned))
					.AddrOfPinnedObject());
			}
			else if (pixels is ByteBuffer)
			{
				GL.TexImage2D((TextureTarget)target, level, (PixelInternalFormat)internalformat, width, height, border, (PixelFormat)format, (PixelType)type,
					(pixelHandle = GCHandle.Alloc(((ByteBuffer)pixels).array(), GCHandleType.Pinned))
					.AddrOfPinnedObject());
			}
			else if (pixels is ShortBuffer)
			{
				GL.TexImage2D((TextureTarget)target, level, (PixelInternalFormat)internalformat, width, height, border, (PixelFormat)format, (PixelType)type,
					(pixelHandle = GCHandle.Alloc(((ShortBuffer)pixels).array(), GCHandleType.Pinned))
					.AddrOfPinnedObject());
			}
			else if (pixels is IntBuffer)
			{
				GL.TexImage2D((TextureTarget)target, level, (PixelInternalFormat)internalformat, width, height, border, (PixelFormat)format, (PixelType)type,
					(pixelHandle = GCHandle.Alloc(((IntBuffer)pixels).array(), GCHandleType.Pinned))
					.AddrOfPinnedObject());
			}
			else if (pixels is FloatBuffer)
			{
				GL.TexImage2D((TextureTarget)target, level, (PixelInternalFormat)internalformat, width, height, border, (PixelFormat)format, (PixelType)type,
					(pixelHandle = GCHandle.Alloc(((FloatBuffer)pixels).array(), GCHandleType.Pinned))
					.AddrOfPinnedObject());
			}
			else if (pixels is DoubleBuffer)
			{
				GL.TexImage2D((TextureTarget)target, level, (PixelInternalFormat)internalformat, width, height, border, (PixelFormat)format, (PixelType)type,
					(pixelHandle = GCHandle.Alloc(((DoubleBuffer)pixels).array(), GCHandleType.Pinned))
					.AddrOfPinnedObject());
			}
			else
			{
				throw new GdxRuntimeException("Can't use " + pixels.GetType().Name
				                                           + " with this method. Use ByteBuffer, ShortBuffer, IntBuffer, FloatBuffer or DoubleBuffer instead. Blame LWJGL");
			}

			pixelHandle.Free();
		}

		public void glTexParameterf(int target, int pname, float param)
		{
			var t = (TextureTarget)target;
			var p = (TextureParameterName)pname;
			var s = GL.GetFloat(GetPName.MaxTextureMaxAnisotropy);
			GL.TexParameter((TextureTarget)target, (TextureParameterName)pname, param);
		}

		public void glTexParameterfv(int target, int pname, FloatBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glTexParameterfv(target, pname, @params);
		}

		public void glTexParameteri(int target, int pname, int param)
		{
			GL.TexParameter((TextureTarget)target, (TextureParameterName)pname, param);
		}

		public void glTexParameteriv(int target, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
			//GL.glTexParameteriv(target, pname, @params);
		}

		public void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format,
			int type,
			Buffer pixels)
		{
			throw new NotImplementedException();
			//if (pixels is ByteBuffer)
			//	GL.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, (ByteBuffer)pixels);
			//else if (pixels is ShortBuffer)
			//	GL.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, (ShortBuffer)pixels);
			//else if (pixels is IntBuffer)
			//	GL.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, (IntBuffer)pixels);
			//else if (pixels is FloatBuffer)
			//	GL.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, (FloatBuffer)pixels);
			//else if (pixels is DoubleBuffer)
			//	GL.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, (DoubleBuffer)pixels);
			//else
			//	throw new GdxRuntimeException("Can't use " + pixels.GetType().Name
			//		+ " with this method. Use ByteBuffer, ShortBuffer, IntBuffer, FloatBuffer or DoubleBuffer instead. Blame LWJGL");
		}

		public void glUniform1f(int location, float x)
		{
			throw new NotImplementedException();
			//	GL.glUniform1f(location, x);
		}

		public void glUniform1fv(int location, int count, FloatBuffer v)
		{
			throw new NotImplementedException();
			//GL.glUniform1fv(location, v);
		}

		public void glUniform1fv(int location, int count, float[] v, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniform1fv(location, toFloatBuffer(v, offset, count));
		}

		public void glUniform1i(int location, int x)
		{
			GL.Uniform1(location, x);
		}

		public void glUniform1iv(int location, int count, IntBuffer v)
		{
			throw new NotImplementedException();
			//GL.glUniform1iv(location, v);
		}

		public void glUniform1iv(int location, int count, int[] v, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniform1iv(location, toIntBuffer(v, offset, count));
		}

		public void glUniform2f(int location, float x, float y)
		{
			throw new NotImplementedException();
			//GL.glUniform2f(location, x, y);
		}

		public void glUniform2fv(int location, int count, FloatBuffer v)
		{
			throw new NotImplementedException();
			//GL.glUniform2fv(location, v);
		}

		public void glUniform2fv(int location, int count, float[] v, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniform2fv(location, toFloatBuffer(v, offset, count << 1));
		}

		public void glUniform2i(int location, int x, int y)
		{
			throw new NotImplementedException();
			//GL.glUniform2i(location, x, y);
		}

		public void glUniform2iv(int location, int count, IntBuffer v)
		{
			throw new NotImplementedException();
			//	GL.glUniform2iv(location, v);
		}

		public void glUniform2iv(int location, int count, int[] v, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniform2iv(location, toIntBuffer(v, offset, count << 1));
		}

		public void glUniform3f(int location, float x, float y, float z)
		{
			throw new NotImplementedException();
			//GL.glUniform3f(location, x, y, z);
		}

		public void glUniform3fv(int location, int count, FloatBuffer v)
		{
			throw new NotImplementedException();
			//GL20.glUniform3fv(location, v);
		}

		public void glUniform3fv(int location, int count, float[] v, int offset)
		{
			throw new NotImplementedException();
			//GL20.glUniform3fv(location, toFloatBuffer(v, offset, count * 3));
		}

		public void glUniform3i(int location, int x, int y, int z)
		{
			throw new NotImplementedException();
			//	GL20.glUniform3i(location, x, y, z);
		}

		public void glUniform3iv(int location, int count, IntBuffer v)
		{
			throw new NotImplementedException();
			//GL20.glUniform3iv(location, v);
		}

		public void glUniform3iv(int location, int count, int[] v, int offset)
		{
			throw new NotImplementedException();
			//GL20.glUniform3iv(location, toIntBuffer(v, offset, count * 3));
		}

		public void glUniform4f(int location, float x, float y, float z, float w)
		{
			throw new NotImplementedException();
			//GL20.glUniform4f(location, x, y, z, w);
		}

		public void glUniform4fv(int location, int count, FloatBuffer v)
		{
			throw new NotImplementedException();
			//GL20.glUniform4fv(location, v);
		}

		public void glUniform4fv(int location, int count, float[] v, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniform4fv(location, toFloatBuffer(v, offset, count << 2));
		}

		public void glUniform4i(int location, int x, int y, int z, int w)
		{
			throw new NotImplementedException();
			//GL.glUniform4i(location, x, y, z, w);
		}

		public void glUniform4iv(int location, int count, IntBuffer v)
		{
			throw new NotImplementedException();
			//GL.glUniform4iv(location, v);
		}

		public void glUniform4iv(int location, int count, int[] v, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniform4iv(location, toIntBuffer(v, offset, count << 2));
		}

		public void glUniformMatrix2fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
			//GL.glUniformMatrix2fv(location, transpose, value);
		}

		public void glUniformMatrix2fv(int location, int count, bool transpose, float[] value, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniformMatrix2fv(location, transpose, toFloatBuffer(value, offset, count << 2));
		}

		public void glUniformMatrix3fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
			//GL.glUniformMatrix3fv(location, transpose, value);
		}

		public void glUniformMatrix3fv(int location, int count, bool transpose, float[] value, int offset)
		{
			throw new NotImplementedException();
			//GL.glUniformMatrix3fv(location, transpose, toFloatBuffer(value, offset, count * 9));
		}

		public void glUniformMatrix4fv(int location, int count, bool transpose, FloatBuffer value)
		{
			// TODO: If it should be writing something back into the buffers array, it needs to be updated.
			GL.UniformMatrix4(location, count, transpose, value.array());
		}

		public void glUniformMatrix4fv(int location, int count, bool transpose, float[] value, int offset)
		{
			// TODO: Verify
			GL.UniformMatrix4(location, count, transpose, toFloatBuffer(value, offset, count << 4).array());
		}

		public void glUseProgram(int program)
		{
			GL.UseProgram(program);
		}

		public void glValidateProgram(int program)
		{
			throw new NotImplementedException();
			//GL.glValidateProgram(program);
		}

		public void glVertexAttrib1f(int indx, float x)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib1f(indx, x);
		}

		public void glVertexAttrib1fv(int indx, FloatBuffer values)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib1f(indx, values.get());
		}

		public void glVertexAttrib2f(int indx, float x, float y)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib2f(indx, x, y);
		}

		public void glVertexAttrib2fv(int indx, FloatBuffer values)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib2f(indx, values.get(), values.get());
		}

		public void glVertexAttrib3f(int indx, float x, float y, float z)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib3f(indx, x, y, z);
		}

		public void glVertexAttrib3fv(int indx, FloatBuffer values)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib3f(indx, values.get(), values.get(), values.get());
		}

		public void glVertexAttrib4f(int indx, float x, float y, float z, float w)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib4f(indx, x, y, z, w);
		}

		public void glVertexAttrib4fv(int indx, FloatBuffer values)
		{
			throw new NotImplementedException();
			//GL.glVertexAttrib4f(indx, values.get(), values.get(), values.get(), values.get());
		}

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, Buffer buffer)
		{
			// TODO: I don't think this method works at all
			if (buffer is ByteBuffer)
			{
				if (type == GL20.GL_BYTE)
				{
					var array = new byte[buffer.limit()];
					Array.Copy(((ByteBuffer)buffer).array(), buffer.position(), array, 0, array.Length);

					var s = string.Join(", ", array);
					//GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ((ByteBuffer)buffer).array());
					GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, array);
				}
				else if (type == GL20.GL_UNSIGNED_BYTE)
				{
					var array = new byte[buffer.limit()];
					Array.Copy(((ByteBuffer)buffer).array(), buffer.position(), array, 0, array.Length);
					var s = string.Join(", ", array);
					//GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ((ByteBuffer)buffer).array());
					GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, array);
				}
				else if (type == GL20.GL_SHORT)
					GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ((ByteBuffer)buffer).asShortBuffer().array());
				else if (type == GL20.GL_UNSIGNED_SHORT)
					GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ((ByteBuffer)buffer).asShortBuffer().array());
				else if (type == GL20.GL_FLOAT)
				{
					var array = new float[buffer.limit()];
					Array.Copy(((ByteBuffer)buffer).asFloatBuffer().array(), buffer.position(), array, 0, array.Length);
					var s = string.Join(", ", array);
					//GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ((ByteBuffer)buffer).asFloatBuffer().array());
					GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, array);
				}
				else
					throw new GdxRuntimeException("Can't use " + buffer.GetType().Name + " with type " + type
						+ " with this method. Use ByteBuffer and one of GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT or GL_FLOAT for type. Blame LWJGL");


			}
			else if (buffer is FloatBuffer)
			{
				if (type == GL20.GL_FLOAT)
				{
					var array = new float[buffer.limit()];
					Array.Copy(((FloatBuffer)buffer).array(), buffer.position(), array, 0, array.Length);
					var s = string.Join(", ", array);
					GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, array);
					//GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ((FloatBuffer)buffer).array());
				}
				else
					throw new GdxRuntimeException(
						"Can't use " + buffer.GetType().Name + " with type " + type + " with this method.");
			}
			else
				throw new GdxRuntimeException(
					"Can't use " + buffer.GetType().Name + " with this method. Use ByteBuffer instead. Blame LWJGL");
		}

		public void glViewport(int x, int y, int width, int height)
		{
			GL.Viewport(x, y, width, height);
		}

		public void glDrawElements(int mode, int count, int type, int indices)
		{
			GL.DrawElements((PrimitiveType)mode, count, (DrawElementsType)type, indices);
		}

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, int ptr)
		{
			GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ptr);
		}

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, byte[] ptr)
		{
			GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ptr);
		}

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, float[] ptr)
		{
			GL.VertexAttribPointer(indx, size, (VertexAttribPointerType)type, normalized, stride, ptr);
		}
	}
}