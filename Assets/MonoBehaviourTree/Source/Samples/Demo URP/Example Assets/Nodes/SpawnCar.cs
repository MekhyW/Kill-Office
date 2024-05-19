using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [MBTNode("Example/Spawn Object")]
    [AddComponentMenu("")]
    public class SpawnCar : Leaf
    {
        public GameObjectReference prefabToSpawn;
        // private Vector3Reference spawnPosition;
        public float speed = 1.0f;


        public override NodeResult Execute()
        {
            if (prefabToSpawn.Value != null)
            {
                // Instantiate the prefab at the specified position and default rotation
                Transform prefabTransform = prefabToSpawn.Value.transform;
                GameObject instantiatedObject = Instantiate(prefabToSpawn.Value, prefabTransform.position, prefabTransform.rotation);
                ConstantMovement movement = instantiatedObject.AddComponent<ConstantMovement>();
                movement.SetMovement(Vector3.right, speed); 
                return NodeResult.success;
            }
            else
            {
                Debug.LogWarning("Prefab to spawn is not set.");
                return NodeResult.failure;
            }
        }
    }
}
