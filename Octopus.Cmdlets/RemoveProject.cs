﻿#region License
// Copyright 2014 Colin Svingen

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Linq;
using System.Management.Automation;
using Octopus.Client;

namespace Octopus.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Remove a project from the Octopus Deploy server.</para>
    /// <para type="description">The Remove-OctoProject cmdlet removes a project from the Octopus Deploy server.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "Project", DefaultParameterSetName = "ByName")]
    public class RemoveProject : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The name of the project to remove.</para>
        /// </summary>
        [Parameter(
            ParameterSetName = "ByName",
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        public string[] Name { get; set; }

        /// <summary>
        /// <para type="description">The id of the project to remove.</para>
        /// </summary>
        [Parameter(
            ParameterSetName = "ById",
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        [Alias("ProjectId")]
        public string[] Id { get; set; }

        private IOctopusRepository _octopus;

        /// <summary>
        /// BeginProcessing
        /// </summary>
        protected override void BeginProcessing()
        {
            _octopus = Session.RetrieveSession(this);
        }

        /// <summary>
        /// ProcessRecord
        /// </summary>
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "ByName":
                    ProcessByName();
                    break;
                case "ById":
                    ProcessById();
                    break;
                default:
                    throw new Exception("Unknown ParameterSetName: " + ParameterSetName);
            }
        }

        private void ProcessById()
        {
             var projects = from id in Id
                         select _octopus.Projects.Get(id);

            foreach (var project in projects)
            {
                WriteVerbose("Deleting project: " + project.Name);
                _octopus.Projects.Delete(project);
            }
        }

        private void ProcessByName()
        {
            var projects = _octopus.Projects.FindByNames(Name);

            foreach (var project in projects)
            {
                WriteVerbose("Deleting project: " + project.Name);
                _octopus.Projects.Delete(project);
            }
        }
    }
}
