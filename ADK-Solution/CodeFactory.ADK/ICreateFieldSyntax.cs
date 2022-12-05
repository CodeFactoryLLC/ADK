//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using CodeFactory.DotNet.CSharp;
using System.Threading.Tasks;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Contract definition for injecting a field definition into a target container.
    /// </summary>
    /// <typeparam name="TContainerType"></typeparam>
    public interface ICreateFieldSyntax<TContainerType> where TContainerType : CsContainer
    {
        /// <summary>
        /// Creates a C# field definition to be injected into a target instance container in a source code file. Checks the container to make sure the field does not already exist.
        /// </summary>
        /// <param name="updateContainerSource">Target update container.</param>
        /// <param name="indentLevel">The target indent level to apply to the syntax definition for the field and potentially documentation.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldType">C# formatted syntax for the fields type.</param>
        /// <param name="security">The target c# security level to set the field to.</param>
        /// <param name="defaultValueSyntax">Optional parameter to provide the fully formatted syntax for the default value of the field.</param>
        /// <param name="staticKeyword">Optional parameter to make the field static.</param>
        /// <param name="constantKeyword">Optional parameter to make the field a constant.</param>
        /// <param name="readonlyKeyword">Optional parameter to make the field read only. </param>
        /// <param name="xmlDocumentationSummaryTag">Optional parameter to create a XML documentation summary tag for the field. Provide the content to go into the summary tag.</param>
        /// <returns>The update instance source once the field has been injected into the end of the field.</returns>
        Task<IUpdateInstanceSource<TContainerType>> CreateFieldSyntaxAsync(IUpdateInstanceSource<TContainerType> updateContainerSource, int indentLevel,
            string fieldName, string fieldType, CsSecurity security, string defaultValueSyntax = null, 
            bool staticKeyword = false, bool constantKeyword = false, bool readonlyKeyword = false, string xmlDocumentationSummaryTag = null);

        /// <summary>
        /// Creates a C# field definition to be injected into a target instance container in a source code file. Checks the container to make sure the field does not already exist.
        /// </summary>
        /// <param name="updateContainerSource">Target update container.</param>
        /// <param name="indentLevel">The target indent level to apply to the syntax definition for the field and potentially documentation.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="type">The <see cref="CsType"/> model to set the field definition to.</param>
        /// <param name="security">The target c# security level to set the field to.</param>
        /// <param name="defaultValueSyntax">Optional parameter to provide the fully formatted syntax for the default value of the field.</param>
        /// <param name="staticKeyword">Optional parameter to make the field static.</param>
        /// <param name="constantKeyword">Optional parameter to make the field a constant.</param>
        /// <param name="readonlyKeyword">Optional parameter to make the field read only. </param>
        /// <param name="xmlDocumentationSummaryTag">Optional parameter to create a XML documentation summary tag for the field. Provide the content to go into the summary tag.</param>
        /// <returns>The update instance source once the field has been injected into the end of the field.</returns>
        Task<IUpdateInstanceSource<TContainerType>> CreateFieldSyntaxAsync(IUpdateInstanceSource<TContainerType> updateContainerSource,int indentLevel,
            string fieldName, CsType type, CsSecurity security, string defaultValueSyntax = null, bool staticKeyword = false, bool constantKeyword = false, bool readonlyKeyword = false, string xmlDocumentationSummaryTag = null);


        /// <summary>
        /// Creates the fully formatted field name.
        /// </summary>
        /// <param name="fieldName">The source field name to use for formatting.</param>
        /// <returns>The fully formatted field name.</returns>
        string CreateFieldName(string fieldName);
    }
}
