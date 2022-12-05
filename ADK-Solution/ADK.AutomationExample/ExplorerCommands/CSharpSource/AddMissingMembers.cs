using CodeFactory.Logging;
using CodeFactory.VisualStudio;
using CodeFactory.VisualStudio.SolutionExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFactory.DotNet.CSharp;
using CodeFactory.ADK;
using CodeFactory.ADK.NDF;
using CodeFactory.ADK.Standard;

namespace ADK.AutomationExample.ExplorerCommands.CSharpSource
{
    /// <summary>
    /// Code factory command for automation of a C# document when selected from a project in solution explorer.
    /// </summary>
    public class AddMissingMembers : CSharpSourceCommandBase
    {
        private static readonly string commandTitle = "Add Missing Members";
        private static readonly string commandDescription = "Adds missing interface members from the class implementation.";
        private static readonly string loggerFieldName = "_logger";

        /// <summary>
        /// Name of the Microsoft logging library.
        /// </summary>
        private const string MicrosoftLoggingNamespace = "Microsoft.Extensions.Logging";

        /// <summary>
        /// Name of the CodeFactory NDF library
        /// </summary>
        private const string NDFNamespace = "CodeFactory.NDF";

#pragma warning disable CS1998

        /// <inheritdoc />
        public AddMissingMembers(ILogger logger, IVsActions vsActions) : base(logger, vsActions, commandTitle, commandDescription)
        {
            //Intentionally blank
        }

        #region Overrides of VsCommandBase<IVsCSharpDocument>

        /// <summary>
        /// Validation logic that will determine if this command should be enabled for execution.
        /// </summary>
        /// <param name="result">The target model data that will be used to determine if this command should be enabled.</param>
        /// <returns>Boolean flag that will tell code factory to enable this command or disable it.</returns>
        public override async Task<bool> EnableCommandAsync(VsCSharpSource result)
        {
            //Result that determines if the the command is enabled and visible in the context menu for execution.
            bool isEnabled = false;

            try
            {
                //Getting the hosting class from the source code file.
                var classData = result.SourceCode.Classes.FirstOrDefault();

                //Determine if a class was found. If not do not enable the command.
                isEnabled = classData != null;

                //If isEnabled check to make sure there are missing interface members. Otherwise do not display the command.
                if (isEnabled) isEnabled = classData.MissingInterfaceMembers().Any();
            }
            catch (Exception unhandledError)
            {
                _logger.Error($"The following unhandled error occurred while checking if the solution explorer C# document command {commandTitle} is enabled. ",
                    unhandledError);
                isEnabled = false;
            }

            return isEnabled;
        }

        /// <summary>
        /// Code factory framework calls this method when the command has been executed. 
        /// </summary>
        /// <param name="result">The code factory model that has generated and provided to the command to process.</param>
        public override async Task ExecuteCommandAsync(VsCSharpSource result)
        {
            try
            {
                //Local variable to keep track of the source code in the file.
                var sourceCode = result.SourceCode;

                //Getting the implementation class in the source code.
                var classData = sourceCode?.Classes.FirstOrDefault();

                //If not class is found exit the command.
                if (classData == null) return;

                //Getting the missing interface members
                var missingMembers = classData.MissingInterfaceMembers();

                //If no members are missing then return nothing to automate.
                if (!missingMembers.Any()) return;

                //Getting the hosting project information for the source code file.
                var project = await result.GetHostingProjectAsync();

                //If there is no hosting project then do not update the code file.
                if(project == null) return;

                //Loading the project references 
                var projectReferences = await project.GetProjectReferencesAsync();

                //If no project references were found do not update the code file. 
                if (projectReferences == null) return;

                //Creating a update class source which will manage the updating the source code file and the class itself.
                IUpdateInstanceSource<CsClass> classSource = new UpdateClassSource(sourceCode, classData, VisualStudioActions, null);

                //Determining if logging is included with the project.
                bool hasLogging = projectReferences.Any(r => r.Name.StartsWith(MicrosoftLoggingNamespace));

                //Determining if the NDF delivery framework has been loaded.
                bool hasNDF = projectReferences.Any(r => r.Name == NDFNamespace);

                //Logging formatter that will be used to inject logging.
                ILoggingFormatter logFormatter = null;

                if (hasLogging)
                {
                    if (hasNDF) logFormatter = new NDFLogger(loggerFieldName);
                    
                    if (logFormatter == null) logFormatter = new MicrosoftLogger(loggerFieldName);

                    await classSource.UsingStatementAddAsync(MicrosoftLoggingNamespace);
                }

                if(hasNDF) {await classSource.UsingStatementAddAsync(NDFNamespace);}

                classSource.LoadNamespaceManager();

                CreateFieldSyntax<CsClass> injectFieldSyntax = new CreateFieldSyntax<CsClass>(true, "_");

                if (hasLogging)
                    classSource = await injectFieldSyntax.CreateFieldSyntaxAsync(classSource, 2, "Logger", "ILogger",
                        CsSecurity.Private, null, false, false, true, "Logger to be used for this class.");

                InjectStandardMethodWithAsync<CsClass> injectMethodAsync = hasNDF
                    ? new InjectStandardMethodWithAsync<CsClass>(logFormatter,
                        new List<IBoundsCheck>
                        {
                            new CodeFactory.ADK.NDF.StringBoundsCheck(logFormatter),
                            new CodeFactory.ADK.NDF.NullBoundsCheck(logFormatter)
                        },
                        new List<ICodeBlockSyntax>
                        {
                            new CodeFactory.ADK.NDF.CatchBlockPassManagedException(logFormatter), 
                            new CodeFactory.ADK.NDF.CatchBlockException(logFormatter)
                        })
                    : new InjectStandardMethodWithAsync<CsClass>(logFormatter,
                        new List<IBoundsCheck>
                        {
                            new CodeFactory.ADK.Standard.StringBoundsCheck(logFormatter),
                            new CodeFactory.ADK.Standard.NullBoundsCheck(logFormatter)
                        },
                        new List<ICodeBlockSyntax>
                        {
                            new CodeFactory.ADK.Standard.CatchBlockException(logFormatter)
                        });

                foreach (var missingMember in missingMembers)
                {
                    switch (missingMember.MemberType)
                    {
                        case CsMemberType.Event:
                            break;
                        case CsMemberType.Field:
                            break;
                        case CsMemberType.Method:
                            var methodMember = missingMember as CsMethod;

                            classSource = await injectMethodAsync.InjectSyntaxAsync(classSource, methodMember, 2);

                            break;
                        case CsMemberType.Property:
                            break;
                    }
                }


            }
            catch (Exception unhandledError)
            {
                _logger.Error($"The following unhandled error occurred while executing the solution explorer C# document command {commandTitle}. ",
                    unhandledError);

            }

        }

        #endregion
    }
}
