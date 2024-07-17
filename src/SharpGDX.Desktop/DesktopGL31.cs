using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;

namespace SharpGDX.Desktop
{
	 internal class DesktopGL31 : DesktopGL30, GL31
	{
		public void glDispatchCompute(int num_groups_x, int num_groups_y, int num_groups_z)
		{
			throw new NotImplementedException();
		}

		public void glDispatchComputeIndirect(long indirect)
		{
			throw new NotImplementedException();
		}

		public void glDrawArraysIndirect(int mode, long indirect)
		{
			throw new NotImplementedException();
		}

		public void glDrawElementsIndirect(int mode, int type, long indirect)
		{
			throw new NotImplementedException();
		}

		public void glFramebufferParameteri(int target, int pname, int param)
		{
			throw new NotImplementedException();
		}

		public void glGetFramebufferParameteriv(int target, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetProgramInterfaceiv(int program, int programInterface, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public int glGetProgramResourceIndex(int program, int programInterface, string name)
		{
			throw new NotImplementedException();
		}

		public string glGetProgramResourceName(int program, int programInterface, int index)
		{
			throw new NotImplementedException();
		}

		public void glGetProgramResourceiv(int program, int programInterface, int index, IntBuffer props, IntBuffer length,
			IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public int glGetProgramResourceLocation(int program, int programInterface, string name)
		{
			throw new NotImplementedException();
		}

		public void glUseProgramStages(int pipeline, int stages, int program)
		{
			throw new NotImplementedException();
		}

		public void glActiveShaderProgram(int pipeline, int program)
		{
			throw new NotImplementedException();
		}

		public int glCreateShaderProgramv(int type, string[] strings)
		{
			throw new NotImplementedException();
		}

		public void glBindProgramPipeline(int pipeline)
		{
			throw new NotImplementedException();
		}

		public void glDeleteProgramPipelines(int n, IntBuffer pipelines)
		{
			throw new NotImplementedException();
		}

		public void glGenProgramPipelines(int n, IntBuffer pipelines)
		{
			throw new NotImplementedException();
		}

		public bool glIsProgramPipeline(int pipeline)
		{
			throw new NotImplementedException();
		}

		public void glGetProgramPipelineiv(int pipeline, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform1i(int program, int location, int v0)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform2i(int program, int location, int v0, int v1)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform3i(int program, int location, int v0, int v1, int v2)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform4i(int program, int location, int v0, int v1, int v2, int v3)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform1ui(int program, int location, int v0)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform2ui(int program, int location, int v0, int v1)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform3ui(int program, int location, int v0, int v1, int v2)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform4ui(int program, int location, int v0, int v1, int v2, int v3)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform1f(int program, int location, float v0)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform2f(int program, int location, float v0, float v1)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform3f(int program, int location, float v0, float v1, float v2)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform4f(int program, int location, float v0, float v1, float v2, float v3)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform1iv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform2iv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform3iv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform4iv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform1uiv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform2uiv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform3uiv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform4uiv(int program, int location, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform1fv(int program, int location, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform2fv(int program, int location, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform3fv(int program, int location, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniform4fv(int program, int location, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix2fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix3fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix4fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix2x3fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix3x2fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix2x4fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix4x2fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix3x4fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glProgramUniformMatrix4x3fv(int program, int location, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glValidateProgramPipeline(int pipeline)
		{
			throw new NotImplementedException();
		}

		public string glGetProgramPipelineInfoLog(int program)
		{
			throw new NotImplementedException();
		}

		public void glBindImageTexture(int unit, int texture, int level, bool layered, int layer, int access, int format)
		{
			throw new NotImplementedException();
		}

		public void glGetBooleani_v(int target, int index, IntBuffer data)
		{
			throw new NotImplementedException();
		}

		public void glMemoryBarrier(int barriers)
		{
			throw new NotImplementedException();
		}

		public void glMemoryBarrierByRegion(int barriers)
		{
			throw new NotImplementedException();
		}

		public void glTexStorage2DMultisample(int target, int samples, int internalformat, int width, int height,
			bool fixedsamplelocations)
		{
			throw new NotImplementedException();
		}

		public void glGetMultisamplefv(int pname, int index, FloatBuffer val)
		{
			throw new NotImplementedException();
		}

		public void glSampleMaski(int maskNumber, int mask)
		{
			throw new NotImplementedException();
		}

		public void glGetTexLevelParameteriv(int target, int level, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetTexLevelParameterfv(int target, int level, int pname, FloatBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glBindVertexBuffer(int bindingindex, int buffer, long offset, int stride)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribFormat(int attribindex, int size, int type, bool normalized, int relativeoffset)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribIFormat(int attribindex, int size, int type, int relativeoffset)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribBinding(int attribindex, int bindingindex)
		{
			throw new NotImplementedException();
		}

		public void glVertexBindingDivisor(int bindingindex, int divisor)
		{
			throw new NotImplementedException();
		}
	}
}
