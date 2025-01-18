using OpenTK.Graphics.OpenGL4;
using WorldOfChaos.Core.Interfaces;

namespace WorldOfChaos.Core.Buffers;

public class RenderBuffer : IDisposable, IBindable
{
	public int Handle { get; }

	public RenderBuffer()
	{
		GL.CreateRenderbuffers(1, out int handle);
		Handle = handle;
	}

	public void Allocate(int width, int height, RenderbufferStorage internalFormat)
	{
		GL.NamedRenderbufferStorage(Handle, internalFormat, width, height);
	}

	public void Bind()
	{
		GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Handle);
	}

	public void Unbind()
	{
		GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
	}

	public void Dispose()
	{
		GL.DeleteRenderbuffer(Handle);
		GC.SuppressFinalize(this);
	}
}