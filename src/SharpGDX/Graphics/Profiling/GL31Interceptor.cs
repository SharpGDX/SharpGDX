using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Mathematics;

namespace SharpGDX.Graphics.Profiling;

public class GL31Interceptor : GL30Interceptor , GL31 {

	internal readonly GL31 gl31;

	public GL31Interceptor (GLProfiler glProfiler, GL31 gl31) 
	: base(glProfiler, gl31)
	{
		
		this.gl31 = gl31;
	}

	protected void check () {
		int error = gl30.glGetError();
		while (error != GL20.GL_NO_ERROR) {
			glProfiler.getListener().onError(error);
			error = gl30.glGetError();
		}
	}

	public void glDispatchCompute (int num_groups_x, int num_groups_y, int num_groups_z) {
		calls++;
		gl31.glDispatchCompute(num_groups_x, num_groups_y, num_groups_z);
		check();
	}

	public void glDispatchComputeIndirect (long indirect) {
		calls++;
		gl31.glDispatchComputeIndirect(indirect);
		check();
	}

	public void glDrawArraysIndirect (int mode, long indirect) {
		drawCalls++;
		calls++;
		gl31.glDrawArraysIndirect(mode, indirect);
		check();
	}

	public void glDrawElementsIndirect (int mode, int type, long indirect) {
		drawCalls++;
		calls++;
		gl31.glDrawElementsIndirect(mode, type, indirect);
		check();
	}

	public void glFramebufferParameteri (int target, int pname, int param) {
		calls++;
		gl31.glFramebufferParameteri(target, pname, param);
		check();
	}

	public void glGetFramebufferParameteriv (int target, int pname, IntBuffer @params) {
		calls++;
		gl31.glGetFramebufferParameteriv(target, pname, @params);
		check();
	}

	public void glGetProgramInterfaceiv (int program, int programInterface, int pname, IntBuffer @params) {
		calls++;
		gl31.glGetProgramInterfaceiv(program, programInterface, pname, @params);
		check();
	}

	public int glGetProgramResourceIndex (int program, int programInterface, String name) {
		calls++;
		int v = gl31.glGetProgramResourceIndex(program, programInterface, name);
		check();
		return v;
	}

	public String glGetProgramResourceName (int program, int programInterface, int index) {
		calls++;
		String s = gl31.glGetProgramResourceName(program, programInterface, index);
		check();
		return s;
	}

	public void glGetProgramResourceiv (int program, int programInterface, int index, IntBuffer props, IntBuffer length,
		IntBuffer @params) {
		calls++;
		gl31.glGetProgramResourceiv(program, programInterface, index, props, length, @params);
		check();
	}

	public int glGetProgramResourceLocation (int program, int programInterface, String name) {
		calls++;
		int v = gl31.glGetProgramResourceLocation(program, programInterface, name);
		check();
		return v;
	}

	public void glUseProgramStages (int pipeline, int stages, int program) {
		calls++;
		gl31.glUseProgramStages(pipeline, stages, program);
		check();
	}

	public void glActiveShaderProgram (int pipeline, int program) {
		calls++;
		gl31.glActiveShaderProgram(pipeline, program);
		check();
	}

	public int glCreateShaderProgramv (int type, String[] strings) {
		calls++;
		int v = gl31.glCreateShaderProgramv(type, strings);
		check();
		return v;
	}

	public void glBindProgramPipeline (int pipeline) {
		calls++;
		gl31.glBindProgramPipeline(pipeline);
		check();
	}

	public void glDeleteProgramPipelines (int count, IntBuffer pipelines) {
		calls++;
		gl31.glDeleteProgramPipelines(count, pipelines);
		check();
	}

	public void glGenProgramPipelines (int count, IntBuffer pipelines) {
		calls++;
		gl31.glGenProgramPipelines(count, pipelines);
		check();
	}

	public bool glIsProgramPipeline (int pipeline) {
		calls++;
		bool v = gl31.glIsProgramPipeline(pipeline);
		check();
		return v;
	}

	public void glGetProgramPipelineiv (int pipeline, int pname, IntBuffer @params) {
		calls++;
		gl31.glGetProgramPipelineiv(pipeline, pname, @params);
		check();
	}

	public void glProgramUniform1i (int program, int location, int v0) {
		calls++;
		gl31.glProgramUniform1i(program, location, v0);
		check();
	}

	public void glProgramUniform2i (int program, int location, int v0, int v1) {
		calls++;
		gl31.glProgramUniform2i(program, location, v0, v1);
		check();
	}

	public void glProgramUniform3i (int program, int location, int v0, int v1, int v2) {
		calls++;
		gl31.glProgramUniform3i(program, location, v0, v1, v2);
		check();
	}

	public void glProgramUniform4i (int program, int location, int v0, int v1, int v2, int v3) {
		calls++;
		gl31.glProgramUniform4i(program, location, v0, v1, v2, v3);
		check();
	}

	public void glProgramUniform1ui (int program, int location, int v0) {
		calls++;
		gl31.glProgramUniform1ui(program, location, v0);
		check();
	}

	public void glProgramUniform2ui (int program, int location, int v0, int v1) {
		calls++;
		gl31.glProgramUniform2ui(program, location, v0, v1);
		check();
	}

	public void glProgramUniform3ui (int program, int location, int v0, int v1, int v2) {
		calls++;
		gl31.glProgramUniform3ui(program, location, v0, v1, v2);
		check();
	}

	public void glProgramUniform4ui (int program, int location, int v0, int v1, int v2, int v3) {
		calls++;
		gl31.glProgramUniform4ui(program, location, v0, v1, v2, v3);
		check();
	}

	public void glProgramUniform1f (int program, int location, float v0) {
		calls++;
		gl31.glProgramUniform1f(program, location, v0);
		check();
	}

	public void glProgramUniform2f (int program, int location, float v0, float v1) {
		calls++;
		gl31.glProgramUniform2f(program, location, v0, v1);
		check();
	}

	public void glProgramUniform3f (int program, int location, float v0, float v1, float v2) {
		calls++;
		gl31.glProgramUniform3f(program, location, v0, v1, v2);
		check();
	}

	public void glProgramUniform4f (int program, int location, float v0, float v1, float v2, float v3) {
		calls++;
		gl31.glProgramUniform4f(program, location, v0, v1, v2, v3);
		check();
	}

	public void glProgramUniform1iv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform1iv(program, location, value);
		check();
	}

	public void glProgramUniform2iv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform2iv(program, location, value);
		check();
	}

	public void glProgramUniform3iv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform3iv(program, location, value);
		check();
	}

	public void glProgramUniform4iv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform4iv(program, location, value);
		check();
	}

	public void glProgramUniform1uiv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform1uiv(program, location, value);
		check();
	}

	public void glProgramUniform2uiv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform2uiv(program, location, value);
		check();
	}

	public void glProgramUniform3uiv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform3uiv(program, location, value);
		check();
	}

	public void glProgramUniform4uiv (int program, int location, IntBuffer value) {
		calls++;
		gl31.glProgramUniform4uiv(program, location, value);
		check();
	}

	public void glProgramUniform1fv (int program, int location, FloatBuffer value) {
		calls++;
		gl31.glProgramUniform1fv(program, location, value);
		check();
	}

	public void glProgramUniform2fv (int program, int location, FloatBuffer value) {
		calls++;
		gl31.glProgramUniform2fv(program, location, value);
		check();
	}

	public void glProgramUniform3fv (int program, int location, FloatBuffer value) {
		calls++;
		gl31.glProgramUniform3fv(program, location, value);
		check();
	}

	public void glProgramUniform4fv (int program, int location, FloatBuffer value) {
		calls++;
		gl31.glProgramUniform4fv(program, location, value);
		check();
	}

	public void glProgramUniformMatrix2fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix2fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix3fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix3fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix4fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix4fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix2x3fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix2x3fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix3x2fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix3x2fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix2x4fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix2x4fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix4x2fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix4x2fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix3x4fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix3x4fv(program, location, transpose, value);
		check();
	}

	public void glProgramUniformMatrix4x3fv (int program, int location, bool transpose, FloatBuffer value) {
		calls++;
		gl31.glProgramUniformMatrix4x3fv(program, location, transpose, value);
		check();
	}

	public void glValidateProgramPipeline (int pipeline) {
		calls++;
		gl31.glValidateProgramPipeline(pipeline);
		check();
	}

	public String glGetProgramPipelineInfoLog (int program) {
		calls++;
		String s = gl31.glGetProgramPipelineInfoLog(program);
		check();
		return s;
	}

	public void glBindImageTexture (int unit, int texture, int level, bool layered, int layer, int access, int format) {
		calls++;
		gl31.glBindImageTexture(unit, texture, level, layered, layer, access, format);
		check();
	}

	public void glGetBooleani_v (int target, int index, IntBuffer data) {
		calls++;
		gl31.glGetBooleani_v(target, index, data);
		check();
	}

	public void glMemoryBarrier (int barriers) {
		calls++;
		gl31.glMemoryBarrier(barriers);
		check();
	}

	public void glMemoryBarrierByRegion (int barriers) {
		calls++;
		gl31.glMemoryBarrierByRegion(barriers);
		check();
	}

	public void glTexStorage2DMultisample (int target, int samples, int internalformat, int width, int height,
		bool fixedsamplelocations) {
		calls++;
		gl31.glTexStorage2DMultisample(target, samples, internalformat, width, height, fixedsamplelocations);
		check();
	}

	public void glGetMultisamplefv (int pname, int index, FloatBuffer val) {
		calls++;
		gl31.glGetMultisamplefv(pname, index, val);
		check();
	}

	public void glSampleMaski (int maskNumber, int mask) {
		calls++;
		gl31.glSampleMaski(maskNumber, mask);
		check();
	}

	public void glGetTexLevelParameteriv (int target, int level, int pname, IntBuffer @params) {
		calls++;
		gl31.glGetTexLevelParameteriv(target, level, pname, @params);
		check();
	}

	public void glGetTexLevelParameterfv (int target, int level, int pname, FloatBuffer @params) {
		calls++;
		gl31.glGetTexLevelParameterfv(target, level, pname, @params);
		check();
	}

	public void glBindVertexBuffer (int bindingindex, int buffer, long offset, int stride) {
		calls++;
		gl31.glBindVertexBuffer(bindingindex, buffer, offset, stride);
		check();
	}

	public void glVertexAttribFormat (int attribindex, int size, int type, bool normalized, int relativeoffset) {
		calls++;
		gl31.glVertexAttribFormat(attribindex, size, type, normalized, relativeoffset);
		check();
	}

	public void glVertexAttribIFormat (int attribindex, int size, int type, int relativeoffset) {
		calls++;
		gl31.glVertexAttribIFormat(attribindex, size, type, relativeoffset);
		check();
	}

	public void glVertexAttribBinding (int attribindex, int bindingindex) {
		calls++;
		gl31.glVertexAttribBinding(attribindex, bindingindex);
		check();
	}

	public void glVertexBindingDivisor (int bindingindex, int divisor) {
		calls++;
		gl31.glVertexBindingDivisor(bindingindex, divisor);
		check();
	}

}
