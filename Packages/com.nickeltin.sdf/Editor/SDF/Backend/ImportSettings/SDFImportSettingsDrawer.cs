﻿using System;
using System.Collections.Generic;
using System.Linq;
using nickeltin.Core.Editor;
using nickeltin.SDF.Editor.DecoupledPipeline;
using nickeltin.SDF.InternalBridge.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace nickeltin.SDF.Editor
{
    /// <summary>
    /// This drawer takes care of drawing properties that is currently used by <see cref="SDFImportSettings.SDFGenerationBackend"/> 
    /// </summary>
    [CustomPropertyDrawer(typeof(SDFImportSettings))]
    internal class SDFImportSettingsDrawer : PropertyDrawer
    {
        private static class Defaults
        {
            public static readonly GUIContent DropdownContent;
            public static readonly GUIContent BackendContent;
            public static readonly HashSet<string> DefaultProperties;

            public static readonly GUIContent Hierarchy;
            public static readonly GUIContent CreateSDFAsset;
            
            static Defaults()
            {
                Hierarchy = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.SceneHierarchyWindow"))
                {
                    tooltip = "Find SDFAsset's using this texture"
                };

                CreateSDFAsset = new GUIContent(EditorGUIUtility.IconContent("d_CreateAddNew"))
                {
                    tooltip = "Create SDFAsset(s) [Experimental]" 
                };
                
                DropdownContent = new GUIContent();
                DefaultProperties = new HashSet<string>
                {
                    nameof(SDFImportSettings.GenerateSDF),
                    nameof(SDFImportSettings.SDFGenerationBackend),
                    nameof(SDFImportSettings.BorderOffset),
                    nameof(SDFImportSettings.ResolutionScale),
                };
                BackendContent = new GUIContent
                {
                    text = "Generation Backend"
                };
            }
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Draw(rect, property, label, true, true, false);
        }

        private static IEnumerable<SerializedProperty> GetAdditionalProperties(SerializedProperty mainProp, SerializedProperty backendProp)
        {
            foreach (var entry in mainProp.GetVisibleChilds())
            {
                if (IsDefaultProperty(mainProp) || !PropertyShouldBeDrawn(backendProp.stringValue, entry.name))
                {
                    continue;
                }

                yield return entry;
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label, true, true, false);
        }

        public static void DrawLayout(SerializedProperty property, bool drawEnabled = true)
        {
            var height = GetHeight(property, GUIContent.none, false, drawEnabled, false);
            var rect = EditorGUILayout.GetControlRect(false, height);
            Draw(rect, property, GUIContent.none, false, drawEnabled, false);
        }

        public static void DrawLayoutForImporter(SerializedProperty property, bool drawOptions, 
            IReadOnlyCollection<TextureImporter> importers, IReadOnlyCollection<SDFImportSettings> settings)
        {
            var height = GetHeight(property, GUIContent.none, false, true, true);
            var rect = EditorGUILayout.GetControlRect(false, height);
            Draw(rect, property, GUIContent.none, false, true, true);

            if (!drawOptions) return;
            
            const int buttonWidth = 38;
            var familyRect = EditorGUI.IndentedRect(rect);
            familyRect.height = EditorGUIUtility.singleLineHeight;
            familyRect.x += familyRect.width - buttonWidth;
            familyRect.width = buttonWidth;
            familyRect.y += 4;
            using (new EditorGUI.DisabledScope(importers.Count != 1))
            {
                if (EditorGUI.DropdownButton(familyRect, Defaults.Hierarchy, FocusType.Passive))
                {
                    if (!SDFAssetsFamilyPopup.isOpen)
                    {
                        PopupWindow.Show(familyRect, new SDFAssetsFamilyPopup(importers.FirstOrDefault()));
                    }
                }
            }

            familyRect.x -= familyRect.width + 2;
            if (GUI.Button(familyRect, Defaults.CreateSDFAsset))
            {
                var count = importers.SelectMany(importer =>
                {
                    var texGUID = AssetDatabase.GUIDFromAssetPath(importer.assetPath).ToString();
                    return SDFEditorUtil.FindSDFAssetsThatUsesTexture(texGUID);
                }).Count();
                
                if (count > 0)
                {
                    if (EditorUtility.DisplayDialog("Generate SDFAsset(s)?",
                            $"SDF Asset(s) ({count}) for this texture(s) is already present in project", 
                            "Generate new anyways", "Abort generation"))
                    {
                        Generate();  
                    }
                }
                else Generate();

                void Generate()
                {
                    EditorApplication.delayCall += () =>
                    {
                        Selection.objects = Array.Empty<Object>();
                        var assetPaths = SDFAssetImporter.CreateForTextures(importers, settings);
                        Selection.objects = assetPaths.Select(s => AssetDatabase.LoadAssetAtPath(s, typeof(Object)))
                            .ToArray();
                    };
                }
            }
        }
        
        public static void Draw(Rect rect, SerializedProperty property, GUIContent label, 
            bool expandable, bool drawEnabled, bool collapseIfDisabled)
        {
            var drawHelper = new DrawHelper(rect);
            
            EditorGUI.BeginProperty(rect, label, property);
            
            if (expandable)
            {
                drawHelper.AddRect();
                property.isExpanded = EditorGUI.Foldout(drawHelper.CurrentRect, property.isExpanded, label, true);
            }

            if ((!expandable || !property.isExpanded) && expandable)
            {
                EditorGUI.EndProperty();
                return;
            }

            using (new EditorGUI.IndentLevelScope(expandable ? 1 : 0))
            {
                var enabled = property.FindPropertyRelative(nameof(SDFImportSettings.GenerateSDF));
                
                if (drawEnabled)
                {
                    drawHelper.DrawProperty(enabled);
                    if (collapseIfDisabled && !enabled.boolValue)
                    {
                        EditorGUI.EndProperty();
                        return;
                    }
                }
                
                var backendProp = property.FindPropertyRelative(nameof(SDFImportSettings.SDFGenerationBackend));
                var borderOffset = property.FindPropertyRelative(nameof(SDFImportSettings.BorderOffset));
                var resolutionScale = property.FindPropertyRelative(nameof(SDFImportSettings.ResolutionScale));
                
                using (new EditorGUI.IndentLevelScope(collapseIfDisabled ? 1 : 0))
                using (new EditorGUI.DisabledScope(drawEnabled && !enabled.boolValue))
                {
                    drawHelper.DrawProperty(borderOffset);
                    drawHelper.DrawProperty(resolutionScale);
                    
                    var backend = SDFGenerationBackend.Provider.GetBackend(backendProp.stringValue).Backend;
                    var desc = backend.BaseData.Description;
                    if (!string.IsNullOrEmpty(desc))
                    {
                        drawHelper.DrawHelpBox(desc, MessageType.Info);
                    }
                    
                    // Draw rect is modified by prefix label.
                    drawHelper.AddRect();
                    
                    var drawRect = EditorGUI.PrefixLabel(drawHelper.CurrentRect, Defaults.BackendContent);
                    {
                        Defaults.DropdownContent.text = backend.GetDisplayName();
                        if (backendProp.hasMultipleDifferentValues)
                        {
                            Defaults.DropdownContent.text = "—";
                        }
                        
                        // Button drawn with modified rect
                        if (EditorGUI.DropdownButton(drawRect, Defaults.DropdownContent, FocusType.Passive))
                        {
                            var menu = new GenericMenu();
                            foreach (var backendContext in SDFGenerationBackend.Provider.GetBackends())
                            {
                                if (backendContext.Backend.BaseData.HideFromInspector)
                                {
                                    continue;
                                }
                                var id = backendContext.Backend.BaseData.Identifier;
                                var content = new GUIContent(backendContext.Backend.GetDisplayName());
                                menu.AddItem(content, backendProp.stringValue == id && !backendProp.hasMultipleDifferentValues, () =>
                                {
                                    backendProp.stringValue = id;
                                    backendProp.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            menu.ShowAsContext();
                        }
                    }
                    
                    if (backend.BaseData.Obsolete)
                    {
                        drawHelper.DrawHelpBox("This backend marked obsolete and will be removed in future", MessageType.Warning);
                    }
                    
                    if (backendProp.hasMultipleDifferentValues)
                    {
                        drawHelper.DrawHelpBox("Different backends selected", MessageType.Warning);
                    }
                    else
                    {
                        foreach (var additionalProperty in GetAdditionalProperties(property, backendProp))
                        {
                            drawHelper.DrawProperty(additionalProperty);
                        }
                    }
                    
                }
            }
            
            EditorGUI.EndProperty();
        }
        
        public static float GetHeight(SerializedProperty property, GUIContent label, bool expandable, bool drawEnabled, bool collapseIfDisabled)
        {
            var drawHelper = new DrawHelper(Rect.zero);
            if (expandable)
            {
                drawHelper.AddRect();
                if (!property.isExpanded)
                {
                    return drawHelper.TotalHeight;
                }
            }
            
            var enabled = property.FindPropertyRelative(nameof(SDFImportSettings.GenerateSDF));

            if (drawEnabled)
            {
                drawHelper.AddProperty(enabled);
                if (collapseIfDisabled && !enabled.boolValue)
                {
                    return drawHelper.TotalHeight;
                }
            }
            
            var borderOffset = property.FindPropertyRelative(nameof(SDFImportSettings.BorderOffset));
            var resolutionScale = property.FindPropertyRelative(nameof(SDFImportSettings.ResolutionScale));
            var backendProp = property.FindPropertyRelative(nameof(SDFImportSettings.SDFGenerationBackend));
            
            drawHelper.AddProperty(borderOffset);
            drawHelper.AddProperty(resolutionScale);
            drawHelper.AddProperty(backendProp);

            var backend = SDFGenerationBackend.Provider.GetBackend(backendProp.stringValue).Backend;
            // If has description drawing it.
            if (!string.IsNullOrEmpty(backend.BaseData.Description)) drawHelper.AddRect();
            // If backend obsolete drawing message about it.
            if (backend.BaseData.Obsolete) drawHelper.AddRect();
            
            // If multiple backends selected drawing message about it.
            if (backendProp.hasMultipleDifferentValues)
            {
                drawHelper.AddRect();
            }
            // Else draw all additional backend properties
            else
            {
                foreach (var additionalProperty in GetAdditionalProperties(property, backendProp))
                {
                    drawHelper.AddProperty(additionalProperty);
                }
            }

            return drawHelper.TotalHeight;
        }
        
        private static bool IsDefaultProperty(SerializedProperty property)
        {
            return Defaults.DefaultProperties.Contains(property.name);
        }
        
        private static bool PropertyShouldBeDrawn(string backendId, string propName)
        {
            var backend = SDFGenerationBackend.Provider.GetBackend(backendId);
            return backend.IsPropertyUsed(propName);
        }
    }
}