using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPU_FlockCompute : MonoBehaviour
{

    public ComputeShader cshader;

    public GameObject boidPrefab;
    public int BoidsCount;
    public float SpawnRadius;
    public GameObject[] boidsGo;
    public GPUBoid_Compute[] boidsData;
    public Transform Target;

    private int kernelHandle;


    public struct GPUBoid_Compute
    {
        public Vector3 position, direction;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.boidsGo = new GameObject[this.BoidsCount];
        this.boidsData = new GPUBoid_Compute[this.BoidsCount];
        this.kernelHandle = cshader.FindKernel("CSMain");

        for (int i = 0; i < this.BoidsCount; i++)
        {
            this.boidsData[i].position = transform.position + Random.insideUnitSphere * SpawnRadius;
            this.boidsGo[i] = Instantiate(boidPrefab, this.boidsData[i].position, Quaternion.Euler(this.boidsData[i].direction)) as GameObject;
            this.boidsData[i].direction = this.boidsGo[i].transform.forward;
        }
    }
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
