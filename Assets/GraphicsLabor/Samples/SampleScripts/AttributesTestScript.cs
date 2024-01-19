﻿using GraphicsLabor.Samples.SampleScripts.SOs;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.Utility;
using GraphicsLabor.Scripts.Core.Tags;
using UnityEngine;

namespace GraphicsLabor.Samples.SampleScripts
{
    public class AttributesTestScript : LaborBehaviour
    {
        [SerializeField] private bool _showSo;

        [SerializeField, Expandable, ShowIf("_showSo")]
        private AnotherEditableScriptableObject _so;

        [SerializeField, HorizontalSeparator, ShowMessage("Select if you want the scene by int or string")] private SceneType _sceneType;
        [SerializeField, Scene, HideIf(nameof(_sceneType), SceneType.Int)]
        private string _sceneString;
        [SerializeField, Scene, HideIf(nameof(_sceneType), SceneType.String)]
        private int _sceneInt;

        [SerializeField, HorizontalSeparator(5, LaborColor.Blue), ReadOnly]
        private string _uneditable = "You can only read this";
        [SerializeField] private bool _enableEditingOfProp3;
        [ShowProperty] public int AutoProp1 { get; private set; }
        [ShowProperty(true), Label("Editable Prop")] public int AutoProp2 { get; private set; }
        [ShowProperty, EnableIf(nameof(_enableEditingOfProp3))] public int AutoProp3 { get; private set; }

        [SerializeField, HorizontalSeparator, TabProperty("")]
        private GameObject _invalidTab;
        [SerializeField, Scene] private GameObject _invalidScene;
    }

    public enum SceneType
    {
        String, Int
    }
}