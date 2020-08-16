using System;
using Android.App;
using Android.Content;
using Android.Util;
using Huawei.Hms.Push;

namespace HmsDemo
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.huawei.push.action.MESSAGING_EVENT" })]
    public class MyMessagingService : HmsMessageService
    {
        private const string TAG = "MyMessagingService";
        private const string PushDemoAction = "com.johnthiriet.hmsdemo.action";

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);

            Log.Info(TAG, $"Push token: {token}");

            Xamarin.Essentials.Preferences.Set("PushToken", token);

            if (!string.IsNullOrEmpty(token))
            {
                RefreshTokenToServer();

                Intent intent = new Intent();
                intent.SetAction(PushDemoAction);
                intent.PutExtra("method", "onNewToken");
                intent.PutExtra("msg", "onNewToken called, token: " + token);

                SendBroadcast(intent);
            }
        }

        private void RefreshTokenToServer()
        {
            Log.Info(TAG, "Sending token to server. token: {token}");
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            Log.Info(TAG, "onMessageReceived is called");
            if (message == null)
            {
                Log.Error(TAG, "Received message entity is null!");
                return;
            }

            Log.Info(TAG, "getCollapseKey: " + message.CollapseKey
                    + "\n getData: " + message.Data
                    + "\n getFrom: " + message.From
                    + "\n getTo: " + message.To
                    + "\n getMessageId: " + message.MessageId
                    + "\n getOriginalUrgency: " + message.OriginalUrgency
                    + "\n getUrgency: " + message.Urgency
                    + "\n getSendTime: " + message.SentTime
                    + "\n getMessageType: " + message.MessageType
                    + "\n getTtl: " + message.Ttl);

            RemoteMessage.Notification notification = message.GetNotification();
            if (notification != null)
            {
                Log.Info(TAG, "\n getImageUrl: " + notification.ImageUrl
                        + "\n getTitle: " + notification.Title
                        + "\n getTitleLocalizationKey: " + notification.TitleLocalizationKey
                        + "\n getBody: " + notification.Body
                        + "\n getBodyLocalizationKey: " + notification.BodyLocalizationKey
                        + "\n getIcon: " + notification.Icon
                        + "\n getSound: " + notification.Sound
                        + "\n getTag: " + notification.Tag
                        + "\n getColor: " + notification.Color
                        + "\n getClickAction: " + notification.ClickAction
                        + "\n getChannelId: " + notification.ChannelId
                        + "\n getLink: " + notification.Link
                        + "\n getNotifyId: " + notification.NotifyId);
            }

            Intent intent = new Intent();
            intent.SetAction(PushDemoAction);
            intent.PutExtra("method", "onMessageReceived");
            intent.PutExtra("msg", "onMessageReceived called, message id:" + message.MessageId + ", payload data:"
                    + message.Data);

            SendBroadcast(intent);

            bool judgeWhetherIn10s = false;

            // If the messages are not processed in 10 seconds, the app needs to use WorkManager for processing.
            if (judgeWhetherIn10s)
            {
                StartWorkManagerJob(message);
            }
            else
            {
                // Process message within 10s
                ProcessWithin10s(message);
            }
        }

        public override void OnMessageSent(string msgId)
        {
            Log.Info(TAG, "onMessageSent called, Message id:" + msgId);
            Intent intent = new Intent();
            intent.SetAction(PushDemoAction);
            intent.PutExtra("method", "onMessageSent");
            intent.PutExtra("msg", "onMessageSent called, Message id:" + msgId);

            SendBroadcast(intent);
        }

        private void StartWorkManagerJob(RemoteMessage message)
        {
            Log.Info(TAG, "Start new Job processing.");
        }
        private void ProcessWithin10s(RemoteMessage message)
        {
            Log.Info(TAG, "Processing now.");
        }

        public override void OnSendError(string msgId, Java.Lang.Exception exception)
        {
            Log.Info(TAG, "onSendError called, message id:" + msgId + ", ErrCode:"
                    + ((SendException)exception).ErrorCode + ", description:" + exception.Message);

            Intent intent = new Intent();
            intent.SetAction(PushDemoAction);
            intent.PutExtra("method", "onSendError");
            intent.PutExtra("msg", "onSendError called, message id:" + msgId + ", ErrCode:"
                + ((SendException)exception).ErrorCode + ", description:" + exception.Message);

            SendBroadcast(intent);
        }
    }
}
