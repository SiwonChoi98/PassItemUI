using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiPopup : BasePoolObject
{
    [SerializeField] private Text _messageText;

    public void SetText(NotificationType notificationType)
    {
        _messageText.text = Utills.GetNotiPopupText(notificationType);
    }
}
