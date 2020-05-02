using UnityEngine;

[HelpURL("http://github.com/mmajko/openhvr")]
[AddComponentMenu("OpenHVR/Effect Source")]
public class OpenHVREffect : OpenHVRBehaviour {
    [Header("Effect properties")]
    [Range(0,60)]
    public int duration;
    public OpenHVRManager.EffectType effectType;
    [Range(0,10)]
    public float range = 1.0f;
    public bool directional = false;

    [Header("Effect behaviour")]
    public bool playOnAwake = false;

    private OpenHVRManager.EffectRequest request = null;
    private bool isPlaying = false;

    protected override void Start() {
        base.Start();
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
        if (directional) {
            var lineEnd = transform.forward * 3.0f;
            Vector3 right = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0,225,0) * new Vector3(0,0,1);
            Vector3 left = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0,135,0) * new Vector3(0,0,1);
            Gizmos.DrawRay(transform.position, lineEnd);
            Gizmos.DrawRay(transform.position + lineEnd, right * 0.5f);
            Gizmos.DrawRay(transform.position + lineEnd, left * 0.5f);
        }
    }

    public void Play() {
        if (!isPlaying) {
            isPlaying = true;
            Invoke("Stop", duration);
            Request();
        }
    }

    public void Cancel() {
        if (request != null) {
            StartCoroutine(manager.CancelEffectRequest(request));
            Stop();
        }
    }

    public void Stop() {
        CancelInvoke("Stop");
        isPlaying = false;
        request = null;
    }

    public bool IsPlaying() {
        return isPlaying;
    }

    private void Request() {
        if (manager.isReady) {
            request = new OpenHVRManager.EffectRequest();
            request.duration = duration;
            request.effectType = effectType;
            request.position = transform.position;
            request.direction = transform.forward;
            request.directional = directional;
            request.range = range;

            StartCoroutine(manager.PostEffectRequest(request));
        }
    }
}
