using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGLHandout.Geometry
{
    /// <summary>
    /// utility class for creating default geometry
    /// </summary>
    public static class GeometryUtilities
    {
      
        /// <summary>
        /// creates a new sphere geometry
        /// </summary>
        /// <param name="radius">radius of the sphere</param>
        /// <param name="numRows">number of sections in longitude direction</param>
        /// <param name="numCols">number of sections in latitude direction</param>
        public static IGeometry CreateSphere(float radius, int numRows = 64, int numCols = 64)
        {
            float[] vertexData = CreateSphereVertexData(numRows, numCols, radius);
            ushort[] indexData = CreateIndexData(numRows, numCols);
            return new IndexedGeometry(vertexData, indexData, PrimitiveType.Triangles, config);
        }

        /// <summary>
        /// creates a new torus geometry
        /// </summary>
        /// <param name="bigRadius">larger radius of the torus</param>
        /// <param name="smallRadius">smaller radius of the torus</param>
        /// <param name="numRows">number of sections on the larger circle</param>
        /// <param name="numCols">number of sections on the smaller circle</param>
        public static IGeometry CreateTorus(float bigRadius, float smallRadius, int numRows = 64, int numCols = 64)
        {
            float[] vertexData = CreateTorusVertexData(numRows, numCols, bigRadius, smallRadius);
            ushort[] indexData = CreateIndexData(numRows, numCols);
            return new IndexedGeometry(vertexData, indexData, PrimitiveType.Triangles, config);
        }

        #region common geometry configuration 
        /// <summary>
        /// common geometry configuration for all the geometries this class generates
        /// </summary>
        private static GeometryInfo config = new GeometryInfo() {
            Attributes = new AttributeInfo[] {
                new() {
                    AttributeName = "a_position",
                    DataType = VertexAttribPointerType.Float,
                    BufferOffset = 0,
                    NumComponents = 3,
                    Stride = 6 * sizeof(float),
                },
                new() {
                    AttributeName = "a_color",
                    DataType = VertexAttribPointerType.Float,
                    BufferOffset = 3 * sizeof(float),
                    NumComponents = 3,
                    Stride = 6 * sizeof(float),
                },
            }
        };
        #endregion

        #region private utility methods 


        /// <summary>
        /// creates the vertex buffer data for the sphere geometry
        /// </summary>
        /// <param name="numRows">number of rows</param>
        /// <param name="numCols">number of columns</param>
        /// <param name="radius">radius of the sphere</param>
        /// <returns>array of floats representing the geometry</returns>
        private static float[] CreateSphereVertexData(int numRows, int numCols, float radius)
        {

            int numVertices = numRows * numCols;
            int numFloatsPerVertex = 6; // x,y,z, R,G,B
            int numFloats = numVertices * numFloatsPerVertex;

            float[] vertexData = new float[numFloats];

            int index = 0;
            for (int v = 0; v < numRows; v++)
            {
                float theta = MathF.PI * v / (numRows - 1.0f);

                for (int u = 0; u < numCols; u++)
                {
                    float phi = 2.0f * MathF.PI * u / (numCols - 1.0f);
                    
                    float x = MathF.Sin(theta) * MathF.Cos(phi) * radius;
                    float y = MathF.Sin(theta) * MathF.Sin(phi) * radius;
                    float z = MathF.Cos(theta) * radius;
                    float R = (x / radius + 1.0f) / 2.0f;
                    float G = (y / radius + 1.0f) / 2.0f;
                    float B = (z / radius + 1.0f) / 2.0f;

                    vertexData[index++] = x;
                    vertexData[index++] = y;
                    vertexData[index++] = z;
                    vertexData[index++] = R;
                    vertexData[index++] = G;
                    vertexData[index++] = B;
                }
            }

            return vertexData;
        }

        /// <summary>
        /// creates the vertex buffer data for the torus geometry
        /// </summary>
        /// <param name="numRows">number of rows</param>
        /// <param name="numCols">number of columns</param>
        /// <param name="largerRadius">larger radius of the torus</param>
        /// <param name="smallerRadius">smaller radius of the torus</param>
        /// <returns>array of floats representing the geometry</returns>
        private static float[] CreateTorusVertexData(int numRows, int numCols, float largerRadius, float smallerRadius)
        {

            int numVertices = numRows * numCols;
            int numFloatsPerVertex = 6; // x,y,z, R,G,B
            int numFloats = numVertices * numFloatsPerVertex;

            float[] vertexData = new float[numFloats];

            int index = 0;
            for (int v = 0; v < numRows; v++)
            {
                float theta = 2.0f * MathF.PI * v / (numRows - 1.0f);

                for (int u = 0; u < numCols; u++)
                {
                    float phi = 2.0f * MathF.PI * u / (numCols - 1.0f);

                    float x = largerRadius * MathF.Cos(theta) + smallerRadius * MathF.Cos(theta) * MathF.Cos(phi);
                    float y = largerRadius * MathF.Sin(theta) + smallerRadius * MathF.Sin(theta) * MathF.Cos(phi);
                    float z = 0 + smallerRadius * MathF.Sin(phi);
                    float R = (x / largerRadius + 1.0f) / 2.0f;
                    float G = (y / largerRadius + 1.0f) / 2.0f;
                    float B = (z / largerRadius + 1.0f) / 2.0f;

                    vertexData[index++] = x;
                    vertexData[index++] = y;
                    vertexData[index++] = z;
                    vertexData[index++] = R;
                    vertexData[index++] = G;
                    vertexData[index++] = B;
                }
            }

            return vertexData;
        }
        /// <summary>
        /// create common index buffer data any quad geometry (i.e any geometry that is based on loops of rows and columns)
        /// </summary>
        /// <param name="numRows">number of rows</param>
        /// <param name="numCols">numer of columns</param>
        private static ushort[] CreateIndexData(int numRows, int numCols)
        {
            int numQuads = (numRows - 1) * (numCols - 1);
            int numTriangles = numQuads * 2;
            int numIndices = numTriangles * 3;
            ushort[] indexData = new ushort[numIndices];
            int index = 0;
            for (int u = 0; u < numCols - 1; u++)
            {
                for (int v = 0; v < numRows - 1; v++)
                {
                    ushort i00 = (ushort)((v + 0) * numCols + u + 0); // unten links
                    ushort i10 = (ushort)((v + 1) * numCols + u + 0); // unten rechts
                    ushort i11 = (ushort)((v + 1) * numCols + u + 1); // oben rechts
                    ushort i01 = (ushort)((v + 0) * numCols + u + 1); // oben links

                    indexData[index++] = i00;
                    indexData[index++] = i10;
                    indexData[index++] = i11;

                    indexData[index++] = i00;
                    indexData[index++] = i11;
                    indexData[index++] = i01;


                }
            }

            return indexData;
        }
        #endregion
    }
}


