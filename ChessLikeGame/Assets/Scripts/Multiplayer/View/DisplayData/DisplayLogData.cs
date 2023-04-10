using TMPro;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class DisplayLogData : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void SetText(string info)
        {
            text.text = info;
        }
    }
}