using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.Utility;
using GraphicsLabor.Scripts.Core;
using GraphicsLabor.Scripts.Core.Laborers;
using GraphicsLabor.Scripts.Core.Laborers.Utils;
using GraphicsLabor.Scripts.Core.Shapes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GraphicsLabor.Tests
{
    public class TestScript : MonoBehaviour
    {
        [Space, Header("Quad")] 
        public bool _drawQuad;
        public Quad _quad;
        public Color _quadBorderColor;
        [FormerlySerializedAs("_quadDrawMode")] public LaborerDrawMode _quadLaborerDrawMode;
        
        [Space, Header("Cube")] 
        public bool _drawCube;
        public Cube _cube;
        public Color _cubeBorderColor;
        [FormerlySerializedAs("_cubeDrawMode")] public LaborerDrawMode _cubeLaborerDrawMode;
        
        [Space, Header("Circle")]
        public bool _drawCircle;
        public Circle _circle;
        public Color _circleBorderColor;
        [FormerlySerializedAs("_circleDrawMode")] public LaborerDrawMode _circleLaborerDrawMode;

        [Space, Header("Triangle")]
        public bool _drawTriangle;
        public Triangle _triangle;
        public Color _triangleBorderColor;
        [FormerlySerializedAs("_triangleDrawMode")] public LaborerDrawMode _triangleLaborerDrawMode;

        [Space, Header("Polygon"), ShowMessage("Helloooo", MessageType.Info)]
        public bool _drawPolygon;
        public List<Transform> _polygonPoints;
        public Polygon _polygon;
        [TabProperty] public Color _polygonBorderColor;
        [FormerlySerializedAs("_polygonDrawMode")] public LaborerDrawMode _polygonLaborerDrawMode;
        [HideIf(ConditionOperator.Or, "_drawPolygon", "_drawCircle")] public int Hello;
        public bool _showSO;
        [ShowProperty] public int Testooo => Hello;
        [ShowIf("_showSO"), Expandable] public TestEditableScriptableObject _testEditableScriptableObject;
        [Scene] public string _scene;
        
        // public static int test;

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
                Laborer2D.DrawQuad(_quad, _quadLaborerDrawMode, _quadBorderColor);
            }
            if (_drawCircle)
            {
                Laborer2D.DrawCircle(_circle, _circleLaborerDrawMode, _circleBorderColor);
            }
            if (_drawTriangle)
            {
                Laborer2D.DrawTriangle(_triangle, _triangleLaborerDrawMode, _triangleBorderColor);
            }
            if (_drawPolygon)
            {
                GatherPolygonPoints();
                Laborer2D.DrawPolygon(_polygon, _polygonLaborerDrawMode, _polygonBorderColor);
            }
            if (_drawCube)
            {
                Laborer3D.DrawCube(_cube, _cubeLaborerDrawMode, _cubeBorderColor); 
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
