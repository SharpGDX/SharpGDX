﻿using SharpGDX.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;

namespace SharpGDX.Graphics.GLUtils
{
	/**
 * <p>
 * A shader program encapsulates a vertex and fragment shader pair linked to form a shader program.
 * </p>
 *
 * <p>
 * After construction a ShaderProgram can be used to draw {@link Mesh}. To make the GPU use a specific ShaderProgram the programs
 * {@link ShaderProgram#bind()} method must be used which effectively binds the program.
 * </p>
 *
 * <p>
 * When a ShaderProgram is bound one can set uniforms, vertex attributes and attributes as needed via the respective methods.
 * </p>
 *
 * <p>
 * A ShaderProgram must be disposed via a call to {@link ShaderProgram#dispose()} when it is no longer needed
 * </p>
 *
 * <p>
 * ShaderPrograms are managed. In case the OpenGL context is lost all shaders get invalidated and have to be reloaded. This
 * happens on Android when a user switches to another application or receives an incoming call. Managed ShaderPrograms are
 * automatically reloaded when the OpenGL context is recreated so you don't have to do this manually.
 * </p>
 *
 * @author mzechner */
public class ShaderProgram : IDisposable {
	/** default name for position attributes **/
	public static readonly String POSITION_ATTRIBUTE = "a_position";
	/** default name for normal attributes **/
	public static readonly String NORMAL_ATTRIBUTE = "a_normal";
	/** default name for color attributes **/
	public static readonly String COLOR_ATTRIBUTE = "a_color";
	/** default name for texcoords attributes, append texture unit number **/
	public static readonly String TEXCOORD_ATTRIBUTE = "a_texCoord";
	/** default name for tangent attribute **/
	public static readonly String TANGENT_ATTRIBUTE = "a_tangent";
	/** default name for binormal attribute **/
	public static readonly String BINORMAL_ATTRIBUTE = "a_binormal";
	/** default name for boneweight attribute **/
	public static readonly String BONEWEIGHT_ATTRIBUTE = "a_boneWeight";

	/** flag indicating whether attributes & uniforms must be present at all times **/
	public static bool pedantic = true;

	/** code that is always added to the vertex shader code, typically used to inject a #version line. Note that this is added
	 * as-is, you should include a newline (`\n`) if needed. */
	public static String prependVertexCode = "";

	/** code that is always added to every fragment shader code, typically used to inject a #version line. Note that this is added
	 * as-is, you should include a newline (`\n`) if needed. */
	public static String prependFragmentCode = "";

	/** the list of currently available shaders **/
	private readonly static ObjectMap<IApplication, Array<ShaderProgram>> shaders = new ObjectMap<IApplication, Array<ShaderProgram>>();

	/** the log **/
	private String log = "";

	/** whether this program compiled successfully **/
	private bool _isCompiled;

	/** uniform lookup **/
	private readonly ObjectIntMap<String> uniforms = new ObjectIntMap<String>();

	/** uniform types **/
	private readonly ObjectIntMap<String> uniformTypes = new ObjectIntMap<String>();

	/** uniform sizes **/
	private readonly ObjectIntMap<String> uniformSizes = new ObjectIntMap<String>();

	/** uniform names **/
	private String[] uniformNames;

	/** attribute lookup **/
	private readonly ObjectIntMap<String> attributes = new ObjectIntMap<String>();

	/** attribute types **/
	private readonly ObjectIntMap<String> attributeTypes = new ObjectIntMap<String>();

	/** attribute sizes **/
	private readonly ObjectIntMap<String> attributeSizes = new ObjectIntMap<String>();

	/** attribute names **/
	private String[] attributeNames;

	/** program handle **/
	private int program;

	/** vertex shader handle **/
	private int vertexShaderHandle;

	/** fragment shader handle **/
	private int fragmentShaderHandle;

	/** matrix float buffer **/
	private readonly FloatBuffer matrix;

	/** vertex shader source **/
	private readonly String vertexShaderSource;

	/** fragment shader source **/
	private readonly String fragmentShaderSource;

	/** whether this shader was invalidated **/
	private bool invalidated;

	/** reference count **/
	private int refCount = 0;

	/** Constructs a new ShaderProgram and immediately compiles it.
	 *
	 * @param vertexShader the vertex shader
	 * @param fragmentShader the fragment shader */

	public ShaderProgram (String vertexShader, String fragmentShader) {
		if (vertexShader == null) throw new IllegalArgumentException("vertex shader must not be null");
		if (fragmentShader == null) throw new IllegalArgumentException("fragment shader must not be null");

		if (prependVertexCode != null && prependVertexCode.Length > 0) vertexShader = prependVertexCode + vertexShader;
		if (prependFragmentCode != null && prependFragmentCode.Length > 0) fragmentShader = prependFragmentCode + fragmentShader;

		this.vertexShaderSource = vertexShader;
		this.fragmentShaderSource = fragmentShader;
		this.matrix = FloatBuffer.allocate(16); // TODO: BufferUtils.newFloatBuffer(16);

		compileShaders(vertexShader, fragmentShader);
		if (isCompiled()) {
			fetchAttributes();
			fetchUniforms();
			addManagedShader(GDX.App, this);
		}
	}

	public ShaderProgram (FileHandle vertexShader, FileHandle fragmentShader) 
	: this(vertexShader.readString(), fragmentShader.readString())
	{
		
	}

	/** Loads and compiles the shaders, creates a new program and links the shaders.
	 *
	 * @param vertexShader
	 * @param fragmentShader */
	private void compileShaders (String vertexShader, String fragmentShader) {
		vertexShaderHandle = loadShader(IGL20.GL_VERTEX_SHADER, vertexShader);
		fragmentShaderHandle = loadShader(IGL20.GL_FRAGMENT_SHADER, fragmentShader);

		if (vertexShaderHandle == -1 || fragmentShaderHandle == -1) {
			_isCompiled = false;
			return;
		}

		program = linkProgram(createProgram());
		if (program == -1) {
			_isCompiled = false;
			return;
		}

		_isCompiled = true;
	}

	private int loadShader (int type, String source) {
		IGL20 gl = GDX.GL20;
		IntBuffer intbuf = IntBuffer.allocate(1); // TODO: BufferUtils.newIntBuffer(1);

		int shader = gl.glCreateShader(type);

		if (shader == 0) return -1;

		gl.glShaderSource(shader, source);
		gl.glCompileShader(shader);
		gl.glGetShaderiv(shader, IGL20.GL_COMPILE_STATUS, intbuf);

		int compiled = intbuf.get(0);
		if (compiled == 0) {
// gl.glGetShaderiv(shader, GL20.GL_INFO_LOG_LENGTH, intbuf);
// int infoLogLength = intbuf.get(0);
// if (infoLogLength > 1) {
			String infoLog = gl.glGetShaderInfoLog(shader);
			log += type == IGL20.GL_VERTEX_SHADER ? "Vertex shader\n" : "Fragment shader:\n";
			log += infoLog;
// }
			return -1;
		}

		return shader;
	}

	protected int createProgram () {
		IGL20 gl = GDX.GL20;
		int program = gl.glCreateProgram();
		return program != 0 ? program : -1;
	}

	private int linkProgram (int program) {
		IGL20 gl = GDX.GL20;
		if (program == -1) return -1;

		gl.glAttachShader(program, vertexShaderHandle);
		gl.glAttachShader(program, fragmentShaderHandle);
		gl.glLinkProgram(program);

		ByteBuffer tmp = ByteBuffer.allocate(4); // TODO: ByteBuffer.allocateDirect(4);
		tmp.order(ByteOrder.nativeOrder());
		IntBuffer intbuf = tmp.asIntBuffer();

		gl.glGetProgramiv(program, IGL20.GL_LINK_STATUS, intbuf);
		int linked = intbuf.get(0);
		if (linked == 0) {
// Gdx.gl20.glGetProgramiv(program, GL20.GL_INFO_LOG_LENGTH, intbuf);
// int infoLogLength = intbuf.get(0);
// if (infoLogLength > 1) {
			log = GDX.GL20.glGetProgramInfoLog(program);
// }
			return -1;
		}

		return program;
	}

	readonly static IntBuffer intbuf = IntBuffer.allocate(1); // TODO: BufferUtils.newIntBuffer(1);

	/** @return the log info for the shader compilation and program linking stage. The shader needs to be bound for this method to
	 *         have an effect. */
	public String getLog () {
		if (_isCompiled) {
// Gdx.gl20.glGetProgramiv(program, GL20.GL_INFO_LOG_LENGTH, intbuf);
// int infoLogLength = intbuf.get(0);
// if (infoLogLength > 1) {
			log = GDX.GL20.glGetProgramInfoLog(program);
// }
			return log;
		} else {
			return log;
		}
	}

	/** @return whether this ShaderProgram compiled successfully. */
	public bool isCompiled () {
		return _isCompiled;
	}

	private int fetchAttributeLocation (String name) {
		IGL20 gl = GDX.GL20;
		// -2 == not yet cached
		// -1 == cached but not found
		int location;
		if ((location = attributes.get(name, -2)) == -2) {
			location = gl.glGetAttribLocation(program, name);
			attributes.put(name, location);
		}
		return location;
	}

	private int fetchUniformLocation (String name) {
		return fetchUniformLocation(name, pedantic);
	}

	public int fetchUniformLocation (String name, bool pedantic) {
		// -2 == not yet cached
		// -1 == cached but not found
		int location;
		if ((location = uniforms.get(name, -2)) == -2) {
			location = GDX.GL20.glGetUniformLocation(program, name);
			if (location == -1 && pedantic) {
				if (_isCompiled) throw new IllegalArgumentException("No uniform with name '" + name + "' in shader");
				throw new IllegalStateException("An attempted fetch uniform from uncompiled shader \n" + getLog());
			}
			uniforms.put(name, location);
		}
		return location;
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value the value */
	public void setUniformi (String name, int value) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform1i(location, value);
	}

	public void setUniformi (int location, int value) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform1i(location, value);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value1 the first value
	 * @param value2 the second value */
	public void setUniformi (String name, int value1, int value2) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform2i(location, value1, value2);
	}

	public void setUniformi (int location, int value1, int value2) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform2i(location, value1, value2);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value1 the first value
	 * @param value2 the second value
	 * @param value3 the third value */
	public void setUniformi (String name, int value1, int value2, int value3) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform3i(location, value1, value2, value3);
	}

	public void setUniformi (int location, int value1, int value2, int value3) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform3i(location, value1, value2, value3);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value1 the first value
	 * @param value2 the second value
	 * @param value3 the third value
	 * @param value4 the fourth value */
	public void setUniformi (String name, int value1, int value2, int value3, int value4) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform4i(location, value1, value2, value3, value4);
	}

	public void setUniformi (int location, int value1, int value2, int value3, int value4) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform4i(location, value1, value2, value3, value4);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value the value */
	public void setUniformf (String name, float value) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform1f(location, value);
	}

	public void setUniformf (int location, float value) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform1f(location, value);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value1 the first value
	 * @param value2 the second value */
	public void setUniformf (String name, float value1, float value2) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform2f(location, value1, value2);
	}

	public void setUniformf (int location, float value1, float value2) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform2f(location, value1, value2);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value1 the first value
	 * @param value2 the second value
	 * @param value3 the third value */
	public void setUniformf (String name, float value1, float value2, float value3) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform3f(location, value1, value2, value3);
	}

	public void setUniformf (int location, float value1, float value2, float value3) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform3f(location, value1, value2, value3);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param value1 the first value
	 * @param value2 the second value
	 * @param value3 the third value
	 * @param value4 the fourth value */
	public void setUniformf (String name, float value1, float value2, float value3, float value4) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform4f(location, value1, value2, value3, value4);
	}

	public void setUniformf (int location, float value1, float value2, float value3, float value4) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform4f(location, value1, value2, value3, value4);
	}

	public void setUniform1fv (String name, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform1fv(location, length, values, offset);
	}

	public void setUniform1fv (int location, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform1fv(location, length, values, offset);
	}

	public void setUniform2fv (String name, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform2fv(location, length / 2, values, offset);
	}

	public void setUniform2fv (int location, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform2fv(location, length / 2, values, offset);
	}

	public void setUniform3fv (String name, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform3fv(location, length / 3, values, offset);
	}

	public void setUniform3fv (int location, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform3fv(location, length / 3, values, offset);
	}

	public void setUniform4fv (String name, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchUniformLocation(name);
		gl.glUniform4fv(location, length / 4, values, offset);
	}

	public void setUniform4fv (int location, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniform4fv(location, length / 4, values, offset);
	}

	/** Sets the uniform matrix with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param matrix the matrix */
	public void setUniformMatrix (String name, Matrix4 matrix) {
		setUniformMatrix(name, matrix, false);
	}

	/** Sets the uniform matrix with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param matrix the matrix
	 * @param transpose whether the matrix should be transposed */
	public void setUniformMatrix (String name, Matrix4 matrix, bool transpose) {
		setUniformMatrix(fetchUniformLocation(name), matrix, transpose);
	}

	public void setUniformMatrix (int location, Matrix4 matrix) {
		setUniformMatrix(location, matrix, false);
	}

	public void setUniformMatrix (int location, Matrix4 matrix, bool transpose) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniformMatrix4fv(location, 1, transpose, matrix.val, 0);
	}

	/** Sets the uniform matrix with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param matrix the matrix */
	public void setUniformMatrix (String name, Matrix3 matrix) {
		setUniformMatrix(name, matrix, false);
	}

	/** Sets the uniform matrix with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param matrix the matrix
	 * @param transpose whether the uniform matrix should be transposed */
	public void setUniformMatrix (String name, Matrix3 matrix, bool transpose) {
		setUniformMatrix(fetchUniformLocation(name), matrix, transpose);
	}

	public void setUniformMatrix (int location, Matrix3 matrix) {
		setUniformMatrix(location, matrix, false);
	}

	public void setUniformMatrix (int location, Matrix3 matrix, bool transpose) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniformMatrix3fv(location, 1, transpose, matrix.val, 0);
	}

	/** Sets an array of uniform matrices with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param buffer buffer containing the matrix data
	 * @param transpose whether the uniform matrix should be transposed */
	public void setUniformMatrix3fv (String name, FloatBuffer buffer, int count, bool transpose) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		((Buffer)buffer).position(0);
		int location = fetchUniformLocation(name);
		gl.glUniformMatrix3fv(location, count, transpose, buffer);
	}

	/** Sets an array of uniform matrices with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param buffer buffer containing the matrix data
	 * @param transpose whether the uniform matrix should be transposed */
	public void setUniformMatrix4fv (String name, FloatBuffer buffer, int count, bool transpose) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		((Buffer)buffer).position(0);
		int location = fetchUniformLocation(name);
		gl.glUniformMatrix4fv(location, count, transpose, buffer);
	}

	public void setUniformMatrix4fv (int location, float[] values, int offset, int length) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUniformMatrix4fv(location, length / 16, false, values, offset);
	}

	public void setUniformMatrix4fv (String name, float[] values, int offset, int length) {
		setUniformMatrix4fv(fetchUniformLocation(name), values, offset, length);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param values x and y as the first and second values respectively */
	public void setUniformf (String name, Vector2 values) {
		setUniformf(name, values.x, values.y);
	}

	public void setUniformf (int location, Vector2 values) {
		setUniformf(location, values.x, values.y);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param values x, y and z as the first, second and third values respectively */
	public void setUniformf (String name, Vector3 values) {
		setUniformf(name, values.x, values.y, values.z);
	}

	public void setUniformf (int location, Vector3 values) {
		setUniformf(location, values.x, values.y, values.z);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param values x, y, z, and w as the first, second, third, and fourth values respectively */
	public void setUniformf (String name, Vector4 values) {
		setUniformf(name, values.x, values.y, values.z, values.w);
	}

	public void setUniformf (int location, Vector4 values) {
		setUniformf(location, values.x, values.y, values.z, values.w);
	}

	/** Sets the uniform with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the name of the uniform
	 * @param values r, g, b and a as the first through fourth values respectively */
	public void setUniformf (String name, Color values) {
		setUniformf(name, values.R, values.G, values.B, values.A);
	}

	public void setUniformf (int location, Color values) {
		setUniformf(location, values.R, values.G, values.B, values.A);
	}

	/** Sets the vertex attribute with the given name. The {@link ShaderProgram} must be bound for this to work.
	 *
	 * @param name the attribute name
	 * @param size the number of components, must be >= 1 and <= 4
	 * @param type the type, must be one of GL20.GL_BYTE, GL20.GL_UNSIGNED_BYTE, GL20.GL_SHORT,
	 *           GL20.GL_UNSIGNED_SHORT,GL20.GL_FIXED, or GL20.GL_FLOAT. GL_FIXED will not work on the desktop
	 * @param normalize whether fixed point data should be normalized. Will not work on the desktop
	 * @param stride the stride in bytes between successive attributes
	 * @param buffer the buffer containing the vertex attributes. */
	public void setVertexAttribute (String name, int size, int type, bool normalize, int stride, Buffer buffer) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchAttributeLocation(name);
		if (location == -1) return;
		gl.glVertexAttribPointer(location, size, type, normalize, stride, buffer);
	}

	public void setVertexAttribute (int location, int size, int type, bool normalize, int stride, Buffer buffer) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glVertexAttribPointer(location, size, type, normalize, stride, buffer);
	}

	public void setVertexAttribute(int location, int size, int type, bool normalize, int stride, byte[] buffer)
	{
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glVertexAttribPointer(location, size, type, normalize, stride, buffer);
	}

	public void setVertexAttribute(int location, int size, int type, bool normalize, int stride, float[] buffer)
	{
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glVertexAttribPointer(location, size, type, normalize, stride, buffer);
	}

		/** Sets the vertex attribute with the given name. The {@link ShaderProgram} must be bound for this to work.
			 *
			 * @param name the attribute name
			 * @param size the number of components, must be >= 1 and <= 4
			 * @param type the type, must be one of GL20.GL_BYTE, GL20.GL_UNSIGNED_BYTE, GL20.GL_SHORT,
			 *           GL20.GL_UNSIGNED_SHORT,GL20.GL_FIXED, or GL20.GL_FLOAT. GL_FIXED will not work on the desktop
			 * @param normalize whether fixed point data should be normalized. Will not work on the desktop
			 * @param stride the stride in bytes between successive attributes
			 * @param offset byte offset into the vertex buffer object bound to GL20.GL_ARRAY_BUFFER. */
		public void setVertexAttribute (String name, int size, int type, bool normalize, int stride, int offset) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchAttributeLocation(name);
		if (location == -1) return;
		gl.glVertexAttribPointer(location, size, type, normalize, stride, offset);
	}

	public void setVertexAttribute (int location, int size, int type, bool normalize, int stride, int offset) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glVertexAttribPointer(location, size, type, normalize, stride, offset);
	}

	public void bind () {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glUseProgram(program);
	}

	/** Disposes all resources associated with this shader. Must be called when the shader is no longer used. */
	public void Dispose () {
		IGL20 gl = GDX.GL20;
		gl.glUseProgram(0);
		gl.glDeleteShader(vertexShaderHandle);
		gl.glDeleteShader(fragmentShaderHandle);
		gl.glDeleteProgram(program);
		if (shaders.get(GDX.App) != null) shaders.get(GDX.App).RemoveValue(this, true);
	}

	/** Disables the vertex attribute with the given name
	 *
	 * @param name the vertex attribute name */
	public void disableVertexAttribute (String name) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchAttributeLocation(name);
		if (location == -1) return;
		gl.glDisableVertexAttribArray(location);
	}

	public void disableVertexAttribute (int location) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glDisableVertexAttribArray(location);
	}

	/** Enables the vertex attribute with the given name
	 *
	 * @param name the vertex attribute name */
	public void enableVertexAttribute (String name) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		int location = fetchAttributeLocation(name);
		if (location == -1) return;
		gl.glEnableVertexAttribArray(location);
	}

	public void enableVertexAttribute (int location) {
		IGL20 gl = GDX.GL20;
		checkManaged();
		gl.glEnableVertexAttribArray(location);
	}

	private void checkManaged () {
		if (invalidated) {
			compileShaders(vertexShaderSource, fragmentShaderSource);
			invalidated = false;
		}
	}

	private void addManagedShader (IApplication app, ShaderProgram shaderProgram) {
		Array<ShaderProgram> managedResources = shaders.get(app);
		if (managedResources == null) managedResources = new Array<ShaderProgram>();
		managedResources.Add(shaderProgram);
		shaders.put(app, managedResources);
	}

	/** Invalidates all shaders so the next time they are used new handles are generated
	 * @param app */
	public static void invalidateAllShaderPrograms (IApplication app) {
		if (GDX.GL20 == null) return;

		Array<ShaderProgram> shaderArray = shaders.get(app);
		if (shaderArray == null) return;

		for (int i = 0; i < shaderArray.size; i++) {
			shaderArray.Get(i).invalidated = true;
			shaderArray.Get(i).checkManaged();
		}
	}

	public static void clearAllShaderPrograms (IApplication app) {
		shaders.remove(app);
	}

	public static String getManagedStatus () {
		StringBuilder builder = new StringBuilder();
		int i = 0;
		builder.Append("Managed shaders/app: { ");
		foreach (IApplication app in shaders.keys()) {
			builder.Append(shaders.get(app).size);
			builder.Append(" ");
		}
		builder.Append("}");
		return builder.ToString();
	}

	/** @return the number of managed shader programs currently loaded */
	public static int getNumManagedShaderPrograms () {
		return shaders.get(GDX.App).size;
	}

	/** Sets the given attribute
	 *
	 * @param name the name of the attribute
	 * @param value1 the first value
	 * @param value2 the second value
	 * @param value3 the third value
	 * @param value4 the fourth value */
	public void setAttributef (String name, float value1, float value2, float value3, float value4) {
		IGL20 gl = GDX.GL20;
		int location = fetchAttributeLocation(name);
		gl.glVertexAttrib4f(location, value1, value2, value3, value4);
	}

	private IntBuffer @params = IntBuffer.allocate(1); // TODO: BufferUtils.newIntBuffer(1);
	private IntBuffer type = IntBuffer.allocate(1); // TODO: BufferUtils.newIntBuffer(1);

	private void fetchUniforms () {
		((Buffer)@params).clear();
		GDX.GL20.glGetProgramiv(program, IGL20.GL_ACTIVE_UNIFORMS, @params);
		int numUniforms = @params.get(0);

		uniformNames = new String[numUniforms];

		for (int i = 0; i < numUniforms; i++) {
			((Buffer)@params).clear();
			@params.put(0, 1);
			((Buffer)type).clear();
			String name = GDX.GL20.glGetActiveUniform(program, i, @params, type);
			int location = GDX.GL20.glGetUniformLocation(program, name);
			uniforms.put(name, location);
			uniformTypes.put(name, type.get(0));
			uniformSizes.put(name, @params.get(0));
			uniformNames[i] = name;
		}
	}

	private void fetchAttributes () {
		((Buffer)@params).clear();
		GDX.GL20.glGetProgramiv(program, IGL20.GL_ACTIVE_ATTRIBUTES, @params);
		int numAttributes = @params.get(0);

		attributeNames = new String[numAttributes];

		for (int i = 0; i < numAttributes; i++) {
			((Buffer)@params).clear();
			@params.put(0, 1);
			((Buffer)type).clear();
			String name = GDX.GL20.glGetActiveAttrib(program, i, @params, type);
			int location = GDX.GL20.glGetAttribLocation(program, name);
			attributes.put(name, location);
			attributeTypes.put(name, type.get(0));
			attributeSizes.put(name, @params.get(0));
			attributeNames[i] = name;
		}
	}

	/** @param name the name of the attribute
	 * @return whether the attribute is available in the shader */
	public bool hasAttribute (String name) {
		return attributes.containsKey(name);
	}

	/** @param name the name of the attribute
	 * @return the type of the attribute, one of {@link GL20#GL_FLOAT}, {@link GL20#GL_FLOAT_VEC2} etc. */
	public int getAttributeType (String name) {
		return attributeTypes.get(name, 0);
	}

	/** @param name the name of the attribute
	 * @return the location of the attribute or -1. */
	public int getAttributeLocation (String name) {
		return attributes.get(name, -1);
	}

	/** @param name the name of the attribute
	 * @return the size of the attribute or 0. */
	public int getAttributeSize (String name) {
		return attributeSizes.get(name, 0);
	}

	/** @param name the name of the uniform
	 * @return whether the uniform is available in the shader */
	public bool hasUniform (String name) {
		return uniforms.containsKey(name);
	}

	/** @param name the name of the uniform
	 * @return the type of the uniform, one of {@link GL20#GL_FLOAT}, {@link GL20#GL_FLOAT_VEC2} etc. */
	public int getUniformType (String name) {
		return uniformTypes.get(name, 0);
	}

	/** @param name the name of the uniform
	 * @return the location of the uniform or -1. */
	public int getUniformLocation (String name) {
		return uniforms.get(name, -1);
	}

	/** @param name the name of the uniform
	 * @return the size of the uniform or 0. */
	public int getUniformSize (String name) {
		return uniformSizes.get(name, 0);
	}

	/** @return the attributes */
	public String[] getAttributes () {
		return attributeNames;
	}

	/** @return the uniforms */
	public String[] getUniforms () {
		return uniformNames;
	}

	/** @return the source of the vertex shader */
	public String getVertexShaderSource () {
		return vertexShaderSource;
	}

	/** @return the source of the fragment shader */
	public String getFragmentShaderSource () {
		return fragmentShaderSource;
	}

	/** @return the handle of the shader program */
	public int getHandle () {
		return program;
	}
}
}
