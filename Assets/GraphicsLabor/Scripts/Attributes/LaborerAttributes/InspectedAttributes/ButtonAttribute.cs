using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    public enum EButtonEnableMode
    {
        Always,
        Editor,
        Playmode
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : InspectedAttribute

    {
    public string Text { get; private set; }
    public EButtonEnableMode SelectedEnableMode { get; private set; }

    public ButtonAttribute(string text = null, EButtonEnableMode enabledMode = EButtonEnableMode.Always)
    {
        Text = text;
        SelectedEnableMode = enabledMode;
    }
    }
}