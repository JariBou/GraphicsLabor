using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Laborers;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Shapes
{
    
    [Serializable]
    public class Quad : Shape
    {
        [SerializeField] private Vector2 _pointA;
        [SerializeField] private Vector2 _pointB;
        [SerializeField] private Vector2 _pointC;
        [SerializeField] private Vector2 _pointD;
        private float _angle;
        private Vector2 _center;
        private Vector2 _size;
        
        public Vector2 PointA => _pointA;
        public Vector2 PointB => _pointB;
        public Vector2 PointC => _pointC;
        public Vector2 PointD => _pointD;
        
        public Quad(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD, Color color)
        {
            _pointA = pointA;
            _pointB = pointB;
            _pointC = pointC;
            _pointD = pointD;

            _color = color;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="angle">The angle of the Quad in Radians</param>
        /// <param name="color"></param>
        public Quad(Vector2 center, Vector2 size, Color color, float angle = 0)
        {
            _center = center;
            _size = size;
            
            CalculatePointsWithAngle(angle);
            
            _color = color;
        }

        private Quad(Quad quad)
        {
            _pointA = quad.PointA;
            _pointB = quad.PointB;
            _pointC = quad.PointC;
            _pointD = quad.PointD;

            _color = quad.GetColor;
        }
        
        private Quad(Quad quad, Color color)
        {
            _pointA = quad.PointA;
            _pointB = quad.PointB;
            _pointC = quad.PointC;
            _pointD = quad.PointD;

            _color = color;
        }

        public Quad CopyWithColor(Color color)
        {
            return new Quad(this, color);
        }

        public void SetCenterAndSize()
        {
            _size = new Vector2(_pointB.x - _pointA.x, _pointA.y - _pointD.y);
            _center = new Vector2(_pointA.x + _size.x / 2, _pointA.y + _size.y / 2);
        }

        /// <summary>
        /// Can only be used on Quads that have a center and a size
        /// </summary>
        /// <param name="angle"></param>
        public void CalculatePointsWithAngle(float angle)
        {
            _pointA = _center - GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Cos(angle) / 2 
                      + GraphicLaborer.GetValueAsX((GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Sin(angle) / 2).y)
                
                      + GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Cos(angle) / 2
                      - GraphicLaborer.GetValueAsY((GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Sin(angle) / 2).y); // Top left
            
            _pointB = _center + GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Cos(angle) / 2 
                      + GraphicLaborer.GetValueAsX((GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Sin(angle) / 2).y)
                
                      + GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Cos(angle) / 2
                      + GraphicLaborer.GetValueAsY((GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Sin(angle) / 2).y); // Top Right
            _pointC = _center + GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Cos(angle) / 2 
                      + GraphicLaborer.GetValueAsX((GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Sin(angle) / 2).y)
                
                      - GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Cos(angle) / 2
                      + GraphicLaborer.GetValueAsY((GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Sin(angle) / 2).y); // Bottom Right
            _pointD = _center - GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Cos(angle) / 2 
                      + GraphicLaborer.GetValueAsX((GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Sin(angle) / 2).y)
                
                      - GraphicLaborer.MaskVector2(_size, GraphicLaborer.YMask) * Mathf.Cos(angle) / 2
                      - GraphicLaborer.GetValueAsY((GraphicLaborer.MaskVector2(_size, GraphicLaborer.XMask) * Mathf.Sin(angle) / 2).y); // Bottom Left
            
            _angle = angle;
        }
    }
    
    [Serializable]
    public class Circle : Shape
    {
        [SerializeField] private Vector2 _center;
        [SerializeField, Min(0.001f)] private float _radius;
        [SerializeField, Range(1, 100)] private int _precision;

        public const int PointsPerPrecision = 5;
        
        public Vector2 Center => _center;
        public float Radius => _radius;
        public int Precision => _precision;

        public Circle(Vector2 center, float radius, Color color, int precision = 9)
        {
            _center = center;
            _radius = radius;
            _color = color;
            _precision = precision;
        }
        
        private Circle(Circle circle, Color color)
        {
            _center = circle.Center;
            _radius = circle.Radius;
            _precision = circle.Precision;

            _color = color;
        }

        public Circle CopyWithColor(Color color)
        {
            return new Circle(this, color);
        }
    }
    
    [Serializable]
    public class Polygon : Shape
    {
        [SerializeField] private List<Vector2> _points;
        
        public List<Vector2> Points => _points;

        public Polygon(List<Vector2> points, Color color)
        {
            _points = points;
            _color = color;
        }
        
        private Polygon(Polygon polygon, Color color)
        {
            _points = polygon.Points;

            _color = color;
        }
        
        public Polygon CopyWithColor(Color color)
        {
            return new Polygon(this, color);
        }

        public void ResetPoints(int length)
        {
            _points = new List<Vector2>(length);
        }
    }
    
    [Serializable]
    public class Triangle : Shape
    {
        [SerializeField] private Vector2 _pointA;
        [SerializeField] private Vector2 _pointB;
        [SerializeField] private Vector2 _pointC;
        
        public Vector2 PointA => _pointA;
        public Vector2 PointB => _pointB;
        public Vector2 PointC => _pointC;
        
        public Triangle(Vector2 pointA, Vector2 pointB, Vector2 pointC, Color color)
        {
            _pointA = pointA;
            _pointB = pointB;
            _pointC = pointC;

            _color = color;
        }

        private Triangle(Triangle triangle, Color color)
        {
            _pointA = triangle.PointA;
            _pointB = triangle.PointB;
            _pointC = triangle.PointC;

            _color = color;
        }

        public Triangle CopyWithColor(Color color)
        {
            return new Triangle(this, color);
        }
    }
}
