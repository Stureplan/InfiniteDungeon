using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
    public const float ENEMY_TIMER = 0.2f;
    public const float BARBARIAN_TIMER = 0.2f;

    static bool TURN_READY = true;

    Map map;

    static List<Agent> enemies = new List<Agent>();
    Barbarian barbarian;

    int turn = 0;

    void Start ()
    {
        // Initialize all static classes here.
        FX.Initialize();
        map = Map.FindMap();
        //map.Generate(false);


        barbarian = FindObjectOfType<Barbarian>();
        enemies = map.Enemies();
	}
	
	void Update ()
    {

	}

    public void NextTurn(int dir)
    {
        if (TURN_READY)
        {
            // Handle Player
            float timer = barbarian.NextTurn(dir, turn);

            // Something went wrong, invalid move etc..
            if (timer > 665.0f) { Debug.LogWarning("Timer was " + timer); return; }

            TURN_READY = false;
            StartCoroutine(BarbarianWaiter(timer));
        }
    }

    void ExecuteEnemyTurn()
    {
        // Handle Enemy turns.
        int e = enemies.Count;
        for (int i = 0; i < e; i++)
        {
            enemies[i].NextTurn(turn);
        }

    }

    IEnumerator BarbarianWaiter(float timer)
    {
        float t = 0.0f;

        while (t < timer)
        {
            t += Time.deltaTime;

            yield return null;
        }

        //Enemy turn
        ExecuteEnemyTurn();
        barbarian.CheckSurroundings();

        //Wait for enemies
        StartCoroutine(EnemyWaiter(ENEMY_TIMER));
    }

    IEnumerator EnemyWaiter(float timer)
    {
        float t = 0.0f;

        while (t < timer)
        {
            t += Time.deltaTime;

            yield return null;
        }


        // Turn count up.
        turn++;

        // Turn is ready for Player input again.
        TURN_READY = true;
    }




    public static void RemoveEnemy(Agent a)
    {
        enemies.Remove(a);
    }

    public static void Over()
    {
        Debug.Log("Game Over!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
