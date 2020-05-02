using UnityEngine;

[HelpURL("http://github.com/mmajko/openhvr")]
[AddComponentMenu("OpenHVR/Devices Enumerator")]
public class OpenHVRDevices : OpenHVRBehaviour {
    public GameObject devicePrefab;

    public class DeviceProperties : MonoBehaviour {
        public int id;
        public OpenHVRManager.EffectType effectType;
        public string type;
        public string connectionURI;
    }

    protected override void Start() {
        base.Start();
        SubscribeOnReady(LoadDevices);
    }

    public void LoadDevices() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        manager.GetAllDevices(devices => {
            Debug.Log("Loaded " + devices.Length.ToString() + " devices.");
            foreach (var device in devices) {
                var node = Instantiate(devicePrefab, device.Location, device.Rotation, transform);
                node.name = device.Name + " (ID" + device.Id + ")";

                node.AddComponent<DeviceProperties>();
                var properties = node.GetComponent<DeviceProperties>();
                properties.id = device.Id;
                properties.effectType = (OpenHVRManager.EffectType)device.EffectType;
                properties.type = device.Type;
                properties.connectionURI = device.ConnectorUri;
            }
        });
    }
}
