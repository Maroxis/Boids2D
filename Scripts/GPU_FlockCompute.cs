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

    private int kernelHandle;
    private ComputeBuffer buffer;

    private float BorderX;
    private float BorderY;
    
    public struct GPUBoid_Compute
    {
        public Vector3 position, direction;
    }

    void Start()
    {
        this.kernelHandle = cshader.FindKernel("CSMain");

        BorderY = Camera.main.orthographicSize;
        BorderX = Camera.main.aspect * BorderY;

        this.boidsGo = new GameObject[this.BoidsCount];
        this.boidsData = new GPUBoid_Compute[this.BoidsCount];
        GameObject boidGroup = GameObject.Find("Boids");
        
        for (int i = 0; i < this.BoidsCount; i++)
        {
            this.boidsData[i].position = transform.position + Random.insideUnitSphere * SpawnRadius;
            this.boidsData[i].position[2] = 0;
            this.boidsData[i].direction = this.boidsData[i].position / SpawnRadius;
            this.boidsGo[i] = Instantiate(boidPrefab, this.boidsData[i].position, Quaternion.Euler(this.boidsData[i].direction)) as GameObject;
            this.boidsGo[i].transform.parent = boidGroup.transform;
            this.boidsGo[i].transform.localScale = new Vector3(50,200,50);
            this.boidsData[i].direction = this.boidsGo[i].transform.forward;
            this.boidsData[i].direction[2] = 0;

        }
    }

    public float RotationSpeed = 1f;
    public float BoidSpeed = 20f;
    public float BoidMinSpeed = 0.2f;
    public float NeighbourDistance = 10f;
    public float AvoidDistance = 4f;

    void FixedUpdate()
    {
        buffer = new ComputeBuffer(BoidsCount, 24);
        buffer.SetData(this.boidsData);

        cshader.SetBuffer(this.kernelHandle, "boidBuffer", buffer);
        cshader.SetFloat("DeltaTime", Time.deltaTime);
        cshader.SetFloat("RotationSpeed", RotationSpeed);
        cshader.SetFloat("BoidSpeed", BoidSpeed);
        cshader.SetFloat("BoidMinSpeed", BoidMinSpeed);
        cshader.SetFloat("NeighbourDistance", NeighbourDistance);
        cshader.SetFloat("AvoidDistance", AvoidDistance);
        cshader.SetFloats("BorderX", BorderX);
        cshader.SetFloats("BorderY", BorderY);
        cshader.SetInt("BoidsCount", BoidsCount);

        cshader.Dispatch(this.kernelHandle, this.BoidsCount, 1, 1);
        buffer.GetData(this.boidsData);
        buffer.Release();
        
        for (int i = 0; i < this.boidsData.Length; i++)
        {
            this.boidsGo[i].transform.localPosition = this.boidsData[i].position;

            if (!this.boidsData[i].direction.Equals(Vector3.zero))
            {
                this.boidsGo[i].transform.rotation = Quaternion.LookRotation(this.boidsData[i].direction);
            }

        }
    }
    void OnDestroy()
    {
        if (buffer != null) buffer.Release();
    }
}

