using System;
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
using SynologyRestDAL;

namespace KDSVideo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(INavigationService navigationService, IDeviceIdProvider deviceIdProvider, INetworkService networkService, ITrustedLoginDataHandler trustedlLoginDataHandler,  IVideoStation videoStation)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _deviceIdProvider = deviceIdProvider ?? throw new ArgumentNullException(nameof(deviceIdProvider));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _trustedLoginDataHandler = trustedlLoginDataHandler ?? throw new ArgumentNullException(nameof(trustedlLoginDataHandler));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            NavigateCommand = new RelayCommand(() => _navigationService.NavigateTo(ViewModelLocator.MainPageKey));

            LoginCommand = new RelayCommand(Login, CanLogin);

            var lastLoginData = _trustedLoginDataHandler.GetLatest();
            if (lastLoginData != null)
            {
                Host = lastLoginData.Host ?? string.Empty;
                Account = lastLoginData.Account ?? string.Empty;
                Password = lastLoginData.Password ?? string.Empty;
                RememberMe = true;
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
                return new LoginResult(loginInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());

                switch (ex)
                {
                    case OperationCanceledException _:
                        return new LoginResult(new LoginException(ApplicationLevelErrorCodes.OperationTimeOut));
                    case WebException webException when webException.Response == null:
                        return new LoginResult(new LoginException(ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished));
                    default:
                        return new LoginResult(ex);
                }
            }
        }

        private void SaveTrustedLoginData(string deviceId)
        {
            if (!string.IsNullOrWhiteSpace(Account))
            {
                _trustedLoginDataHandler.AddOrUpdate(RememberMe
                    ? new TrustedLoginData
                    {
                        Host = Host,
                        Account = Account,
                        Password = Password,
                        DeviceId = deviceId
                    }
                    : new TrustedLoginData
                        { Host = Host, Account = Account, Password = string.Empty, DeviceId = string.Empty });
            }
        }

        private async void Login()
        {
            ShowProgressIndicator = true;
            var deviceId = _trustedLoginDataHandler.Get(Host, Account, Password)?.DeviceId;
            var cts = new CancellationTokenSource(_timeout);
            var loginResult = await LoginAsync(Host, Account, Password, null, deviceId, DeviceName, null, _webProxy, cts.Token);
            if (loginResult.Success)
            {
                SaveTrustedLoginData(deviceId);
                ShowProgressIndicator = false;
                _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
                return;
            }

            IsEnabledCredentialsInput = false;
            ShowProgressIndicator = false;

            try
            {
                for (;;)
                {
                    if (loginResult.ErrorCode == ErrorCodes.OneTimePasswordNotSpecified ||
                        loginResult.ErrorCode == ErrorCodes.OneTimePasswordAuthenticateFailed)
                    {
                        OtpCode = string.Empty;
                        TrustThisDevice = false;
                        var otpDialog = new LoginDialogOtpRequestDialog();
                        var dialogResult = await otpDialog.ShowAsync();
                        if (dialogResult == ContentDialogResult.Primary
                            && !string.IsNullOrWhiteSpace(OtpCode) && OtpCode.Length == 6)
                        {
                            ShowProgressIndicator = true;
                            cts = new CancellationTokenSource(_timeout);
                            loginResult = await LoginAsync(Host, Account, Password, OtpCode,
                                _deviceIdProvider.GetDeviceId(), DeviceName, null, _webProxy, cts.Token);
                            ShowProgressIndicator = false;
                            if (loginResult.Success)
                            {
                                SaveTrustedLoginData(TrustThisDevice
                                    ? loginResult.LoginInfo.DeviceId
                                    : string.Empty);
                                _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
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

        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Account) &&
            !string.IsNullOrWhiteSpace(Password);

        private readonly INavigationService _navigationService;
        private readonly IDeviceIdProvider _deviceIdProvider;
        private readonly INetworkService _networkService;
        private readonly ITrustedLoginDataHandler _trustedLoginDataHandler;
        private readonly IVideoStation _videoStation;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
        private const string DeviceName = "SM-T580 - DS video";

        private IWebProxy _webProxy;
        private string _host = string.Empty;
        private string _account = string.Empty;
        private string _password = string.Empty;
        private string _otpCode = string.Empty;

        private bool _rememberMe;
        private bool _trustThisDevice;
        private bool _showProgressIndicator;
        private bool _isEnabledCredentialsInput = true;

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

        public bool IsEnabledCredentialsInput
        {
            get => _isEnabledCredentialsInput;
            private set
            {
                _isEnabledCredentialsInput = value;
                RaisePropertyChanged(nameof(IsEnabledCredentialsInput));
            }
        }
    }
}