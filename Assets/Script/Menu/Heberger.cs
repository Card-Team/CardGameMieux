using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Heberger : MonoBehaviour
{
    public TMP_InputField port;
    public const string PortH="PortHebergement"; 
    public GameObject panelHeberger;
    public TMP_Text compteur;
    public TMP_Text textErreur;
    public Loading loading;
    public Image erreurConnexion;
    void Start()
    {
        if (PlayerPrefs.HasKey(PortH) && !PortH.Equals("."))
        {
            //mettre le port depuis le PlayerPrefs enregistré
            port.text=(PlayerPrefs.GetString("PortHebergement"));
        }
        else
        {
            Debug.Log("Port jamais rentrer ou Vide");
        }
    }
    IEnumerator OnCoroutine(int i)
    {
        while (i>=0)
        {
            //compteur de 1 seconde
            yield return new WaitForSeconds(1f);
            compteur.SetText(""+i--);
            if (i < 10)
            {
                compteur.color = Color.red;
            }
            if (i<0)
            {
                compteur.SetText("");
                textErreur.SetText("Connexion Impossible");
                loading.Avancer = false;
                yield return new WaitForSeconds(0.5f);
                loading.gameObject.SetActive(false);
                erreurConnexion.gameObject.SetActive(true);
            }
        }
    }
    public void Herberger()
    {
        PlayerPrefs.SetString(PortH,port.text);                //playerPrefs port
    }

    public void AppuieHerberger()
    {
        compteur.SetText("");
        textErreur.SetText("");
        panelHeberger.SetActive(true);
        loading.Avancer = true;
        erreurConnexion.gameObject.SetActive(false);
        loading.gameObject.SetActive(true);
        StartCoroutine(OnCoroutine(15));
    }
    
    public void RetourHerberger()
    {
        StopAllCoroutines();
        panelHeberger.SetActive(false);
    }
}

