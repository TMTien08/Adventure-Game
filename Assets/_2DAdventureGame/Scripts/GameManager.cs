using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    EnemyController[] enemies;
    public UIHandler uiHandler;
    private int enemiesFixed = 0;
    void Start()
    {
        enemies = FindObjectsByType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.OnFixed += HandleEnemyFixed;
        }
        uiHandler.SetCouter(0, enemies.Length);
        player.OnTalkedToNPC += HandlePlayerTalkedToNPC;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.health <= 0)
        {
            uiHandler.DisplayLoseScreen();
            Invoke(nameof(ReloadScene), 3.0f);
        }
        
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }
    bool AllEnemiesFixed()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isBrooken) return false;
        }
        return true;
    }

    void HandleEnemyFixed()
    {
        enemiesFixed++;
        uiHandler.SetCouter(enemiesFixed, enemies.Length);
    }
    void HandlePlayerTalkedToNPC()
    {
        if (AllEnemiesFixed())
        {
            uiHandler.DisplayWinScreen();
            Invoke(nameof(LoadStartScene), 3f);
        }
        else
        {
            uiHandler.DisplayDialogue();
        }
    }
}
