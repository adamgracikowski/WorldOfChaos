using OpenTK.Graphics.OpenGL4;
using WorldOfChaos.Core.Interfaces;

namespace WorldOfChaos.Core.Buffers;

public class FrameBuffer : IBindable, IDisposable
{
	public int Handle { get; }
	private Dictionary<FramebufferAttachment, Texture> TextureAttachments { get; } = [];
	private Dictionary<FramebufferAttachment, RenderBuffer> RenderBufferAttachments { get; } = [];

	public FrameBuffer()
	{
		GL.CreateFramebuffers(1, out int handle);
		Handle = handle;
	}

	public void AttachTexture(FramebufferAttachment attachment, Texture texture, int level = 0)
	{
		TextureAttachments[attachment] = texture;
		GL.NamedFramebufferTexture(Handle, attachment, texture.Handle, level);
	}

	public void AttachRenderBuffer(FramebufferAttachment attachment, RenderBuffer renderbuffer)
	{
		RenderBufferAttachments[attachment] = renderbuffer;
		GL.NamedFramebufferRenderbuffer(Handle, attachment, RenderbufferTarget.Renderbuffer, renderbuffer.Handle);
	}

	public void CheckStatus()
	{
		var status = GL.CheckNamedFramebufferStatus(Handle, FramebufferTarget.Framebuffer);
		if (status != FramebufferStatus.FramebufferComplete)
		{
			throw new Exception($"Framebuffer is incomplete: {status}");
		}
	}

	public Texture GetTexture(FramebufferAttachment attachment)
	{
		return TextureAttachments[attachment];
	}

	public void Bind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
	}

	public void Unbind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public void Dispose()
	{
		foreach (var attachment in TextureAttachments)
		{
			attachment.Value.Dispose();
		}

		foreach (var attachment in RenderBufferAttachments)
		{
			attachment.Value.Dispose();
		}

		GC.SuppressFinalize(this);
	}
}