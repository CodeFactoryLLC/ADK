//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Base class implementation for Log formatting. 
    /// </summary>
    public abstract class BaseLoggingFormatter:ILoggingFormatter
    { 
        //Backing fields for properties
        private readonly string _fieldName;
        private readonly string _traceMethodSyntax;
        private readonly string _debugMethodSyntax;
        private readonly string _informationMethodSyntax;
        private readonly string _warningMethodSyntax;
        private readonly string _errorMethodSyntax;
        private readonly string _criticalMethodSyntax;

        /// <summary>
        /// Constructor for the base class implementation.
        /// </summary>
        /// <param name="fieldName">The name of the logger field.</param>
        /// <param name="traceMethodSyntax">The name of the trace method.</param>
        /// <param name="debugMethodSyntax">The name of the debug method.</param>
        /// <param name="informationMethodSyntax">The name of the information method.</param>
        /// <param name="warningMethodSyntax">The name of the warning method.</param>
        /// <param name="errorMethodSyntax">The name of the error method.</param>
        /// <param name="criticalMethodSyntax">The name of the critical method.</param>
        
        protected BaseLoggingFormatter(string fieldName, string traceMethodSyntax, string debugMethodSyntax, string informationMethodSyntax,
            string warningMethodSyntax, string errorMethodSyntax, string criticalMethodSyntax)
        {
            _fieldName = fieldName;
            _traceMethodSyntax = traceMethodSyntax;
            _debugMethodSyntax = debugMethodSyntax;
            _informationMethodSyntax = informationMethodSyntax;
            _warningMethodSyntax = warningMethodSyntax;
            _errorMethodSyntax = errorMethodSyntax;
            _criticalMethodSyntax = criticalMethodSyntax;
        }

        /// <summary>
        /// the field name used for injecting logger syntax.
        /// </summary>
        public string FieldName => _fieldName;

        /// <summary>
        /// Method name for the trace method.
        /// </summary>
        public string TraceMethodSyntax => _traceMethodSyntax;

        /// <summary>
        /// Method name for the debug method.
        /// </summary>
        public string DebugMethodSyntax => _debugMethodSyntax;

        /// <summary>
        /// Method name for the information method.
        /// </summary>
        public string InformationMethodSyntax => _informationMethodSyntax;

        /// <summary>
        /// Method name for the warning method.
        /// </summary>
        public string WarningMethodSyntax => _warningMethodSyntax;

        /// <summary>
        /// Method name for the error method.
        /// </summary>
        public string ErrorMethodSyntax => _errorMethodSyntax;

        /// <summary>
        /// Method name for the critical method. 
        /// </summary>
        public string CriticalMethodSyntax => _criticalMethodSyntax;

        /// <summary>
        /// Returns the name of the method used by the logging framework based on the provided logging level.
        /// </summary>
        /// <param name="level">The logging level to get the method name for.</param>
        /// <returns>The logging method name based on the logging level.</returns>
        public string LogMethodName(LogLevel level)
        {
            string methodName = null;

            switch (level)
            {
                case LogLevel.Trace:
                    methodName = _traceMethodSyntax;
                    break;
                case LogLevel.Debug:
                    methodName = _debugMethodSyntax;
                    break;
                case LogLevel.Information:
                    methodName = _informationMethodSyntax;
                    break;
                case LogLevel.Warning:
                    methodName = _warningMethodSyntax;
                    break;
                case LogLevel.Error:
                    methodName = _errorMethodSyntax;
                    break;
                case LogLevel.Critical:
                    methodName = _criticalMethodSyntax;
                    break;
                case LogLevel.None:
                    methodName = null;
                    break;
                default:
                    methodName = null;
                    break;
            }

            return methodName;
        }

        /// <summary>
        /// Create formatted logging syntax to be used with automation.
        /// </summary>
        /// <param name="level">The logging level for the logger syntax.</param>
        /// <param name="message">the target message for logging.</param>
        /// <param name="isFormattedMessage">optional parameter that determines if the string uses a $ formatted string for the message with double quotes in the formatted output.</param>
        /// <param name="exceptionSyntax">Optional parameter to pass the exception field name to be included with the logging.</param>
        /// <returns>The formatted logging syntax to be injected. If no message is provided will return null.</returns>
        public string InjectLoggingSyntax(LogLevel level, string message, bool isFormattedMessage = false, string exceptionSyntax = null)
        {
            if (string.IsNullOrEmpty(message)) return null;

            string loggingSyntax = null;
            if(!isFormattedMessage) loggingSyntax =  string.IsNullOrEmpty(exceptionSyntax)
                ? $"{_fieldName}.{LogMethodName(level)}(\"{message}\");"
                : $"{_fieldName}.{LogMethodName(level)}({exceptionSyntax}, \"{message}\");";
            else loggingSyntax =  string.IsNullOrEmpty(exceptionSyntax)
                ? $"{_fieldName}.{LogMethodName(level)}({message});"
                : $"{_fieldName}.{LogMethodName(level)}({exceptionSyntax}, {message});";

            return loggingSyntax;
        }

        /// <summary>
        /// Injects a logging message entering the target member name.
        /// </summary>
        /// <param name="level">The level to log the message at.</param>
        /// <param name="memberName">Optional parameter that provides the member name.</param>
        /// <returns>The formatted logging string.</returns>
        public string InjectEnterLoggingSyntax(LogLevel level, string memberName = null)
        {
            return !string.IsNullOrEmpty(memberName)
                ? $"{_fieldName}.{LogMethodName(level)}($\"Exiting '{{nameof({memberName})}}'\");"
                : $"{_fieldName}.{LogMethodName(level)}(\"Entering 'member'\");";
        }

        /// <summary>
        /// Injects a logging message exiting the target member name.
        /// </summary>
        /// <param name="level">The level to log the message at.</param>
        /// <param name="memberName">Optional parameter that provides the member name.</param>
        /// <returns>The formatted logging string.</returns>
        public string InjectExitLoggingSyntax(LogLevel level, string memberName = null)
        {
            return !string.IsNullOrEmpty(memberName)
                ? $"{_fieldName}.{LogMethodName(level)}($\"Exiting '{{nameof({memberName})}}'\");"
                : $"{_fieldName}.{LogMethodName(level)}(\"Exiting 'member'\");";
        }
    }
}
