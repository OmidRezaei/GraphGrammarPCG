﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace GenGra {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    [System.Xml.Serialization.XmlRootAttribute("GenGra", Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG", IsNullable=false)]
    public partial class GenGraType {
        
        private GraphsType graphsField;
        
        private GrammarType grammarField;
        
        /// <remarks/>
        public GraphsType Graphs {
            get {
                return this.graphsField;
            }
            set {
                this.graphsField = value;
            }
        }
        
        /// <remarks/>
        public GrammarType Grammar {
            get {
                return this.grammarField;
            }
            set {
                this.grammarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class GraphsType {
        
        private GraphType[] graphField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Graph")]
        public GraphType[] Graph {
            get {
                return this.graphField;
            }
            set {
                this.graphField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    [System.Xml.Serialization.XmlRootAttribute("Graph", Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG", IsNullable=false)]
    public partial class GraphType {
        
        private NodesType nodesField;
        
        private EdgesType edgesField;
        
        private string idField;
        
        /// <remarks/>
        public NodesType Nodes {
            get {
                return this.nodesField;
            }
            set {
                this.nodesField = value;
            }
        }
        
        /// <remarks/>
        public EdgesType Edges {
            get {
                return this.edgesField;
            }
            set {
                this.edgesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class NodesType {
        
        private NodeType[] nodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Node")]
        public NodeType[] Node {
            get {
                return this.nodeField;
            }
            set {
                this.nodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class NodeType {
        
        private string idField;
        
        private string symbolField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string symbol {
            get {
                return this.symbolField;
            }
            set {
                this.symbolField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class RuleType {
        
        private string idField;
        
        private string sourceField;
        
        private string targetField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string source {
            get {
                return this.sourceField;
            }
            set {
                this.sourceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string target {
            get {
                return this.targetField;
            }
            set {
                this.targetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class RulesType {
        
        private RuleType[] ruleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Rule")]
        public RuleType[] Rule {
            get {
                return this.ruleField;
            }
            set {
                this.ruleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class StartGraphType {
        
        private string refField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @ref {
            get {
                return this.refField;
            }
            set {
                this.refField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class GrammarType {
        
        private StartGraphType startGraphField;
        
        private RulesType rulesField;
        
        /// <remarks/>
        public StartGraphType StartGraph {
            get {
                return this.startGraphField;
            }
            set {
                this.startGraphField = value;
            }
        }
        
        /// <remarks/>
        public RulesType Rules {
            get {
                return this.rulesField;
            }
            set {
                this.rulesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class EdgeType {
        
        private string idField;
        
        private string sourceField;
        
        private string targetField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string source {
            get {
                return this.sourceField;
            }
            set {
                this.sourceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string target {
            get {
                return this.targetField;
            }
            set {
                this.targetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/DylanRodgers98/GraphGrammarPCG")]
    public partial class EdgesType {
        
        private EdgeType[] edgeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Edge")]
        public EdgeType[] Edge {
            get {
                return this.edgeField;
            }
            set {
                this.edgeField = value;
            }
        }
    }
}
