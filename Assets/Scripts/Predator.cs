using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Predator : MonoBehaviour
{
    public int range;

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 target;

    private float maxSpeed = 50;


    void Start()
    {
        target = findNewTarget();
        this.velocity = target - this.transform.position;
        velocity.Normalize();
        this.velocity *= maxSpeed;
    }

    void Update()
    {
        teleportIfOutOfBounds();
        velocity += acceleration;
        if (Vector3.Magnitude(this.velocity) > maxSpeed / 3)
        {
            this.velocity.Normalize();
            this.velocity *= maxSpeed / 3;
        }
        this.transform.position = this.transform.position + velocity;
    }

    private Vector3 findNewTarget()
    {
        return new Vector3(Random.Range(1000, 10000), Random.Range(1000, 10000), Random.Range(1000, 10000));
    }

    private bool isPositionBeyond(float x)
    {
        return x > 10000 || x < 0;
    }
    private void teleportIfOutOfBounds()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = transform.position;

        if (isPositionBeyond(position.x))
        {
            if (position.x > 10000)
            {
                newPosition.x = 1;
            }
            if (position.x < 0)
            {
                newPosition.x = 9999;
            }
        }
        if (isPositionBeyond(position.y))
        {
            if (position.y > 10000)
            {
                newPosition.y = 1;
            }
            if (position.y < 0)
            {
                newPosition.y = 9999;
            }
        }
        if (isPositionBeyond(position.z))
        {
            if (position.z > 10000)
            {
                newPosition.z = 1;
            }
            if (position.z < 0)
            {
                newPosition.z = 9999;
            }
        }
        if (!position.Equals(newPosition))
        {
            this.transform.position = newPosition;
        }
    }

    public Vector3 huntBoid(List<Boid> boids)
    {

        Vector3 steering = new Vector3();
        Boid huntedBoid = null;
        float distance = 1000000;
        foreach (Boid boid in boids)
        {
            float currentDist = Vector3.Distance(this.transform.position, boid.transform.position);
            if (currentDist < distance)
            {
                huntedBoid = boid;
                distance = currentDist;
            }
        }
        steering += huntedBoid.transform.position - this.transform.position;
        steering = applySpeedAndForce(steering, 0.5f, new Vector3(1.0f, 1.0f, 1.0f));

        return steering;
    }


    private Vector3 applySpeedAndForce(Vector3 steering, float maxForce, Vector3 c)
    {
        steering.Normalize();
        steering *= maxSpeed / 2;

        steering -= (this.velocity);

        steering.Normalize();
        steering *= maxForce;

        if (this.name.Equals("Boid 0"))
        {
            Color color = new Color(c.x, c.y, c.z);
            Debug.DrawLine(this.transform.position, this.transform.position + steering * 1000, color);
        }

        return steering;
    }
    public void hunt(List<Boid> boids)
    {
        acceleration *= 0;
        acceleration += huntBoid(boids);
    }
}
