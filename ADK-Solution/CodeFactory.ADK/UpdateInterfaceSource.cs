//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using CodeFactory.DotNet.CSharp;
using CodeFactory.Formatting.CSharp;
using CodeFactory.VisualStudio;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Update source interface definition.
    /// </summary>
    public class UpdateInterfaceSource:UpdateSourceBase<CsInterface>
    {
        /// <summary>
        /// Creates an instance of <see cref="UpdateInterfaceSource"/>
        /// </summary>
        /// <param name="source">The source code file to update.</param>
        /// <param name="container">The interface model that is to be updated.</param>
        /// <param name="vsActions">The CodeFactory actions API.</param>
        /// <param name="namespaceManager">The namespace manager to use with the update source implementation.</param>
        public UpdateInterfaceSource(CsSource source, CsInterface container, IVsActions vsActions, NamespaceManager namespaceManager) : base(source, container, vsActions, namespaceManager)
        {
            //Intentionally blank
        }
    }
}
