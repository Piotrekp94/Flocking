using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int size = 1000;
    public int range = 2000;

    public Collider coll;
    public void Start()
    {
        coll = GetComponent<Collider>();
    }

    public float getDistance(Vector3 explosionPos)
    {
        Vector3 closestPoint = coll.ClosestPointOnBounds(explosionPos);
        float distance = Vector3.Distance(closestPoint, explosionPos);
        return distance;
    }
    public Vector3 getDiff(Vector3 diff)
    {
        Vector3 closestPoint = coll.ClosestPointOnBounds(diff);
        return diff - closestPoint;
    }
}
