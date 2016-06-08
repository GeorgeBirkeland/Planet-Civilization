using UnityEngine;
using System.Collections.Generic;

public class PlanetMaker : MonoBehaviour
{
    public Shader triplanar;
    public Texture water;
    public Texture ground;

    void Start()
    {
        CreatePlanet(10, 0.05f, 0.5f, new Color(Random.value, Random.value, Random.value), 10, new Color(Random.value, Random.value, Random.value), true, new Orbit(700, 200, 25, GameObject.Find("Sun"), 86));
    }

    void Update()
    {

    }

    void CreatePlanet(float radius, float variation, float seaLevel, Color planetColor, float atmosphereRadius, Color atmosphereColor, bool habitableAtmosphere, Orbit orbit)
    {
        PlanetVars planetVars = new PlanetVars(radius, variation, seaLevel, planetColor, atmosphereRadius, atmosphereColor, habitableAtmosphere, orbit);
        GameObject planet = new GameObject();
        Icosphere.Create(planet, 6, planetVars.radius, Mathf.RoundToInt(Random.value * int.MaxValue), planetVars.variation, planetVars.variation);
        planet.AddComponent<MeshRenderer>();
        planet.AddComponent<MeshCollider>();
        planet.AddComponent<Orbiter>();

        //Generate Material
        Material planetMaterial = new Material(triplanar);
        planetMaterial.SetTexture("_MainTex", ground);
        planetMaterial.SetTexture("_MainTex1", ground);
        planetMaterial.SetTexture("_MainTex2", ground);
        planet.GetComponent<MeshRenderer>().material = planetMaterial;

        //Generate Orbit
        planet.GetComponent<Orbiter>().orbit = planetVars.orbit;

        //Make ocean
        GameObject ocean = new GameObject();
        Icosphere.Create(ocean, 6, planetVars.radius, Mathf.RoundToInt(Random.value * int.MaxValue), 0, 0);
        ocean.AddComponent<MeshRenderer>();
        ocean.transform.parent = planet.transform;

        //Make ocean material
        Material oceanMaterial = new Material(triplanar);
        oceanMaterial.SetTexture("_MainTex", water);
        oceanMaterial.SetTexture("_MainTex1", water);
        oceanMaterial.SetTexture("_MainTex2", water);
        ocean.GetComponent<MeshRenderer>().material = oceanMaterial;


        //Set starting position
        planet.transform.localScale = new Vector3(radius, radius, radius);
        planet.transform.position = Orbiter.GetPointOnEclipse(90, planetVars.orbit);
    }
}

/// <summary>
/// A type countaining all the variables needed to generate a planet
/// </summary>
public struct PlanetVars
{
    public float radius;
    public float variation;
    public float seaLevel;
    public Color planetColor;
    public float atmosphereRadius;
    public Color atmosphereColor;
    public bool habitableAtmosphere;
    public Orbit orbit;

    /// <summary>
    /// A type countaining all the variables needed to generate a planet
    /// </summary>
    /// <param name="Radius (>0)"></param>
    /// <param name="Variation (>=0)"></param>
    /// <param name="Sea Level (0-1)"></param>
    /// <param name="Planet Color"></param>
    /// <param name="Atmosphere Radius (>=0)"></param>
    /// <param name="Atmosphere Color"></param>
    /// <param name="Habitable Atmosphere"></param>
    public PlanetVars(float _radius, float _variation, float _seaLevel, Color _planetColor, float _atmosphereRadius, Color _atmosphereColor, bool _habitableAtmosphere, Orbit _orbit) : this()
    {
        radius = Clamp(_radius, 0, false);
        variation = Clamp(_variation, 0, true);
        seaLevel = Clamp(_seaLevel, 0, 1, true, true);
        planetColor = _planetColor;
        atmosphereRadius = Clamp(_atmosphereRadius, 0, true);
        atmosphereColor = _atmosphereColor;
        habitableAtmosphere = _habitableAtmosphere;
        orbit = _orbit;
    }

    float Clamp(float value, float min, float max, bool inclusiveMin, bool inclusiveMax)
    {
        if (inclusiveMin && inclusiveMax)
            return (value <= min) ? min : (value >= max) ? max : value;
        else if (inclusiveMin)
            return (value <= min) ? min : (value > max) ? max : value;
        else if (inclusiveMax)
            return (value < min) ? min : (value >= max) ? max : value;
        else
            return (value < min) ? min : (value > max) ? max : value;
    }

    float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    float Clamp(float value, float min, bool inclusiveMin)
    {
        if (inclusiveMin)
            return (value <= min) ? min : value;
        else
            return (value < min) ? min : value;
    }
}