using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelStack : MonoBehaviour
{
    public static LevelStack instance;
    public int n_levels = 0;
    private int score = 0;
    public int levels_before_boss = 5;
    public Stack<string> levelstack = new Stack<string>();
    private List<string> levels = new List<string> { 
        "snake_barrels", "snake_enemies", "snake_enemies2", "snake_spikes", "the_pit_barrels", 
        "the_pit_enemies", "the_pit_hard", "the_pit_platform", "the_pit_walls", "easy_level_1", 
        "easy_level_2", "easy_level_3", "easy_level_4", "easy_level_5", "business_level_1", "business_level_2"
    };
    private string bosslevel = "boss_scene";
    private string hub = "hub_scene";


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        PrepareLevels();
        score = 0;
        PlayerPrefs.SetInt("Score", score);
        DontDestroyOnLoad(gameObject);
    }

    public void PrepareLevels()
    {
        levelstack.Clear();
        n_levels = 0;
        foreach (string l in levels)
        {
            n_levels++;
            levelstack.Push(l);
        }
        // randomize the level order
        for (int i = 0; i < levelstack.Count; i++)
        {
            string temp = levelstack.Pop();
            int randomIndex = Random.Range(0, levelstack.Count);
            Stack<string> tempStack = new Stack<string>();
            for (int j = 0; j < randomIndex; j++)
            {
                tempStack.Push(levelstack.Pop());
            }
            levelstack.Push(temp);
            for (int j = 0; j < randomIndex; j++)
            {
                levelstack.Push(tempStack.Pop());
            }
        }
        print("Levels Prepared");
    }

    public string LoadNextLevel()
    {
        levels_before_boss--;
        n_levels--;
        score = PlayerPrefs.GetInt("Score");
        score++;
        PlayerPrefs.SetInt("Score", score);
        print("Score: " + score);
        if (levels_before_boss == 0)
        {
            
            print("Boss Level");
            levels_before_boss = 5;
            SceneManager.LoadScene(bosslevel);
            PrepareLevels();
            return bosslevel;
        }

        if (n_levels == 0)
        {
            PrepareLevels();
        }
        print("Level " + levelstack.Peek());
        SceneManager.LoadScene(levelstack.Peek());
        print("Level " + levelstack.Peek() + " Loaded");
        return levelstack.Pop();
    }

    public string LoadHub()
    {
        levels_before_boss = 5;
        PrepareLevels();
        return hub;
    }
}
