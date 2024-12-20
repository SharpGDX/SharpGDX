using System;
using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.Profiling;

public class GL32Interceptor : GL31Interceptor , IGL32 {

	internal readonly IGL32 gl32;

	public GL32Interceptor (GLProfiler glProfiler, IGL32 gl32) 
	: base(glProfiler, gl32)
	{
		
		this.gl32 = gl32;
	}

	public void glBlendBarrier () {
		calls++;
		gl32.glBlendBarrier();
		check();
	}

	public void glCopyImageSubData (int srcName, int srcTarget, int srcLevel, int srcX, int srcY, int srcZ, int dstName,
		int dstTarget, int dstLevel, int dstX, int dstY, int dstZ, int srcWidth, int srcHeight, int srcDepth) {
		calls++;
		gl32.glCopyImageSubData(srcName, srcTarget, srcLevel, srcX, srcY, srcZ, dstName, dstTarget, dstLevel, dstX, dstY, dstZ,
			srcWidth, srcHeight, srcDepth);
		check();
	}

	public void glDebugMessageControl (int source, int type, int severity, IntBuffer ids, bool enabled) {
		calls++;
		gl32.glDebugMessageControl(source, type, severity, ids, enabled);
		check();
	}

	public void glDebugMessageInsert (int source, int type, int id, int severity, String buf) {
		calls++;
		gl32.glDebugMessageInsert(source, type, id, severity, buf);
		check();
	}

	public void glDebugMessageCallback (IGL32.DebugProc callsback) {
		calls++;
		gl32.glDebugMessageCallback(callsback);
		check();
		check();
	}

	public int glGetDebugMessageLog (int count, IntBuffer sources, IntBuffer types, IntBuffer ids, IntBuffer severities,
		IntBuffer lengths, ByteBuffer messageLog) {
		calls++;
		int v = gl32.glGetDebugMessageLog(count, sources, types, ids, severities, lengths, messageLog);
		check();
		return v;
	}

	public void glPushDebugGroup (int source, int id, String message) {
		calls++;
		gl32.glPushDebugGroup(source, id, message);
		check();
	}

	public void glPopDebugGroup () {
		calls++;
		gl32.glPopDebugGroup();
		check();
	}

	public void glObjectLabel (int identifier, int name, String label) {
		calls++;
		gl32.glObjectLabel(identifier, name, label);
		check();
	}

	public String glGetObjectLabel (int identifier, int name) {
		calls++;
		String v = gl32.glGetObjectLabel(identifier, name);
		check();
		return v;
	}

	public long glGetPointerv (int pname) {
		calls++;
		long v = gl32.glGetPointerv(pname);
		check();
		return v;
	}

	public void glEnablei (int target, int index) {
		calls++;
		gl32.glEnablei(target, index);
		check();
	}

	public void glDisablei (int target, int index) {
		calls++;
		gl32.glDisablei(target, index);
		check();
	}

	public void glBlendEquationi (int buf, int mode) {
		calls++;
		gl32.glBlendEquationi(buf, mode);
		check();
	}

	public void glBlendEquationSeparatei (int buf, int modeRGB, int modeAlpha) {
		calls++;
		gl32.glBlendEquationSeparatei(buf, modeRGB, modeAlpha);
		check();
	}

	public void glBlendFunci (int buf, int src, int dst) {
		calls++;
		gl32.glBlendFunci(buf, src, dst);
		check();
	}

	public void glBlendFuncSeparatei (int buf, int srcRGB, int dstRGB, int srcAlpha, int dstAlpha) {
		calls++;
		gl32.glBlendFuncSeparatei(buf, srcRGB, dstRGB, srcAlpha, dstAlpha);
		check();
	}

	public void glColorMaski (int index, bool r, bool g, bool b, bool a) {
		calls++;
		gl32.glColorMaski(index, r, g, b, a);
		check();
	}

	public bool glIsEnabledi (int target, int index) {
		calls++;
		bool v = gl32.glIsEnabledi(target, index);
		check();
		return v;
	}

	public void glDrawElementsBaseVertex (int mode, int count, int type, Buffer indices, int basevertex) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl32.glDrawElementsBaseVertex(mode, count, type, indices, basevertex);
		check();
	}

	public void glDrawRangeElementsBaseVertex (int mode, int start, int end, int count, int type, Buffer indices, int basevertex) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl32.glDrawRangeElementsBaseVertex(mode, start, end, count, type, indices, basevertex);
		check();
	}

	public void glDrawElementsInstancedBaseVertex (int mode, int count, int type, Buffer indices, int instanceCount,
		int basevertex) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl32.glDrawElementsInstancedBaseVertex(mode, count, type, indices, instanceCount, basevertex);
		check();
	}

	public void glDrawElementsInstancedBaseVertex (int mode, int count, int type, int indicesOffset, int instanceCount,
		int basevertex) {
		vertexCount.put(count);
		drawCalls++;
		calls++;
		gl32.glDrawElementsInstancedBaseVertex(mode, count, type, indicesOffset, instanceCount, basevertex);
		check();
	}

	public void glFramebufferTexture (int target, int attachment, int texture, int level) {
		calls++;
		gl32.glFramebufferTexture(target, attachment, texture, level);
		check();
	}

	public int glGetGraphicsResetStatus () {
		calls++;
		int v = gl32.glGetGraphicsResetStatus();
		check();
		return v;
	}

	public void glReadnPixels (int x, int y, int width, int height, int format, int type, int bufSize, Buffer data) {
		calls++;
		gl32.glReadnPixels(x, y, width, height, format, type, bufSize, data);
		check();
	}

	public void glGetnUniformfv (int program, int location, FloatBuffer @params) {
		calls++;
		gl32.glGetnUniformfv(program, location, @params);
		check();
	}

	public void glGetnUniformiv (int program, int location, IntBuffer @params) {
		calls++;
		gl32.glGetnUniformiv(program, location, @params);
		check();
	}

	public void glGetnUniformuiv (int program, int location, IntBuffer @params) {
		calls++;
		gl32.glGetnUniformuiv(program, location, @params);
		check();
	}

	public void glMinSampleShading (float value) {
		calls++;
		gl32.glMinSampleShading(value);
		check();
	}

	public void glPatchParameteri (int pname, int value) {
		calls++;
		gl32.glPatchParameteri(pname, value);
		check();
	}

	public void glTexParameterIiv (int target, int pname, IntBuffer @params) {
		calls++;
		gl32.glTexParameterIiv(target, pname, @params);
		check();
	}

	public void glTexParameterIuiv (int target, int pname, IntBuffer @params) {
		calls++;
		gl32.glTexParameterIuiv(target, pname, @params);
		check();
	}

	public void glGetTexParameterIiv (int target, int pname, IntBuffer @params) {
		calls++;
		gl32.glGetTexParameterIiv(target, pname, @params);
		check();
	}

	public void glGetTexParameterIuiv (int target, int pname, IntBuffer @params) {
		calls++;
		gl32.glGetTexParameterIuiv(target, pname, @params);
		check();
	}

	public void glSamplerParameterIiv (int sampler, int pname, IntBuffer param) {
		calls++;
		gl32.glSamplerParameterIiv(sampler, pname, param);
		check();
	}

	public void glSamplerParameterIuiv (int sampler, int pname, IntBuffer param) {
		calls++;
		gl32.glSamplerParameterIuiv(sampler, pname, param);
		check();
	}

	public void glGetSamplerParameterIiv (int sampler, int pname, IntBuffer @params) {
		calls++;
		gl32.glGetSamplerParameterIiv(sampler, pname, @params);
		check();
	}

	public void glGetSamplerParameterIuiv (int sampler, int pname, IntBuffer @params) {
		calls++;
		gl32.glGetSamplerParameterIuiv(sampler, pname, @params);
		check();
	}

	public void glTexBuffer (int target, int internalformat, int buffer) {
		calls++;
		gl32.glTexBuffer(target, internalformat, buffer);
		check();
	}

	public void glTexBufferRange (int target, int internalformat, int buffer, int offset, int size) {
		calls++;
		gl32.glTexBufferRange(target, internalformat, buffer, offset, size);
		check();
	}

	public void glTexStorage3DMultisample (int target, int samples, int internalformat, int width, int height, int depth,
		bool fixedsamplelocations) {
		calls++;
		gl32.glTexStorage3DMultisample(target, samples, internalformat, width, height, depth, fixedsamplelocations);
		check();
	}

}
