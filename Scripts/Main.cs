using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private List<GameObject> boids = new List<GameObject>();
    private List<BoidBehaviour> boidsBeh = new List<BoidBehaviour>();
    private GlobalSettings settings;
    //public ComputeShader compute;

    private int stTurn = 1;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("Settings").GetComponent<GlobalSettings>();
        settings.Init();
        boids = settings.boids;
        boidsBeh.Clear();
        for (int i = 0; i < boids.Count; i++)
        {
            BoidBehaviour beh = boids[i].GetComponent<BoidBehaviour>();
            boidsBeh.Add(beh);
        }

        //int kernel = compute.FindKernel("CSMain");
    }
    void CheckSettings()
    {
       bool update = settings.CheckForUpdate();
        if (update)
        {
            boids = settings.boids;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (stTurn % 10 == 0)
        {
            CheckSettings();
            stTurn = 1;
        }
        else stTurn++;

        for(int i = 0; i < boids.Count; i++)
        {
            BoidBehaviour boidBeh = boids[i].GetComponent<BoidBehaviour>();
            boidBeh.Calculate();
        }
    }
}
