using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{

    [SerializeField] Light light;
    [SerializeField] Animator lustreAnim;
    [SerializeField] List<Light> doorsLight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void DeactivateAnim()
    {
        lustreAnim.enabled = false;
    }
    public void SetLight(float intensity, Color color)
    {
        light.intensity = intensity;
        light.color = color;
    }
    
    public void SetLight(float intensity)
    {
        light.intensity = intensity;
    }
    
    public void SetLight(Color color)
    {
        light.color = color;
    }

    public void SetDoorsLight(int side,bool value)
    {
        doorsLight[side].gameObject.SetActive(value);
    }
}
