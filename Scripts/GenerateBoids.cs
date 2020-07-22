using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateBoids : MonoBehaviour
{
    public GameObject boidPref;
    private List<GameObject> boids = new List<GameObject>();

    private void Start()
    {
        
    }

    public List<GameObject> Generate(int amm,int pAmm,float speed,float sRad)
    {
        //Object prefab = LoadAsset("Assets / Prefabs / Boid.prefab");
        //Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Boid.prefab", typeof(GameObject));

        for (int i = pAmm; i < amm; i++)
        {
            // GameObject boid = Instantiate(Resources.Load("Assets/Prefabs/Boid.prefab")) as GameObject;
            //GameObject boid = Instantiate(prefab) as GameObject;
            GameObject boid = Instantiate(boidPref);
            GameObject group = GameObject.Find("Boids");
            boid.transform.parent = group.transform;

            BoidBehaviour boidBeh = boid.GetComponent<BoidBehaviour>();
            boidBeh.speed = speed;
            boidBeh.sensRadius = sRad;

            boids.Add(boid);
        }
        return boids;
    }
    public List<GameObject> Remove(int amm)
    {
        for(int i = 0; i< amm; i++)
        {
            GameObject boid = boids[boids.Count - 1];
            Destroy(boid);
            boids.RemoveAt(boids.Count - 1);
        }
        return boids;
    }
}
