﻿using System;
using System.Linq;
using System.Management.Automation;
using Octopus.Client;

namespace Octopus.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "OctoVariable")]
    public class RemoveVariable : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The project to get the variables for."
            )]
        public string Project { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 1
            )]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var octopus = (OctopusRepository)SessionState.PSVariable.GetValue("OctopusRepository");
            if (octopus == null)
            {
                throw new Exception(
                    "Connection not established. Please connect to you Octopus Deploy instance with Connect-OctoServer");
            }

            // Find the project that owns the variables we want to edit
            var project = octopus.Projects.FindByName(Project);

            if (project == null)
            {
                const string msg = "Project '{0}' was found.";
                throw new Exception(string.Format(msg, Project));
            }

            // Get the variables for editing
            var variableSet = octopus.VariableSets.Get(project.Link("Variables"));
            
            // This is debatable. If you have more than one of the same name,
            // it's going to remove one at random. On the other hand, if you 
            // get all matching the name, put pipe the name in multiple times 
            // (ie: find Test.*) you'll get an error on the successive attempts.
            var variable = variableSet.Variables.FirstOrDefault(x => x.Name == Name);

            if (variable == null)
            {
                const string msg = "No variable with the name '{0}' in the project '{1}' was found.";
                //throw new Exception(string.Format(msg, Name, Project));
                WriteWarning(string.Format(msg, Name, Project));
            }
            else
            {
                variableSet.Variables.Remove(variable);

                // Save the variables
                octopus.VariableSets.Modify(variableSet);
            }
        }
    }
}
