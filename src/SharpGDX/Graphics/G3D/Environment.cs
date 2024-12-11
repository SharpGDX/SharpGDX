using System;
using SharpGDX.Graphics.G3D.Attributess;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Shims;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.G3D.Environments;

namespace SharpGDX.Graphics.G3D;

public class Environment : Attributes {

	/** Shadow map used to render shadows */
	public ShadowMap shadowMap;

	public Environment () {
	}

	public Environment add ( BaseLight[] lights) {
		foreach (BaseLight light in lights)
			add(light);
		return this;
	}

	public Environment add (Array<BaseLight> lights) {
		foreach (BaseLight light in lights)
			add(light);
		return this;
	}

	public Environment add (BaseLight light) {
		if (light is DirectionalLight)
			add((DirectionalLight)light);
		else if (light is PointLight) {
			add((PointLight)light);
		} else if (light is SpotLight)
			add((SpotLight)light);
		else
			throw new GdxRuntimeException("Unknown light type");
		return this;
	}

	public Environment add (DirectionalLight light) {
		DirectionalLightsAttribute dirLights = ((DirectionalLightsAttribute)get(DirectionalLightsAttribute.Type));
		if (dirLights == null) set(dirLights = new DirectionalLightsAttribute());
		dirLights.lights.Add(light);
		return this;
	}

	public Environment add (PointLight light) {
		PointLightsAttribute pointLights = ((PointLightsAttribute)get(PointLightsAttribute.Type));
		if (pointLights == null) set(pointLights = new PointLightsAttribute());
		pointLights.lights.Add(light);
		return this;
	}

	public Environment add (SpotLight light) {
		SpotLightsAttribute spotLights = ((SpotLightsAttribute)get(SpotLightsAttribute.Type));
		if (spotLights == null) set(spotLights = new SpotLightsAttribute());
		spotLights.lights.Add(light);
		return this;
	}

	public Environment remove (BaseLight[] lights) {
		foreach (BaseLight light in lights)
			remove(light);
		return this;
	}

	public Environment remove (Array<BaseLight> lights) {
		foreach ( BaseLight light in lights)
			remove(light);
		return this;
	}

	public Environment remove (BaseLight light) {
		if (light is DirectionalLight)
			remove((DirectionalLight)light);
		else if (light is PointLight)
			remove((PointLight)light);
		else if (light is SpotLight)
			remove((SpotLight)light);
		else
			throw new GdxRuntimeException("Unknown light type");
		return this;
	}

	public Environment remove (DirectionalLight light) {
		if (has(DirectionalLightsAttribute.Type)) {
			DirectionalLightsAttribute dirLights = ((DirectionalLightsAttribute)get(DirectionalLightsAttribute.Type));
			dirLights.lights.RemoveValue(light, false);
			if (dirLights.lights.size == 0) remove(DirectionalLightsAttribute.Type);
		}
		return this;
	}

	public Environment remove (PointLight light) {
		if (has(PointLightsAttribute.Type)) {
			PointLightsAttribute pointLights = ((PointLightsAttribute)get(PointLightsAttribute.Type));
			pointLights.lights.RemoveValue(light, false);
			if (pointLights.lights.size == 0) remove(PointLightsAttribute.Type);
		}
		return this;
	}

	public Environment remove (SpotLight light) {
		if (has(SpotLightsAttribute.Type)) {
			SpotLightsAttribute spotLights = ((SpotLightsAttribute)get(SpotLightsAttribute.Type));
			spotLights.lights.RemoveValue(light, false);
			if (spotLights.lights.size == 0) remove(SpotLightsAttribute.Type);
		}
		return this;
	}
}
