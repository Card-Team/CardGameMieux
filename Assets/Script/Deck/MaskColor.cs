using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskColor : MonoBehaviour
{
    
    void Start()
    {
        GetComponent<SpriteRenderer>().color = Camera.main.backgroundColor;     //change la couleur du composant spriterender avec celle de la camera
    }
}
