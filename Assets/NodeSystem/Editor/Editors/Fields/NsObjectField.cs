using System;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Search;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.Fields
{
    public class NsObjectField : VisualElement
    {
        // ===============================================

        internal new static readonly string ussClassName = "unity-object-field";
        internal new static readonly string labelUssClassName = ussClassName + "__label";
        internal new static readonly string inputUssClassName = ussClassName + "__input";
        internal static readonly string objectUssClassName = ussClassName + "__object";
        internal static readonly string selectorUssClassName = ussClassName + "__selector";
        private readonly NsObjectFieldDisplay _nsObjectField;

        private readonly Action m_AsyncOnProjectOrHierarchyChangedCallback;
        private readonly Action m_OnProjectOrHierarchyChangedCallback;
        public Type objectType;

        public Object value;


        public NsObjectField(string label)
        {
            AddToClassList(Utils.ussClassName);


            AddToClassList(ussClassName);
            Label labelElement = new(label);
            labelElement.focusable = false;
            Add(labelElement);
            labelElement.AddToClassList(Utils.labelUssClassName);
            labelElement.AddToClassList(labelUssClassName);

            VisualElement container = new();
            container.AddToClassList(inputUssClassName);

            _nsObjectField = new NsObjectFieldDisplay(this);
            _nsObjectField.focusable = true;
            _nsObjectField.AddToClassList(objectUssClassName);
            container.Add(_nsObjectField);

            ObjectFieldSelector child = new(this);
            child.AddToClassList(selectorUssClassName);
            container.Add(child);

            Add(container);

            m_AsyncOnProjectOrHierarchyChangedCallback = () => schedule.Execute(m_OnProjectOrHierarchyChangedCallback);
            m_OnProjectOrHierarchyChangedCallback = () => _nsObjectField.Update();
            RegisterCallback((EventCallback<AttachToPanelEvent>)(evt =>
            {
                EditorApplication.projectChanged += m_AsyncOnProjectOrHierarchyChangedCallback;
                EditorApplication.hierarchyChanged += m_AsyncOnProjectOrHierarchyChangedCallback;
            }));
            RegisterCallback((EventCallback<DetachFromPanelEvent>)(evt =>
            {
                EditorApplication.projectChanged -= m_AsyncOnProjectOrHierarchyChangedCallback;
                EditorApplication.hierarchyChanged -= m_AsyncOnProjectOrHierarchyChangedCallback;
            }));
        }


        /// <summary>
        ///   <para>Search query context used to populate the object picker.</para>
        /// </summary>
        public SearchContext searchContext { get; set; }

        /// <summary>
        ///   <para>Search flags used to open the search picker window.</para>
        /// </summary>
        public SearchViewFlags searchViewFlags { get; set; }
        
        /// <summary>
        ///   <para>SearchViewState|Search view state used to configure the object picker.</para>
        /// </summary>
        public SearchViewState searchViewState { get; set; }


        public Type ObjectType
        {
            get => objectType;
            set
            {
                if (value == objectType) return;
                objectType = value;
                _nsObjectField.Update();
            }
        }

        public Object Value
        {
            get => value;
            set
            {
                if (value == this.value) return;
                this.value = value;
                _nsObjectField.Update();
                OnSelection(value, false);
            }
        }

        static UnityEngine.Object ToObject(SearchItem item, System.Type filterType)
        {
            if (item == null || item.provider == null)
                return (UnityEngine.Object) null;
            Func<SearchItem, System.Type, UnityEngine.Object> toObject = item.provider.toObject;
            return toObject != null ? toObject(item, filterType) : (UnityEngine.Object) null;
        }

        internal void ShowObjectSelector()
        {
            if (this.searchContext == null)
                this.searchContext = SearchService.CreateContext("", SearchFlags.None);

            searchViewState.selectHandler = (item, b) => OnSelection(ToObject(item, objectType), b);
            searchViewState.trackingHandler = item => OnObjectChanged(ToObject(item, objectType));
            // SearchContext searchContext1 = searchViewState?.context ?? this.searchContext;
            // SearchContext searchContext2 = new(searchContext1.providers, searchContext1.searchText,
                // searchContext1.options);
            // string title = ObjectNames.NicifyVariableName(objectType.Name);
            // SearchViewState pickerState = SearchViewState.CreatePickerState(title, searchContext2,
            //     this.OnSelection, this.OnObjectChanged,
            //     objectType.ToString(), objectType, this.searchViewFlags);
            
            // if (searchViewState != null)
            // {
            //     pickerState.Assign(searchViewState, searchContext2);
            //     pickerState.SetSearchViewFlags(pickerState.flags | SearchViewFlags.ObjectPicker | this.searchViewFlags);
            //     pickerState.title = title;
            // }
            SearchService.ShowPicker(searchViewState);
        }

        private void OnObjectChanged(Object obj)
        {
            Debug.Log(obj.name);
        }

        private void OnSelection(Object arg1, bool arg2)
        {
            Debug.Log($"Ouaaaaais, arrete  de te brrrrr | {arg1.name}");
        }

        // =========== BaseFieldT ========================
        private static class Utils
        {
            public static readonly string ussClassName = "unity-base-field";
            public static readonly string labelUssClassName = ussClassName + "__label";
            public static readonly string inputUssClassName = ussClassName + "__input";
            public static readonly string noLabelVariantUssClassName = ussClassName + "--no-label";

            public static readonly string labelDraggerVariantUssClassName =
                labelUssClassName + "--with-dragger";

            public static readonly string mixedValueLabelUssClassName =
                labelUssClassName + "--mixed-value";

            public static readonly string alignedFieldUssClassName = ussClassName + "__aligned";

            public static readonly string inspectorFieldUssClassName =
                ussClassName + "__inspector-field";

            public static readonly string mixedValueString = "—";

            public static readonly PropertyName serializedPropertyCopyName =
                (PropertyName)"SerializedPropertyCopyName";

            private static CustomStyleProperty<float> s_LabelWidthRatioProperty =
                new("--unity-property-field-label-width-ratio");

            private static CustomStyleProperty<float> s_LabelExtraPaddingProperty =
                new("--unity-property-field-label-extra-padding");

            private static CustomStyleProperty<float> s_LabelBaseMinWidthProperty =
                new("--unity-property-field-label-base-min-width");
        }
    }
}