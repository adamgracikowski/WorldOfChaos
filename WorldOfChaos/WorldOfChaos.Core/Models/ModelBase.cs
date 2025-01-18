using OpenTK.Mathematics;
using WorldOfChaos.Core.Buffers;

namespace WorldOfChaos.Core.Models;

public abstract class ModelBase(Mesh mesh, Material? material)
{
	public Mesh Mesh { get; protected set; } = mesh;
	public Material? Material { get; protected set; } = material;

	protected Vector3 position = Vector3.Zero;
	protected Vector3 rotation = Vector3.Zero;
	protected Vector3 scale = Vector3.One;

	public Matrix4 Model { get; protected set; } = Matrix4.Identity;
	public Matrix4 NormalMatrix { get; protected set; }
		= Matrix4.Transpose(Matrix4.Invert(Matrix4.Identity));

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

	protected virtual void UpdateModel()
	{
		Model = Matrix4.CreateScale(Scale) *
			Matrix4.CreateRotationX(Rotation.X) *
			Matrix4.CreateRotationY(Rotation.Y) *
			Matrix4.CreateRotationZ(Rotation.Z) *
			Matrix4.CreateTranslation(Position);
	}
}