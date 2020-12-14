﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using KDSVideo.Views;
using SynologyAPI;
using SynologyRestDAL;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KDSVideo.ViewModels
{
    public class LoginViewModel : ViewModelBase, INavigable
    {
        private const string DeviceName = "UWP - KDS video";

        private readonly INavigationServiceEx _navigationService;
        private readonly IDeviceIdProvider _deviceIdProvider;
        private readonly INetworkService _networkService;
        private readonly IAutoLoginDataHandler _autoLoginDataHandler;
        private readonly IHistoricalLoginDataHandler _historicalLoginDataHandler;
        private readonly ITrustedLoginDataHandler _trustedLoginDataHandler;
        private readonly IVideoStation _videoStation;
        private readonly IMessenger _messenger;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
        private IWebProxy _webProxy;

        private string _host = string.Empty;
        private string _account = string.Empty;
        private string _password = string.Empty;
        private string _otpCode = string.Empty;

        private bool _rememberMe;
        private bool _trustThisDevice;
        private bool _showProgressIndicator;
        private bool _isEnabledCredentialsInput = true;

        public LoginViewModel(INavigationServiceEx navigationService, IDeviceIdProvider deviceIdProvider, INetworkService networkService, IAutoLoginDataHandler autoLoginDataHandler,  IHistoricalLoginDataHandler historicalLoginDataHandler,  ITrustedLoginDataHandler trustedLoginDataHandler,  IVideoStation videoStation, IMessenger messenger)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _deviceIdProvider = deviceIdProvider ?? throw new ArgumentNullException(nameof(deviceIdProvider));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _autoLoginDataHandler = autoLoginDataHandler ?? throw new ArgumentNullException(nameof(autoLoginDataHandler));
            _historicalLoginDataHandler = historicalLoginDataHandler ?? throw new ArgumentNullException(nameof(historicalLoginDataHandler));
            _trustedLoginDataHandler = trustedLoginDataHandler ?? throw new ArgumentNullException(nameof(trustedLoginDataHandler));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

            LoginCommand = new RelayCommand(Login, CanLogin);

            var autoLoginData = _autoLoginDataHandler.Get();
            if (autoLoginData != null)
            {
                Host = autoLoginData.Host ?? string.Empty;
                Account = autoLoginData.Account ?? string.Empty;
                Password = autoLoginData.Password ?? string.Empty;
                RememberMe = !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Account) && !string.IsNullOrWhiteSpace(Password);
            }
        }

        private async Task<LoginResult> LoginAsync(string host, string username, string password, string otpCode = null, string deviceId = null, string deviceName = null, string cipherText = null, IWebProxy proxy = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _webProxy = _networkService.GetProxy();
                var baseUri = _networkService.GetHostUri(host);
                if (baseUri == null)
                {
                    throw new LoginException(ApplicationLevelErrorCodes.InvalidHost);
                }

                var loginInfo = await _videoStation.LoginAsync(baseUri, username, password, otpCode, deviceId, deviceName, cipherText, proxy, cancellationToken);
                var librariesInfo = await _videoStation.LibraryListAsync(0, -1, cancellationToken);
                var libraries = librariesInfo.Libraries.ToList().AsReadOnly();
                if (librariesInfo.Libraries.Any())
                {
                    return new LoginResult(loginInfo, libraries);
                }

                throw new LoginException(ApplicationLevelErrorCodes.NoVideoLibraries);
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());

                switch (ex)
                {
                    case LoginException _:
                        return new LoginResult(ex);
                    case OperationCanceledException _:
                        return new LoginResult(new LoginException(ApplicationLevelErrorCodes.OperationTimeOut));
                    case WebException webException when webException.Response == null:
                        return new LoginResult(new LoginException(ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished));
                    default:
                        return new LoginResult(ex);
                }
            }
        }

        private void SaveAutoLogin()
        {
            if (!RememberMe || string.IsNullOrWhiteSpace(Host) || string.IsNullOrWhiteSpace(Account))
            {
                return;
            }

            _autoLoginDataHandler.SetOrUpdate(Host, Account, Password);
        }

        private void SaveHistoricalLoginData()
        {
            if (!RememberMe || string.IsNullOrWhiteSpace(Host) || string.IsNullOrWhiteSpace(Account))
            {
                return;
            }

            _historicalLoginDataHandler.AddOrUpdate(Host, Account, Password);
        }

        private void SaveTrustedLoginData(string deviceId)
        {
            if (!TrustThisDevice || string.IsNullOrWhiteSpace(Host) || string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }
            
            _trustedLoginDataHandler.AddOrUpdate(Host, Account, Password, deviceId);
        }

        private async void Login()
        {
            ShowProgressIndicator = true;
            var deviceId = _trustedLoginDataHandler.GetDeviceId(Host, Account, Password);
            var cts = new CancellationTokenSource(_timeout);
            var loginResult = await LoginAsync(Host, Account, Password, null, deviceId, DeviceName, null, _webProxy, cts.Token);

            if (loginResult.Success)
            {
                SaveAutoLogin();
                SaveHistoricalLoginData();
                SaveTrustedLoginData(deviceId);
                ShowProgressIndicator = false;
                _messenger.Send(new LoginMessage(Account, loginResult.Libraries));
                return;
            }

            IsEnabledCredentialsInput = false;
            ShowProgressIndicator = false;

            try
            {
                for (;;)
                {
                    if (loginResult.ErrorCode == ErrorCodes.OneTimePasswordNotSpecified || loginResult.ErrorCode == ErrorCodes.OneTimePasswordAuthenticateFailed)
                    {
                        _trustedLoginDataHandler.RemoveIfExist(Host, Account);
                        OtpCode = string.Empty;
                        TrustThisDevice = false;
                        var otpDialog = new LoginDialogOtpRequestDialog();
                        var dialogResult = await otpDialog.ShowAsync();
                        if (dialogResult == ContentDialogResult.Primary
                            && !string.IsNullOrWhiteSpace(OtpCode) && OtpCode.Length == 6)
                        {
                            ShowProgressIndicator = true;
                            cts = new CancellationTokenSource(_timeout);
                            loginResult = await LoginAsync(Host, Account, Password, OtpCode, _deviceIdProvider.GetDeviceId(), DeviceName, null, _webProxy, cts.Token);
                            ShowProgressIndicator = false;
                            if (loginResult.Success)
                            {
                                SaveAutoLogin();
                                SaveHistoricalLoginData();
                                SaveTrustedLoginData(loginResult.LoginInfo.DeviceId);
                                _messenger.Send(new LoginMessage(Account, loginResult.Libraries));
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(loginResult.ErrorMessage))
                        {
                            // TODO: Display error message to the user
                        }

                        break;
                    }
                }
            }
            finally
            {
                IsEnabledCredentialsInput = true;
            }
        }

        private bool HostIsValid()
        {
            var host = (Host ?? string.Empty).Trim();
            return host.Length <= 255 && !string.IsNullOrWhiteSpace(host)
                && !host.Any(char.IsWhiteSpace) && host.All(c => c != ' ');
        }

        private bool AccountIsValid()
        {
            var account = (Account ?? string.Empty).Trim();
            return account.Length <= 64 && !string.IsNullOrWhiteSpace(account)
                && !account.Any(char.IsWhiteSpace) && account.All(c => c != ' ');
        }

        private bool PasswordIsValid()
        {
            // Remark: Space character is allowed in the password!
            var password = Password ?? string.Empty;
            return password.Length <= 127 && !string.IsNullOrWhiteSpace(password)
                && !password.Any(char.IsWhiteSpace);
        }

        private bool CanLogin() => HostIsValid() && AccountIsValid() && PasswordIsValid();

        public RelayCommand LoginCommand { get; }

        public string Host
        {
            get => _host ?? string.Empty;
            set
            {
                Set(nameof(Host), ref _host, value ?? string.Empty);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string Account
        {
            get => _account ?? string.Empty;
            set
            {
                Set(nameof(Account), ref _account, value ?? string.Empty);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string Password
        {
            get => _password ?? string.Empty;
            set
            {
                Set(nameof(Password), ref _password, value ?? string.Empty);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string OtpCode
        {
            get => _otpCode;
            set => Set(nameof(OtpCode), ref _otpCode, value ?? string.Empty);
        }
        
        public bool RememberMe
        {
            get => _rememberMe;
            set => Set(nameof(RememberMe), ref _rememberMe, value);
        }

        public bool TrustThisDevice
        {
            get => _trustThisDevice;
            set => Set(nameof(TrustThisDevice), ref _trustThisDevice, value);
        }

        public bool ShowProgressIndicator
        {
            get => _showProgressIndicator;
            set => Set(nameof(ShowProgressIndicator), ref _showProgressIndicator, value);
        }

        public bool IsEnabledCredentialsInput
        {
            get => _isEnabledCredentialsInput;
            private set => Set(nameof(IsEnabledCredentialsInput), ref _isEnabledCredentialsInput, value);
        }

        public void Navigated(in object sender, in NavigationEventArgs e)
        {
            Trace.WriteLine($"Navigated. NavigationMode:{e.NavigationMode}, Parameter:{e.Parameter}");
        }
    }
}