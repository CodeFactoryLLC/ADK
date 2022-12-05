//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using CodeFactory.DotNet.CSharp;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFactory.DotNet.CSharp.FormattedSyntax;
using CodeFactory.Formatting.CSharp;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Create a field definition from the provided data.
    /// </summary>
    /// <typeparam name="TContainerType">Target container type the field will be created in.</typeparam>
    public class CreateFieldSyntax<TContainerType>:ICreateFieldSyntax<TContainerType> where TContainerType : CsContainer
    {
        //Configuration fields used for formatting the field syntax.
        private readonly bool _camelCaseForFieldName;
        private readonly string _fieldNamePrefix;
        private readonly string _fieldNameSuffix;


        /// <summary>
        /// Creates a new instance of the <see cref="CreateFieldSyntax{TContainerType}"/>
        /// </summary>
        /// <param name="camelCaseForFieldName">Flag that determines if the field name should be formatted camel case.</param>
        /// <param name="fieldNamePrefix">Optional parameter for the prefix to add to the field name.</param>
        /// <param name="fieldNameSuffix">Optional parameter for the suffix to add to the field name.</param>
        public CreateFieldSyntax(bool camelCaseForFieldName, string fieldNamePrefix = null,
            string fieldNameSuffix = null)
        {
            _camelCaseForFieldName = camelCaseForFieldName;
            _fieldNamePrefix = fieldNamePrefix;
            _fieldNameSuffix = fieldNameSuffix;
        }

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
        public async Task<IUpdateInstanceSource<TContainerType>> CreateFieldSyntaxAsync(IUpdateInstanceSource<TContainerType> updateContainerSource, int indentLevel, string fieldName,
            string fieldType, CsSecurity security, string defaultValueSyntax = null, 
            bool staticKeyword = false, bool constantKeyword = false, bool readonlyKeyword = false, string xmlDocumentationSummaryTag = null)
        {
            //Bounds checking
            if(updateContainerSource == null) throw new ArgumentNullException(nameof(updateContainerSource));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException(nameof(fieldName));
            if (string.IsNullOrEmpty(fieldType)) throw new ArgumentNullException(fieldType);
            if (security == CsSecurity.Unknown)
                throw new ArgumentException("The security cannot be set to Unknown when creating field syntax.");

            //Storing the formatted name of the field.
            var formattedFieldName = CreateFieldName(fieldName); 

            //If the field already exists then return the update container.
            if (updateContainerSource.Container.Members.Any(m => m.FieldMemberHasName(formattedFieldName)))
                return updateContainerSource;

            //Loading the namespace manager
            updateContainerSource.LoadNamespaceManager();

            SourceFormatter fieldFormatter = new SourceFormatter();

            //Appending a line to the beginning of the definition.
            fieldFormatter.AppendCodeLine(indentLevel);

            //If XML documentation provided add to the field definition.
            if (!string.IsNullOrEmpty(xmlDocumentationSummaryTag))
            {
                fieldFormatter.AppendCodeLine(indentLevel,"/// <summary>");
                fieldFormatter.AppendCodeLine(indentLevel,$"/// {xmlDocumentationSummaryTag}");
                fieldFormatter.AppendCodeLine(indentLevel,"/// </summary>");
            }

            //String builder used to assemble the different parts of the field definition.
            var fieldBuilder = new StringBuilder();

            //Adding the security
            fieldBuilder.Append($"{security.CSharpFormatKeyword()} ");

            //If the constant keyword is required append it.
            if (constantKeyword) fieldBuilder.Append($"{Keywords.Constant} ");
            else
            {
                //If the static keyword is required append it.
                if (staticKeyword) fieldBuilder.Append($"{Keywords.Static} ");

                //if the read only keyword is required append it.
                if (readonlyKeyword) fieldBuilder.Append($"{Keywords.Readonly} ");
            }

            //Append the defined c# field type definition.
            fieldBuilder.Append($"{fieldType} ");

            //Append the formatted field name.
            fieldBuilder.Append(formattedFieldName);

            //Append the ending C# statement if the default value syntax was provided it is added here.
            fieldBuilder.Append(string.IsNullOrEmpty(defaultValueSyntax) ?";":$" = {defaultValueSyntax};");

            //Add the fully formatted field definition to the formatter to be written to the source file.
            fieldFormatter.AppendCodeLine(indentLevel,fieldBuilder.ToString());

            //Add the field definition to the end of the fields section in the hosting container in the source file.
            await updateContainerSource.FieldsAddAfterAsync(fieldFormatter.ReturnSource());

            //Return the updates source and container.
            return updateContainerSource;

        }

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
        public async Task<IUpdateInstanceSource<TContainerType>> CreateFieldSyntaxAsync(IUpdateInstanceSource<TContainerType> updateContainerSource, int indentLevel, string fieldName, CsType type,
            CsSecurity security, string defaultValueSyntax = null, 
            bool staticKeyword = false, bool constantKeyword = false, bool readonlyKeyword = false,
            string xmlDocumentationSummaryTag = null)
        {
            //Bounds checking
            if(updateContainerSource == null) throw new ArgumentNullException(nameof(updateContainerSource));
            if (type == null) throw new ArgumentNullException(nameof(type));

            //Loading the namespace manager
            updateContainerSource.LoadNamespaceManager();

            //Getting the correctly qualified name of the field type.
            var fieldType = type.CSharpFormatTypeName(updateContainerSource.NamespaceManager);

            return await CreateFieldSyntaxAsync(updateContainerSource, indentLevel, fieldName, fieldType, security,defaultValueSyntax,
                staticKeyword, constantKeyword, readonlyKeyword, xmlDocumentationSummaryTag);
        }

        /// <summary>
        /// Creates the fully formatted field name.
        /// </summary>
        /// <param name="fieldName">The source field name to use for formatting.</param>
        /// <returns>The fully formatted field name.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the field name is null or empty.</exception>
        public string CreateFieldName(string fieldName)
        {
            if(string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException(nameof(fieldName));

            //Building the name of the field
            var formattedFieldNameBuilder = new StringBuilder();

            //Adding the prefix if one was provided
            if (!string.IsNullOrEmpty(_fieldNamePrefix)) formattedFieldNameBuilder.Append(_fieldNamePrefix);

            //Adding the field name. formatting for camel case if requested.
            formattedFieldNameBuilder.Append( !_camelCaseForFieldName ?fieldName : fieldName.ConvertToCamelCase());

            //Adding the suffix if one was provided
            if (!string.IsNullOrEmpty(_fieldNameSuffix)) formattedFieldNameBuilder.Append(_fieldNameSuffix);

            //Storing the formatted name of the field.
            return formattedFieldNameBuilder.ToString();
        }
    }
}
