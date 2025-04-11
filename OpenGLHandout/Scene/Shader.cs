using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;

namespace OpenGLHandout.Scene {

    public class Shader {

        /// <summary>
        /// activate the shader
        /// </summary>
        public void Use() {
            GL.UseProgram(ShaderId);
        }

        /// <summary>
        /// initialize a new shader program from the source code
        /// </summary>
        /// <param name="vertexShaderSource">source code for the vertex shader</param>
        /// <param name="fragmentShaderSource">source code for the fragment shader</param>
        /// <exception cref="ArgumentException">thrown if the shader compilation failed</exception>
        public Shader(string vertexShaderSource, string fragmentShaderSource) {

            // check if any OpenGL error has occurred before entering this constructor
            // just to make sure that if an error is reported later, it is actually
            // caused by the code  in this method
            Debug.Assert(GL.GetError() == ErrorCode.NoError);

            // create a new vertex shader
            int vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            // submit the source code for the vertex shader
            GL.ShaderSource(vertexShaderId, vertexShaderSource);

            // create a new fragment shader
            int fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);
            // submit the source code for the fragment shader
            GL.ShaderSource(fragmentShaderId, fragmentShaderSource);

            // try to compile the vertex shader and ...
            GL.CompileShader(vertexShaderId);

            // ... check if the compilation was successful (!)
            GL.GetShader(vertexShaderId, ShaderParameter.CompileStatus, out int success);
            if (success == 0) {
                // an error occurred, retrieve the error log from OpenGL
                string infoLog = GL.GetShaderInfoLog(vertexShaderId);
                Console.WriteLine(infoLog);
                Debugger.Break();
                throw new ArgumentException("Vertex Shader Compilation Failed", infoLog);
            }

            // compile the fragment shader and ...
            GL.CompileShader(fragmentShaderId);

            // ... check if the compilation was successful (!)
            GL.GetShader(fragmentShaderId, ShaderParameter.CompileStatus, out success);
            if (success == 0) {
                // an error occurred, retrieve the error log from OpenGL
                string infoLog = GL.GetShaderInfoLog(fragmentShaderId);
                Console.WriteLine(infoLog);
                Debugger.Break();
                throw new ArgumentException("Fragment Shader Compilation Failed", infoLog);
            }

            // at this point, we can be sure that the shader code does not contain any syntax errors

            // Create a complete Shader Programm...
            ShaderId = GL.CreateProgram();
            //.. consisting of the vertex shader
            GL.AttachShader(ShaderId, vertexShaderId);
            //.. and the fragment shader
            GL.AttachShader(ShaderId, fragmentShaderId);

            // try to link both programmes together and ...
            GL.LinkProgram(ShaderId);

            // check if the linking was successful
            GL.GetProgram(ShaderId, GetProgramParameterName.LinkStatus, out success);
            if (success == 0) {
                // an error occurred, retrieve the error log from OpenGL
                string infoLog = GL.GetProgramInfoLog(ShaderId);
                Console.WriteLine(infoLog);
                Debugger.Break();
                throw new ArgumentException("Shader Linking Failed", infoLog);
            }

            // since out programm has been successfully created,
            // we do not need the code of the individual vertex and frament shaders anymore.
            // We detach both shaders and delete them
            GL.DetachShader(ShaderId, vertexShaderId);
            GL.DetachShader(ShaderId, fragmentShaderId);
            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);

            // check of any OpenGL error has occurred
            Debug.Assert(GL.GetError() == ErrorCode.NoError);
        }

        /// <summary>
        /// Attributes are values which are passed through the pipline. This method
        /// retrieves the location (like the address) of an attribute with the 
        /// given <paramref name="attribName"/> to bind vertex buffer data to it.
        /// </summary>
        public int GetAttribLocation(string attribName) {
            return GL.GetAttribLocation(ShaderId, attribName);
        }

        /// <summary>
        /// Uniforms are external values to be set and considered constant
        /// during the execution of the pipline. Uniform value include the transformation matrices
        /// as well as any other parameter to configure the pipline.
        /// This method retrieves the location 
        /// (like the address) of a uniform with the given <paramref name="uniformName"/>,
        /// in order to explicitly set its value before rendering
        /// </summary>
        public int GetUniformLocation(string uniformName) {
            return GL.GetUniformLocation(ShaderId, uniformName);
        }

        /// <summary>
        /// integral id of the shader
        /// </summary>
        public int ShaderId { get; private set; }

    }
}
