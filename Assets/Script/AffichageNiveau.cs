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

        void Start()
        {
            RefreshCercle();
        }

        public void RefreshCercle()
        {
        
            for (var i = this.transform.childCount - 1; i >= 0; i--)
            {
                var objectA = this.transform.GetChild(i);
                DestroyImmediate(objectA.gameObject);
            }

            float decalage = ampoule.bounds.size.x + 0.2f;
            for (int i = 0; i < niveauMax; i++)
            {
                SpriteRenderer cercle;
                cercle = Instantiate(ampoule, this.transform);

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
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
