using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStickBehaviour : MonoBehaviour
{
    public GameObject fireEffect;
    public void Burn()
    {
        foreach (ParticleSystem effect in fireEffect.GetComponentsInChildren<ParticleSystem>()) {
            effect.Play();
        }
    }
    public void PutOut()
    {
        foreach (ParticleSystem effect in fireEffect.GetComponentsInChildren<ParticleSystem>())
        {
            effect.Stop();
        }
    }
}
