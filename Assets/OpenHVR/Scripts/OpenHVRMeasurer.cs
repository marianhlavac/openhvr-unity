using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[HelpURL("http://github.com/mmajko/openhvr")]
public class OpenHVRMeasurer : OpenHVRBehaviour {
    [Header("Input Configuration")]
    public Transform trackedUsing;
    public string switchDevicesAxisInput = "Horizontal";
    public string saveDeviceLocationInput = "Submit";

    private TextMesh selectionLabel;
    private TextMesh positionLabel;
    private Transform faceLabelsTo;
    private OpenHVRManager.Device[] availableDevices = new OpenHVRManager.Device[0];
    private int selectedDevice = 0;
    private bool debounceDeviceSel = true;
    private const float selectAxisThreshold = 0.5f;
    private Color measuringColor = new Color(0.1f, 0.6f, 0.8f);
    private Color saveOkColor = new Color(0.2f, 0.8f, 0.2f);
    private Color errorColor = new Color(0.8f, 0.2f, 0.2f);

    protected override void Start() {
        base.Start();
        SubscribeOnReady(LoadDevices);
        SubscribeOnReady(StartMeasuring);

        selectionLabel = transform.Find("Selection Label").GetComponent<TextMesh>();
        positionLabel = transform.Find("Position Label").GetComponent<TextMesh>();
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        faceLabelsTo = camera.transform;
    }

    void Update() {
        if (Input.GetButtonDown(saveDeviceLocationInput)) {
            SaveDevice();
        }

        var selAxis = Input.GetAxisRaw(switchDevicesAxisInput);
        if (selAxis > selectAxisThreshold) {
            if (debounceDeviceSel) SwitchDevice(+1);
            debounceDeviceSel = false;
        } else if (selAxis < -selectAxisThreshold) {
            if (debounceDeviceSel) SwitchDevice(-1);
            debounceDeviceSel = false;
        } else {
            debounceDeviceSel = true;
        }

        UpdateLabels(true);
    }

    void UpdateLabels(bool onlyPosition = false) {
        if (availableDevices.Length > 0) {
            if (!onlyPosition) {
                selectionLabel.text = availableDevices[selectedDevice].Name;
            }
        }
        positionLabel.text = trackedUsing.position.ToString();
        positionLabel.transform.LookAt(faceLabelsTo);
        selectionLabel.transform.LookAt(faceLabelsTo);
    }

    void ChangeLabelColor(Color color) {
        selectionLabel.color = color;
        positionLabel.color = color;
    }

    void LoadDevices() {
        manager.GetAllDevices(devices => {
            availableDevices = devices;
            UpdateLabels();
        });
    }

    void StartMeasuring() {
        ChangeLabelColor(measuringColor);
    }

    void SwitchDevice(int moveBy) {
        selectedDevice += moveBy;
        if (selectedDevice < 0) { selectedDevice = availableDevices.Length - 1; }
        if (selectedDevice >= availableDevices.Length) { selectedDevice = 0; }
        ChangeLabelColor(measuringColor);
        UpdateLabels();
    }

    void SaveDevice() {
        var device = availableDevices[selectedDevice];
        device.Location = trackedUsing.position;
        device.DirectionX = trackedUsing.forward.x;
        device.DirectionY = trackedUsing.forward.y;
        device.DirectionZ = trackedUsing.forward.z;
        manager.UpdateDevice(device, result => {
            ChangeLabelColor(result ? saveOkColor : errorColor);
            if (result) {
                TriggerDevicesDisplaysUpdate();
            }
        });
    }

    void TriggerDevicesDisplaysUpdate() {
        foreach (var devicesDisplay in FindObjectsOfType<OpenHVRDevices>()) {
            devicesDisplay.LoadDevices();
        }
    }
}
