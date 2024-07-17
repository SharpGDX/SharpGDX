using SharpGDX.Graphics;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Desktop
{
	internal class DesktopGL30 : DesktopGL20, GL30
	{
		public void glReadBuffer(int mode)
		{
			throw new NotImplementedException();
		}

		public void glDrawRangeElements(int mode, int start, int end, int count, int type, Buffer indices)
		{
			throw new NotImplementedException();
		}

		public void glDrawRangeElements(int mode, int start, int end, int count, int type, int offset)
		{
			throw new NotImplementedException();
		}

		public void glTexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type,
			int offset)
		{
			throw new NotImplementedException();
		}

		public void glTexImage3D(int target, int level, int internalformat, int width, int height, int depth, int border, int format,
			int type, Buffer pixels)
		{
			throw new NotImplementedException();
		}

		public void glTexImage3D(int target, int level, int internalformat, int width, int height, int depth, int border, int format,
			int type, int offset)
		{
			throw new NotImplementedException();
		}

		public void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type,
			int offset)
		{
			throw new NotImplementedException();
		}

		public void glTexSubImage3D(int target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth,
			int format, int type, Buffer pixels)
		{
			throw new NotImplementedException();
		}

		public void glTexSubImage3D(int target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth,
			int format, int type, int offset)
		{
			throw new NotImplementedException();
		}

		public void glCopyTexSubImage3D(int target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width,
			int height)
		{
			throw new NotImplementedException();
		}

		public void glGenQueries(int n, int[] ids, int offset)
		{
			throw new NotImplementedException();
		}

		public void glGenQueries(int n, IntBuffer ids)
		{
			throw new NotImplementedException();
		}

		public void glDeleteQueries(int n, int[] ids, int offset)
		{
			throw new NotImplementedException();
		}

		public void glDeleteQueries(int n, IntBuffer ids)
		{
			throw new NotImplementedException();
		}

		public bool glIsQuery(int id)
		{
			throw new NotImplementedException();
		}

		public void glBeginQuery(int target, int id)
		{
			throw new NotImplementedException();
		}

		public void glEndQuery(int target)
		{
			throw new NotImplementedException();
		}

		public void glGetQueryiv(int target, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetQueryObjectuiv(int id, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public bool glUnmapBuffer(int target)
		{
			throw new NotImplementedException();
		}

		public Buffer glGetBufferPointerv(int target, int pname)
		{
			throw new NotImplementedException();
		}

		public void glDrawBuffers(int n, IntBuffer bufs)
		{
			throw new NotImplementedException();
		}

		public void glUniformMatrix2x3fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniformMatrix3x2fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniformMatrix2x4fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniformMatrix4x2fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniformMatrix3x4fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniformMatrix4x3fv(int location, int count, bool transpose, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, int mask,
			int filter)
		{
			throw new NotImplementedException();
		}

		public void glRenderbufferStorageMultisample(int target, int samples, int internalformat, int width, int height)
		{
			throw new NotImplementedException();
		}

		public void glFramebufferTextureLayer(int target, int attachment, int texture, int level, int layer)
		{
			throw new NotImplementedException();
		}

		public Buffer glMapBufferRange(int target, int offset, int length, int access)
		{
			throw new NotImplementedException();
		}

		public void glFlushMappedBufferRange(int target, int offset, int length)
		{
			throw new NotImplementedException();
		}

		public void glBindVertexArray(int array)
		{
			throw new NotImplementedException();
		}

		public void glDeleteVertexArrays(int n, int[] arrays, int offset)
		{
			throw new NotImplementedException();
		}

		public void glDeleteVertexArrays(int n, IntBuffer arrays)
		{
			throw new NotImplementedException();
		}

		public void glGenVertexArrays(int n, int[] arrays, int offset)
		{
			throw new NotImplementedException();
		}

		public void glGenVertexArrays(int n, IntBuffer arrays)
		{
			throw new NotImplementedException();
		}

		public bool glIsVertexArray(int array)
		{
			throw new NotImplementedException();
		}

		public void glBeginTransformFeedback(int primitiveMode)
		{
			throw new NotImplementedException();
		}

		public void glEndTransformFeedback()
		{
			throw new NotImplementedException();
		}

		public void glBindBufferRange(int target, int index, int buffer, int offset, int size)
		{
			throw new NotImplementedException();
		}

		public void glBindBufferBase(int target, int index, int buffer)
		{
			throw new NotImplementedException();
		}

		public void glTransformFeedbackVaryings(int program, string[] varyings, int bufferMode)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribIPointer(int index, int size, int type, int stride, int offset)
		{
			throw new NotImplementedException();
		}

		public void glGetVertexAttribIiv(int index, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetVertexAttribIuiv(int index, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribI4i(int index, int x, int y, int z, int w)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribI4ui(int index, int x, int y, int z, int w)
		{
			throw new NotImplementedException();
		}

		public void glGetUniformuiv(int program, int location, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public int glGetFragDataLocation(int program, string name)
		{
			throw new NotImplementedException();
		}

		public void glUniform1uiv(int location, int count, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniform3uiv(int location, int count, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glUniform4uiv(int location, int count, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glClearBufferiv(int buffer, int drawbuffer, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glClearBufferuiv(int buffer, int drawbuffer, IntBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glClearBufferfv(int buffer, int drawbuffer, FloatBuffer value)
		{
			throw new NotImplementedException();
		}

		public void glClearBufferfi(int buffer, int drawbuffer, float depth, int stencil)
		{
			throw new NotImplementedException();
		}

		public string glGetStringi(int name, int index)
		{
			throw new NotImplementedException();
		}

		public void glCopyBufferSubData(int readTarget, int writeTarget, int readOffset, int writeOffset, int size)
		{
			throw new NotImplementedException();
		}

		public void glGetUniformIndices(int program, string[] uniformNames, IntBuffer uniformIndices)
		{
			throw new NotImplementedException();
		}

		public void glGetActiveUniformsiv(int program, int uniformCount, IntBuffer uniformIndices, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public int glGetUniformBlockIndex(int program, string uniformBlockName)
		{
			throw new NotImplementedException();
		}

		public void glGetActiveUniformBlockiv(int program, int uniformBlockIndex, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetActiveUniformBlockName(int program, int uniformBlockIndex, Buffer length, Buffer uniformBlockName)
		{
			throw new NotImplementedException();
		}

		public string glGetActiveUniformBlockName(int program, int uniformBlockIndex)
		{
			throw new NotImplementedException();
		}

		public void glUniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
		{
			throw new NotImplementedException();
		}

		public void glDrawArraysInstanced(int mode, int first, int count, int instanceCount)
		{
			throw new NotImplementedException();
		}

		public void glDrawElementsInstanced(int mode, int count, int type, int indicesOffset, int instanceCount)
		{
			throw new NotImplementedException();
		}

		public void glGetInteger64v(int pname, LongBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetBufferParameteri64v(int target, int pname, LongBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGenSamplers(int count, int[] samplers, int offset)
		{
			throw new NotImplementedException();
		}

		public void glGenSamplers(int count, IntBuffer samplers)
		{
			throw new NotImplementedException();
		}

		public void glDeleteSamplers(int count, int[] samplers, int offset)
		{
			throw new NotImplementedException();
		}

		public void glDeleteSamplers(int count, IntBuffer samplers)
		{
			throw new NotImplementedException();
		}

		public bool glIsSampler(int sampler)
		{
			throw new NotImplementedException();
		}

		public void glBindSampler(int unit, int sampler)
		{
			throw new NotImplementedException();
		}

		public void glSamplerParameteri(int sampler, int pname, int param)
		{
			throw new NotImplementedException();
		}

		public void glSamplerParameteriv(int sampler, int pname, IntBuffer param)
		{
			throw new NotImplementedException();
		}

		public void glSamplerParameterf(int sampler, int pname, float param)
		{
			throw new NotImplementedException();
		}

		public void glSamplerParameterfv(int sampler, int pname, FloatBuffer param)
		{
			throw new NotImplementedException();
		}

		public void glGetSamplerParameteriv(int sampler, int pname, IntBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glGetSamplerParameterfv(int sampler, int pname, FloatBuffer @params)
		{
			throw new NotImplementedException();
		}

		public void glVertexAttribDivisor(int index, int divisor)
		{
			throw new NotImplementedException();
		}

		public void glBindTransformFeedback(int target, int id)
		{
			throw new NotImplementedException();
		}

		public void glDeleteTransformFeedbacks(int n, int[] ids, int offset)
		{
			throw new NotImplementedException();
		}

		public void glDeleteTransformFeedbacks(int n, IntBuffer ids)
		{
			throw new NotImplementedException();
		}

		public void glGenTransformFeedbacks(int n, int[] ids, int offset)
		{
			throw new NotImplementedException();
		}

		public void glGenTransformFeedbacks(int n, IntBuffer ids)
		{
			throw new NotImplementedException();
		}

		public bool glIsTransformFeedback(int id)
		{
			throw new NotImplementedException();
		}

		public void glPauseTransformFeedback()
		{
			throw new NotImplementedException();
		}

		public void glResumeTransformFeedback()
		{
			throw new NotImplementedException();
		}

		public void glProgramParameteri(int program, int pname, int value)
		{
			throw new NotImplementedException();
		}

		public void glInvalidateFramebuffer(int target, int numAttachments, IntBuffer attachments)
		{
			throw new NotImplementedException();
		}

		public void glInvalidateSubFramebuffer(int target, int numAttachments, IntBuffer attachments, int x, int y, int width,
			int height)
		{
			throw new NotImplementedException();
		}
	}
}
