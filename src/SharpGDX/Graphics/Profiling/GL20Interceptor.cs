using System;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.Profiling;

/** @author Daniel Holderbaum
 * @author Jan Polák */
public class GL20Interceptor : GLInterceptor, GL20
{

	protected internal readonly GL20 gl20;

	internal protected GL20Interceptor(GLProfiler glProfiler, GL20 gl20)
	: base(glProfiler)
	{
		this.gl20 = gl20;
	}

	private void check()
	{
		int error = gl20.glGetError();
		while (error != GL20.GL_NO_ERROR)
		{
			glProfiler.getListener().onError(error);
			error = gl20.glGetError();
		}
	}

	
	public override void glActiveTexture(int texture)
	{
		calls++;
		gl20.glActiveTexture(texture);
		check();
	}

		public override void glBindTexture(int target, int texture)
	{
		textureBindings++;
		calls++;
		gl20.glBindTexture(target, texture);
		check();
	}

		public override void glBlendFunc(int sfactor, int dfactor)
	{
		calls++;
		gl20.glBlendFunc(sfactor, dfactor);
		check();
	}

		public override void glClear(int mask)
	{
		calls++;
		gl20.glClear(mask);
		check();
	}

		public override void glClearColor(float red, float green, float blue, float alpha)
	{
		calls++;
		gl20.glClearColor(red, green, blue, alpha);
		check();
	}

		public override void glClearDepthf(float depth)
	{
		calls++;
		gl20.glClearDepthf(depth);
		check();
	}

		public override void glClearStencil(int s)
	{
		calls++;
		gl20.glClearStencil(s);
		check();
	}

		public override void glColorMask(bool red, bool green, bool blue, bool alpha)
	{
		calls++;
		gl20.glColorMask(red, green, blue, alpha);
		check();
	}

		public override void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border,
		int imageSize, Buffer data)
	{
		calls++;
		gl20.glCompressedTexImage2D(target, level, internalformat, width, height, border, imageSize, data);
		check();
	}

		public override void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height,
		int format,
		int imageSize, Buffer data)
	{
		calls++;
		gl20.glCompressedTexSubImage2D(target, level, xoffset, yoffset, width, height, format, imageSize, data);
		check();
	}

	public override void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height,
		int border)
	{
		calls++;
		gl20.glCopyTexImage2D(target, level, internalformat, x, y, width, height, border);
		check();
	}

	public override void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width,
		int height)
	{
		calls++;
		gl20.glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);
		check();
	}

		public override void glCullFace(int mode)
	{
		calls++;
		gl20.glCullFace(mode);
		check();
	}

		public override void glDeleteTextures(int n, IntBuffer textures)
	{
		calls++;
		gl20.glDeleteTextures(n, textures);
		check();
	}

		public override void glDeleteTexture(int texture)
	{
		calls++;
		gl20.glDeleteTexture(texture);
		check();
	}

		public override void glDepthFunc(int func)
	{
		calls++;
		gl20.glDepthFunc(func);
		check();
	}

		public override void glDepthMask(bool flag)
	{
		calls++;
		gl20.glDepthMask(flag);
		check();
	}

		public override void glDepthRangef(float zNear, float zFar)
	{
		calls++;
		gl20.glDepthRangef(zNear, zFar);
		check();
	}

		public override void glDisable(int cap)
	{
		calls++;
		gl20.glDisable(cap);
		check();
	}

		public override void glDrawArrays(int mode, int first, int count)
	{
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl20.glDrawArrays(mode, first, count);
		check();
	}

		public override void glDrawElements(int mode, int count, int type, Buffer indices)
	{
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl20.glDrawElements(mode, count, type, indices);
		check();
	}

		public override void glEnable(int cap)
	{
		calls++;
		gl20.glEnable(cap);
		check();
	}

		public override void glFinish()
	{
		calls++;
		gl20.glFinish();
		check();
	}

		public override void glFlush()
	{
		calls++;
		gl20.glFlush();
		check();
	}

		public override void glFrontFace(int mode)
	{
		calls++;
		gl20.glFrontFace(mode);
		check();
	}

		public override void glGenTextures(int n, IntBuffer textures)
	{
		calls++;
		gl20.glGenTextures(n, textures);
		check();
	}

		public override int glGenTexture()
	{
		calls++;
		int result = gl20.glGenTexture();
		check();
		return result;
	}

		public override int glGetError()
	{
		calls++;
		// Errors by glGetError are undetectable
		return gl20.glGetError();
	}

		public override void glGetIntegerv(int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetIntegerv(pname,  @params);
		check();
	}

		public override String glGetString(int name)
	{
		calls++;
		String result = gl20.glGetString(name);
		check();
		return result;
	}

		public override void glHint(int target, int mode)
	{
		calls++;
		gl20.glHint(target, mode);
		check();
	}

		public override void glLineWidth(float width)
	{
		calls++;
		gl20.glLineWidth(width);
		check();
	}

		public override void glPixelStorei(int pname, int param)
	{
		calls++;
		gl20.glPixelStorei(pname, param);
		check();
	}

		public override void glPolygonOffset(float factor, float units)
	{
		calls++;
		gl20.glPolygonOffset(factor, units);
		check();
	}

		public override void glReadPixels(int x, int y, int width, int height, int format, int type, Buffer pixels)
	{
		calls++;
		gl20.glReadPixels(x, y, width, height, format, type, pixels);
		check();
	}

		public override void glScissor(int x, int y, int width, int height)
	{
		calls++;
		gl20.glScissor(x, y, width, height);
		check();
	}

	public override void glStencilFunc(int func, int @ref, int mask) {
		calls++;
		gl20.glStencilFunc(func, @ref, mask);
		check();
	}

		public override void glStencilMask(int mask)
	{
		calls++;
		gl20.glStencilMask(mask);
		check();
	}

		public override void glStencilOp(int fail, int zfail, int zpass)
	{
		calls++;
		gl20.glStencilOp(fail, zfail, zpass);
		check();
	}

		public override void glTexImage2D(int target, int level, int internalformat, int width, int height, int border, int format,
		int type,
		Buffer pixels)
	{
		calls++;
		gl20.glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
		check();
	}

	public override void glTexImage2D<T>(int target, int level, int internalFormat, int width, int height, int border, int format, int type,
		T[] pixels)
	{
		calls++;
		gl20.glTexImage2D<T>(target, level, internalFormat, width, height, border, format, type, pixels);
		check();
	}

	public override void glTexParameterf(int target, int pname, float param)
	{
		calls++;
		gl20.glTexParameterf(target, pname, param);
		check();
	}

		public override void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format,
		int type,
		Buffer pixels)
	{
		calls++;
		gl20.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
		check();
	}

		public override void glViewport(int x, int y, int width, int height)
	{
		calls++;
		gl20.glViewport(x, y, width, height);
		check();
	}

		public override void glAttachShader(int program, int shader)
	{
		calls++;
		gl20.glAttachShader(program, shader);
		check();
	}

		public override void glBindAttribLocation(int program, int index, String name)
	{
		calls++;
		gl20.glBindAttribLocation(program, index, name);
		check();
	}

		public override void glBindBuffer(int target, int buffer)
	{
		calls++;
		gl20.glBindBuffer(target, buffer);
		check();
	}

		public override void glBindFramebuffer(int target, int framebuffer)
	{
		calls++;
		gl20.glBindFramebuffer(target, framebuffer);
		check();
	}

		public override void glBindRenderbuffer(int target, int renderbuffer)
	{
		calls++;
		gl20.glBindRenderbuffer(target, renderbuffer);
		check();
	}

		public override void glBlendColor(float red, float green, float blue, float alpha)
	{
		calls++;
		gl20.glBlendColor(red, green, blue, alpha);
		check();
	}

		public override void glBlendEquation(int mode)
	{
		calls++;
		gl20.glBlendEquation(mode);
		check();
	}

		public override void glBlendEquationSeparate(int modeRGB, int modeAlpha)
	{
		calls++;
		gl20.glBlendEquationSeparate(modeRGB, modeAlpha);
		check();
	}

		public override void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha)
	{
		calls++;
		gl20.glBlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
		check();
	}

		public override void glBufferData(int target, int size, Buffer data, int usage)
	{
		calls++;
		gl20.glBufferData(target, size, data, usage);
		check();
	}

		public override void glBufferSubData(int target, int offset, int size, Buffer data)
	{
		calls++;
		gl20.glBufferSubData(target, offset, size, data);
		check();
	}

		public override int glCheckFramebufferStatus(int target)
	{
		calls++;
		int result = gl20.glCheckFramebufferStatus(target);
		check();
		return result;
	}

		public override void glCompileShader(int shader)
	{
		calls++;
		gl20.glCompileShader(shader);
		check();
	}

		public override int glCreateProgram()
	{
		calls++;
		int result = gl20.glCreateProgram();
		check();
		return result;
	}

		public override int glCreateShader(int type)
	{
		calls++;
		int result = gl20.glCreateShader(type);
		check();
		return result;
	}

		public override void glDeleteBuffer(int buffer)
	{
		calls++;
		gl20.glDeleteBuffer(buffer);
		check();
	}

		public override void glDeleteBuffers(int n, IntBuffer buffers)
	{
		calls++;
		gl20.glDeleteBuffers(n, buffers);
		check();
	}

		public override void glDeleteFramebuffer(int framebuffer)
	{
		calls++;
		gl20.glDeleteFramebuffer(framebuffer);
		check();
	}

		public override void glDeleteFramebuffers(int n, IntBuffer framebuffers)
	{
		calls++;
		gl20.glDeleteFramebuffers(n, framebuffers);
		check();
	}

		public override void glDeleteProgram(int program)
	{
		calls++;
		gl20.glDeleteProgram(program);
		check();
	}

		public override void glDeleteRenderbuffer(int renderbuffer)
	{
		calls++;
		gl20.glDeleteRenderbuffer(renderbuffer);
		check();
	}

		public override void glDeleteRenderbuffers(int n, IntBuffer renderbuffers)
	{
		calls++;
		gl20.glDeleteRenderbuffers(n, renderbuffers);
		check();
	}

		public override void glDeleteShader(int shader)
	{
		calls++;
		gl20.glDeleteShader(shader);
		check();
	}

		public override void glDetachShader(int program, int shader)
	{
		calls++;
		gl20.glDetachShader(program, shader);
		check();
	}

		public override void glDisableVertexAttribArray(int index)
	{
		calls++;
		gl20.glDisableVertexAttribArray(index);
		check();
	}

		public override void glDrawElements(int mode, int count, int type, int indices)
	{
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl20.glDrawElements(mode, count, type, indices);
		check();
	}

		public override void glEnableVertexAttribArray(int index)
	{
		calls++;
		gl20.glEnableVertexAttribArray(index);
		check();
	}

		public override void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer)
	{
		calls++;
		gl20.glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
		check();
	}

		public override void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level)
	{
		calls++;
		gl20.glFramebufferTexture2D(target, attachment, textarget, texture, level);
		check();
	}

		public override int glGenBuffer()
	{
		calls++;
		int result = gl20.glGenBuffer();
		check();
		return result;
	}

		public override void glGenBuffers(int n, IntBuffer buffers)
	{
		calls++;
		gl20.glGenBuffers(n, buffers);
		check();
	}

		public override void glGenerateMipmap(int target)
	{
		calls++;
		gl20.glGenerateMipmap(target);
		check();
	}

		public override int glGenFramebuffer()
	{
		calls++;
		int result = gl20.glGenFramebuffer();
		check();
		return result;
	}

		public override void glGenFramebuffers(int n, IntBuffer framebuffers)
	{
		calls++;
		gl20.glGenFramebuffers(n, framebuffers);
		check();
	}

		public override int glGenRenderbuffer()
	{
		calls++;
		int result = gl20.glGenRenderbuffer();
		check();
		return result;
	}

		public override void glGenRenderbuffers(int n, IntBuffer renderbuffers)
	{
		calls++;
		gl20.glGenRenderbuffers(n, renderbuffers);
		check();
	}

		public override String glGetActiveAttrib(int program, int index, IntBuffer size, IntBuffer type)
	{
		calls++;
		String result = gl20.glGetActiveAttrib(program, index, size, type);
		check();
		return result;
	}

		public override String glGetActiveUniform(int program, int index, IntBuffer size, IntBuffer type)
	{
		calls++;
		String result = gl20.glGetActiveUniform(program, index, size, type);
		check();
		return result;
	}

		public override void glGetAttachedShaders(int program, int maxcount, Buffer count, IntBuffer shaders)
	{
		calls++;
		gl20.glGetAttachedShaders(program, maxcount, count, shaders);
		check();
	}

		public override int glGetAttribLocation(int program, String name)
	{
		calls++;
		int result = gl20.glGetAttribLocation(program, name);
		check();
		return result;
	}

		public override void glGetBooleanv(int pname, Buffer @params)
	{
		calls++;
		gl20.glGetBooleanv(pname,  @params);
		check();
	}

		public override void glGetBufferParameteriv(int target, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetBufferParameteriv(target, pname,  @params);
		check();
	}

		public override void glGetFloatv(int pname, FloatBuffer @params)
	{
		calls++;
		gl20.glGetFloatv(pname,  @params);
		check();
	}

		public override void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetFramebufferAttachmentParameteriv(target, attachment, pname,  @params);
		check();
	}

		public override void glGetProgramiv(int program, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetProgramiv(program, pname,  @params);
		check();
	}

		public override String glGetProgramInfoLog(int program)
	{
		calls++;
		String result = gl20.glGetProgramInfoLog(program);
		check();
		return result;
	}

		public override void glGetRenderbufferParameteriv(int target, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetRenderbufferParameteriv(target, pname,  @params);
		check();
	}

		public override void glGetShaderiv(int shader, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetShaderiv(shader, pname,  @params);
		check();
	}

		public override String glGetShaderInfoLog(int shader)
	{
		calls++;
		String result = gl20.glGetShaderInfoLog(shader);
		check();
		return result;
	}

		public override void glGetShaderPrecisionFormat(int shadertype, int precisiontype, IntBuffer range, IntBuffer precision)
	{
		calls++;
		gl20.glGetShaderPrecisionFormat(shadertype, precisiontype, range, precision);
		check();
	}

		public override void glGetTexParameterfv(int target, int pname, FloatBuffer @params)
	{
		calls++;
		gl20.glGetTexParameterfv(target, pname,  @params);
		check();
	}

		public override void glGetTexParameteriv(int target, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetTexParameteriv(target, pname,  @params);
		check();
	}

		public override void glGetUniformfv(int program, int location, FloatBuffer @params)
	{
		calls++;
		gl20.glGetUniformfv(program, location,  @params);
		check();
	}

		public override void glGetUniformiv(int program, int location, IntBuffer @params)
	{
		calls++;
		gl20.glGetUniformiv(program, location,  @params);
		check();
	}

		public override int glGetUniformLocation(int program, String name)
	{
		calls++;
		int result = gl20.glGetUniformLocation(program, name);
		check();
		return result;
	}

		public override void glGetVertexAttribfv(int index, int pname, FloatBuffer @params)
	{
		calls++;
		gl20.glGetVertexAttribfv(index, pname,  @params);
		check();
	}

		public override void glGetVertexAttribiv(int index, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glGetVertexAttribiv(index, pname,  @params);
		check();
	}

		public override void glGetVertexAttribPointerv(int index, int pname, Buffer pointer)
	{
		calls++;
		gl20.glGetVertexAttribPointerv(index, pname, pointer);
		check();
	}

		public override bool glIsBuffer(int buffer)
	{
		calls++;
		bool result = gl20.glIsBuffer(buffer);
		check();
		return result;
	}

		public override bool glIsEnabled(int cap)
	{
		calls++;
		bool result = gl20.glIsEnabled(cap);
		check();
		return result;
	}

		public override bool glIsFramebuffer(int framebuffer)
	{
		calls++;
		bool result = gl20.glIsFramebuffer(framebuffer);
		check();
		return result;
	}

		public override bool glIsProgram(int program)
	{
		calls++;
		bool result = gl20.glIsProgram(program);
		check();
		return result;
	}

		public override bool glIsRenderbuffer(int renderbuffer)
	{
		calls++;
		bool result = gl20.glIsRenderbuffer(renderbuffer);
		check();
		return result;
	}

		public override bool glIsShader(int shader)
	{
		calls++;
		bool result = gl20.glIsShader(shader);
		check();
		return result;
	}

		public override bool glIsTexture(int texture)
	{
		calls++;
		bool result = gl20.glIsTexture(texture);
		check();
		return result;
	}

		public override void glLinkProgram(int program)
	{
		calls++;
		gl20.glLinkProgram(program);
		check();
	}

		public override void glReleaseShaderCompiler()
	{
		calls++;
		gl20.glReleaseShaderCompiler();
		check();
	}

		public override void glRenderbufferStorage(int target, int internalformat, int width, int height)
	{
		calls++;
		gl20.glRenderbufferStorage(target, internalformat, width, height);
		check();
	}

		public override void glSampleCoverage(float value, bool invert)
	{
		calls++;
		gl20.glSampleCoverage(value, invert);
		check();
	}

		public override void glShaderBinary(int n, IntBuffer shaders, int binaryformat, Buffer binary, int length)
	{
		calls++;
		gl20.glShaderBinary(n, shaders, binaryformat, binary, length);
		check();
	}

		public override void glShaderSource(int shader, String @string)
	{
		calls++;
		gl20.glShaderSource(shader, @string);
		check();
	}

	public override void glStencilFuncSeparate(int face, int func, int @ref, int mask) {
		calls++;
		gl20.glStencilFuncSeparate(face, func, @ref, mask);
		check();
	}

		public override void glStencilMaskSeparate(int face, int mask)
	{
		calls++;
		gl20.glStencilMaskSeparate(face, mask);
		check();
	}

		public override void glStencilOpSeparate(int face, int fail, int zfail, int zpass)
	{
		calls++;
		gl20.glStencilOpSeparate(face, fail, zfail, zpass);
		check();
	}

		public override void glTexParameterfv(int target, int pname, FloatBuffer @params)
	{
		calls++;
		gl20.glTexParameterfv(target, pname,  @params);
		check();
	}

		public override void glTexParameteri(int target, int pname, int param)
	{
		calls++;
		gl20.glTexParameteri(target, pname, param);
		check();
	}

		public override void glTexParameteriv(int target, int pname, IntBuffer @params)
	{
		calls++;
		gl20.glTexParameteriv(target, pname,  @params);
		check();
	}

		public override void glUniform1f(int location, float x)
	{
		calls++;
		gl20.glUniform1f(location, x);
		check();
	}

		public override void glUniform1fv(int location, int count, FloatBuffer v)
	{
		calls++;
		gl20.glUniform1fv(location, count, v);
		check();
	}

		public override void glUniform1fv(int location, int count, float[] v, int offset)
	{
		calls++;
		gl20.glUniform1fv(location, count, v, offset);
		check();
	}

		public override void glUniform1i(int location, int x)
	{
		calls++;
		gl20.glUniform1i(location, x);
		check();
	}

		public override void glUniform1iv(int location, int count, IntBuffer v)
	{
		calls++;
		gl20.glUniform1iv(location, count, v);
		check();
	}

		public override void glUniform1iv(int location, int count, int[] v, int offset)
	{
		calls++;
		gl20.glUniform1iv(location, count, v, offset);
		check();
	}

		public override void glUniform2f(int location, float x, float y)
	{
		calls++;
		gl20.glUniform2f(location, x, y);
		check();
	}

		public override void glUniform2fv(int location, int count, FloatBuffer v)
	{
		calls++;
		gl20.glUniform2fv(location, count, v);
		check();
	}

		public override void glUniform2fv(int location, int count, float[] v, int offset)
	{
		calls++;
		gl20.glUniform2fv(location, count, v, offset);
		check();
	}

		public override void glUniform2i(int location, int x, int y)
	{
		calls++;
		gl20.glUniform2i(location, x, y);
		check();
	}

		public override void glUniform2iv(int location, int count, IntBuffer v)
	{
		calls++;
		gl20.glUniform2iv(location, count, v);
		check();
	}

		public override void glUniform2iv(int location, int count, int[] v, int offset)
	{
		calls++;
		gl20.glUniform2iv(location, count, v, offset);
		check();
	}

		public override void glUniform3f(int location, float x, float y, float z)
	{
		calls++;
		gl20.glUniform3f(location, x, y, z);
		check();
	}

		public override void glUniform3fv(int location, int count, FloatBuffer v)
	{
		calls++;
		gl20.glUniform3fv(location, count, v);
		check();
	}

		public override void glUniform3fv(int location, int count, float[] v, int offset)
	{
		calls++;
		gl20.glUniform3fv(location, count, v, offset);
		check();
	}

		public override void glUniform3i(int location, int x, int y, int z)
	{
		calls++;
		gl20.glUniform3i(location, x, y, z);
		check();
	}

		public override void glUniform3iv(int location, int count, IntBuffer v)
	{
		calls++;
		gl20.glUniform3iv(location, count, v);
		check();
	}

		public override void glUniform3iv(int location, int count, int[] v, int offset)
	{
		calls++;
		gl20.glUniform3iv(location, count, v, offset);
		check();
	}

		public override void glUniform4f(int location, float x, float y, float z, float w)
	{
		calls++;
		gl20.glUniform4f(location, x, y, z, w);
		check();
	}

		public override void glUniform4fv(int location, int count, FloatBuffer v)
	{
		calls++;
		gl20.glUniform4fv(location, count, v);
		check();
	}

		public override void glUniform4fv(int location, int count, float[] v, int offset)
	{
		calls++;
		gl20.glUniform4fv(location, count, v, offset);
		check();
	}

		public override void glUniform4i(int location, int x, int y, int z, int w)
	{
		calls++;
		gl20.glUniform4i(location, x, y, z, w);
		check();
	}

		public override void glUniform4iv(int location, int count, IntBuffer v)
	{
		calls++;
		gl20.glUniform4iv(location, count, v);
		check();
	}

		public override void glUniform4iv(int location, int count, int[] v, int offset)
	{
		calls++;
		gl20.glUniform4iv(location, count, v, offset);
		check();
	}

		public override void glUniformMatrix2fv(int location, int count, bool transpose, FloatBuffer value)
	{
		calls++;
		gl20.glUniformMatrix2fv(location, count, transpose, value);
		check();
	}

		public override void glUniformMatrix2fv(int location, int count, bool transpose, float[] value, int offset)
	{
		calls++;
		gl20.glUniformMatrix2fv(location, count, transpose, value, offset);
		check();
	}

		public override void glUniformMatrix3fv(int location, int count, bool transpose, FloatBuffer value)
	{
		calls++;
		gl20.glUniformMatrix3fv(location, count, transpose, value);
		check();
	}

		public override void glUniformMatrix3fv(int location, int count, bool transpose, float[] value, int offset)
	{
		calls++;
		gl20.glUniformMatrix3fv(location, count, transpose, value, offset);
		check();
	}

		public override void glUniformMatrix4fv(int location, int count, bool transpose, FloatBuffer value)
	{
		calls++;
		gl20.glUniformMatrix4fv(location, count, transpose, value);
		check();
	}

		public override void glUniformMatrix4fv(int location, int count, bool transpose, float[] value, int offset)
	{
		calls++;
		gl20.glUniformMatrix4fv(location, count, transpose, value, offset);
		check();
	}

		public override void glUseProgram(int program)
	{
		shaderSwitches++;
		calls++;
		gl20.glUseProgram(program);
		check();
	}

		public override void glValidateProgram(int program)
	{
		calls++;
		gl20.glValidateProgram(program);
		check();
	}

		public override void glVertexAttrib1f(int indx, float x)
	{
		calls++;
		gl20.glVertexAttrib1f(indx, x);
		check();
	}

		public override void glVertexAttrib1fv(int indx, FloatBuffer values)
	{
		calls++;
		gl20.glVertexAttrib1fv(indx, values);
		check();
	}

		public override void glVertexAttrib2f(int indx, float x, float y)
	{
		calls++;
		gl20.glVertexAttrib2f(indx, x, y);
		check();
	}

		public override void glVertexAttrib2fv(int indx, FloatBuffer values)
	{
		calls++;
		gl20.glVertexAttrib2fv(indx, values);
		check();
	}

		public override void glVertexAttrib3f(int indx, float x, float y, float z)
	{
		calls++;
		gl20.glVertexAttrib3f(indx, x, y, z);
		check();
	}

		public override void glVertexAttrib3fv(int indx, FloatBuffer values)
	{
		calls++;
		gl20.glVertexAttrib3fv(indx, values);
		check();
	}

		public override void glVertexAttrib4f(int indx, float x, float y, float z, float w)
	{
		calls++;
		gl20.glVertexAttrib4f(indx, x, y, z, w);
		check();
	}

		public override void glVertexAttrib4fv(int indx, FloatBuffer values)
	{
		calls++;
		gl20.glVertexAttrib4fv(indx, values);
		check();
	}

		public override void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, Buffer ptr)
	{
		calls++;
		gl20.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

		public override void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, int ptr)
	{
		calls++;
		gl20.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

	public override void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride,
		byte[] ptr)
	{
		calls++;
		gl20.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

	public override void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride,
		float[] ptr)
	{
		calls++;
		gl20.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}
}