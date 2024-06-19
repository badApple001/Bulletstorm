using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetsLinkMap
{
    internal class AssetsLinkMapResolver_Dependencies
    {
        private AssetsLinkMapSettings m_settings;

        public AssetsLinkMapResolver_Dependencies(AssetsLinkMapSettings settings)
        {
            m_settings = settings;
        }
        private Dictionary<AssetsLinkMapNode, Dictionary<Type, AssetsLinkMapNode>> _dDistinctDic = new Dictionary<AssetsLinkMapNode, Dictionary<Type, AssetsLinkMapNode>>();
        public Dictionary<AssetsLinkMapNode, Dictionary<Type, AssetsLinkMapNode>> m_dDistinctDic
        {
            get
            {
                return _dDistinctDic;
            }
        }

        public void FindDependencies(AssetsLinkMapNode node, int depth = 1)
        {
            if (node.m_targetObject is GameObject)
            {
                GameObject targetGameObject = node.m_targetObject as GameObject;
                Component[] components = targetGameObject.GetComponents<Component>();

                for (int i = 0; i < components.Length; ++i)
                {
                    FindDependencies(node, components[i], depth);
                }

                if (IsPrefab(node.m_targetObject))
                {
                    ForeachChildrenGameObject(targetGameObject, (childGo) =>
                    {
                        components = childGo.GetComponents<Component>();
                        for (int i = 1; i < components.Length; ++i)
                        {
                            FindDependencies(node, components[i], depth, targetGameObject);
                        }
                    });
                }
            }
            else
            {
                FindDependencies(node, node.m_targetObject, depth);
            }
        }

        private void FindDependencies(AssetsLinkMapNode node, UnityEngine.Object obj, int depth = 1, GameObject prefabRoot = null)
        {
            SerializedObject targetObjectSO = new SerializedObject(obj);
            SerializedProperty sp = targetObjectSO.GetIterator();
            while (sp.NextVisible(true))
            {
                // Debug.LogError(obj.GetHashCode());
                if (sp.propertyType == SerializedPropertyType.ObjectReference &&
                    sp.objectReferenceValue != null &&
                    IsObjectAllowedBySettings(sp.objectReferenceValue))
                {
                    // Dependency found!
                    AssetsLinkMapNode dependencyNode;
                    if (m_settings.DistinctDependenceMonoScript && sp.objectReferenceValue is MonoScript)
                    {
                        var type = targetObjectSO.targetObject.GetType();
                        if (TypeExisted(node, type))
                        {
                            m_dDistinctDic[node][type].m_distinctedCount += 1;
                            continue;
                        }
                        else
                        {
                            dependencyNode = AssetsLinkMapNode.CreateNode(sp.objectReferenceValue, m_settings.DependenciesDepth - depth + 1);
                            dependencyNode.m_distinctedCount += 1;
                            m_dDistinctDic[node].Add(type, dependencyNode);
                        }
                    }
                    else
                    {
                        dependencyNode = AssetsLinkMapNode.CreateNode(sp.objectReferenceValue, m_settings.DependenciesDepth - depth + 1);
                    }
                    AssetsLinkMapGraph.GenerateNodeLinks(node, dependencyNode);
                    if (prefabRoot != null)
                    {
                        // Component comp = obj as Component;
                        dependencyNode.SetAsPrefabContainerInfo(prefabRoot);
                    }

                    if (depth > 1)
                    {
                        FindDependencies(dependencyNode, sp.objectReferenceValue, depth - 1);
                    }
                }
            }
        }

        private bool IsObjectAllowedBySettings(UnityEngine.Object obj)
        {
            return (m_settings.CanObjectTypeBeIncluded(obj));
        }

        public static bool IsPrefab(UnityEngine.Object obj)
        {
            return IsObjectAnAsset(obj) && (obj is GameObject);
        }

        public static bool IsObjectAnAsset(UnityEngine.Object obj)
        {
            return AssetDatabase.Contains(obj);
        }

        public static void ForeachChildrenGameObject(GameObject rootGameObject, Action<GameObject> callback)
        {
            Transform rootTransform = rootGameObject.transform;
            for (int i = 0; i < rootTransform.childCount; ++i)
            {
                Transform childTransform = rootTransform.GetChild(i);
                callback(childTransform.gameObject);
                ForeachChildrenGameObject(childTransform.gameObject, callback);
            }
        }

        public bool TypeExisted(AssetsLinkMapNode leftNode, Type type)
        {
            if (!m_dDistinctDic.ContainsKey(leftNode))
            {
                m_dDistinctDic[leftNode] = new Dictionary<Type, AssetsLinkMapNode>();
            }

            if (m_dDistinctDic[leftNode].ContainsKey(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}