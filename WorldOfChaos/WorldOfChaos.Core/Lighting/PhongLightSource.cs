using OpenTK.Mathematics;

namespace WorldOfChaos.Core.Lighting;

public abstract class PhongLightSource
{
	public Vector3 Ambient { get; set; }
	public Vector3 Diffuse { get; set; }
	public Vector3 Specular { get; set; }

	public PhongLightSource(Vector3 ambient, Vector3 diffuse, Vector3 specular)
	{
		Ambient = ambient;
		Diffuse = diffuse;
		Specular = specular;
	}

	public abstract void Apply(Shader shader, string? collectionName = null, int? collectionIndex = null);
}