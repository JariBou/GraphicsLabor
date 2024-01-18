using GraphicsLabor.Samples.SampleScripts.SOs;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
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

        [SerializeField, HorizontalSeparator, ReadOnly]
        private string _uneditable = "You can't edit this";
        [SerializeField] private bool _enableEditing;
        [ShowProperty] public int AutoProp1 { get; private set; }
        [ShowProperty(true), Label("Editable Prop")] public int AutoProp2 { get; private set; }
        [ShowProperty, EnableIf(nameof(_enableEditing))] public int AutoProp3 { get; private set; }
    }

    public enum SceneType
    {
        String, Int
    }
}