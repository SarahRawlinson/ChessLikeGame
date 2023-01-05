using UnityEngine;

namespace Chess.Misc
{
    [DisallowMultipleComponent]
    public class GameSpeed : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] float gameSpeed;


        // Update is called once per frame
        void Update()
        {
            Time.timeScale = gameSpeed;
        }
    }
}
