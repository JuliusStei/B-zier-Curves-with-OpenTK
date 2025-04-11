using OpenGLHandout.Geometry;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace OpenGLHandout.Scene
{
    /// <summary>
    /// base scene class holding an arbitrary number of geometries
    /// </summary>
    public class OpenGLBaseScene
    {

        /// <summary>
        /// the view distance for the camera
        /// </summary>
        public float ViewDistance { get; set; } = 1.0f;

        /// <summary>
        /// initializes a new empty scene with the default shaders
        /// </summary>
        public OpenGLBaseScene()
        {
            string vertexShaderCode = File.ReadAllText(@"Shaders\vertexShader.vert");
            string fragmentShaderCode = File.ReadAllText(@"Shaders\fragmentShader.frag");
            shader = new Shader(vertexShaderCode, fragmentShaderCode);
        }

        /// <summary>
        /// adds a new geometry to the scene
        /// </summary>
        /// <param name="geom">geometry to add</param>
        public void AddGeometry(IGeometry geom)
        {
            geom.Shader = shader;
            geometries.Add(geom);
        }

        /// <summary>
        /// draws the scene using the given <paramref name="viewMatrix"/> and <paramref name="projMatrix"/>
        /// </summary>
        public void Draw(Matrix4 viewMatrix, Matrix4 projMatrix)
        {
            foreach (var geo in geometries)
            {
                geo.Draw(viewMatrix, projMatrix);
            }
        }

        #region private fields 
        // shader for the geometris
        protected readonly Shader shader;
        // list of geometries to be drawn
        private readonly List<IGeometry> geometries = new();
        #endregion
    }
}
