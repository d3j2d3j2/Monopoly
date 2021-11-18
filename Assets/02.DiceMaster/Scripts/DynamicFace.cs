// Copyright Michele Pirovano 2014-2016
using UnityEngine;
using System.Collections;

namespace DiceMaster
{
    /// <summary>
    /// A face that can be changed dynamically
    /// </summary>
    public class DynamicFace : MonoBehaviour
    {
        public SpriteRenderer sr;

        public void SetSprite(Sprite sprite)
        {
            sr.sprite = sprite;
        }
    }

}
