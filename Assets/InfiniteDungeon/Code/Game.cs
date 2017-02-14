using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
    static bool TURN_READY = true;

    public Map map;

    List<Agent> enemies = new List<Agent>();
    Barbarian barbarian;

    int turn = 0;

    void Start ()
    {
        barbarian = FindObjectOfType<Barbarian>();
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
            if (timer > 665.0f) { return; }

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
            enemies[i].NextTurn(map, turn);
            enemies[i].VisualTurn(map, turn);
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






    public static void Over()
    {
        Debug.Log("Game Over!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
