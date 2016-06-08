using UnityEngine;
using System.Collections;

public class CameraScripts : MonoBehaviour
{
    public Transform _light;
    public LensFlare lf;
    public float value = 1;

    void Update()
    {
        Vector3 heading = _light.position - GetComponent<Camera>().transform.position;
        float dist = Vector3.Dot(heading, GetComponent<Camera>().transform.forward);
        lf.brightness = value / dist;
    }
}
