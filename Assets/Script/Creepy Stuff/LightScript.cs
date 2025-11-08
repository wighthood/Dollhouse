using UnityEngine;

public class LightScript : MonoBehaviour
{

    private Light _light;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _light = GetComponent<Light>();
    }
    
    public void SetLight(float intensity, Color color)
    {
        _light.intensity = intensity;
        _light.color = color;
    }
    
    public void SetLight(float intensity)
    {
        _light.intensity = intensity;
    }
    
    public void SetLight(Color color)
    {
        _light.color = color;
    }
}
