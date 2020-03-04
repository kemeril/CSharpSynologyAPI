using System;
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

            LoginCommand = new RelayCommand(Login);
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
                var proxy = _networkService.GetProxy();

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
        private readonly INetworkService _networkService;
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
                RaisePropertyChanged(nameof(Host));
            }
        }
        
        public string Account
        {
            get => _account ?? string.Empty;
            set
            {
                _account = value ?? string.Empty;
                RaisePropertyChanged(nameof(Account));
            }
        }
        
        public string Password
        {
            get => _password ?? string.Empty;
            set
            {
                _password = value ?? string.Empty;
                RaisePropertyChanged(nameof(Password));
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
    }
}