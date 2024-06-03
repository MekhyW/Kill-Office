using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Unity.VisualScripting;

namespace MBTExample
{
    [MBTNode("Example/SpawnLaser")]
    [AddComponentMenu("")]
    public class SpawnLaser : Leaf
    {
        public FloatReference coord_x;
        public GameObjectReference prefabToSpawn;
        public GameObjectReference player;

        private float speed = 10.0f;
        private Vector3 newPosition;

        public override NodeResult Execute()
        {
            if (prefabToSpawn.Value != null)
            {
                Transform prefabTransform = prefabToSpawn.Value.transform;
                Transform playerTransform = player.Value.transform;
                // Corrigir a criação do Vector3 com valores float apropriados
                newPosition = new Vector3(playerTransform.position.x, 6, prefabTransform.position.z);
                
                // Aplicar a nova posição (se necessário)
                prefabTransform.position = newPosition;


                GameObject instantiatedObject = Instantiate(prefabToSpawn.Value, prefabTransform.position, prefabTransform.rotation);
                ConstantMovement movement = instantiatedObject.AddComponent<ConstantMovement>();
                movement.SetMovement(Vector3.right, speed); 
                return NodeResult.success;
            }else{
                return NodeResult.failure;
            }

        }
    }
}
