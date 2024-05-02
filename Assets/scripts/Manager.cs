using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private Vector2[] checkpoints;  
    private Vector2 currentCheckpoint;
    static private int checkpointIndex;
    [SerializeField] private string next_level;
    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        checkpointIndex = 0;
        currentCheckpoint = checkpoints[checkpointIndex];
        
    }
    

    // Update is called once per frame
    void Update()
    {
        for (int i=checkpointIndex;i<checkpoints.Length;i++){
            if (player.gameObject.transform.position.x>=checkpoints[i].x){
                currentCheckpoint = checkpoints[i];
                checkpointIndex++;
            }
        }
    }

    public void restartLevel(){

        
        player.gameObject.GetComponent<PlayerController>().revive();
        player.transform.position = new Vector3(currentCheckpoint.x,currentCheckpoint.y,0);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        
    }

    public string getNextLevel()
    {
        return next_level;
    }
}
