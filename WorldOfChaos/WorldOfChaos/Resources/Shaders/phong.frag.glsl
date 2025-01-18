#version 330 core

#define MAX_DIRECTIONAL_LIGHTS 10
#define MAX_POINT_LIGHTS 10
#define MAX_FLASH_LIGHTS 10

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    
    float shininess;
};

struct DirectionalLight {
    vec4 direction;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct FlashLight {
    vec3 position;
    vec3 direction;

    float cutOff;
    float outerCutOff;
    
    float constant;
    float linear;
    float quadratic;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct Fog {
    vec3 color;
    float start;
    float end;
    bool use;
};

uniform PointLight PointLights[MAX_POINT_LIGHTS];
uniform DirectionalLight DirectionalLights[MAX_DIRECTIONAL_LIGHTS];
uniform FlashLight FlashLights[MAX_FLASH_LIGHTS];

uniform int PointLightsCount;
uniform int DirectionalLightsCount;
uniform int FlashLightsCount;

uniform Material material;
uniform vec3 viewPos;

uniform bool night;
uniform bool useBlinn;

uniform Fog fog;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

out vec4 FragColor;

// Function prototypes
vec3 CalculateDirectionalLight(DirectionalLight light, vec3 normal, vec3 viewDir);
vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalculateFlashLight(FlashLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 result = vec3(0, 0, 0);

    // Calculate directional lighting
    if(!night) {
        for (int i = 0; i < DirectionalLightsCount; i++) {
            result += CalculateDirectionalLight(DirectionalLights[i], norm, viewDir);
        }
    }

    // Calculate point lighting
    for (int i = 0; i < PointLightsCount; i++) {
        result += CalculatePointLight(PointLights[i], norm, FragPos, viewDir);
    }

    // Calculate flash lighting
    for (int i = 0; i < FlashLightsCount; i++) {
        result += CalculateFlashLight(FlashLights[i], norm, FragPos, viewDir);
    }

    vec4 finalColor = vec4(result, 1.0);

    if (fog.use) {
        float distance = length(viewPos - FragPos);
        float fogFactor = clamp((fog.end - distance) / (fog.end - fog.start), 0.0, 1.0);
        vec3 foggedColor = mix(fog.color, finalColor.rgb, fogFactor);
        FragColor = vec4(foggedColor, finalColor.a);
    } else {
        FragColor = finalColor;
    }
}

// Calculates the color using a directional light.
vec3 CalculateDirectionalLight(DirectionalLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction.xyz);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    float spec;
    if (useBlinn) {
        vec3 halfwayDir = normalize(lightDir + viewDir);
        spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
    } else {
        vec3 reflectDir = reflect(-lightDir, normal);
        spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    }
    
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    
    return (ambient + diffuse + specular);
}

// Calculates the color using a point light.
vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    float spec;
    if (useBlinn) {
        vec3 halfwayDir = normalize(lightDir + viewDir);
        spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
    } else {
        vec3 reflectDir = reflect(-lightDir, normal);
        spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    }

    float distance = length(light.position - fragPos);
    
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    
    return (ambient + diffuse + specular);
}

// Calculates the color using a flash light.
vec3 CalculateFlashLight(FlashLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    float spec;
    if (useBlinn) {
        vec3 halfwayDir = normalize(lightDir + viewDir);
        spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
    } else {
        vec3 reflectDir = reflect(-lightDir, normal);
        spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    }

    float distance = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    
    ambient *= attenuation;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;

    return (ambient + diffuse + specular);
}