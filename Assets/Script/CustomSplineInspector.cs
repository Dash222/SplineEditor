using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class CustomSplineInspector : Editor
{
    Spline self;
    private void OnEnable()
    { 
        self = (Spline)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save"))
        {
            self.Save();
        }
        if (GUILayout.Button("Load"))
        {
            self.Load();
        }
    }

    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        

        Handles.color = Color.red;
        float rad = 0.10f;
        for (int i = 0; i < self.points.Count; i++)
        {
            self.points[i] = Handles.FreeMoveHandle(self.points[i], Quaternion.identity,
                HandleUtility.GetHandleSize(self.points[i]) * rad, Vector3.one, Handles.SphereHandleCap);
        }
        
        switch (self.type)
        {
            case Spline.SplineType.Hermit:
                self.CreateHermit();
                break;
            case Spline.SplineType.Bezier:
                self.CreateBezier();
                break;
            case Spline.SplineType.BSplines:
                self.CreateBSplines();
                break;
            case Spline.SplineType.CatmullRomSpline:
                self.CreateCatmullRomSpline();
                break;
            default:
                break;
        }
        Handles.color = Color.white;
        Draw(self.GetSplinePoints());
    }
    private void Draw(Vector3[] points)
    {
        for (int i = 1; i < points.Length; i++)
            Handles.DrawLine(points[i - 1], points[i]);
    }
}
