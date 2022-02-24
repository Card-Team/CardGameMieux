using System;
using CardGameEngine.Cards;
using TMPro;
using UnityEngine;

namespace Script
{
    public class CardRenderer : MonoBehaviour
    {
        public Card Card;
        public TextMeshPro nom;
        public TextMeshPro description;
        public SpriteRenderer niveau;
        public SpriteRenderer cout;
        public SpriteRenderer illustration;
        public SpriteRenderer fond;

        public bool faceCachee;

        [SerializeField] private CardImageDatabase imagesCartes;

        public float Width => fond.bounds.size.x;

        // Start is called before the first frame update
        void Start()
        {
            nom.text = Card.Name.Value;
            description.text = Card.Description.Value;
            niveau.GetComponentInChildren<TextMeshPro>().text = Card.CurrentLevel.Value + "/" + Card.MaxLevel;
            cout.GetComponentInChildren<TextMeshPro>().text = Card.Cost.Value.ToString();
            illustration.sprite = imagesCartes[Card.ImageId.Value];

            Flip();
        }

        public void Flip()
        {
            faceCachee = !faceCachee;
            nom.gameObject.SetActive(!faceCachee);
            description.gameObject.SetActive(!faceCachee);
            niveau.gameObject.SetActive(!faceCachee);
            cout.gameObject.SetActive(!faceCachee);
            illustration.gameObject.SetActive(!faceCachee);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}