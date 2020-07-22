using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [Range(0, 500)]
    public int boidAmmount;
    private int prevAmmount;
    [Range(10f, 200f)]
    public float speed;
    private float prevSpeed;
    [Range(1f, 50f)]
    public float sensRadius;
    private float prevRad;

    private GameObject generator;
    private GenerateBoids genBoids;
    public List<GameObject> boids;

    public void Init()
    {
        boidAmmount = 100;
        prevAmmount = 0;
        prevSpeed = speed = 50f;
        prevRad = sensRadius = 10f;

        generator = GameObject.Find("Generator");
        genBoids = generator.GetComponent<GenerateBoids>();
        boids = genBoids.Generate(boidAmmount,prevAmmount,speed,sensRadius);

        prevAmmount = boidAmmount;
    }


    public bool CheckForUpdate()
    {
        bool change = false;
        if (boidAmmount > prevAmmount)
        {
            boids = genBoids.Generate(boidAmmount, prevAmmount, speed, sensRadius);
            prevAmmount = boidAmmount;
            change = true;
        }
        else if (boidAmmount < prevAmmount)
        {
            boids = genBoids.Remove(prevAmmount - boidAmmount);
            prevAmmount = boidAmmount;
            change = true;
        }

        if(speed != prevSpeed && boidAmmount > 0)
        {
            for (int i = 0; i < boidAmmount; i++)
            {
                BoidBehaviour boiBeh = boids[i].GetComponent<BoidBehaviour>();
                boiBeh.speed = speed;
            }
            prevSpeed = speed;
            change = true;
        }
        if (sensRadius != prevRad && boidAmmount > 0)
        {
            for (int i = 0; i < boidAmmount; i++)
            {
                BoidBehaviour boiBeh = boids[i].GetComponent<BoidBehaviour>();
                boiBeh.sensRadius = sensRadius;
            }
            prevRad = sensRadius;
            change = true;
        }
        return change;
    }
}
