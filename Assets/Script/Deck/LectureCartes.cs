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
    public TextMeshProUGUI textTemplate;
    TextMeshPro text;
    public CardRenderer cardTemplate;
    List<CardRenderer> cartes = new List<CardRenderer>();
    private float tailleListe;
    public ContactFilter2D _contactFilter2D;

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
            //Debug.Log(fileSansPath2);
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

    private CardRenderer selectionCarte;
    void Update(){
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Connaitre la position de la souris par rapport a la camera
        Debug.Log("x : " +mousePos.x + "et y: "+mousePos.y);
        if (mousePos.x > -9 && mousePos.y < 5.2)
        {
            //changer la postion du GameobjectCartes : faire desendre les cartes
            Vector3 pos = GameObjectCartes.transform.position;
            float scrool = Input.GetAxis("Mouse ScrollWheel");
            //Debug.Log("Molette Bas");
        
            pos.y -= scrool * Time.deltaTime * 2000;
            if (pos.y < 0 || pos.y > tailleListe-1 ) {return;}
            GameObjectCartes.transform.position = pos;
        }
        
        //recupere les objets ou on est dessus dans une liste
        List<Collider2D> proche = new List<Collider2D>();
        //nombre d'element sous la souris
        var count = Physics2D.OverlapPoint(mousePos,_contactFilter2D,proche);
        //si le nombre d'element est plus grand que 0
        if (count > 0)
        {
            //boite de colision de la carte en dessous
            var first = proche.First().GetComponent<CardRenderer>();
            if (first != selectionCarte && selectionCarte!= null)
            {
                selectionCarte.transform.localScale = Vector3.one;
            }
            first.transform.localScale = new Vector3(1.2f,1.2f,1);
            selectionCarte = first;
        }
        else
        {
            if (selectionCarte != null)
            {
                selectionCarte.transform.localScale = Vector3.one;
            }
        }
    }
}