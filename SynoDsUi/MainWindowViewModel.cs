using AutoMapper;
using SynoDsUi.Annotations;
using SynologyAPI;
using SynologyRestDAL.Ds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace SynoDsUi
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private TaskStatusViewModel _currentStatus = new TaskStatusViewModel("all");

        private IDictionary<string, ObservableCollection<TaskViewModel>> _tasksByStatus;

        public ObservableCollection<TaskViewModel> CurrentTasks => _tasksByStatus[_currentStatus.Title];

        public ObservableCollection<TaskStatusViewModel> Statuses { get; }

        public TaskStatusViewModel CurrentStatusTab
        {
            get => _currentStatus;
            set { _currentStatus = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            //Mapper.CreateMap<Task, TaskViewModel>();
            //Replacement but not tested yet:
            Mapper.Initialize(cfg=> cfg.CreateMap<Task, TaskViewModel>());
            
            var appSettings = ConfigurationManager.AppSettings;
            var downloadStation = new DownloadStation(new Uri(appSettings["host"]), appSettings["username"], appSettings["password"], CreateProxy(appSettings["proxy"]));

            if (downloadStation.LoginAsync().GetAwaiter().GetResult())
            {
                var listResult = downloadStation.ListAsync(string.Join(",", new []{ "detail", "transfer", "file", "tracker" })).GetAwaiter().GetResult();
                if (listResult.Success)
                {
                    var taskList = (from task in listResult.Data.Tasks orderby task.Additional.Detail.CreateTime select Mapper.Map<TaskViewModel>(task)).ToList();
                    var allTasks = new ObservableCollection<TaskViewModel>(taskList);
                    var statusList = (new List<TaskStatusViewModel>() {new TaskStatusViewModel("all")}).Concat(
                        taskList.Select(t => t.Status).Distinct().OrderBy(s => s).Select(s => new TaskStatusViewModel(s)));
                    Statuses = new ObservableCollection<TaskStatusViewModel>(statusList);
                    _tasksByStatus = new Dictionary<string, ObservableCollection<TaskViewModel>>();
                    foreach (var taskStatus in Statuses)
                    {
                        if (taskStatus.Title == "all")
                        {
                            _tasksByStatus.Add(taskStatus.Title, allTasks);
                            continue;
                        }
                        var tasks = new ObservableCollection<TaskViewModel>(allTasks.Where(t => t.Status == taskStatus.Title).OrderBy(t => t.Additional.Detail.CreateTime));
                        _tasksByStatus.Add(taskStatus.Title, tasks);
                    }

                }
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("CurrentTasks");
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("Statuses");

                downloadStation.LogoutAsync().GetAwaiter();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static WebProxy CreateProxy(string proxyUrl)
        {
            return string.IsNullOrWhiteSpace(proxyUrl) ? null : new WebProxy(new Uri(proxyUrl));
        }
    }
}