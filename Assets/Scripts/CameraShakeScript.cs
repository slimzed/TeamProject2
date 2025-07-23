using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
    public bool start = false;

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private AnimationCurve shakeIntensityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = shakeIntensityCurve.Evaluate(elapsed / shakeDuration);
            transform.position = originalPosition + Random.insideUnitSphere * t;
            yield return null; // stops the script from returning until the elapsed time is over 
        }
        transform.position = originalPosition;
    }
}
