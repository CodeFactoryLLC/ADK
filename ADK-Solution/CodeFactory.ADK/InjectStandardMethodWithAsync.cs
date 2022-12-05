//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CodeFactory.DotNet.CSharp;
using CodeFactory.Formatting.CSharp;
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Creates a implementation of a method from the provided method member model. If the target type supports <see cref="Task"/> will implement with the async keyword. 
    /// </summary>
    /// <typeparam name="TContainerType">The target instance container type to inject the method into.</typeparam>
    public class InjectStandardMethodWithAsync<TContainerType>:IInjectInstanceContainerMemberSyntax<TContainerType,CsMethod> where TContainerType : CsContainer
    {
        //Local fields
        private readonly ILoggingFormatter _logger;

        private readonly IReadOnlyList<IBoundsCheck> _boundsChecks;

        private readonly IReadOnlyList<ICodeBlockSyntax> _codeBlocks;

        /// <summary>
        /// Create an instance of <see cref="InjectStandardMethodWithAsync{TContainerType}"/>
        /// </summary>
        /// <param name="logger">The logging formatter to use when generating the method. Null if you do not what to include logging.</param>
        /// <param name="boundsChecks">Enumeration of the bounds checks you wish to include. Null if you do not want to include bounds checking.</param>
        /// <param name="codeBlocks">Enumeration of the catch code blocks you wish to include. Null if you do not want catch blocks in your method.</param>
        public InjectStandardMethodWithAsync(ILoggingFormatter logger = null, IEnumerable<IBoundsCheck> boundsChecks = null, IEnumerable<ICodeBlockSyntax> codeBlocks = null)
        {
            _logger = logger;
            _boundsChecks = boundsChecks.ToImmutableList() ?? ImmutableList<IBoundsCheck>.Empty;
            _codeBlocks = codeBlocks.ToImmutableList() ?? ImmutableList<ICodeBlockSyntax>.Empty;
        }


        /// <summary>
        /// Generates syntax to be injected into the target source.
        /// </summary>
        /// <param name="updateSource">The source and instance container to be updated.</param>
        /// <param name="member">The member model used to generate the syntax.</param>
        /// <param name="indentLevel">The starting ident level to use for formatting the syntax.</param>
        /// <param name="defaultLogLevel">Sets the default logging level if logging is enabled for the method. Default is Information level logging.</param>
        /// <param name="targetSecurity">Optional parameter that determines the target security level to set the syntax to.Default is unknown.</param>
        /// <param name="replace">Optional parameter that determines if an existing model should be replaced by the syntax for injection. Default is false.</param>
        /// <returns>The updated source implementation once the syntax is injected or the original source if no changed where made.</returns>
        public async Task<IUpdateInstanceSource<TContainerType>> InjectSyntaxAsync(IUpdateInstanceSource<TContainerType> updateSource, CsMethod member, int indentLevel,
            LogLevel defaultLogLevel = LogLevel.Information, CsSecurity targetSecurity = CsSecurity.Unknown,
            bool replace = false)
        {
            //Confirming the required models are provided to generate the syntax for injection.
            if (updateSource == null) throw new ArgumentNullException(nameof(updateSource));
            if (member == null) throw new ArgumentNullException(nameof(member));

            var memberSource = updateSource;

            //Getting the hashcode for the method.
            int methodHashCode = member.FormatCSharpComparisonHashCode();

            //Checking to see if the method already exists.
            bool methodExists = updateSource.Container.Methods.Any(m => m.FormatCSharpComparisonHashCode() == methodHashCode);

            //If the method exists and were not replacing it just return the current source and container.
            if (methodExists & !replace) return updateSource;

            //Determining if we have catch code blocks.
            bool hasCatchBlocks = _codeBlocks.Any(c => c.BlockType == CodeBlockType.CatchStatement);

            //Determining if we have bounds checks
            bool hasBoundsChecks = _boundsChecks.Any();

            //Determining if we have logging.
            bool hasLogging = _logger != null;

            //Adding target indent levels that will be needed for formatting the method
            int indentLevel1 = indentLevel + 1;

            SourceFormatter methodFormatter = new SourceFormatter();

            //Checking to see if the method has xml docs
            if (member.HasDocumentation)
            {
                //Generating the xml docs and adding it to the source formatter.
                var docs = member.CSharpGenerateXmlDocsSyntax();

                //if docs were returned add them to the syntax formatter. Update the indent level as each doc line is added.
                if(docs != null) {methodFormatter.AppendCodeBlock(indentLevel,docs);}
            }

            //Checking to make sure all namespaces that are used are registered as using statements at the source level.
            memberSource = await member.AddMissingUsingStatementsAsync(memberSource) as IUpdateInstanceSource<TContainerType>;

            //Confirming we got back a fully loaded member source.
            if (memberSource == null) throw new ArgumentNullException(nameof(updateSource));

            //Getting a copy of the namespace manager.
            var manager = memberSource.NamespaceManager;

            //Checking to see if the method has attribute definitions.
            if (member.HasAttributes)
            {
                foreach (var memberAttribute in member.Attributes)
                {   //Adding the attribute c# syntax to the method definition.
                    methodFormatter.AppendCodeLine(indentLevel, memberAttribute.CSharpFormatAttributeSignature(manager));
                }
            }

            //Formatting C# method signature
            methodFormatter.AppendCodeLine(indentLevel,member.CSharpFormatStandardMethodSignatureWithAsync(manager));
            methodFormatter.AppendCodeLine(indentLevel,"{");

            if (hasLogging)
            {
                //Adding the enter method logging.
                methodFormatter.AppendCodeLine(indentLevel1, _logger.InjectEnterLoggingSyntax(defaultLogLevel,member.Name));
            }

            //If the method has parameters and has provided bound checking process the bounds checking
            if (member.HasParameters & hasBoundsChecks)
            {
                foreach (var memberParameter in member.Parameters)
                {
                    foreach (var boundsCheck in _boundsChecks)
                    {   
                        //Calling the bounds check to see if the bounds check logic was generated
                        var check = boundsCheck.GenerateSyntax(member, memberParameter);

                        //If the bounds check was not generated check the next bounds check.
                        if (!check.hasBoundsCheck) continue;

                        //Add the bounds check logic 
                        methodFormatter.AppendCodeBlock(indentLevel1,check.boundsCheckSyntax);

                        //If logic was added then do not check the other bounds check types and fall out.
                        break;
                    }
                }
            }

            //Return type variables
            CsType returnType = member.ReturnType;
            bool hasReturnType = false;

            // Determining if the method has a return type.
            if (returnType != null)
            {
                //Checking to see if the return type is a task if so format for the correct return type.
                if (member.ReturnType.Namespace == "System.Threading.Tasks" & member.ReturnType.Name == "Task")
                {
                    //If the task is a generic then extract the target type and set that as the return type.
                    if (member.ReturnType.IsGeneric)
                    {
                        hasReturnType = true;

                        returnType = member.ReturnType.GenericTypes.FirstOrDefault();

                        // If we cannot find the return type then set the return type to false.
                        if (returnType == null)
                        {
                            hasReturnType = false;
                            returnType = null;
                        }
                    }
                    else
                    {
                        //Is strictly a Task type. No return statement is needed for the async method.
                        returnType = null;
                    }
                }
            }

            //Confirming that a return type is still set after checking the task.
            if (returnType != null)
            {

                //Format the result variable to support a default value being set if supported.
                if (returnType.IsValueType)
                {
                    hasReturnType = true;
                    methodFormatter.AppendCodeLine(indentLevel1, string.IsNullOrEmpty(returnType.ValueTypeDefaultValue) ? $"{returnType.CSharpFormatTypeName(manager)} result;" : $"{returnType.CSharpFormatTypeName(manager)} result = {returnType.ValueTypeDefaultValue};");
                    methodFormatter.AppendCodeLine(indentLevel1);
                }
                //Format the result variable and set its initial value to null.
                else
                {
                    hasReturnType = true;
                    methodFormatter.AppendCodeLine(indentLevel1, $"{returnType.CSharpFormatTypeName(manager)} result = null;");
                    methodFormatter.AppendCodeLine(indentLevel1);
                }

            }

            if (hasCatchBlocks)
            {
                //Adding try block
                methodFormatter.AppendCodeLine(indentLevel1, "try");
                methodFormatter.AppendCodeLine(indentLevel1, "{");
                methodFormatter.AppendCodeLine(indentLevel1);
                methodFormatter.AppendCodeLine(indentLevel1, "}");

                //Appending each catch block.
                foreach (var catchBlock in _codeBlocks.Where(c=> c.BlockType == CodeBlockType.CatchStatement))
                {
                    methodFormatter.AppendCodeBlock(indentLevel1,catchBlock.GenerateSyntax());
                }
            }

            if (hasLogging)
            {
                //Adding the exit logging for the method.
                methodFormatter.AppendCodeLine(indentLevel1, _logger.InjectExitLoggingSyntax(defaultLogLevel,member.Name));
            }

            if (hasReturnType)
            {
                //Setting the return of the result variable.
                methodFormatter.AppendCodeLine(indentLevel1,"return result;");
                methodFormatter.AppendCodeLine(indentLevel1);
            }

            methodFormatter.AppendCodeLine(indentLevel,"}");
            methodFormatter.AppendCodeLine(indentLevel);

            //Adding the formatted method to the end of the methods 
            await memberSource.MethodsAddAfterAsync(methodFormatter.ReturnSource());

            //Returning the member source.
            return memberSource;

        }
    }
}
