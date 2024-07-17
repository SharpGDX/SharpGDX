using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Mathematics;
using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.Profiling
{
	public abstract class GLInterceptor : GL20 {

	protected int calls;
	protected int textureBindings;
	protected int drawCalls;
	protected int shaderSwitches;
	protected readonly FloatCounter vertexCount = new FloatCounter(0);

	protected GLProfiler glProfiler;

	protected GLInterceptor (GLProfiler profiler) {
		this.glProfiler = profiler;
	}

	public static String resolveErrorNumber (int error) {
		switch (error) {
		case GL20.GL_INVALID_VALUE:
			return "GL_INVALID_VALUE";
		case GL20.GL_INVALID_OPERATION:
			return "GL_INVALID_OPERATION";
		case GL20.GL_INVALID_FRAMEBUFFER_OPERATION:
			return "GL_INVALID_FRAMEBUFFER_OPERATION";
		case GL20.GL_INVALID_ENUM:
			return "GL_INVALID_ENUM";
		case GL20.GL_OUT_OF_MEMORY:
			return "GL_OUT_OF_MEMORY";
		default:
			return "number " + error;
		}
	}

	public int getCalls () {
		return calls;
	}

	public int getTextureBindings () {
		return textureBindings;
	}

	public int getDrawCalls () {
		return drawCalls;
	}

	public int getShaderSwitches () {
		return shaderSwitches;
	}

	public FloatCounter getVertexCount () {
		return vertexCount;
	}

	public void reset () {
		calls = 0;
		textureBindings = 0;
		drawCalls = 0;
		shaderSwitches = 0;
		vertexCount.reset();
	}

	public abstract void glActiveTexture(int texture);
	public abstract void glBindTexture(int target, int texture);
	public abstract void glBlendFunc(int sfactor, int dfactor);
	public abstract void glClear(int mask);
	public abstract void glClearColor(float red, float green, float blue, float alpha);
	public abstract void glClearDepthf(float depth);
	public abstract void glClearStencil(int s);
	public abstract void glColorMask(bool red, bool green, bool blue, bool alpha);

	public abstract void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border, int imageSize,
		Buffer data);

	public abstract void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format,
		int imageSize, Buffer data);

	public abstract void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border);
	public abstract void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height);
	public abstract void glCullFace(int mode);
	public abstract void glDeleteTextures(int n, IntBuffer textures);
	public abstract void glDeleteTexture(int texture);
	public abstract void glDepthFunc(int func);
	public abstract void glDepthMask(bool flag);
	public abstract void glDepthRangef(float zNear, float zFar);
	public abstract void glDisable(int cap);
	public abstract void glDrawArrays(int mode, int first, int count);
	public abstract void glDrawElements(int mode, int count, int type, Buffer indices);
	public abstract void glEnable(int cap);
	public abstract void glFinish();
	public abstract void glFlush();
	public abstract void glFrontFace(int mode);
	public abstract void glGenTextures(int n, IntBuffer textures);
	public abstract int glGenTexture();
	public abstract int glGetError();
	public abstract void glGetIntegerv(int pname, IntBuffer @params);
	public abstract string glGetString(int name);
	public abstract void glHint(int target, int mode);
	public abstract void glLineWidth(float width);
	public abstract void glPixelStorei(int pname, int param);
	public abstract void glPolygonOffset(float factor, float units);
	public abstract void glReadPixels(int x, int y, int width, int height, int format, int type, Buffer pixels);
	public abstract void glScissor(int x, int y, int width, int height);
	public abstract void glStencilFunc(int func, int @ref, int mask);
	public abstract void glStencilMask(int mask);
	public abstract void glStencilOp(int fail, int zfail, int zpass);

	public abstract void glTexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type,
		Buffer pixels);

	public abstract void glTexImage2D<T>(int target, int level, int internalFormat, int width, int height, int border, int format, int type,
		T[] pixels) where T : struct;

	public abstract void glTexParameterf(int target, int pname, float param);

	public abstract void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type,
		Buffer pixels);

	public abstract void glViewport(int x, int y, int width, int height);
	public abstract void glAttachShader(int program, int shader);
	public abstract void glBindAttribLocation(int program, int index, string name);
	public abstract void glBindBuffer(int target, int buffer);
	public abstract void glBindFramebuffer(int target, int framebuffer);
	public abstract void glBindRenderbuffer(int target, int renderbuffer);
	public abstract void glBlendColor(float red, float green, float blue, float alpha);
	public abstract void glBlendEquation(int mode);
	public abstract void glBlendEquationSeparate(int modeRGB, int modeAlpha);
	public abstract void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);
	public abstract void glBufferData(int target, int size, Buffer data, int usage);
	public abstract void glBufferSubData(int target, int offset, int size, Buffer data);
	public abstract int glCheckFramebufferStatus(int target);
	public abstract void glCompileShader(int shader);
	public abstract int glCreateProgram();
	public abstract int glCreateShader(int type);
	public abstract void glDeleteBuffer(int buffer);
	public abstract void glDeleteBuffers(int n, IntBuffer buffers);
	public abstract void glDeleteFramebuffer(int framebuffer);
	public abstract void glDeleteFramebuffers(int n, IntBuffer framebuffers);
	public abstract void glDeleteProgram(int program);
	public abstract void glDeleteRenderbuffer(int renderbuffer);
	public abstract void glDeleteRenderbuffers(int n, IntBuffer renderbuffers);
	public abstract void glDeleteShader(int shader);
	public abstract void glDetachShader(int program, int shader);
	public abstract void glDisableVertexAttribArray(int index);
	public abstract void glDrawElements(int mode, int count, int type, int indices);
	public abstract void glEnableVertexAttribArray(int index);
	public abstract void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer);
	public abstract void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level);
	public abstract int glGenBuffer();
	public abstract void glGenBuffers(int n, IntBuffer buffers);
	public abstract void glGenerateMipmap(int target);
	public abstract int glGenFramebuffer();
	public abstract void glGenFramebuffers(int n, IntBuffer framebuffers);
	public abstract int glGenRenderbuffer();
	public abstract void glGenRenderbuffers(int n, IntBuffer renderbuffers);
	public abstract string glGetActiveAttrib(int program, int index, IntBuffer size, IntBuffer type);
	public abstract string glGetActiveUniform(int program, int index, IntBuffer size, IntBuffer type);
	public abstract void glGetAttachedShaders(int program, int maxcount, Buffer count, IntBuffer shaders);
	public abstract int glGetAttribLocation(int program, string name);
	public abstract void glGetBooleanv(int pname, Buffer @params);
	public abstract void glGetBufferParameteriv(int target, int pname, IntBuffer @params);
	public abstract void glGetFloatv(int pname, FloatBuffer @params);
	public abstract void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, IntBuffer @params);
	public abstract void glGetProgramiv(int program, int pname, IntBuffer @params);
	public abstract string glGetProgramInfoLog(int program);
	public abstract void glGetRenderbufferParameteriv(int target, int pname, IntBuffer @params);
	public abstract void glGetShaderiv(int shader, int pname, IntBuffer @params);
	public abstract string glGetShaderInfoLog(int shader);
	public abstract void glGetShaderPrecisionFormat(int shadertype, int precisiontype, IntBuffer range, IntBuffer precision);
	public abstract void glGetTexParameterfv(int target, int pname, FloatBuffer @params);
	public abstract void glGetTexParameteriv(int target, int pname, IntBuffer @params);
	public abstract void glGetUniformfv(int program, int location, FloatBuffer @params);
	public abstract void glGetUniformiv(int program, int location, IntBuffer @params);
	public abstract int glGetUniformLocation(int program, string name);
	public abstract void glGetVertexAttribfv(int index, int pname, FloatBuffer @params);
	public abstract void glGetVertexAttribiv(int index, int pname, IntBuffer @params);
	public abstract void glGetVertexAttribPointerv(int index, int pname, Buffer pointer);
	public abstract bool glIsBuffer(int buffer);
	public abstract bool glIsEnabled(int cap);
	public abstract bool glIsFramebuffer(int framebuffer);
	public abstract bool glIsProgram(int program);
	public abstract bool glIsRenderbuffer(int renderbuffer);
	public abstract bool glIsShader(int shader);
	public abstract bool glIsTexture(int texture);
	public abstract void glLinkProgram(int program);
	public abstract void glReleaseShaderCompiler();
	public abstract void glRenderbufferStorage(int target, int internalformat, int width, int height);
	public abstract void glSampleCoverage(float value, bool invert);
	public abstract void glShaderBinary(int n, IntBuffer shaders, int binaryformat, Buffer binary, int length);
	public abstract void glShaderSource(int shader, string @string);
	public abstract void glStencilFuncSeparate(int face, int func, int @ref, int mask);
	public abstract void glStencilMaskSeparate(int face, int mask);
	public abstract void glStencilOpSeparate(int face, int fail, int zfail, int zpass);
	public abstract void glTexParameterfv(int target, int pname, FloatBuffer @params);
	public abstract void glTexParameteri(int target, int pname, int param);
	public abstract void glTexParameteriv(int target, int pname, IntBuffer @params);
	public abstract void glUniform1f(int location, float x);
	public abstract void glUniform1fv(int location, int count, FloatBuffer v);
	public abstract void glUniform1fv(int location, int count, float[] v, int offset);
	public abstract void glUniform1i(int location, int x);
	public abstract void glUniform1iv(int location, int count, IntBuffer v);
	public abstract void glUniform1iv(int location, int count, int[] v, int offset);
	public abstract void glUniform2f(int location, float x, float y);
	public abstract void glUniform2fv(int location, int count, FloatBuffer v);
	public abstract void glUniform2fv(int location, int count, float[] v, int offset);
	public abstract void glUniform2i(int location, int x, int y);
	public abstract void glUniform2iv(int location, int count, IntBuffer v);
	public abstract void glUniform2iv(int location, int count, int[] v, int offset);
	public abstract void glUniform3f(int location, float x, float y, float z);
	public abstract void glUniform3fv(int location, int count, FloatBuffer v);
	public abstract void glUniform3fv(int location, int count, float[] v, int offset);
	public abstract void glUniform3i(int location, int x, int y, int z);
	public abstract void glUniform3iv(int location, int count, IntBuffer v);
	public abstract void glUniform3iv(int location, int count, int[] v, int offset);
	public abstract void glUniform4f(int location, float x, float y, float z, float w);
	public abstract void glUniform4fv(int location, int count, FloatBuffer v);
	public abstract void glUniform4fv(int location, int count, float[] v, int offset);
	public abstract void glUniform4i(int location, int x, int y, int z, int w);
	public abstract void glUniform4iv(int location, int count, IntBuffer v);
	public abstract void glUniform4iv(int location, int count, int[] v, int offset);
	public abstract void glUniformMatrix2fv(int location, int count, bool transpose, FloatBuffer value);
	public abstract void glUniformMatrix2fv(int location, int count, bool transpose, float[] value, int offset);
	public abstract void glUniformMatrix3fv(int location, int count, bool transpose, FloatBuffer value);
	public abstract void glUniformMatrix3fv(int location, int count, bool transpose, float[] value, int offset);
	public abstract void glUniformMatrix4fv(int location, int count, bool transpose, FloatBuffer value);
	public abstract void glUniformMatrix4fv(int location, int count, bool transpose, float[] value, int offset);
	public abstract void glUseProgram(int program);
	public abstract void glValidateProgram(int program);
	public abstract void glVertexAttrib1f(int indx, float x);
	public abstract void glVertexAttrib1fv(int indx, FloatBuffer values);
	public abstract void glVertexAttrib2f(int indx, float x, float y);
	public abstract void glVertexAttrib2fv(int indx, FloatBuffer values);
	public abstract void glVertexAttrib3f(int indx, float x, float y, float z);
	public abstract void glVertexAttrib3fv(int indx, FloatBuffer values);
	public abstract void glVertexAttrib4f(int indx, float x, float y, float z, float w);
	public abstract void glVertexAttrib4fv(int indx, FloatBuffer values);
	public abstract void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, Buffer ptr);
	public abstract void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, int ptr);
	public abstract void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, byte[] ptr);

	public abstract void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, float[] ptr);
	}
}
