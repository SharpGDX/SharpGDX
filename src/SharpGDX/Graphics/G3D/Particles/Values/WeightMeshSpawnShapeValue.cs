using System;
using static SharpGDX.Graphics.VertexAttributes;
using SharpGDX.Assets;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;
using FloatChannel = SharpGDX.Graphics.G3D.Particles.ParallelArray.FloatChannel;

namespace SharpGDX.Graphics.G3D.Particles.Values;

/** Encapsulate the formulas to spawn a particle on a mesh shape dealing with not uniform area triangles.
 * @author Inferno */
public sealed class WeightMeshSpawnShapeValue : MeshSpawnShapeValue {

	private CumulativeDistribution<Triangle> distribution;

	public WeightMeshSpawnShapeValue (WeightMeshSpawnShapeValue value) 
    : base(value)
    {
        
		distribution = new CumulativeDistribution<Triangle>();
		load(value);
	}

	public WeightMeshSpawnShapeValue () 
    : base()
    {
		
		distribution = new CumulativeDistribution<Triangle>();
	}

	public override void init () {
		calculateWeights();
	}

	/** Calculate the weights of each triangle of the wrapped mesh. If the mesh has indices: the function will calculate the weight
	 * of those triangles. If the mesh has not indices: the function will consider the vertices as a triangle strip. */
	public void calculateWeights () {
		distribution.clear();
		VertexAttributes attributes = mesh.getVertexAttributes();
		int indicesCount = mesh.getNumIndices();
		int vertexCount = mesh.getNumVertices();
		int vertexSize = (short)(attributes.vertexSize / 4),
			positionOffset = (short)(attributes.findByUsage(Usage.Position).offset / 4);
		float[] vertices = new float[vertexCount * vertexSize];
		mesh.getVertices(vertices);
		if (indicesCount > 0) {
			short[] indices = new short[indicesCount];
			mesh.getIndices(indices);

			// Calculate the Area
			for (int i = 0; i < indicesCount; i += 3) {
				int p1Offset = indices[i] * vertexSize + positionOffset, p2Offset = indices[i + 1] * vertexSize + positionOffset,
					p3Offset = indices[i + 2] * vertexSize + positionOffset;
				float x1 = vertices[p1Offset], y1 = vertices[p1Offset + 1], z1 = vertices[p1Offset + 2], x2 = vertices[p2Offset],
					y2 = vertices[p2Offset + 1], z2 = vertices[p2Offset + 2], x3 = vertices[p3Offset], y3 = vertices[p3Offset + 1],
					z3 = vertices[p3Offset + 2];
				float area = Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2f);
				distribution.add(new Triangle(x1, y1, z1, x2, y2, z2, x3, y3, z3), area);
			}
		} else {
			// Calculate the Area
			for (int i = 0; i < vertexCount; i += vertexSize) {
				int p1Offset = i + positionOffset, p2Offset = p1Offset + vertexSize, p3Offset = p2Offset + vertexSize;
				float x1 = vertices[p1Offset], y1 = vertices[p1Offset + 1], z1 = vertices[p1Offset + 2], x2 = vertices[p2Offset],
					y2 = vertices[p2Offset + 1], z2 = vertices[p2Offset + 2], x3 = vertices[p3Offset], y3 = vertices[p3Offset + 1],
					z3 = vertices[p3Offset + 2];
				float area = Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2f);
				distribution.add(new Triangle(x1, y1, z1, x2, y2, z2, x3, y3, z3), area);
			}
		}

		// Generate cumulative distribution
		distribution.generateNormalized();
	}

	public override void spawnAux (Vector3 vector, float percent) {
		Triangle t = distribution.value();
		float a = MathUtils.random(), b = MathUtils.random();
		vector.Set(t.x1 + a * (t.x2 - t.x1) + b * (t.x3 - t.x1), t.y1 + a * (t.y2 - t.y1) + b * (t.y3 - t.y1),
			t.z1 + a * (t.z2 - t.z1) + b * (t.z3 - t.z1));
	}

    public override SpawnShapeValue copy () {
		return new WeightMeshSpawnShapeValue(this);
	}

}
