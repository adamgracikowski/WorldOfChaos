using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using WorldOfChaos.Core.Buffers;

namespace WorldOfChaos.Core.Models;

public sealed class Sphere(Mesh mesh, Material? material)
	: ModelBase(mesh, material)
{
	public void UpdatePositionAndRotation(float deltaTime, float speed)
	{
		var angle = speed * deltaTime;
		var rotationMatrix = Matrix4.CreateRotationY(angle);

		position = Vector3.TransformPosition(position, rotationMatrix);

		var directionToCenter = -position.Normalized();

		rotation = new Vector3(0, MathF.Atan2(directionToCenter.Z, directionToCenter.X), 0);

		UpdateModel();
	}

	protected override void UpdateModel()
	{
		base.UpdateModel();
		NormalMatrix = Matrix4.Transpose(Matrix4.Invert(Model));
	}

	public static Mesh GenerateMesh(int latitudeBands = 50, int longitudeBands = 50, float radius = 1.0f)
	{
		var vertexData = new List<float>();
		var indices = new List<int>();

		for (var latNumber = 0; latNumber <= latitudeBands; latNumber++)
		{
			var theta = latNumber * MathF.PI / latitudeBands;
			var sinTheta = MathF.Sin(theta);
			var cosTheta = MathF.Cos(theta);

			for (var longNumber = 0; longNumber <= longitudeBands; longNumber++)
			{
				var phi = longNumber * 2 * MathF.PI / longitudeBands;
				var sinPhi = MathF.Sin(phi);
				var cosPhi = MathF.Cos(phi);

				var vertex = new Vector3(
					cosPhi * sinTheta,
					cosTheta,
					sinPhi * sinTheta
				) * radius;

				var normal = Vector3.Normalize(vertex);

				vertexData.Add(vertex.X);
				vertexData.Add(vertex.Y);
				vertexData.Add(vertex.Z);
				vertexData.Add(normal.X);
				vertexData.Add(normal.Y);
				vertexData.Add(normal.Z);

				vertexData.Add((float)longNumber / longitudeBands);
				vertexData.Add(1.0f - (float)latNumber / latitudeBands);
			}
		}

		for (var latNumber = 0; latNumber < latitudeBands; latNumber++)
		{
			for (var longNumber = 0; longNumber < longitudeBands; longNumber++)
			{
				var first = latNumber * (longitudeBands + 1) + longNumber;
				var second = first + longitudeBands + 1;

				indices.Add(first);
				indices.Add(second);
				indices.Add(first + 1);

				indices.Add(second);
				indices.Add(second + 1);
				indices.Add(first + 1);
			}
		}

		var vertexArray = vertexData.ToArray();

		var verticesAttribute = new VertexBuffer.Attribute(
			index: 0,
			count: 3,
			type: VertexAttribType.Float,
			stride: 8 * sizeof(float),
			offset: 0
		);

		var normalsAttribute = new VertexBuffer.Attribute(
			index: 1,
			count: 3,
			type: VertexAttribType.Float,
			stride: 8 * sizeof(float),
			offset: 3 * sizeof(float)
		);

		var texturesAttribute = new VertexBuffer.Attribute(
			index: 2,
			count: 2,
			type: VertexAttribType.Float,
			stride: 8 * sizeof(float),
			offset: 6 * sizeof(float)
		);

		var vertexBuffer = new VertexBuffer(
			data: vertexArray,
			size: vertexArray.Length * sizeof(float),
			count: vertexArray.Length / 8,
			usage: BufferUsageHint.StaticDraw,
			attributes: [verticesAttribute, normalsAttribute, texturesAttribute]
		);

		var indexBuffer = new IndexBuffer(
			data: indices.ToArray(),
			size: indices.Count * sizeof(int),
			elementsType: DrawElementsType.UnsignedInt,
			count: indices.Count
		);

		var mesh = new Mesh(nameof(Cube), PrimitiveType.Triangles, indexBuffer, vertexBuffer);

		return mesh;
	}
}