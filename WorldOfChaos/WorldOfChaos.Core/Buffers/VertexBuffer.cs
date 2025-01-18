using OpenTK.Graphics.OpenGL4;

namespace WorldOfChaos.Core.Buffers;

public class VertexBuffer : Buffer
{
	public override BufferTarget Target => BufferTarget.ArrayBuffer;
	public Attribute[] Attributes { get; }
	public int Count { get; set; }


	public VertexBuffer(int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw, params Attribute[] attributes)
		: base(usage)
	{
		Count = count;
		Attributes = attributes;
	}

	public VertexBuffer(int size, int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw, params Attribute[] attributes)
		: this(count, usage, attributes)
	{
		Allocate(size);
	}

	public VertexBuffer(Array data, int size, int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw, params Attribute[] attributes)
		: this(count, usage, attributes)
	{
		Load(data, size);
	}

	public VertexBuffer(nint data, int size, int count = 0, BufferUsageHint usage = BufferUsageHint.DynamicCopy, params Attribute[] attributes)
		: this(count, usage, attributes)
	{
		Load(data, size);
	}

	public void CreateLayout(int vao, int index)
	{
		var stride = Attributes.Select(attrib => attrib.Size).Sum();

		for (int i = 0, offset = 0; i < Attributes.Length; i++)
		{
			Attributes[i].Offset = offset;
			Attributes[i].Stride = stride;
			offset += Attributes[i].Size;
		}

		GL.VertexArrayVertexBuffer(vao, index, Handle, nint.Zero, stride);

		foreach (var attribte in Attributes)
		{
			attribte.Load(vao, index);
		}
	}

	public class Attribute
	{
		public int Index { get; set; }
		public int Count { get; set; }
		public VertexAttribType Type { get; set; }
		public bool Normalized { get; set; }
		public int Stride { get; set; }
		public int Offset { get; set; }
		public int Size => Count * Sizes[Type];

		private static Dictionary<VertexAttribType, int> Sizes { get; } =
			new()
			{
				{ VertexAttribType.Byte, 1 },
				{ VertexAttribType.UnsignedByte, 1 },
				{ VertexAttribType.Short, 2 },
				{ VertexAttribType.UnsignedShort, 2 },
				{ VertexAttribType.HalfFloat, 2 },
				{ VertexAttribType.Int, 4 },
				{ VertexAttribType.UnsignedInt, 4 },
				{ VertexAttribType.Float, 4 },
				{ VertexAttribType.Fixed, 4 },
				{ VertexAttribType.Int2101010Rev, 4 },
				{ VertexAttribType.UnsignedInt2101010Rev, 4 },
				{ VertexAttribType.UnsignedInt10F11F11FRev, 4 },
				{ VertexAttribType.Double, 8 }
			};
		public Attribute(int index, int count, VertexAttribType type = VertexAttribType.Float, bool normalized = false, int stride = 0, int offset = 0)
		{
			Index = index;
			Count = count;
			Type = type;
			Normalized = normalized;
			Stride = stride;
			Offset = offset;
		}

		public virtual void Load(int vao, int index)
		{
			GL.EnableVertexArrayAttrib(vao, Index);
			GL.VertexArrayAttribBinding(vao, Index, index);
			GL.VertexArrayAttribFormat(vao, Index, Count, Type, Normalized, Offset);
		}
	}

	public class IntegerAttribute : Attribute
	{
		public IntegerAttribute(int index, int count, VertexAttribType type = VertexAttribType.Int, int stride = 0, int offset = 0)
			: base(index, count, type, false, stride, offset)
		{
		}

		public override void Load(int vao, int index)
		{
			GL.EnableVertexArrayAttrib(vao, Index);
			GL.VertexArrayAttribBinding(vao, Index, index);
			GL.VertexArrayAttribIFormat(vao, Index, Count, Type, Offset);
		}
	}
}