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

/** Encapsulate the formulas to spawn a particle on a mesh shape.
 * @author Inferno */
public sealed class UnweightedMeshSpawnShapeValue : MeshSpawnShapeValue {
	private float[] vertices;
	private short[] indices;
	private int positionOffset, vertexSize, vertexCount, triangleCount;

	public UnweightedMeshSpawnShapeValue (UnweightedMeshSpawnShapeValue value) 
    : base(value)
    {
		
		load(value);
	}

	public UnweightedMeshSpawnShapeValue () {
	}

	public override void setMesh (Mesh mesh, Model model) {
		base.setMesh(mesh, model);
		vertexSize = mesh.getVertexSize() / 4;
		positionOffset = mesh.getVertexAttribute(Usage.Position).offset / 4;
		int indicesCount = mesh.getNumIndices();
		if (indicesCount > 0) {
			indices = new short[indicesCount];
			mesh.getIndices(indices);
			triangleCount = indices.Length / 3;
		} else
			indices = null;
		vertexCount = mesh.getNumVertices();
		vertices = new float[vertexCount * vertexSize];
		mesh.getVertices(vertices);
	}

    public override void spawnAux (Vector3 vector, float percent) {
		if (indices == null) {
			// Triangles
			int triangleIndex = MathUtils.random(vertexCount - 3) * vertexSize;
			int p1Offset = triangleIndex + positionOffset, p2Offset = p1Offset + vertexSize, p3Offset = p2Offset + vertexSize;
			float x1 = vertices[p1Offset], y1 = vertices[p1Offset + 1], z1 = vertices[p1Offset + 2], x2 = vertices[p2Offset],
				y2 = vertices[p2Offset + 1], z2 = vertices[p2Offset + 2], x3 = vertices[p3Offset], y3 = vertices[p3Offset + 1],
				z3 = vertices[p3Offset + 2];
			Triangle.pick(x1, y1, z1, x2, y2, z2, x3, y3, z3, vector);
		} else {
			// Indices
			int triangleIndex = MathUtils.random(triangleCount - 1) * 3;
			int p1Offset = indices[triangleIndex] * vertexSize + positionOffset,
				p2Offset = indices[triangleIndex + 1] * vertexSize + positionOffset,
				p3Offset = indices[triangleIndex + 2] * vertexSize + positionOffset;
			float x1 = vertices[p1Offset], y1 = vertices[p1Offset + 1], z1 = vertices[p1Offset + 2], x2 = vertices[p2Offset],
				y2 = vertices[p2Offset + 1], z2 = vertices[p2Offset + 2], x3 = vertices[p3Offset], y3 = vertices[p3Offset + 1],
				z3 = vertices[p3Offset + 2];
			Triangle.pick(x1, y1, z1, x2, y2, z2, x3, y3, z3, vector);
		}
	}

    public override SpawnShapeValue copy () {
		return new UnweightedMeshSpawnShapeValue(this);
	}

}
