using System;
using Script.Input;
using Script.Networking;
using Script.Networking.Commands.Extern;
using TMPro;
using UnityEngine;

namespace Script
{
    public class PlayerTargetCommandProvider : CommandProviderBehaviour
    {
        public TextMeshProUGUI cibleTexte;

        public GameObject choosePanel;
        private InputManager _inputManager;
        private UnityGame _unityGame;

        private void Start()
        {
            choosePanel.SetActive(false);
            _inputManager = FindObjectOfType<InputManager>();
            _unityGame = FindObjectOfType<UnityGame>();
            NetworkedGame.RegisterCommandProvider<ChoosePlayerTargetCommand>(this);
        }

        protected override void DoAction()
        {
            Debug.Log("do action player target");
            _inputManager.EnableThis(InputManager.InputType.UI);
            var data = (ChoosePlayerTargetData)this.InfoStruct;
            cibleTexte.text = data.TargetName;
            choosePanel.SetActive(true);
        }

        public void OnChoose(bool local)
        {
            _inputManager.DisableAll();
            var player = local ? UnityGame.LocalGamePlayer : UnityGame.LocalGamePlayer.OtherPlayer;
            cibleTexte.text = "";
            choosePanel.SetActive(false);

            var ourTurn = _unityGame.Game.CurrentPlayer == UnityGame.LocalGamePlayer;
            if (ourTurn) _inputManager.EnableThis(InputManager.InputType.Main);
            NetworkedGame.DoLocalAction(new ChoosePlayerTargetCommand(){PlayerId = player.Id});
        }
        
    }
}