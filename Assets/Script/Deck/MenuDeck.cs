using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class MenuDeck : MonoBehaviour
{
    public GameObject CreationDeck;
    public Button CreerDeck;
    public TMP_Text ModifierDeck;
    public GameObject List;
    public Button buttonTemplate;
    public Transform ParentDeck;
    public GameObject FenetreActive;
    public GameObject PanelSurppressionDeck;
    public TMP_Text TextCard;

    void Start()
    {
        //lire tout les fichier deck 
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = di.GetFiles("*.txt");
        //si aucun fichier
        if (files.Length == 0)
        {
            CreerDeck.transform.localPosition = new Vector3(0f,0f,0f);
            CreerDeck.GetComponentInChildren<TMP_Text>().fontSize=35;
            List.gameObject.SetActive(false);
            ModifierDeck.gameObject.SetActive(false);
        }
        else
        {
            foreach (var file in files)
            {
                Button button = Instantiate(buttonTemplate, ParentDeck, false); //creer et un copie un bouton
                String file2 = file.Name.Replace(".txt", "");
                button.name = file2; //donne aux champs texte le nom du bouton

                var removeDeck = button.GetComponentInChildren<RemoveDeck>();
                removeDeck.InterfaceSuppresionDeck = PanelSurppressionDeck;
                removeDeck.Text = TextCard;
                removeDeck.CardAppuye = file2;
                
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TextMeshProUGUI>().SetText(file2);
            }
        }
    }

    public void Retour()
    {
        SceneManager.LoadScene("Menu Principal");
    }

    public void CreerUnDeck()
    {
        StartCoroutine(ActivDelayFrame());
    }

    public IEnumerator ActivDelayFrame()
    {
        FenetreActive.gameObject.SetActive(false);
        yield return FindObjectOfType<LectureCartes>().NomCartes();
        yield return new WaitForEndOfFrame();
        CreationDeck.gameObject.SetActive(true);
    }

    public void ModificationDeck()
    {
        
    }
}
