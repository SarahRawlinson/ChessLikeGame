using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.UI
{
    public class UIDropDownShowGameObject : MonoBehaviour
    {
        [Serializable]
        public class DropOption
        {
            public GameObject showOnSelectGameObject;
            public string option;
        }

        private void Awake()
        {
            var drops = new List<TMP_Dropdown.OptionData>();
            foreach (var o in options)
            {
                drops.Add(new TMP_Dropdown.OptionData(o.option));
            }
            _dropdown.ClearOptions();
            if (drops.Count == 0)
            {
                return;
            }
            _dropdown.AddOptions(drops);
        }

        [SerializeField] private List<DropOption> options = new List<DropOption>();
        [SerializeField] private TMP_Dropdown _dropdown;
        public void DoSomething(int i)
        {
            foreach (var o in options)
            {
                o.showOnSelectGameObject.SetActive(false);
            }
            options[i].showOnSelectGameObject.SetActive(true);
        }
    }
}

