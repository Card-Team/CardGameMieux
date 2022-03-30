using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Script;
using Script.Input;
using Script.Networking;
using TMPro;
using UnityEngine;

public class CardPickerDisplay : MonoBehaviour
{
    public TextMeshPro locationPrefab;
    public GameObject targetPicker;
    public GameObject cardContainer;
    public GameObject cancelButton;

    public float maxScrollDistance = 3;
    private InputManager _inputManager;
    private NetworkedGame _networkedGame;
    [CanBeNull] private Action _onCancelCallback;
    private Action<CardRenderer> _onPickCallback;

    private readonly Dictionary<CardRenderer, Vector3> _originalPositions = new Dictionary<CardRenderer, Vector3>();
    private readonly Dictionary<CardRenderer, Transform> _parents = new Dictionary<CardRenderer, Transform>();

    private TextMeshPro _targetText;
    private UnityGame _unityGame;
    public List<CardRenderer> Pickable { get; } = new List<CardRenderer>();
    private List<TextMeshPro> texts { get; } = new List<TextMeshPro>();

    // Start is called before the first frame update
    private void Start()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _networkedGame = FindObjectOfType<NetworkedGame>();
        _unityGame = FindObjectOfType<UnityGame>();
        _targetText = targetPicker.GetComponentInChildren<TextMeshPro>();
    }

    private void OnDrawGizmos()
    {
        if (Pickable.Count > 0)
        {
            Gizmos.color = Color.blue;
            var firstCardLeft = GetFirstCardLeft();
            Gizmos.DrawCube(new Vector3(firstCardLeft, 0), Vector3.one * 0.5f);
            Gizmos.color = Color.red;
            var lastCardRight = GetLastCardRight();
            Gizmos.DrawCube(new Vector3(lastCardRight, 0), Vector3.one * 0.5f);

            var firstX = maxScrollDistance * 1 / cardContainer.transform.localScale.x;
            var lastX = -maxScrollDistance * 1 / cardContainer.transform.localScale.x;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(firstX, 0), new Vector3(lastX, 0));
        }
    }


    public void DisplayPicker(IEnumerable<CardRenderer> cardRenderers, string text, Action<CardRenderer> onPick,
        Action onCancel = null)
    {
        DisplayPicker(cardRenderers.Select(c => (c, "")).ToList(), text, onPick, onCancel);
    }

    public void DisplayPicker(List<(CardRenderer, string)> cardRenderers, string text, Action<CardRenderer> onPick,
        Action onCancel = null)
    {
        _onPickCallback = onPick;
        _onCancelCallback = onCancel;
        cancelButton.SetActive(_onCancelCallback != null);
        _inputManager.DisableAll();
        if (cardRenderers.Count == 0) throw new InvalidOperationException("DisplayPicker appelé avec une liste vide !");

        _targetText.text = text;

        var cardWidth = cardRenderers[0].Item1.Width;

        var margin = 0.4f;
        var toalWidth = (cardWidth + margin) * cardRenderers.Count;
        var inverseScale = 1 / targetPicker.transform.localScale.x;
        var positionZ = targetPicker.transform.position.z + 2;
        for (var index = 0; index < cardRenderers.Count; index++)
        {
            var (cardRenderer, location) = cardRenderers[index];
            _originalPositions[cardRenderer] = cardRenderer.transform.localPosition;
            _parents[cardRenderer] = cardRenderer.transform.parent;
            cardRenderer.transform.parent = cardContainer.transform;

            var xpos = (cardWidth + margin) * index - toalWidth / 2.0f;

            StartCoroutine(
                PileRenderer.MoveCardInTime(cardRenderer, new Vector3(xpos * inverseScale, 0f, positionZ), 0.2f,
                    c => _inputManager.EnableThis(InputManager.InputType.Targeting))
            );
            Pickable.Add(cardRenderer);
            var locPref = Instantiate(locationPrefab, cardContainer.transform);
            locPref.transform.parent = cardRenderer.transform.GetChild(0);
            locPref.transform.localPosition =
                new Vector3(0, -(cardRenderer.Height + 4f));
            locPref.text = location;
            texts.Add(locPref);
        }

        targetPicker.SetActive(true);
    }

    public void OnSelected(CardRenderer cardRenderer)
    {
        Cleanup();

        _onPickCallback(cardRenderer);
    }

    private void Cleanup()
    {
        _inputManager.DisableAll();
        cancelButton.SetActive(false);
        foreach (var (card, pos) in _originalPositions.Select(x => (x.Key, x.Value)))
        {
            card.transform.parent = _parents[card];
            card.HoverHeight = true; // si autre part que la main ,pas selectionnable donc on s'en fiche
            var curPos = card.transform.localPosition;
            card.transform.localPosition = new Vector3(curPos.x, curPos.y, pos.z);
            StartCoroutine(
                PileRenderer.MoveCardInTime(card, pos, 0.2f, c =>
                {
                    var ourTurn = _unityGame.Game.CurrentPlayer == UnityGame.LocalGamePlayer;
                    if (ourTurn) _inputManager.EnableThis(InputManager.InputType.Main);
                })
            );
        }

        _originalPositions.Clear();
        _parents.Clear();
        Pickable.Clear();

        foreach (var t in texts) Destroy(t.gameObject);

        texts.Clear();

        targetPicker.SetActive(false);
    }

    public void OnCancel()
    {
        Cleanup();
        _onCancelCallback?.Invoke();
    }

    private float GetFirstCardLeft()
    {
        var cardRenderer = Pickable[0];
        return cardRenderer.transform.position.x - cardRenderer.Width * 1 / cardContainer.transform.localScale.x / 2;
    }

    private float GetLastCardRight()
    {
        var cardRenderer = Pickable[Pickable.Count - 1];
        return cardRenderer.transform.position.x + cardRenderer.Width * 1 / cardContainer.transform.localScale.x / 2;
    }

    public void Scroll(float scrollValue)
    {
        float newPos;
        if (scrollValue < 0)
        {
            newPos = GetLastCardRight() + scrollValue * 1 / cardContainer.transform.localScale.x;
            if (newPos <= maxScrollDistance) return;
        }
        else if (scrollValue > 0)
        {
            newPos = GetFirstCardLeft() + scrollValue * 1 / cardContainer.transform.localScale.x;
            if (newPos >= -maxScrollDistance) return;
        }


        var oldPos = cardContainer.transform.localPosition;
        oldPos.x += scrollValue;
        cardContainer.transform.localPosition = oldPos;
    }

    public (CardRenderer, string) WithLocation(CardRenderer cardRenderer)
    {
        var pile = _unityGame.PileRenderers.Select(p => p.Value)
            .FirstOrDefault(f => f.cards.Contains(cardRenderer));

        if (pile == null) return (cardRenderer, "Inconnu");
        var texte = pile.pileType switch
        {
            PileType.Deck => "Deck",
            PileType.Discard => "Défausse",
            PileType.Hand => "Main",
            _ => throw new ArgumentOutOfRangeException()
        };
        texte += $"[{pile.cards.IndexOf(cardRenderer) + 1}] (" + (UnityGame.LocalPlayer == pile.owner ? "Moi" : "Adv") +
                 ")";

        return (cardRenderer, texte);
    }
}