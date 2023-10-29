using System;
using GraphicsLabor.Scripts.Core.Shapes;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Laborers
{
    public class Laborer3D : GraphicLaborer
    {
        #region Faces

        public static void DrawFace(Face face, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            CreateLineMaterial();
            
            int glDrawMode;
            switch (drawMode)
            {
                case DrawMode.Wired:
                    glDrawMode = GL.LINE_STRIP;
                    break;
                case DrawMode.Filled:
                    glDrawMode = GL.QUADS;
                    break;
                case DrawMode.FilledWithBorders:
                    glDrawMode = GL.QUADS;
                    if (borderColor == default) borderColor = BaseBorderColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            }
            
            GL.PushMatrix();
            
            GL.Begin(glDrawMode);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(face.GetColor);

            GL.Vertex(face.PointA);
            GL.Vertex(face.PointB);
            GL.Vertex(face.PointC);
            GL.Vertex(face.PointD);
            if (drawMode == DrawMode.Wired)
            {
                GL.Vertex(face.PointA);
            }
            
            GL.End();
            
            if (drawMode == DrawMode.FilledWithBorders)
            {
                DrawFace(face.CopyWithColor(borderColor));
            }
            
            GL.PopMatrix();
        }

        #endregion
        
        #region Cubes

        /// <summary>
        /// Draws a Cube on screen
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="drawMode">FilledWithBorders mode is not perfect for Method DrawCube</param>
        /// <param name="borderColor"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void DrawCube(Cube cube, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            switch (drawMode)
            {
                case DrawMode.Filled:
                    if (cube.Faces.Count == 0) cube.CreateFaces();
                    foreach (Face cubeFace in cube.Faces)
                    {
                        DrawFace(cubeFace, DrawMode.Filled);
                    }
                    break;
                
                case DrawMode.Wired:
                    if (cube.Faces.Count == 0) cube.CreateFaces();
                    foreach (Face cubeFace in cube.Faces)
                    {
                        DrawFace(cubeFace);
                    }
                    break;
                
                case DrawMode.FilledWithBorders:
                    Debug.Log("DrawMode FilledWithBorders is not perfect for Method DrawCube");
                    if (cube.Faces.Count == 0) cube.CreateFaces();
                    if (borderColor == default) borderColor = BaseBorderColor;
                    foreach (Face cubeFace in cube.Faces)
                    {
                        DrawFace(cubeFace, DrawMode.FilledWithBorders, borderColor);
                    }
                    break;

                case DrawMode.InternalWired:
                    throw new NotImplementedException("InternalWiredMode is not supported for Method DrawCube");
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            }
        }

        #endregion
    }
}
