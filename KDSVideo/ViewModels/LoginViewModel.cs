using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using KDSVideo.Infrastructure;
using SynologyAPI;
using SynologyAPI.Exception;
using SynologyRestDAL;

namespace KDSVideo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(INavigationService navigationService, INetworkService networkService, IVideoStation videoStation)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            NavigateCommand = new RelayCommand(() => _navigationService.NavigateTo(ViewModelLocator.MainPageKey));

            LoginCommand = new RelayCommand(Login, CanLogin);
            LoginTwoFactorAuthenticationCommand = new RelayCommand(LoginTwoFactorAuthentication, CanLoginTwoFactorAuthentication);
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
                var deviceId = LoadDeviceId(Host, Account, Password);
                var cts = new CancellationTokenSource(_timeout);
                var loginInfo = await _videoStation.LoginAsync(_baseUri, Account, Password, null, deviceId, _webProxy, cts.Token);
                if (RememberMe && !string.IsNullOrWhiteSpace(loginInfo.DeviceId))
                {
                    SaveDeviceId(Host, Account, Password, loginInfo.DeviceId);
                }
                _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
            }
            catch (SynoRequestException e)
            {
                OtpCode = string.Empty;
                if (e.ErrorCode == ErrorCodes.OneTimePasswordNotSpecified)
                {
                    // TODO: Request new 6 digits OTP CODE view state
                }
                else
                {
                    Trace.TraceInformation(e.ToString());
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

        private async void LoginTwoFactorAuthentication()
        {
            try
            {
                var cts = new CancellationTokenSource(_timeout);
                var loginInfo = await _videoStation.LoginAsync(_baseUri, Account, Password, OtpCode, null, _webProxy, cts.Token);
                if (RememberMe)
                {
                    SaveDeviceId(Host, Account, Password, loginInfo.DeviceId);
                }
                _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
            }
            catch (SynoRequestException e)
            {
                // ignored (because of invalid 6 digits OTP CODE)
                if (e.ErrorCode == ErrorCodes.OneTimePasswordNotSpecified)
                {
                    // ignored (because of invalid 6 digits OTP CODE)
                }
                else
                {
                    Trace.TraceInformation(e.ToString());
                }
            }
            catch (Exception e)
            {
                // ignored (because of OperationCanceledException or other exception)
                Trace.TraceInformation(e.ToString());
            }
        }

        private bool CanLoginTwoFactorAuthentication() => CanLogin() && !string.IsNullOrWhiteSpace(OtpCode);

        private string LoadDeviceId(string host, string account, string password)
        {
            // TODO: Implement DeviceId reloading if available here
            return string.Empty;
        }

        private void SaveDeviceId(string host, string account, string password, string deviceId)
        {
            // TODO: Implement DeviceId saving if available here
        }

        private readonly INavigationService _navigationService;
        private readonly INetworkService _networkService;
        private readonly IVideoStation _videoStation;

        private TimeSpan _timeout = TimeSpan.FromSeconds(30);
        private IWebProxy _webProxy;
        private Uri _baseUri;
        private string _host = string.Empty;
        private string _account = string.Empty;
        private string _password = string.Empty;
        private string _otpCode = string.Empty;

        private bool _rememberMe;

        public RelayCommand NavigateCommand { get; }

        public RelayCommand LoginCommand { get; }

        public RelayCommand LoginTwoFactorAuthenticationCommand { get; }

        public string Host
        {
            get => _host ?? string.Empty;
            set
            {
                _host = value ?? string.Empty;
                RaisePropertyChanged(nameof(Host));
                LoginCommand.RaiseCanExecuteChanged();
                LoginTwoFactorAuthenticationCommand.RaiseCanExecuteChanged();
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
                LoginTwoFactorAuthenticationCommand.RaiseCanExecuteChanged();
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
                LoginTwoFactorAuthenticationCommand.RaiseCanExecuteChanged();
            }
        }
        
        public string OtpCode
        {
            get => _otpCode;
            set
            {
                _otpCode = value ?? string.Empty;
                RaisePropertyChanged(nameof(OtpCode));
                LoginTwoFactorAuthenticationCommand.RaiseCanExecuteChanged();
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
    }
}