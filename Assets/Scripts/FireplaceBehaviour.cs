using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceBehaviour : MonoBehaviour
{
    public GameObject fireEffect;
    public OpenHVREffect warmnessEffect;
    public AudioSource soundEffect;

    [Space]
    public bool enableVisualEffect = true;
    public bool enableWarmnessEffect = true;

    private bool hasBurned = false;
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.StartsWith("Match") && !hasBurned)
        {
            hasBurned = true;
            foreach (ParticleSystem effect in fireEffect.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
                Invoke("FireEnds", 20);
            }
            soundEffect.Play();
            if (enableVisualEffect)
            {
                GameObject.FindObjectOfType<TemperatureLightBehaviour>().SetTemperatureTarget(TemperatureLightBehaviour.TemperatureTarget.Warm);
                Invoke("VisualEffectEnds", 20);
            }
            if (enableWarmnessEffect)
            {
                warmnessEffect.Play();
            }
        }
    }

    void FireEnds()
    {
        foreach (ParticleSystem effect in fireEffect.GetComponentsInChildren<ParticleSystem>())
        {
            effect.Stop();
        }
    }

    void VisualEffectEnds()
    {
        GameObject.FindObjectOfType<TemperatureLightBehaviour>().SetTemperatureTarget(TemperatureLightBehaviour.TemperatureTarget.Normal);
    }


}
