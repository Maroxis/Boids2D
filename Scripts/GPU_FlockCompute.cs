﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GPUBoid_Compute
{
    public Vector3 position, direction;
}
/*
public struct Grid
{
    public int[,][] cells;
}

public struct Test
{
    public int x;
}*/
public class GPU_FlockCompute : MonoBehaviour
{

    public int boidsAmmount = 100;
    private int subMeshIndex = 0;

    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;
    private ComputeBuffer buffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    public Mesh boidMesh;
    public Material boidMaterial;

    public float SpawnRadius;

    public GPUBoid_Compute[] boidsData;
    //public Test[] testData;

    public ComputeShader cshader;
     private int mainKernelHandle;
     private int startKernelHandle;
    
    
    public float RotationSpeed = 1f;
    public float BoidSpeed = 20f;
    public float BoidMinSpeed = 0.2f;
    public float NeighbourDistance = 10f;
    public float AvoidDistance = 4f;

    private float BorderX;
    private float BorderY;

    //public ComputeBuffer test;
    

    void Start()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        this.boidsData = new GPUBoid_Compute[this.boidsAmmount];
        //this.testData = new Test[this.boidsAmmount];

        this.mainKernelHandle = cshader.FindKernel("CSMain");
        this.startKernelHandle = cshader.FindKernel("CSStart");

        BorderY = Camera.main.orthographicSize;
        BorderX = Camera.main.aspect * BorderY;

        UpdateBuffers();
        //cshader.Dispatch(this.startKernelHandle, boidsAmmount / 32 + 1, 1, 1);
    }

    void UpdateBuffers()
    {
        // Ensure submesh index is in range
        if (boidMesh != null)
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, boidMesh.subMeshCount - 1);

        // Positions
        if (buffer != null)
            buffer.Release();
        buffer = new ComputeBuffer(boidsAmmount, sizeof(float) * 6);
        // test = new ComputeBuffer(boidsAmmount, sizeof(int));

        for (int i = 0; i < boidsAmmount; i++)
        {
            //this.testData[i].x = 0;
            this.boidsData[i].position = transform.position + Random.insideUnitSphere * SpawnRadius;
            this.boidsData[i].position[2] = 0;
            this.boidsData[i].direction = this.boidsData[i].position / SpawnRadius;
            this.boidsData[i].direction[2] = 0;
            //this.boidsGo[i].GetComponent<Renderer>().material.SetFloat("_Noise", Mathf.Abs(Mathf.Sin(this.boidsData[i].position.x * this.boidsData[i].position.y)) * 10);
        }
        buffer.SetData(boidsData);
        //test.SetData(testData);
        boidMaterial.SetBuffer("boidBuffer", buffer);
        if (boidMesh != null)
        {
            args[0] = (uint)boidMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)boidsAmmount;
            args[2] = (uint)boidMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)boidMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }
        argsBuffer.SetData(args);

        cachedInstanceCount = boidsAmmount;
        cachedSubMeshIndex = subMeshIndex;

        //cshader.SetBuffer(this.startKernelHandle, "boidBuffer", buffer);
        //cshader.SetBuffer(this.startKernelHandle, "testBuffer", test);
        cshader.SetBuffer(this.mainKernelHandle, "boidBuffer", buffer);
        //cshader.SetBuffer(this.mainKernelHandle, "testBuffer", test);
        cshader.SetFloat("DeltaTime", Time.deltaTime);
        cshader.SetFloat("RotationSpeed", RotationSpeed);
        cshader.SetFloat("BoidSpeed", BoidSpeed);
        cshader.SetFloat("BoidMinSpeed", BoidMinSpeed);
        cshader.SetFloat("NeighbourDistance", NeighbourDistance);
        cshader.SetFloat("AvoidDistance", AvoidDistance);
        cshader.SetFloat("BorderX", BorderX);
        cshader.SetFloat("BorderY", BorderY);
        //cshader.SetFloat("CellSizeX", CellSizeX);
        //cshader.SetFloat("CellSizeY", CellSizeY);
        cshader.SetInt("BoidsCount", boidsAmmount);
        
    }

    private void FixedUpdate()
    {
        cshader.Dispatch(this.mainKernelHandle, boidsAmmount / 32 + 1, 1, 1);

        if (cachedInstanceCount != boidsAmmount || cachedSubMeshIndex != subMeshIndex)
            UpdateBuffers();
    }
    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(boidMesh, 0, boidMaterial, new Bounds(Vector3.zero, new Vector3(1000.0f, 1000.0f, 1000.0f)), argsBuffer);
    }
    void OnDestroy()
    {
        if (buffer != null)
            buffer.Release();
        buffer = null;

        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }
}

