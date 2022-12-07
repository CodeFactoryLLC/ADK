//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using CodeFactory.DotNet.CSharp;
using System;
using System.Threading.Tasks;
using CodeFactory.Formatting.CSharp;
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK
{
    public class InjectStandardProperty<TContainerType> : IInjectInstanceContainerMemberSyntax<TContainerType, CsProperty> where TContainerType : CsContainer
    {
        /// <summary>
        /// Syntax generator for creating the backing field for the target property.
        /// </summary>
        private readonly CreateFieldSyntax<TContainerType> _backingFieldSyntax;

        /// <summary>
        /// Creates a new instance of the <see cref="InjectStandardProperty{TContainerType}"/>
        /// </summary>
        /// <param name="backingFieldSyntax">Optional parameter to past the field creation syntax logic for the backing field for the property.</param>
        public InjectStandardProperty(CreateFieldSyntax<TContainerType> backingFieldSyntax = null)
        {
            _backingFieldSyntax = backingFieldSyntax;
        }

        /// <summary>
        /// Generates syntax to be injected into the target source.
        /// </summary>
        /// <param name="updateSource">The source and instance container to be updated.</param>
        /// <param name="member">The member model used to generate the syntax.</param>
        /// <param name="indentLevel">The starting indent level to use for formatting the syntax.</param>
        /// <param name="defaultLogLevel">Sets the default logging level if logging is enabled for the method. Default is Information level logging.</param>
        /// <param name="targetSecurity">Optional parameter that determines the target security level to set the syntax to.Default is unknown.</param>
        /// <param name="replace">Optional parameter that determines if an existing model should be replaced by the syntax for injection. Default is false.</param>
        /// <returns>The updated source implementation once the syntax is injected or the original source if no changed where made.</returns>
        public async Task<IUpdateInstanceSource<TContainerType>> InjectSyntaxAsync(IUpdateInstanceSource<TContainerType> updateSource, CsProperty member, int indentLevel,
            LogLevel defaultLogLevel = LogLevel.Information, CsSecurity targetSecurity = CsSecurity.Unknown,
            bool replace = false)
        {
            return await InjectSyntaxAsync(updateSource, member, indentLevel, defaultLogLevel, targetSecurity,PropertyGenerationType.PropertyStandard,  CsSecurity.Unknown, CsSecurity.Unknown, replace,
                false, CsSecurity.Unknown);
        }

        /// <summary>
        /// Generates syntax to be injected into the target source.
        /// </summary>
        /// <param name="updateSource">The source and instance container to be updated.</param>
        /// <param name="member">The member model used to generate the syntax.</param>
        /// <param name="indentLevel">The starting indent level to use for formatting the syntax.</param>
        /// <param name="defaultLogLevel">Sets the default logging level if logging is enabled for the method. Default is Information level logging.</param>
        /// <param name="targetSecurity">Optional parameter that determines the target security level to set the syntax to.Default is unknown.</param>
        /// <param name="targetGetSecurity">Optional parameter that determines the target get security level if the member has a get definition.</param>
        /// <param name="targetSetSecurity">Optional parameter that determines the target set security level if the member has a set definition.</param>
        /// <param name="replace">Optional parameter that determines if an existing model should be replaced by the syntax for injection. Default is false.</param>
        /// <param name="requireSetMethod">Optional parameter that determines if the property must have a set method. This parameter is only used when the member does not have a set.</param>
        /// <param name="requiredSetSecurity">The target <see cref="CsSecurity"/> to set the required set. This is only used with <see cref="requiredSetSecurity"/> is true and the member does not have a set definition.</param>
        /// <returns>The updated source implementation once the syntax is injected or the original source if no changed where made.</returns>
        public async Task<IUpdateInstanceSource<TContainerType>> InjectSyntaxAsync(IUpdateInstanceSource<TContainerType> updateSource, CsProperty member, int indentLevel,
            LogLevel defaultLogLevel = LogLevel.Information, CsSecurity targetSecurity = CsSecurity.Unknown, PropertyGenerationType propertyType = PropertyGenerationType.PropertyStandard, CsSecurity targetGetSecurity = CsSecurity.Unknown,
            CsSecurity targetSetSecurity = CsSecurity.Unknown,  bool replace = false, bool requireSetMethod = false, CsSecurity requiredSetSecurity = CsSecurity.Protected)
        {
            if (updateSource == null) throw new ArgumentNullException(nameof(updateSource));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (updateSource.Source == null)
                throw new ArgumentException($"The parameter '{nameof(updateSource)}' does not have a source value set.");
            if (updateSource.Container == null)
                throw new ArgumentException($"The parameter '{nameof(updateSource)}' does not have a container value set.");

            var propertySecurity = targetSecurity == CsSecurity.Unknown ? member.Security : targetSecurity;

            var updatedSource = updateSource;

            updatedSource = await member.AddMissingUsingStatementsAsync(updatedSource) as IUpdateInstanceSource<TContainerType>;

            if (updatedSource == null)
                throw new CodeFactoryException(
                    "The update source for the property did not load correctly, could not load the property type using statements.");

            CsSecurity propSecurity = targetGetSecurity == CsSecurity.Unknown ? member.Security : targetSecurity;

            bool hasGet = member.HasGet;

            CsSecurity getSecurity = CsSecurity.Unknown;
            if (hasGet)
            {
                if (member.GetSecurity != CsSecurity.Unknown) getSecurity = member.GetSecurity;
                else
                {
                    getSecurity = targetGetSecurity == CsSecurity.Unknown ? propertySecurity : targetGetSecurity;
                }
            }

            bool hasSet = member.HasSet;

            CsSecurity setSecurity = CsSecurity.Unknown;

            if (!hasSet) hasSet = requireSetMethod;
            

            if (hasSet & !requireSetMethod) setSecurity = targetSetSecurity == CsSecurity.Unknown ? propertySecurity : targetSetSecurity;
            
            else if(hasSet & requireSetMethod) setSecurity = requiredSetSecurity == CsSecurity.Unknown ? propertySecurity : requiredSetSecurity;
            

            SourceFormatter propertyFormatter = new SourceFormatter(); 

            //Add the backing field for the property if for field create syntax was provided.
            if (_backingFieldSyntax != null)
            {
                updatedSource = await _backingFieldSyntax.CreateFieldSyntaxAsync(updatedSource, indentLevel,
                    member.Name, member.PropertyType, CsSecurity.Private, null, false, false, false,
                    $"Backing field for the property '{member.Name}'");


                var fieldName = _backingFieldSyntax.CreateFieldName(member.Name);

                int indentLevel1 = indentLevel + 1;

                if (member.HasDocumentation)
                {
                    var docs = member.CSharpGenerateXmlDocsSyntax(0);
                    if (!string.IsNullOrEmpty(docs)) propertyFormatter.AppendCodeBlock(indentLevel, docs);
                }

                string getStatement = null;
                string setStatement = null;

                if (propertyType == PropertyGenerationType.PropertyStandard)
                {
                    propertyFormatter.AppendCodeLine(indentLevel,
                        $"{propertySecurity.CSharpFormatKeyword()} {member.PropertyType.CSharpFormatTypeName(updatedSource.NamespaceManager)} {member.Name}");
                    propertyFormatter.AppendCodeLine(indentLevel, "{");

                    if (hasGet)
                    {
                        getStatement =
                            $"{(propertySecurity != getSecurity ? getSecurity.CSharpFormatKeyword() : null)} get {{return {fieldName}; }}";
                        propertyFormatter.AppendCodeLine(indentLevel1, getStatement.Trim());
                    }

                    if (hasSet)
                    {
                        setStatement =
                            $"{(propertySecurity != setSecurity ? setSecurity.CSharpFormatKeyword() : null)} set {{{fieldName} = value; }}";
                        propertyFormatter.AppendCodeLine(indentLevel1, setStatement.Trim());
                    }

                    propertyFormatter.AppendCodeLine(indentLevel, "}");
                }
                else
                {
                    if (hasGet & !hasSet)
                    {
                        getStatement =
                            $"{(propertySecurity != getSecurity ? getSecurity.CSharpFormatKeyword() : propertySecurity.CSharpFormatKeyword())} {member.PropertyType.CSharpFormatTypeName(updatedSource.NamespaceManager)} {member.Name} => {fieldName};";
                        propertyFormatter.AppendCodeLine(indentLevel, getStatement);
                            
                    }
                    else
                    {
                        propertyFormatter.AppendCodeLine(indentLevel,
                            $"{propertySecurity.CSharpFormatKeyword()} {member.PropertyType.CSharpFormatTypeName(updatedSource.NamespaceManager)} {member.Name}");
                        propertyFormatter.AppendCodeLine(indentLevel, "{");

                        if (hasGet)
                        {
                            getStatement =
                                $"{(propertySecurity != getSecurity ? getSecurity.CSharpFormatKeyword() : null)} get => {fieldName};";
                            propertyFormatter.AppendCodeLine(indentLevel1, getStatement.Trim());
                        }

                        if (hasSet)
                        {
                            setStatement =
                                $"{(propertySecurity != setSecurity ? setSecurity.CSharpFormatKeyword() : null)} set => {fieldName} = value;";
                            propertyFormatter.AppendCodeLine(indentLevel1, setStatement.Trim());
                        }

                        propertyFormatter.AppendCodeLine(indentLevel, "}");
                    }
                }
            }
            else
            {
                propertyFormatter.AppendCodeLine(indentLevel,
                    $"{propertySecurity.CSharpFormatKeyword()} {member.PropertyType.CSharpFormatTypeName(updatedSource.NamespaceManager)} {member.Name} {{ {(hasGet ? $"{(propertySecurity != getSecurity ? getSecurity.CSharpFormatKeyword() : null)} get; " :null)}{(hasSet ? $"{(propertySecurity != setSecurity ? setSecurity.CSharpFormatKeyword() : null)} set; " :null)}}}");
            }

            await updatedSource.PropertiesAddAfterAsync(propertyFormatter.ReturnSource());
            return updatedSource;
        }

    }
}

