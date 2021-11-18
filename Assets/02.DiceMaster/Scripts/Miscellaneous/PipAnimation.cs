using UnityEngine;
using System.Collections;

/// <summary>
/// A 3D pip that rotates on itself and changes scale
/// </summary>
public class PipAnimation : MonoBehaviour
{
    public void Trigger()
    {
        StartCoroutine(AnimateCO());
    }

    IEnumerator AnimateCO()
    {
        float t = 0;
        var startingLocalScale = transform.localScale;
        while (true)
        {
            t += Time.deltaTime;
            var pingPong = Mathf.PingPong(t, 1);
            transform.localScale = startingLocalScale * (1 + pingPong);
            transform.Rotate(Vector3.forward, Time.deltaTime * 360);
            yield return null;
        }
    }
}
