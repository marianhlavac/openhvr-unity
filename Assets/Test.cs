using UnityEngine;

public class Test : MonoBehaviour {
    public OpenHVREffect effect;

    void Start() {
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            effect.Play();
        }
        if (Input.GetButtonDown("Fire2")) {
            effect.Cancel();
        }
    }
}
