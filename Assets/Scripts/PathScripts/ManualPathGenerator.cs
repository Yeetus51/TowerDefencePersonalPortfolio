using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ManualPathGenerator : WayPointPath
{
    [SerializeField]
    List<Transform> points = new List<Transform>();

    [EditorCools.Button]
    private void Update_Geometry() => UpdateGeometry();


    // Start is called before the first frame update
    void Start()
    {
    }

    void GeneratePath()
    {
        // PathDrawer pathDrawer;
        // if (gameObject.GetComponent<PathDrawer>() != null) pathDrawer = gameObject.GetComponent<PathDrawer>();
        // else pathDrawer = gameObject.AddComponent<PathDrawer>();


        path = ConvertPoints(); 
        UpdateGizmos();
       // pathDrawer.points = ConvertPoints();
        //pathDrawer.UpdateGizmos();
    }

    Vector2[] ConvertPoints()
    {
        List<Vector2> newPoints = new List<Vector2>();
        foreach (Transform point in points) newPoints.Add(new Vector2(point.position.x, point.position.z));

            return newPoints.ToArray(); 
    }

    // Update is called once per frame
    void Update()
    {
        GeneratePath(); 
    }
}
