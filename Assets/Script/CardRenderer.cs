using System;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events.CardEvents;
using CardGameEngine.EventSystem.Events.CardEvents.PropertyChange;
using Script.Networking;
using TMPro;
using UnityEngine;
using Event = CardGameEngine.EventSystem.Events.Event;

namespace Script
{
    public class CardRenderer : MonoBehaviour, IEventSubscriber
    {
        public Card Card;
        public TextMeshPro nom;
        public TextMeshPro description;
        public AffichageNiveau niveau;
        public TextMeshPro cout;
        public SpriteRenderer illustration;
        public SpriteRenderer fond;

        public string scriptToDisplay;

        private bool DisplayMode => scriptToDisplay != string.Empty;

        

        [NonSerialized] public bool faceCachee = false;

        [SerializeField] private CardImageDatabase imagesCartes;

        private Animator _animator;

        public float Width => fond.bounds.size.x;
        public float Height => fond.bounds.size.y;
        

        private bool _hover = false;
        private static readonly int HoverProp = Animator.StringToHash("Hovered");
        
        [NonSerialized] public bool PreconditionJouable;
        [NonSerialized] public bool AssezDePa;
        
        [SerializeField] private Color couleurJouable;
        [SerializeField] private Color couleurPasJouable;
        
        [SerializeField] private Color paColorQuandAssez;
        [SerializeField] private Color paColorQuandPasAssez;

        public bool Hover
        {
            get => _hover;
            set
            {
                if (_hover != value)
                {
                    _animator.SetBool(HoverProp, value);
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value ? -5 : 0);
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
            if (DisplayMode)
            {
                Game game = new Game(Application.streamingAssetsPath + "/EffectsScripts",
                    new DumbCallbacks(),
                    new List<string>{scriptToDisplay},
                    new List<string>());
                this.Card = game.Player1.Deck[0];
                Subscribe(game.EventManager);
                // game.StartGame();
            }
            SetData();
        }

        private void SetData()
        {
            nom.text = Card.Name.Value;
            gameObject.name = Card.Name.Value;
            description.text = Card.Description.Value;
            SetLevel();
            cout.text = Card.Cost.Value.ToString();
            illustration.sprite = imagesCartes[Card.ImageId.Value];
        }

        private void SetLevel()
        {
            niveau.niveauActuel = Card.CurrentLevel.Value;
            niveau.niveauMax = Card.MaxLevel;
            niveau.RefreshCercle();
        }
        

        public void Flip()
        {
            if(DisplayMode) return;
            faceCachee = !faceCachee;
            nom.gameObject.SetActive(!faceCachee);
            description.gameObject.SetActive(!faceCachee);
            niveau.gameObject.SetActive(!faceCachee);
            cout.transform.parent.gameObject.SetActive(!faceCachee);
            illustration.gameObject.SetActive(!faceCachee);
            if (faceCachee)
            {
                fond.color = couleurJouable;
            }
            else
            {
                RefreshPrecondition();
            }
        }

        //void 

        // Update is called once per frame
        void Update()
        {
        }

        public void RefreshPrecondition()
        {
            if(DisplayMode) return;
            this.PreconditionJouable = Card.CanBePlayed(UnityGame.LocalGamePlayer);
            this.AssezDePa = Card.Cost.Value <= UnityGame.LocalGamePlayer.ActionPoints.Value;

            fond.color = this.PreconditionJouable || faceCachee ? couleurJouable : couleurPasJouable;

            this.cout.color = AssezDePa ? paColorQuandAssez : paColorQuandPasAssez;
        }

        public void Subscribe(SyncEventWrapper eventManager)
        {
            eventManager.SubscribeToEvent<CardEvent>(e =>
            {
                Debug.Log($"CardEvent : {e.GetType()}");
                if (!Equals(e.Card, Card)) return;
                SetData();
            }, postEvent: true);
        }
        
        private void Subscribe(EventManager eventManager)
        {
            eventManager.SubscribeToEvent<CardEvent>(e =>
            {
                Debug.Log($"CardEvent : {e.GetType()}");
                if (!Equals(e.Card, Card)) return;
                SetData();
            }, postEvent: true);
        }
    }
}