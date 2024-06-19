using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetsLinkMap {
    public class AssetsLinkMapSettings
    {
        private const string AssetsLinkMapSettingsSaveName = "AssetsLinkMapSettings";
        private static AssetsLinkMapSettings _instance;
        public static AssetsLinkMapSettings Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                else
                {
                    return new AssetsLinkMapSettings();
                }
            }
        }

        [Flags]
        public enum ObjectType
        {
            ScriptableObject = 0x01,
            Component = 0x02,
            MonoScript = 0x04,
            Material = 0x08,
            Texture2D = 0x10,
            Shader = 0x20,
            Font = 0x40,
            Sprite = 0x80
        }

        public enum SceneSearchMode
        {
            NoSearch,
            SearchOnlyInCurrentScene,
            SearchEverywhere
        }

        [SerializeField]
        private bool _distinctDependenceMonoScript = true;
        public bool DistinctDependenceMonoScript
        {
            get { return _distinctDependenceMonoScript; }
            set { _distinctDependenceMonoScript = value; }
        }

        [SerializeField]
        private bool _distinctReferenceObjects = true;
        public bool DistinctReferenceObjects
        {
            get { return _distinctReferenceObjects; }
            set { _distinctReferenceObjects = value; }
        }

        [Header("References")]

        [SerializeField]
        [Tooltip("If checked, the references will be searched")]
        private bool _findReferences = true;
        public bool FindReferences
        {
            get { return _findReferences; }
            set { _findReferences = value; }
        }

        [SerializeField]
        [Tooltip("Filters when browsing project files for asset referencing")]
        private string _excludeAssetFilters = ".dll,.a,.so,.asmdef,.aar,.bundle,.jar";
        public string ExcludeAssetFilters
        {
            get { return _excludeAssetFilters; }
            set { _excludeAssetFilters = value; }
        }

        [SerializeField]
        [Tooltip("If set, only these directories will be browsed for references. Can really improve search speed.")]
        private string[] _referencesAssetsDirectories = new string[] { null };
        public string[] ReferencesAssetDirectories
        {
            get { return _referencesAssetsDirectories; }
            set { _referencesAssetsDirectories = value; }
        }

        [SerializeField]
        private SceneSearchMode _sceneSearchType = SceneSearchMode.SearchEverywhere;
        public SceneSearchMode SceneSearchType
        {
            get { return _sceneSearchType; }
            set { _sceneSearchType = value; }
        }

        [Header("Dependencies")]

        [SerializeField]
        [Tooltip("If checked, the dependencies will be searched")]
        private bool _findDependencies = true;
        public bool FindDependencies
        {
            get { return _findDependencies; }
            set { _findDependencies = value; }
        }

        public static int defaultDependenciesDepth = 2;

        [SerializeField]
        [Tooltip("Defines the depth of the search among the dependencies")]
        private int _dependenciesDepth = defaultDependenciesDepth;
        public int DependenciesDepth
        {
            get { return _dependenciesDepth; }
            set { _dependenciesDepth = value; }
        }

        [Header("Common")]

        [SerializeField]
        // [EnumFlags]
        [Tooltip("Defines the object types to analyze")]
        private ObjectType _objectTypesFilter = (ObjectType)0xFFFF;
        public ObjectType ObjectTypesFilter
        {
            get { return _objectTypesFilter; }
            set { _objectTypesFilter = value; }
        }

        [SerializeField]
        private bool _doSearchOnceSettingChange = true;
        public bool DoSearchOnceSettingChange
        {
            get { return _doSearchOnceSettingChange; }
            set { _doSearchOnceSettingChange = value; }
        }

        public AssetsLinkMapSettings()
        {
            _instance = this;
        }

        public void Save()
        {
            var data = EditorJsonUtility.ToJson(this, false);
            EditorPrefs.SetString(AssetsLinkMapSettingsSaveName, data);
        }

        public void Load()
        {
            if (EditorPrefs.HasKey(AssetsLinkMapSettingsSaveName))
            {
                var data = EditorPrefs.GetString(AssetsLinkMapSettingsSaveName);
                EditorJsonUtility.FromJsonOverwrite(data, this);
            }
        }


        public bool CanObjectTypeBeIncluded(UnityEngine.Object refObj)
        {
            if (refObj == AssetsLinkMapWindow.Instance.m_targetObj)
                return true;

            if ((ObjectTypesFilter & ObjectType.ScriptableObject) == 0 &&
                refObj is ScriptableObject)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.Component) == 0 &&
                refObj is Component)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.MonoScript) == 0 &&
                refObj is MonoScript)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.Material) == 0 &&
                refObj is Material)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.Texture2D) == 0 &&
                refObj is Texture2D)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.Shader) == 0 &&
                refObj is Shader)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.Font) == 0 &&
                refObj is Font)
            {
                return false;
            }

            if ((ObjectTypesFilter & ObjectType.Sprite) == 0 &&
                refObj is Sprite)
            {
                return false;
            }
            // Debug.Log(obj.GetType());

            return true;
        }
    }
}