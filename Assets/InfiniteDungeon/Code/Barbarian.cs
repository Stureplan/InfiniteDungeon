using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian : MonoBehaviour
{
    public Map map;
    public BarbarianUI ui;

    public void NextTurn(int turn) { }
    public void DecideMove(int turn) { }
    public void ExecuteMove(int turn) { }
    public float VisualTurn(int turn)
    {
        // Handle animations, effects etc.

        return 1.0f;
    }



    private void Start()
    {
        ui.FadePanelsIn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ui.FadePanelsOut();
        }
    }
}
