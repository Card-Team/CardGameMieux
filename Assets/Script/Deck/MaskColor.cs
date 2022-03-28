using UnityEngine;

public class MaskColor : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().color =
            Camera.main.backgroundColor; //change la couleur du composant spriterender avec celle de la camera
    }
}