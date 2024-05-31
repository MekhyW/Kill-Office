using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelStack : MonoBehaviour
{
    public static LevelStack instance;
    public int levels_before_boss = 5;
    public Stack<string> levelstack = new Stack<string>();
    private List<string> levels = new List<string> { 
        "snake_barrels", "snake_enemies", "snake_enemies2", "snake_spikes", "the_pit", "the_pit_barrels", "the_pit_enemies", "the_pit_hard", "the_pit_platform", "the_pit_walls", "easy_level_1", "easy_level_2", "easy_level_3", "easy_level_4", "easy_level_5" 
    };
    private string bosslevel = "boss_scene";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        PrepareLevels();
        DontDestroyOnLoad(gameObject);
    }

    public void PrepareLevels()
    {
        levelstack.Clear();
        foreach (string l in levels)
        {
            levelstack.Push(l);
        }
        for (int i = 0; i < levelstack.Count; i++)
        {
            int r = Random.Range(0, levelstack.Count);
            string temp = levelstack.Pop();
            levelstack.Push(levelstack.ToArray()[r]);
            levelstack.Pop();
            levelstack.Push(temp);
        }
        print("Levels Prepared");
    }

    public string LoadNextLevel()
    {
        if (levelstack.Count == 0)
        {
            PrepareLevels();
            print("Boss Level");
            SceneManager.LoadScene(bosslevel);
            return bosslevel;
        }
        print("Level " + levelstack.Peek());
        SceneManager.LoadScene(levelstack.Peek());
        print("Level " + levelstack.Peek() + " Loaded");
        return levelstack.Pop();
    }
}
