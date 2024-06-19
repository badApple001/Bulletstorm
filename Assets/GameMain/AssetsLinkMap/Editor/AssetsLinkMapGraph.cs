using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetsLinkMap
{
    internal class AssetsLinkMapGraph : GraphView
    {
        private AssetsLinkMapNode _targetNode;
        internal AssetsLinkMapNode m_targetNode
        {
            get { return _targetNode; }
            set { _targetNode = value; }
        }
        public AssetsLinkMapGraph(UnityEngine.Object targetObj)
        {
            //按照父级的宽高全屏填充
            this.StretchToParentSize();
            //滚轮缩放
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            //graphview窗口内容的拖动
            this.AddManipulator(new ContentDragger());
            //选中Node移动功能
            this.AddManipulator(new SelectionDragger());
            //多个node框选功能
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(new FreehandSelector());

            CreateTargetNode(targetObj);
        }

        public void CreateTargetNode(UnityEngine.Object targetObj)
        {
            _targetNode = AssetsLinkMapNode.CreateNode(targetObj, 0);
        }

        // private static void AddDivider(Node objNode)
        // {
        //     var divider = new VisualElement { name = "divider" };
        //     divider.AddToClassList("horizontal");
        //     objNode.extensionContainer.Add(divider);
        // }

        internal static void GenerateNodeLinks(AssetsLinkMapNode leftNode, AssetsLinkMapNode rightNode)
        {
            if (!leftNode.RightInputs.Contains(rightNode))
            {
                leftNode.RightInputs.Add(rightNode);
            }

            if (!rightNode.LeftInputs.Contains(leftNode))
            {
                rightNode.LeftInputs.Add(leftNode);
            }
        }
    }
}
