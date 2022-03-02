using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "CardImageDatabase", menuName = "ScriptableObjects")]
    public class CardImageDatabase : ScriptableObject
    {
        public List<ImageInstance> images = new List<ImageInstance>();

        private Dictionary<int, Sprite> _dict = new Dictionary<int, Sprite>();

        [Serializable]
        public class ImageInstance
        {
            public int id;
            public Sprite sprite;
        }

        private void OnEnable()
        {
            foreach (var image in images)
            {
                _dict[image.id] = image.sprite;
            }
        }

        public Sprite this[int imageIdValue]
        {
            get
            {
                if (_dict.ContainsKey(imageIdValue))
                    return _dict[imageIdValue];
                else
                    return _dict.First().Value;
            }
        }
    }
}