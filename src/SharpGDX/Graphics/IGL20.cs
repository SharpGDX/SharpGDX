using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Shims;

namespace SharpGDX.Graphics
{
	/** Interface wrapping all the methods of OpenGL ES 2.0
 * @author mzechner */
	public interface GL20
	{
		public const int GL_ES_VERSION_2_0 = 1;
		public const int GL_DEPTH_BUFFER_BIT = 0x00000100;
		public const int GL_STENCIL_BUFFER_BIT = 0x00000400;
		public const int GL_COLOR_BUFFER_BIT = 0x00004000;
		public const int GL_FALSE = 0;
		public const int GL_TRUE = 1;
		public const int GL_POINTS = 0x0000;
		public const int GL_LINES = 0x0001;
		public const int GL_LINE_LOOP = 0x0002;
		public const int GL_LINE_STRIP = 0x0003;
		public const int GL_TRIANGLES = 0x0004;
		public const int GL_TRIANGLE_STRIP = 0x0005;
		public const int GL_TRIANGLE_FAN = 0x0006;
		public const int GL_ZERO = 0;
		public const int GL_ONE = 1;
		public const int GL_SRC_COLOR = 0x0300;
		public const int GL_ONE_MINUS_SRC_COLOR = 0x0301;
		public const int GL_SRC_ALPHA = 0x0302;
		public const int GL_ONE_MINUS_SRC_ALPHA = 0x0303;
		public const int GL_DST_ALPHA = 0x0304;
		public const int GL_ONE_MINUS_DST_ALPHA = 0x0305;
		public const int GL_DST_COLOR = 0x0306;
		public const int GL_ONE_MINUS_DST_COLOR = 0x0307;
		public const int GL_SRC_ALPHA_SATURATE = 0x0308;
		public const int GL_FUNC_ADD = 0x8006;
		public const int GL_BLEND_EQUATION = 0x8009;
		public const int GL_BLEND_EQUATION_RGB = 0x8009;
		public const int GL_BLEND_EQUATION_ALPHA = 0x883D;
		public const int GL_FUNC_SUBTRACT = 0x800A;
		public const int GL_FUNC_REVERSE_SUBTRACT = 0x800B;
		public const int GL_BLEND_DST_RGB = 0x80C8;
		public const int GL_BLEND_SRC_RGB = 0x80C9;
		public const int GL_BLEND_DST_ALPHA = 0x80CA;
		public const int GL_BLEND_SRC_ALPHA = 0x80CB;
		public const int GL_CONSTANT_COLOR = 0x8001;
		public const int GL_ONE_MINUS_CONSTANT_COLOR = 0x8002;
		public const int GL_CONSTANT_ALPHA = 0x8003;
		public const int GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004;
		public const int GL_BLEND_COLOR = 0x8005;
		public const int GL_ARRAY_BUFFER = 0x8892;
		public const int GL_ELEMENT_ARRAY_BUFFER = 0x8893;
		public const int GL_ARRAY_BUFFER_BINDING = 0x8894;
		public const int GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895;
		public const int GL_STREAM_DRAW = 0x88E0;
		public const int GL_STATIC_DRAW = 0x88E4;
		public const int GL_DYNAMIC_DRAW = 0x88E8;
		public const int GL_BUFFER_SIZE = 0x8764;
		public const int GL_BUFFER_USAGE = 0x8765;
		public const int GL_CURRENT_VERTEX_ATTRIB = 0x8626;
		public const int GL_FRONT = 0x0404;
		public const int GL_BACK = 0x0405;
		public const int GL_FRONT_AND_BACK = 0x0408;
		public const int GL_TEXTURE_2D = 0x0DE1;
		public const int GL_CULL_FACE = 0x0B44;
		public const int GL_BLEND = 0x0BE2;
		public const int GL_DITHER = 0x0BD0;
		public const int GL_STENCIL_TEST = 0x0B90;
		public const int GL_DEPTH_TEST = 0x0B71;
		public const int GL_SCISSOR_TEST = 0x0C11;
		public const int GL_POLYGON_OFFSET_FILL = 0x8037;
		public const int GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
		public const int GL_SAMPLE_COVERAGE = 0x80A0;
		public const int GL_NO_ERROR = 0;
		public const int GL_INVALID_ENUM = 0x0500;
		public const int GL_INVALID_VALUE = 0x0501;
		public const int GL_INVALID_OPERATION = 0x0502;
		public const int GL_OUT_OF_MEMORY = 0x0505;
		public const int GL_CW = 0x0900;
		public const int GL_CCW = 0x0901;
		public const int GL_LINE_WIDTH = 0x0B21;
		public const int GL_ALIASED_POINT_SIZE_RANGE = 0x846D;
		public const int GL_ALIASED_LINE_WIDTH_RANGE = 0x846E;
		public const int GL_CULL_FACE_MODE = 0x0B45;
		public const int GL_FRONT_FACE = 0x0B46;
		public const int GL_DEPTH_RANGE = 0x0B70;
		public const int GL_DEPTH_WRITEMASK = 0x0B72;
		public const int GL_DEPTH_CLEAR_VALUE = 0x0B73;
		public const int GL_DEPTH_FUNC = 0x0B74;
		public const int GL_STENCIL_CLEAR_VALUE = 0x0B91;
		public const int GL_STENCIL_FUNC = 0x0B92;
		public const int GL_STENCIL_FAIL = 0x0B94;
		public const int GL_STENCIL_PASS_DEPTH_FAIL = 0x0B95;
		public const int GL_STENCIL_PASS_DEPTH_PASS = 0x0B96;
		public const int GL_STENCIL_REF = 0x0B97;
		public const int GL_STENCIL_VALUE_MASK = 0x0B93;
		public const int GL_STENCIL_WRITEMASK = 0x0B98;
		public const int GL_STENCIL_BACK_FUNC = 0x8800;
		public const int GL_STENCIL_BACK_FAIL = 0x8801;
		public const int GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802;
		public const int GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803;
		public const int GL_STENCIL_BACK_REF = 0x8CA3;
		public const int GL_STENCIL_BACK_VALUE_MASK = 0x8CA4;
		public const int GL_STENCIL_BACK_WRITEMASK = 0x8CA5;
		public const int GL_VIEWPORT = 0x0BA2;
		public const int GL_SCISSOR_BOX = 0x0C10;
		public const int GL_COLOR_CLEAR_VALUE = 0x0C22;
		public const int GL_COLOR_WRITEMASK = 0x0C23;
		public const int GL_UNPACK_ALIGNMENT = 0x0CF5;
		public const int GL_PACK_ALIGNMENT = 0x0D05;
		public const int GL_MAX_TEXTURE_SIZE = 0x0D33;
		public const int GL_MAX_TEXTURE_UNITS = 0x84E2;
		public const int GL_MAX_VIEWPORT_DIMS = 0x0D3A;
		public const int GL_SUBPIXEL_BITS = 0x0D50;
		public const int GL_RED_BITS = 0x0D52;
		public const int GL_GREEN_BITS = 0x0D53;
		public const int GL_BLUE_BITS = 0x0D54;
		public const int GL_ALPHA_BITS = 0x0D55;
		public const int GL_DEPTH_BITS = 0x0D56;
		public const int GL_STENCIL_BITS = 0x0D57;
		public const int GL_POLYGON_OFFSET_UNITS = 0x2A00;
		public const int GL_POLYGON_OFFSET_FACTOR = 0x8038;
		public const int GL_TEXTURE_BINDING_2D = 0x8069;
		public const int GL_SAMPLE_BUFFERS = 0x80A8;
		public const int GL_SAMPLES = 0x80A9;
		public const int GL_SAMPLE_COVERAGE_VALUE = 0x80AA;
		public const int GL_SAMPLE_COVERAGE_INVERT = 0x80AB;
		public const int GL_NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2;
		public const int GL_COMPRESSED_TEXTURE_FORMATS = 0x86A3;
		public const int GL_DONT_CARE = 0x1100;
		public const int GL_FASTEST = 0x1101;
		public const int GL_NICEST = 0x1102;
		public const int GL_GENERATE_MIPMAP = 0x8191;
		public const int GL_GENERATE_MIPMAP_HINT = 0x8192;
		public const int GL_BYTE = 0x1400;
		public const int GL_UNSIGNED_BYTE = 0x1401;
		public const int GL_SHORT = 0x1402;
		public const int GL_UNSIGNED_SHORT = 0x1403;
		public const int GL_INT = 0x1404;
		public const int GL_UNSIGNED_INT = 0x1405;
		public const int GL_FLOAT = 0x1406;
		public const int GL_FIXED = 0x140C;
		public const int GL_DEPTH_COMPONENT = 0x1902;
		public const int GL_ALPHA = 0x1906;
		public const int GL_RGB = 0x1907;
		public const int GL_RGBA = 0x1908;
		public const int GL_LUMINANCE = 0x1909;
		public const int GL_LUMINANCE_ALPHA = 0x190A;
		public const int GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033;
		public const int GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034;
		public const int GL_UNSIGNED_SHORT_5_6_5 = 0x8363;
		public const int GL_FRAGMENT_SHADER = 0x8B30;
		public const int GL_VERTEX_SHADER = 0x8B31;
		public const int GL_MAX_VERTEX_ATTRIBS = 0x8869;
		public const int GL_MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB;
		public const int GL_MAX_VARYING_VECTORS = 0x8DFC;
		public const int GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D;
		public const int GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C;
		public const int GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872;
		public const int GL_MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD;
		public const int GL_SHADER_TYPE = 0x8B4F;
		public const int GL_DELETE_STATUS = 0x8B80;
		public const int GL_LINK_STATUS = 0x8B82;
		public const int GL_VALIDATE_STATUS = 0x8B83;
		public const int GL_ATTACHED_SHADERS = 0x8B85;
		public const int GL_ACTIVE_UNIFORMS = 0x8B86;
		public const int GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87;
		public const int GL_ACTIVE_ATTRIBUTES = 0x8B89;
		public const int GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A;
		public const int GL_SHADING_LANGUAGE_VERSION = 0x8B8C;
		public const int GL_CURRENT_PROGRAM = 0x8B8D;
		public const int GL_NEVER = 0x0200;
		public const int GL_LESS = 0x0201;
		public const int GL_EQUAL = 0x0202;
		public const int GL_LEQUAL = 0x0203;
		public const int GL_GREATER = 0x0204;
		public const int GL_NOTEQUAL = 0x0205;
		public const int GL_GEQUAL = 0x0206;
		public const int GL_ALWAYS = 0x0207;
		public const int GL_KEEP = 0x1E00;
		public const int GL_REPLACE = 0x1E01;
		public const int GL_INCR = 0x1E02;
		public const int GL_DECR = 0x1E03;
		public const int GL_INVERT = 0x150A;
		public const int GL_INCR_WRAP = 0x8507;
		public const int GL_DECR_WRAP = 0x8508;
		public const int GL_VENDOR = 0x1F00;
		public const int GL_RENDERER = 0x1F01;
		public const int GL_VERSION = 0x1F02;
		public const int GL_EXTENSIONS = 0x1F03;
		public const int GL_NEAREST = 0x2600;
		public const int GL_LINEAR = 0x2601;
		public const int GL_NEAREST_MIPMAP_NEAREST = 0x2700;
		public const int GL_LINEAR_MIPMAP_NEAREST = 0x2701;
		public const int GL_NEAREST_MIPMAP_LINEAR = 0x2702;
		public const int GL_LINEAR_MIPMAP_LINEAR = 0x2703;
		public const int GL_TEXTURE_MAG_FILTER = 0x2800;
		public const int GL_TEXTURE_MIN_FILTER = 0x2801;
		public const int GL_TEXTURE_WRAP_S = 0x2802;
		public const int GL_TEXTURE_WRAP_T = 0x2803;
		public const int GL_TEXTURE = 0x1702;
		public const int GL_TEXTURE_CUBE_MAP = 0x8513;
		public const int GL_TEXTURE_BINDING_CUBE_MAP = 0x8514;
		public const int GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515;
		public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516;
		public const int GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517;
		public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518;
		public const int GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519;
		public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851A;
		public const int GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C;
		public const int GL_TEXTURE0 = 0x84C0;
		public const int GL_TEXTURE1 = 0x84C1;
		public const int GL_TEXTURE2 = 0x84C2;
		public const int GL_TEXTURE3 = 0x84C3;
		public const int GL_TEXTURE4 = 0x84C4;
		public const int GL_TEXTURE5 = 0x84C5;
		public const int GL_TEXTURE6 = 0x84C6;
		public const int GL_TEXTURE7 = 0x84C7;
		public const int GL_TEXTURE8 = 0x84C8;
		public const int GL_TEXTURE9 = 0x84C9;
		public const int GL_TEXTURE10 = 0x84CA;
		public const int GL_TEXTURE11 = 0x84CB;
		public const int GL_TEXTURE12 = 0x84CC;
		public const int GL_TEXTURE13 = 0x84CD;
		public const int GL_TEXTURE14 = 0x84CE;
		public const int GL_TEXTURE15 = 0x84CF;
		public const int GL_TEXTURE16 = 0x84D0;
		public const int GL_TEXTURE17 = 0x84D1;
		public const int GL_TEXTURE18 = 0x84D2;
		public const int GL_TEXTURE19 = 0x84D3;
		public const int GL_TEXTURE20 = 0x84D4;
		public const int GL_TEXTURE21 = 0x84D5;
		public const int GL_TEXTURE22 = 0x84D6;
		public const int GL_TEXTURE23 = 0x84D7;
		public const int GL_TEXTURE24 = 0x84D8;
		public const int GL_TEXTURE25 = 0x84D9;
		public const int GL_TEXTURE26 = 0x84DA;
		public const int GL_TEXTURE27 = 0x84DB;
		public const int GL_TEXTURE28 = 0x84DC;
		public const int GL_TEXTURE29 = 0x84DD;
		public const int GL_TEXTURE30 = 0x84DE;
		public const int GL_TEXTURE31 = 0x84DF;
		public const int GL_ACTIVE_TEXTURE = 0x84E0;
		public const int GL_REPEAT = 0x2901;
		public const int GL_CLAMP_TO_EDGE = 0x812F;
		public const int GL_MIRRORED_REPEAT = 0x8370;
		public const int GL_FLOAT_VEC2 = 0x8B50;
		public const int GL_FLOAT_VEC3 = 0x8B51;
		public const int GL_FLOAT_VEC4 = 0x8B52;
		public const int GL_INT_VEC2 = 0x8B53;
		public const int GL_INT_VEC3 = 0x8B54;
		public const int GL_INT_VEC4 = 0x8B55;
		public const int GL_BOOL = 0x8B56;
		public const int GL_BOOL_VEC2 = 0x8B57;
		public const int GL_BOOL_VEC3 = 0x8B58;
		public const int GL_BOOL_VEC4 = 0x8B59;
		public const int GL_FLOAT_MAT2 = 0x8B5A;
		public const int GL_FLOAT_MAT3 = 0x8B5B;
		public const int GL_FLOAT_MAT4 = 0x8B5C;
		public const int GL_SAMPLER_2D = 0x8B5E;
		public const int GL_SAMPLER_CUBE = 0x8B60;
		public const int GL_VERTEX_ATTRIB_ARRAY_ENABLED = 0x8622;
		public const int GL_VERTEX_ATTRIB_ARRAY_SIZE = 0x8623;
		public const int GL_VERTEX_ATTRIB_ARRAY_STRIDE = 0x8624;
		public const int GL_VERTEX_ATTRIB_ARRAY_TYPE = 0x8625;
		public const int GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 0x886A;
		public const int GL_VERTEX_ATTRIB_ARRAY_POINTER = 0x8645;
		public const int GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F;
		public const int GL_IMPLEMENTATION_COLOR_READ_TYPE = 0x8B9A;
		public const int GL_IMPLEMENTATION_COLOR_READ_FORMAT = 0x8B9B;
		public const int GL_COMPILE_STATUS = 0x8B81;
		public const int GL_INFO_LOG_LENGTH = 0x8B84;
		public const int GL_SHADER_SOURCE_LENGTH = 0x8B88;
		public const int GL_SHADER_COMPILER = 0x8DFA;
		public const int GL_SHADER_BINARY_FORMATS = 0x8DF8;
		public const int GL_NUM_SHADER_BINARY_FORMATS = 0x8DF9;
		public const int GL_LOW_FLOAT = 0x8DF0;
		public const int GL_MEDIUM_FLOAT = 0x8DF1;
		public const int GL_HIGH_FLOAT = 0x8DF2;
		public const int GL_LOW_INT = 0x8DF3;
		public const int GL_MEDIUM_INT = 0x8DF4;
		public const int GL_HIGH_INT = 0x8DF5;
		public const int GL_FRAMEBUFFER = 0x8D40;
		public const int GL_RENDERBUFFER = 0x8D41;
		public const int GL_RGBA4 = 0x8056;
		public const int GL_RGB5_A1 = 0x8057;
		public const int GL_RGB565 = 0x8D62;
		public const int GL_DEPTH_COMPONENT16 = 0x81A5;
		public const int GL_STENCIL_INDEX = 0x1901;
		public const int GL_STENCIL_INDEX8 = 0x8D48;
		public const int GL_RENDERBUFFER_WIDTH = 0x8D42;
		public const int GL_RENDERBUFFER_HEIGHT = 0x8D43;
		public const int GL_RENDERBUFFER_INTERNAL_FORMAT = 0x8D44;
		public const int GL_RENDERBUFFER_RED_SIZE = 0x8D50;
		public const int GL_RENDERBUFFER_GREEN_SIZE = 0x8D51;
		public const int GL_RENDERBUFFER_BLUE_SIZE = 0x8D52;
		public const int GL_RENDERBUFFER_ALPHA_SIZE = 0x8D53;
		public const int GL_RENDERBUFFER_DEPTH_SIZE = 0x8D54;
		public const int GL_RENDERBUFFER_STENCIL_SIZE = 0x8D55;
		public const int GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 0x8CD0;
		public const int GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 0x8CD1;
		public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 0x8CD2;
		public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 0x8CD3;
		public const int GL_COLOR_ATTACHMENT0 = 0x8CE0;
		public const int GL_DEPTH_ATTACHMENT = 0x8D00;
		public const int GL_STENCIL_ATTACHMENT = 0x8D20;
		public const int GL_NONE = 0;
		public const int GL_FRAMEBUFFER_COMPLETE = 0x8CD5;
		public const int GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6;
		public const int GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7;
		public const int GL_FRAMEBUFFER_INCOMPLETE_DIMENSIONS = 0x8CD9;
		public const int GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD;
		public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;
		public const int GL_RENDERBUFFER_BINDING = 0x8CA7;
		public const int GL_MAX_RENDERBUFFER_SIZE = 0x84E8;
		public const int GL_INVALID_FRAMEBUFFER_OPERATION = 0x0506;
		public const int GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642;

		// Extensions
		public const int GL_COVERAGE_BUFFER_BIT_NV = 0x8000;
		public const int GL_TEXTURE_MAX_ANISOTROPY_EXT = 0x84FE;
		public const int GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT = 0x84FF;

		public void glActiveTexture(int texture);

		public void glBindTexture(int target, int texture);

		public void glBlendFunc(int sfactor, int dfactor);

		public void glClear(int mask);

		public void glClearColor(float red, float green, float blue, float alpha);

		public void glClearDepthf(float depth);
		public void glClearStencil(int s);
		public void glColorMask(bool red, bool green, bool blue, bool alpha);
		public void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border,
		int imageSize, Buffer data);

		public void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format,
			int imageSize, Buffer data);

		public void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border);

		public void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height);

		public void glCullFace(int mode);

		public void glDeleteTextures(int n, IntBuffer textures);

		public void glDeleteTexture(int texture);

		public void glDepthFunc(int func);

		public void glDepthMask(bool flag);

		public void glDepthRangef(float zNear, float zFar);

		public void glDisable(int cap);

		public void glDrawArrays(int mode, int first, int count);

		/** Not fully supported with GWT backend: indices content is ignored, only buffer position is used. */
		public void glDrawElements(int mode, int count, int type, Buffer indices);

		public void glEnable(int cap);

		public void glFinish();

		public void glFlush();

		public void glFrontFace(int mode);

		public void glGenTextures(int n, IntBuffer textures);

		public int glGenTexture();

		public int glGetError();

		public void glGetIntegerv(int pname, IntBuffer @params);

		public String glGetString(int name);

		public void glHint(int target, int mode);

		public void glLineWidth(float width);

		public void glPixelStorei(int pname, int param);

		public void glPolygonOffset(float factor, float units);

		public void glReadPixels(int x, int y, int width, int height, int format, int type, Buffer pixels);

		public void glScissor(int x, int y, int width, int height);

		public void glStencilFunc(int func, int @ref, int mask);

		public void glStencilMask(int mask);

		public void glStencilOp(int fail, int zfail, int zpass);

		public void glTexImage2D
		(
			int target,
			int level,
			int internalFormat,
			int width,
			int height,
			int border,
			int format,
			int type,
			Buffer pixels
		);

		public void glTexImage2D<T>
		(
			int target,
			int level,
			int internalFormat,
			int width,
			int height,
			int border,
			int format,
			int type,
			T[] pixels
		) where T: struct;

		public void glTexParameterf(int target, int pname, float param);

		public void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type,
			Buffer pixels);

		public void glViewport(int x, int y, int width, int height);

		public void glAttachShader(int program, int shader);

		public void glBindAttribLocation(int program, int index, String name);

		public void glBindBuffer(int target, int buffer);

		public void glBindFramebuffer(int target, int framebuffer);

		public void glBindRenderbuffer(int target, int renderbuffer);

		public void glBlendColor(float red, float green, float blue, float alpha);

		public void glBlendEquation(int mode);

		public void glBlendEquationSeparate(int modeRGB, int modeAlpha);

		public void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);

		public void glBufferData(int target, int size, Buffer data, int usage);

		public void glBufferSubData(int target, int offset, int size, Buffer data);

		public int glCheckFramebufferStatus(int target);

		public void glCompileShader(int shader);

		public int glCreateProgram();

		public int glCreateShader(int type);

		public void glDeleteBuffer(int buffer);

		public void glDeleteBuffers(int n, IntBuffer buffers);

		public void glDeleteFramebuffer(int framebuffer);

		public void glDeleteFramebuffers(int n, IntBuffer framebuffers);

		public void glDeleteProgram(int program);

		public void glDeleteRenderbuffer(int renderbuffer);

		public void glDeleteRenderbuffers(int n, IntBuffer renderbuffers);

		public void glDeleteShader(int shader);

		public void glDetachShader(int program, int shader);

		public void glDisableVertexAttribArray(int index);

		public void glDrawElements(int mode, int count, int type, int indices);

		public void glEnableVertexAttribArray(int index);

		public void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer);

		public void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level);

		public int glGenBuffer();

		public void glGenBuffers(int n, IntBuffer buffers);

		public void glGenerateMipmap(int target);

		public int glGenFramebuffer();

		public void glGenFramebuffers(int n, IntBuffer framebuffers);

		public int glGenRenderbuffer();

		public void glGenRenderbuffers(int n, IntBuffer renderbuffers);

		// deviates
		public String glGetActiveAttrib(int program, int index, IntBuffer size, IntBuffer type);

		// deviates
		public String glGetActiveUniform(int program, int index, IntBuffer size, IntBuffer type);

		public void glGetAttachedShaders(int program, int maxcount, Buffer count, IntBuffer shaders);

		public int glGetAttribLocation(int program, String name);

		public void glGetBooleanv(int pname, Buffer @params);

		public void glGetBufferParameteriv(int target, int pname, IntBuffer @params);

		public void glGetFloatv(int pname, FloatBuffer @params);

		public void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, IntBuffer @params);

		public void glGetProgramiv(int program, int pname, IntBuffer @params);

		// deviates
		public String glGetProgramInfoLog(int program);

		public void glGetRenderbufferParameteriv(int target, int pname, IntBuffer @params);

		public void glGetShaderiv(int shader, int pname, IntBuffer @params);

		// deviates
		public String glGetShaderInfoLog(int shader);

		public void glGetShaderPrecisionFormat(int shadertype, int precisiontype, IntBuffer range, IntBuffer precision);

		public void glGetTexParameterfv(int target, int pname, FloatBuffer @params);

		public void glGetTexParameteriv(int target, int pname, IntBuffer @params);

		public void glGetUniformfv(int program, int location, FloatBuffer @params);

		public void glGetUniformiv(int program, int location, IntBuffer @params);

		public int glGetUniformLocation(int program, String name);

		public void glGetVertexAttribfv(int index, int pname, FloatBuffer @params);

		public void glGetVertexAttribiv(int index, int pname, IntBuffer @params);

		public void glGetVertexAttribPointerv(int index, int pname, Buffer pointer);

		public bool glIsBuffer(int buffer);

		public bool glIsEnabled(int cap);

		public bool glIsFramebuffer(int framebuffer);
		public bool glIsProgram(int program);

		public bool glIsRenderbuffer(int renderbuffer);

		public bool glIsShader(int shader);

		public bool glIsTexture(int texture);

		public void glLinkProgram(int program);

		public void glReleaseShaderCompiler();

		public void glRenderbufferStorage(int target, int internalformat, int width, int height);

		public void glSampleCoverage(float value, bool invert);

		public void glShaderBinary(int n, IntBuffer shaders, int binaryformat, Buffer binary, int length);

		// Deviates
		public void glShaderSource(int shader, String @string);

		public void glStencilFuncSeparate(int face, int func, int @ref, int mask);

		public void glStencilMaskSeparate(int face, int mask);

		public void glStencilOpSeparate(int face, int fail, int zfail, int zpass);

		public void glTexParameterfv(int target, int pname, FloatBuffer @params);

		public void glTexParameteri(int target, int pname, int param);

		public void glTexParameteriv(int target, int pname, IntBuffer @params);

		public void glUniform1f(int location, float x);

		public void glUniform1fv(int location, int count, FloatBuffer v);

		public void glUniform1fv(int location, int count, float[] v, int offset);

		public void glUniform1i(int location, int x);

		public void glUniform1iv(int location, int count, IntBuffer v);

		public void glUniform1iv(int location, int count, int[] v, int offset);

		public void glUniform2f(int location, float x, float y);

		public void glUniform2fv(int location, int count, FloatBuffer v);

		public void glUniform2fv(int location, int count, float[] v, int offset);

		public void glUniform2i(int location, int x, int y);

		public void glUniform2iv(int location, int count, IntBuffer v);

		public void glUniform2iv(int location, int count, int[] v, int offset);

		public void glUniform3f(int location, float x, float y, float z);

		public void glUniform3fv(int location, int count, FloatBuffer v);

		public void glUniform3fv(int location, int count, float[] v, int offset);

		public void glUniform3i(int location, int x, int y, int z);

		public void glUniform3iv(int location, int count, IntBuffer v);

		public void glUniform3iv(int location, int count, int[] v, int offset);

		public void glUniform4f(int location, float x, float y, float z, float w);

		public void glUniform4fv(int location, int count, FloatBuffer v);

		public void glUniform4fv(int location, int count, float[] v, int offset);

		public void glUniform4i(int location, int x, int y, int z, int w);

		public void glUniform4iv(int location, int count, IntBuffer v);

		public void glUniform4iv(int location, int count, int[] v, int offset);

		public void glUniformMatrix2fv(int location, int count, bool transpose, FloatBuffer value);

		public void glUniformMatrix2fv(int location, int count, bool transpose, float[] value, int offset);

		public void glUniformMatrix3fv(int location, int count, bool transpose, FloatBuffer value);

		public void glUniformMatrix3fv(int location, int count, bool transpose, float[] value, int offset);

		public void glUniformMatrix4fv(int location, int count, bool transpose, FloatBuffer value);

		public void glUniformMatrix4fv(int location, int count, bool transpose, float[] value, int offset);

		public void glUseProgram(int program);

		public void glValidateProgram(int program);

		public void glVertexAttrib1f(int indx, float x);

		public void glVertexAttrib1fv(int indx, FloatBuffer values);

		public void glVertexAttrib2f(int indx, float x, float y);

		public void glVertexAttrib2fv(int indx, FloatBuffer values);

		public void glVertexAttrib3f(int indx, float x, float y, float z);

		public void glVertexAttrib3fv(int indx, FloatBuffer values);

		public void glVertexAttrib4f(int indx, float x, float y, float z, float w);

		public void glVertexAttrib4fv(int indx, FloatBuffer values);

		/** In OpenGl core profiles (3.1+), passing a pointer to client memory is not valid. In 3.0 and later, use the other version of
		 * this function instead, pass a zero-based offset which references the buffer currently bound to GL_ARRAY_BUFFER. */
		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, Buffer ptr);

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, byte[] ptr);

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, float[] ptr);

		public void glVertexAttribPointer(int indx, int size, int type, bool normalized, int stride, int ptr);
	}
}
