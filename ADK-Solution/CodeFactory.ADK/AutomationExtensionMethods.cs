//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using System;
using System.Threading.Tasks;
using CodeFactory.DotNet;
using CodeFactory.DotNet.CSharp;
using CodeFactory.Formatting.CSharp;
using CodeFactory.VisualStudio;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Extension methods class that contain utilities that are used for formatting with automation.
    /// </summary>
    public static class AutomationExtensionMethods
    {
        /// <summary>
        /// Converts a string to camel case format.
        /// </summary>
        /// <param name="source">string for format the data.</param>
        /// <returns>Formatted camel case format or null if no string is provided.</returns>
        public static string ConvertToCamelCase(this string source)
        {
            if (string.IsNullOrEmpty(source)) return null;

            return source.Length == 1 ? $"{source.Substring(0, 1).ToLower()}" 
                : $"{source.Substring(0, 1).ToLower()}{source.Substring(1)}";
        }

        /// <summary>
        /// Generates the XML documentation for a target model.
        /// </summary>
        /// <param name="source">Model that could have xml documentation.</param>
        /// <param name="indentLevel">The indent level to start each line with.</param>
        /// <returns>Formatted documentation or null if no documentation was provided.</returns>
        public static string CSharpGenerateXmlDocsSyntax(this IDocumentation source,int indentLevel = 0)
        {
            if (source == null) return null;

            if (source.HasDocumentation) return null;
            
            SourceFormatter docFormatter = new SourceFormatter();

            foreach (var xmlDoc in source.CSharpFormatXmlDocumentationEnumerator())
            {
                docFormatter.AppendCodeLine(indentLevel,xmlDoc);
            }

            return docFormatter.ReturnSource();
        }

        /// <summary>
        /// Checks all types definitions and makes sure they are included in the namespace manager for the target update source.
        /// </summary>
        /// <typeparam name="TContainerType">The target container type that is being used in the update source.</typeparam>
        /// <param name="sourceMethod">The target model to check using statements on.</param>
        /// <param name="updateSource">the update source to update.</param>
        /// <returns>Missing using statements added or the original update source if no additional using statements needed.</returns>
        public static async Task<IUpdateSource<TContainerType>> AddMissingUsingStatementsAsync<TContainerType>(
            this CsMethod sourceMethod, IUpdateSource<TContainerType> updateSource) where TContainerType : CsContainer
        {
            if(sourceMethod == null) throw new ArgumentNullException(nameof(sourceMethod));
            if (updateSource == null) throw new ArgumentNullException(nameof(updateSource));

            var updatedSource = updateSource;
            if(updatedSource.NamespaceManager == null) updatedSource.LoadNamespaceManager();

            if (sourceMethod.HasStrongTypesInGenerics)
            {
                foreach (var sourceGenericType in sourceMethod.GenericTypes)
                {
                    updatedSource = await sourceGenericType.AddMissingUsingStatementsAsync(updatedSource);
                }
            }

            if (sourceMethod.HasAttributes)
            {
                foreach (var methodAttribute in sourceMethod.Attributes)
                {
                    updatedSource = await methodAttribute.AddMissingUsingStatementsAsync(updatedSource);
                }
            }

            if (sourceMethod.HasParameters)
            {
                foreach (var methodParameter in sourceMethod.Parameters)
                {
                    updatedSource =
                        await methodParameter.ParameterType.AddMissingUsingStatementsAsync(updatedSource);
                }
            }

            if (sourceMethod.ReturnType != null)
                updatedSource = await sourceMethod.ReturnType.AddMissingUsingStatementsAsync(updatedSource);

            return updatedSource;
        }

        /// <summary>
        /// Checks all types definitions and makes sure they are included in the namespace manager for the target update source.
        /// </summary>
        /// <typeparam name="TContainerType">The target container type that is being used in the update source.</typeparam>
        /// <param name="sourceProperty">The target model to check using statements on.</param>
        /// <param name="updateSource">the update source to update.</param>
        /// <returns>Missing using statements added or the original update source if no additional using statements needed.</returns>
        public static async Task<IUpdateSource<TContainerType>> AddMissingUsingStatementsAsync<TContainerType>(
            this CsProperty sourceProperty, IUpdateSource<TContainerType> updateSource) where TContainerType : CsContainer
        {
            if(sourceProperty == null) throw new ArgumentNullException(nameof(sourceProperty));
            if (updateSource == null) throw new ArgumentNullException(nameof(updateSource));

            var updatedSource = updateSource;
            if(updatedSource.NamespaceManager == null) updatedSource.LoadNamespaceManager();

            if (sourceProperty.HasAttributes)
            {
                foreach (var methodAttribute in sourceProperty.Attributes)
                {
                    updatedSource = await methodAttribute.AddMissingUsingStatementsAsync(updatedSource);
                }
            }

            updatedSource = await sourceProperty.PropertyType.AddMissingUsingStatementsAsync(updatedSource);

            return updatedSource;
        }

        /// <summary>
        /// Checks all types definitions and makes sure they are included in the namespace manager for the target update source.
        /// </summary>
        /// <typeparam name="TContainerType">The target container type that is being used in the update source.</typeparam>
        /// <param name="sourceAttribute">The target model to check using statements on.</param>
        /// <param name="updateSource">the update source to update.</param>
        /// <returns>Missing using statements added or the original update source if no additional using statements needed.</returns>
        public static async Task<IUpdateSource<TContainerType>> AddMissingUsingStatementsAsync<TContainerType>(
            this CsAttribute sourceAttribute, IUpdateSource<TContainerType> updateSource) where TContainerType : CsContainer
        {
            if(sourceAttribute == null) throw new ArgumentNullException(nameof(sourceAttribute));
            if (updateSource == null) throw new ArgumentNullException(nameof(updateSource));

            var updatedSource = updateSource;

            updatedSource = await sourceAttribute.Type.AddMissingUsingStatementsAsync(updatedSource);

            return updatedSource;
        }

        /// <summary>
        /// Checks the type definition and makes sure it is included in the namespace manager for the target update source.
        /// </summary>
        /// <typeparam name="TContainerType">The target container type that is being used in the update source.</typeparam>
        /// <param name="sourceType">The target model to check using statements on.</param>
        /// <param name="updateSource">the update source to update.</param>
        /// <returns>Missing using statements added or the original update source if no additional using statements needed.</returns>
        public static async Task<IUpdateSource<TContainerType>> AddMissingUsingStatementsAsync<TContainerType>(
            this CsType sourceType, IUpdateSource<TContainerType> updateSource) where TContainerType : CsContainer
        {
            if(sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (updateSource == null) throw new ArgumentNullException(nameof(updateSource));

            var updatedSource = updateSource;
            if(updatedSource.NamespaceManager == null) updatedSource.LoadNamespaceManager();

            var manager = updatedSource.NamespaceManager;

            if (manager == null) throw new ArgumentNullException(nameof(NamespaceManager));

            if (sourceType.IsGenericPlaceHolder) return updatedSource;

            if (sourceType.HasStrongTypesInGenerics)
            {
                foreach (var sourceGenericType in sourceType.GenericTypes)
                {
                    updatedSource = await sourceGenericType.AddMissingUsingStatementsAsync<TContainerType>(updatedSource);
                }
            }

            var validate = manager.ValidNameSpace(sourceType.Namespace);

            if (!validate.namespaceFound) await updatedSource.UsingStatementAddAsync(sourceType.Namespace);

            return updatedSource;
        }

        /// <summary>
        /// Gets the hosting project from the provided <see cref="VsModel"/>
        /// </summary>
        /// <param name="source">The visual studio model to search the project.</param>
        /// <returns>The <see cref="VsProject"/> model that hosts the current model or null if the project is not found.</returns>
        /// <exception cref="ArgumentNullException">Raised when the model is null.</exception>
        /// <exception cref="CodeFactoryException">Raised if the source code functionality cannot load the project file.</exception>
        public static async Task<VsProject> GetHostingProjectAsync(this VsModel source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            VsProject hostingProject = null;

            VsModel currentModel = source;

            bool projectFound = false;
            while (!projectFound)
            {
                switch (currentModel.ModelType)
                {
                    case VisualStudioModelType.Project:
                        projectFound = true;
                        hostingProject = currentModel as VsProject;
                        break;

                    case VisualStudioModelType.ProjectFolder:
                        var projectFolder = currentModel as VsProjectFolder;
                        if (projectFolder.HasParent) currentModel = await projectFolder.GetParentAsync();
                        else projectFound = true;
                        break;

                    case VisualStudioModelType.Document:
                        var projectDoc = currentModel as VsDocument;
                        if(projectDoc.HasParent) currentModel = await projectDoc.GetParentAsync();
                        else projectFound = true;
                        break;

                    case VisualStudioModelType.CSharpSource:
                        var csSource = currentModel as VsCSharpSource;
                        var doc = await csSource.LoadDocumentModelAsync();
                        if (doc == null)
                            throw new CodeFactoryException("Could not load the source document from the project system.");

                        if(doc.HasParent) currentModel = await doc.GetParentAsync();
                        else projectFound = true;
                        break;

                    default:
                        projectFound = true;
                        break;
                }
            }
            
            return hostingProject;
        }

        /// <summary>
        /// Extension method that checks to confirm if a member model is a field and has the same name as provided.
        /// </summary>
        /// <param name="source">target member to check.</param>
        /// <param name="name">Name of the member to confirm.</param>
        /// <returns>True if the member is a field member and has the same name as provided in the <see cref="name"/> parameter.</returns>
        public static bool FieldMemberHasName(this CsMember source, string name)
        {
            if (source == null) return false;
            if(source.MemberType != CsMemberType.Field) return false;

            return source.Name == name;
        }
    }
}
