using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[HelpURL("http://github.com/mmajko/openhvr")]
[AddComponentMenu("OpenHVR/OpenHVR Manager")]
public class OpenHVRManager : MonoBehaviour
{
    [Header("OpenHVR Server Connection")]
    public ConnectionType connectionType;
    public string connectionURI = "http://127.0.0.1:47023/v1";

    [Header("Configuration")]
    [Tooltip("Outputs debug messages to the console.")]
    public bool debugMode = false;

    public bool isReady { get; private set; }
    public Action onServerReady;

    public enum ConnectionType { HTTP, MQTT }
    public enum EffectType { None, Relay, Wind, Heat }

    [Serializable]
    public class EffectRequest {
        public EffectType effectType;
        public int duration;
        public Vector3 position;
        public Vector3 direction;
        public bool directional = false;
        public float range;
        public int? id = null;

        public string SerializeJSON() {
            return JsonUtility.ToJson(this, true);
        }
    }

    [Serializable]
    public class Device {
        public int Id;
        public string Name;
        public string Type;
        public int EffectType;
        public float LocationX;
        public float LocationY;
        public float LocationZ;
        public Vector3 Location {
            get { return new Vector3(LocationX, LocationY, LocationZ); }
            set { LocationX = value.x; LocationY = value.y; LocationZ = value.z; }
        }
        public float DirectionX;
        public float DirectionY;
        public float DirectionZ;
        public Quaternion Rotation {
            get {
                return Quaternion.LookRotation(
                    new Vector3(DirectionX, DirectionY, DirectionZ)
                );
            }
        }
        public float DirectionSpread;
        public string ConnectorUri;
        public string ConnectorParam;
    }

    [Serializable]
    public class DeviceList {
        public Device[] d;
    }

    void Start() {
        if (connectionType == ConnectionType.MQTT) {
            Debug.LogError("MQTT is not implemented yet.");
        }

        GetStatus(status => {
            if (status) {
                onServerReady();
                isReady = true;
                if (debugMode) {
                    Debug.Log("OpenHVR server at " + connectionURI + " is ready.");
                }
            }
        });
    }

    public void GetStatus(Action<bool> result) {
        if (debugMode) {
            Debug.Log("Testing connection to OpenHVR server...");
        }
        var req = StartCoroutine(Get("/system/status", res => {
            var ok = res.responseCode == 200;
            if (!ok) {
                Debug.LogError("OpenHVR server at " + connectionURI + " is unreachable!");
            }
            result(ok);
        }));
    }

    public void GetAllDevices(Action<Device[]> resultDevices) {
        var req = StartCoroutine(Get("/devices/", result => {
            if (result.responseCode == 200) {
                var contents = "{\"d\":" + result.downloadHandler.text + "}";
                resultDevices(JsonUtility.FromJson<DeviceList>(contents).d);
            } else {
                Debug.LogError("Failed to read OpenHVR devices from server.");
            }
        }));
    }

    public void UpdateDevice(Device device, Action<bool> callback) {
        string payload = JsonUtility.ToJson(device);
        if (debugMode) {
            Debug.Log("Updating device " + device.Id.ToString() + ":");
            Debug.Log(payload);
        }
        var url = "/devices/" + device.Id.ToString();
        var req = StartCoroutine(Put(url, payload, result => {
            callback(result.responseCode >= 200 && result.responseCode < 300);
        }));
    }

    public IEnumerator Get(string resource, System.Action<UnityWebRequest> result, string method = "GET") {
        string url = connectionURI + resource;
        var request = new UnityWebRequest(url, method);

        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        result(request);

        if (debugMode) {
            Debug.Log("GET " + url + " returned " + request.responseCode);
        }
    }

    public IEnumerator Post(string resource, string payload,
            System.Action<UnityWebRequest> result, string method = "POST") {
        string url = connectionURI + resource;
        var request = new UnityWebRequest(url, method);
        request.SetRequestHeader("Content-Type", "application/json");

        byte[] bodyBytes = Encoding.UTF8.GetBytes(payload);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyBytes);

        yield return request.SendWebRequest();
        result(request);

        if (debugMode) {
            Debug.Log("POSTed data:\n" + payload);
            Debug.Log("POST " + url + " returned " + request.responseCode);
        }
    }

    public IEnumerator Put(string resource, string payload, System.Action<UnityWebRequest> result) {
        return Post(resource, payload, result, "PUT");
    }

    public IEnumerator PostEffectRequest(EffectRequest request) {
        return Post("/effects/", request.SerializeJSON(), result => {
            if (result.responseCode != 200) {
                Debug.LogError("Failed to request effect performance");
            }
        });
    }

    public IEnumerator CancelEffectRequest(EffectRequest request) {
        return Get("/effects/", result => {
            if (result.responseCode != 200) {
                Debug.LogError("Failed to cancel effect performance");
            }
        }, "DELETE");
    }
}
