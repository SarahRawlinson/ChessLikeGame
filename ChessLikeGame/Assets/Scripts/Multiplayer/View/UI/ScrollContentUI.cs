using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.View.UI
{
    public class ScrollContentUI : MonoBehaviour
    {
        [SerializeField] ScrollRect scroll;

        public GameObject AddContent(GameObject button)
        {
            return Instantiate(button, scroll.content);
        }
    }
}
