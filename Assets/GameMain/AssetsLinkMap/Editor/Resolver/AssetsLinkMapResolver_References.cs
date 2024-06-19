using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetsLinkMap
{
    internal class AssetsLinkMapResolver_References
    {
        private AssetsLinkMapGraph m_graph;
        private AssetsLinkMapSettings m_settings;
        private Dictionary<GameObject, AssetsLinkMapNode> _dDistinctDic = new Dictionary<GameObject, AssetsLinkMapNode>();
        public Dictionary<GameObject, AssetsLinkMapNode> m_dDistinctDic
        {
            get
            {
                return _dDistinctDic;
            }
        }

        public AssetsLinkMapResolver_References(AssetsLinkMapGraph graph, AssetsLinkMapSettings settings)
        {
            m_graph = graph;
            m_settings = settings;
        }

        public void FindReferences()
        {
            _dDistinctDic = new Dictionary<GameObject, AssetsLinkMapNode>();
            if (m_settings.SceneSearchType != AssetsLinkMapSettings.SceneSearchMode.NoSearch)
            {
                // Search references in scenes
                List<Scene> currentOpenedScenes = AssetsLinkMapUtils.GetCurrentOpenedScenes();
                FindReferencesAmongGameObjects(m_graph.m_targetNode, currentOpenedScenes);
            }

            bool searchOnlyInCurrentScene = (m_settings.SceneSearchType == AssetsLinkMapSettings.SceneSearchMode.SearchOnlyInCurrentScene);
            if (!searchOnlyInCurrentScene)
            {
                FindReferencesAmongAssets(m_graph.m_targetNode);
            }
        }

        private void FindReferencesAmongGameObjects(AssetsLinkMapNode node, List<Scene> scenes)
        {
            List<GameObject> allGameObjects = GetAllGameObjectsFromScenes(scenes);
            for (int i = 0; i < allGameObjects.Count; ++i)
            {
                GameObject currentGo = allGameObjects[i];
                Component[] components = currentGo.GetComponents<Component>();

                for (int componentIndex = 0; componentIndex < components.Length; ++componentIndex)
                {
                    Component component = components[componentIndex];
                    if (component == null)
                    {
                        continue;
                    }

                    SerializedObject componentSO = new SerializedObject(component);
                    SerializedProperty componentSP = componentSO.GetIterator();

                    while (componentSP.NextVisible(true))
                    {
                        // Reference found!
                        if (componentSP.propertyType == SerializedPropertyType.ObjectReference &&
                            componentSP.objectReferenceValue == node.m_targetObject &&
                            IsObjectAllowedBySettings(component))
                        {
                            AssetsLinkMapNode referenceNode = AssetsLinkMapNode.CreateNode(component, -1);
                            AssetsLinkMapGraph.GenerateNodeLinks(referenceNode, node);
                        }
                    }
                }
            }
        }

        private void FindReferencesAmongAssets(AssetsLinkMapNode node)
        {
            string[] excludeFilters = m_settings.ExcludeAssetFilters.Split(',');

            var allLocalAssetPaths = from assetPath in AssetDatabase.GetAllAssetPaths()
                                     where assetPath.StartsWith("Assets/") && !IsAssetPathExcluded(assetPath, ref excludeFilters)
                                     select assetPath;

            foreach (string assetPath in allLocalAssetPaths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (asset is UnityEditor.SceneAsset)
                    continue;
                UnityEngine.Object[] objs;
                if (asset is Texture2D)
                {
                    objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                }
                else
                {
                    objs = new UnityEngine.Object[] { asset };
                }

                foreach (var obj in objs)
                {
                    if (obj != null)
                    {
                        bool isPrefab = (obj is GameObject);
                        if (isPrefab)
                        {
                            GameObject prefab = obj as GameObject;
                            FindReferencesAmongPrefabChildren(node, prefab, prefab);
                        }
                        else
                        {
                            FindReferencesOnUnityObject(node, obj);
                        }
                    }
                }
            }
        }

        private void FindReferencesOnUnityObject(
            AssetsLinkMapNode node,
            UnityEngine.Object obj,
            GameObject prefabRoot = null)
        {
            SerializedObject objSO = new SerializedObject(obj);
            SerializedProperty sp = objSO.GetIterator();
            while (sp.NextVisible(true))
            {
                if (IsPropertyADependency(sp, node))
                {
                    // Reference found!
                    AssetsLinkMapNode reference;
                    if (prefabRoot != null && m_settings.DistinctReferenceObjects)
                    {
                        if (m_dDistinctDic.TryGetValue(prefabRoot, out AssetsLinkMapNode _nodeExisted))
                        {
                            m_dDistinctDic[prefabRoot].m_distinctedCount += 1;
                            continue;
                        }
                        else
                        {
                            reference = AssetsLinkMapNode.CreateNode(obj, -1);
                            reference.m_distinctedCount += 1;
                            m_dDistinctDic.Add(prefabRoot, reference);
                        }
                    }
                    else
                    {
                        reference = AssetsLinkMapNode.CreateNode(obj, -1);
                    }
                    AssetsLinkMapGraph.GenerateNodeLinks(reference, node);
                    if (prefabRoot != null)
                    {
                        reference.SetAsPrefabContainerInfo(prefabRoot, prefabRoot.name);
                    }
                }
            }
        }

        private void FindReferencesAmongPrefabChildren(
            AssetsLinkMapNode node,
            GameObject gameObject,
            GameObject prefabRoot)
        {
            // Find references among the components of the GameObject...
            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; ++i)
            {
                if (components[i] != null)
                    FindReferencesOnUnityObject(node, components[i], prefabRoot);
            }

            // ...then make same thing on children
            Transform trans = gameObject.transform;
            for (int i = 0; i < trans.childCount; ++i)
            {
                GameObject child = trans.GetChild(i).gameObject;
                FindReferencesAmongPrefabChildren(node, child, prefabRoot);
            }
        }

        private bool IsPropertyADependency(SerializedProperty sp, AssetsLinkMapNode node)
        {
            return sp.propertyType == SerializedPropertyType.ObjectReference &&
                    sp.objectReferenceValue != null &&
                    sp.objectReferenceValue == node.m_targetObject &&
                    IsObjectAllowedBySettings(sp.objectReferenceValue);
        }

        private List<GameObject> GetAllGameObjectsFromScenes(List<Scene> scenes)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            List<GameObject> gameObjectsToCheck = new List<GameObject>();

            List<GameObject> rootGameObjects = new List<GameObject>();
            for (int sceneIdx = 0; sceneIdx < scenes.Count; ++sceneIdx)
            {
                Scene scene = scenes[sceneIdx];
                scene.GetRootGameObjects(rootGameObjects);
                gameObjectsToCheck.AddRange(rootGameObjects);
            }

            for (int gameObjectsToCheckIdx = 0; gameObjectsToCheckIdx < gameObjectsToCheck.Count; ++gameObjectsToCheckIdx)
            {
                GameObject currentGo = gameObjectsToCheck[gameObjectsToCheckIdx];
                for (int childIdx = 0; childIdx < currentGo.transform.childCount; ++childIdx)
                {
                    gameObjectsToCheck.Add(currentGo.transform.GetChild(childIdx).gameObject);
                }
                gameObjects.Add(currentGo);
            }

            return gameObjects;
        }

        private bool IsObjectAllowedBySettings(UnityEngine.Object obj)
        {
            return (m_settings.CanObjectTypeBeIncluded(obj));
        }

        private bool IsAssetPathExcluded(string assetPath, ref string[] excludeFilters)
        {
            for (int i = 0; i < excludeFilters.Length; ++i)
            {
                if (assetPath.EndsWith(excludeFilters[i]))
                {
                    return true;
                }
            }

            if (m_settings.ReferencesAssetDirectories != null &&
                m_settings.ReferencesAssetDirectories.Length > 0)
            {
                bool isAssetAmongReferencesDirectory = false;
                string assetFullPath = Path.GetFullPath(assetPath);
                for (int i = 0; i < m_settings.ReferencesAssetDirectories.Length; ++i)
                {
                    if (string.IsNullOrWhiteSpace(m_settings.ReferencesAssetDirectories[i]))
                    {
                        isAssetAmongReferencesDirectory = true;
                        continue;
                    }
                    if (Directory.Exists(m_settings.ReferencesAssetDirectories[i]))
                    {
                        string referenceAssetFullPath = Path.GetFullPath(m_settings.ReferencesAssetDirectories[i]);
                        if (assetFullPath.StartsWith(referenceAssetFullPath))
                        {
                            isAssetAmongReferencesDirectory = true;
                            break;
                        }
                    }
                }

                return !isAssetAmongReferencesDirectory;
            }

            return false;
        }
    }
}