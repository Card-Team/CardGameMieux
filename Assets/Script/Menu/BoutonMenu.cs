using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutonMenu : MonoBehaviour
{
    private void Start()
    {
    }

    public void OuvrirCanvas(Canvas nouvelfenetre)
    {
        var actuel = FindObjectOfType<Canvas>(); //recupere la fenetre active
        actuel.gameObject.SetActive(false);
        nouvelfenetre.gameObject.SetActive(true);
    }

    public void AllezDeck()
    {
        SceneManager.LoadScene("Deck");
    }

    public void Exit()
    {
        Application.Quit();
    }
}