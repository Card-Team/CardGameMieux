using System;
using Script.Input;
using Script.Networking;
using Script.Networking.Commands.Extern;
using Sentry;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class GameEndCommandProvider : CommandProviderBehaviour
    {
        public GameObject victorypanel;
        public TextMeshProUGUI victoryText;

        private InputManager _inputManager;

        private void Start()
        {
            _inputManager = FindObjectOfType<InputManager>();
            NetworkedGame.RegisterCommandProvider<GameEndedFalseCommand>(this);
        }

        protected override void DoAction()
        {
            _inputManager.DisableAll();
            _inputManager.EnableThis(InputManager.InputType.UI);
            var data = (GameEndedFalseData)this.InfoStruct;
            victoryText.text = data.Winner == UnityGame.LocalGamePlayer
                ? "Bravo !\n" +
                  "Vous avez gagné la partie !"
                : "Dommage...\n" +
                  "Vous avez perdu";
            
            victorypanel.SetActive(true);
        }

        public void OnQuit()
        {
            SentrySdk.EndSession();
            SceneManager.LoadScene("Scenes/Menu Principal");
        }
    }
}