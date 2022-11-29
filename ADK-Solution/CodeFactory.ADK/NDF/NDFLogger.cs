//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK.NDF
{
    public class NDFLogger:BaseLoggingFormatter
    {
        /// <summary>
        /// Constructor for the logging implementation.
        /// </summary>
        /// <param name="fieldName">The name of the logger field.</param>
        public NDFLogger(string fieldName) : base(fieldName,"TraceLog","DebugLog","InformationLog","WarningLog","ErrorLog","CriticalLog")
        {
            //Intentionally blank
        }

        /// <summary>
        /// Create formatted logging syntax to be used with automation.
        /// </summary>
        /// <param name="level">The logging level for the logger syntax.</param>
        /// <param name="message">the target message for logging.</param>
        /// <param name="exceptionSyntax">Optional parameter to pass the exception field name to be included with the logging.</param>
        /// <returns>The formatted logging syntax to be injected. If no message is provided will return null.</returns>
        public new string InjectLoggingSyntax(LogLevel level, string message, string exceptionSyntax = null)
        {
            if (string.IsNullOrEmpty(message)) return null;

            return string.IsNullOrEmpty(exceptionSyntax)
                ? $"{FieldName}.{LogMethodName(level)}(\"{message}\");"
                : $"{FieldName}.{LogMethodName(level)}(\"{message}\",{exceptionSyntax});";
        }

        /// <summary>
        /// Injects a logging message entering the target member name.
        /// </summary>
        /// <param name="level">The level to log the message at.</param>
        /// <param name="memberName">Optional parameter that provides the member name.</param>
        /// <returns>The formatted logging string.</returns>
        public new string InjectEnterLoggingSyntax(LogLevel level, string memberName = null)
        {
            return $"{FieldName}.EnterLog({level});";
        }

        /// <summary>
        /// Injects a logging message exiting the target member name.
        /// </summary>
        /// <param name="level">The level to log the message at.</param>
        /// <param name="memberName">Optional parameter that provides the member name.</param>
        /// <returns>The formatted logging string.</returns>
        public new string InjectExitLoggingSyntax(LogLevel level, string memberName = null)
        {
            return $"{FieldName}.ExitLog({level});";
        }

    }
}
