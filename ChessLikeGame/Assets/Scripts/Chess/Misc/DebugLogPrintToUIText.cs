using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Chess.Misc
{
    public class DebugLogPrintToUIText : MonoBehaviour
    {
        // Adjust via the Inspector
        public int maxLines = 8;
        private Queue<string> queue = new Queue<string>();
        private string currentText = "";
        [SerializeField] TMP_Text debugText;

        private void Update()
        {
            debugText.text = "Debug Log: " + currentText;
        }

        void OnEnable()
        {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            // Delete oldest message
            if (queue.Count >= maxLines) queue.Dequeue();

            queue.Enqueue(logString);

            var builder = new StringBuilder();
            foreach (string st in queue)
            {
                builder.Append(st).Append("\n");
            }

            currentText = builder.ToString();
        }
    }
}
