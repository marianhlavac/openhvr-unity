using System;
using UnityEngine;

public class OpenHVRBehaviour : MonoBehaviour {
    protected OpenHVRManager manager = null;

    protected virtual void Start() {
        if (manager == null) {
            var managers = FindObjectsOfType<OpenHVRManager>();
            if (managers.Length == 1) {
                manager = managers[0];
            } else {
                Debug.LogError("There must be single OpenHVRManager in the scene!");
            }
        }
    }

    protected void SubscribeOnReady(Action callback) {
        if (manager.isReady) {
            callback();
        } else {
            manager.onServerReady += callback;
        }
    }
}
