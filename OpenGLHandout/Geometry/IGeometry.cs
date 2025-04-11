using OpenTK.Mathematics;
using OpenGLHandout.Scene;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace OpenGLHandout.Geometry {
  
    /// <summary>
    /// interface for geometry to be drawn
    /// </summary>
    public interface IGeometry {

        /// <summary>
        /// shader assigned to the geometry
        /// </summary>
        public Shader Shader { get; set; }

        /// <summary>
        /// current world transformation of this geometry
        /// </summary>
        public Matrix4 WorldMatrix { get; set; } 

        /// <summary>
        /// draws the geometry using the current <paramref name="viewMatrix"/> and <paramref name="projectionMatrix"/>
        /// </summary>
        public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix);
    }

    /// <summary>
    /// data structure holding information about a geometry's vertex attributes
    /// </summary>
    public struct GeometryInfo {
        /// <summary>
        /// information about the individual vertex attribues
        /// </summary>
        public AttributeInfo[] Attributes;
    }

    /// <summary>
    /// data structure holding information about vertex attributes
    /// </summary>
    public struct AttributeInfo {
        /// <summary>name of the attribute (in the shader) </summary>
        public string AttributeName;
        
        /// <summary>number of vector components</summary>
        public int NumComponents;
        
        /// <summary>buffer offset in bytes</summary>
        public int BufferOffset;
        
        /// <summary>attribute data type</summary>
        public VertexAttribPointerType DataType;

        /// <summary>should values be normalized?</summary>
        public bool Normalized;

        /// <summary>buffer offset (in bytes) between one element and the next</summary>
        public int Stride;
    }
}


