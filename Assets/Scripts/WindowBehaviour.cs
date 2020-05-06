using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindowBehaviour : MonoBehaviour
{
    public float breakThreshold = 2.0f;
    public GameObject normalGlass;
    public GameObject brokenGlass;
    public GameObject shatterPrefab;
    public ParticleSystem particleEffect;
    public OpenHVREffect windEffect;
    [Space]
    public bool enableSoundEffect = true;
    public bool enableVisualEffect = true;
    public bool enableWindEffect = true;


    private bool isBroken = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isBroken && !collision.collider.CompareTag("Shatters"))
        {
            float bamStrength = collision.relativeVelocity.magnitude;
            if (bamStrength > breakThreshold)
            {
                Destroy(normalGlass);
                brokenGlass.SetActive(true);
                Func<float> genForce = () => { return Random.Range(-100f, 100f) * bamStrength; };
                Func<float> genTorq = () => { return Random.Range(-30f, 30f) * bamStrength; };

                for (int i = 0; i < 18; i++)
                {
                    var shatter = Instantiate(shatterPrefab);
                    shatter.transform.position = collision.transform.position;
                    shatter.GetComponent<Rigidbody>().AddForce(new Vector3(genForce(), genForce(), genForce()));
                    shatter.GetComponent<Rigidbody>().AddTorque(new Vector3(genTorq(), genTorq(), genTorq()));
                    shatter.transform.localScale = Vector3.one * Random.Range(0.8f, 3f);
                }
                windEffect.Play();
                particleEffect.Play();

                isBroken = true;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
