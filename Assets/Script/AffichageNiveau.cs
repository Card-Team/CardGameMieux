using System.Collections.Generic;
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


        public SpriteRenderer ampoule;
        private readonly List<SpriteRenderer> ListSpriteRender = new List<SpriteRenderer>();
        private float transparence = 1;

        private void Start()
        {
            RefreshCercle();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void RefreshCercle()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var objectA = transform.GetChild(i);
                ListSpriteRender.Remove(objectA.GetComponent<SpriteRenderer>());
                DestroyImmediate(objectA.gameObject);
            }

            var decalage = ampoule.bounds.size.x + 0.2f;
            for (var i = 0; i < niveauMax; i++)
            {
                SpriteRenderer cercle;
                cercle = Instantiate(ampoule, transform);
                var coutColor = cercle.color;
                coutColor.a = transparence;
                cercle.color = coutColor;
                ListSpriteRender.Add(cercle);

                if (niveauActuel == niveauMax)
                    cercle.sprite = full;
                else if (i < niveauActuel)
                    cercle.sprite = plein;
                else
                    cercle.sprite = vide;

                cercle.transform.localPosition = new Vector3(decalage * i - decalage * (niveauMax - 1) / 2, 0f);
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
    }
}