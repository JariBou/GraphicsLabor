using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Shapes;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Laborers
{
    public class Laborer2D : GraphicLaborer
    {
        #region Quads
        
        public static void DrawQuad(Quad quad, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
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
            
            GL.Color(quad.GetColor);
            GL.Vertex(quad.PointA);
            GL.Vertex(quad.PointB);
            GL.Vertex(quad.PointC);
            GL.Vertex(quad.PointD);
            
            if (drawMode == DrawMode.Wired)
            {
                GL.Vertex(quad.PointA);
            } 
            
            GL.End();
            
            if (drawMode == DrawMode.FilledWithBorders)
            {
                DrawQuad(quad.CopyWithColor(borderColor));
            }
            
            GL.PopMatrix();
        }
        
        public static void DrawQuad(Vector2 center, Vector2 size, Color color, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            DrawQuad(new Quad(center, size, color), drawMode, borderColor);
        }

        public static void DrawQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            DrawQuad(new Quad(a, b, c, d, color), drawMode, borderColor);
        }
        
        // TODO: maybe Queue draw calls? and apply them after... should look into that for optimization
        public static void DrawQuads(IEnumerable<Quad> quads, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            foreach (Quad quad in quads)
            {
                DrawQuad(quad, drawMode, borderColor);
            }
        }
        
        #region OLD

        [Obsolete]
        public static void DrawFilledQuad(Quad quad)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.QUADS);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(quad.GetColor);
            GL.Vertex(quad.PointA);
            GL.Vertex(quad.PointB);
            GL.Vertex(quad.PointC);
            GL.Vertex(quad.PointD);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawFilledQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.QUADS);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(color);
            GL.Vertex(a);
            GL.Vertex(b);
            GL.Vertex(c);
            GL.Vertex(d);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawFilledQuad(Vector2 center, Vector2 size, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.QUADS);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(color);
            GL.Vertex(center - MaskVector2(size, XMask) / 2 + MaskVector2(size, YMask) / 2); // Top left
            GL.Vertex(center + MaskVector2(size, XMask) / 2 + MaskVector2(size, YMask) / 2); // Top Right
            GL.Vertex(center + MaskVector2(size, XMask) / 2 - MaskVector2(size, YMask) / 2); // Bottom Right
            GL.Vertex(center - MaskVector2(size, XMask) / 2 - MaskVector2(size, YMask) / 2); // Bottom Left
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawFilledQuads(IEnumerable<Quad> quads)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.QUADS);
            Instance._renderMaterial.SetPass(0);

            foreach (Quad quad in quads)
            {
                GL.Color(quad.GetColor);
                GL.Vertex(quad.PointA);
                GL.Vertex(quad.PointB);
                GL.Vertex(quad.PointC);
                GL.Vertex(quad.PointD);
            }
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredQuad(Quad quad)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(quad.GetColor);
            GL.Vertex(quad.PointA);
            GL.Vertex(quad.PointB);
            GL.Vertex(quad.PointC);
            GL.Vertex(quad.PointD);
            GL.Vertex(quad.PointA);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(color);
            GL.Vertex(a);
            GL.Vertex(b);
            GL.Vertex(c);
            GL.Vertex(d);
            GL.Vertex(a);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredQuad(Vector2 center, Vector2 size, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(color);
            GL.Vertex(center - MaskVector2(size, XMask) / 2 + MaskVector2(size, YMask) / 2); // Top left
            GL.Vertex(center + MaskVector2(size, XMask) / 2 + MaskVector2(size, YMask) / 2); // Top Right
            GL.Vertex(center + MaskVector2(size, XMask) / 2 - MaskVector2(size, YMask) / 2); // Bottom Right
            GL.Vertex(center - MaskVector2(size, XMask) / 2 - MaskVector2(size, YMask) / 2); // Bottom Left
            GL.Vertex(center - MaskVector2(size, XMask) / 2 + MaskVector2(size, YMask) / 2); // Top left
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredQuads(IEnumerable<Quad> quads)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);

            foreach (Quad quad in quads)
            {
                GL.Color(quad.GetColor);
                GL.Vertex(quad.PointA);
                GL.Vertex(quad.PointB);
                GL.Vertex(quad.PointC);
                GL.Vertex(quad.PointD);
                GL.Vertex(quad.PointA);
            }
            
            GL.End();
            GL.PopMatrix();
        }

        #endregion
        
        #endregion

        // For filled circles can use triangles with center point
        #region Circles
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="drawMode"></param>
        /// <param name="borderColor"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void DrawCircle(Circle circle, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            if (circle.Precision <= 0)
                throw new ArgumentException(
                    $"Invalid value for parameter precision (value of {circle.Precision}, expected more than 0)");
            
            int numberOfPoints = Circle.PointsPerPrecision * circle.Precision;
            
            Vector3 firstPoint = Vector3.zero;
            List<Vector3> circlePoints = new List<Vector3>(numberOfPoints+1);
            
            for (int i = 0; i < numberOfPoints; ++i)
            {
                // Draw triangles
                float a = i / (float)numberOfPoints;
                float angle = a * Mathf.PI * 2;
                circlePoints.Add(new Vector3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y, 0));
                //GL.Vertex3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y, 0);
                if (i == 0)
                    firstPoint = new Vector3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y,
                        0);
            }
            circlePoints.Add(firstPoint);

            switch (drawMode)
            {
                case DrawMode.Wired:
                    DrawBrokenLine(circlePoints, circle.GetColor);
                    break;
                case DrawMode.InternalWired:
                    for (int i = 0; i < circlePoints.Count-1; i++)
                    {
                        Triangle tempTriangle = new Triangle(circle.Center, circlePoints[i], circlePoints[i + 1],
                            circle.GetColor);
                        DrawTriangle(tempTriangle);
                    }
                    break;
                case DrawMode.Filled:
                    for (int i = 0; i < circlePoints.Count-1; i++)
                    {
                        Triangle tempTriangle = new Triangle(circle.Center, circlePoints[i], circlePoints[i + 1],
                            circle.GetColor);
                        DrawTriangle(tempTriangle, DrawMode.Filled);
                    }
                    break;
                case DrawMode.FilledWithBorders:
                    for (int i = 0; i < circlePoints.Count-1; i++)
                    {
                        Triangle tempTriangle = new Triangle(circle.Center, circlePoints[i], circlePoints[i + 1],
                            circle.GetColor);
                        DrawTriangle(tempTriangle, DrawMode.Filled);
                    }
                    DrawBrokenLine(circlePoints, borderColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            } 
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, int precision = 1,
            DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            DrawCircle(new Circle(center, radius, color, precision), drawMode, borderColor);
        }

        public static void DrawCircles(IEnumerable<Circle> circles, int precision = 1,
            DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            foreach (Circle circle in circles)
            {
                DrawCircle(circle, drawMode, borderColor);
            }
        }

        #region OLD

        /// <summary>
        /// 
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="precision">The number of points used to draw the sphere is 45*precision</param>
        [Obsolete]
        public static void DrawWiredCircle(Circle circle, int precision = 1)
        {
            if (precision <= 0)
                throw new ArgumentException(
                    $"Invalid value for parameter precision (value of {precision}, expected more than 0)");
            
            int numberOfPoints = 45 * precision;
            
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            Vector3 firstPoint = Vector3.zero;
            GL.Color(circle.GetColor);
            for (int i = 0; i < numberOfPoints; ++i)
            {
                float a = i / (float)numberOfPoints;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y, 0);
                if (i == 0)
                    firstPoint = new Vector3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y,
                        0);
            }
            GL.Vertex(firstPoint);
            
            GL.End();
            GL.PopMatrix();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        /// <param name="precision">The number of points used to draw the sphere is 45*precision</param>
        [Obsolete]
        public static void DrawWiredCircle(Vector2 center, float radius, Color color, int precision = 1)
        {
            if (precision <= 0)
                throw new ArgumentException(
                    $"Invalid value for parameter precision (value of {precision}, expected more than 0)");
            if (radius < 0) 
                throw new ArgumentException(
                    $"Invalid value for parameter radius (value of {radius}, expected more than 0)");

            int numberOfPoints = 45 * precision;
            
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            Vector3 firstPoint = Vector3.zero;
            GL.Color(color);
            for (int i = 0; i < numberOfPoints; ++i)
            {
                float a = i / (float)numberOfPoints;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(Mathf.Cos(angle) * radius + center.x, Mathf.Sin(angle) * radius + center.y, 0);
                if (i == 0)
                    firstPoint = new Vector3(Mathf.Cos(angle) * radius + center.x, Mathf.Sin(angle) * radius + center.y,
                        0);
            }
            GL.Vertex(firstPoint);
            
            GL.End();
            GL.PopMatrix();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="precision">The number of points used to draw the sphere is 45*precision</param>
        [Obsolete]
        public static void DrawWiredCircles(IEnumerable<Circle> circles, int precision = 1)
        {
            if (precision <= 0)
                throw new ArgumentException(
                    $"Invalid value for parameter precision (value of {precision}, expected more than 0)");
            
            int numberOfPoints = 45 * precision;
            
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            Vector3 firstPoint = Vector3.zero;

            foreach (Circle circle in circles)
            {
                GL.Color(circle.GetColor);
                for (int i = 0; i < numberOfPoints; ++i)
                {
                    float a = i / (float)numberOfPoints;
                    float angle = a * Mathf.PI * 2;
                    GL.Vertex3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y, 0);
                    if (i == 0)
                        firstPoint = new Vector3(Mathf.Cos(angle) * circle.Radius + circle.Center.x, Mathf.Sin(angle) * circle.Radius + circle.Center.y,
                            0);
                }
                GL.Vertex(firstPoint);
            }
            
            GL.End();
            GL.PopMatrix();
        }

        #endregion

        

        #endregion

        #region Polygon
        
        /// <summary>
        /// Filled and InternalWired options are unsafe to use and may cause problems
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="drawMode"></param>
        /// <param name="borderColor"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void DrawPolygon(Polygon polygon, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            CreateLineMaterial();

            switch (drawMode)
            {
                case DrawMode.Filled:
                    DrawTriangles(PolygonTriangulator.TriangulatePolygon(polygon), DrawMode.Filled);
                    break;
                case DrawMode.InternalWired:
                    DrawTriangles(PolygonTriangulator.TriangulatePolygon(polygon));
                    break;
                case DrawMode.Wired:
                    List<Vector2> polygonPoints = new List<Vector2>(polygon.Points);
                    if (polygon.Points[^1] != polygon.Points[0]) polygonPoints.Add(polygon.Points[0]);

                    DrawBrokenLine(polygonPoints, polygon.GetColor);
                    break;
                case DrawMode.FilledWithBorders:
                    DrawTriangles(PolygonTriangulator.TriangulatePolygon(polygon), DrawMode.Filled);
                    DrawPolygon(polygon.CopyWithColor(borderColor));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            }
        }

        /// <summary>
        /// Filled and InternalWired options are unsafe to use and may cause problems
        /// </summary>
        /// <param name="points"></param>
        /// <param name="color"></param>
        /// <param name="drawMode"></param>
        /// <param name="borderColor"></param>
        public static void DrawPolygon(List<Vector2> points, Color color, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            DrawPolygon(new Polygon(points, color), drawMode, borderColor);
        }

        /// <summary>
        /// Filled and InternalWired options are unsafe to use and may cause problems
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="drawMode"></param>
        /// <param name="borderColor"></param>
        public static void DrawPolygons(IEnumerable<Polygon> polygons, DrawMode drawMode = DrawMode.Wired,
            Color borderColor = default)
        {
            foreach (Polygon polygon in polygons)
            {
                DrawPolygon(polygon, drawMode, borderColor);
            }
        }


        #region OLD

        /// <summary>
        /// Automatically closes the polygon if needed
        /// </summary>
        /// <param name="polygon"></param>
        [Obsolete]
        public static void DrawWiredPolygon(Polygon polygon)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(polygon.GetColor);
            foreach (Vector2 point in polygon.Points)
            {
                GL.Vertex(point);
            }
            
            if (polygon.Points[^1] != polygon.Points[0]) GL.Vertex(polygon.Points[0]);
            
            GL.End();
            GL.PopMatrix();
        }
        
        /// <summary>
        /// Automatically closes the polygon if needed
        /// </summary>
        /// <param name="points"></param>
        /// <param name="color"></param>
        [Obsolete]
        public static void DrawWiredPolygon(List<Vector2> points, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);

            GL.Color(color);
            foreach (Vector2 point in points)
            {
                GL.Vertex(point);
            }
            
            if (points[^1] != points[0]) GL.Vertex(points[0]);
            
            GL.End();
            GL.PopMatrix();
        }
        
        /// <summary>
        /// Automatically closes the polygon if needed
        /// </summary>
        /// <param name="polygons"></param>
        [Obsolete]
        public static void DrawWiredPolygons(IEnumerable<Polygon> polygons)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            
            Instance._renderMaterial.SetPass(0);

            foreach (Polygon polygon in polygons)
            {
                GL.Color(polygon.GetColor);
                foreach (Vector2 point in polygon.Points)
                {
                    GL.Vertex(point);
                }
            
                if (polygon.Points[^1] != polygon.Points[0]) GL.Vertex(polygon.Points[0]);
            }
            
            GL.End();
            GL.PopMatrix();
        }

        #endregion
        
        #endregion

        #region Triangles

        public static void DrawTriangle(Triangle triangle, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            CreateLineMaterial();
            
            int glDrawMode;
            switch (drawMode)
            {
                case DrawMode.Wired:
                    glDrawMode = GL.LINE_STRIP;
                    break;
                case DrawMode.Filled:
                    glDrawMode = GL.TRIANGLES;
                    break;
                case DrawMode.FilledWithBorders:
                    glDrawMode = GL.TRIANGLES;
                    if (borderColor == default) borderColor = BaseBorderColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawMode), drawMode, null);
            }
            
            GL.PushMatrix();
            
            GL.Begin(glDrawMode);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(triangle.GetColor);
            GL.Vertex(triangle.PointA);
            GL.Vertex(triangle.PointB);
            GL.Vertex(triangle.PointC);
            
            if (drawMode == DrawMode.Wired)
            {
                GL.Vertex(triangle.PointA);
            } 
            
            GL.End();
            
            if (drawMode == DrawMode.FilledWithBorders)
            {
                DrawTriangle(triangle.CopyWithColor(borderColor));
            }
            
            GL.PopMatrix();
        }
        
        public static void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            DrawTriangle(new Triangle(a, b, c, color), drawMode, borderColor);
        }
        
        public static void DrawTriangles(IEnumerable<Triangle> triangles, DrawMode drawMode = DrawMode.Wired, Color borderColor = default)
        {
            foreach (Triangle triangle in triangles)
            {
                DrawTriangle(triangle, drawMode, borderColor);
            }
        }
        
        #region OLD

        [Obsolete]
        public static void DrawFilledTriangle(Triangle triangle)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.TRIANGLES);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(triangle.GetColor);
            GL.Vertex(triangle.PointA);
            GL.Vertex(triangle.PointB);
            GL.Vertex(triangle.PointC);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawFilledTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.TRIANGLES);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(color);
            GL.Vertex(a);
            GL.Vertex(b);
            GL.Vertex(c);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawFilledTriangles(IEnumerable<Triangle> triangles)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.TRIANGLES);
            Instance._renderMaterial.SetPass(0);

            foreach (Triangle triangle in triangles)
            {
                GL.Color(triangle.GetColor);
                GL.Vertex(triangle.PointA);
                GL.Vertex(triangle.PointB);
                GL.Vertex(triangle.PointC);
            }
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredTriangle(Triangle triangle)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(triangle.GetColor);
            GL.Vertex(triangle.PointA);
            GL.Vertex(triangle.PointB);
            GL.Vertex(triangle.PointC);
            GL.Vertex(triangle.PointA);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);
            
            GL.Color(color);
            GL.Vertex(a);
            GL.Vertex(b);
            GL.Vertex(c);
            GL.Vertex(a);
            
            GL.End();
            GL.PopMatrix();
        }
        
        [Obsolete]
        public static void DrawWiredTriangles(IEnumerable<Triangle> triangles)
        {
            CreateLineMaterial();
            
            GL.PushMatrix();
            
            GL.Begin(GL.LINE_STRIP);
            Instance._renderMaterial.SetPass(0);

            foreach (Triangle triangle in triangles)
            {
                GL.Color(triangle.GetColor);
                GL.Vertex(triangle.PointA);
                GL.Vertex(triangle.PointB);
                GL.Vertex(triangle.PointC);
                GL.Vertex(triangle.PointA);
            }
            
            GL.End();
            GL.PopMatrix();
        }

        #endregion
        
        #endregion
    }
}
