using System;
using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.Profiling;

/** @author Daniel Holderbaum
 * @author Jan Polák */
public class GL30Interceptor : GLInterceptor , GL30 {

	internal protected readonly GL30 gl30;

	internal protected GL30Interceptor (GLProfiler glProfiler, GL30 gl30) 
	: base(glProfiler)
	{
		
		this.gl30 = gl30;
	}

	public override void glTexImage2D<T>(int target, int level, int internalFormat, int width, int height, int border, int format, int type,
		T[] pixels)
	{
		calls++;
		gl30.glTexImage2D<T>(target, level, internalFormat, width, height, border, format, type, pixels);
		check();
	}

	private void check () {
		int error = gl30.glGetError();
		while (error != GL20.GL_NO_ERROR) {
			glProfiler.getListener().onError(error);
			error = gl30.glGetError();
		}
	}

		public override void glActiveTexture (int texture) {
		calls++;
		gl30.glActiveTexture(texture);
		check();
	}

	public override void glBindTexture (int target, int texture) {
		textureBindings++;
		calls++;
		gl30.glBindTexture(target, texture);
		check();
	}

	public override void glBlendFunc (int sfactor, int dfactor) {
		calls++;
		gl30.glBlendFunc(sfactor, dfactor);
		check();
	}

	public override void glClear (int mask) {
		calls++;
		gl30.glClear(mask);
		check();
	}

	public override void glClearColor (float red, float green, float blue, float alpha) {
		calls++;
		gl30.glClearColor(red, green, blue, alpha);
		check();
	}

	public override void glClearDepthf (float depth) {
		calls++;
		gl30.glClearDepthf(depth);
		check();
	}

	public override void glClearStencil (int s) {
		calls++;
		gl30.glClearStencil(s);
		check();
	}

	public override void glColorMask (bool red, bool green, bool blue, bool alpha) {
		calls++;
		gl30.glColorMask(red, green, blue, alpha);
		check();
	}

	public override void glCompressedTexImage2D (int target, int level, int internalformat, int width, int height, int border,
		int imageSize, Buffer data) {
		calls++;
		gl30.glCompressedTexImage2D(target, level, internalformat, width, height, border, imageSize, data);
		check();
	}

	public override void glCompressedTexSubImage2D (int target, int level, int xoffset, int yoffset, int width, int height, int format,
		int imageSize, Buffer data) {
		calls++;
		gl30.glCompressedTexSubImage2D(target, level, xoffset, yoffset, width, height, format, imageSize, data);
		check();
	}

	public override void glCopyTexImage2D (int target, int level, int internalformat, int x, int y, int width, int height, int border) {
		calls++;
		gl30.glCopyTexImage2D(target, level, internalformat, x, y, width, height, border);
		check();
	}

	public override void glCopyTexSubImage2D (int target, int level, int xoffset, int yoffset, int x, int y, int width, int height) {
		calls++;
		gl30.glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);
		check();
	}

	public override void glCullFace (int mode) {
		calls++;
		gl30.glCullFace(mode);
		check();
	}

	public override void glDeleteTextures (int n, IntBuffer textures) {
		calls++;
		gl30.glDeleteTextures(n, textures);
		check();
	}

	public override void glDeleteTexture (int texture) {
		calls++;
		gl30.glDeleteTexture(texture);
		check();
	}

	public override void glDepthFunc (int func) {
		calls++;
		gl30.glDepthFunc(func);
		check();
	}

	public override void glDepthMask (bool flag) {
		calls++;
		gl30.glDepthMask(flag);
		check();
	}

	public override void glDepthRangef (float zNear, float zFar) {
		calls++;
		gl30.glDepthRangef(zNear, zFar);
		check();
	}

	public override void glDisable (int cap) {
		calls++;
		gl30.glDisable(cap);
		check();
	}

	public override void glDrawArrays (int mode, int first, int count) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawArrays(mode, first, count);
		check();
	}

	public override void glDrawElements (int mode, int count, int type, Buffer indices) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawElements(mode, count, type, indices);
		check();
	}

	public override void glEnable (int cap) {
		calls++;
		gl30.glEnable(cap);
		check();
	}

	public override void glFinish () {
		calls++;
		gl30.glFinish();
		check();
	}

	public override void glFlush () {
		calls++;
		gl30.glFlush();
		check();
	}

	public override void glFrontFace (int mode) {
		calls++;
		gl30.glFrontFace(mode);
		check();
	}

	public override void glGenTextures (int n, IntBuffer textures) {
		calls++;
		gl30.glGenTextures(n, textures);
		check();
	}

	public override int glGenTexture () {
		calls++;
		int result = gl30.glGenTexture();
		check();
		return result;
	}

	public override int glGetError () {
		calls++;
		// Errors by glGetError are undetectable
		return gl30.glGetError();
	}

	public override void glGetIntegerv (int pname, IntBuffer @params) {
		calls++;
		gl30.glGetIntegerv(pname, @params);
		check();
	}

	public override String glGetString (int name) {
		calls++;
		String result = gl30.glGetString(name);
		check();
		return result;
	}

	public override void glHint (int target, int mode) {
		calls++;
		gl30.glHint(target, mode);
		check();
	}

	public override void glLineWidth (float width) {
		calls++;
		gl30.glLineWidth(width);
		check();
	}

	public override void glPixelStorei (int pname, int param) {
		calls++;
		gl30.glPixelStorei(pname, param);
		check();
	}

	public override void glPolygonOffset (float factor, float units) {
		calls++;
		gl30.glPolygonOffset(factor, units);
		check();
	}

	public override void glReadPixels (int x, int y, int width, int height, int format, int type, Buffer pixels) {
		calls++;
		gl30.glReadPixels(x, y, width, height, format, type, pixels);
		check();
	}

	public override void glScissor (int x, int y, int width, int height) {
		calls++;
		gl30.glScissor(x, y, width, height);
		check();
	}

	public override void glStencilFunc (int func, int @ref, int mask) {
		calls++;
		gl30.glStencilFunc(func, @ref, mask);
		check();
	}

	public override void glStencilMask (int mask) {
		calls++;
		gl30.glStencilMask(mask);
		check();
	}

	public override void glStencilOp (int fail, int zfail, int zpass) {
		calls++;
		gl30.glStencilOp(fail, zfail, zpass);
		check();
	}

	public override void glTexImage2D (int target, int level, int internalformat, int width, int height, int border, int format, int type,
		Buffer pixels) {
		calls++;
		gl30.glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
		check();
	}
	
	public void glTexImage2D (int target, int level, int internalformat, int width, int height, int border, int format, int type,
		int offset) {
		calls++;
		gl30.glTexImage2D(target, level, internalformat, width, height, border, format, type, offset);
		check();
	}

	public override void glTexParameterf (int target, int pname, float param) {
		calls++;
		gl30.glTexParameterf(target, pname, param);
		check();
	}

	public override void glTexSubImage2D (int target, int level, int xoffset, int yoffset, int width, int height, int format, int type,
		Buffer pixels) {
		calls++;
		gl30.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
		check();
	}

		public void glTexSubImage2D (int target, int level, int xoffset, int yoffset, int width, int height, int format, int type,
		int offset) {
		calls++;
		gl30.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, offset);
		check();
	}

	public override void glViewport (int x, int y, int width, int height) {
		calls++;
		gl30.glViewport(x, y, width, height);
		check();
	}

	public override void glAttachShader (int program, int shader) {
		calls++;
		gl30.glAttachShader(program, shader);
		check();
	}

	public override void glBindAttribLocation (int program, int index, String name) {
		calls++;
		gl30.glBindAttribLocation(program, index, name);
		check();
	}

	public override void glBindBuffer (int target, int buffer) {
		calls++;
		gl30.glBindBuffer(target, buffer);
		check();
	}

	public override void glBindFramebuffer (int target, int framebuffer) {
		calls++;
		gl30.glBindFramebuffer(target, framebuffer);
		check();
	}

	public override void glBindRenderbuffer (int target, int renderbuffer) {
		calls++;
		gl30.glBindRenderbuffer(target, renderbuffer);
		check();
	}

	public override void glBlendColor (float red, float green, float blue, float alpha) {
		calls++;
		gl30.glBlendColor(red, green, blue, alpha);
		check();
	}

	public override void glBlendEquation (int mode) {
		calls++;
		gl30.glBlendEquation(mode);
		check();
	}

	public override void glBlendEquationSeparate (int modeRGB, int modeAlpha) {
		calls++;
		gl30.glBlendEquationSeparate(modeRGB, modeAlpha);
		check();
	}

	public override void glBlendFuncSeparate (int srcRGB, int dstRGB, int srcAlpha, int dstAlpha) {
		calls++;
		gl30.glBlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
		check();
	}

	public override void glBufferData (int target, int size, Buffer data, int usage) {
		calls++;
		gl30.glBufferData(target, size, data, usage);
		check();
	}

	public override void glBufferSubData (int target, int offset, int size, Buffer data) {
		calls++;
		gl30.glBufferSubData(target, offset, size, data);
		check();
	}

	public override int glCheckFramebufferStatus (int target) {
		calls++;
		int result = gl30.glCheckFramebufferStatus(target);
		check();
		return result;
	}

	public override void glCompileShader (int shader) {
		calls++;
		gl30.glCompileShader(shader);
		check();
	}

	public override int glCreateProgram () {
		calls++;
		int result = gl30.glCreateProgram();
		check();
		return result;
	}

	public override int glCreateShader (int type) {
		calls++;
		int result = gl30.glCreateShader(type);
		check();
		return result;
	}

	public override void glDeleteBuffer (int buffer) {
		calls++;
		gl30.glDeleteBuffer(buffer);
		check();
	}

	public override void glDeleteBuffers (int n, IntBuffer buffers) {
		calls++;
		gl30.glDeleteBuffers(n, buffers);
		check();
	}

	public override void glDeleteFramebuffer (int framebuffer) {
		calls++;
		gl30.glDeleteFramebuffer(framebuffer);
		check();
	}

	public override void glDeleteFramebuffers (int n, IntBuffer framebuffers) {
		calls++;
		gl30.glDeleteFramebuffers(n, framebuffers);
		check();
	}

	public override void glDeleteProgram (int program) {
		calls++;
		gl30.glDeleteProgram(program);
		check();
	}

	public override void glDeleteRenderbuffer (int renderbuffer) {
		calls++;
		gl30.glDeleteRenderbuffer(renderbuffer);
		check();
	}

	public override void glDeleteRenderbuffers (int n, IntBuffer renderbuffers) {
		calls++;
		gl30.glDeleteRenderbuffers(n, renderbuffers);
		check();
	}

	public override void glDeleteShader (int shader) {
		calls++;
		gl30.glDeleteShader(shader);
		check();
	}

	public override void glDetachShader (int program, int shader) {
		calls++;
		gl30.glDetachShader(program, shader);
		check();
	}

	public override void glDisableVertexAttribArray (int index) {
		calls++;
		gl30.glDisableVertexAttribArray(index);
		check();
	}

	public override void glDrawElements (int mode, int count, int type, int indices) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawElements(mode, count, type, indices);
		check();
	}

	public override void glEnableVertexAttribArray (int index) {
		calls++;
		gl30.glEnableVertexAttribArray(index);
		check();
	}

	public override void glFramebufferRenderbuffer (int target, int attachment, int renderbuffertarget, int renderbuffer) {
		calls++;
		gl30.glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
		check();
	}

	public override void glFramebufferTexture2D (int target, int attachment, int textarget, int texture, int level) {
		calls++;
		gl30.glFramebufferTexture2D(target, attachment, textarget, texture, level);
		check();
	}

	public override int glGenBuffer () {
		calls++;
		int result = gl30.glGenBuffer();
		check();
		return result;
	}

	public override void glGenBuffers (int n, IntBuffer buffers) {
		calls++;
		gl30.glGenBuffers(n, buffers);
		check();
	}

	public override void glGenerateMipmap (int target) {
		calls++;
		gl30.glGenerateMipmap(target);
		check();
	}

	public override int glGenFramebuffer () {
		calls++;
		int result = gl30.glGenFramebuffer();
		check();
		return result;
	}

	public override void glGenFramebuffers (int n, IntBuffer framebuffers) {
		calls++;
		gl30.glGenFramebuffers(n, framebuffers);
		check();
	}

	public override int glGenRenderbuffer () {
		calls++;
		int result = gl30.glGenRenderbuffer();
		check();
		return result;
	}

	public override void glGenRenderbuffers (int n, IntBuffer renderbuffers) {
		calls++;
		gl30.glGenRenderbuffers(n, renderbuffers);
		check();
	}

	public override String glGetActiveAttrib (int program, int index, IntBuffer size, IntBuffer type) {
		calls++;
		String result = gl30.glGetActiveAttrib(program, index, size, type);
		check();
		return result;
	}

	public override String glGetActiveUniform (int program, int index, IntBuffer size, IntBuffer type) {
		calls++;
		String result = gl30.glGetActiveUniform(program, index, size, type);
		check();
		return result;
	}

	public override void glGetAttachedShaders (int program, int maxcount, Buffer count, IntBuffer shaders) {
		calls++;
		gl30.glGetAttachedShaders(program, maxcount, count, shaders);
		check();
	}

	public override int glGetAttribLocation (int program, String name) {
		calls++;
		int result = gl30.glGetAttribLocation(program, name);
		check();
		return result;
	}

	public override void glGetBooleanv (int pname, Buffer @params) {
		calls++;
		gl30.glGetBooleanv(pname, @params);
		check();
	}

	public override void glGetBufferParameteriv (int target, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetBufferParameteriv(target, pname, @params);
		check();
	}

	public override void glGetFloatv (int pname, FloatBuffer @params) {
		calls++;
		gl30.glGetFloatv(pname, @params);
		check();
	}

	public override void glGetFramebufferAttachmentParameteriv (int target, int attachment, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetFramebufferAttachmentParameteriv(target, attachment, pname, @params);
		check();
	}

	public override void glGetProgramiv (int program, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetProgramiv(program, pname, @params);
		check();
	}

		public override String glGetProgramInfoLog (int program) {
		calls++;
		String result = gl30.glGetProgramInfoLog(program);
		check();
		return result;
	}

		public override void glGetRenderbufferParameteriv (int target, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetRenderbufferParameteriv(target, pname, @params);
		check();
	}

		public override void glGetShaderiv (int shader, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetShaderiv(shader, pname, @params);
		check();
	}

		public override String glGetShaderInfoLog (int shader) {
		calls++;
		String result = gl30.glGetShaderInfoLog(shader);
		check();
		return result;
	}

		public override void glGetShaderPrecisionFormat (int shadertype, int precisiontype, IntBuffer range, IntBuffer precision) {
		calls++;
		gl30.glGetShaderPrecisionFormat(shadertype, precisiontype, range, precision);
		check();
	}

		public override void glGetTexParameterfv (int target, int pname, FloatBuffer @params) {
		calls++;
		gl30.glGetTexParameterfv(target, pname, @params);
		check();
	}

		public override void glGetTexParameteriv (int target, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetTexParameteriv(target, pname, @params);
		check();
	}

		public override void glGetUniformfv (int program, int location, FloatBuffer @params) {
		calls++;
		gl30.glGetUniformfv(program, location, @params);
		check();
	}

		public override void glGetUniformiv (int program, int location, IntBuffer @params) {
		calls++;
		gl30.glGetUniformiv(program, location, @params);
		check();
	}

		public override int glGetUniformLocation (int program, String name) {
		calls++;
		int result = gl30.glGetUniformLocation(program, name);
		check();
		return result;
	}

		public override void glGetVertexAttribfv (int index, int pname, FloatBuffer @params) {
		calls++;
		gl30.glGetVertexAttribfv(index, pname, @params);
		check();
	}

		public override void glGetVertexAttribiv (int index, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetVertexAttribiv(index, pname, @params);
		check();
	}

		public override void glGetVertexAttribPointerv (int index, int pname, Buffer pointer) {
		calls++;
		gl30.glGetVertexAttribPointerv(index, pname, pointer);
		check();
	}

		public override bool glIsBuffer (int buffer) {
		calls++;
		bool result = gl30.glIsBuffer(buffer);
		check();
		return result;
	}

		public override bool glIsEnabled (int cap) {
		calls++;
		bool result = gl30.glIsEnabled(cap);
		check();
		return result;
	}

		public override bool glIsFramebuffer (int framebuffer) {
		calls++;
		bool result = gl30.glIsFramebuffer(framebuffer);
		check();
		return result;
	}

		public override bool glIsProgram (int program) {
		calls++;
		bool result = gl30.glIsProgram(program);
		check();
		return result;
	}

		public override bool glIsRenderbuffer (int renderbuffer) {
		calls++;
		bool result = gl30.glIsRenderbuffer(renderbuffer);
		check();
		return result;
	}

		public override bool glIsShader (int shader) {
		calls++;
		bool result = gl30.glIsShader(shader);
		check();
		return result;
	}

		public override bool glIsTexture (int texture) {
		calls++;
		bool result = gl30.glIsTexture(texture);
		check();
		return result;
	}

		public override void glLinkProgram (int program) {
		calls++;
		gl30.glLinkProgram(program);
		check();
	}

		public override void glReleaseShaderCompiler () {
		calls++;
		gl30.glReleaseShaderCompiler();
		check();
	}

		public override void glRenderbufferStorage (int target, int internalformat, int width, int height) {
		calls++;
		gl30.glRenderbufferStorage(target, internalformat, width, height);
		check();
	}

		public override void glSampleCoverage (float value, bool invert) {
		calls++;
		gl30.glSampleCoverage(value, invert);
		check();
	}

		public override void glShaderBinary (int n, IntBuffer shaders, int binaryformat, Buffer binary, int length) {
		calls++;
		gl30.glShaderBinary(n, shaders, binaryformat, binary, length);
		check();
	}

		public override void glShaderSource (int shader, String @string) {
		calls++;
		gl30.glShaderSource(shader, @string);
		check();
	}

		public override void glStencilFuncSeparate (int face, int func, int @ref, int mask) {
		calls++;
		gl30.glStencilFuncSeparate(face, func, @ref, mask);
		check();
	}

		public override void glStencilMaskSeparate (int face, int mask) {
		calls++;
		gl30.glStencilMaskSeparate(face, mask);
		check();
	}

		public override void glStencilOpSeparate (int face, int fail, int zfail, int zpass) {
		calls++;
		gl30.glStencilOpSeparate(face, fail, zfail, zpass);
		check();
	}

		public override void glTexParameterfv (int target, int pname, FloatBuffer @params) {
		calls++;
		gl30.glTexParameterfv(target, pname, @params);
		check();
	}

		public override void glTexParameteri (int target, int pname, int param) {
		calls++;
		gl30.glTexParameteri(target, pname, param);
		check();
	}

		public override void glTexParameteriv (int target, int pname, IntBuffer @params) {
		calls++;
		gl30.glTexParameteriv(target, pname, @params);
		check();
	}

		public override void glUniform1f (int location, float x) {
		calls++;
		gl30.glUniform1f(location, x);
		check();
	}

		public override void glUniform1fv (int location, int count, FloatBuffer v) {
		calls++;
		gl30.glUniform1fv(location, count, v);
		check();
	}

		public override void glUniform1fv (int location, int count, float[] v, int offset) {
		calls++;
		gl30.glUniform1fv(location, count, v, offset);
		check();
	}

		public override void glUniform1i (int location, int x) {
		calls++;
		gl30.glUniform1i(location, x);
		check();
	}

		public override void glUniform1iv (int location, int count, IntBuffer v) {
		calls++;
		gl30.glUniform1iv(location, count, v);
		check();
	}

		public override void glUniform1iv (int location, int count, int[] v, int offset) {
		calls++;
		gl30.glUniform1iv(location, count, v, offset);
		check();
	}

		public override void glUniform2f (int location, float x, float y) {
		calls++;
		gl30.glUniform2f(location, x, y);
		check();
	}

		public override void glUniform2fv (int location, int count, FloatBuffer v) {
		calls++;
		gl30.glUniform2fv(location, count, v);
		check();
	}

		public override void glUniform2fv (int location, int count, float[] v, int offset) {
		calls++;
		gl30.glUniform2fv(location, count, v, offset);
		check();
	}

		public override void glUniform2i (int location, int x, int y) {
		calls++;
		gl30.glUniform2i(location, x, y);
		check();
	}

		public override void glUniform2iv (int location, int count, IntBuffer v) {
		calls++;
		gl30.glUniform2iv(location, count, v);
		check();
	}

		public override void glUniform2iv (int location, int count, int[] v, int offset) {
		calls++;
		gl30.glUniform2iv(location, count, v, offset);
		check();
	}

		public override void glUniform3f (int location, float x, float y, float z) {
		calls++;
		gl30.glUniform3f(location, x, y, z);
		check();
	}

		public override void glUniform3fv (int location, int count, FloatBuffer v) {
		calls++;
		gl30.glUniform3fv(location, count, v);
		check();
	}

		public override void glUniform3fv (int location, int count, float[] v, int offset) {
		calls++;
		gl30.glUniform3fv(location, count, v, offset);
		check();
	}

		public override void glUniform3i (int location, int x, int y, int z) {
		calls++;
		gl30.glUniform3i(location, x, y, z);
		check();
	}

		public override void glUniform3iv (int location, int count, IntBuffer v) {
		calls++;
		gl30.glUniform3iv(location, count, v);
		check();
	}

		public override void glUniform3iv (int location, int count, int[] v, int offset) {
		calls++;
		gl30.glUniform3iv(location, count, v, offset);
		check();
	}

		public override void glUniform4f (int location, float x, float y, float z, float w) {
		calls++;
		gl30.glUniform4f(location, x, y, z, w);
		check();
	}

		public override void glUniform4fv (int location, int count, FloatBuffer v) {
		calls++;
		gl30.glUniform4fv(location, count, v);
		check();
	}

		public override void glUniform4fv (int location, int count, float[] v, int offset) {
		calls++;
		gl30.glUniform4fv(location, count, v, offset);
		check();
	}

		public override void glUniform4i (int location, int x, int y, int z, int w) {
		calls++;
		gl30.glUniform4i(location, x, y, z, w);
		check();
	}

		public override void glUniform4iv (int location, int count, IntBuffer v) {
		calls++;
		gl30.glUniform4iv(location, count, v);
		check();
	}

		public override void glUniform4iv (int location, int count, int[] v, int offset) {
		calls++;
		gl30.glUniform4iv(location, count, v, offset);
		check();
	}

		public override void glUniformMatrix2fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix2fv(location, count, transpose, value);
		check();
	}

		public override void glUniformMatrix2fv (int location, int count, bool transpose, float[] value, int offset) {
		calls++;
		gl30.glUniformMatrix2fv(location, count, transpose, value, offset);
		check();
	}

		public override void glUniformMatrix3fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix3fv(location, count, transpose, value);
		check();
	}

		public override void glUniformMatrix3fv (int location, int count, bool transpose, float[] value, int offset) {
		calls++;
		gl30.glUniformMatrix3fv(location, count, transpose, value, offset);
		check();
	}

		public override void glUniformMatrix4fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix4fv(location, count, transpose, value);
		check();
	}

		public override void glUniformMatrix4fv (int location, int count, bool transpose, float[] value, int offset) {
		calls++;
		gl30.glUniformMatrix4fv(location, count, transpose, value, offset);
		check();
	}

		public override void glUseProgram (int program) {
		shaderSwitches++;
		calls++;
		gl30.glUseProgram(program);
		check();
	}

		public override void glValidateProgram (int program) {
		calls++;
		gl30.glValidateProgram(program);
		check();
	}

		public override void glVertexAttrib1f (int indx, float x) {
		calls++;
		gl30.glVertexAttrib1f(indx, x);
		check();
	}

		public override void glVertexAttrib1fv (int indx, FloatBuffer values) {
		calls++;
		gl30.glVertexAttrib1fv(indx, values);
		check();
	}

		public override void glVertexAttrib2f (int indx, float x, float y) {
		calls++;
		gl30.glVertexAttrib2f(indx, x, y);
		check();
	}

		public override void glVertexAttrib2fv (int indx, FloatBuffer values) {
		calls++;
		gl30.glVertexAttrib2fv(indx, values);
		check();
	}

		public override void glVertexAttrib3f (int indx, float x, float y, float z) {
		calls++;
		gl30.glVertexAttrib3f(indx, x, y, z);
		check();
	}

		public override void glVertexAttrib3fv (int indx, FloatBuffer values) {
		calls++;
		gl30.glVertexAttrib3fv(indx, values);
		check();
	}

		public override void glVertexAttrib4f (int indx, float x, float y, float z, float w) {
		calls++;
		gl30.glVertexAttrib4f(indx, x, y, z, w);
		check();
	}

		public override void glVertexAttrib4fv (int indx, FloatBuffer values) {
		calls++;
		gl30.glVertexAttrib4fv(indx, values);
		check();
	}

		public override void glVertexAttribPointer (int indx, int size, int type, bool normalized, int stride, Buffer ptr) {
		calls++;
		gl30.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

		public override void glVertexAttribPointer (int indx, int size, int type, bool normalized, int stride, int ptr) {
		calls++;
		gl30.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

	public override void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, byte[] ptr)
	{
		calls++;
		gl30.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

	public override void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, float[] ptr)
	{
		calls++;
		gl30.glVertexAttribPointer(indx, size, type, normalized, stride, ptr);
		check();
	}

	// GL30 Unique

	public void glReadBuffer (int mode) {
		calls++;
		gl30.glReadBuffer(mode);
		check();
	}

		public void glDrawRangeElements (int mode, int start, int end, int count, int type, Buffer indices) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawRangeElements(mode, start, end, count, type, indices);
		check();
	}

		public void glDrawRangeElements (int mode, int start, int end, int count, int type, int offset) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawRangeElements(mode, start, end, count, type, offset);
		check();
	}

		public void glTexImage3D (int target, int level, int internalformat, int width, int height, int depth, int border, int format,
		int type, Buffer pixels) {
		calls++;
		gl30.glTexImage3D(target, level, internalformat, width, height, depth, border, format, type, pixels);
		check();
	}

		public void glTexImage3D (int target, int level, int internalformat, int width, int height, int depth, int border, int format,
		int type, int offset) {
		calls++;
		gl30.glTexImage3D(target, level, internalformat, width, height, depth, border, format, type, offset);
		check();
	}

		public void glTexSubImage3D (int target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth,
		int format, int type, Buffer pixels) {
		calls++;
		gl30.glTexSubImage3D(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);
		check();
	}

		public void glTexSubImage3D (int target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth,
		int format, int type, int offset) {
		calls++;
		gl30.glTexSubImage3D(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, offset);
		check();
	}

		public void glCopyTexSubImage3D (int target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width,
		int height) {
		calls++;
		gl30.glCopyTexSubImage3D(target, level, xoffset, yoffset, zoffset, x, y, width, height);
		check();
	}

		public void glGenQueries (int n, int[] ids, int offset) {
		calls++;
		gl30.glGenQueries(n, ids, offset);
		check();
	}

		public void glGenQueries (int n, IntBuffer ids) {
		calls++;
		gl30.glGenQueries(n, ids);
		check();
	}

		public void glDeleteQueries (int n, int[] ids, int offset) {
		calls++;
		gl30.glDeleteQueries(n, ids, offset);
		check();
	}

		public void glDeleteQueries (int n, IntBuffer ids) {
		calls++;
		gl30.glDeleteQueries(n, ids);
		check();
	}

		public bool glIsQuery (int id) {
		calls++;
		 bool result = gl30.glIsQuery(id);
		check();
		return result;
	}

		public void glBeginQuery (int target, int id) {
		calls++;
		gl30.glBeginQuery(target, id);
		check();
	}

		public void glEndQuery (int target) {
		calls++;
		gl30.glEndQuery(target);
		check();
	}

		public void glGetQueryiv (int target, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetQueryiv(target, pname, @params);
		check();
	}

		public void glGetQueryObjectuiv (int id, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetQueryObjectuiv(id, pname, @params);
		check();
	}

		public bool glUnmapBuffer (int target) {
		calls++;
		 bool result = gl30.glUnmapBuffer(target);
		check();
		return result;
	}

		public Buffer glGetBufferPointerv (int target, int pname) {
		calls++;
		 Buffer result = gl30.glGetBufferPointerv(target, pname);
		check();
		return result;
	}

		public void glDrawBuffers (int n, IntBuffer bufs) {
		drawCalls++;
		calls++;
		gl30.glDrawBuffers(n, bufs);
		check();
	}

		public void glUniformMatrix2x3fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix2x3fv(location, count, transpose, value);
		check();
	}

		public void glUniformMatrix3x2fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix3x2fv(location, count, transpose, value);
		check();
	}

		public void glUniformMatrix2x4fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix2x4fv(location, count, transpose, value);
		check();
	}

		public void glUniformMatrix4x2fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix4x2fv(location, count, transpose, value);
		check();
	}

		public void glUniformMatrix3x4fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix3x4fv(location, count, transpose, value);
		check();
	}

		public void glUniformMatrix4x3fv (int location, int count, bool transpose, FloatBuffer value) {
		calls++;
		gl30.glUniformMatrix4x3fv(location, count, transpose, value);
		check();
	}

		public void glBlitFramebuffer (int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1,
		int mask, int filter) {
		calls++;
		gl30.glBlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
		check();
	}

		public void glRenderbufferStorageMultisample (int target, int samples, int internalformat, int width, int height) {
		calls++;
		gl30.glRenderbufferStorageMultisample(target, samples, internalformat, width, height);
		check();
	}

		public void glFramebufferTextureLayer (int target, int attachment, int texture, int level, int layer) {
		calls++;
		gl30.glFramebufferTextureLayer(target, attachment, texture, level, layer);
		check();
	}

		public Buffer glMapBufferRange (int target, int offset, int length, int access) {
		calls++;
		Buffer result = gl30.glMapBufferRange(target, offset, length, access);
		check();
		return result;
	}

		public void glFlushMappedBufferRange (int target, int offset, int length) {
		calls++;
		gl30.glFlushMappedBufferRange(target, offset, length);
		check();
	}

		public void glBindVertexArray (int array) {
		calls++;
		gl30.glBindVertexArray(array);
		check();
	}

		public void glDeleteVertexArrays (int n, int[] arrays, int offset) {
		calls++;
		gl30.glDeleteVertexArrays(n, arrays, offset);
		check();
	}

		public void glDeleteVertexArrays (int n, IntBuffer arrays) {
		calls++;
		gl30.glDeleteVertexArrays(n, arrays);
		check();
	}

		public void glGenVertexArrays (int n, int[] arrays, int offset) {
		calls++;
		gl30.glGenVertexArrays(n, arrays, offset);
		check();
	}

		public void glGenVertexArrays (int n, IntBuffer arrays) {
		calls++;
		gl30.glGenVertexArrays(n, arrays);
		check();
	}

		public bool glIsVertexArray (int array) {
		calls++;
		 bool result = gl30.glIsVertexArray(array);
		check();
		return result;
	}

		public void glBeginTransformFeedback (int primitiveMode) {
		calls++;
		gl30.glBeginTransformFeedback(primitiveMode);
		check();
	}

		public void glEndTransformFeedback () {
		calls++;
		gl30.glEndTransformFeedback();
		check();
	}

		public void glBindBufferRange (int target, int index, int buffer, int offset, int size) {
		calls++;
		gl30.glBindBufferRange(target, index, buffer, offset, size);
		check();
	}

		public void glBindBufferBase (int target, int index, int buffer) {
		calls++;
		gl30.glBindBufferBase(target, index, buffer);
		check();
	}

		public void glTransformFeedbackVaryings (int program, String[] varyings, int bufferMode) {
		calls++;
		gl30.glTransformFeedbackVaryings(program, varyings, bufferMode);
		check();
	}

		public void glVertexAttribIPointer (int index, int size, int type, int stride, int offset) {
		calls++;
		gl30.glVertexAttribIPointer(index, size, type, stride, offset);
		check();
	}

		public void glGetVertexAttribIiv (int index, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetVertexAttribIiv(index, pname, @params);
		check();
	}

		public void glGetVertexAttribIuiv (int index, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetVertexAttribIuiv(index, pname, @params);
		check();
	}

		public void glVertexAttribI4i (int index, int x, int y, int z, int w) {
		calls++;
		gl30.glVertexAttribI4i(index, x, y, z, w);
		check();
	}

		public void glVertexAttribI4ui (int index, int x, int y, int z, int w) {
		calls++;
		gl30.glVertexAttribI4ui(index, x, y, z, w);
		check();
	}

		public void glGetUniformuiv (int program, int location, IntBuffer @params) {
		calls++;
		gl30.glGetUniformuiv(program, location, @params);
		check();
	}

		public int glGetFragDataLocation (int program, String name) {
		calls++;
		 int result = gl30.glGetFragDataLocation(program, name);
		check();
		return result;
	}

		public void glUniform1uiv (int location, int count, IntBuffer value) {
		calls++;
		gl30.glUniform1uiv(location, count, value);
		check();
	}

		public void glUniform3uiv (int location, int count, IntBuffer value) {
		calls++;
		gl30.glUniform3uiv(location, count, value);
		check();
	}

		public void glUniform4uiv (int location, int count, IntBuffer value) {
		calls++;
		gl30.glUniform4uiv(location, count, value);
		check();
	}

		public void glClearBufferiv (int buffer, int drawbuffer, IntBuffer value) {
		calls++;
		gl30.glClearBufferiv(buffer, drawbuffer, value);
		check();
	}

		public void glClearBufferuiv (int buffer, int drawbuffer, IntBuffer value) {
		calls++;
		gl30.glClearBufferuiv(buffer, drawbuffer, value);
		check();
	}

		public void glClearBufferfv (int buffer, int drawbuffer, FloatBuffer value) {
		calls++;
		gl30.glClearBufferfv(buffer, drawbuffer, value);
		check();
	}

		public void glClearBufferfi (int buffer, int drawbuffer, float depth, int stencil) {
		calls++;
		gl30.glClearBufferfi(buffer, drawbuffer, depth, stencil);
		check();
	}

		public String glGetStringi (int name, int index) {
		calls++;
		 String result = gl30.glGetStringi(name, index);
		check();
		return result;
	}

		public void glCopyBufferSubData (int readTarget, int writeTarget, int readOffset, int writeOffset, int size) {
		calls++;
		gl30.glCopyBufferSubData(readTarget, writeTarget, readOffset, writeOffset, size);
		check();
	}

		public void glGetUniformIndices (int program, String[] uniformNames, IntBuffer uniformIndices) {
		calls++;
		gl30.glGetUniformIndices(program, uniformNames, uniformIndices);
		check();
	}

		public void glGetActiveUniformsiv (int program, int uniformCount, IntBuffer uniformIndices, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetActiveUniformsiv(program, uniformCount, uniformIndices, pname, @params);
		check();
	}

		public int glGetUniformBlockIndex (int program, String uniformBlockName) {
		calls++;
		 int result = gl30.glGetUniformBlockIndex(program, uniformBlockName);
		check();
		return result;
	}

		public void glGetActiveUniformBlockiv (int program, int uniformBlockIndex, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetActiveUniformBlockiv(program, uniformBlockIndex, pname, @params);
		check();
	}

		public void glGetActiveUniformBlockName (int program, int uniformBlockIndex, Buffer length, Buffer uniformBlockName) {
		calls++;
		gl30.glGetActiveUniformBlockName(program, uniformBlockIndex, length, uniformBlockName);
		check();
	}

		public String glGetActiveUniformBlockName (int program, int uniformBlockIndex) {
		calls++;
		 String result = gl30.glGetActiveUniformBlockName(program, uniformBlockIndex);
		check();
		return result;
	}

		public void glUniformBlockBinding (int program, int uniformBlockIndex, int uniformBlockBinding) {
		calls++;
		gl30.glUniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding);
		check();
	}

		public void glDrawArraysInstanced (int mode, int first, int count, int instanceCount) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawArraysInstanced(mode, first, count, instanceCount);
		check();
	}

		public void glDrawElementsInstanced (int mode, int count, int type, int indicesOffset, int instanceCount) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl30.glDrawElementsInstanced(mode, count, type, indicesOffset, instanceCount);
		check();
	}

		public void glGetInteger64v (int pname, LongBuffer @params) {
		calls++;
		gl30.glGetInteger64v(pname, @params);
		check();
	}

		public void glGetBufferParameteri64v (int target, int pname, LongBuffer @params) {
		calls++;
		gl30.glGetBufferParameteri64v(target, pname, @params);
		check();
	}

		public void glGenSamplers (int count, int[] samplers, int offset) {
		calls++;
		gl30.glGenSamplers(count, samplers, offset);
		check();
	}

		public void glGenSamplers (int count, IntBuffer samplers) {
		calls++;
		gl30.glGenSamplers(count, samplers);
		check();
	}

		public void glDeleteSamplers (int count, int[] samplers, int offset) {
		calls++;
		gl30.glDeleteSamplers(count, samplers, offset);
		check();
	}

		public void glDeleteSamplers (int count, IntBuffer samplers) {
		calls++;
		gl30.glDeleteSamplers(count, samplers);
		check();
	}

		public bool glIsSampler (int sampler) {
		calls++;
		 bool result = gl30.glIsSampler(sampler);
		check();
		return result;
	}

		public void glBindSampler (int unit, int sampler) {
		calls++;
		gl30.glBindSampler(unit, sampler);
		check();
	}

		public void glSamplerParameteri (int sampler, int pname, int param) {
		calls++;
		gl30.glSamplerParameteri(sampler, pname, param);
		check();
	}

		public void glSamplerParameteriv (int sampler, int pname, IntBuffer param) {
		calls++;
		gl30.glSamplerParameteriv(sampler, pname, param);
		check();
	}

		public void glSamplerParameterf (int sampler, int pname, float param) {
		calls++;
		gl30.glSamplerParameterf(sampler, pname, param);
		check();
	}

		public void glSamplerParameterfv (int sampler, int pname, FloatBuffer param) {
		calls++;
		gl30.glSamplerParameterfv(sampler, pname, param);
		check();
	}

		public void glGetSamplerParameteriv (int sampler, int pname, IntBuffer @params) {
		calls++;
		gl30.glGetSamplerParameteriv(sampler, pname, @params);
		check();
	}

		public void glGetSamplerParameterfv (int sampler, int pname, FloatBuffer @params) {
		calls++;
		gl30.glGetSamplerParameterfv(sampler, pname, @params);
		check();
	}

		public void glVertexAttribDivisor (int index, int divisor) {
		calls++;
		gl30.glVertexAttribDivisor(index, divisor);
		check();
	}

		public void glBindTransformFeedback (int target, int id) {
		calls++;
		gl30.glBindTransformFeedback(target, id);
		check();
	}

		public void glDeleteTransformFeedbacks (int n, int[] ids, int offset) {
		calls++;
		gl30.glDeleteTransformFeedbacks(n, ids, offset);
		check();
	}

		public void glDeleteTransformFeedbacks (int n, IntBuffer ids) {
		calls++;
		gl30.glDeleteTransformFeedbacks(n, ids);
		check();
	}

		public void glGenTransformFeedbacks (int n, int[] ids, int offset) {
		calls++;
		gl30.glGenTransformFeedbacks(n, ids, offset);
		check();
	}

		public void glGenTransformFeedbacks (int n, IntBuffer ids) {
		calls++;
		gl30.glGenTransformFeedbacks(n, ids);
		check();
	}

		public bool glIsTransformFeedback (int id) {
		calls++;
		 bool result = gl30.glIsTransformFeedback(id);
		check();
		return result;
	}

		public void glPauseTransformFeedback () {
		calls++;
		gl30.glPauseTransformFeedback();
		check();
	}

		public void glResumeTransformFeedback () {
		calls++;
		gl30.glResumeTransformFeedback();
		check();
	}

		public void glProgramParameteri (int program, int pname, int value) {
		calls++;
		gl30.glProgramParameteri(program, pname, value);
		check();
	}

		public void glInvalidateFramebuffer (int target, int numAttachments, IntBuffer attachments) {
		calls++;
		gl30.glInvalidateFramebuffer(target, numAttachments, attachments);
		check();
	}

		public void glInvalidateSubFramebuffer (int target, int numAttachments, IntBuffer attachments, int x, int y, int width,
		int height) {
		calls++;
		gl30.glInvalidateSubFramebuffer(target, numAttachments, attachments, x, y, width, height);
		check();
	}
}
