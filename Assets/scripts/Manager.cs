using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private Vector2[] checkpoints;  
    public  GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void restartLevel(){
        Vector2 respawnPoint = new Vector2(0f,0f);
        for (int i=0;i<checkpoints.Length;i++){
            if (player.gameObject.transform.position.x>=checkpoints[i].x){
                respawnPoint = checkpoints[i];
            }
        }
        player.transform.position = new Vector3(respawnPoint.x,respawnPoint.y,0);
        player.gameObject.GetComponent<PlayerController>().revive();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        
    }
}
