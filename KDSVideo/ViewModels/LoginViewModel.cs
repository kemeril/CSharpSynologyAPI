﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using KDSVideo.Infrastructure;
using KDSVideo.Views;
using SynologyAPI;
using SynologyAPI.Exception;
using SynologyRestDAL;

namespace KDSVideo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(INavigationService navigationService, IDeviceIdProvider deviceIdProvider, INetworkService networkService, IHistoricalLoginDataHandler historicalLoginDataHandler,  IVideoStation videoStation)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _historicalLoginDataHandler = historicalLoginDataHandler ?? throw new ArgumentNullException(nameof(historicalLoginDataHandler));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            NavigateCommand = new RelayCommand(() => _navigationService.NavigateTo(ViewModelLocator.MainPageKey));

            LoginCommand = new RelayCommand(Login, CanLogin);

            var lastLoginData = _historicalLoginDataHandler.GetLatest();
            if (lastLoginData != null)
            {
                Host = lastLoginData.Host ?? string.Empty;
                Account = lastLoginData.Account ?? string.Empty;
                Password = lastLoginData.Password ?? string.Empty;
                _deviceId = lastLoginData.DeviceId ?? string.Empty;
                RememberMe = true;
            }

            if (string.IsNullOrWhiteSpace(_deviceId))
            {
                _deviceId = deviceIdProvider.GetDeviceId(); // Must have a default deviceId value!
            }
        }

        private async void Login()
        {
            try
            {
                _baseUri = _networkService.GetHostUri(Host);
                if (_baseUri == null)
                {
                    return;
                }
                _webProxy = _networkService.GetProxy();
                var cts = new CancellationTokenSource(_timeout);
                ShowProgressIndicator = true;
                try
                {
                    var loginInfo = await _videoStation.LoginAsync(_baseUri, Account, Password, null, _deviceId, DeviceName, null, _webProxy, cts.Token);
                    if (RememberMe)
                    {
                        _historicalLoginDataHandler.AddOrUpdate(new HistoricalLoginData { Host = Host, Account = Account, Password = Password, DeviceId = loginInfo.DeviceId ?? _deviceId });
                    }
                }
                finally
                {
                    ShowProgressIndicator = false;
                }
                _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
            }
            catch (SynoRequestException e)
            {
                Trace.TraceInformation(e.ToString());
                if (e.ErrorCode == ErrorCodes.OneTimePasswordNotSpecified)
                {
                    var loginSuccess = await LoginDialogOtpRequestDialogShowAsync();
                    if (loginSuccess)
                    {
                        _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
                    }
                }
            }
            catch (Exception e)
            {
                // ignored (because of OperationCanceledException or other exception)
                Trace.TraceInformation(e.ToString());
            }
        }

        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Account) &&
            !string.IsNullOrWhiteSpace(Password);

        private async Task<bool> LoginDialogOtpRequestDialogShowAsync()
        {
            do
            {
                OtpCode = string.Empty;
                TrustThisDevice = false;
                var otpDialog = new LoginDialogOtpRequestDialog();
                var dialogResult = await otpDialog.ShowAsync();
                if (dialogResult == ContentDialogResult.Primary)
                {
                    var loginSuccess = await Login2FA();
                    if (loginSuccess)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            } while (true);
        }

        // ReSharper disable once InconsistentNaming
        private async Task<bool> Login2FA()
        {
            if (string.IsNullOrWhiteSpace(OtpCode) || OtpCode.Length != 6)
            {
                return false;
            }

            try
            {
                ShowProgressIndicator = true;
                var cts = new CancellationTokenSource(_timeout);
                try
                {
                    var loginInfo = await _videoStation.LoginAsync(_baseUri, Account, Password, OtpCode, deviceId: _deviceId, DeviceName, null, _webProxy, cts.Token);
                    if (TrustThisDevice)
                    {
                        _historicalLoginDataHandler.AddOrUpdate(new HistoricalLoginData { Host = Host, Account = Account, Password = Password, DeviceId = loginInfo.DeviceId });
                        _deviceId = loginInfo.DeviceId;
                    }
                }
                finally
                {
                    ShowProgressIndicator = false;
                }

                return true;
            }
            catch (Exception e)
            {
                // ignored (because of OperationCanceledException, SynoRequestException or other exception)
                Trace.TraceInformation(e.ToString());
                return false;
            }
        }

        private readonly INavigationService _navigationService;
        private readonly INetworkService _networkService;
        private readonly IHistoricalLoginDataHandler _historicalLoginDataHandler;
        private readonly IVideoStation _videoStation;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
        private const string DeviceName = "SM-T580 - DS video";

        private IWebProxy _webProxy;
        private Uri _baseUri;
        private string _deviceId = string.Empty;
        private string _host = string.Empty;
        private string _account = string.Empty;
        private string _password = string.Empty;
        private string _otpCode = string.Empty;

        private bool _rememberMe;
        private bool _trustThisDevice;
        private bool _showProgressIndicator;

        public RelayCommand NavigateCommand { get; }

        public RelayCommand LoginCommand { get; }

        public string Host
        {
            get => _host ?? string.Empty;
            set
            {
                _host = value ?? string.Empty;
                RaisePropertyChanged(nameof(Host));
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string Account
        {
            get => _account ?? string.Empty;
            set
            {
                _account = value ?? string.Empty;
                RaisePropertyChanged(nameof(Account));
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string Password
        {
            get => _password ?? string.Empty;
            set
            {
                _password = value ?? string.Empty;
                RaisePropertyChanged(nameof(Password));
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string OtpCode
        {
            get => _otpCode;
            set
            {
                _otpCode = value ?? string.Empty;
                RaisePropertyChanged(nameof(OtpCode));
            }
        }
        
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                RaisePropertyChanged(nameof(RememberMe));
            }
        }

        public bool TrustThisDevice
        {
            get => _trustThisDevice;
            set
            {
                _trustThisDevice = value;
                RaisePropertyChanged(nameof(TrustThisDevice));
            }
        }

        public bool ShowProgressIndicator
        {
            get => _showProgressIndicator;
            set
            {
                _showProgressIndicator = value;
                RaisePropertyChanged(nameof(ShowProgressIndicator));
            }
        }
    }
}