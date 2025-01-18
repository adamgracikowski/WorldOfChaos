using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using WorldOfChaos.Core.Interfaces;

namespace WorldOfChaos.Core.Buffers;

public abstract class Buffer : IBindable, IDisposable
{
	public abstract BufferTarget Target { get; }
	public BufferUsageHint Usage { get; protected set; }
	public int Handle { get; protected set; }

	public Buffer(BufferUsageHint usage)
	{
		GL.CreateBuffers(1, out int handle);
		Handle = handle;
		Usage = usage;
	}

	public void Allocate(int size)
	{
		GL.NamedBufferData(Handle, size, nint.Zero, Usage);
	}

	public void Load(Array data, int size)
	{
		var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		Load(gcHandle.AddrOfPinnedObject(), size);
		gcHandle.Free();
	}

	public void Load(nint data, int size)
	{
		GL.NamedBufferData(Handle, size, data, Usage);
	}

	public void Update(Array data, int dataOffset, int offset, int size)
	{
		var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		Update(gcHandle.AddrOfPinnedObject(), dataOffset, offset, size);
		gcHandle.Free();
	}

	public void Update(nint data, int dataOffset, int offset, int size)
	{
		GL.NamedBufferSubData(Handle, offset, size, data + dataOffset);
	}

	public nint Map(BufferAccess access = BufferAccess.ReadWrite)
	{
		return GL.MapNamedBuffer(Handle, access);
	}

	public void Unmap()
	{
		GL.UnmapNamedBuffer(Handle);
	}

	public void Bind()
	{
		GL.BindBuffer(Target, Handle);
	}

	public void Unbind()
	{
		GL.BindBuffer(Target, 0);
	}

	public void Dispose()
	{
		GL.DeleteBuffer(Handle);
		GC.SuppressFinalize(this);
	}
}