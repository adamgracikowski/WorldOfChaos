using OpenTK.Graphics.OpenGL4;
using WorldOfChaos.Core.Buffers;

namespace WorldOfChaos.Core.Models;

public class Plane(Mesh mesh, Material? material)
	: ModelBase(mesh, material)
{
	public static Mesh GenerateMesh()
	{
		var verticesAttribute = new VertexBuffer.Attribute(
			index: 0,
			count: 3,
			type: VertexAttribType.Float,
			stride: 5 * sizeof(float),
			offset: 0
		);

		var texturesAttribute = new VertexBuffer.Attribute(
			index: 1,
			count: 2,
			type: VertexAttribType.Float,
			stride: 5 * sizeof(float),
			offset: 3 * sizeof(float)
		);

		var vertexBuffer = new VertexBuffer(
			data: Vertices,
			size: Vertices.Length * sizeof(float),
			count: Vertices.Length / 5,
			usage: BufferUsageHint.StaticDraw,
			attributes: [verticesAttribute, texturesAttribute]
		);

		var indexBuffer = new IndexBuffer(
			data: Indices,
			size: Indices.Length * sizeof(byte),
			count: Indices.Length,
			elementsType: DrawElementsType.UnsignedByte
		);

		return new Mesh(nameof(Plane), PrimitiveType.Triangles, indexBuffer, vertexBuffer);
	}

	private static readonly float[] Vertices =
	[
		-0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
		 0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
		 0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
		-0.5f,  0.5f, 0.0f, 0.0f, 1.0f
	];

	private static readonly byte[] Indices =
	[
		0, 1, 2,
		2, 3, 0
	];
}