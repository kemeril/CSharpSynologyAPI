using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using SynologyAPI;
using SynologyAPI.Exception;
using SynologyRestDAL;

namespace KDSVideo.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LoginViewModel(INavigationService navigationService, IVideoStation videoStation)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _videoStation = videoStation ?? throw new ArgumentNullException(nameof(videoStation));
            NavigateCommand = new RelayCommand(() => _navigationService.NavigateTo(ViewModelLocator.MainPageKey));

            LoginCommand = new RelayCommand(Login);
        }

        private IWebProxy CreateProxy(string proxyUrl)
        {
            return string.IsNullOrWhiteSpace(proxyUrl) ? null : new WebProxy(new Uri(proxyUrl));
        }

        private IWebProxy GetDefaultProxy()
        {
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            return proxy;
        }

        private async void Login()
        {
            // TODO: Implement DeviceId reload if available

            var success = false;
            OtpCode = string.Empty;
 
            do
            {
                var baseUri = new Uri(Host);

                // TODO: Implement custom, user defined proxy usage
                var proxy = GetDefaultProxy() ?? CreateProxy("");

                try
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    var loginInfo = await _videoStation.LoginAsync(baseUri, Account, Password, OtpCode, _deviceId, proxy, cts.Token);
                    if (RememberMe)
                    {
                        _deviceId = loginInfo.DeviceId ?? string.Empty;
                    }
                    success = true;
                }
                catch (SynoRequestException e)
                {
                    RememberMe = false;
                    OtpCode = string.Empty;
                    if (e.ErrorCode == ErrorCodes.OneTimePasswordNotSpecified)
                    {
                        // TODO: Request new OTP CODE view state
                    }
                    else
                    {
                        // TODO: Request Host, Account, Password view state 
                    }
                }
                catch
                {
                    // ignored (because of OperationCanceledException or other exception)
                }
            } while (!success);

            _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
        }

        private readonly INavigationService _navigationService;
        private readonly IVideoStation _videoStation;

        private string _deviceId = string.Empty;
        private string _host = string.Empty;
        private string _account = string.Empty;
        private string _password = string.Empty;
        private string _otpCode = string.Empty;

        private bool _rememberMe;

        public RelayCommand NavigateCommand { get; }

        public RelayCommand LoginCommand { get; }

        public string Host
        {
            get => _host ?? string.Empty;
            set
            {
                _host = value ?? string.Empty;
                OnPropertyChanged(nameof(Host));
            }
        }
        
        public string Account
        {
            get => _account ?? string.Empty;
            set
            {
                _account = value ?? string.Empty;
                OnPropertyChanged(nameof(Account));
            }
        }
        
        public string Password
        {
            get => _password ?? string.Empty;
            set
            {
                _password = value ?? string.Empty;
                OnPropertyChanged(nameof(Password));
            }
        }
        
        public string OtpCode
        {
            get => _otpCode;
            set
            {
                _otpCode = value ?? String.Empty;
                OnPropertyChanged(nameof(OtpCode));
            }
        }
        
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                OnPropertyChanged(nameof(RememberMe));
            }
        }
    }
}