﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SynologyAPI;
using SynologyRestDAL;
using StdUtils;
using CommandLine;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace synods
{
    class Program
    {
        static void Main(string[] args)
        {
            string invokedVerb = "";
            object invokedVerbInstance = null;
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options,
                                        (verb, subOptions) =>
                                        {
                                            invokedVerb = verb;
                                            invokedVerbInstance = subOptions;
                                        }))
            {
                Console.ReadLine();
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            var ds = new DownloadStation(new Uri(appSettings["host"]), appSettings["username"], appSettings["password"], CreateProxy(appSettings["proxy"]));
            // todo: this is ugly, needs refactoring
            switch(invokedVerb)
            {
                case("list"):
                    var listOptions = (ListOptions)invokedVerbInstance;
                    if (ds.Login())
                    {
                        var listResult = ds.List(String.Join(",", listOptions.Details));
                        if (listResult.Success)
                        {
                            var taskList = from task in listResult.Data.Tasks select task;
                            if (listOptions.Status.Any())
                            {
                                var statusesToList = new List<string>(listOptions.Status);
                                taskList = from task in taskList where statusesToList.Contains(task.Status) select task;
                            }
                            foreach (var task in taskList)
                            {
                                Console.WriteLine(ObjectUtils.HumanReadable(task));
                                Console.WriteLine();
                            }
                        }
                        ds.Logout();
                    }
                    break;

                case ("task"):
                    var taskOptions = (TaskOptions)invokedVerbInstance;
                    if (ds.Login())
                    {
                        var taskResult = ds.GetTasks(taskOptions.Id, taskOptions.Details);
                        if (taskResult.Success)
                        {
                            foreach (var task in taskResult.Data.Tasks)
                            {
                                Console.WriteLine(ObjectUtils.HumanReadable(task));
                                Console.WriteLine();
                            }
                        }
                        ds.Logout();
                    }
                    break;
                case ("new"):
                    var newOptions = (NewOptions)invokedVerbInstance;
                    if (ds.Login())
                    {
                        TResult<Object> taskResult = null;
                        var fileName = Path.GetFileName(newOptions.Filename);
                        var msgResult = String.Empty;
                        if (!String.IsNullOrWhiteSpace(newOptions.Filename))
                        {
                            using (var taskFile = new FileStream(newOptions.Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                taskResult = ds.CreateTask(fileName, taskFile);
                            }
                            msgResult = String.Format("{0} upload", fileName);
                        }
                        if (!String.IsNullOrWhiteSpace(newOptions.Uri))
                        {
                            taskResult = ds.CreateTask(newOptions.Uri);
                            msgResult = String.Format("Create task to download {0}", newOptions.Uri);
                        }
                        Console.WriteLine("{0}: {1}", msgResult, taskResult != null && taskResult.Success ? "Ok" : "Failed ");
                        ds.Logout();
                    }
                    break;
                case ("delete"):
                    var deleteOptions = (TaskDeleteOptions)invokedVerbInstance;
                    if (ds.Login())
                    {
                        if (deleteOptions.Id.Any())
                        {
                            var taskResult = ds.DeleteTasks(deleteOptions.Id, deleteOptions.Force);
                            if (taskResult.Success)
                            {
                                foreach (var taskError in taskResult.Data)
                                {
                                    Console.WriteLine(ObjectUtils.HumanReadable(taskError));
                                    Console.WriteLine();
                                }
                            }
                        }
                        ds.Logout();
                    }
                    break;
                case ("pause"):
                    var pauseOptions = (TaskPauseOptions)invokedVerbInstance;
                    if (ds.Login())
                    {
                        if (pauseOptions.Id.Any())
                        {
                            var taskResult = ds.PauseTasks(pauseOptions.Id);
                            if (taskResult.Success)
                            {
                                foreach (var taskError in taskResult.Data)
                                {
                                    Console.WriteLine(ObjectUtils.HumanReadable(taskError));
                                    Console.WriteLine();
                                }
                            }
                        }
                        ds.Logout();
                    }
                    break;
                case ("resume"):
                    var resumeOptions = (TaskResumeOptions)invokedVerbInstance;
                    if (ds.Login())
                    {
                        if (resumeOptions.Id.Any())
                        {
                            var taskResult = ds.ResumeTasks(resumeOptions.Id);
                            if (taskResult.Success)
                            {
                                foreach (var taskError in taskResult.Data)
                                {
                                    Console.WriteLine(ObjectUtils.HumanReadable(taskError));
                                    Console.WriteLine();
                                }
                            }
                        }
                        ds.Logout();
                    }
                    break;
                default:
                    break;
            }
            // Console.ReadLine();
        }


        public static WebProxy CreateProxy(string proxyUrl)
        {
            if (String.IsNullOrWhiteSpace(proxyUrl))
            {
                return null;
            }
            return new WebProxy(new Uri(proxyUrl));
        }
    }
}
