using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Core;
using GraphicsLabor.Scripts.Core.Laborers;
using GraphicsLabor.Scripts.Core.Shapes;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    public class TestScript : MonoBehaviour
    {
        [Space, Header("Quad")] 
        public bool _drawQuad;
        public Quad _quad;
        public Color _quadBorderColor;
        public DrawMode _quadDrawMode;
        
        [Space, Header("Cube")] 
        public bool _drawCube;
        public Cube _cube;
        public Color _cubeBorderColor;
        public DrawMode _cubeDrawMode;
        
        [Space, Header("Circle")]
        public bool _drawCircle;
        public Circle _circle;
        public Color _circleBorderColor;
        public DrawMode _circleDrawMode;

        [Space, Header("Triangle")]
        public bool _drawTriangle;
        public Triangle _triangle;
        public Color _triangleBorderColor;
        public DrawMode _triangleDrawMode;

        [Space, Header("Polygon")]
        public bool _drawPolygon;

        public List<Transform> _polygonPoints;
        public Polygon _polygon;
        public Color _polygonBorderColor;
        public DrawMode _polygonDrawMode;
        [Space, ShowMessage("Hello this is really fun maybe it works idk \nlets see aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", MessageType.Info)] public float _test;
        
        void OnEnable()
        {
            GraphicLaborer.DrawCallback += OnDraw;
        }

        void OnDisable()
        {
            GraphicLaborer.DrawCallback -= OnDraw;
        }

        private void OnDraw()
        {
            if (_drawQuad)
            {
                Laborer2D.DrawQuad(_quad, _quadDrawMode, _quadBorderColor);
            }
            if (_drawCircle)
            {
                Laborer2D.DrawCircle(_circle, _circleDrawMode, _circleBorderColor);
            }
            if (_drawTriangle)
            {
                Laborer2D.DrawTriangle(_triangle, _triangleDrawMode, _triangleBorderColor);
            }
            if (_drawPolygon)
            {
                GatherPolygonPoints();
                Laborer2D.DrawPolygon(_polygon, _polygonDrawMode, _polygonBorderColor);
            }
            if (_drawCube)
            {
                Laborer3D.DrawCube(_cube, _cubeDrawMode, _cubeBorderColor); 
            }
        }
        
        private void OnValidate()
        {
            GatherPolygonPoints();
        }

        [Button]
        private void GatherPolygonPoints()
        {
            _polygon.ResetPoints(_polygonPoints.Count);
            foreach (Transform polygonPoint in _polygonPoints)
            {
                _polygon.Points.Add(polygonPoint.position);
            }
        }
    }
}
