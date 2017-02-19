using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
    static bool TURN_READY = true;

    public Map map;

    static List<Agent> enemies = new List<Agent>();
    Barbarian barbarian;

    int turn = 0;

    void Start ()
    {
        // Initialize all static classes here.
        FX.Initialize();


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
            StartCoroutine(Waiter(timer));
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

        // Turn count up.
        turn++;

        // Turn is ready for Player input again.
        TURN_READY = true;
    }

    IEnumerator Waiter(float timer)
    {
        float t = 0.0f;

        while (t < timer)
        {
            t += Time.deltaTime;

            yield return null;
        }

        //execute
        ExecuteEnemyTurn();
        barbarian.CheckSurroundings();
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
