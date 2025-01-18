#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
	vec4 worldPosition = model * vec4(position, 1.0);
	
	gl_Position = projection * view * worldPosition;

	Normal = normalize(normalMatrix * normal);
	FragPos = vec3(worldPosition);
	TexCoords = texCoords;
}