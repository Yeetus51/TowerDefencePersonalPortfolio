using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class PathAnchor
{
    public Transform transform;
    public Transform sprite;
    [HideInInspector]
    public Vector2 normal; 
}
[ExecuteInEditMode]
public class AutoPathGenerator : WayPointPath
{

    [SerializeField]
    PathAnchor startAnchor;

    [SerializeField]
    PathAnchor endAnchor;

    [SerializeField]
    Transform[] borders; 


    [MinMaxSlider(-180,180f)]
    [SerializeField] Vector2 lineAngleRange;

    [MinMaxSlider(0f, 10f)]
    [SerializeField] Vector2 lineLengthRange;


    float time;


    [EditorCools.Button]
    private void Generate_Path() => GeneratePath();

    Vector2[] GeneratePoints()
    {
        int crashCount = 0;
        int stepBackCount = 0; 

        List<Vector2> borderCorners = new List<Vector2>
        {
            new Vector2(borders[0].position.x, borders[3].position.z),
            new Vector2(borders[0].position.x, borders[1].position.z),
            new Vector2(borders[2].position.x, borders[1].position.z),
            new Vector2(borders[2].position.x, borders[3].position.z),
            new Vector2(borders[0].position.x, borders[3].position.z)
        };

        List<Vector2> points = new List<Vector2>();

        Vector2 normalVector = startAnchor.normal;
        points.Add(new Vector2(startAnchor.sprite.position.x, startAnchor.sprite.position.z));
        points.Add(new Vector2(startAnchor.sprite.position.x, startAnchor.sprite.position.z) + startAnchor.normal);

        float maxPointCount = 200; 
        int index = 0; 
        while(index < maxPointCount)
        {
            float calculatedAngle = CalculateEndAngle(points[points.Count-1], endAnchor.sprite.position, normalVector);
            float randomInterplation = 1 ;
            float factor = index / maxPointCount;
            factor += 1/ GetPointsDistance(points[points.Count - 1], endAnchor.sprite.position)*0.5f; 
             

            float lineLength = UnityEngine.Random.Range(lineLengthRange.x, lineLengthRange.y);
            float angle = UnityEngine.Random.Range(Mathf.Clamp(Mathf.Lerp(-randomInterplation, calculatedAngle , factor), -1f,1f), Mathf.Clamp(Mathf.Lerp(randomInterplation, calculatedAngle, factor), -1f, 1f));
            //if (index > 20) angle = calculatedAngle; 
            Vector2 vector = (normalVector).normalized * lineLength;
            vector = RotateVector(vector, angle);
            Vector2 newPoint = vector + points[points.Count - 1];


            if (CheckIntersection(points[points.Count - 1], newPoint, borderCorners))
            {
               if(GetPointsDistance(newPoint, endAnchor.sprite.position) < 2)
                {
                    Debug.Log("EndFOund! ");
                    points.Add(newPoint);
                    break; 
                }
                crashCount++;
                if (crashCount > 10)
                {
                    stepBackCount++;
                    crashCount = 0;
                    normalVector = points[points.Count - (stepBackCount + 1)] - points[points.Count - (stepBackCount + 2)];
                    points.RemoveAt(points.Count-1);
                   // Debug.Log("Step taken Back : " + stepBackCount + " number of points : " + points.Count);
                    continue;
                }
                continue; 
            }
            else
            {
                normalVector = vector; 
                crashCount = 0; 
                stepBackCount = 0;
            }
            points.Add(newPoint);
            index++; 
        }
        if (index > maxPointCount -1)
        {
            //GeneratePoints();
        }

        return points.ToArray();


    }

    float CalculateEndAngle(Vector2 startPoint, Vector3 endPoint , Vector2 normalVector)
    {
        float angle = Mathf.Atan2((endPoint.x - startPoint.x), (endPoint.z - startPoint.y));
        float normalAngle = Mathf.Atan2(normalVector.x , normalVector.y);
        return normalAngle - angle; 
    }

    float GetPointsDistance(Vector2 p1, Vector3 p2)
    {
        Vector2 diff = p1 - new Vector2(p2.x, p2.z); 


        return diff.magnitude; 

    }



    bool CheckIntersection(Vector2 point1, Vector2 point2, List<Vector2> vector2List)
    {
        for (int i = 0; i < vector2List.Count - 1; i++)
        {
            Vector2 p1 = vector2List[i];
            Vector2 p2 = vector2List[i + 1];

            if (LinesIntersect(point1, point2, p1, p2)) return true;
        }

        return false;
    }
    bool LinesIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
    {
        float denominator = (point4.y - point3.y) * (point2.x - point1.x) - (point4.x - point3.x) * (point2.y - point1.y);
        float ua = (point4.x - point3.x) * (point1.y - point3.y) - (point4.y - point3.y) * (point1.x - point3.x);
        float ub = (point2.x - point1.x) * (point1.y - point3.y) - (point2.y - point1.y) * (point1.x - point3.x);

        if (denominator == 0)
        {
            if (ua == 0 && ub == 0) return true;
            else return false;
        }
        else
        {
            ua /= denominator;
            ub /= denominator;
            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1) return true;
            else return false;
        }
    }

    void GeneratePath()
    {
        path = GeneratePoints(); 
        UpdateGizmos();
    }
    private void Update()
    {

        SnapToBorder(startAnchor);
        SnapToBorder(endAnchor);

        time += Time.deltaTime;
        
        Debug.DrawLine(new Vector3(borders[0].position.x, 0, borders[3].position.z), new Vector3(borders[0].position.x, 0, borders[1].position.z), Color.red);
        Debug.DrawLine(new Vector3(borders[0].position.x, 0, borders[1].position.z), new Vector3(borders[2].position.x, 0, borders[1].position.z), Color.red);
        Debug.DrawLine(new Vector3(borders[2].position.x, 0, borders[1].position.z), new Vector3(borders[2].position.x, 0, borders[3].position.z), Color.red);
        Debug.DrawLine(new Vector3(borders[2].position.x, 0, borders[3].position.z), new Vector3(borders[0].position.x, 0, borders[3].position.z), Color.red);


        if (time > 0.1f)
        {
            GeneratePath();
            time = 0;
        }
    }

    Vector3 ContainTransform(Vector3 transform)
    {
        Vector3 newTransform = transform;
        if (transform.x > borders[2].position.x) newTransform.x = borders[2].position.x;
        if (transform.x < borders[0].position.x) newTransform.x = borders[0].position.x;
        if (transform.z > borders[1].position.z) newTransform.z = borders[1].position.z;
        if (transform.z < borders[3].position.z) newTransform.z = borders[3].position.z;
        return newTransform;
    }

    void SnapToBorder(PathAnchor _snapItem)
    {
        Vector3 containedTransform = ContainTransform(_snapItem.transform.position);
        _snapItem.transform.position = containedTransform;
        Vector2[] points = new Vector2[]
        {
        new Vector2(borders[0].position.x, borders[3].position.z),
        new Vector2(borders[0].position.x, borders[1].position.z),
        new Vector2(borders[2].position.x, borders[1].position.z),
        new Vector2(borders[2].position.x, borders[3].position.z)
        };

        float distanceFromLine = float.MaxValue;
        int smallestIndex = -1;
        Vector2 pointPos = new Vector2(_snapItem.transform.position.x , _snapItem.transform.position.z);

        for (int i = 0; i < 4; i++)
        {
            int j = (i + 1) % 4;
            float lineDistance = GetLineDistance(points[i], points[j], pointPos);
            if (lineDistance < distanceFromLine)
            {
                distanceFromLine = lineDistance;
                smallestIndex = (j + 2) % 4;
            }
        }
        Vector2 closeBorder = new Vector2(borders[smallestIndex].position.x, borders[smallestIndex].position.z);
        _snapItem.sprite.position = new Vector3(smallestIndex % 2 != 0 ? pointPos.x : closeBorder.x, 0, smallestIndex % 2 != 0 ? closeBorder.y : pointPos.y);
        _snapItem.normal = new Vector2(smallestIndex % 2 != 0 ?  0: pointPos.x > 0 ? -1 : 1, smallestIndex % 2 != 0 ? pointPos.y > 0 ? -1 : 1 :0);
    }


    float GetLineDistance(Vector2 lineStart, Vector2 lineEnd, Vector2 pointPos)
    {
        lineEnd -= lineStart;
        pointPos -= lineStart;
        return Mathf.Abs(Vector2.Dot(lineEnd.normalized, pointPos));
    }

    Vector2 RotateVector(Vector2 _vector, float _radians)
    {
        return new Vector2(Mathf.Cos(_radians) * _vector.x - Mathf.Sin(_radians) * _vector.y, Mathf.Cos(_radians) * _vector.y + Mathf.Sin(_radians) * _vector.x);
    }
}
