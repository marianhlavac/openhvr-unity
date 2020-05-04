using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceBehaviour : MonoBehaviour
{
    public GameObject fireEffect;
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.StartsWith("Match"))
        {
            foreach (ParticleSystem effect in fireEffect.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }
        }
    }


}
