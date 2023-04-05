using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.UI
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] string onText;
        [SerializeField] string offText;
        [FormerlySerializedAs("on")] [SerializeField] bool isOn;
        [SerializeField] private TMP_Text _tmpText;

        private void Start()
        {
            UpdateText();
        }

        public bool IsOn()
        {
            return isOn;
        }

        public void SetIsOn(bool on)
        {
            isOn = on;
            UpdateText();
        }

        public void UpdateText()
        {
            _tmpText.text = isOn ? onText : offText;
        }

        public void Toggle()
        {
            isOn = !isOn;
            UpdateText();
        }
    }
}
