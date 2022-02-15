using System.Collections;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using Script;
using TMPro;
using UnityEngine;

public class CardRenderer : MonoBehaviour
{
    public Card card;
    public GameObject nom;
    public GameObject description;
    public GameObject niveauText;
    public GameObject cout;
    public SpriteRenderer illustration;

    public bool retournee;

    [SerializeField] private CardImageDatabase imagesCartes;
    [SerializeField] private Sprite dos;

    // Start is called before the first frame update
    void Start()
    {
        nom.GetComponent<TextMeshPro>().text = card.Name.Value;
        description.GetComponent<TextMeshPro>().text = card.Description.Value;
        niveauText.GetComponent<TextMeshPro>().text = card.CurrentLevel.Value + "/" + card.MaxLevel;
        cout.GetComponent<TextMeshPro>().text = card.Cost.Value + "";

        illustration.sprite = retournee ? dos : imagesCartes[card.ImageId.Value];
    }

    // Update is called once per frame
    void Update()
    {
    }
}