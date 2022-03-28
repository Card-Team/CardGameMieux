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

        private readonly Dictionary<int, Sprite> _dict = new Dictionary<int, Sprite>();

        public Sprite this[int imageIdValue]
        {
            get
            {
                if (_dict.ContainsKey(imageIdValue))
                    return _dict[imageIdValue];
                return _dict.First().Value;
            }
        }

        private void OnEnable()
        {
            foreach (var image in images) _dict[image.id] = image.sprite;
        }

        [Serializable]
        public class ImageInstance
        {
            public int id;
            public Sprite sprite;
        }
    }
}