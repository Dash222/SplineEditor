using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SplineData
{
    public List<Vector3> points;
    public Spline.SplineType type;

    public SplineData(Spline spline)
    {
        points = spline.points;
        type = spline.type;
    }
}