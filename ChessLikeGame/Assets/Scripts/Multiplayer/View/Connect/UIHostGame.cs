using Multiplayer.Controllers;
using UnityEngine;

namespace Multiplayer.View.Connect
{
    public class UIHostGame : MonoBehaviour
    {
        [SerializeField] private GameObject createOnStartPrefab;
        private GameObject activeObject = null;
        // Start is called before the first frame update
        void Start()
        {
            WebSocketConnection.onHostGame += StartGame;
        }

        public void AskToHostGame()
        {
            FindObjectOfType<WebSocketConnection>().CreateNewRoom(2,true);
        }

        private void StartGame(bool obj)
        {
            if (activeObject != null || !obj) return;
            activeObject = Instantiate(createOnStartPrefab);
        }
        
        private void EndGame()
        {
            if (activeObject == null) return;
            Destroy(activeObject);
            activeObject = null;
        }

    }
}
