using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Editors.Fields
{
    public static class BaseFieldConsts
    {
        public const string USSClassName = "unity-base-field";
        public const string LabelUssClassName = USSClassName + "__label";
        public const string InputUssClassName = USSClassName + "__input";
        public static readonly string NoLabelVariantUssClassName = USSClassName + "--no-label";

        public static readonly string LabelDraggerVariantUssClassName =
            LabelUssClassName + "--with-dragger";

        public static readonly string MixedValueLabelUssClassName =
            LabelUssClassName + "--mixed-value";

        public static readonly string AlignedFieldUssClassName = USSClassName + "__aligned";

        public static readonly string InspectorFieldUssClassName =
            USSClassName + "__inspector-field";

        public static readonly string MixedValueString = "—";

        public static readonly PropertyName SerializedPropertyCopyName =
            (PropertyName)"SerializedPropertyCopyName";

        private static CustomStyleProperty<float> _sLabelWidthRatioProperty =
            new("--unity-property-field-label-width-ratio");

        private static CustomStyleProperty<float> _sLabelExtraPaddingProperty =
            new("--unity-property-field-label-extra-padding");

        private static CustomStyleProperty<float> _sLabelBaseMinWidthProperty =
            new("--unity-property-field-label-base-min-width");
    }
}