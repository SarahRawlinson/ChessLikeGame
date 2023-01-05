using System.Collections.Generic;
using Chess.Control;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chess.UI
{
    public class PiecePicker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpawnButton spawnButton;
        [SerializeField] private Scrollbar scrollBar;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject infoObject;
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private ScrollRect scroll;
        private readonly List<SpawnButton> _spawnButtons = new List<SpawnButton>();


        public bool UIActive { get; private set; } = false;

        public void AddSpawnButton(ControlItem item)
        {
            if (!item.gameObject.TryGetComponent(out RTSUnitSpawner spawner))
            {
                //Debug.Log("No Spawner");
                return;
            }
            for (int i = 0; i < spawner.UnitPrefabs.Length; i++)
            {
                //Debug.Log("Try Add Button");
                ControlItem controlItem = (ControlItem)item.GetComponent<RTSUnitSpawner>().UnitPrefabs[i];

                SpawnButton buttonInstance = Instantiate(spawnButton, scroll.content);
                ButtonActivate(buttonInstance);
                buttonInstance.SetSpawner(spawner, i, controlItem);
                _spawnButtons.Add(buttonInstance);
                //Debug.Log("Button Added");
            }
        }

        private void ButtonActivate(ControlItemButton buttonInstance)
        {
            buttonInstance.OnButtonDeactivate += HandleButtonDeactivate;
            buttonInstance.OnButtonActive += HandleButtonActive;
            buttonInstance.OnButtonDestroy += HandleButtonDestroyed;
            buttonInstance.OnEnterButton += HandleButtonEnter;
            buttonInstance.OnExitButton += HandleButtonExit;
        }

        private void HandleButtonDestroyed(ControlItemButton button)
        {
            button.OnButtonDeactivate -= HandleButtonDeactivate;
            button.OnButtonActive -= HandleButtonActive;
            button.OnButtonDestroy -= HandleButtonDestroyed;
            button.OnEnterButton -= HandleButtonEnter;
            button.OnExitButton -= HandleButtonExit;
        }

        private void HandleButtonActive(ControlItemButton button)
        {
            scrollRect.StopMovement();
            scrollRect.enabled = false;
        }

        private void HandleButtonEnter(ControlItemButton button)
        {
            infoObject.SetActive(true);
            string description = button.StationOrUnit.GetDescription();
            infoText.text = description;
            // Debug.Log(description);
        }
        private void HandleButtonExit(ControlItemButton button)
        {
            infoObject.SetActive(false);
        }

        private void HandleButtonDeactivate(ControlItemButton button)
        {
            scrollRect.StopMovement();
            scrollRect.enabled = true;
        }
        

        public void OnPointerExit(PointerEventData eventData)
        {
            UIActive = false;
        }

        internal void ClearSpawnButtons()
        {
            //Debug.Log("Remove Buttons");
            int count = _spawnButtons.Count;
            for (int i = count; i > 0; i--)
            {
                SpawnButton button = _spawnButtons[i-1];
                _spawnButtons.Remove(button);
                Destroy(button.gameObject);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            UIActive = true;
        }
    }
    
}