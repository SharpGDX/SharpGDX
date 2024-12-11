using System;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Utils;


namespace SharpGDX.Graphics.G3D.Shaders;

/** @author Xoppa A BaseShader is a wrapper around a ShaderProgram that keeps track of the uniform and attribute locations. It
 *         does not manage the ShaderPogram, you are still responsible for disposing the ShaderProgram. */
public abstract class BaseShader : Shader {
	public interface Validator {
		/** @return True if the input is valid for the renderable, false otherwise. */
		bool validate (BaseShader shader, int inputID,  Renderable renderable);
	}

	public interface Setter {
		/** @return True if the uniform only has to be set once per render call, false if the uniform must be set for each
		 *         renderable. */
		bool isGlobal ( BaseShader shader,  int inputID);

		void set ( BaseShader shader,  int inputID,  Renderable renderable,  Attributes combinedAttributes);
	}

	public abstract  class GlobalSetter : Setter {
		public  bool isGlobal ( BaseShader shader,  int inputID) {
			return true;
		}

        public abstract void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes);
    }

	public abstract  class LocalSetter : Setter {
		public bool isGlobal ( BaseShader shader,  int inputID) {
			return false;
		}

        public abstract void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes);
    }

	public class Uniform : Validator {
		public readonly String alias;
		public readonly long materialMask;
		public readonly long environmentMask;
		public readonly long overallMask;

		public Uniform ( String alias,  long materialMask,  long environmentMask,  long overallMask) {
			this.alias = alias;
			this.materialMask = materialMask;
			this.environmentMask = environmentMask;
			this.overallMask = overallMask;
		}

		public Uniform ( String alias,  long materialMask,  long environmentMask) 
        : this(alias, materialMask, environmentMask, 0)
        {
			
		}

		public Uniform ( String alias,  long overallMask) 
        : this(alias, 0, 0, overallMask)
        {
			
		}

		public Uniform ( String alias) 
        : this(alias, 0, 0)
        {
			
		}

		public bool validate ( BaseShader shader,  int inputID,  Renderable renderable) {
			 long matFlags = (renderable != null && renderable.material != null) ? renderable.material.getMask() : 0;
			 long envFlags = (renderable != null && renderable.environment != null) ? renderable.environment.getMask() : 0;
			return ((matFlags & materialMask) == materialMask) && ((envFlags & environmentMask) == environmentMask)
				&& (((matFlags | envFlags) & overallMask) == overallMask);
		}
	}

	private readonly Array<String> uniforms = new Array<String>();
	private readonly Array<Validator> validators = new Array<Validator>();
	private readonly Array<Setter> setters = new Array<Setter>();
	private int[] locations;
	private readonly IntArray globalUniforms = new IntArray();
	private readonly IntArray localUniforms = new IntArray();
	private readonly IntIntMap attributes = new IntIntMap();
	private readonly IntIntMap instancedAttributes = new IntIntMap();

	public ShaderProgram program;
	public RenderContext context;
	public Camera camera;
	private Mesh currentMesh;

	/** Register an uniform which might be used by this shader. Only possible prior to the call to init().
	 * @return The ID of the uniform to use in this shader. */
	public int register ( String alias,  Validator validator,  Setter setter) {
		if (locations != null) throw new GdxRuntimeException("Cannot register an uniform after initialization");
		 int existing = getUniformID(alias);
		if (existing >= 0) {
			validators.set(existing, validator);
			setters.set(existing, setter);
			return existing;
		}
		uniforms.Add(alias);
		validators.Add(validator);
		setters.Add(setter);
		return uniforms.size - 1;
	}

	public int register ( String alias,  Validator validator) {
		return register(alias, validator, null);
	}

	public int register ( String alias,  Setter setter) {
		return register(alias, null, setter);
	}

	public int register ( String alias) {
		return register(alias, null, null);
	}

	public int register ( Uniform uniform,  Setter setter) {
		return register(uniform.alias, uniform, setter);
	}

	public int register ( Uniform uniform) {
		return register(uniform, null);
	}

	/** @return the ID of the input or negative if not available. */
	public int getUniformID ( String alias) {
		 int n = uniforms.size;
		for (int i = 0; i < n; i++)
			if (uniforms.Get(i).Equals(alias)) return i;
		return -1;
	}

	/** @return The input at the specified id. */
	public String getUniformAlias ( int id) {
		return uniforms.Get(id);
	}

	/** Initialize this shader, causing all registered uniforms/attributes to be fetched. */
	public virtual void init ( ShaderProgram program,  Renderable renderable) {
		if (locations != null) throw new GdxRuntimeException("Already initialized");
		if (!program.isCompiled()) throw new GdxRuntimeException(program.getLog());
		this.program = program;

		 int n = uniforms.size;
		locations = new int[n];
		for (int i = 0; i < n; i++) {
			 String input = uniforms.Get(i);
			 Validator validator = validators.Get(i);
			 Setter setter = setters.Get(i);
			if (validator != null && !validator.validate(this, i, renderable))
				locations[i] = -1;
			else {
				locations[i] = program.fetchUniformLocation(input, false);
				if (locations[i] >= 0 && setter != null) {
					if (setter.isGlobal(this, i))
						globalUniforms.add(i);
					else
						localUniforms.add(i);
				}
			}
			if (locations[i] < 0) {
				validators.set(i, null);
				setters.set(i, null);
			}
		}
		if (renderable != null) {
			 VertexAttributes attrs = renderable.meshPart.mesh.getVertexAttributes();
			 int c = attrs.Size();
			for (int i = 0; i < c; i++) {
				 VertexAttribute attr = attrs.Get(i);
				 int location = program.getAttributeLocation(attr.alias);
				if (location >= 0) attributes.put(attr.getKey(), location);
			}
			 VertexAttributes iattrs = renderable.meshPart.mesh.getInstancedAttributes();
			if (iattrs != null) {
				 int ic = iattrs.Size();
				for (int i = 0; i < ic; i++) {
					 VertexAttribute attr = iattrs.Get(i);
					 int location = program.getAttributeLocation(attr.alias);
					if (location >= 0) instancedAttributes.put(attr.getKey(), location);
				}
			}
		}
	}

    public abstract void init();

    public abstract int compareTo(Shader other);

    public abstract bool canRender(Renderable instance);

    public virtual void begin (Camera camera, RenderContext context) {
		this.camera = camera;
		this.context = context;
		program.bind();
		currentMesh = null;
		for (int u, i = 0; i < globalUniforms.size; ++i)
			if (setters.Get(u = globalUniforms.get(i)) != null) setters.Get(u).set(this, u, null, null);
	}

	private readonly IntArray tempArray = new IntArray();
	private readonly IntArray tempArray2 = new IntArray();

	private  int[] getAttributeLocations ( VertexAttributes attrs) {
		tempArray.clear();
		 int n = attrs.Size();
		for (int i = 0; i < n; i++) {
			tempArray.add(attributes.get(attrs.Get(i).getKey(), -1));
		}
		tempArray.shrink();
		return tempArray.items;
	}

	private  int[] getInstancedAttributeLocations ( VertexAttributes attrs) {
		// Instanced attributes may be null
		if (attrs == null) return null;
		tempArray2.clear();
		 int n = attrs.Size();
		for (int i = 0; i < n; i++) {
			tempArray2.add(instancedAttributes.get(attrs.Get(i).getKey(), -1));
		}
		tempArray2.shrink();
		return tempArray2.items;
	}

	private Attributes combinedAttributes = new Attributes();

	public virtual void render (Renderable renderable) {
		if (renderable.worldTransform.det3x3() == 0) return;
		combinedAttributes.clear();
		if (renderable.environment != null) combinedAttributes.set(renderable.environment);
		if (renderable.material != null) combinedAttributes.set(renderable.material);
		render(renderable, combinedAttributes);
	}

	public virtual void render (Renderable renderable,  Attributes combinedAttributes) {
		for (int u, i = 0; i < localUniforms.size; ++i)
			if (setters.Get(u = localUniforms.get(i)) != null) setters.Get(u).set(this, u, renderable, combinedAttributes);
		if (currentMesh != renderable.meshPart.mesh) {
			if (currentMesh != null) currentMesh.unbind(program, tempArray.items, tempArray2.items);
			currentMesh = renderable.meshPart.mesh;
			currentMesh.bind(program, getAttributeLocations(renderable.meshPart.mesh.getVertexAttributes()),
				getInstancedAttributeLocations(renderable.meshPart.mesh.getInstancedAttributes()));
		}
		renderable.meshPart.render(program, false);
	}

	public virtual void end () {
		if (currentMesh != null) {
			currentMesh.unbind(program, tempArray.items, tempArray2.items);
			currentMesh = null;
		}
	}

	public virtual void Dispose () {
		program = null;
		uniforms.clear();
		validators.clear();
		setters.clear();
		localUniforms.clear();
		globalUniforms.clear();
		locations = null;
	}

	/** Whether this Shader instance implements the specified uniform, only valid after a call to init(). */
	public  bool has ( int inputID) {
		return inputID >= 0 && inputID < locations.Length && locations[inputID] >= 0;
	}

	public  int loc ( int inputID) {
		return (inputID >= 0 && inputID < locations.Length) ? locations[inputID] : -1;
	}

	public  bool set ( int uniform,  Matrix4 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformMatrix(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  Matrix3 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformMatrix(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  Vector3 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  Vector2 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  Color value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  float value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  float v1,  float v2) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], v1, v2);
		return true;
	}

	public  bool set ( int uniform,  float v1,  float v2,  float v3) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], v1, v2, v3);
		return true;
	}

	public  bool set ( int uniform,  float v1,  float v2,  float v3,  float v4) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], v1, v2, v3, v4);
		return true;
	}

	public  bool set ( int uniform,  int value) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], value);
		return true;
	}

	public  bool set ( int uniform,  int v1,  int v2) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], v1, v2);
		return true;
	}

	public  bool set ( int uniform,  int v1,  int v2,  int v3) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], v1, v2, v3);
		return true;
	}

	public  bool set ( int uniform,  int v1,  int v2,  int v3,  int v4) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], v1, v2, v3, v4);
		return true;
	}

	public  bool set( int uniform,  TextureDescriptor textureDesc){
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], context.textureBinder.bind(textureDesc));
		return true;
	}

	public  bool set ( int uniform,  GLTexture texture) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], context.textureBinder.bind(texture));
		return true;
	}
}
