using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.Notifications;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

namespace PushNotification.Android
{
    public class AndroidPushNotification : IPushNotification
    {
        private IList storedNotificationIds;
        public const string CHANNEL_ID = "MLGAME_ID";

        public IList StoredNotificationIds { get => this.storedNotificationIds; set => this.storedNotificationIds = value; }

        public AndroidPushNotification()
        {
            this.storedNotificationIds = new List<int>();
        }

        ~AndroidPushNotification()
        {
            // this.storedNotificationIds.Clear();
        }

        public async UniTask Initialize()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.Initialize();
            var group = new AndroidNotificationChannelGroup()
            {
                Id = "Main",
                Name = "Main notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannelGroup(group);
            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = "Triple Tile 3D",
                Importance = Importance.Default,
                Description = "Generic notifications",
                Group = "Main",  // must be same as Id of previously registered group
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            await RequestPermission();
            AndroidNotificationCenter.CancelNotification(101);
#endif
        }

        public async UniTask RequestPermission()
        {
#if UNITY_ANDROID

            var request = new PermissionRequest();
            await UniTask.WaitUntil(() => request.Status != PermissionStatus.RequestPending);
            // switch (request.Status)
            // {
            //     case PermissionStatus.Allowed:
            //         break;
            //     case PermissionStatus.Denied:
            //         break;
            //     case PermissionStatus.DeniedDontAskAgain:
            //         break;
            //     case PermissionStatus.NotificationsBlockedForApp:
            //         break;
            // }

            Debug.Log($"Android: User has {request.Status} Push Notification!!!");
#endif
        }

        public void ScheduleNotification(string title, string body, TimeSpan when)
        {
#if UNITY_ANDROID
            var notification = new AndroidNotification()
            {
                Title = title,
                Text = body,
                FireTime = DateTime.Now + when,
                RepeatInterval = when
            };

            int id = AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);

            this.storedNotificationIds.Add(id);
#endif
        }

        public void CancelNotification(object id, bool ignoreSaveList)
        {
            int ix = (int)id;
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelNotification(ix);
#endif
            if (!ignoreSaveList) storedNotificationIds.Remove(ix);
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                foreach (var id in storedNotificationIds)
                {
                    bw.Write((int)id);
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
                    int id = br.ReadInt32();
                    this.storedNotificationIds.Add(id);
                }
            }
        }
    }
}