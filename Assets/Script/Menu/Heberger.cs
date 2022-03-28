using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using Script.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Heberger : MonoBehaviour
{
    public const string PortH = "PortHebergement";
    public TMP_InputField port;
    public GameObject panelHeberger;
    public TMP_Text compteur;
    public TMP_Text textErreur;
    public Loading loading;
    public Image erreurConnexion;

    private void Start()
    {
        if (PlayerPrefs.HasKey(PortH) && !PortH.Equals("."))
            //mettre le port depuis le PlayerPrefs enregistré
            port.text = PlayerPrefs.GetString("PortHebergement");
        else
            Debug.Log("Port jamais rentrer ou Vide");
    }

    private IEnumerator OnCoroutine(int i)
    {
        while (i >= 0)
        {
            //compteur de 1 seconde
            yield return new WaitForSeconds(1f);
            compteur.SetText("" + i--);
            if (i < 10) compteur.color = Color.red;

            if (i < 0)
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
        var portText = port.text;

        var deck = PlayerPrefs.GetString(NomButton.Nomdeck);
        var deckFile = Directory.EnumerateFiles(Application.persistentDataPath, "*.txt")
            .First(f => f.EndsWith(deck));
        var cartes = File.ReadLines(deckFile);

        var gameSettingsContainer = new GameObject("ParametresContainer")
            .AddComponent<GameSettingsContainer>();

        gameSettingsContainer.IPAddress = IPAddress.Any;
        gameSettingsContainer.port = int.Parse(portText.Trim());
        gameSettingsContainer.playerDeck = cartes;
        gameSettingsContainer.NetworkMode = NetworkMode.Server;
        PlayerPrefs.SetString(PortH, portText); //playerPrefs port
        SceneManager.LoadScene("Scenes/Partie");
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
        // StopAllCoroutines();
        // panelHeberger.SetActive(false);
        SceneManager.LoadScene("Scenes/Menu Principal");
    }
}