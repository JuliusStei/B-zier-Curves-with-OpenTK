#version 330 core

// attribute (input)
in vec3 a_position;
in vec3 a_color;

// attribute (output)
out vec3 color;

// uniform values
uniform mat4 u_modelViewProj;

void main() {

	gl_Position = u_modelViewProj * vec4(a_position, 1.0);

	// übergebe einfach die Farbe die Du bekommen hast
	color = a_color; 
}
