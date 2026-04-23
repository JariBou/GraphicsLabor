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
        private const string USSClassName = "unity-object-field";
        private const string LabelUssClassName = USSClassName + "__label";
        private const string InputUssClassName = USSClassName + "__input";
        private const string ObjectUssClassName = USSClassName + "__object";
        private const string SelectorUssClassName = USSClassName + "__selector";

        private readonly NsObjectFieldDisplay _nsObjectField;

        private readonly Action _onProjectOrHierarchyChangedCallback;
        private Type _objectType;

        private Object _value;


        public NsObjectField(string label)
        {
            AddToClassList(BaseFieldConsts.USSClassName);


            AddToClassList(USSClassName);
            Label labelElement = new(label)
            {
                focusable = false
            };

            Add(labelElement);
            labelElement.AddToClassList(BaseFieldConsts.LabelUssClassName);
            labelElement.AddToClassList(LabelUssClassName);

            VisualElement container = new();
            container.AddToClassList(InputUssClassName);
            container.AddToClassList(BaseFieldConsts.InputUssClassName);

            _nsObjectField = new NsObjectFieldDisplay(this)
            {
                focusable = true
            };
            _nsObjectField.AddToClassList(ObjectUssClassName);
            container.Add(_nsObjectField);

            ObjectFieldSelector child = new(this);
            child.AddToClassList(SelectorUssClassName);
            container.Add(child);

            Add(container);

            Action asyncOnProjectOrHierarchyChangedCallback =
                () => schedule.Execute(_onProjectOrHierarchyChangedCallback);
            _onProjectOrHierarchyChangedCallback = () =>
            {
                ResetSearchService();
                EnsureSearchServiceReady();
                _nsObjectField.Update();
            };
            RegisterCallback((EventCallback<AttachToPanelEvent>)(_ =>
            {
                EditorApplication.projectChanged += asyncOnProjectOrHierarchyChangedCallback;
                EditorApplication.hierarchyChanged += asyncOnProjectOrHierarchyChangedCallback;
            }));
            RegisterCallback((EventCallback<DetachFromPanelEvent>)(_ =>
            {
                EditorApplication.projectChanged -= asyncOnProjectOrHierarchyChangedCallback;
                EditorApplication.hierarchyChanged -= asyncOnProjectOrHierarchyChangedCallback;
            }));
        }


        /// <summary>
        ///     <para>Search query context used to populate the object picker.</para>
        /// </summary>
        private SearchContext SearchContext { get; set; }

        /// <summary>
        ///     <para>Search flags used to open the search picker window.</para>
        /// </summary>
        public SearchViewFlags SearchViewFlags { get; set; }

        /// <summary>
        ///     <para>SearchViewState|Search view state used to configure the object picker.</para>
        /// </summary>
        private SearchViewState SearchViewState { get; set; }


        public Type ObjectType
        {
            get => _objectType;
            set
            {
                if (value == _objectType) return;
                _objectType = value;
                _nsObjectField.Update();
            }
        }

        public Object Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                if (!ObjectType.IsAssignableFrom(value.GetType())) return;
                _value = value;
                _nsObjectField.Update();
                DoSelectionCallbacks(value, false);
            }
        }

        public SearchProvider SearchProvider { get; set; }

        public bool HideTabs { get; set; }
        public GUIContent WindowTitle { get; set; }

        private Action<Object, bool> OnSelectionCallback { get; set; }
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global Justification: User can set these
        public bool PreventDefaultSelectionHandler { get; set; } = false;
        public Action<Object> OnTrackCallback { get; set; } = null;
        public bool PreventDefaultTrackingHandler { get; set; } = false;
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        private void ResetSearchService()
        {
            SearchContext = null;
            SearchViewState = null;
        }

        public static Object ToObject(SearchItem item, Type filterType)
        {
            Func<SearchItem, Type, Object> toObject = item?.provider?.toObject;
            return toObject?.Invoke(item, filterType);
        }

        internal void ShowObjectSelector()
        {
            ResetSearchService();
            EnsureSearchServiceReady();

            SearchService.ShowPicker(SearchViewState);
        }

        private void EnsureSearchServiceReady()
        {
            SearchContext ??= SearchProvider == null
                ? SearchService.CreateContext("", SearchFlags.None)
                : SearchService.CreateContext(SearchProvider);

            SearchViewState ??= new SearchViewState(SearchContext, SearchViewFlags | SearchViewFlags.ObjectPicker)
            {
                hideTabs = HideTabs,
                windowTitle = WindowTitle ?? new GUIContent($"Select a {ObjectType.Name}"),
                selectHandler = DoSelectionCallbacks,
                trackingHandler = item =>
                {
                    if (!PreventDefaultTrackingHandler) OnObjectChanged(ToObject(item, _objectType));

                    OnTrackCallback?.Invoke(ToObject(item, _objectType));
                }
            };
        }

        private void DoSelectionCallbacks(SearchItem item, bool cancelled)
        {
            if (!PreventDefaultSelectionHandler) OnSelection(ToObject(item, _objectType), cancelled);
            OnSelectionCallback?.Invoke(ToObject(item, _objectType), cancelled);
        }

        private void DoSelectionCallbacks(Object obj, bool cancelled)
        {
            if (!PreventDefaultSelectionHandler) OnSelection(obj, cancelled);
            OnSelectionCallback?.Invoke(obj, cancelled);
        }

        private void OnObjectChanged(Object _)
        {
        }

        private void OnSelection(Object obj, bool cancelled)
        {
            if (cancelled)
            {
                return;
            }

            _value = obj;
            _nsObjectField.Update();
        }

        public void SetValueWithoutNotify(Object newValue)
        {
            if (newValue == _value) return;
            if (!ObjectType.IsAssignableFrom(newValue.GetType())) return;
            MarkDirtyRepaint();
            _value = newValue;
            _nsObjectField.Update();
        }

        public void RegisterSelectionCallback(Action<Object /*newValue*/, bool /*cancelled*/> callback)
        {
            OnSelectionCallback = callback;
        }
    }
}