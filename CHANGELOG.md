# Changelog

All notable changes to GraphicsLabor will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [UNRELEASED] - 2024-??-??

### Added

- Manageable and Editable Attributes for ScriptableObjects 
- SO Editor for Scriptable objects with the Editable Attribute
- SO Creator to ease SO creation for ScriptableOBjects with the Manageable Attribute
- EnableIf/DisableIf Attributes to Enable/Disable fields, works like ShowIf/HideIf 
- AutoProperties with the ShowProperty can now be edited via the inspector
- Links to versions compare in CHANGELOG.md

### Changed

- ShowIfAttributeBase is now abstract and cannot be used as an attribute
- EditorGui.ObjectField now uses allowSceneObjects param
- ShowProperty can now take in a parameter of wether or not to enable PropertyEditing

### Fixed

- ReadOnly Attribute now properly works
- LabelAttribute not working on Properties
- TabAttribute info box now properly displays when using it outside of a ScriptableObject 



## [3.0.2] - 2023-11-07

### Added

- Settigns SO to hold GraphicsLabor custom editor windows related settings

### Changed

- Custom Editor Windows now are remembered when compiling scripts



## [3.0.1] - 2023-11-03

### Added

- SelectedTab button now has a darker background to indicate it is selected


### Changed

- TabProperty now properly works on Properties when no field is part of that Tab
- TabProperty now requires at least 1 tab name to function



## [3.0.0] - 2023-11-02

### Added

- Editable Attribute for SO
- TabProperty Attribute for SO fields and Properties with the ShowIf Attribute that have the Editable Attribute
- Manager Attribute for SO
- ObjectExtension, extension for UnityEngine.Object to test for Inheritance of Types and GetInheritedTypes
- LaborerGUIUtility, a class to hold needed constants for Drawing GUIs
- AssetHandler, handles double clicking on supported assets
- EditorWindowBase, Base class for custom EditorWindows, implements functionality such as opening various windows with the same type
- SciptableObjectEditorWindow, window that enhances scriptable objects' edit
- ShowIf Attributes also works with TabProperty on Fields
- GetAttributes overload for PropertyInfo


### Changed

- Code Refactoring, mainly changes in method scopes and cleaned imports
- ExpandableDrawer now uses LaborerGUIUtility for spacing
- HorizontalSeparatorDrawer now uses LaborerGUIUtility for spacing
- GetSelfAndBaseTypes is replaced by Object.GetTypes()
- Updated CHANGELOG


### Know Bugs

- Opening a custom Editor window after recompilation always creates a new window



## [2.0.0] - 2023-10-29

### Added

- ShowIf Attribute
- HideIf Attribute
- Expandable Attribute for ScriptableObjects
- HorizontalSeparator Decoration Attribute
- ColorExtension for HorizontalSeparator 
- ShowProperty Attribute
- Scene Attribute
- Drawers for Expandable, Scene and BasePropertyDrawer
- A TestScriptableObject to test Attributes
- Documentation on certain Functions, more will be made on future patches


### Changed

- ReadOnly Attribute now is usable with other attributes and handled by the GraphicsLaborInspector
- ReadOnly Attribute can no longer change the display name of the field
- TestScrip to show usage of Attributes


### Removed

- ReadOnlyDrawer (not necessary anymore)
- CHANGELOG and README meta files



## [1.1.1] - 2023-10-29

### Added

- CHANGELOG.md
- README.md


### Changed
- License from MIT to GNU
- DrawMode enum to LaborerDrawMode to avoid name conflicts



## [1.1.0] - 2023-10-29

### Added

- Custom Attributes (ReadOnly, Button and ShowMessage)

### Changed

- Code Refactoring


[UNRELEASED]: https://github.com/JariBou/GraphicsLabor/compare/v3.0.2...HEAD
[3.0.2]: https://github.com/JariBou/GraphicsLabor/compare/v3.0.1...v3.0.2
[3.0.1]: https://github.com/JariBou/GraphicsLabor/compare/v3.0.0...v3.0.1
[3.0.0]: https://github.com/JariBou/GraphicsLabor/compare/v2.0.0...v3.0.0
[2.0.0]: https://github.com/JariBou/GraphicsLabor/compare/v1.1.1...v2.0.0
[1.1.1]: https://github.com/JariBou/GraphicsLabor/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/JariBou/GraphicsLabor/releases/tag/v1.1.0