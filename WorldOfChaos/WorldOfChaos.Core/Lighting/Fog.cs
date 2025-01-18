using OpenTK.Mathematics;

namespace WorldOfChaos.Core.Lighting;

public sealed class Fog(float start, float end)
{
	public Vector3 Color { get; set; } = new Vector3(0.7f, 0.7f, 0.7f);

	public float Start { get; set; } = start;
	public float End { get; set; } = end;

	public bool IsUsed { get; set; }

	public void Apply(Shader shader)
	{
		shader.LoadFloat3($"fog.color", Color);
		shader.LoadFloat($"fog.start", Start);
		shader.LoadFloat($"fog.end", End);
		shader.LoadBool($"fog.use", IsUsed);
	}
}