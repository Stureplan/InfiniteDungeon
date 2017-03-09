using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemTest : MonoBehaviour 
{
    private void Update()
    {
        long memory = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
        Debug.Log(memory);
    }
}
