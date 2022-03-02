using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CardGameEngine.Cards;
using Script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LectureCartes : MonoBehaviour
{
    public Transform GameObjectCartes;
    public TextMeshPro textTemplate;
    TextMeshPro text;
    public CardRenderer cardTemplate;
    List<CardRenderer> cartes = new List<CardRenderer>();
    private float tailleListe;
    public void Start()
    {
        NomCartes();
    }

    public void NomCartes()
    {
        //recuperer tout les noms de cartes du jeu qui finnissent en .lua sauf ceux qui commencent par un '_'
        Directory.CreateDirectory(Application.streamingAssetsPath);
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/EffectsScripts/Card");
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/EffectsScripts/Card", "*.lua",
                SearchOption.AllDirectories)
            .Where(s => !(s.Contains("_")) && s.EndsWith(".lua")).ToList();

        int tour = 0;
        float posY = 0;
        float posX = 0;
        foreach (var file in files)
        {
            //faire un tri pour le nom des fichier avec le Path en affaiblissement
            String fileSansPath = file.Replace(Application.streamingAssetsPath + "/EffectsScripts/Card", "");
            String fileSansPath1 = fileSansPath.Replace(@"\", "");
            String fileSansPath2 = fileSansPath1.Replace(".lua", "");
            Debug.Log(fileSansPath2);
            // Text
            /*
            text = Instantiate(textTemplate, null);
            text.text = fileSansPath2; // NE MARCHE PAS POUR RENOMé le composant text
            text.SetText(fileSansPath2);
            text.fontSize = 5;
            text.transform.position = new Vector3(posX, posY, 0);
            */
            //carte
            var cardRenderer = Instantiate(cardTemplate, GameObjectCartes); //Creer un nouveau cardRenderer avec instantiate
            cartes.Add(cardRenderer);
            cardRenderer.scriptToDisplay = fileSansPath2; //champs pour afficher ce script la
            cardRenderer.transform.localPosition = new Vector3(posX, posY, 0);
            tour += 1;
            if (tour % 5 == 0)
            {
                posX = 0;
                tailleListe += cardRenderer.Height * 1 / GameObjectCartes.transform.localScale.y + 1.5f ;
                posY -= cardRenderer.Height * 1 / GameObjectCartes.transform.localScale.y + 1.5f; //Corriger pour l'échelle du Game Object Parent en y
            }
            else
            {
                posX += cardRenderer.Width * 1 / GameObjectCartes.transform.localScale.x + 1.2f; //Corriger pour l'échelle du Game Object Parent en X
            }
        }
    }

    void Update()
    {//changer la postion du GameobjectCartes
        Vector3 pos = GameObjectCartes.transform.position;
        float scrool = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log("Molette Bas");
        
        pos.y -= scrool * Time.deltaTime * 2000;
        if (pos.y < 0 || pos.y > tailleListe-10 ) {return;}
        GameObjectCartes.transform.position = pos;
    }
}