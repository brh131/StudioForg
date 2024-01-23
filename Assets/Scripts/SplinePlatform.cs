using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Splines;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using Unity.Mathematics;

public class SplinePlatform : MonoBehaviour
{
    SpriteShapeController controller;
    UnityEngine.U2D.Spline u2d_spline;
    UnityEngine.Splines.Spline spline;
    List<BezierKnot> knots = new List<BezierKnot>();
    public GameObject testObject;
    public GameObject testTangent;
    public GameObject testNormal;
    public GameObject lObject;
    public GameObject rObject;
    public float t;

    [Header("Test Variables")]
    public Vector3 observedVelocity;
    public Vector3 lastPosition;
    public float observedSpeed;
    void Awake()
    {
        controller = GetComponent<SpriteShapeController>();
        u2d_spline = controller.spline;
        ReloadSpline();

        foreach(BezierKnot knot in spline)
        {
            Instantiate(testObject, knot.Position, Quaternion.identity);
            Instantiate(lObject, knot.Position + knot.TangentIn, Quaternion.identity);
            Instantiate(lObject, knot.Position + knot.TangentOut, Quaternion.identity);
        }
        testObject.transform.position = spline.EvaluatePosition(0.0f);
        lastPosition = testObject.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        t = (Time.time / 10.0f) % 1f;
        float3 position, tangent, normal;
        spline.Evaluate(t, out position, out tangent, out normal);
        testObject.transform.position = position;
        testTangent.transform.position = position + tangent;
        testNormal.transform.position = testObject.transform.position + GetNormal(t);

        observedVelocity = testObject.transform.position - lastPosition;
        observedVelocity /= Time.deltaTime;
        observedSpeed = observedVelocity.magnitude;
        lastPosition = testObject.transform.position;
    }
    public void ReloadSpline()
    {
        spline = new UnityEngine.Splines.Spline();
        spline.Closed = !u2d_spline.isOpenEnded;
        int count = u2d_spline.GetPointCount();
        for (int i = 0; i < count; i++)
        {
            Vector3 pt = transform.TransformPoint(u2d_spline.GetPosition(i));
            Vector3 lTan = Vector3.Scale(u2d_spline.GetLeftTangent(i), transform.localScale);
            Vector3 rTan = Vector3.Scale(u2d_spline.GetRightTangent(i), transform.localScale);

            spline.Add(new BezierKnot(pt, lTan, rTan));
        }
    }
// Built in normal calculation sucks ass for my purposes so I will make my own.
    public Vector3 GetNormal(float t)
    {
        Vector3 tangent = spline.EvaluateTangent(t);
        return Vector3.Normalize(Vector3.Cross(tangent, Vector3.back));
    }
}
