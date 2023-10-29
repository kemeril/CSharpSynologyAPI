using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyAPI;

namespace KDSVideo.ViewModels
{
    public partial class SettingsViewModel : ObservableRecipient, IDisposable
    {
        private readonly IMessenger _messenger;
        private readonly IVideoSettingsDataHandler _videoSettingsDataHandler;
        private bool _disposedValue;
        private int _playbackQuality;
        private bool _ac3PassthroughIsEnabled;

        [ObservableProperty]
        private string _account = string.Empty;

        [ObservableProperty]
        private string _host = string.Empty;

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
            _messenger.Register<LoginMessage>(this, (_, loginMessage) =>
            {
                Host = loginMessage.Host;
                Account = loginMessage.Account;
            });

            _messenger.Register<LogoutMessage>(this, (_, _) =>
            {
                Host = string.Empty;
                Account = string.Empty;
            });
        }

        protected override void OnDeactivated()
        {
            _messenger.UnregisterAll(this);
        }

        private void InitVideoSettings()
        {
            var videoSettings = _videoSettingsDataHandler.Get();
            PlaybackQuality =
                videoSettings.VideoTranscoding switch
                {
                    VideoStation.VideoTranscoding.Raw => 0,
                    VideoStation.VideoTranscoding.HighQuality => 1,
                    VideoStation.VideoTranscoding.MediumQuality => 2,
                    VideoStation.VideoTranscoding.LowQuality => 3,
                    _ => 4
                };

            Ac3PassthroughIsEnabled = videoSettings.Ac3PassThrough;
        }

        private void UpdateSettings()
        {
            var videoTranscoding = 
                PlaybackQuality switch
                {
                    0 => VideoStation.VideoTranscoding.Raw,
                    1 => VideoStation.VideoTranscoding.HighQuality,
                    2 => VideoStation.VideoTranscoding.MediumQuality,
                    3 => VideoStation.VideoTranscoding.LowQuality,
                    _ => VideoStation.VideoTranscoding.Raw
                };

            _videoSettingsDataHandler.SetOrUpdate(videoTranscoding, Ac3PassthroughIsEnabled);
        }
    }
}
