//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Contract definition all logging formatters must implement.
    /// </summary>
    public interface ILoggingFormatter
    {
        /// <summary>
        /// the field name used for injecting logger syntax.
        /// </summary>
        string FieldName { get; }

        /// <summary>
        /// Method name for the trace method.
        /// </summary>
        string TraceMethodSyntax { get; }

        /// <summary>
        /// Method name for the debug method.
        /// </summary>
        string DebugMethodSyntax { get; }

        /// <summary>
        /// Method name for the information method.
        /// </summary>
        string InformationMethodSyntax { get; }

        /// <summary>
        /// Method name for the warning method.
        /// </summary>
        string WarningMethodSyntax { get; }

        /// <summary>
        /// Method name for the error method.
        /// </summary>
        string ErrorMethodSyntax { get; }

        /// <summary>
        /// Method name for the critical method. 
        /// </summary>
        string CriticalMethodSyntax { get; }

        /// <summary>
        /// Returns the name of the method used by the logging framework based on the provided logging level.
        /// </summary>
        /// <param name="level">The logging level to get the method name for.</param>
        /// <returns>The logging method name based on the logging level.</returns>
        string LogMethodName(LogLevel level);

        /// <summary>
        /// Create formatted logging syntax to be used with automation.
        /// </summary>
        /// <param name="level">The logging level for the logger syntax.</param>
        /// <param name="message">the target message for logging.</param>
        /// <param name="exceptionSyntax">Optional parameter to pass the exception field name to be included with the logging.</param>
        /// <returns>The formatted logging syntax to be injected. If no message is provided will return null.</returns>
        string InjectLoggingSyntax(LogLevel level, string message, string exceptionSyntax = null);


        /// <summary>
        /// Injects a logging message entering the target member name.
        /// </summary>
        /// <param name="level">The level to log the message at.</param>
        /// <param name="memberName">Optional parameter that provides the member name.</param>
        /// <returns>The formatted logging string.</returns>
        string InjectEnterLoggingSyntax(LogLevel level, string memberName = null);

        /// <summary>
        /// Injects a logging message exiting the target member name.
        /// </summary>
        /// <param name="level">The level to log the message at.</param>
        /// <param name="memberName">Optional parameter that provides the member name.</param>
        /// <returns>The formatted logging string.</returns>
        string InjectExitLoggingSyntax(LogLevel level,string memberName = null);


    }
}
