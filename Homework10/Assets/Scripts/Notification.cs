using System;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private Button _button;
    private string _idAndroidNitification = "id2";
    void Start()
    {
        _button.onClick.AddListener(NotificationMethod);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void NotificationMethod()
    {
        var androidChanel = new AndroidNotificationChannel
        {
            Id = _idAndroidNitification,
            Name = "THIS IS NAME",
            Importance = Importance.Low,
            Description = "THIS IS DESCRIPTION"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(androidChanel);

        var androidSettingsNotification = new AndroidNotification
        {
            Color = Color.red,
            RepeatInterval = TimeSpan.FromSeconds(0),
            Text = "ЕБАТЬ ТЕКСТ УВЕДОМЛЕНИЯ НАХУЙ",
            Title = "Ты нахуй на кнопку нажал?"
        };

        AndroidNotificationCenter.SendNotification(androidSettingsNotification, _idAndroidNitification);

    }
}
