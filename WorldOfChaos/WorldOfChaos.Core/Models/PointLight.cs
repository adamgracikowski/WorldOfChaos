using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using WorldOfChaos.Core.Buffers;

namespace WorldOfChaos.Core.Models;

public sealed class PointLight(Mesh mesh, Vector3 color)
{
	public Vector3 Color { get; private set; } = color;
	public Mesh Mesh { get; } = mesh;

	private Vector3 position = Vector3.Zero;
	private Vector3 rotation = Vector3.Zero;
	private Vector3 scale = Vector3.One;

	public Matrix4 Model { get; private set; } = Matrix4.Identity;

	public Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			var old = position;

			position = value;

			if (old != value)
			{
				UpdateModel();
			}
		}
	}

	public Vector3 Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			var old = rotation;

			rotation = value;

			if (old != value)
			{
				UpdateModel();
			}
		}
	}

	public Vector3 Scale
	{
		get
		{
			return scale;
		}
		set
		{
			var old = scale;

			scale = value;

			if (old != value)
			{
				UpdateModel();
			}
		}
	}

	private void UpdateModel()
	{
		Model = Matrix4.CreateScale(Scale) *
			Matrix4.CreateRotationX(Rotation.X) *
			Matrix4.CreateRotationY(Rotation.Y) *
			Matrix4.CreateRotationZ(Rotation.Z) *
			Matrix4.CreateTranslation(Position);
	}

	private static readonly Vector3[] Vertices =
	{
		new(-0.5f, -0.5f,  0.5f),
		new( 0.5f, -0.5f,  0.5f),
		new( 0.5f,  0.5f,  0.5f),
		new(-0.5f,  0.5f,  0.5f),
		new(-0.5f, -0.5f, -0.5f),
		new( 0.5f, -0.5f, -0.5f),
		new( 0.5f,  0.5f, -0.5f),
		new(-0.5f,  0.5f, -0.5f),
	};

	private static readonly uint[] Indices =
	{
		0, 1, 2,
		2, 3, 0,

		4, 5, 6,
		6, 7, 4,

		4, 0, 3,
		3, 7, 4,

		1, 5, 6,
		6, 2, 1,

		3, 2, 6,
		6, 7, 3,

		4, 5, 1,
		1, 0, 4
	};

	public static Mesh GenerateMesh()
	{
		var vertexAttribute = new VertexBuffer.Attribute(
			index: 0,
			count: 3,
			type: VertexAttribType.Float,
			stride: 0,
			offset: 0
		);

		var vertexBuffer = new VertexBuffer(
			data: Vertices,
			size: Vertices.Length * Vector3.SizeInBytes,
			count: Vertices.Length,
			BufferUsageHint.StaticDraw,
			attributes: [vertexAttribute]
		);

		var indexBuffer = new IndexBuffer(
			data: Indices,
			size: Indices.Length * sizeof(uint),
			count: Indices.Length,
			usage: BufferUsageHint.StaticDraw,
			elementsType: DrawElementsType.UnsignedInt
		);

		var mesh = new Mesh(
			nameof(PointLight),
			PrimitiveType.Triangles,
			indexBuffer,
			vertexBuffer
		);

		return mesh;
	}
}