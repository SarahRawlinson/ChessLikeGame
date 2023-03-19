using System;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class DisplayHostUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;

        public void UpdateText(string txt)
        {
            _tmpText.text = txt;
        }
    }
}