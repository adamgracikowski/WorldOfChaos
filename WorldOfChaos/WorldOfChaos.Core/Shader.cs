using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Text;
using System.Text.RegularExpressions;

namespace WorldOfChaos.Core;

public sealed class Shader : IDisposable
{
	public int Handle { get; private set; }
	private Dictionary<string, int> Uniforms { get; } = [];

	public Shader(params (string path, ShaderType type)[] paths)
	{
		var sources = new List<(string source, ShaderType type)>();

		foreach (var (path, type) in paths)
		{
			var source = ReadSource(path);

			sources.Add((source, type));
		}

		var shaders = new List<int>();

		foreach (var (source, type) in sources)
		{
			var handle = CreateShader(source, type);

			shaders.Add(handle);
		}

		foreach (var shader in shaders)
		{
			CompileShader(shader);
		}

		CreateProgram([.. shaders]);

		CleanupShaders([.. shaders]);

		InitializeUniformsMap();
	}

	public void Use()
	{
		GL.UseProgram(Handle);
	}

	public int GetAttributeLocation(string attribName)
	{
		return GL.GetAttribLocation(Handle, attribName);
	}

	private int GetUniformLocation(string name)
	{
		var found = Uniforms.TryGetValue(name, out var location);

		if (found) return location;

		Console.WriteLine($"Uniform not found: {name}");

		return GL.GetUniformLocation(Handle, name);
	}

	public void LoadInt(string name, int value)
	{
		GL.ProgramUniform1(Handle, GetUniformLocation(name), value);
	}

	public void LoadBool(string name, bool value)
	{
		GL.ProgramUniform1(Handle, GetUniformLocation(name), value ? 1 : 0);
	}

	public void LoadFloat(string name, float value)
	{
		GL.ProgramUniform1(Handle, GetUniformLocation(name), value);
	}

	public void LoadFloat2(string name, Vector2 value)
	{
		GL.ProgramUniform2(Handle, GetUniformLocation(name), ref value);
	}

	public void LoadFloat2(string name, ref Vector2 value)
	{
		GL.ProgramUniform2(Handle, GetUniformLocation(name), ref value);
	}

	public void LoadFloat3(string name, Vector3 value)
	{
		GL.ProgramUniform3(Handle, GetUniformLocation(name), ref value);
	}

	public void LoadFloat3(string name, ref Vector3 value)
	{
		GL.ProgramUniform3(Handle, GetUniformLocation(name), ref value);
	}

	public void LoadFloat4(string name, Vector4 value)
	{
		GL.ProgramUniform4(Handle, GetUniformLocation(name), ref value);
	}

	public void LoadFloat4(string name, ref Vector4 value)
	{
		GL.ProgramUniform4(Handle, GetUniformLocation(name), ref value);
	}

	public void LoadMatrix2(string name, Matrix2 value, bool transpose = false)
	{
		GL.ProgramUniformMatrix2(Handle, GetUniformLocation(name), transpose, ref value);
	}

	public void LoadMatrix2(string name, ref Matrix2 value, bool transpose = false)
	{
		GL.ProgramUniformMatrix2(Handle, GetUniformLocation(name), transpose, ref value);
	}

	public void LoadMatrix3(string name, Matrix3 value, bool transpose = false)
	{
		GL.ProgramUniformMatrix3(Handle, GetUniformLocation(name), transpose, ref value);
	}

	public void LoadMatrix3(string name, ref Matrix3 value, bool transpose = false)
	{
		GL.ProgramUniformMatrix3(Handle, GetUniformLocation(name), transpose, ref value);
	}

	public void LoadMatrix4(string name, Matrix4 value, bool transpose = false)
	{
		GL.ProgramUniformMatrix4(Handle, GetUniformLocation(name), transpose, ref value);
	}

	public void LoadMatrix4(string name, ref Matrix4 value, bool transpose = false)
	{
		GL.ProgramUniformMatrix4(Handle, GetUniformLocation(name), transpose, ref value);
	}

	public void Dispose()
	{
		GL.DeleteProgram(Handle);
		GC.SuppressFinalize(this);
	}

	private static string ReadSource(string path)
	{
		using var stream = ResourceExtensions.GetResourceStream(path)
			?? throw new Exception($"Shader not found: {path}");

		using var reader = new StreamReader(stream, Encoding.UTF8);

		return reader.ReadToEnd();
	}

	private static int CreateShader(string source, ShaderType type)
	{
		var shader = GL.CreateShader(type);

		GL.ShaderSource(shader, source);

		return shader;
	}

	private static void CompileShader(int shader)
	{
		GL.CompileShader(shader);

		GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);

		if (status == (int)All.False)
		{
			var infoLog = GL.GetShaderInfoLog(shader);
			throw new Exception($"Shader compilation failed: {infoLog}");
		}
	}

	private void CreateProgram(params int[] shaders)
	{
		Handle = GL.CreateProgram();

		foreach (var shader in shaders)
		{
			GL.AttachShader(Handle, shader);
		}

		GL.LinkProgram(Handle);

		GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var status);

		if (status == (int)All.False)
		{
			var infoLog = GL.GetProgramInfoLog(Handle);
			throw new Exception($"Program linking failed: {infoLog}");
		}
	}

	private void CleanupShaders(params int[] shaders)
	{
		foreach (var shader in shaders)
		{
			GL.DetachShader(Handle, shader);
			GL.DeleteShader(shader);
		}
	}

	private void InitializeUniformsMap()
	{
		GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var uniforms);
		GL.GetProgram(Handle, GetProgramParameterName.ActiveUniformMaxLength, out var maxUniformLength);

		for (var i = 0; i < uniforms; i++)
		{
			GL.GetActiveUniform(Handle, i, maxUniformLength, out _, out var size, out _, out var nameTemplate);

			for (var j = 0; j < size; j++)
			{
				var name = Regex.Replace(nameTemplate, @"\[0\]$", $"[{j}]");

				Uniforms[name] = GL.GetUniformLocation(Handle, name);
			}
		}
	}
}