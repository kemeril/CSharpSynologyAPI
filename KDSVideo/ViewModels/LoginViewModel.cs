using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KDSVideo.Infrastructure;
using KDSVideo.Messages;
using KDSVideo.Views;
using SynologyAPI;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SynologyAPI.SynologyRestDAL;

namespace KDSVideo.ViewModels
{
    public partial class LoginViewModel : ObservableRecipient, INavigable
    {
        private const string APP_NAME = "KDSVideo";

        private readonly IDeviceIdProvider _deviceIdProvider;
        private readonly INetworkService _networkService;
        private readonly IAutoLoginDataHandler _autoLoginDataHandler;
        private readonly IHistoricalLoginDataHandler _historicalLoginDataHandler;
        private readonly ITrustedLoginDataHandler _trustedLoginDataHandler;
        private readonly IVideoStation _videoStation;
        private readonly IMessenger _messenger;

#if DEBUG
        private readonly TimeSpan _timeout = TimeSpan.FromHours(1);
#else
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
#endif

        [ObservableProperty]
        private string _host = string.Empty;

        [ObservableProperty]
        private string _account = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _otpCode = string.Empty;

        [ObservableProperty]
        private bool _rememberMe;

        [ObservableProperty]
        private bool _trustThisDevice;

        [ObservableProperty]
        private bool _showProgressIndicator;

        [ObservableProperty]
        private bool _isEnabledCredentialsInput = true;

        [ObservableProperty]
        private List<HistoricalLoginData> _historicalLoginData = new();

        [ObservableProperty]
        private HistoricalLoginData? _selectedHistoricalLoginData;

        public LoginViewModel(IDeviceIdProvider deviceIdProvider, INetworkService networkService, IAutoLoginDataHandler autoLoginDataHandler, IHistoricalLoginDataHandler historicalLoginDataHandler, ITrustedLoginDataHandler trustedLoginDataHandler, IVideoStation videoStation, IMessenger messenger)
            : base(messenger)
        {
            _deviceIdProvider = deviceIdProvider ?? throw new ArgumentNullException(nameof(deviceIdProvider));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _autoLoginDataHandler = autoLoginDataHandler ?? throw new ArgumentNullException(nameof(autoLoginDataHandler));
            _historicalLoginDataHandler = historicalLoginDataHandler ?? throw new ArgumentNullException(nameof(historicalLoginDataHandler));
            _trustedLoginDataHandler = trustedLoginDataHandler ?? throw new ArgumentNullException(nameof(trustedLoginDataHandler));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            _messenger = messenger;

            var autoLoginData = _autoLoginDataHandler.Get();
            if (autoLoginData != null)
            {
                Host = autoLoginData.Host;
                Account = autoLoginData.Account;
                Password = autoLoginData.Password;
                RememberMe = !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Account) && !string.IsNullOrWhiteSpace(Password);
            }

            ReloadHistoricalLoginData();

            IsActive = true;
        }

        protected override void OnActivated()
        {
            _messenger.Register<LogoutMessage>(this, (_, _) => LogoutMessageReceived());
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

        private async Task<EncryptionInfoResult> GetEncryptionInfoAsync(IWebProxy proxy, Uri baseUri, CancellationToken cancellationToken = default)
        {
            try
            {
                var encryptionInfoInfo = await _videoStation.GetEncryptionInfoAsync(baseUri, proxy, cancellationToken);
                return new EncryptionInfoResult(encryptionInfoInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());
                return new EncryptionInfoResult(ex);
            }
        }

        private async Task<LoginResult> LoginAsync(IWebProxy proxy, Uri baseUri, string username, string password, string otpCode = "", string deviceId = "", string deviceName = "", string cipherText = "", CancellationToken cancellationToken = default)
        {
            try
            {
                var loginInfo = await _videoStation.LoginAsync(baseUri, username, password, otpCode, deviceId, deviceName, cipherText, proxy, cancellationToken);
                var librariesInfo = await _videoStation.LibraryListAsync(cancellationToken: cancellationToken);
                var libraries = librariesInfo.Libraries.ToList().AsReadOnly();
                if (libraries.Any())
                {
                    return new LoginResult(loginInfo, libraries);
                }

                throw new LoginException(ApplicationLevelErrorCodes.NO_VIDEO_LIBRARIES);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());

                return ex switch
                {
                    NotSupportedException _ => new LoginResult(new LoginException(ApplicationLevelErrorCodes.INVALID_HOST)),
                    QuickConnectLoginNotSupportedException _ => new LoginResult(new LoginException(ApplicationLevelErrorCodes.QUICK_CONNECT_IS_NOT_SUPPORTED)),
                    LoginException _ => new LoginResult(ex),
                    OperationCanceledException _ => new LoginResult(new LoginException(ApplicationLevelErrorCodes.OPERATION_TIME_OUT)),
                    WebException { Response: null } => new LoginResult(new LoginException(ApplicationLevelErrorCodes.CONNECTION_WITH_THE_SERVER_COULD_NOT_BE_ESTABLISHED)),
                    _ => new LoginResult(ex)
                };
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

        private void ReloadHistoricalLoginData() => HistoricalLoginData = _historicalLoginDataHandler.GetAll().ToList();

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

            return $"{computerName}-{APP_NAME}";
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task Login()
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
            Uri baseUri;
            try
            {
                baseUri = _networkService.GetHostUri(Host);
            }
            catch
            {
                await new ErrorDialog(ApplicationLevelErrorMessages.GetErrorMessage(ApplicationLevelErrorCodes.INVALID_HOST)).ShowAsync();
                return;
            }

            _videoStation.ClearCookies();
            var encryptionInfoResult = await GetEncryptionInfoAsync(webProxy, baseUri, cts.Token);
            if (!encryptionInfoResult.Success)
            {
                ShowProgressIndicator = false;
                if (!string.IsNullOrWhiteSpace(encryptionInfoResult.ErrorMessage))
                {
                    await new ErrorDialog(encryptionInfoResult.ErrorMessage).ShowAsync();
                }
                return;
            }

            var cipherText = encryptionInfoResult.EncryptionInfo?.PublicKey;
            var loginResult = await LoginAsync(webProxy, baseUri, Account, Password, "", deviceId!, deviceName, cipherText!, cts.Token);

            if (loginResult.Success)
            {
                SaveAutoLogin();
                SaveHistoricalLoginData();
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

                            loginResult = await LoginAsync(webProxy, baseUri, Account, Password, OtpCode, "", deviceName, cipherText!, cts.Token);

                            if (loginResult.Success)
                            {
                                SaveAutoLogin();
                                SaveHistoricalLoginData();

                                SaveTrustedLoginData(loginResult.LoginInfo!.DeviceId);

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

        [RelayCommand(CanExecute = nameof(CanSelectHistoricalLoginData))]
        private async Task SelectHistoricalLoginData()
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
            var host = Host.Trim();
            return host.Length <= 255 && !string.IsNullOrWhiteSpace(host)
                && !host.Any(char.IsWhiteSpace) && host.All(c => c != ' ');
        }

        private bool AccountIsValid()
        {
            var account = Account.Trim();
            return account.Length <= 64 && !string.IsNullOrWhiteSpace(account)
                && !account.Any(char.IsWhiteSpace) && account.All(c => c != ' ');
        }

        private bool PasswordIsValid()
        {
            // Remark: Space character is allowed in the password!
            var password = Password;
            return password.Length <= 127 && !string.IsNullOrWhiteSpace(password)
                && !password.Any(char.IsWhiteSpace);
        }

        private bool CanLogin() => HostIsValid() && AccountIsValid() && PasswordIsValid();

        private bool CanSelectHistoricalLoginData() => HistoricalLoginData.Any();

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnHostChanged(string value) => LoginCommand.NotifyCanExecuteChanged();

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnAccountChanged(string value) => LoginCommand.NotifyCanExecuteChanged();

        // ReSharper disable once UnusedParameterInPartialMethod
        partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();

        public void Navigated(in object sender, in NavigationEventArgs args)
        {
            Trace.WriteLine($"Navigated. NavigationMode:{args.NavigationMode}, Parameter:{args.Parameter}");
        }
    }
}
