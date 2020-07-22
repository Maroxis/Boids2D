using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehaviour : MonoBehaviour
{
    public float speed;
    private Vector3 velocity;
    public float sensRadius;
    readonly private float velMin = 0.4f;

    //map soft boundries
    private float xMax;
    private float xMin;
    private float yMax;
    private float yMin;
    private float borderForce = 0.01f;

    //split fixedupdate workload to multiple frames
    private int turn = 0;
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sensRadius);
    }
    Vector3 Separation()
    {
        int layerMask = 1 << 8;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, layerMask);
        Vector3 vel = new Vector3(0,0,0);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Collider hitCollider = hitColliders[i];
            if (hitCollider.gameObject.transform != transform)
            {
                if (hitCollider != null && hitCollider.transform.position != transform.position)
                {
                    vel = (vel - (hitCollider.transform.position - transform.position));
                }
            }
        }
        vel.Normalize();
        return vel;
    }
    Vector3 Aligment(List<BoidBehaviour> collidedBoids)
    {
        Vector3 vel = new Vector3(0, 0, 0);

        int amm = collidedBoids.Count;

        for (int i = 0; i < amm; i++)
        {
            vel += collidedBoids[i].velocity;
        }

        if (amm > 0)
        {
            vel = vel / amm;
        }

        return (vel- velocity)/8;
    }
    Vector3 Cohesion(List<BoidBehaviour> collidedBoids)
    {
        Vector3 vel = new Vector3(0, 0, 0);
        int amm = collidedBoids.Count;

        for (int i = 0; i < amm; i++)
        {
            vel += collidedBoids[i].transform.position;
        }
       
        if (amm > 0)
        {
            vel = vel / amm;
        }

        return (vel-transform.position)/500;
    }
    Vector3 CheckBorder()
    {
        Vector3 vel = new Vector3(0, 0, 0);
        float slack;
        if (borderForce != 0)
            slack = 1 / borderForce;
        else return vel;

        if (transform.position[0] < xMin + slack)
            vel[0] = borderForce;
        else if(transform.position[0] > xMax - slack)
            vel[0] = borderForce * -1;
        
        if (transform.position[1] < yMin + slack)
            vel[1] = borderForce;
        else if (transform.position[1] > yMax - slack)
            vel[1] = borderForce * -1;

        return vel;
    }

    // Start is called before the first frame update
    void Start()
    {
        yMax = Camera.main.orthographicSize;
        yMin = yMax * -1;
        xMax = yMax * Camera.main.aspect;
        xMin = xMax * -1;

        velocity = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
        velocity.Normalize();
    }
    List<BoidBehaviour> GetColidedBoids()
    {
        List<BoidBehaviour> boidBehaviours = new List<BoidBehaviour>();

        int layerMask = 1 << 8;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sensRadius, layerMask);
        
        
        for (int i =0; i<hitColliders.Length; i++)
        {
            Collider hitCollider = hitColliders[i];
            BoidBehaviour boidBeh = hitCollider.gameObject.GetComponentInParent<BoidBehaviour>();
            if (boidBeh.transform != transform)
            {
                boidBehaviours.Add(boidBeh);
            }
        }
        return boidBehaviours;
    }

    public void Calculate()
    {
        if (turn == 0)
        {
            List<BoidBehaviour> collidedBoids = GetColidedBoids();
            Vector3 ali = Aligment(collidedBoids);
            Vector3 coh = Cohesion(collidedBoids);

            velocity += ali;
            velocity += coh;

            turn = 1;
        }
        else if (turn == 1)
        {
            Vector3 sep = Separation();
            Vector3 bor = CheckBorder();

            velocity += sep;
            velocity += bor;

            turn = 0;
        }


        float mag = velocity.magnitude;
        if (mag > 1f)
            velocity.Normalize();
        else if (mag == 0f)
        {
            velocity[0] = velMin / Mathf.Sqrt(2f) + 0.001f;
            velocity[1] = velMin / Mathf.Sqrt(2f) + 0.001f;
        }
        else if (mag < velMin)
            velocity = velocity / mag * velMin;

        velocity[2] = 0f;
        transform.Translate(velocity * speed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.LookRotation(velocity);
    }
    
}
