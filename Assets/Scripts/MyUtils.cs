using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;

public static class MyUtils
{
    public static Vector2 Project(Vector2 v1, Vector2 on)
    {
        return Vector2.Dot(v1, on) / on.SqrMagnitude() * on;
    }

    public static Vector3 BezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float rt = 1f - t;
        return rt * rt * rt * p0 + 3f * rt * rt * t * p1 + 3f * rt * t * t * p2 + t * t * t * p3;
    }

    public static Vector2 BezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float rt = 1f - t;
        return rt * rt * rt * p0 + 3f * rt * rt * t * p1 + 3f * rt * t * t * p2 + t * t * t * p3;
    }
    public static Vector3 BezierTangent(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        Vector3 p0p1 = p1 - p0;
        Vector3 p1p2 = p2 - p1;
        Vector3 p2p3 = p3 - p2;
        float rt = 1f - t;
        return 3f*rt*rt*p0p1 + 6f*rt*t*p1p2 + 3f*t*t*p2p3;
    }
    public static Vector3 BezierTangentNormalized(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return Vector3.Normalize(BezierTangent(p0, p1, p2, p3, t));
    }
}

public class BezierSegment
{
    Vector3 p0;
    Vector3 p1;
    Vector3 p2;
    Vector3 p3;

    public BezierSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p0 = p0; 
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public Vector3 GetPosition(float t)
    {
        return BezierUtility.BezierPoint(p0, p1, p2, p3, t);
    }
    public Vector3 GetTangent(float t) {
        return MyUtils.BezierTangent(p0, p1, p2, p3, t);
    }
    //This class assumes that all splines move counter-clockwise (paths moving to the right are 'up' and paths moving to the left are 'down').
    public Vector3 GetNormal(float t)
    {
        Vector3 tan = GetTangent(t);
        return Vector3.Normalize(Vector3.Cross(tan,new Vector3(0f,0f,-1f)));

    }
}
