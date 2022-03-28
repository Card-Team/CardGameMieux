using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LectureDeck : MonoBehaviour
{
    public NomButton buttonTemplate; //Game object avec forcement un composant Nombutton
    public Canvas nouvelfenetre;
    public TMP_Text deckVide;
    public Button allezAuDeck;
    public ScrollRect ScrollRect;

    private void OnEnable()
    {
        LireFichier();
        ScrollRect.verticalNormalizedPosition = 1;
    }

    public void LireFichier()
    {
        Directory.CreateDirectory(Application.persistentDataPath);
        var dir = new DirectoryInfo(Application.persistentDataPath);
        var files = dir.GetFiles("*.txt");
        deckVide.gameObject.SetActive(false);
        allezAuDeck.gameObject.SetActive(false);
        if (files.Length == 0)
        {
            deckVide.gameObject.SetActive(true);
            allezAuDeck.gameObject.SetActive(true);
        }

        //supprimer les buttons
        foreach (Transform child in transform) Destroy(child.gameObject);
        //affiche tout les fichier texte present : 
        //Debug.Log(string.Join(" ; ",files.Select(f=>f.Name)));
        //ajouter les boutons 
        foreach (var file in files)
        {
            var button = Instantiate(buttonTemplate, transform, false); //creer et un copie un bouton
            var file2 = file.Name.Replace(".txt", "");
            button.name = file2; //donne aux champs texte le nom du bouton
            button.text = file.Name;
            //Debug.Log( button.name + " / "+ PlayerPrefs.GetString(NomButton.Nomdeck));
            if (button.text == PlayerPrefs.GetString(NomButton.Nomdeck))
            {
                button.transform.SetSiblingIndex(0); //le mettre a la premiere position
                button.GetComponent<Image>().color = new Color(160f / 255f, 160f / 255f, 160f / 255f);
            }

            button.nouvelfenetre = nouvelfenetre;
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(file2);
        }
    }
}