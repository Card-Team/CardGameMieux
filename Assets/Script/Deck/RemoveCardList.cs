using UnityEngine;

public class RemoveCardList : MonoBehaviour
{
    public string NomCard;

    public void DeleteCardDeckList()
    {
        //clique sur une carte dans la liste : le supprimer de la liste
        //Debug.Log("carte a supprimer : "+NomCard);
        FindObjectOfType<LectureCartes>().listeCarteSelectionner.Remove(NomCard);
        var nbCarte = FindObjectOfType<LectureCartes>().listeCarteSelectionner.Count;
        FindObjectOfType<LectureCartes>().nbCartes.SetText(nbCarte + "");
        FindObjectOfType<LectureCartes>().nbCartes.color = new Color(255, 255, 255);
        var cardRenderer = FindObjectOfType<LectureCartes>().ListeCartes[NomCard];
        cardRenderer.SetTransparence(1);
        Destroy(gameObject);
    }
}