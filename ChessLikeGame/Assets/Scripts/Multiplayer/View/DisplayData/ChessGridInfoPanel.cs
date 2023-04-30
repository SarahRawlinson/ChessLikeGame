using System;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class ChessGridInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private GameObject hide_show_display;
        private bool visable = false;
        [SerializeField] private float timeShow;
        private float _timeSincePressed;

        public void UpdateInfoText(string text)
        {
            infoText.text = text;
            _timeSincePressed = 0;
            visable = true;
            hide_show_display.SetActive(true);
        }

        private void Update()
        {
            if (!visable) return;
            if (_timeSincePressed > timeShow)
            {
                hide_show_display.SetActive(false);
                visable = false;
                return;
            }
            _timeSincePressed += Time.deltaTime;
        }
    }
}