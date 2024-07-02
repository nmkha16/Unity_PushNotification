using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Cysharp.Threading.Tasks;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

namespace PushNotification.IOS
{
    public class IOSPushNotification : IPushNotification
    {
        private IList storedNotificationIds;
        public IList StoredNotificationIds { get => this.storedNotificationIds; set => this.storedNotificationIds = value; }

        public IOSPushNotification()
        {
            storedNotificationIds = new List<string>();
        }

        ~IOSPushNotification()
        {
            //     storedNotificationIds.Clear();
        }

        public async UniTask Initialize()
        {
            await RequestPermission();
        }

        public async UniTask RequestPermission()
        {
#if UNITY_IOS
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                await UniTask.WaitUntil(() => req.IsFinished);

                string res = "\n RequestAuthorization:";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                Debug.Log($"IOS: {res}");
            }
#endif
        }

        public void ScheduleNotification(string title, string body, TimeSpan when)
        {
#if UNITY_IOS
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = when,
                Repeats = true
            };

            var notification = new iOSNotification()
            {
                // You can specify a custom identifier which can be used to manage the notification later.
                // If you don't provide one, a unique string will be generated automatically.
                // Identifier = "_notification_01",
                Title = title,
                Body = body,
                // Subtitle = "This is a subtitle, something, something important...",
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "Triple Tile 3D",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };
            iOSNotificationCenter.ScheduleNotification(notification);
            string id = notification.Identifier;
            storedNotificationIds.Add(id);
#endif
        }

        public void CancelNotification(object id, bool ignoreSaveList)
        {
#if UNITY_IOS
            string ix = (string)id;
            iOSNotificationCenter.RemoveScheduledNotification(ix);
            if (!ignoreSaveList) storedNotificationIds.Remove(ix);
#endif
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                foreach (var id in storedNotificationIds)
                {
                    bw.Write((string)id);
                }
            }
        }

        public void Load(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    string id = br.ReadString();
                    this.storedNotificationIds.Add(id);
                }
            }
        }
    }
}
