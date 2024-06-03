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
       // player.transform.position = new Vector3(currentCheckpoint.x,currentCheckpoint.y+1,0);
        GameObject levelStack = GameObject.FindWithTag("LevelStack");
        LevelStack levelStackComponent = levelStack.GetComponent<LevelStack>();
        // load the hub scene
        //print Score and HighScore
        int score = PlayerPrefs.GetInt("Score");
        int highScore = PlayerPrefs.GetInt("HighScore");
        print ("HighScore: " + highScore);
        PlayerPrefs.SetInt("Score", 0);
        SceneManager.LoadScene(levelStackComponent.LoadHub());

        
    }

    public string getNextLevel()
    {
        // find the object with the tag "LevelStack"
        GameObject levelStack = GameObject.FindWithTag("LevelStack");
        // get the LevelStack component from the object
        LevelStack levelStackComponent = levelStack.GetComponent<LevelStack>();
        // call the LoadNextLevel method from the LevelStack component
        return levelStackComponent.LoadNextLevel();

    }
}
