#version 330 core

// attribute (input)
in vec3 color;

// attribute (output)
out vec4 fragColor;

void main() {
	fragColor = vec4(color, 1.0);
}