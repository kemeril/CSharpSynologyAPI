using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyAPI;

namespace KDSVideo.ViewModels
{
    public class SettingsViewModel : ObservableRecipient, IDisposable
    {
        private readonly IMessenger _messenger;
        private readonly IVideoSettingsDataHandler _videoSettingsDataHandler;
        private bool _disposedValue;
        private string _account = string.Empty;
        private string _host = string.Empty;
        private int _playbackQuality;
        private bool _ac3PassthroughIsEnabled;

        public SettingsViewModel(IMessenger messenger, IApplicationInfoService applicationInfoService, IVideoSettingsDataHandler videoSettingsDataHandler)
            : base(messenger)
        {
            _messenger = messenger;
            _videoSettingsDataHandler = videoSettingsDataHandler ?? throw new ArgumentNullException(nameof(videoSettingsDataHandler));
            ApplicationVersion = applicationInfoService.GetAppVersion();

            InitVideoSettings();
            IsActive = true;
        }

        ~SettingsViewModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public string ApplicationVersion { get; }

        public string Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public int PlaybackQuality
        {
            get => _playbackQuality;
            set
            {
                SetProperty(ref _playbackQuality, value);
                UpdateSettings();
            }
        }

        public bool Ac3PassthroughIsEnabled
        {
            get => _ac3PassthroughIsEnabled;
            set
            {
                SetProperty(ref _ac3PassthroughIsEnabled, value);
                UpdateSettings();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    IsActive = false;
                }

                _disposedValue = true;
            }
        }

        protected override void OnActivated()
        {
            _messenger.Register<LoginMessage>(this, (recipient, loginMessage) => LoginMessageReceived(loginMessage));
            _messenger.Register<LogoutMessage>(this, (recipient, logoutMessage) => LogoutMessageReceived());
        }

        protected override void OnDeactivated()
        {
            _messenger.UnregisterAll(this);
        }

        private void LoginMessageReceived(LoginMessage loginMessage)
        {
            Host = loginMessage.Host;
            Account = loginMessage.Account;
        }

        private void LogoutMessageReceived()
        {
            Host = string.Empty;
            Account = string.Empty;
        }

        private void InitVideoSettings()
        {
            var videoSettings = _videoSettingsDataHandler.Get();
            switch (videoSettings.VideoTranscoding)
            {
                case VideoStation.VideoTranscoding.Raw:
                    PlaybackQuality = 0;
                    break;
                case VideoStation.VideoTranscoding.HighQuality:
                    PlaybackQuality = 1;
                    break;
                case VideoStation.VideoTranscoding.MediumQuality:
                    PlaybackQuality = 2;
                    break;
                case VideoStation.VideoTranscoding.LowQuality:
                    PlaybackQuality = 3;
                    break;
                default:
                    PlaybackQuality = 4;
                    break;
            }

            Ac3PassthroughIsEnabled = videoSettings.Ac3PassThrough;
        }

        private void UpdateSettings()
        {
            VideoStation.VideoTranscoding videoTranscoding;
            switch (PlaybackQuality)
            {
                case 0:
                    videoTranscoding = VideoStation.VideoTranscoding.Raw;
                    break;
                case 1:
                    videoTranscoding = VideoStation.VideoTranscoding.HighQuality;
                    break;
                case 2:
                    videoTranscoding = VideoStation.VideoTranscoding.MediumQuality;
                    break;
                case 3:
                    videoTranscoding = VideoStation.VideoTranscoding.LowQuality;
                    break;
                default:
                    videoTranscoding = VideoStation.VideoTranscoding.Raw;
                    break;
            }

            _videoSettingsDataHandler.SetOrUpdate(videoTranscoding, Ac3PassthroughIsEnabled);
        }
    }
}
