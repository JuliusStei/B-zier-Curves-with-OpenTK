using OpenGLHandout.Geometry;
using OpenGLHandout.Scene;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;

namespace OpenGLHandout
{
    /// <summary>
    /// main window class
    /// </summary>
    class OpenGLWindow : GameWindow
    {
        /// <summary>
        /// initializes a new window with default OpenGL settings
        /// </summary>
        public OpenGLWindow() : base(new GameWindowSettings(), new NativeWindowSettings())
        {
            // create the scene
            scene = new OpenGLBaseScene();

            // move camera back to view the entire scene
            viewMatrix = Matrix4.Identity;
        }

        /// <summary>
        /// method to be called when the screen needs to be redrawn
        /// </summary>
        /// <param name="args">frame event arguments</param>
        protected override void OnRenderFrame(FrameEventArgs args)
        {

            // check that no error has occurred before rendering
            Debug.Assert(GL.GetError() == ErrorCode.NoError);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            scene.Draw(viewMatrix, projMatrix);
            SwapBuffers();

            // check that no error has occurred during rendering
            Debug.Assert(GL.GetError() == ErrorCode.NoError);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            var p = new Vector2(MousePosition.X, screenHeight - MousePosition.Y);
            collectedPoints.Add(p);
            UpdateGeometry();
        }


        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
      
            if (e.Key == Keys.R)
            {
                collectedPoints.Clear();
                bezierCurvePoints.Clear();
                UpdateGeometry();
            }
        }



        protected void UpdateGeometry()
        {
            CalculatePointsOnCurve();

            var vertexDataLines = collectedPoints.SelectMany(x => new float[] { x.X, x.Y, 0f, 0.5f, 0.9f, 0.5f }).ToArray();
            var vertexDataPoints = collectedPoints.SelectMany(x => new float[] { x.X, x.Y, 0f, 1f, 1f, 1f }).ToArray();
            var vertexDataBezier = bezierCurvePoints.ToArray();



            if (lineGeometry is null)
            {
                var config = new GeometryInfo()
                { 
                    Attributes = new AttributeInfo[] {
                        new() {
                            AttributeName="a_position",
                            NumComponents = 3,
                            BufferOffset = 0,
                            DataType = VertexAttribPointerType.Float,
                            Normalized= false,
                            Stride = 6 * sizeof(float)
                        },

                        new()
                    {
                        AttributeName = "a_color",
                        NumComponents = 3,  // r, g, b
                        BufferOffset = 3 * sizeof(float),
                        DataType  = VertexAttribPointerType.Float,
                        Normalized = false,
                        Stride = 6 * sizeof(float)
                    }


                    }
                };
                lineGeometry = new NonIndexedGeometry(vertexDataLines, collectedPoints.Count, PrimitiveType.LineStrip, config);
                scene.AddGeometry(lineGeometry);
            }
            else
            {
                lineGeometry.Update(vertexDataLines, collectedPoints.Count);
            }

            GL.PointSize(6f);
            

            if (pointGeometry is null)
            {
                var config = new GeometryInfo()
                {
                    Attributes = new AttributeInfo[] {
                        new() {
                            AttributeName="a_position",
                            NumComponents = 3, // x ,y ,z
                            BufferOffset = 0,
                            DataType = VertexAttribPointerType.Float,
                            Normalized= false,
                            Stride = 6 * sizeof(float)
                        },

                    new()
                    {
                        AttributeName = "a_color",
                        NumComponents = 3,  // r, g, b
                        BufferOffset = 3 * sizeof(float),
                        DataType  = VertexAttribPointerType.Float,
                        Normalized = false,
                        Stride = 6 * sizeof(float)
                    }

                    }
                };
                pointGeometry = new NonIndexedGeometry(vertexDataPoints, collectedPoints.Count, PrimitiveType.Points, config);
                scene.AddGeometry(pointGeometry);
            }
            else
            {
                pointGeometry.Update(vertexDataPoints, collectedPoints.Count);
            }

            if (bezierGeometry is null)
            {
                var config = new GeometryInfo()
                {
                    Attributes = new AttributeInfo[]
                    {
                        new() {
                            AttributeName = "a_position",
                            NumComponents = 3,
                            BufferOffset = 0,
                            DataType = VertexAttribPointerType.Float,
                            Normalized = false,
                            Stride = 6 * sizeof(float)
                        },
                        new()
                        {
                            AttributeName = "a_color",
                            NumComponents = 3,
                            BufferOffset = 3 * sizeof(float),
                            DataType = VertexAttribPointerType.Float,
                            Normalized = false,
                            Stride = 6 * sizeof(float)
                        }
                    }
                };
                bezierGeometry = new NonIndexedGeometry(vertexDataBezier, bezierCurvePoints.Count / 6, PrimitiveType.LineStrip, config);
                scene.AddGeometry(bezierGeometry);
            }
            else
            {
                bezierGeometry.Update(vertexDataBezier, bezierCurvePoints.Count / 6);
            }
        }

        private void CalculatePointsOnCurve()
        {
            bezierCurvePoints.Clear();

            if (collectedPoints.Count < 2) return;

            int numberOfFragments = 1000;
            for (int i = 0; i <= numberOfFragments; i++)
            {
                float t = i / (float)numberOfFragments;
                Vector2 pointOnCurve = DeCasteljau(t, collectedPoints);
                
                bezierCurvePoints.AddRange(new float[] { pointOnCurve.X, pointOnCurve.Y, 0f, 1f, 0f, 0f});
            }
        }

        private Vector2 DeCasteljau(float t, List<Vector2> points)
        {
            List<Vector2> tempPoints = new(points);
            int n = tempPoints.Count;   

            for(int k=0; k <  n - 1; k++)
            {
                for(int i=0; i < n - 1 - k; i++)
                {
                    tempPoints[i]= t * tempPoints[i] + (1 - t) * tempPoints[i+1];   
                }
            }




            return tempPoints[0];
        }

        /// <summary>
        /// method to be called when the screen resolution changed
        /// </summary>
        /// <param name="e">resize event argument</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            screenWidth = e.Width;
            screenHeight = e.Height;
            projMatrix = Matrix4.CreateOrthographicOffCenter(0, screenWidth, 0, screenHeight, 0, 1);
            GL.Viewport(0, 0, e.Width, e.Height);
            GL.ClearColor(Color.BlueViolet);

        }

        #region private fields 
        // current scene to be rendered
        private readonly OpenGLBaseScene scene;
        // viewing matrix
        private Matrix4 viewMatrix = Matrix4.Identity;
        // projection matrix
        private Matrix4 projMatrix;
        // width of the window
        private int screenWidth;
        // height of the window
        private int screenHeight;
        // list of collected points
        private List<Vector2> collectedPoints = new();
        private List<float> bezierCurvePoints = new();
        // geometry to draw;
        private NonIndexedGeometry lineGeometry;
        private NonIndexedGeometry pointGeometry;
        private NonIndexedGeometry bezierGeometry;
        
        #endregion

    }
}
