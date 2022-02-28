using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffichageNiveau : MonoBehaviour
{
    // Start is called before the first frame update

    public int niveauActuel;
    public int niveauMax; 
    public SpriteRenderer cercleVide;
    public SpriteRenderer cerclePlein;

    void Start()
    {
        RefreshCercle();
    }

    public void RefreshCercle()
    {
        
        for (var i = this.transform.childCount - 1; i >= 0; i--)
        {
            var objectA = this.transform.GetChild(i);
            objectA.transform.parent = null;
        }

        for (int i = 0; i < niveauMax; i++)
        {
            SpriteRenderer cercle;
            if (i < niveauActuel)
            {
                cercle = Instantiate(cerclePlein, this.transform);
            }
            else
            {
                cercle = Instantiate(cercleVide, this.transform);

            }

            cercle.sortingOrder = 1;
            cercle.transform.position = new Vector3(this.transform.position.x + cercle.bounds.size.x/2 - (cercle.bounds.size.x * niveauMax)/2 + cercle.bounds.size.x*i, 
                this.transform.position.y);
        } 
    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
