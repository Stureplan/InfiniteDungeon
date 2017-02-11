using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour {
    public Gradient gr;

    // Use this for initialization
    void Start () {
        Shader.SetGlobalVector("_GLOBAL_LIGHT_0", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
	}
	
	// Update is called once per frame
	void Update () {

	}
}
