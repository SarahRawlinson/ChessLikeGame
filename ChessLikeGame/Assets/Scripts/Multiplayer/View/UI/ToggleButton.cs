using TMPro;
using UnityEngine;

namespace Multiplayer.View.UI
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] string onText;
        [SerializeField] string offText;
        [SerializeField] bool on;
        [SerializeField] private TMP_Text _tmpText;

        private void Start()
        {
            UpdateText();
        }

        public bool IsOn()
        {
            return on;
        }

        public void UpdateText()
        {
            _tmpText.text = on ? onText : offText;
        }

        public void Toggle()
        {
            on = !on;
            UpdateText();
        }
    }
}
