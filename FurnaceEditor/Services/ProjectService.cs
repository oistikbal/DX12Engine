﻿using System.Diagnostics;
using System.IO;
using FurnaceEditor.Models;
using FurnaceEditor.Utilities.Serializers;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace FurnaceEditor.Services
{
    public class ProjectService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger _logger;
        private Project _project;

        public ProjectService(string path, IMessageBus messageBus, ILogger<ProjectService> logger)
        {
            Debug.Assert(path != null, "Path cannot be null");
            Debug.Assert(messageBus != null, "MessageBus Cannot be null");

            _messageBus = messageBus;
            _logger = logger;
            _project = Serializer.FromFile<Project>(path);
            _project.Path = System.IO.Path.GetDirectoryName(path);
            InitializeProjectStructure();

            _messageBus.SendMessage(_project, "ProjectLoaded");
            _logger.LogInformation($"Project loaded at path {path}");
        }

        public Project GetProject()
        {
            return _project;
        }

        private void InitializeProjectStructure()
        {
            // Define the folders that should be present in the project
            string[] requiredFolders = new string[]
            {
            System.IO.Path.Combine(_project.Path, "Assets"),
            System.IO.Path.Combine(_project.Path, "Library"),
            System.IO.Path.Combine(_project.Path, "Logs"),
                // Add more folders as needed
            };

            // Check if each folder exists, and create it if it doesn't
            foreach (string folder in requiredFolders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }
    }
}
