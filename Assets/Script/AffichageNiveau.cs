using UnityEngine;

namespace Script
{
    public class AffichageNiveau : MonoBehaviour
    {
        // Start is called before the first frame update

        public int niveauActuel;
        public int niveauMax;

        public Color couleureNormal;
        public Color couleureMax;

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
                DestroyImmediate(objectA.gameObject);
            }

            float decalage = cercleVide.bounds.size.x + 0.01f;
            for (int i = 0; i < niveauMax; i++)
            {
                SpriteRenderer cercle;
                cercle = Instantiate(i < niveauActuel ? cerclePlein : cercleVide, this.transform);
                
                if (i < niveauActuel)
                {
                    cercle.color = niveauActuel == niveauMax ? couleureMax : couleureNormal;
                }
                
                cercle.transform.localPosition = new Vector3(decalage*i - (decalage * (niveauMax - 1))/2,0f,-1f);
            } 
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
