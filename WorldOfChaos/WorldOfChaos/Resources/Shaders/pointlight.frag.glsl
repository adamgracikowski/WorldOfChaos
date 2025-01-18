#version 330 core

struct Fog {
    vec3 color;
    float start;
    float end;
    bool use;
};

out vec4 FragColor;

uniform vec3 viewPos;
uniform vec3 lightColor;

uniform Fog fog;

in vec3 FragPos;

void main()
{
    vec4 finalColor = vec4(lightColor, 1.0);

    if (fog.use) {
        float distance = length(viewPos - FragPos);
        float fogFactor = clamp((fog.end - distance) / (fog.end - fog.start), 0.0, 1.0);
        vec3 foggedColor = mix(fog.color, finalColor.rgb, fogFactor);
        FragColor = vec4(foggedColor, finalColor.a);
    } else {
        FragColor = finalColor;
    }
}