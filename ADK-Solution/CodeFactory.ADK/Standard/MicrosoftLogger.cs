//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK.Standard
{
    /// <summary>
    /// Standard syntax log formatter that implements the <see cref="ILogger"/> interface.
    /// </summary>
    public class MicrosoftLogger:BaseLoggingFormatter
    {
        /// <summary>
        /// Constructor for the logging implementation.
        /// </summary>
        /// <param name="fieldName">The name of the logger field.</param>
        public MicrosoftLogger(string fieldName) : base(fieldName,"LogTrace","LogDebug","LogInformation","LogWarning","LogError","LogCritical")
        {
            //Intentionally blank
        }
    }
}
