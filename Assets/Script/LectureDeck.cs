using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LectureDeck : MonoBehaviour
{
    public NomButton buttonTemplate; //Game object avec forcement un composant Nombutton
    public Canvas nouvelfenetre;
    void OnEnable()
    {
        LireFichier();
    }

    public void LireFichier()
    {
        Directory.CreateDirectory(Application.persistentDataPath);
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = di.GetFiles("*.txt");
        //supprimer les buttons
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //affiche tout les fichier texte present : 
        //Debug.Log(string.Join(" ; ",files.Select(f=>f.Name)));
        //ajouter les boutons 
        foreach (var file in files)
        {
            NomButton button = Instantiate(buttonTemplate, this.transform, false); //creer et un copie un bouton
            button.text = file.Name; //donne aux champs texte le nom du bouton
            if (button.text == PlayerPrefs.GetString("NomDeck"))
            {
                button.GetComponent<UnityEngine.UI.Image>().color =  new Color(160f/255f, 160f/255f, 160f/255f);
            }
            button.nouvelfenetre = nouvelfenetre;
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(file.Name);
        }
    }
}