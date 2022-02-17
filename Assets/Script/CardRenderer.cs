using System.Collections;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using Script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardRenderer : MonoBehaviour
{
    public Card card;
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
        nom.text = card.Name.Value;
        description.text = card.Description.Value;
        niveau.GetComponentInChildren<TextMeshPro>().text = card.CurrentLevel.Value + "/" + card.MaxLevel;
        cout.GetComponentInChildren<TextMeshPro>().text = card.Cost.Value.ToString();
        illustration.sprite = imagesCartes[card.ImageId.Value];

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