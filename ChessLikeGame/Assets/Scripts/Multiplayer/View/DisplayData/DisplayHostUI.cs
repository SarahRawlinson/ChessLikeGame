using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayHostUI : MonoBehaviour
    {
        [FormerlySerializedAs("_tmpText")] [SerializeField] private TMP_Text _roomNameText;
        private string id = "-1";
        private static event Action<string> onJoinRoom; 
        public void UpdateHostInfo(string txt, string index)
        {
            _roomNameText.text = txt;
            id = index;
        }

        public void Join()
        {
            onJoinRoom?.Invoke(id);
        }
        
    }
}