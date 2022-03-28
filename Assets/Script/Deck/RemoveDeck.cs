using TMPro;
using UnityEngine;

public class RemoveDeck : MonoBehaviour
{
    public GameObject InterfaceSuppresionDeck;
    public TMP_Text Text;
    public string CardAppuye;

    public void RemoveFichierDeckList()
    {
        InterfaceSuppresionDeck.gameObject.SetActive(true);
        Text.SetText("Etes vous sur de vouloir supprimer le deck  <i>" + CardAppuye + "</i> ?");
        InterfaceSuppresionDeck.GetComponent<OuiNonInterfaceSuppression>().CarteASupprimer = CardAppuye;
    }

    public void ModifierDeck()
    {
        FindObjectOfType<LectureCartes>().DeckAModifier = CardAppuye;
        FindObjectOfType<MenuDeck>().CreerUnDeck();
    }
}