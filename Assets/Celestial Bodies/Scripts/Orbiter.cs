using UnityEngine;
using System.Collections;

public class Orbiter : MonoBehaviour
{
    public Orbit orbit;
    float angle;

    public static Vector3 GetPointOnEclipse(float angle, Orbit orbit)
    {
        Vector2 point = new Vector2(orbit.pericenter - orbit.semiMajorAxis + orbit.semiMajorAxis * Mathf.Cos(angle), orbit.semiMinorAxis * Mathf.Sin(angle));
        Vector3 semiFixedPoint = new Vector2(Mathf.Cos(orbit.rotation) * point.x - Mathf.Sin(orbit.rotation) * point.y, Mathf.Sin(orbit.rotation) * point.x + Mathf.Cos(orbit.rotation) * point.y);
        Vector3 axis = new Vector3(-Mathf.Sin(orbit.rotation) * orbit.semiMinorAxis, Mathf.Cos(orbit.rotation) * orbit.semiMinorAxis).normalized;
        Quaternion rotation = Quaternion.AngleAxis(orbit.inclination, axis);
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
        Vector3 fixedPoint = m.MultiplyPoint3x4(semiFixedPoint);

        return fixedPoint;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 10 * Time.deltaTime);
        angle += Time.deltaTime / Vector3.Distance(orbit.parentBody.transform.position, transform.position) * 100;
        transform.position = GetPointOnEclipse(angle, orbit);
    }
}

public struct Orbit
{
    public float apocenter;
    public float pericenter;
    public float eccentricity;
    public float inclination;
    public float semiMajorAxis;
    public float semiMinorAxis;
    public GameObject parentBody;
    public float rotation;

    public Orbit(float _apocenter, float _pericenter, float _inclination, GameObject _parentBody, float _rotation)
    {
        apocenter = _apocenter;
        pericenter = _pericenter;
        eccentricity = (apocenter - pericenter) / (apocenter + pericenter);
        inclination = _inclination % 360;
        semiMajorAxis = (apocenter + pericenter) / 2;
        semiMinorAxis = semiMajorAxis * Mathf.Pow(1 - Mathf.Pow(eccentricity, 2), 0.5f);
        parentBody = _parentBody;
        rotation = _rotation;
    }
}