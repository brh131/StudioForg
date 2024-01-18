using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Splines;
using Unity.VisualScripting;
using UnityEditor.SearchService;

public class SplinePlatform : MonoBehaviour, ISpline
{
    SpriteShapeController controller;
    UnityEngine.U2D.Spline spline;
    List<BezierKnot> knots = new List<BezierKnot>();
    public GameObject testObject;
    public GameObject testTangent;
    public GameObject testNormal;
    public GameObject lObject;
    public GameObject rObject;
    public float t;

    void Awake()
    {
        controller = GetComponent<SpriteShapeController>();
        spline = controller.spline;
        ReloadSpline();

        float partsSum = 0f;
        for(int i = 0; i < knots.Count; i++)
        {
            BezierKnot knot = knots[i];
            Instantiate(testObject, knot.Position, Quaternion.identity);
            Instantiate(lObject, knot.Position + knot.TangentIn, Quaternion.identity);
            Instantiate(lObject, knot.Position + knot.TangentOut, Quaternion.identity);
            StartCoroutine(CurvesTest(GetCurve(i)));
            float partLength = GetCurveLength(i);
            print(String.Format("Length of curve {0} is {1}",i,partLength));
            partsSum += partLength;
        }
        print(String.Format("Sum of curve lengths is {0}", partsSum));
        print(String.Format("Total length via method is {0}", GetLength()));

    }

    public IEnumerator<BezierKnot> GetEnumerator()
    {
        return knots.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    // Update is called once per frame
    void Update()
    {
        //t = Time.time;
        //testObject.transform.position = GetPoint(t);
        //testTangent.transform.position = testObject.transform.position + GetTangent(t);
        //testNormal.transform.position = testObject.transform.position + GetNormal(t);
    }

    public void ReloadSpline()
    {
        knots = new List<BezierKnot>();
        int count = spline.GetPointCount();
        for (int i = 0; i < count; i++)
        {
            Vector3 pt = transform.TransformPoint(spline.GetPosition(i));
            Vector3 lTan = Vector3.Scale(spline.GetLeftTangent(i), transform.localScale);
            Vector3 rTan = Vector3.Scale(spline.GetRightTangent(i), transform.localScale);

            knots.Add(new BezierKnot(pt,lTan,rTan));
        }
    }
    public BezierCurve GetCurve(int i)
    {
        int start;
        int end;
        if(Closed)
        {
            start = i % Count;
            end = (i + 1) % Count;
        }
        else
        {
            start = i;
            end = i + 1;
        }
        return new BezierCurve(this[start], this[end]);
    }

    public float GetCurveLength(int i)
    {
        return CurveUtility.CalculateLength(GetCurve(i));
    }

    public float GetLength()
    {
        float sum = 0f;
        int numCurves;
        if(Closed)
        {
            numCurves = Count;
        }
        else
        {
            numCurves = Count - 1;
        }

        for(int i = 0; i<numCurves; i++)
        {
            sum += GetCurveLength(i);
        }
        return sum;
    }
    /*
    Vector3 GetPoint(float t)
    {
        if(spline.isOpenEnded)
        {
            if (t < 0f || t > knots.Count)
                throw new IndexOutOfRangeException("Spline parameter out of range for open-ended spline.");

            int seg = Mathf.FloorToInt(t);
            return BezierUtility.BezierPoint(knots[seg] + rTangents[seg], knots[seg], knots[seg + 1], knots[seg + 1] + lTangents[seg + 1], t);
        }
        else
        {
            // closed splines have one extra segment than open ended ones: from Knot n-1 to Knot 0.
            t = t % knots.Count;
            if (t < 0f) { t += knots.Count; }
            int seg = Mathf.FloorToInt(t % knots.Count);
            int end = (seg + 1) % knots.Count;
            return BezierUtility.BezierPoint(knots[seg] + rTangents[seg], knots[seg], knots[end], knots[end] + lTangents[end], t % 1f);
        }
    }

    Vector3 GetTangent(float t)
    {
        //NOTE: Parameters for my tangent function are in a different order than those in the built in BezierPoint.
        if (spline.isOpenEnded)
        {
            if (t < 0f || t > knots.Count)
                throw new IndexOutOfRangeException("Spline parameter out of range for open-ended spline.");

            int seg = Mathf.FloorToInt(t);
            return MyUtils.BezierTangentNormalized(knots[seg], knots[seg] + rTangents[seg], knots[seg + 1] + lTangents[seg + 1], knots[seg + 1], t);
        }
        else
        {
            // closed splines have one extra segment than open ended ones: from Knot n-1 to Knot 0.
            t = t % knots.Count;
            if(t < 0f) { t += knots.Count; }
            int seg = Mathf.FloorToInt(t % knots.Count);
            int end = (seg + 1) % knots.Count;
            return MyUtils.BezierTangentNormalized(knots[seg], knots[seg] + rTangents[seg], knots[end] + lTangents[end], knots[end], t % 1f);
        }
    }
    //This class assumes that all sprite shapes move counter-clockwise (paths moving to the right are 'up' and paths moving to the left are 'down'
    Vector3 GetNormal(float t)
    {
        Vector3 tan = GetTangent(t);
        return Vector3.Normalize(Vector3.Cross(tan, new Vector3(0f,0f,-1f)));
    }
    */
    //ISpline Methods and Properties
    public bool Closed
    {
        get { return !spline.isOpenEnded; }
    }

    public BezierKnot this[int i]
    {
        get
        {
            return knots[i];
        }
    }
    public int Count
    {
        get { return spline.GetPointCount(); }
    }

    IEnumerator CurvesTest(BezierCurve curve)
    {
        GameObject testPoint = Instantiate(testObject, curve.P0, Quaternion.identity);
        while(true)
        {
            Vector3 pos = CurveUtility.EvaluatePosition(curve, (0.5f * Time.time) % 1f);
            testPoint.transform.position = pos;
            yield return null;
        }
    }
}
