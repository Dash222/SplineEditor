using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    public List<Vector3> points;
    private List<Vector3> splinePoints = new List<Vector3>();
    
    public int steps = 25;
    public SplineType type = SplineType.Bezier;
    public GameObject obj = null;

    private int index = 0;
    private float alpha = 0f;
    public float speed = 10f;

    public string fileName = "Spline";

    public enum SplineType
    {
        Hermit = 0,
        Bezier = 1,
        BSplines = 2,
        CatmullRomSpline = 3
    }

    
    private void OnDrawGizmos()
    {
        float rad = 0.10f;
        Gizmos.color = Color.red;
        foreach (Vector3 point in points)
            Gizmos.DrawSphere(point, rad);
        
        Gizmos.color = Color.white;
        Draw();
    }
    private void Update()
    {
        if (obj == null || splinePoints.Count <= 0)
            return;


        if (splinePoints.Count - 1 < index)
            index = 0;

        if (alpha >= 1)
        {
            alpha = 0;
            index = (index + 1) % (splinePoints.Count - 1);
        }
        obj.transform.position = Interpolation(splinePoints[index], splinePoints[index + 1], alpha);
        obj.transform.LookAt(splinePoints[index +1]);
        alpha += Time.deltaTime * speed;
    }

    public Vector3[] GetSplinePoints()
    {
        return splinePoints.ToArray();
    }
    private void Draw()
    {
        for(int i = 1; i < splinePoints.Count; i++)
            Gizmos.DrawLine(splinePoints[i - 1], splinePoints[i]);
    }
    
    public void CreateBezier()
    {
        splinePoints.Clear();
        float step = 0;

        Vector3[] tempPoints = new Vector3[4];

        for (int i = 0; i < 4; i++)
            tempPoints[i] = points[i];
        
        for (int i = 0; i < steps; i++)
        {
            step = (float)i / (steps - 1);
            splinePoints.Add(Bezier(tempPoints, step));
        }
        

        if (points.Count < 7)
            return;

        int nbPoints = points.Count - 4;
        while (nbPoints % 3 != 0)
        {
            nbPoints--;
        }

        for (int i = 1; i <= nbPoints / 3; i++)
        {
            points[i * 3 + 1] = points[i * 3] + points[i * 3] - points[i * 3 - 1];
            for (int j = 0; j < 4; j++)
                tempPoints[j] = points[i * 3 + j];


            for (int j = 1; j < steps; j++)
            {
                step = (float)j / (steps - 1);
                splinePoints.Add(Bezier(tempPoints, step));
            }
        }
    }

    public void CreateHermit()
    {
        splinePoints.Clear();
        float step = 0;

        Vector3[] tempPoints = new Vector3[4];

        for (int i = 0; i < 4; i++)
            tempPoints[i] = points[i];

        for (int i = 0; i < steps; i++)
        {
            step = (float)i / (steps - 1);
            splinePoints.Add(Hermit(tempPoints, step));
        }

        if (points.Count < 7)
            return;

        int nbPoints = points.Count - 4;
        while (nbPoints % 3 != 0)
        {
            nbPoints--;
        }

        for (int i = 1; i <= nbPoints / 3; i++)
        {
            points[i * 3 + 1] = points[i * 3] + points[i * 3] - points[i * 3 - 1];
            for (int j = 0; j < 4; j++)
                tempPoints[j] = points[i * 3 + j];


            for (int j = 1; j < steps; j++)
            {
                step = (float)j / (steps - 1);
                splinePoints.Add(Hermit(tempPoints, step));
            }
        }
    }

    public void CreateBSplines()
    {
        if (points.Count < 4)
            return;

        splinePoints.Clear();

        
        for (int i = 3; i < points.Count; i++)
        {
            int startValue = i == 3 ? 0 : 1;
            for (int j = startValue; j < steps; j++)
            {
                float step = (float)j / (steps - 1);
                splinePoints.Add(BSplines(points.ToArray(), i, step));
            }
        }
    }

    public void CreateCatmullRomSpline()
    {
        if (points.Count < 4)
            return;

        splinePoints.Clear();
        
        for (int i = 3; i < points.Count; i++)
        {
            int startValue = i == 3 ? 0 : 1;
            for (int j = startValue; j < steps; j++)
            {
                float step = (float)j / (steps - 1);
                splinePoints.Add(CatmullRomSpline(points.ToArray(), i, step));
            }
        }
    }

    static Vector3 Hermit(Vector3[] points, float alpha)
    {
        if (points.Length != 4)
            return Vector3.zero;

        Vector3 R1 = points[1] - points[0];
        Vector3 R2 = points[3] - points[2];

        return (2 * Mathf.Pow(alpha, 3) - 3 * Mathf.Pow(alpha, 2) + 1) * points[0] +
              (-2 * Mathf.Pow(alpha, 3) + 3 * Mathf.Pow(alpha, 2)) * points[3] +
              (Mathf.Pow(alpha, 3) - 2 * Mathf.Pow(alpha, 2) + alpha) * R1 + 
              (Mathf.Pow(alpha, 3) - Mathf.Pow(alpha, 2)) * R2;
    }

    static Vector3 Bezier(Vector3[] points, float alpha)
    {
        if (points.Length != 4)
            return Vector3.zero;

        return Mathf.Pow(1 - alpha, 3) * points[0] + 3 * alpha * Mathf.Pow(1 - alpha, 2) * points[1] + 3 * Mathf.Pow(alpha, 2) * (1 - alpha) * points[2] + Mathf.Pow(alpha, 3) * points[3];
    }

    static Vector3 BSplines(Vector3[] points, int i,float alpha)
    {   
        return 1f/6f * (Mathf.Pow(1 - alpha, 3) * points[i - 3] + (3 * Mathf.Pow(alpha, 3) - 6 * Mathf.Pow(alpha, 2) + 4) * points[i - 2] + (-3 * Mathf.Pow(alpha, 3) + 3 * Mathf.Pow(alpha, 2) + 3 * alpha + 1) * points[i - 1] + Mathf.Pow(alpha, 3) * points[i]);
    }

    static Vector3 CatmullRomSpline(Vector3[] points, int i, float alpha)
    {
        if (points.Length < 4)
            return Vector3.zero;

        Vector4 T = new Vector4(Mathf.Pow(alpha, 3), Mathf.Pow(alpha, 2), alpha, 1);
        Matrix4x4 G = new Matrix4x4(points[i - 3], points[i - 2], points[i - 1], points[i]);
        Matrix4x4 M = new Matrix4x4
            (
                new Vector4(-0.5f, 1.5f, -1.5f, 0.5f),
                new Vector4(1f, -2.5f, 2f, -0.5f),
                new Vector4(-0.5f, 0, 0.5f, 0),
                new Vector4(0, 1f, 0, 0)
            );
        return G * M * T;
    }
    Vector3 Interpolation(Vector3 pt1, Vector3 pt2, float alpha)
    {
        alpha = alpha < 0f ? 0f: alpha > 1f ? 1f : alpha;

        return (1 - alpha) * pt1 + alpha * pt2;
    }
    public void Save()
    {
        SaveSystem.SaveSpline(this, fileName);
    }

    public void Load()
    {
        SplineData data = SaveSystem.LoadSpline(fileName);
        points = data.points;
        type = data.type;
    }
}
