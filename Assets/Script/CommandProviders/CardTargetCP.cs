using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script;
using Script.Input;
using Script.Networking;
using Script.Networking.Commands.Extern;
using TMPro;
using UnityEngine;

public class CardTargetCP : CommandProviderBehaviour
{
    private NetworkedGame _networkedGame;
    private InputManager _inputManager;


    public TextMeshPro locationPrefab;
    public GameObject targetPicker;
    public GameObject cardContainer;
    private TextMeshPro _targetText;
    public List<CardRenderer> Pickable { get; } = new List<CardRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _networkedGame = FindObjectOfType<NetworkedGame>();
        _unityGame = FindObjectOfType<UnityGame>();
        _networkedGame.RegisterCommandProvider<ChooseCardTargetCommand>(this);
        _targetText = targetPicker.GetComponentInChildren<TextMeshPro>();
    }

    private Dictionary<CardRenderer, Vector3> _originalPositions = new Dictionary<CardRenderer, Vector3>();
    private Dictionary<CardRenderer, Transform> _parents = new Dictionary<CardRenderer, Transform>();
    private UnityGame _unityGame;

    protected override void DoAction()
    {
        _inputManager.OnTarget();
        var cardTarget = (ChooseCardTargetData)infoStruct;

        _targetText.text = cardTarget.TargetName;

        var cardRenderers = cardTarget.CardList.Select(c => _unityGame.CardRenderers[c]).Select(WithLocation).ToList();

        float cardWidth = cardRenderers[0].Item1.Width;

        float margin = 0.1f;
        float toalWidth = (cardWidth + margin) * cardRenderers.Count;
        float inverseScale = 1 / targetPicker.transform.localScale.x;
        var positionZ = targetPicker.transform.position.z + 2;
        for (var index = 0; index < cardRenderers.Count; index++)
        {
            var (cardRenderer,location) = cardRenderers[index];
            _originalPositions[cardRenderer] = cardRenderer.transform.localPosition;
            _parents[cardRenderer] = cardRenderer.transform.parent;
            cardRenderer.transform.parent = cardContainer.transform;

            float xpos = (cardWidth + margin) * index - toalWidth / 2.0f;

            cardRenderer.transform.localPosition = new Vector3(xpos * inverseScale, 0f, positionZ);
            Pickable.Add(cardRenderer);
            TextMeshPro locPref = Instantiate(locationPrefab, cardContainer.transform);
            locPref.transform.localPosition = new Vector3(xpos * inverseScale, -(cardRenderer.Height + 0.2f) * inverseScale);
            locPref.text = location;
        }

        targetPicker.SetActive(true);

        // var chooseCardTargetCommand = new ChooseCardTargetCommand {CardId = cardId};
        // _networkedGame.DoLocalAction(chooseCardTargetCommand);
    }

    private (CardRenderer,string) WithLocation(CardRenderer cardRenderer)
    {
        var pile = _unityGame.PileRenderers.Select(p => p.Value).FirstOrDefault(f => f.cards.Contains(cardRenderer));

        if (pile == null) return (cardRenderer, "Inconnu");
        string texte = pile.pileType switch
        {
            PileType.Deck => "Deck",
            PileType.Discard => "Défausse",
            PileType.Hand => "Main",
            _ => throw new ArgumentOutOfRangeException()
        };
        texte += $"[{pile.cards.IndexOf(cardRenderer)}] (" + (UnityGame.LocalPlayer == pile.owner ? "Moi" : "Adv") + ")";

        return (cardRenderer,texte);
    }

    public void OnSelected(CardRenderer cardRenderer)
    {
        _inputManager.OnAfterTarget();
        foreach (var (card, pos) in _originalPositions.Select(x => (x.Key, x.Value)))
        {
            card.transform.parent = _parents[card];
            card.transform.localPosition = pos;
        }

        _originalPositions.Clear();
        _parents.Clear();
        Pickable.Clear();
        foreach (Transform o in cardContainer.transform)
        {
            Destroy(o.gameObject);
        }
        targetPicker.SetActive(false);
        
        var chooseCardTargetCommand = new ChooseCardTargetCommand { CardId = cardRenderer.Card.Id };
        _networkedGame.DoLocalAction(chooseCardTargetCommand);
    }
}