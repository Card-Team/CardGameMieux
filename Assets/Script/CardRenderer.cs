using System;
using CardGameEngine.Cards;
using Script.Networking;
using TMPro;
using UnityEngine;

namespace Script
{
    public class CardRenderer : MonoBehaviour
    {
        public Card Card;
        public TextMeshPro nom;
        public TextMeshPro description;
        public SpriteRenderer niveau;
        public SpriteRenderer cout;
        public SpriteRenderer illustration;
        public SpriteRenderer fond;

        public bool faceCachee;
        
        [SerializeField] private CardImageDatabase imagesCartes;

        private Animator _animator;

        public float Width => fond.bounds.size.x;

        private bool _hover = false;
        private static readonly int HoverProp = Animator.StringToHash("Hovered");
        [NonSerialized] public bool preconditionJouable;
        [NonSerialized] public bool assezDePA;

        public bool Hover
        {
            get => _hover;
            set
            {
                if (_hover != value)
                {
                    _animator.SetBool(HoverProp,value);
                }
                _hover = value;
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            nom.text = Card.Name.Value;
            description.text = Card.Description.Value;
            niveau.GetComponentInChildren<TextMeshPro>().text = Card.CurrentLevel.Value + "/" + Card.MaxLevel;
            cout.GetComponentInChildren<TextMeshPro>().text = Card.Cost.Value.ToString();
            illustration.sprite = imagesCartes[Card.ImageId.Value];
            
            Flip();
        }

        public void Flip()
        {
            faceCachee = !faceCachee;
            nom.gameObject.SetActive(!faceCachee);
            description.gameObject.SetActive(!faceCachee);
            niveau.gameObject.SetActive(!faceCachee);
            cout.gameObject.SetActive(!faceCachee);
            illustration.gameObject.SetActive(!faceCachee);
        }
        
        //void 

        // Update is called once per frame
        void Update()
        {
            
        }

        public void RefreshPrecondition()
        {
            this.preconditionJouable = Card.CanBePlayed(UnityGame.LocalGamePlayer);
            this.assezDePA = Card.Cost.Value <= UnityGame.LocalGamePlayer.ActionPoints.Value;
        }
    }
}