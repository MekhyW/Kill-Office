using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node

{
    public Node node;
    public NodeView(Node node) {
        this.node = node;
        this.title = node.name;
    }
}
