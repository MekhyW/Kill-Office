using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelStack : MonoBehaviour
{
    public static LevelStack instance;
    public int levels_before_boss = 5;
    public Stack<string> levelstack = new Stack<string>();
    private List<string> levels = new List<string> { 
        "Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "Level7", "Level8", "Level9", "Level10" 
    };
    private string bosslevel = "BossLevel";

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
        return levelstack.Pop();
    }
}
