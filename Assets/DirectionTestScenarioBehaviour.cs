using System.Linq;
using System;
using UnityEngine;
using UnityEngine.iOS;

public class DirectionTestScenarioBehaviour : MonoBehaviour
{
    public OpenHVRDevices devicesEnumerator;
    public TextMesh instructionTextLabel;
    public OpenHVREffect dynamicEffect;
    public AudioSource music;
    public Transform userHand;
    public Transform userHead;

    private OpenHVRManager.Device[] devices;
    private int position = -1;
    private bool awaitsTrigger = false;
    void Start() {
        instructionTextLabel.text = "Čekej, načítám...";
        devicesEnumerator.onDevicesLoaded += BeginScenario;
    }

    void Update() {
        if (awaitsTrigger && Input.GetAxis("XRI_Combined_Trigger") > 0.5f) {
            SaveResults();
            PrepareForNextFan();
            awaitsTrigger = false;
        }
    }

    public void BeginScenario(OpenHVRManager.Device[] devices) {
        this.devices = devices;

        instructionTextLabel.text = "Začínáme test";
        Invoke("SpinupNext", 5f);
    }

    private void SpinupNext() {
        var device = devices[++position];
        dynamicEffect.transform.position = device.Location;
        dynamicEffect.transform.rotation = device.Rotation;
        dynamicEffect.Play();
        music.Play();

        instructionTextLabel.text = "Chvilku čekej";
        Invoke("AwaitInteraction", 10f);
    }

    private void AwaitInteraction()
    {
        instructionTextLabel.text = "Odkud fouká?";
        awaitsTrigger = true;
    }

    private void SaveResults()
    {
        var device = devices[position];
        var realDirection = (device.Location - userHead.position).normalized;
        var reportedDirection = userHand.forward;

        Debug.Log(reportedDirection);
        Debug.Log(realDirection);
        Debug.Log((realDirection - reportedDirection).magnitude);
    }

    private void PrepareForNextFan()
    {
        dynamicEffect.Cancel();
        music.Stop();
        if (position + 1 >= devices.Length)
        {
            instructionTextLabel.text = "Test je hotov!";
            return;
        }
        instructionTextLabel.text = "Ok!";
        Invoke("SpinupNext", 10f);
    }
}
