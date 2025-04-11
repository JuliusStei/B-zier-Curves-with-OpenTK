using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Numerics;
using System;
using System.Diagnostics;
using System.Drawing;
using Vector3 = OpenTK.Mathematics.Vector3;
using System.IO;
using OpenGLHandout.Scene;
using System.Data;
using System.Collections.Generic;

namespace OpenGLHandout.Geometry {

    /// <summary>
    /// geometry class for non-indexed geometries, 
    /// i.e. geometries that consist of a vertex array that holds the vertex attributes
    /// This geometry class is helpful for lines, line strips and line loops
    /// </summary>

    public class NonIndexedGeometry : IGeometry {

        #region private fields
        // vertex buffer object of this geometry
        private readonly int vbo;
        // vertex buffer for this geometry
        private readonly int vertexBuffer;
        // information about the vertex attributes contained in the vertex buffer
        private readonly GeometryInfo config;
        // primitive type for the geometry
        private readonly PrimitiveType primitiveType;
        // number of vertices to be drawm
        private int numVertices;
        // assigned shader
        private Shader shader;
        #endregion

        /// <summary>
        /// initializes a new non-indexed geometry
        /// </summary>
        /// <param name="vertexData">vertex buffer data</param>
        /// <param name="numVertices">number of vertices to be drawn</param>
        /// <param name="primitiveType">primitive type (lines, triangles, etc)</param>
        /// <param name="config">information about the vertex attributes</param>
        public NonIndexedGeometry(float[] vertexData, int numVertices, PrimitiveType primitiveType, GeometryInfo config) {
            this.config = config;
            this.primitiveType = primitiveType;

            // create a new vertex array object...
            vbo = GL.GenVertexArray();
            // ... and bind it (= make it the currently active vertex array object)
            GL.BindVertexArray(vbo);

            // reserve a new memory buffer on the graphics board (for the vertex data)
            vertexBuffer = GL.GenBuffer();

            Update(vertexData, numVertices);

        }

        /// <summary>
        /// update the geometry with new vertex and index data
        /// </summary>
        public void Update(float[] vertexData, int numVertices) {

            this.numVertices = numVertices;

            // tell opengl that the data we are now sending go into the vertex buffer ...
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            // ... and send the data
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

        }

        /// <summary>
        /// sets the shader for this geometry
        /// </summary>
        /// <inheritdoc cref="IGeometry.Shader"/>
        /// <para>Note that setting the shader will changed the vertex attribute assignments</para>
        public Shader Shader {
            get => shader;
            set {
                shader = value;
                SetMeshConfiguration(shader);
            }
        }

        /// <summary>
        /// current world transformation of this geometry
        /// </summary>
        /// <inheritdoc cref="IGeometry.WorldMatrix"/>
        public Matrix4 WorldMatrix {
            get; set;
        } = Matrix4.Identity;

        /// <summary>
        /// draws the geometry using the <paramref name="viewMatrix"/> and <paramref name="projectionMatrix"/>
        /// </summary>
        /// <inheritdoc cref="IGeometry.Draw(Matrix4, Matrix4)"/>
        public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix) {
            if (shader is null) return;

            GL.BindVertexArray(vbo);
            Matrix4 modelViewProj = WorldMatrix * viewMatrix * projectionMatrix;
            shader.Use();
            int mvp = shader.GetUniformLocation("u_modelViewProj");
            GL.UniformMatrix4(mvp, false, ref modelViewProj);
            GL.DrawArrays(primitiveType, 0, numVertices);
            Debug.Assert(GL.GetError() == ErrorCode.NoError);
        }

        #region private utility methods

        /// <summary>
        /// utility method called whenever a new shader is assigned. It will retrieve the vertex attribute
        /// locations from the given <paramref name="shader"/> and assign the attributes from the vertex buffer
        /// using the attribute information stored in <see cref="config"/>
        /// <seealso cref="GeometryInfo"/>
        /// <seealso cref="AttributeInfo"/>
        /// </summary>
        private void SetMeshConfiguration(Shader shader) {
            Debug.Assert(GL.GetError() == ErrorCode.NoError);

            GL.BindVertexArray(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

            foreach (var attribute in config.Attributes) {

                int loc = shader.GetAttribLocation(attribute.AttributeName);
                if (loc < 0) continue; // the shader does not have any attribute with this name
                GL.EnableVertexAttribArray(loc);
                GL.VertexAttribPointer(loc, attribute.NumComponents, attribute.DataType, attribute.Normalized, attribute.Stride, attribute.BufferOffset);
                Debug.Assert(GL.GetError() == ErrorCode.NoError);
            }

            Debug.Assert(GL.GetError() == ErrorCode.NoError);
        }

        #endregion
    }
}


