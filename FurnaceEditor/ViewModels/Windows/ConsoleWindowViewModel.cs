﻿using System.ComponentModel;
using System.Reactive;
using System.Windows.Data;
using FurnaceEditor.Utilities.Loggers;
using FurnaceEditor.Utilities.Providers;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Windows.Foundation.Collections;

namespace FurnaceEditor.ViewModels.Windows
{
    public class ConsoleWindowViewModel : ViewModelBase
    {
        private readonly ILogger<ConsoleWindowViewModel> _logger;
        private ICollectionView _filteredLogs;
        private int _messageFilter = (int)(LogType.Info | LogType.Warn | LogType.Error);
        private LogMessage _selectedLog;

        private bool _isInfoChecked;
        private bool _isWarnChecked;
        private bool _isErrorChecked;
        private string _filterText = string.Empty;


        #region Commands
        public ReactiveCommand<Unit, Unit> ToggleInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleWarnCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleErrorCommand { get; }
        public ReactiveCommand<LogMessage, Unit> SelectedLogCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ClearCommand { get; private set; }
        #endregion

        #region Properties
        public LogMessage SelectedLog
        {
            get => _selectedLog;
            set => this.RaiseAndSetIfChanged(ref _selectedLog, value);
        }

        public ICollectionView FilteredLogs
        {
            get => _filteredLogs;
            set => this.RaiseAndSetIfChanged(ref _filteredLogs, value);
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterText, value);
                UpdateFilter();
            }
        }

        public bool IsInfoChecked
        {
            get => _isInfoChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isInfoChecked, value))
                {
                    UpdateFilter();
                }
            }
        }

        public bool IsWarnChecked
        {
            get => _isWarnChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isWarnChecked, value))
                {
                    UpdateFilter();
                }
            }
        }

        public bool IsErrorChecked
        {
            get => _isErrorChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isErrorChecked, value))
                {
                    UpdateFilter();
                }
            }
        }
        #endregion

        public ConsoleWindowViewModel(ILogger<ConsoleWindowViewModel> logger, IObservableLoggerProvider loggerProvider)
        {
            _logger = logger;
            FilteredLogs = new CollectionViewSource().View;
            FilteredLogs = CollectionViewSource.GetDefaultView(loggerProvider.Logs);
            SelectedLogCommand = ReactiveCommand.Create<LogMessage>(log => { SelectedLog = log; });
            ClearCommand = ReactiveCommand.Create(loggerProvider.Logs.Clear);

            IsInfoChecked = true;
            IsWarnChecked = true;
            IsErrorChecked = true;

            FilteredLogs.Filter = log =>
            {
                var logMessage = (LogMessage)log;

                bool isMessageTypeMatch = ((int)logMessage.LogType & _messageFilter) != 0;

                // Check if the log message contains the filter text, if any
                bool isTextMatch = string.IsNullOrEmpty(FilterText) ||
                                    logMessage.Message.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;

                return isMessageTypeMatch && isTextMatch;
            };
        }

        private void UpdateFilter()
        {
            _messageFilter = 0;

            if (IsInfoChecked) _messageFilter |= (int)LogType.Info;
            if (IsWarnChecked) _messageFilter |= (int)LogType.Warn;
            if (IsErrorChecked) _messageFilter |= (int)LogType.Error;

            FilteredLogs.Refresh();
        }
    }
}
