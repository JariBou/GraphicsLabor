# Graphics Labor 

![version](https://img.shields.io/badge/Version-4.0.4-blue)
[![license](https://img.shields.io/badge/License-GNU-green)](https://github.com/KSXGitHub/GPL-3.0)
[![author](https://img.shields.io/badge/Author-JariBou-orange)](https://jaribou.github.io/)  
![unity](https://img.shields.io/badge/Unity-2022.3-blue)  
Graphics labor is a unity package composed as of **v4.0.0** of 4 main parts:
- GraphicLaborer
- LaborerAttributes
- LaborerEditor
- LaborerTags


It's main goal is to help Game Programmers and Game Designers to make a game.
[![Doc files](https://img.shields.io/badge/Doc-8A2BE2)](https://github.com/JariBou/GraphicsLabor/tree/main/Assets/Documentation)

> [!IMPORTANT]
> DISCLAIMER: This package was made using Unity 2022.3.12f1.
> It has been tested from version 2022.3.8f1 up and there is no guarantee that it will work for any lower version.
> Tests show compatibility with Unity 2021 and Unity 2023.

## Installation

1. You can add the package to your assets from the Unity Asset Store:
```
https://assetstore.unity.com/packages/tools/utilities/graphicslabor-274494
```
2. You can install the package in unity via the package manager by using this link:
```
https://github.com/JariBou/GraphicsLabor.git?path=/Assets/GraphicsLabor#main
```
3. You can choose the version you want to use by changing the target branch (also works with tags):
```
https://github.com/JariBou/GraphicsLabor.git?path=/Assets/GraphicsLabor#v4.0.3
```
4. You can download the last package version uploaded to github and manually import it:
```
https://github.com/JariBou/GraphicsLabor/releases/latest
```


## Overview

### GraphicLaborer:
Provides simple solutions to draw shapes on the screen while in play mode. Works in both the editor and the build.  
Its main purpose is to allow for debugging 2D Hitboxes and Hurtboxes in games.

### LaborerAttributes:
Provides Custom Attributes for all Scripts:
- ReadOnly
- ShowMessage
- Button
- ShowIf/HideIf
- EnableIf/DisableIf
- ShowProperty
- Label
- Expandable
- Scene
- HorizontalSeparator

It also allows to enable Auto-Properties editing via the inspector using the ShowProperty or EnableIf/DisableIf Attributes
It also provides ScriptableObjects-only Attributes aimed to be used with the custom editor window:
- Editable
- Manageable
- TabProperty

### LaborerEditor:
Provides Custom Editor Window to help managing ScriptableObjects:
- ScriptableObjects Editor
- ScriptableObjects Creator

### LaborerTags:

Provides User-defined tags via the GL Settings window that can be used with a ITagHolder component. Labor Tags are BitMasks and thus multiple can be applied to a single object. Graphics Labor provides 2 ready-to-use components:
- LaborTagComponent
- LinkedLaborTagComponent
As well as a MonoBehaviour subclass that natively implements LaborTags:
- LaborBehaviour

*Rest Coming Soon...*

