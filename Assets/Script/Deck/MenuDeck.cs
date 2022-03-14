using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MenuDeck : MonoBehaviour
{
    public GameObject CreationDeck;
    public Button CreerDeck;
    public TMP_Text ModifierDeck;
    public GameObject List;
    public Button buttonTemplate;
    public Transform ParentDeck;

    void Start()
    {
        //lire tout les fichier deck 
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = di.GetFiles("*.txt");
        Debug.Log(files);
        //si aucun fichier
        if (files.Length == 0)
        {
            CreerDeck.transform.position = new Vector3(-193f,-42.627f,0f);
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
        var actuel = GameObject.FindObjectOfType<Canvas>(); //recupere la fenetre active
        actuel.gameObject.SetActive(false);
        CreationDeck.gameObject.SetActive(true);
    }
}
