using System.Collections;
using System.Collections.Generic;
using Script.Networking;
using Script.Networking.Commands.Extern;
using UnityEngine;

public class CardTargetCP : CommandProviderBehaviour
{
    private NetworkedGame _networkedGame;

    // Start is called before the first frame update
    void Start()
    {
        _networkedGame = FindObjectOfType<NetworkedGame>();
        _networkedGame.RegisterCommandProvider<ChooseCardTargetCommand>(this);
    }

    protected override void DoAction()
    {
        var cardTarget = (ChooseCardTargetData) infoStruct;

        // TODO Demander carte à prendre et utiliser son id après
        const int cardId = 0;

        var chooseCardTargetCommand = new ChooseCardTargetCommand {CardId = cardId};
        _networkedGame.DoLocalAction(chooseCardTargetCommand);
    }
}