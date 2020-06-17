using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public int perception;
    public float maxSpeedSeparation;
    public float maxSpeedCohesion;
    public float maxSpeedAlign;
    private float maxSpeed = 40;
    public bool randomDirection;
    public Collider coll;
    void Start()
    {
        coll = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        // always draw a 5-unit colored line from the origin

    }
    // Update is called once per frame
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

    private void teleportIfOutOfBounds()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = transform.position;


        if (position.x > 10000)
        {
            newPosition.x = 1;
        }
        if (position.x < 0)
        {
            newPosition.x = 9999;
        }
        if (position.y > 5000)
        {
            newPosition.y = 1;
        }
        if (position.y < 0)
        {
            newPosition.y = 4999;
        }

        if (position.z > 10000)
        {
            newPosition.z = 1;
        }
        if (position.z < 0)
        {
            newPosition.z = 9999;
        }
        if (!position.Equals(newPosition))
        {
            this.transform.position = newPosition;
        }
    }

    public Vector3 alignBoid(List<Movement> boidsInRange)
    {
        Vector3 steering = new Vector3();

        foreach (Movement other in boidsInRange)
        {
            steering += other.velocity;
        }
        steering /= boidsInRange.Count;
        steering = applySpeedAndForce(steering, maxSpeedAlign, new Vector3(1.0f, 1.0f, 1.0f));

        return steering;
    }

    public Vector3 separationBoid(List<Movement> boidsInRange)
    {
        Vector3 steering = new Vector3();

        foreach (Movement other in boidsInRange)
        {
            Vector3 diff = this.transform.position - other.transform.position;
            diff /= Vector3.Distance(this.transform.position, other.transform.position);
            steering += diff;
        }

        steering /= boidsInRange.Count;
        steering = applySpeedAndForce(steering, maxSpeedSeparation, new Vector3(1.0f, 1.0f, 1.0f));

        return steering;
    }

    public Vector3 cohesionBoid(List<Movement> boidsInRange)
    {
        Vector3 steering = new Vector3();

        foreach (Movement other in boidsInRange)
        {
            steering += other.transform.position;
        }

        steering /= boidsInRange.Count;
        steering -= this.transform.position;
        steering = applySpeedAndForce(steering, maxSpeedCohesion, new Vector3(1.0f, 1.0f, 1.0f));

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

    private List<Movement> getBoidsInRange(List<Movement> boids)
    {
        return (from boid in boids
                where
                Vector3.Distance(this.transform.position, boid.transform.position) < perception
                && (!boid.Equals(this))
                select boid).ToList();
    }
    private List<Obstacle> getObstaclesinRange(List<Obstacle> obstacles)
    {
        return (from obstacle in obstacles
                where
                obstacle.getDistance(this.transform.position) < obstacle.range
                select obstacle).ToList();
    }
    private Vector3 avoidingObstacles(List<Obstacle> obstacles)
    {
        Vector3 steering = new Vector3();

        foreach (Obstacle obstacle in obstacles)
        {
            Vector3 diff = obstacle.getDiff(this.transform.position);
            float dist = obstacle.range - obstacle.size - (obstacle.getDistance(this.transform.position) - obstacle.size);
            steering += diff;
        }

        steering /= obstacles.Count;
        steering = applySpeedAndForce(steering, 1.2f, new Vector3(0.0f, 1.0f, 1.0f));
        return steering;
    }

    private Vector3 avoidingPredator(List<Predator> predators)
    {
        float power = 3;
        Vector3 steering = new Vector3();

        foreach (Predator predator in predators)
        {
            Vector3 diff = this.transform.position - predator.transform.position;
            float dist = predator.range - (Vector3.Distance(this.transform.position, predator.transform.position));
            float prop = dist / predator.range;
            power *= prop;
            steering += diff;
        }

        steering /= predators.Count;
        steering = applySpeedAndForce(steering, power, new Vector3(0.0f, 1.0f, 0.3f));
        return steering;
    }
    public void flock(List<Movement> boids, List<Obstacle> obstacles, List<Predator> predators)
    {
        acceleration *= 0;
        List<Movement> boidsInRange = getBoidsInRange(boids);
        List<Predator> predatorsInRange = getPredatorsInRange(predators);
        List<Obstacle> obstaclesInRange = getObstaclesinRange(obstacles);
        if (this.name.Equals("Boid 0"))
        {
            foreach (Movement boid in boids)
            {
                if (!boid.Equals(this))
                {
                    boid.GetComponent<Renderer>().material.color = Color.black;
                }
            }
            foreach (Movement boid in boidsInRange)
            {
                boid.GetComponent<Renderer>().material.color = Color.blue;
            }

        }
        List<Movement> boidsInView = getBoidsInView(boids);


        if (obstaclesInRange.Count != 0)
        {
            acceleration += avoidingObstacles(obstaclesInRange);
        }

        if (predatorsInRange.Count != 0)
        {
            acceleration += avoidingPredator(predatorsInRange);
        }


        if (boidsInRange.Count == 0)
        {
            return;
        }



        if (maxSpeedAlign != 0)
        {
            acceleration += alignBoid(boidsInRange);
        }
        if (maxSpeedCohesion != 0 && boidsInView.Count == 0)
        {
            acceleration += cohesionBoid(boidsInRange);
        }
        if (maxSpeedSeparation != 0)
        {
            acceleration += separationBoid(boidsInRange);
        }


    }

    private List<Predator> getPredatorsInRange(List<Predator> boids)
    {
        return (from predator in boids
                where
                Vector3.Distance(this.transform.position, predator.transform.position) < predator.range
                && (!predator.Equals(this))
                select predator).ToList();
    }

    private List<Movement> getBoidsInView(List<Movement> boidsInRange)
    {
        List<Movement> boidsInView = new List<Movement>();
        foreach (Movement boid in boidsInRange)
        {
            if (Vector3.Distance(this.transform.position, boid.transform.position) < perception
                && (!boid.Equals(this)))
            {
                Vector3 normalizedVelocity = this.velocity;
                normalizedVelocity.Normalize();
                Vector3 normalizedDistanceVector = boid.transform.position - this.transform.position;
                normalizedDistanceVector.Normalize();
                float dot = Vector3.Dot(normalizedVelocity, normalizedDistanceVector);
                if (dot > 0.6f)
                {

                    if (this.name.Equals("Boid 0"))
                    {
                        this.GetComponent<Renderer>().material.color = Color.white;
                        boid.GetComponent<Renderer>().material.color = Color.green;

                    }

                    boidsInView.Add(boid);
                }
            }
        }
        return boidsInView;
    }
}

