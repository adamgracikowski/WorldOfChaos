using OpenTK.Graphics.OpenGL4;

namespace WorldOfChaos.Core.Buffers;

public class IndexBuffer : Buffer
{
	public override BufferTarget Target => BufferTarget.ElementArrayBuffer;
	public DrawElementsType ElementsType { get; }
	public int Count { get; set; }

	public IndexBuffer(DrawElementsType elementsType, int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw)
		: base(usage)
	{
		ElementsType = elementsType;
		Count = count;
	}

	public IndexBuffer(int size, DrawElementsType elementsType, int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw)
		: this(elementsType, count, usage)
	{
		Allocate(size);
	}

	public IndexBuffer(Array data, int size, DrawElementsType elementsType, int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw)
		: this(elementsType, count, usage)
	{
		Load(data, size);
	}

	public IndexBuffer(nint data, int size, DrawElementsType elementsType, int count = 0, BufferUsageHint usage = BufferUsageHint.StaticDraw)
		: this(elementsType, count, usage)
	{
		Load(data, size);
	}
}