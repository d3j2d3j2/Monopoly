// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using System.Collections;
using DiceMaster;

/// <summary>
/// This script randomly changes the color of the resulting dynamic face.
/// Also performs an animation.
/// </summary>
public class TestDynamicFaces : MonoBehaviour
{
    public Dice targetDice;
    public Sprite[] possibleSprites;

    void Start()
    {
        Debug.Assert(targetDice != null, "Make sure a target dice is set.");
        Debug.Assert(targetDice.dynamicFaceGos != null, "Make sure you are using a dice with dynamic faces.");
        targetDice.onShowNumber.AddListener(HandleShownFace);
    }

    public void HandleShownFace(int number)
    {
        var dynamicFace = targetDice.dynamicFaceGos[number - 1].GetComponent<DynamicFace>();
        if (dynamicFace != null)
        {
            dynamicFace.SetSprite(possibleSprites[Random.Range(0, possibleSprites.Length)]);

            int colorIndex = Random.Range(0, 3);
            switch (colorIndex)
            {
                case 0: dynamicFace.sr.color = Color.red; break;
                case 1: dynamicFace.sr.color = Color.green; break;
                case 2: dynamicFace.sr.color = Color.blue; break;
            }

            StartCoroutine(AnimateFaceCO(dynamicFace));
        }
    }

    IEnumerator AnimateFaceCO(DynamicFace dynamicFace)
    {
        float t = 0f;
        var startScale = dynamicFace.transform.localScale;
        float period = 0.25f;
        float maxScale = 0.2f;
        while (t < 1f)
        {
            t += Time.deltaTime / period;
            var pingpong = Mathf.PingPong(t, 0.5f);
            dynamicFace.transform.localScale = startScale + Vector3.one * pingpong * maxScale;
            yield return null;
        }
    }
}
