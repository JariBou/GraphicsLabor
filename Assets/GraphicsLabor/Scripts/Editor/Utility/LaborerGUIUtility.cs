using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class LaborerGUIUtility
    {
        public static float ScrollBarWidth => 16;
        public static float SingleLineHeight => EditorGUIUtility.singleLineHeight;
        public static float PropertyHeightSpacing => SingleLineHeight * 0.1f;

        public static float LineSeparatorSpacing => SingleLineHeight / 3f;
        public static float LabelWidth => EditorGUIUtility.labelWidth;
        public static float MinHelpBoxHeight => SingleLineHeight * 2f;
        public static float CurrentViewWidth => EditorGUIUtility.currentViewWidth;

        #region Window Colors
            public static Color SelectedTabColor => Color.gray;
            public static Color BaseBackgroundColor => new(0.9f, 0.9f, 0.9f);
        #endregion
    }
}