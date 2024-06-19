using System.Linq;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetsLinkMap {
    internal class AssetsLinkMapGraphDrawer {
        enum NodeInputSide { Left, Right }
        private AssetsLinkMapGraph m_graphView;
        private AssetsLinkMapSettings m_settings;
        private EditorWindow m_window;
        public AssetsLinkMapNode m_targetNode { get { return m_graphView.m_targetNode; } }

        public AssetsLinkMapGraphDrawer(AssetsLinkMapGraph graph, AssetsLinkMapSettings _settings, EditorWindow _window) {
            m_graphView = graph;
            m_settings = _settings;
            m_window = _window;
        }

        private Vector2 m_gridLayoutCellSize = new Vector2(200f, 150f);

        public void Draw() {
            if (m_targetNode != null) {
                DrawHierarchyNodesFromRefTargetNode();
            }
        }

        private void DrawHierarchyNodesFromRefTargetNode() {
            int layoutDepth = 1;
            float yAxisWeightRight = 0;
            float yAxisWeightLeft = 0;
            List<AssetsLinkMapNode> nodesToReLayoutRight = new List<AssetsLinkMapNode>();
            List<AssetsLinkMapNode> nodesToReLayoutLeft = new List<AssetsLinkMapNode>();
            CreateMainNodeGroup(m_targetNode, out Group groupNode);
            DrawInputsNodesRecursively(m_targetNode, NodeInputSide.Right, groupNode, layoutDepth, ref yAxisWeightRight, nodesToReLayoutRight);
            DrawInputsNodesRecursively(m_targetNode, NodeInputSide.Left, groupNode, -1, ref yAxisWeightLeft, nodesToReLayoutLeft);
            ReLayoutNodes(nodesToReLayoutRight, yAxisWeightRight, m_gridLayoutCellSize.y);
            ReLayoutNodes(nodesToReLayoutLeft, yAxisWeightLeft, m_gridLayoutCellSize.y);
            groupNode.Focus();
        }

        private void DrawInputsNodesRecursively(AssetsLinkMapNode fromNode, NodeInputSide inputSide, Group groupNode, int layoutDepth, ref float yAxisWeight, List<AssetsLinkMapNode> nodesToReLayout) {
            var nodes = inputSide == NodeInputSide.Left ? fromNode.LeftInputs : fromNode.RightInputs;
            var len = nodes.Count;
            for (int i = 0; i < len; i++) {
                AssetsLinkMapNode node = nodes[i];
                var nextList = inputSide == NodeInputSide.Left ? nodes[i].LeftInputs : nodes[i].RightInputs;
                DrawInputsNodesRecursively(node, inputSide, groupNode, layoutDepth + 1, ref yAxisWeight, nodesToReLayout);
                if (nextList.Count == 0) {
                    node.m_yAxisWeight = yAxisWeight;
                    yAxisWeight++;
                }
                if (nextList.Count != 0) {
                    node.m_yAxisWeight = (nextList[0].m_yAxisWeight + nextList[nextList.Count - 1].m_yAxisWeight) / 2f;
                }

                if (m_settings.DistinctDependenceMonoScript && inputSide == NodeInputSide.Right && node.m_distinctedCount > 0) {
                    Port temp = node.inputContainer[0] as Port;
                    temp.portName += " *";
                    temp.Add(new Label {
                        text = node.m_distinctedCount.ToString(),
                        style = {
                        color = new Color(0,1,1,1)
                    }
                    });
                }

                if (m_settings.DistinctReferenceObjects && inputSide == NodeInputSide.Left && node.m_distinctedCount > 0) {
                    Port temp = node.outputContainer[0] as Port;
                    temp.portName = "* " + temp.portName;
                    temp.Add(new Label {
                        text = node.m_distinctedCount.ToString(),
                        style = {
                        color = new Color(0,1,1,1)
                    }
                    });
                }

                m_graphView.AddElement(node);
                Edge edge = new Edge {
                    input = inputSide == NodeInputSide.Left ? fromNode.inputContainer[0] as Port : node.inputContainer[0] as Port,
                    output = inputSide == NodeInputSide.Left ? node.outputContainer[0] as Port : fromNode.outputContainer[0] as Port,
                };
                edge.input?.Connect(edge);
                edge.output?.Connect(edge);

                node.RefreshPorts();
                m_graphView.AddElement(edge);

                groupNode.AddElement(node);
                nodesToReLayout.Add(node);

                edge.capabilities &= ~Capabilities.Deletable;
            }
        }

        public AssetsLinkMapNode CreateMainNodeGroup(AssetsLinkMapNode mainNode, out Group groupNode) {
            groupNode = new Group { title = mainNode.Name };
            mainNode.mainContainer.style.borderBottomColor =
            mainNode.mainContainer.style.borderTopColor =
            mainNode.mainContainer.style.borderLeftColor =
            mainNode.mainContainer.style.borderRightColor =
            new Color(0, 0.5f, 0, 1);

            mainNode.userData = 0;
            mainNode.SetCustomPosition(new Rect((m_window.position.width - m_gridLayoutCellSize.x) * 0.5f , (m_window.position.height - m_gridLayoutCellSize.y) * 0.5f, 0, 0));
            m_graphView.AddElement(groupNode);
            m_graphView.AddElement(mainNode);

            groupNode.AddElement(mainNode);

            return mainNode;
        }

        private void ReLayoutNodes(List<AssetsLinkMapNode> nodesToReLayout, float lineCount, float lineHeight) {
            float halfY = lineCount * lineHeight * 0.5f;
            foreach (var node in nodesToReLayout) {
                var pos = new Vector2(m_gridLayoutCellSize.x * 1.5f * node.m_depth, (node.m_yAxisWeight * lineHeight) - halfY) + m_targetNode.m_position.position;
                var rect = new Rect(pos, Vector2.zero);
                node.SetCustomPosition(rect);
            }
        }
    }
}