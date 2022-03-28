using System.Linq;
using Script.Networking;
using Script.Networking.Commands.Extern;

namespace Script
{
    public class CardTargetCommandProvider : CommandProviderBehaviour
    {
        private CardPickerDisplay _cardPickerDisplay;
        private UnityGame _unityGame;


        private void Start()
        {
            NetworkedGame.RegisterCommandProvider<ChooseCardTargetCommand>(this);
            _unityGame = FindObjectOfType<UnityGame>();
            _cardPickerDisplay = FindObjectOfType<CardPickerDisplay>();
        }

        protected override void DoAction()
        {
            var infoStruct = (ChooseCardTargetData) InfoStruct;
            _cardPickerDisplay.DisplayPicker(
                infoStruct.CardList.Select(c => _cardPickerDisplay.WithLocation(_unityGame.CardRenderers[c])).ToList(),
                infoStruct.TargetName,
                OnPick
            );
        }

        private void OnPick(CardRenderer cardRenderer)
        {
            var chooseCardTargetCommand = new ChooseCardTargetCommand {CardId = cardRenderer.Card.Id};
            NetworkedGame.DoLocalAction(chooseCardTargetCommand);
        }
    }
}