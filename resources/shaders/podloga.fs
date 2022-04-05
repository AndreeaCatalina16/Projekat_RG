#version 330 core


layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 BrightColor;


struct PointLight {
    vec3 position;

    vec3 specular;
    vec3 diffuse;
    vec3 ambient;

    float constant;
    float linear;
    float quadratic;
};

struct DirLight {
    vec3 direction;

    vec3 specular;
    vec3 diffuse;
    vec3 ambient;
};

in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;


uniform PointLight pointLight;
uniform DirLight dirLight;

uniform bool blinn;
uniform sampler2D terrainTexture;

uniform vec3 viewPosition;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main() {
    vec3 normal = normalize(Normal);
    vec3 viewDir = normalize(viewPosition - FragPos);
    vec3 result = CalcDirLight(dirLight, normal, viewDir);
    result += CalcPointLight(pointLight, normal, FragPos, viewDir);
    float brightness = dot(result, vec3(0.2126, 0.7152, 0.0722));
        if(brightness > 1.0)
            BrightColor = vec4(result, 1.0);
        else
            BrightColor = vec4(0.0, 0.0, 0.0, 1.0);

    FragColor = vec4(result, 1.0);

}

//Calculates the color when using a point light.
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    if(blinn) {
        vec3 halfwayDir = normalize(lightDir + viewDir);
        spec = pow(max(dot(normal, halfwayDir), 0.0), 32);
    }
    else {
        spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    }
    // combine results
    vec3 ambient = light.ambient * vec3(texture(terrainTexture, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(terrainTexture, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(terrainTexture, TexCoords));
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir) {
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0f;
        if (blinn) {
            vec3 halfwayDir = normalize(lightDir + viewDir);
            spec = pow(max(dot(normal, halfwayDir), 0.0), 32);
        } else {
                      spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
              }

    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    // combine results

    vec3 ambient = light.ambient * vec3(texture(terrainTexture, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(terrainTexture, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(terrainTexture, TexCoords).xxx);
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}


