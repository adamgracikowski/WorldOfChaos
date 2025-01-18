#version 330 core

struct Fog {
    vec3 color;
    float start;
    float end;
    bool use;
};

uniform sampler2D diffuse;
uniform bool reflect;
uniform Fog fog;
uniform vec3 viewPos;

in vec2 TexCoords;
in vec3 FragPos;

out vec4 FragColor;

void main()
{
    vec4 finalColor = vec4(0, 0, 0, 0);
    
    if(reflect){
        finalColor = texture(diffuse, TexCoords);
    } else {
        finalColor = vec4(1.0, 1.0, 1.0, 1.0);
    }

    if (fog.use) {
        float distance = length(viewPos - FragPos);
        float fogFactor = clamp((fog.end - distance) / (fog.end - fog.start), 0.0, 1.0);
        vec3 foggedColor = mix(fog.color, finalColor.rgb, fogFactor);
        FragColor = vec4(foggedColor, finalColor.a);
    } else {
        FragColor = finalColor;
    }
}