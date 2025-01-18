#version 330 core

layout (location = 0) in vec3 position;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;

void main()
{
    vec4 worldPosition = model * vec4(position, 1.0);

    gl_Position = projection * view * worldPosition;

    FragPos = vec3(worldPosition);
}