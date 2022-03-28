using System;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events;
using CardGameEngine.EventSystem.Events.CardEvents;
using MoonSharp.Interpreter;
using Script.Networking;
using Sentry;
using TMPro;
using UnityEngine;

namespace Script
{
    public class CardRenderer : MonoBehaviour, IEventSubscriber
    {
        private static readonly int HoverProp = Animator.StringToHash("Hovered");

        private static readonly int HoverHeightProp = Animator.StringToHash("HoverHeight");
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

        [SerializeField] private CardImageDatabase imagesCartes;

        [SerializeField] private Color couleurJouable;
        [SerializeField] private Color couleurPasJouable;

        [SerializeField] private Color paColorQuandAssez;
        [SerializeField] private Color paColorQuandPasAssez;
        [SerializeField] private SpriteRenderer ameliorationImage;
        [SerializeField] private SpriteRenderer jouableCalque;
        [SerializeField] private List<Sprite> spriteChainage;

        private Animator _animator;
        private UnityGame _game;


        private bool _hover;
        private bool _hoverHeight;
        public Card Card;


        [NonSerialized] public bool faceCachee;

        private bool DisplayMode => scriptToDisplay != string.Empty;

        public float Width => fond.bounds.size.x;
        public float Height => fond.bounds.size.y;

        public bool PreconditionJouable { get; private set; }
        public bool AssezDePa { get; private set; }
        public bool Ameliorable { get; private set; }

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

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _game = FindObjectOfType<UnityGame>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            SetData();
        }

        //void 

        // Update is called once per frame
        private void Update()
        {
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

        public void SetScript(string ScriptName)
        {
            scriptToDisplay = ScriptName;
            var game = new Game(Application.streamingAssetsPath + "/EffectsScripts",
                new DumbCallbacks(),
                new List<string> {scriptToDisplay},
                new List<string>());
            Card = game.Player1.Deck[0];
            Subscribe(game.EventManager);
            RefreshPrecondition(true);
            // game.StartGame();
        }

        private void SetData()
        {
            nom.text = Card.Name.Value;
            gameObject.name = Card.Name.Value;
            description.text = Card.Description.Value;
            SetLevel();
            cout.text = Card.Cost.Value.ToString();
            illustration.sprite = imagesCartes[Card.ImageId.Value];
            chainage.sprite = spriteChainage[(int) Card.ChainMode.Value];
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

        public void RefreshPrecondition(bool hideAll = false)
        {
            if (!DisplayMode)
            {
                var cardHolder = _game.Game.Player1.Hand.Contains(Card) ? _game.Game.Player1 : _game.Game.Player2;

                if (UnityGame.IsLocalPlayer(cardHolder) && !hideAll)
                {
                    PreconditionJouable = _game.RunOnGameThread(g =>
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
                    AssezDePa = Card.Cost.Value <= UnityGame.LocalGamePlayer.ActionPoints.Value;
                }
                else
                {
                    PreconditionJouable = true;
                    AssezDePa = true;
                }
            }
            else
            {
                PreconditionJouable = true;
                AssezDePa = true;
            }

            Ameliorable = !Card.IsMaxLevel;

            SetLampVert(ameliorationImage, Ameliorable && !faceCachee);
            SetLampVert(jouableCalque, PreconditionJouable && !faceCachee);


            cout.color = AssezDePa ? paColorQuandAssez : paColorQuandPasAssez;
        }

        private void SetLampVert(SpriteRenderer spriteRenderer, bool vert)
        {
            spriteRenderer.sprite = vert ? lvlVert : lvlRouge;
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
            SetTransparence(pourcentage, cout);
            SetTransparence(pourcentage, description);
            SetTransparence(pourcentage, nom);
            SetTransparenceSprite(pourcentage, illustration);
            SetTransparenceSprite(pourcentage, fond);
            SetTransparenceSprite(pourcentage, ameliorationImage);
            SetTransparenceSprite(pourcentage, jouableCalque);
            SetTransparenceSprite(pourcentage, chainage);
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