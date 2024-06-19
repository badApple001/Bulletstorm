using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace AssetsLinkMap
{
    internal class AssetsLinkMapNode : Node
    {
        public string Name
        {
            get
            {
                if (_targetObject == null)
                {
                    return "(null)";
                }

                string suffix = (_targetObject is UnityEditor.MonoScript) ? " (Script)" : string.Empty;

                return $"{_targetObject.name}{suffix}";
            }
        }
        private UnityEngine.Object _targetObject;
        public UnityEngine.Object m_targetObject
        {
            get { return _targetObject; }
            private set { _targetObject = value; }
        }

        private List<AssetsLinkMapNode> _leftInputs = new List<AssetsLinkMapNode>();
        public List<AssetsLinkMapNode> LeftInputs
        {
            get { return _leftInputs; }
            set { _leftInputs = value; }
        }

        private List<AssetsLinkMapNode> _rightInputs = new List<AssetsLinkMapNode>();
        public List<AssetsLinkMapNode> RightInputs
        {
            get { return _rightInputs; }
            set { _rightInputs = value; }
        }

        private Rect _position;
        public Rect m_position
        {
            get { return _position; }
            set { _position = value; }
        }

        private int _depth;
        public int m_depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        public AssetsLinkMapNode(UnityEngine.Object targetObject)
        {
            _targetObject = targetObject;
        }

        public void SetCustomPosition(Rect posRect)
        {
            this.SetPosition(posRect);
            this._position = posRect;
        }

        public Rect GetCustomPosition()
        {
            return this.m_position;
        }
        public float m_yAxisWeight = -1;
        public int m_distinctedCount = 0;


        public static AssetsLinkMapNode CreateNode(UnityEngine.Object obj, int _depth)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            AssetsLinkMapNode _node = new AssetsLinkMapNode(obj)
            {
                title = obj.name,
                tooltip = obj.ToString(),
                m_depth = _depth,
                style = {
                width = 230f
            }
            };

            _node.extensionContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
            _node.titleButtonContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
            _node.titleContainer.Children().First().style.maxWidth = 120f;
            _node.titleContainer.Children().First().style.overflow = Overflow.Hidden;
            _node.titleButtonContainer.Add(new Button(() =>
                {
                    AssetsLinkMapWindow.Instance.Init(obj);
                })
            {
                style = {
                        height = 16f,
                        alignSelf = Align.Center,
                        alignItems = Align.Center
                    },
                text = "Select"
            });

            var infoContainer = new VisualElement
            {
                style =
                {
                    height = 34f,
                    paddingBottom = 4.0f,
                    paddingTop = 4.0f,
                    paddingLeft = 4.0f,
                    paddingRight = 4.0f,
                    alignContent = Align.FlexStart,
                    flexDirection = FlexDirection.Row
                }
            };

            var typeName = obj.GetType().Name;
            var typeButton = new Button(() =>
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            })
            {
                text = $"{typeName}",
                style = {
                unityTextAlign = TextAnchor.MiddleRight,
                flexGrow = 1
            }
            };
            Texture assetTexture = AssetPreview.GetAssetPreview(obj);
            if (!assetTexture)
                assetTexture = AssetPreview.GetMiniThumbnail(obj);

            if (assetTexture)
            {
                typeButton.Add(new Image
                {
                    image = assetTexture,
                    scaleMode = ScaleMode.ScaleToFit,
                    style =
                    {
                        width = 20,
                        alignItems = Align.FlexStart,
                        flexShrink = 1,
                        flexGrow = 0
                    }
                });
            }
            infoContainer.Add(new Label
            {
                text = "Type: ",
                style = {
                unityTextAlign = TextAnchor.MiddleLeft
            }
            });
            infoContainer.Add(typeButton);

            _node.extensionContainer.Add(infoContainer);

            // Ports
            // if (!isMainNode)
            if (true)
            {
                Port realPort = _node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Object));
                realPort.portName = "Dependent";
                _node.inputContainer.Add(realPort);
            }

            // if (hasDependencies)
            if (true)
            {
#if UNITY_2018_1
            Port port = _node.InstantiatePort(Orientation.Horizontal, Direction.Output, typeof(Object));
#else
                Port port = _node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Object));
#endif
                port.portName = "References";
                _node.outputContainer.Add(port);
                _node.RefreshPorts();
            }

            // resultNode.expanded = false;
            _node.RefreshExpandedState();
            _node.RefreshPorts();
            _node.capabilities &= ~Capabilities.Deletable;
            _node.capabilities |= Capabilities.Collapsible;
            return _node;
        }

        public void SetAsPrefabContainerInfo(GameObject prefabRoot, string newName = null)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                this.title = newName;
            }

            var infoContainer = new VisualElement
            {
                style =
                {
                    height = 34f,
                    paddingBottom = 4.0f,
                    paddingTop = 4.0f,
                    paddingLeft = 4.0f,
                    paddingRight = 4.0f,
                    alignContent = Align.FlexStart,
                    flexDirection = FlexDirection.Row
                }
            };

            var prefabButton = new Button(() =>
            {
                Selection.activeObject = prefabRoot;
                EditorGUIUtility.PingObject(prefabRoot);
            })
            {
                text = $"{prefabRoot.name}",
                style = {
                unityTextAlign = TextAnchor.MiddleRight,
                flexGrow = 1
            }
            };
            Texture assetTexture = AssetPreview.GetAssetPreview(prefabRoot);
            if (!assetTexture)
                assetTexture = AssetPreview.GetMiniThumbnail(prefabRoot);

            if (assetTexture)
            {
                prefabButton.Add(new Image
                {
                    image = assetTexture,
                    scaleMode = ScaleMode.ScaleToFit,
                    style =
                    {
                        width = 20,
                        alignItems = Align.FlexStart,
                        flexShrink = 1,
                        flexGrow = 0
                    }
                });
            }
            infoContainer.Add(new Label
            {
                text = "Prefab:",
                style = {
                unityTextAlign = TextAnchor.MiddleLeft
            }
            });
            infoContainer.Add(prefabButton);

            this.extensionContainer.Add(infoContainer);
        }

        private VisualElement CreateSelectButton(GameObject obj)
        {
            var selectContainer = new VisualElement
            {
                style =
                {
                    height = 34f,
                    paddingBottom = 4.0f,
                    paddingTop = 4.0f,
                    paddingLeft = 4.0f,
                    paddingRight = 4.0f,
                    alignContent = Align.FlexStart,
                    flexDirection = FlexDirection.Row
                }
            };

            var typeName = obj.GetType().Name;
            var typeButton = new Button(() =>
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            })
            {
                text = $"{typeName}",
                style = {
                unityTextAlign = TextAnchor.MiddleRight,
                flexGrow = 1
            }
            };
            Texture tempAssetTexture = AssetPreview.GetAssetPreview(obj);
            if (!tempAssetTexture)
                tempAssetTexture = AssetPreview.GetMiniThumbnail(obj);

            if (tempAssetTexture)
            {
                typeButton.Add(new Image
                {
                    image = tempAssetTexture,
                    scaleMode = ScaleMode.ScaleToFit,
                    style =
                    {
                        width = 20,
                        alignItems = Align.FlexStart,
                        flexShrink = 1,
                        flexGrow = 0
                    }
                });
            }
            selectContainer.Add(new Label
            {
                text = "Type: ",
                style = {
                unityTextAlign = TextAnchor.MiddleLeft
            }
            });
            return selectContainer;
        }
    }
}