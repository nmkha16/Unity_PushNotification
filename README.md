# Unity_PushNotification

Interface
```cs

public interface IPushNotification
{
        public IList StoredNotificationIds { get; set; }
        public UniTask Initialize();
        public UniTask RequestPermission();
        public void ScheduleNotification(string title, string body, TimeSpan when);
        public void CancelNotification(object id, bool ignoreSaveList = false);
        public void Save(string path);
        public void Load(string path);
}

```

How to use

Create
```cs

        IPushNotification platformPushNotification = null;
#if UNITY_ANDROID
        platformPushNotification = new AndroidPushNotification();
#elif UNITY_IOS
        platformPushNotification = new IOSPushNotification();
#endif
        PushNotificationService pushNotificationService = new PushNotificationService(platformPushNotification);

```

Usage

```cs

private async void ConfiguratePushNotification()
  {
      await pushNotificationService.Initalize();
      pushNotificationService.ScheduleNotification("Where have you been? We've been missing you!", "Tap to play now", new System.TimeSpan(24, 0, 0));
      pushNotificationService.SaveNotification();
  }

```
