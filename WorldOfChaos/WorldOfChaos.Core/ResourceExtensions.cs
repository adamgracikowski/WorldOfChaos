namespace WorldOfChaos.Core;

public static class ResourceExtensions
{
	public static Stream? GetResourceStream(string path)
	{
		return AppDomain.CurrentDomain
			.GetAssemblies()
			.Select(assembly => assembly.GetManifestResourceStream(path))
			.OfType<Stream>()
			.FirstOrDefault();
	}
}