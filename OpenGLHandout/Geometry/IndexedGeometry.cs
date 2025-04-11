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
    /// geometry class for indexed geometries, 
    /// i.e. geometries that consist of a vertex array that holds the vertex attributes
    /// and an index array which holds the topology information
    /// </summary>

    public class IndexedGeometry : IGeometry
    {

        #region private fields
        // vertex buffer object of this geometry
        private readonly int vbo;
        // vertex buffer for this geometry
        private readonly int vertexBuffer;
        // index buffer for this geometry
        private readonly int indexBuffer;
        // information about the vertex attributes contained in the vertex buffer
        private readonly GeometryInfo config;
        // primitive type for the geometry
        private readonly PrimitiveType primitiveType;
        // number of indices to be drawm
        private int numIndices;
        // assigned shader
        private Shader shader;
        #endregion

        /// <summary>
        /// initializes a new indexed geometry
        /// </summary>
        /// <param name="vertexData">vertex buffer data</param>
        /// <param name="indexData">index buffer data</param>
        /// <param name="primitiveType">primitive type (lines, triangles, etc)</param>
        /// <param name="config">information about the vertex attributes</param>
        public IndexedGeometry(float[] vertexData, ushort[] indexData, PrimitiveType primitiveType, GeometryInfo config)
        {
            this.config = config;
            this.primitiveType = primitiveType;

            // create a new vertex array object...
            vbo = GL.GenVertexArray();
            // ... and bind it (= make it the currently active vertex array object)
            GL.BindVertexArray(vbo);

            // reserve a new memory buffer on the graphics board (for the vertex data)
            vertexBuffer = GL.GenBuffer();
            // reserve a new memory buffer on the graphics board (for the fragment data)
            indexBuffer = GL.GenBuffer();

            Update(vertexData, indexData);

        }

        /// <summary>
        /// update the geometry with new vertex and index data
        /// </summary>
        public void Update(float[] vertexData, ushort[] indexData) {

            numIndices = indexData.Length;
            // tell opengl that the data we are now sending go into the vertex buffer ...
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            // ... and send the data
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            // tell opengl that the data we are now sending go into the index buffer ...
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            // ... and send the data
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexData.Length * sizeof(ushort), indexData, BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// sets the shader for this geometry
        /// </summary>
        /// <inheritdoc cref="IGeometry.Shader"/>
        /// <para>Note that setting the shader will changed the vertex attribute assignments</para>
        public Shader Shader
        {
            get => shader;
            set
            {
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
        public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (shader is null) return;

            GL.BindVertexArray(vbo);
            Matrix4 modelViewProj = WorldMatrix * viewMatrix * projectionMatrix;
            shader.Use();
            int mvp = shader.GetUniformLocation("u_modelViewProj");
            GL.UniformMatrix4(mvp, false, ref modelViewProj);
            GL.DrawElements(primitiveType, numIndices, DrawElementsType.UnsignedShort, 0);
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
        private void SetMeshConfiguration(Shader shader)
        {
            Debug.Assert(GL.GetError() == ErrorCode.NoError);

            GL.BindVertexArray(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);

            foreach(var attribute in config.Attributes) {
                
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


