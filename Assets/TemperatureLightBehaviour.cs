using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureLightBehaviour : MonoBehaviour
{
    private float lerpSpeed = 0.005f;
    public enum TemperatureTarget
    {
        Normal,
        Warm,
        Cold
    }
    public Light light;
    private Color currentTarget = new Color(1, 1, 1);
    public void SetTemperatureTarget(TemperatureTarget target)
    {
        switch (target)
        {
            case TemperatureTarget.Normal:
                currentTarget = new Color(1, 1, 1);
                break;
            case TemperatureTarget.Warm:
                currentTarget = new Color(1, 0.53f, 0.19f);
                break;
            case TemperatureTarget.Cold:
                currentTarget = new Color(0.2f, 0.52f, 1);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Lerp to color
        light.color = new Color(
            Mathf.Lerp(light.color.r, currentTarget.r, lerpSpeed),
            Mathf.Lerp(light.color.g, currentTarget.g, lerpSpeed),
            Mathf.Lerp(light.color.b, currentTarget.b, lerpSpeed)
        );
    }
}
