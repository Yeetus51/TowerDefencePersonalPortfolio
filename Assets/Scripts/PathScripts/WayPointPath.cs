using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPointPath : MonoBehaviour
{
    [HideInInspector]
    public Vector2[] path;

    [SerializeField] GameObject pathObject;
    [SerializeField] GameObject circleObject; 
    [SerializeField] GameObject pathSpawnPoint;

    public float pathWidth; 



    public void UpdateGizmos()
    {
        for (int i = 1; i < path.Length; i++)
        {

            Debug.DrawLine(new Vector3(path[i].x, 0, path[i].y), new Vector3(path[i - 1].x, 0, path[i - 1].y), Color.white);
        }
    }

    public void UpdateGeometry()
    {
        int childCount = pathSpawnPoint.transform.childCount;
        for (int i = childCount -1; i >= 0; i--)
        {
            DestroyImmediate(pathSpawnPoint.transform.GetChild(i).gameObject); 
        }

        for (int i = 1; i < path.Length; i++)
        {
            GameObject plane = Instantiate(pathObject);
            plane.transform.position = (new Vector3(path[i].x, 0, path[i].y) + new Vector3(path[i - 1].x, 0, path[i - 1].y))/2;
            plane.transform.parent = pathSpawnPoint.transform;
            Vector3 directionVector = new Vector3(path[i].x, 0, path[i].y) - new Vector3(path[i - 1].x, 0, path[i - 1].y);
            plane.transform.right = directionVector.normalized;
            plane.transform.localScale = new Vector3(directionVector.magnitude/10, plane.transform.localScale.y, pathWidth);
            plane.SetActive(true);

            GameObject circle =  Instantiate(circleObject);
            circle.transform.position = new Vector3(path[i].x, 0, path[i].y);
            circle.transform.parent = pathSpawnPoint.transform;
            circle.transform.localScale = new Vector3(plane.transform.localScale.z*10, circle.transform.localScale.y, plane.transform.localScale.z*10);
            circle.SetActive(true);
        }
    }
}
