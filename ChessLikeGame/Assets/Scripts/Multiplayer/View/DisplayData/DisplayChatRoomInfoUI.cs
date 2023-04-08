using System.Collections;
using System.Collections.Generic;
using LibObjects;
using TMPro;
using UnityEngine;

public class DisplayChatRoomInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text roomCreatorName;
    [SerializeField] private TMP_Text roomCreatedDate;
    [SerializeField] private TMP_Text roomGuid;
    [SerializeField] private TMP_Text roomLimit;
    [SerializeField] private TMP_Text roomPublic;
    [SerializeField] private TMP_Text roomLocked;
    [SerializeField] private TMP_Text roomMeta;

    public void SetData(Room room)
    {
        roomName.text = $"Room Name: {room.GetRoomName()}";
        roomCreatorName.text = $"Creator Name: {room.GetCreator()}";
        roomCreatedDate.text = $"Created Date: {room.GetCreationDate().ToString("MM/dd/yyyy h:mm:ss tt zz")}";
        roomGuid.text = $"Room Guid: {room.GetGuid().ToString()}";
        roomLimit.text = $"Room Limit: {room.GetRoomLimit().ToString()}";
        roomPublic.text = $"Room Is Public: {(room.GetAccessLevel() ? "Yes" : "No")}";
        roomLocked.text = $"Room Is Locked: {(room.GetIsRoomLocked() ? "Yes" : "No")}";
        roomMeta.text = $"Room Meta: {room.GetMeta()}";
    }
    
}
