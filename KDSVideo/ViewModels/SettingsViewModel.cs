using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyAPI;

namespace KDSVideo.ViewModels
{
    public partial class SettingsViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IVideoSettingsDataHandler _videoSettingsDataHandler;

        [ObservableProperty]
        private string _applicationVersion = string.Empty;

        [ObservableProperty]
        private string _account = string.Empty;

        [ObservableProperty]
        private string _host = string.Empty;

        [ObservableProperty]
        private int _playbackQuality;

        [ObservableProperty]
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

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnPlaybackQualityChanged(int value) => UpdateSettings();

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnAc3PassthroughIsEnabledChanged(bool value) => UpdateSettings();

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

        private void InitVideoSettings()
        {
            var videoSettings = _videoSettingsDataHandler.Get();
            PlaybackQuality = videoSettings.VideoTranscoding switch
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
            var videoTranscoding = PlaybackQuality switch
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
