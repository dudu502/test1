﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace nickeltin.Core.Editor
{
    /// <summary>
    /// Inherit from this class to draw icon in hierarchy for MonoBehaviour
    /// </summary>
    public abstract class HierarchyIconsDrawer
    {
        private const int ICON_SIZE = 20;

        private static List<HierarchyIconsDrawer> _drawers;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            var types1 = new HashSet<Type>();
            _drawers = new List<HierarchyIconsDrawer>();
            var types = TypeCache.GetTypesDerivedFrom<HierarchyIconsDrawer>();
            foreach (var type in types)
            {
                if (!types1.Add(type) || type.IsAbstract)
                {
                    continue;
                }

                var instance = Activator.CreateInstance(type);
                _drawers.Add((HierarchyIconsDrawer)instance);
            }
        }

        private static void HierarchyWindowItemOnGUI(int instanceId, Rect rect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (gameObject == null) return;
            
            foreach (var drawer in _drawers)
            {
                if (!drawer.ShouldDraw) return;

                var component = (MonoBehaviour)gameObject.GetComponent(drawer.TargetType);
                    
                if (component == null) continue;
                    
                var isPrefab = PrefabUtility.IsPartOfAnyPrefab(gameObject);
                if (isPrefab)
                {
                    rect.x += rect.width - ICON_SIZE;
                }
                else
                {
                    rect.x += rect.width - 2;
                }
                rect.width = ICON_SIZE;
                var icon = AssetPreview.GetMiniThumbnail(component);
                var wasEnabled = GUI.enabled;
                GUI.enabled = component.enabled && gameObject.activeInHierarchy;
                GUI.Label(rect, icon);
                GUI.enabled = wasEnabled;
                    
                return;
            }
        }
        
        public abstract Type TargetType { get; }
        
        public abstract bool ShouldDraw { get; }
    }
}