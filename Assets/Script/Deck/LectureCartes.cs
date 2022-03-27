using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Script;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LectureCartes : MonoBehaviour
{
    public Transform GameObjectCartes;
    TextMeshPro text;
    public CardRenderer cardTemplate;
    List<CardRenderer> cartes = new List<CardRenderer>();
    private float tailleListe;
    public ContactFilter2D _contactFilter2D;
    public GameObject ButtonDeck;
    public Transform ParentDeck;
    public TMP_Text nbCartes;
    public TMP_Text titre;
    public Image bar;
    public TMP_Text charg;
    public Dictionary<String, CardRenderer> ListeCartes = new Dictionary<string, CardRenderer>();
    [NonSerialized] public String DeckAModifier=null;
    public TMP_InputField NomDeck;
    
    public void ChargerDeck()
    {
        if (DeckAModifier == null)
        {
            return;
        }
        foreach (var cardCharger in File.ReadAllLines(Application.persistentDataPath + @"\"+DeckAModifier+".txt"))
        {
            AjouterCarte(ListeCartes[cardCharger]);
        }
        NomDeck.text = DeckAModifier;
    }

    public IEnumerator NomCartes()
    {
        //recuperer tout les noms de cartes du jeu qui finnissent en .lua sauf ceux qui commencent par un '_'
        Directory.CreateDirectory(Application.streamingAssetsPath);
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/EffectsScripts/Card");
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/EffectsScripts/Card", "*.lua",
                SearchOption.AllDirectories)
            .Where(s =>
            {
                String fileName = Path.GetFileName(s);
                return !(fileName.StartsWith("_")) && fileName.EndsWith(".lua") && !(fileName.Contains("example"));
            }).ToList();
        int tour = 0;
        float posY = 0;
        float posX = -0.5f;
        foreach (var file in files)
        {
            //faire un tri pour le nom des fichier avec le Path en affaiblissement
            String fileSansPath = file.Replace(Application.streamingAssetsPath + "/EffectsScripts/Card", "");
            String fileSansPath1 = fileSansPath.Replace(@"\", "");
            String fileSansPath2 = fileSansPath1.Replace(".lua", "");
            //Debug.Log(fileSansPath2);
            //carte
            var cardRenderer =
                Instantiate(cardTemplate, GameObjectCartes); //Creer un nouveau cardRenderer avec instantiate
            cartes.Add(cardRenderer);
            ListeCartes[fileSansPath2] = cardRenderer;
            cardRenderer.SetScript(fileSansPath2); //champs pour afficher ce script la
            cardRenderer.transform.localPosition = new Vector3(posX, posY, 0);
            tour += 1;
            if (tour % 4 == 0)
            {
                posX = -0.5f;
                tailleListe += cardRenderer.Height * 1 / GameObjectCartes.transform.localScale.y + 1.5f;
                posY -= cardRenderer.Height * 1 / GameObjectCartes.transform.localScale.y +
                        1.5f; //Corriger pour l'échelle du Game Object Parent en y
            }
            else
            {
                posX += cardRenderer.Width * 1 / GameObjectCartes.transform.localScale.x +
                        1.2f; //Corriger pour l'échelle du Game Object Parent en X
            }
            titre.SetText("Chargement en cours ...");
            float progress = Mathf.Clamp01((float) tour / files.Count);
            bar.fillAmount = progress;
            charg.SetText("<i> Loading "+Mathf.RoundToInt(progress * 100) + " %</i>");
            yield return new WaitForEndOfFrame();
        }
        titre.gameObject.SetActive(false);
        bar.gameObject.SetActive(false);
        charg.gameObject.SetActive(false);
        ChargerDeck();
    }

    private CardRenderer selectionCarte;
    [NonSerialized] public List<string> listeCarteSelectionner = new List<string>();
    public Color NbCartesColor = new Color(0, 204, 102);

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Connaitre la position de la souris par rapport a la camera
        //Debug.Log("x : " +mousePos.x + " et y: "+mousePos.y);
        if (mousePos.x > -9 && mousePos.y < 7.8 && mousePos.y > -9.5)
        {
            //changer la postion du GameobjectCartes : faire desendre les cartes
            Vector3 pos = GameObjectCartes.transform.position;
            float scrool = Input.GetAxis("Mouse ScrollWheel");
            //Debug.Log("Molette Bas");

            pos.y -= scrool * Time.deltaTime * 2000;
            if (pos.y < 0 || pos.y > tailleListe - 1)
            {
                return;
            }

            GameObjectCartes.transform.position = pos;
        }

        //recupere les objets ou on est dessus avec la souris dans une liste
        List<Collider2D> proche = new List<Collider2D>();
        //nombre d'element sous la souris
        var count = Physics2D.OverlapPoint(mousePos, _contactFilter2D, proche);
        //si le nombre d'element est plus grand que 0
        if (count > 0)
        {
            //boite de colision de la carte en dessous
            var first = proche.First().GetComponentInParent<CardRenderer>();
            //verification que la carte n'a pas ete pris plus de 2 fois sinon ne pas la mettre dans la liste
            if (listeCarteSelectionner.Count(list => first.scriptToDisplay == list) == 2)
            {
                return;
            }

            //Debug.Log(first.name);
            if (first != selectionCarte && selectionCarte != null)
            {
                selectionCarte.Hover = false;
            }

            if (mousePos.x > -9 && mousePos.y < 7.8 && mousePos.y > -9.5)
            {
                first.Hover = true;
                selectionCarte = first;
            }

            //clique souris dans le rectangle des cartes du deck et liste inferieur a 12 alors ajout dans la liste  
            if (Input.GetMouseButtonUp(0) && listeCarteSelectionner.Count < 12 && mousePos.x > -9 && mousePos.y < 7.8 &&
                mousePos.y > -9.5)
            {
                AjouterCarte(first);
            }
        }
        else
        {
            if (selectionCarte != null)
            {
                selectionCarte.Hover = false;
                selectionCarte = null;
            }
        }
    }

    private void AjouterCarte(CardRenderer first)
    {
        GameObject b = Instantiate(ButtonDeck, ParentDeck, false);
        b.GetComponentInChildren<TextMeshProUGUI>().text = first.Card.Name.Value;
        b.name = first.Card.Name.Value;
        b.GetComponent<RemoveCardList>().NomCard = first.scriptToDisplay;
        listeCarteSelectionner.Add(first.scriptToDisplay);
        if (listeCarteSelectionner.Count == 12)
        {
            nbCartes.color= NbCartesColor;
        }
        nbCartes.SetText(listeCarteSelectionner.Count.ToString());
        //Debug.Log(listeCarteSelectionner[listeCarteSelectionner.Count-1]);
        
        //si les 2 cartes on ete selectionner
        if (listeCarteSelectionner.Count(c => c == first.scriptToDisplay) == 2)
        {
            first.SetTransparence(0.5f);
        }
    }

    public void RetourMenu(GameObject kill)
    {
        SceneManager.LoadScene("Deck");
        //SceneManager.LoadScene(kill+"");
    }
}