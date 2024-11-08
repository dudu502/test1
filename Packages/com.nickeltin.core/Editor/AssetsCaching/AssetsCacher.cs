﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

using AMP = UnityEditor.AssetModificationProcessor;

namespace nickeltin.Core.Editor
{
    public sealed class AssetsCacher : AMP
    {
        private static class Provider
        {
            private static readonly Dictionary<Type, List<IAssetCacher>> _cachers;
            static Provider()
            {
                _cachers = new Dictionary<Type, List<IAssetCacher>>();
                var cahcerTypes = TypeCache.GetTypesDerivedFrom<IAssetCacher>();
                foreach (var cacherType in cahcerTypes)
                {
                    var instance = (IAssetCacher)Activator.CreateInstance(cacherType);
                
                    if (_cachers.ContainsKey(instance.TargetedType))
                    {
                        _cachers[instance.TargetedType].Add(instance);
                    }
                    else
                    {
                        _cachers.Add(instance.TargetedType, new List<IAssetCacher> {instance});
                    }
                }
            }
            
            public static IEnumerable<IAssetCacher> EnumerateAllCachers() => _cachers.Values.SelectMany(l => l);
        }
        

        private static bool TypeValid(IAssetCacher forCacher, Type type)
        {
            return forCacher.TargetedType == type || type.IsSubclassOf(forCacher.TargetedType);
        }
        
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (assetType == null) return AssetDeleteResult.DidNotDelete;

            foreach (var cacher in Provider.EnumerateAllCachers())
            {
                if (TypeValid(cacher, assetType))
                {
                    var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                    cacher.OnAssetDeletion(asset);
                    return AssetDeleteResult.DidNotDelete;
                }
            }
            
            return AssetDeleteResult.DidNotDelete;
        }

        internal static void OnAssetsCreated(string[] createdPaths)
        {    
            foreach (var assetPath in createdPaths)
            {
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                if (assetType == null) continue;
                
                foreach (var cacher in Provider.EnumerateAllCachers())
                {
                    if (TypeValid(cacher, assetType))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                        cacher.OnAssetCreation(asset);
                        break;
                    }
                }
            }
        }

        public static void AddCreatedAsset<T>(ref T[] array, T obj)
        {
            array ??= Array.Empty<T>();
            if (!array.Contains(obj)) ArrayUtility.Add(ref array, obj);
        }

        public static void RemoveDeletedAsset<T>(ref T[] array, T obj)
        {
            array ??= Array.Empty<T>();
            if (array.Contains(obj)) ArrayUtility.Remove(ref array, obj);
        }
    }
}