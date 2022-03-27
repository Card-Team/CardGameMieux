using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Script
{
    public class AffichageNiveau : MonoBehaviour
    {
        // Start is called before the first frame update

        public int niveauActuel;
        public int niveauMax;

        public Sprite vide;
        public Sprite plein;
        public Sprite full;
        private float transparence=1;
        private List<SpriteRenderer> ListSpriteRender = new List<SpriteRenderer>();
        

        public SpriteRenderer ampoule;

        void Start()
        {
            RefreshCercle();
        }

        public void RefreshCercle()
        {
        
            for (var i = this.transform.childCount - 1; i >= 0; i--)
            {
                var objectA = this.transform.GetChild(i);
                ListSpriteRender.Remove(objectA.GetComponent<SpriteRenderer>());
                DestroyImmediate(objectA.gameObject);
            }

            float decalage = ampoule.bounds.size.x + 0.2f;
            for (int i = 0; i < niveauMax; i++)
            {
                SpriteRenderer cercle;
                cercle = Instantiate(ampoule, this.transform);
                var coutColor = cercle.color;
                coutColor.a = transparence;
                cercle.color = coutColor;
                ListSpriteRender.Add(cercle);

                if (niveauActuel == niveauMax)
                {
                    cercle.sprite = full;
                } else if (i < niveauActuel)
                {
                    cercle.sprite = plein;
                }
                else
                {
                    cercle.sprite = vide;
                }

                cercle.transform.localPosition = new Vector3(decalage*i - (decalage * (niveauMax - 1))/2,0f);
            } 
        }

        public void fontTransparent(float pourcentage)
        {
            transparence = pourcentage;
            foreach (var list in ListSpriteRender)
            {
                var coutColor = list.color;
                coutColor.a = pourcentage;
                list.color = coutColor;
            }
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
