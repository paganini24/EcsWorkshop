using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmallGame
{
    [AlwaysUpdateSystem]
    public class GameHud : ComponentSystem
    {
        private GameObject _canvasNewGame,_canvasScore;
        private TextMeshProUGUI _textScore;

        private int _cachedScore = Int32.MinValue;

        struct PlayerState
        {
            public readonly int Length;
            public ComponentDataArray<PlayerScore> playerScore;
        }

        [Inject]
        PlayerState playerStateGroup;

        protected override void OnCreateManager()
        {
            // new game menu
            var menu = Resources.Load<GameObject>("UI/Canvas_NewGame");
            _canvasNewGame = Object.Instantiate(menu);
            // Score menu
            var scoremenu = Resources.Load<GameObject>("UI/Canvas_Score");
            _canvasScore = Object.Instantiate(scoremenu);
            _textScore = _canvasScore.GetComponentInChildren<TextMeshProUGUI>();
            _canvasScore.SetActive(false);
        }

        protected override void OnUpdate()
        {
            var isAlive = playerStateGroup.playerScore.Length > 0;
            if (isAlive)
            {
                _cachedScore = playerStateGroup.playerScore[0].score;
                if(_textScore==null)return;
                _textScore.text = $"Score: {_cachedScore}";
                _canvasScore.SetActive(true);
                _canvasNewGame.SetActive(false);
            }
            else
            {
                _canvasNewGame.SetActive(true);
                if (_textScore == null)
                    return;
                _textScore.text = $"Game Over\nYour Score: {_cachedScore}";
            }
        }
    }
}
