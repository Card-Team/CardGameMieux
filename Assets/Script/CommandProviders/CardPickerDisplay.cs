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

public class CardPickerDisplay : MonoBehaviour
{
    private NetworkedGame _networkedGame;
    private InputManager _inputManager;


    public TextMeshPro locationPrefab;
    public GameObject targetPicker;
    public GameObject cardContainer;

    public float maxScrollDistance = 3;

    private TextMeshPro _targetText;
    public List<CardRenderer> Pickable { get; } = new List<CardRenderer>();
    private List<TextMeshPro> texts { get; } = new List<TextMeshPro>();

    // Start is called before the first frame update
    void Start()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _networkedGame = FindObjectOfType<NetworkedGame>();
        _unityGame = FindObjectOfType<UnityGame>();
        _targetText = targetPicker.GetComponentInChildren<TextMeshPro>();
    }

    private Dictionary<CardRenderer, Vector3> _originalPositions = new Dictionary<CardRenderer, Vector3>();
    private Dictionary<CardRenderer, Transform> _parents = new Dictionary<CardRenderer, Transform>();
    private UnityGame _unityGame;
    private Action<CardRenderer> _onPickCallback;


    public void DisplayPicker(IEnumerable<CardRenderer> cardRenderers,string text,Action<CardRenderer> onPick)
    {
        DisplayPicker(cardRenderers.Select(c => (c,"")).ToList(),text,onPick);
    }
    
    public void DisplayPicker(List<(CardRenderer,string)> cardRenderers,string text,Action<CardRenderer> onPick)
    {
        _onPickCallback = onPick;
        _inputManager.DisableAll();

        _targetText.text = text;
        
        float cardWidth = cardRenderers[0].Item1.Width;

        float margin = 0.4f;
        float toalWidth = (cardWidth + margin) * cardRenderers.Count;
        float inverseScale = 1 / targetPicker.transform.localScale.x;
        var positionZ = targetPicker.transform.position.z + 2;
        Debug.Log("counting pickable");
        for (var index = 0; index < cardRenderers.Count; index++)
        {
            var (cardRenderer, location) = cardRenderers[index];
            _originalPositions[cardRenderer] = cardRenderer.transform.localPosition;
            _parents[cardRenderer] = cardRenderer.transform.parent;
            cardRenderer.transform.parent = cardContainer.transform;

            float xpos = (cardWidth + margin) * index - toalWidth / 2.0f;

            StartCoroutine(
                PileRenderer.MoveCardInTime(cardRenderer, new Vector3(xpos * inverseScale, 0f, positionZ), 0.1f,
                    c => _inputManager.EnableThis(InputManager.InputType.Targeting))
            );
            Pickable.Add(cardRenderer);
            TextMeshPro locPref = Instantiate(locationPrefab, cardContainer.transform);
            locPref.transform.parent = cardRenderer.transform.GetChild(0);
            Debug.Log($"CardHeight : {cardRenderer.Height}");
            locPref.transform.localPosition =
                new Vector3(0, -(cardRenderer.Height + 1.3f) );
            locPref.text = location;
            texts.Add(locPref);
            Debug.Log("pickable found");
        }

        targetPicker.SetActive(true);
        Debug.Log("picker active");
    }

    public void OnSelected(CardRenderer cardRenderer)
    {
        _inputManager.DisableAll();
        foreach (var (card, pos) in _originalPositions.Select(x => (x.Key, x.Value)))
        {
            card.transform.parent = _parents[card];
            StartCoroutine(
                PileRenderer.MoveCardInTime(card, pos, 0.1f, c =>
                {
                    var ourTurn = Equals(_unityGame.RunOnGameThread(g => g.CurrentPlayer), UnityGame.LocalGamePlayer);
                    if(ourTurn) _inputManager.EnableThis(InputManager.InputType.Main);
                })
            );
        }

        _originalPositions.Clear();
        _parents.Clear();
        Pickable.Clear();
        
        foreach (var t in texts)
        {
            Destroy(t.gameObject);
        }
        texts.Clear();

        targetPicker.SetActive(false);

        _onPickCallback(cardRenderer);
    }

    private float GetFirstCardLeft()
    {
        var cardRenderer = Pickable[0];
        return cardRenderer.transform.position.x - (cardRenderer.Width * 1 / cardContainer.transform.localScale.x) / 2;
    }

    private float GetLastCardRight()
    {
        var cardRenderer = Pickable[Pickable.Count - 1];
        return cardRenderer.transform.position.x + (cardRenderer.Width * 1 / cardContainer.transform.localScale.x) / 2;
    }

    public void Scroll(float scrollValue)
    {
        float newPos;
        if (scrollValue < 0)
        {
            newPos = GetLastCardRight() + scrollValue * 1 / cardContainer.transform.localScale.x;
            if (newPos <= maxScrollDistance)
            {
                return;
            }
        }
        else if (scrollValue > 0)
        {
            newPos = GetFirstCardLeft() + scrollValue * 1 / cardContainer.transform.localScale.x;
            if (newPos >= -maxScrollDistance)
            {
                return;
            }
        }
        

        var oldPos = cardContainer.transform.localPosition;
        oldPos.x += scrollValue;
        cardContainer.transform.localPosition = oldPos;
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

    public (CardRenderer, string) WithLocation(CardRenderer cardRenderer)
    {
        var pile = _unityGame.PileRenderers.Select(p => p.Value)
            .FirstOrDefault(f => f.cards.Contains(cardRenderer));

        if (pile == null) return (cardRenderer, "Inconnu");
        string texte = pile.pileType switch
        {
            PileType.Deck => "Deck",
            PileType.Discard => "Défausse",
            PileType.Hand => "Main",
            _ => throw new ArgumentOutOfRangeException()
        };
        texte += $"[{pile.cards.IndexOf(cardRenderer)}] (" + (UnityGame.LocalPlayer == pile.owner ? "Moi" : "Adv") +
                 ")";

        return (cardRenderer, texte);
    }
}