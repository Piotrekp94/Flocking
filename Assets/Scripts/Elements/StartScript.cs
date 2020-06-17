using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class StartScript : MonoBehaviour
{
    public GameObject boidPrefab;
    public GameObject obstaclePrefab;
    public GameObject predatorPrefab;

    public int boidsAmount;
    public int obstaclesAmount;
    public int predatorsAmount;
    List<Movement> boids = new List<Movement>();
    List<Obstacle> obstacles = new List<Obstacle>();
    List<Predator> predators = new List<Predator>();


    void Start()
    {

        for (int i = 0; i < boidsAmount; i++)
        {
            Vector3 position = new Vector3(Random.Range(1000, 10000), Random.Range(5, 10000), Random.Range(5, 10000));
            Movement boid = Instantiate(boidPrefab, position, Quaternion.identity).GetComponent<Movement>();
            boid.name = "Boid " + i;
            boid.position = position;
            boid.velocity = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f));
            boid.acceleration = new Vector3(0, 0, 0);
            boid.randomDirection = UnityEngine.Random.Range(0, 2) % 2 == 0;
            boids.Add(boid);
            if (boid.name != "Boid 0")
            {
                boid.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                boid.GetComponent<Renderer>().material.color = Color.white;
            }
        }
        //for (int i = 0; i < obstaclesAmount; i++)
        //{
        //    Vector3 position = new Vector3(Random.Range(1000, 10000), Random.Range(1000, 10000), Random.Range(1000, 10000));
        //    Obstacle obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity).GetComponent<Obstacle>();
        //    obstacle.name = "Obstacle " + i;
        //    obstacle.GetComponent<Renderer>().material.color = Color.blue;
        //    obstacles.Add(obstacle);
        //}
        //Vector3 position2 = new Vector3(5000, 5000, 5000);
        //Obstacle obstacle2 = Instantiate(obstaclePrefab, position2, Quaternion.identity).GetComponent<Obstacle>();
        //obstacle2.name = "Obstacle Constant" ;
        //obstacle2.GetComponent<Renderer>().material.color = Color.blue;
        //obstacles.Add(obstacle2);

        GameObject[] cubes = GameObject.FindGameObjectsWithTag("obstacle");
        for(int i  = 0; i <cubes.Length; i++)
        {
            obstacles.Add(cubes[i].GetComponent<Obstacle>());
        }

        for (int i = 0; i < predatorsAmount; i++)
        {
            Vector3 position = new Vector3(Random.Range(5, 10000), Random.Range(1000, 10000), Random.Range(1000, 10000));
            Predator predator = Instantiate(predatorPrefab, position, Quaternion.identity).GetComponent<Predator>();
            predator.name = "Predator " + i;
            predator.GetComponent<Renderer>().material.color = Color.red;
            predators.Add(predator);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Movement boid in boids)
        {
            boid.flock(boids, obstacles, predators);
        }
        foreach (Predator predator in predators)
        {
            predator.hunt(boids);
        }
    }
    public void setSeparationMaxSpeed(Single maxSpeed)
    {
        foreach (Movement boid in boids)
        {
            boid.maxSpeedSeparation = maxSpeed;
        }
    }

    public void setCohesionMaxSpeed(Single maxSpeed)
    {
        foreach (Movement boid in boids)
        {
            boid.maxSpeedCohesion = maxSpeed;
        }
    }
    public void setAlignMaxSpeed(Single maxSpeed)
    {
        foreach (Movement boid in boids)
        {
            boid.maxSpeedAlign = maxSpeed;
        }
    }
}
