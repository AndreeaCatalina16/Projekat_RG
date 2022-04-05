#version 330 core

layout (location = 0) out vec4 FragColor;

layout (location = 1) out vec4 BrightColor;

uniform samplerCube skybox;

in vec3 TexCoords;

void main()
{
    float brightness = dot(vec3(FragColor), vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.5)
        BrightColor = FragColor;
    else
        BrightColor = vec4(0.0, 0.0, 0.0, 1.0);

    FragColor = texture(skybox, TexCoords);
}
