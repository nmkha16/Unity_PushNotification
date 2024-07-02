using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.Notifications;
using UnityEngine;

namespace PushNotification
{
    public class PushNotificationService
    {
        private const string DEFAULT_FILE_NAME = "notifications.bin";
        private readonly IPushNotification pushNotification;
        public PushNotificationService(IPushNotification pushNotification)
        {
            this.pushNotification = pushNotification;
            LoadNotifications();
            this.CancelAllPreviousNotification();
            // pushNotification.Initialize();
        }

        public async UniTask Initalize()
        {
            await pushNotification.Initialize();
        }

        public void ScheduleNotification(string title, string body, TimeSpan when)
        {
            pushNotification.ScheduleNotification(title, body, when);
        }

        public void CancelAllPreviousNotification()
        {
            IList list = pushNotification.StoredNotificationIds;

            foreach (var id in list)
            {
                pushNotification.CancelNotification(id, true);
            }
            pushNotification.StoredNotificationIds.Clear();
        }

        private void LoadNotifications()
        {
            string pathToFile = Path.Combine(Application.persistentDataPath, DEFAULT_FILE_NAME);
            this.pushNotification.Load(pathToFile);
        }

        public void SaveNotification()
        {
            string pathToFile = Path.Combine(Application.persistentDataPath, DEFAULT_FILE_NAME);
            this.pushNotification.Save(pathToFile);
        }
    }
}
