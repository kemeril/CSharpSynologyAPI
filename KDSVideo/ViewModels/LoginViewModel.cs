using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using KDSVideo.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using SynologyAPI;
using SynologyRestDAL;
using Windows.UI.Xaml.Controls;

namespace KDSVideo.ViewModels
{
    public class LoginViewModel : ObservableRecipient, IDisposable, INavigable
    {
        private const string AppName = "KDSVideo";

        private readonly IAuthenticationIdProvider _authenticationIdProvider;
        private readonly IDeviceIdProvider _deviceIdProvider;
        private readonly INetworkService _networkService;
        private readonly IAutoLoginDataHandler _autoLoginDataHandler;
        private readonly IHistoricalLoginDataHandler _historicalLoginDataHandler;
        private readonly ITrustedLoginDataHandler _trustedLoginDataHandler;
        private readonly IVideoStation _videoStation;
        private readonly IMessenger _messenger;

        private bool _disposedValue;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        private readonly RelayCommand _loginCommand;
        private readonly RelayCommand _selectHistoricalLoginDataCommand;

        private string _host = string.Empty;
        private string _account = string.Empty;
        private string _password = string.Empty;
        private string _otpCode = string.Empty;

        private bool _rememberMe;
        private bool _trustThisDevice;
        private bool _showProgressIndicator;
        private bool _isEnabledCredentialsInput = true;

        private IReadOnlyCollection<HistoricalLoginData> _historicalLoginData = new List<HistoricalLoginData>().AsReadOnly();
        private HistoricalLoginData _selectedHistoricalLoginData;

        public LoginViewModel(IAuthenticationIdProvider authenticationIdProvider, IDeviceIdProvider deviceIdProvider, INetworkService networkService, IAutoLoginDataHandler autoLoginDataHandler, IHistoricalLoginDataHandler historicalLoginDataHandler, ITrustedLoginDataHandler trustedLoginDataHandler, IVideoStation videoStation, IMessenger messenger)
            : base(messenger)
        {
            _authenticationIdProvider = authenticationIdProvider ?? throw new ArgumentNullException(nameof(authenticationIdProvider));
            _deviceIdProvider = deviceIdProvider ?? throw new ArgumentNullException(nameof(deviceIdProvider));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _autoLoginDataHandler = autoLoginDataHandler ?? throw new ArgumentNullException(nameof(autoLoginDataHandler));
            _historicalLoginDataHandler = historicalLoginDataHandler ?? throw new ArgumentNullException(nameof(historicalLoginDataHandler));
            _trustedLoginDataHandler = trustedLoginDataHandler ?? throw new ArgumentNullException(nameof(trustedLoginDataHandler));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            _messenger = messenger;

            _loginCommand = new RelayCommand(Login, CanLogin);
            _selectHistoricalLoginDataCommand = new RelayCommand(SelectHistoricalLoginData, CanSelectHistoricalLoginData);

            var autoLoginData = _autoLoginDataHandler.Get();
            if (autoLoginData != null)
            {
                Host = autoLoginData.Host ?? string.Empty;
                Account = autoLoginData.Account ?? string.Empty;
                Password = autoLoginData.Password ?? string.Empty;
                RememberMe = !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Account) && !string.IsNullOrWhiteSpace(Password);
            }

            ReloadHistoricalLoginData();

            IsActive = true;
        }

        ~LoginViewModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            _messenger.Register<LogoutMessage>(this, (recipient, logoutMessage) => LogoutMessageReceived());
        }

        protected override void OnDeactivated()
        {
            _messenger.UnregisterAll(this);
        }

        private async void LogoutMessageReceived()
        {
            var cts = new CancellationTokenSource(_timeout);
            try
            {
                await _videoStation.LogoutAsync(cts.Token);
            }
            catch (Exception e)
            {
                Trace.Write($"Logout failed. Exception: {e}");
            }
        }

        private async Task<EncryptionInfoResult> GetEncryptionInfoAsync(IWebProxy proxy, Uri baseUri, string authenticationId, string deviceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var encryptionInfoInfo = await _videoStation.GetEncryptionInfoAsync(baseUri, authenticationId, deviceId, proxy, cancellationToken);
                return new EncryptionInfoResult(encryptionInfoInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());
                return new EncryptionInfoResult(ex);
            }
        }

        private async Task<LoginResult> LoginAsync(IWebProxy proxy, Uri baseUri, string username, string password, string otpCode = null, string authenticationId = null, string deviceId = null, string deviceName = null, string cipherText = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginInfo = await _videoStation.LoginAsync(baseUri, username, password, otpCode, authenticationId, deviceId, deviceName, cipherText, proxy, cancellationToken);
                var librariesInfo = await _videoStation.LibraryListAsync(cancellationToken: cancellationToken);
                var libraries = librariesInfo.Libraries.ToList().AsReadOnly();
                if (libraries.Any())
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
                    case NotSupportedException _:
                        return new LoginResult(new LoginException(ApplicationLevelErrorCodes.InvalidHost));
                    case QuickConnectLoginNotSupportedException _:
                        return new LoginResult(new LoginException(ApplicationLevelErrorCodes.QuickConnectIsNotSupported));
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

        private void ReloadHistoricalLoginData() => HistoricalLoginData = _historicalLoginDataHandler.GetAll().ToList().AsReadOnly();

        private void SaveTrustedLoginData(string deviceId)
        {
            if (!TrustThisDevice || string.IsNullOrWhiteSpace(Host) || string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            _trustedLoginDataHandler.AddOrUpdate(Host, Account, Password, deviceId);
        }

        private string GetDeviceName()
        {
            var computerName = _networkService.GetComputerName();
            if (string.IsNullOrWhiteSpace(computerName))
            {
                computerName = "UWP";
            }

            return $"{computerName}-{AppName}";
        }

        private async void Login()
        {
            ShowProgressIndicator = true;
            var deviceId = _trustedLoginDataHandler.GetDeviceId(Host, Account, Password);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = _deviceIdProvider.GetDeviceId();
            }

            var cts = new CancellationTokenSource(_timeout);
            var deviceName = GetDeviceName();
            var webProxy = _networkService.GetProxy();
            var baseUri = _networkService.GetHostUri(Host);
            var authenticationId = _authenticationIdProvider.GetNewAuthenticationId();

            _videoStation.ClearCookies();
            var encryptionInfoResult = await GetEncryptionInfoAsync(webProxy, baseUri, authenticationId, deviceId, cts.Token);
            if (!encryptionInfoResult.Success)
            {
                ShowProgressIndicator = false;
                if (!string.IsNullOrWhiteSpace(encryptionInfoResult.ErrorMessage))
                {
                    await new ErrorDialog(encryptionInfoResult.ErrorMessage).ShowAsync();
                }
                return;
            }

            var cipherText = encryptionInfoResult.EncryptionInfo.PublicKey;
            var loginResult = await LoginAsync(webProxy, baseUri, Account, Password, null, authenticationId, deviceId, deviceName, cipherText, cts.Token);

            if (loginResult.Success)
            {
                SaveAutoLogin();
                SaveHistoricalLoginData();
                SaveTrustedLoginData(deviceId);
                ReloadHistoricalLoginData();
                ShowProgressIndicator = false;
                _messenger.Send(new LoginMessage(Host, Account, loginResult.Libraries));
                return;
            }

            IsEnabledCredentialsInput = false;
            ShowProgressIndicator = false;

            try
            {
                for (; ; )
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
                            var newDeviceId = _deviceIdProvider.GetDeviceId();
                            loginResult = await LoginAsync(webProxy, baseUri, Account, Password, OtpCode, authenticationId, newDeviceId, deviceName, cipherText, cts.Token);

                            if (loginResult.Success)
                            {
                                SaveAutoLogin();
                                SaveHistoricalLoginData();
                                SaveTrustedLoginData(loginResult.LoginInfo.DeviceId);
                                ReloadHistoricalLoginData();
                                ShowProgressIndicator = false;
                                _messenger.Send(new LoginMessage(Host, Account, loginResult.Libraries));
                                return;
                            }
                            else
                            {
                                ShowProgressIndicator = false;
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
                            await new ErrorDialog(loginResult.ErrorMessage).ShowAsync();
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

        private async void SelectHistoricalLoginData()
        {
            SelectedHistoricalLoginData = null;
            var loginDialogHistoricalData = new LoginDialogHistoricalData();
            if (ContentDialogResult.Primary == await loginDialogHistoricalData.ShowAsync() && SelectedHistoricalLoginData != null)
            {
                Host = SelectedHistoricalLoginData.Host;
                Account = SelectedHistoricalLoginData.Account;
                Password = SelectedHistoricalLoginData.Password;
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

        public ICommand LoginCommand => _loginCommand;

        private bool CanSelectHistoricalLoginData() => HistoricalLoginData != null && HistoricalLoginData.Any();

        public ICommand SelectHistoricalLoginDataCommand => _selectHistoricalLoginDataCommand;

        public string Host
        {
            get => _host ?? string.Empty;
            set
            {
                SetProperty(ref _host, value ?? string.Empty);
                _loginCommand.NotifyCanExecuteChanged();
            }
        }

        public string Account
        {
            get => _account ?? string.Empty;
            set
            {
                SetProperty(ref _account, value);
                _loginCommand.NotifyCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password ?? string.Empty;
            set
            {
                SetProperty(ref _password, value);
                _loginCommand.NotifyCanExecuteChanged();
            }
        }

        public string OtpCode
        {
            get => _otpCode;
            set => SetProperty(ref _otpCode, value ?? string.Empty);
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public bool TrustThisDevice
        {
            get => _trustThisDevice;
            set => SetProperty(ref _trustThisDevice, value);
        }

        public bool ShowProgressIndicator
        {
            get => _showProgressIndicator;
            set => SetProperty(ref _showProgressIndicator, value);
        }

        public bool IsEnabledCredentialsInput
        {
            get => _isEnabledCredentialsInput;
            private set => SetProperty(ref _isEnabledCredentialsInput, value);
        }

        public IReadOnlyCollection<HistoricalLoginData> HistoricalLoginData
        {
            get => _historicalLoginData;
            private set => SetProperty(ref _historicalLoginData, value);
        }

        public HistoricalLoginData SelectedHistoricalLoginData
        {
            get => _selectedHistoricalLoginData;
            set => SetProperty(ref _selectedHistoricalLoginData, value);
        }

        public void Navigated(in object sender, in NavigationEventArgs args)
        {
            Trace.WriteLine($"Navigated. NavigationMode:{args.NavigationMode}, Parameter:{args.Parameter}");
        }
    }
}
