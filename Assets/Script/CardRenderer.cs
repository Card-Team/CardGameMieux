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

        public bool retournee = true;

        [SerializeField] private CardImageDatabase imagesCartes;

        // Start is called before the first frame update
        void Start()
        {
            nom.text = Card.Name.Value;
            description.text = Card.Description.Value;
            niveau.GetComponentInChildren<TextMeshPro>().text = Card.CurrentLevel.Value + "/" + Card.MaxLevel;
            cout.GetComponentInChildren<TextMeshPro>().text = Card.Cost.Value.ToString();
            illustration.sprite = imagesCartes[Card.ImageId.Value];

            if (retournee)
            {
                nom.gameObject.SetActive(false);
                description.gameObject.SetActive(false);
                niveau.gameObject.SetActive(false);
                cout.gameObject.SetActive(false);
                illustration.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}