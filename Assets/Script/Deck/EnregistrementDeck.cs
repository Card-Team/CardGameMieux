using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

[Serializable]
public class EnregistrementDeck : MonoBehaviour
{
    public TMP_InputField nomDeck;
    private List<string> liste;
    public GameObject panelDeckExistant;
    public Button Ok;
    public Button Non;
    public GameObject pas12Cartes;
    public GameObject panelNomDeckVide;
    private String name;

    void Start()
    {
        liste = FindObjectOfType<LectureCartes>().listeCarteSelectionner = new List<string>();
    }

    IEnumerator OnCoroutine(GameObject panel)
    {
        yield return new WaitForSeconds(5);
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            panelDeckExistant.SetActive(false);
            panelNomDeckVide.SetActive(false);
            pas12Cartes.SetActive(false);
        }
    }

    public void Enregistrement()
    {
        //Debug.Log("Nom du deck : " + nomDeck.text + nomDeck.text.Length);
        if (String.IsNullOrWhiteSpace(nomDeck.text))
        {
            //Visible 5 sec
            //Debug.Log("Nom du deck : ");
            panelNomDeckVide.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(panelNomDeckVide));
        }
        else if (liste.Count == 12)
        {
            var nomDeckText = nomDeck.text + ".txt";
            name = nomDeckText;
            if (File.Exists(nomDeckText))
            {
                //interface de validation pour suppression du fichier deja existant
                panelDeckExistant.SetActive(true);
            }
            //creation de fichier
            File.Create(nomDeckText);
            // Ajouter du texte au fichier  
            File.WriteAllLines(nomDeckText, liste);
            //Debug.Log("fichier"+nomDeckText+ " creer");
        }
        else
        {
            pas12Cartes.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(pas12Cartes));
        }
    }

    public void CliqueOk()
    {
        File.Delete(name+".txt");
    }

    public void CliqueNon()
    {
        panelDeckExistant.gameObject.SetActive(false);
    }
}