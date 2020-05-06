using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class ActuTestScenarioBehaviour : MonoBehaviour
{
    public TextMesh instructionTextLabel;
    public OpenHVREffect effect;
    public AudioSource music;

    private const int totalTests = 5;

    private int awaits = 0;
    private int tests = 0;
    private bool debounceTrigger = true;
    private Stopwatch stopwatch = new Stopwatch();
    private List<long> measurements = new List<long>();

    void Start()
    {
        instructionTextLabel.text = "Příprava testu...";
        Invoke("WaitForUser", 5f);
    }

    void Update()
    {
        if (debounceTrigger && awaits == 1 && Input.GetAxis("XRI_Combined_Trigger") > 0.2f)
        {
            SpinupNext();
            debounceTrigger = false;
        }
        if (debounceTrigger && awaits == 2 && Input.GetAxis("XRI_Combined_Trigger") > 0.2f)
        {
            tests++;
            SaveResults();
            if (tests < totalTests)
            {
                instructionTextLabel.text = "Ok! Ještě " + (totalTests - tests).ToString() + "x";
                Invoke("Delay", 3f);
            }
            else
            {
                instructionTextLabel.text = "Test hotov! Prům. " + Math.Round(measurements.Sum() / (double)totalTests / 1000, 1) + "s";

            }
            awaits = 0;
            debounceTrigger = false;
        }

        if (Input.GetAxis("XRI_Combined_Trigger") < 0.2f) debounceTrigger = true;
    }

    public void Delay()
    {
        instructionTextLabel.text = "Moment...";
        Invoke("WaitForUser", 12f);
    }

    public void WaitForUser()
    {
        if (!music.isPlaying) music.Play();
        instructionTextLabel.text = "Stisk když připraven";
        awaits = 1;
    }

    private void SpinupNext()
    {
        effect.Play();
        stopwatch.Start();
        instructionTextLabel.text = "Cítíš vítr? Stisk!";
        awaits = 2;
    }


    private void SaveResults()
    {
        effect.Cancel();
        var elapsed = stopwatch.ElapsedMilliseconds;
        measurements.Add(elapsed);
        stopwatch.Reset();

        UnityEngine.Debug.Log(elapsed);
    }
}
