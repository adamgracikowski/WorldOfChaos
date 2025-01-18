using OpenTK.Graphics.OpenGL4;

namespace WorldOfChaos.Core;

public sealed class Material(Texture diffuse, Texture specular, float shininess)
{
	public Texture Diffuse { get; set; } = diffuse;
	public Texture Specular { get; set; } = specular;
	public float Shininess { get; set; } = shininess;

	public void Use()
	{
		Diffuse.Use(TextureUnit.Texture0);
		Specular.Use(TextureUnit.Texture1);
	}
}

public static partial class ShaderExensions
{
	public static void LoadMaterial(this Shader shader, Material material)
	{
		shader.LoadInt("material.diffuse", 0);
		shader.LoadInt("material.specular", 1);
		shader.LoadFloat("material.shininess", material.Shininess);
	}
}