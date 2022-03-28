using System;
using System.Collections.Generic;
using System.Linq;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events;
using CardGameEngine.EventSystem.Events.CardEvents;
using CardGameEngine.EventSystem.Events.CardEvents.PropertyChange;
using CardGameEngine.GameSystems;
using MoonSharp.Interpreter;
using Script.Networking;
using Sentry;
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
        public SpriteRenderer chainage;

        public Sprite lvlVert;
        public Sprite lvlRouge;

        public Sprite fondNormal;
        public Sprite fondDos;

        public string scriptToDisplay;

        private bool DisplayMode => scriptToDisplay != string.Empty;


        [NonSerialized] public bool faceCachee = false;

        [SerializeField] private CardImageDatabase imagesCartes;

        private Animator _animator;

        public float Width => fond.bounds.size.x;
        public float Height => fond.bounds.size.y;


        private bool _hover = false;

        private static readonly int HoverProp = Animator.StringToHash("Hovered");

        public bool PreconditionJouable { get; private set; }
        public bool AssezDePa { get; private set; }
        public bool Ameliorable { get; private set; }

        [SerializeField] private Color couleurJouable;
        [SerializeField] private Color couleurPasJouable;

        [SerializeField] private Color paColorQuandAssez;
        [SerializeField] private Color paColorQuandPasAssez;
        private UnityGame _game;
        private bool _hoverHeight = false;
        [SerializeField] private SpriteRenderer ameliorationImage;
        [SerializeField] private SpriteRenderer jouableCalque;

        public bool Hover
        {
            get => _hover;
            set
            {
                if (_hover != value)
                {
                    _animator.SetBool(HoverProp, value);
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                        value ? -5 : 0);
                }

                _hover = value;
            }
        }


        public bool HoverHeight
        {
            get => _hoverHeight;
            set
            {
                _animator.SetBool(HoverHeightProp, value);
                _hoverHeight = value;
            }
        }

        private static readonly int HoverHeightProp = Animator.StringToHash("HoverHeight");
        [SerializeField] private List<Sprite> spriteChainage;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _game = FindObjectOfType<UnityGame>();
        }

        public void SetScript(String ScriptName)
        {
            scriptToDisplay = ScriptName;
            Game game = new Game(Application.streamingAssetsPath + "/EffectsScripts",
                new DumbCallbacks(),
                new List<string> { scriptToDisplay },
                new List<string>());
            this.Card = game.Player1.Deck[0];
            Subscribe(game.EventManager);
            RefreshPrecondition(true);
            // game.StartGame();
        }

        // Start is called before the first frame update
        void Start()
        {
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
            chainage.sprite = spriteChainage[(int)Card.ChainMode.Value];
        }

        private void SetLevel()
        {
            niveau.niveauActuel = Card.CurrentLevel.Value;
            niveau.niveauMax = Card.MaxLevel;
            niveau.RefreshCercle();
        }


        public void Flip()
        {
            if (DisplayMode) return;
            faceCachee = !faceCachee;
            nom.gameObject.SetActive(!faceCachee);
            description.gameObject.SetActive(!faceCachee);
            niveau.gameObject.SetActive(!faceCachee);
            cout.transform.parent.gameObject.SetActive(!faceCachee);
            illustration.gameObject.SetActive(!faceCachee);
            ameliorationImage.gameObject.SetActive(!faceCachee);
            jouableCalque.gameObject.SetActive(!faceCachee);
            chainage.gameObject.SetActive(!faceCachee);
            fond.sprite = faceCachee ? fondDos : fondNormal;
            RefreshPrecondition();
        }

        //void 

        // Update is called once per frame
        void Update()
        {
        }

        public void RefreshPrecondition(bool hideAll = false)
        {

            if (!DisplayMode)
            {
                Player cardHolder = _game.Game.Player1.Hand.Contains(Card) ? _game.Game.Player1 : _game.Game.Player2;

                if (UnityGame.IsLocalPlayer(cardHolder) && !hideAll)
                {
                    this.PreconditionJouable = _game.RunOnGameThread(g =>
                    {
                        try
                        {
                            return Card.CanBePlayed(cardHolder);
                        }
                        catch (ScriptRuntimeException e)
                        {
                            Debug.LogError($"Erreur lors du CanBePlayed de {Card}");
                            SentrySdk.AddBreadcrumb($"Appel de CanBePlayed pour {Card}/{Card.Id}");
                            _game.Network.errorUtils.toPrint.Enqueue(e);
                            return false;
                        }
                    });
                    this.AssezDePa = Card.Cost.Value <= UnityGame.LocalGamePlayer.ActionPoints.Value;

                }
                else
                {
                    this.PreconditionJouable = true;
                    this.AssezDePa = true;
                }
            }
            else
            {
                this.PreconditionJouable = true;
                this.AssezDePa = true;
            }

            this.Ameliorable = !Card.IsMaxLevel;

            SetLampVert(this.ameliorationImage, this.Ameliorable && !faceCachee);
            SetLampVert(this.jouableCalque, this.PreconditionJouable && !faceCachee);


            this.cout.color = AssezDePa ? paColorQuandAssez : paColorQuandPasAssez;
        }

        private void SetLampVert(SpriteRenderer spriteRenderer, bool vert)
        {
            spriteRenderer.sprite = vert ? lvlVert : lvlRouge;
        }


        public void Subscribe(SyncEventWrapper eventManager)
        {
            eventManager.SubscribeToEvent<CardEvent>(e =>
            {
                // Debug.Log($"CardEvent : {e.GetType()}");
                if (Equals(e.Card, Card))
                {
                    SetData();
                    return;
                }

                RefreshPrecondition();
            }, postEvent: true);
            eventManager.SubscribeToEvent<ActionPointsEditEvent>(e => { RefreshPrecondition(); }, false, true);
        }

        private void Subscribe(EventManager eventManager)
        {
            eventManager.SubscribeToEvent<CardEvent>(e =>
            {
                // Debug.Log($"CardEvent : {e.GetType()}");
                if (!Equals(e.Card, Card)) return;
                SetData();
            }, postEvent: true);
        }

        public void SetTransparence(float pourcentage)
        {
            SetTransparence(pourcentage, this.cout);
            SetTransparence(pourcentage, this.description);
            SetTransparence(pourcentage, this.nom);
            SetTransparenceSprite(pourcentage, this.illustration);
            SetTransparenceSprite(pourcentage, this.fond);
            SetTransparenceSprite(pourcentage, this.ameliorationImage);
            SetTransparenceSprite(pourcentage, this.jouableCalque);
            SetTransparenceSprite(pourcentage, this.chainage);
            niveau.fontTransparent(pourcentage);
        }

        private void SetTransparence(float pourcentage, TextMeshPro text)
        {
            var coutColor = text.color;
            coutColor.a = pourcentage;
            text.color = coutColor;
        }

        private void SetTransparenceSprite(float pourcentage, SpriteRenderer sprite)
        {
            var coutColor = sprite.color;
            coutColor.a = pourcentage;
            sprite.color = coutColor;
        }
    }
}