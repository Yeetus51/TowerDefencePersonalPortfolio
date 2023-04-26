using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField]
    public WayPointPath designatedPath;

    //[SerializeField]
    //public float speed;
    [HideInInspector]
    public Vector2[] pathToFollow;


    int pointIndex = 0;
    [HideInInspector] public bool endReached = false;

    public delegate void LifeLostHandler();
    public event LifeLostHandler OnLifeLost;


    private void Awake()
    {
        OnLifeLost += GameManager.Instance.LifeLost; 

        Restart();
    }

    private void Start()
    {

    }

    public void Restart()
    {
        if (designatedPath != null)
        {
            pathToFollow = designatedPath.path;
        }
        else Debug.LogWarning("No Path imported ! ");

        pointIndex = 0;
        endReached = false;

        transform.position = new Vector3(pathToFollow[pointIndex].x, 0, pathToFollow[pointIndex].y); 
        transform.forward = (pathToFollow[pointIndex + 1] - pathToFollow[pointIndex]).normalized; 
    }

    private void FixedUpdate()
    {
       
    }
    public void Translate(float speed)
    {
        if (!endReached)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.z);
            float distToPoint = Vector2.Distance(pos, pathToFollow[pointIndex+1]);
            if (distToPoint < speed)
            {
                pointIndex++;
                if (pointIndex > pathToFollow.Length - 2)
                {
                    endReached = true;
                    OnLifeLost.Invoke(); 
                    return;
                }
                transform.position = new Vector3(pathToFollow[pointIndex].x, 0, pathToFollow[pointIndex].y);
                transform.forward = (new Vector3(pathToFollow[pointIndex+1].x, 0, pathToFollow[pointIndex+1].y) - transform.position).normalized;
            }
            transform.Translate(transform.forward * speed, Space.World);
        }
    }

    private void OnDestroy()
    {
        OnLifeLost -= GameManager.Instance.LifeLost;
    }

}
