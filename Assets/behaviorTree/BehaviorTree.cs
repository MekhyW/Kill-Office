using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviorTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    public Node.State Update(){
        if (rootNode.state == Node.State.Running){
            treeState = rootNode.Update();
        }
        return treeState;
    }
}
