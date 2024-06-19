using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsLinkMap
{
    internal class AssetsLinkMapResolver
    {
        private AssetsLinkMapGraph m_graph;
        private AssetsLinkMapSettings m_settings;
        private AssetsLinkMapResolver_References m_referencesResolver;
        private AssetsLinkMapResolver_Dependencies m_dependenciesResolver;

        public AssetsLinkMapResolver(AssetsLinkMapGraph graph, AssetsLinkMapSettings settings)
        {
            m_graph = graph;
            m_settings = settings;
            m_referencesResolver = new AssetsLinkMapResolver_References(graph, settings);
            m_dependenciesResolver = new AssetsLinkMapResolver_Dependencies(settings);
        }

        public void BuildGraph()
        {
            if (m_settings.FindDependencies)
            {
                m_dependenciesResolver.FindDependencies(m_graph.m_targetNode, m_settings.DependenciesDepth);
            }
            if (m_settings.FindReferences)
            {
                m_referencesResolver.FindReferences();
            }
        }
    }
}