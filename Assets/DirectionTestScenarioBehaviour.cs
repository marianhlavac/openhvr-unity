using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DirectionTestScenarioBehaviour : MonoBehaviour
{
    public OpenHVRDevices devicesEnumerator;
    public TextMesh instructionTextLabel;
    public OpenHVREffect dynamicEffect;
    public AudioSource music;
    public Transform userHand;
    public string resultsOutputDirectory;
    public Text subjectName;

    private OpenHVRManager.Device[] devices;
    private int position = -1;
    private bool awaitsTrigger = false;
    private List<Tuple<Vector3, Vector3, string>> measurements = new List<Tuple<Vector3, Vector3, string>>();
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
        if (position + 1 >= devices.Length)
        {
            instructionTextLabel.text = "Test je hotov!";
            StoreResults();
            return;
        }

        var device = devices[++position];
        if (device.EffectType != (int)OpenHVRManager.EffectType.Wind)
        {
            SpinupNext();
            return;
        }

        dynamicEffect.transform.position = device.Location;
        dynamicEffect.transform.rotation = device.Rotation;
        dynamicEffect.Play();
        music.Play();

        instructionTextLabel.text = "Chvilku čekej";
        Invoke("AwaitInteraction", 12f);
    }

    private void AwaitInteraction()
    {
        instructionTextLabel.text = "Odkud fouká?";
        awaitsTrigger = true;
    }

    private void SaveResults()
    {
        var device = devices[position];
        var realDirection = (device.Location - userHand.position).normalized;
        var reportedDirection = userHand.forward;
        measurements.Add(Tuple.Create(realDirection, reportedDirection, device.Name));
    }

    private void PrepareForNextFan()
    {
        dynamicEffect.Cancel();
        music.Stop();
        instructionTextLabel.text = "Ok!";
        Invoke("SpinupNext", 3f);
    }

    private void StoreResults()
    {
        string filename = "results-" + subjectName.text + "-direction-" + DateTime.Now.ToFileTime() + ".csv";
        Debug.Log("Saving results to " + filename);
        using (StreamWriter sw = File.AppendText(Path.Combine(resultsOutputDirectory, filename)))
        {
            sw.WriteLine("devicename;real;reported;difference");
            foreach ((Vector3 real, Vector3 reported, string deviceName) in measurements)
            {
                sw.Write(deviceName + ";");
                sw.Write(real.ToString() + ";");
                sw.Write(reported.ToString() + ";");
                sw.WriteLine((reported - real).magnitude.ToString());
            }
        }
    }
}
