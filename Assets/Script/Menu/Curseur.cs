using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Curseur : MonoBehaviour
{
    public const string PPCurseur = "PlayerPrefsCurseur";
    public const string PPBordures = "PlayerPrefsBordures";
    public List<Texture2D> cursorTexture;
    public Image image;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public Toggle ToggleBordures;
    private int curent;


    public void Start()
    {
        curent = PlayerPrefs.GetInt(PPCurseur, 0);
        //Debug.Log("curseur numero : "+nbCursor);
        Cursor.SetCursor(cursorTexture[curent], hotSpot, cursorMode);
        image.sprite = Sprite.Create(cursorTexture[curent],
            new Rect(0, 0, cursorTexture[curent].width, cursorTexture[curent].height), Vector2.zero);

        var bordures = PlayerPrefs.GetInt(PPBordures, 0);
        //Debug.Log(bordures);
        if (bordures == 0)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            ToggleBordures.SetIsOnWithoutNotify(false);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            ToggleBordures.SetIsOnWithoutNotify(true);
        }
    }

    public void Suivant()
    {
        curent++;
        curent = curent % cursorTexture.Count;
        Cursor.SetCursor(cursorTexture[curent], hotSpot, cursorMode);
        image.sprite = Sprite.Create(cursorTexture[curent],
            new Rect(0, 0, cursorTexture[curent].width, cursorTexture[curent].height), Vector2.zero);
        PlayerPrefs.SetInt(PPCurseur, curent);
    }

    public void Precedant()
    {
        curent--;
        if (curent <= 0) curent = cursorTexture.Count - 1;
        Cursor.SetCursor(cursorTexture[curent], hotSpot, cursorMode);
        image.sprite = Sprite.Create(cursorTexture[curent],
            new Rect(0, 0, cursorTexture[curent].width, cursorTexture[curent].height), Vector2.zero);
        PlayerPrefs.SetInt(PPCurseur, curent);
    }
}