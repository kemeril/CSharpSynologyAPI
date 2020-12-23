﻿using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using SynologyAPI;

namespace KDSVideo.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IDisposable
    {
        private readonly IMessenger _messenger;
        private readonly IVideoSettingsDataHandler _videoSettingsDataHandler;
        private bool _disposedValue;
        private string _account = string.Empty;
        private string _host = string.Empty;
        private int _playbackQuality;
        private bool _ac3PassthroughIsEnabled;

        public SettingsViewModel(IMessenger messenger, IApplicationInfoService applicationInfoService, IVideoSettingsDataHandler videoSettingsDataHandler)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _videoSettingsDataHandler = videoSettingsDataHandler ?? throw new ArgumentNullException(nameof(videoSettingsDataHandler));
            ApplicationVersion = applicationInfoService.GetAppVersion();

            if (IsInDesignModeStatic)
            {
                _account = "video.station.dev";
                _host = "192.168.0.1";
            }

            InitVideoSettings();
            RegisterMessages();
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
            set => Set(nameof(Account), ref _account, value);
        }

        public string Host
        {
            get => _host;
            set => Set(nameof(Host), ref _host, value);
        }

        public int PlaybackQuality
        {
            get => _playbackQuality;
            set
            {
                Set(nameof(PlaybackQuality), ref _playbackQuality, value);
                UpdateSettings();
            }
        }

        public bool Ac3PassthroughIsEnabled
        {
            get => _ac3PassthroughIsEnabled;
            set
            {
                Set(nameof(Ac3PassthroughIsEnabled), ref _ac3PassthroughIsEnabled, value);
                UpdateSettings();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    UnregisterMessages();
                }

                _disposedValue = true;
            }
        }

        private void RegisterMessages()
        {
            _messenger.Register<LoginMessage>(this, LoginMessageReceived);
            _messenger.Register<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void UnregisterMessages()
        {
            _messenger.Unregister<LoginMessage>(this, LoginMessageReceived);
            _messenger.Unregister<LogoutMessage>(this, LogoutMessageReceived);
        }

        private void LoginMessageReceived(LoginMessage loginMessage)
        {
            Host = loginMessage.Host;
            Account = loginMessage.Account;
        }

        private void LogoutMessageReceived(LogoutMessage logoffMessage)
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