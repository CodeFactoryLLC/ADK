//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using System;
using CodeFactory.DotNet.CSharp;
using Microsoft.Extensions.Logging;

namespace CodeFactory.ADK.Standard
{
    /// <summary>
    /// Bound checking to see if a parameter is null. 
    /// </summary>
    public class NullBoundsCheck:BoundsCheckBase
    {
        /// <summary>
        /// Initializes the base class for the bounds check.
        /// </summary>
        /// <param name="loggingFormatter">The logging formatter to inject into string bounds check. This is optional and can be null if you do not want logging.</param>
        public NullBoundsCheck(ILoggingFormatter loggingFormatter = null) : base("NDFNullBoundsCheck", true,loggingFormatter)
        {
            //Intentionally blank.
        }

        /// <summary>
        /// Generates the bounds check syntax if the parameter meets the criteria for a bounds check.
        /// </summary>
        /// <param name="sourceMethod">The target method the parameter belongs to.</param>
        /// <param name="checkParameter">The parameter to build the bounds check for.</param>
        /// <returns>Returns a tuple that contains a boolean that determines if the bounds check syntax was created for the parameter.</returns>
        public override (bool hasBoundsCheck, string boundsCheckSyntax) GenerateSyntax(CsMethod sourceMethod, CsParameter checkParameter)
        {
            //bounds check to make sure we have parameter data.
            if (checkParameter == null) return (false, null);

            if(checkParameter.ParameterType.IsValueType) return (false, null);

            if(checkParameter.HasDefaultValue) return (false, null);

            SourceFormatter formatter = new SourceFormatter();

            formatter.AppendCodeLine(0,$"if ({checkParameter.Name} == null)");
            formatter.AppendCodeLine(0,"{");
            if (LoggingFormatter != null)
            {
                formatter.AppendCodeLine(1,LoggingFormatter.InjectLoggingSyntax(LogLevel.Error, 
                    string.Format("$\"The parameter {nameof({0})} was not provided. Will raise an argument exception\"", checkParameter.Name),null));
                formatter.AppendCodeLine(1, LoggingFormatter.InjectExitLoggingSyntax(LogLevel.Error));
            }
            formatter.AppendCodeLine(1,$"throw new ArgumentNullException(nameof({checkParameter.Name}));");
            formatter.AppendCodeLine(0,"}");
            
            return (true,formatter.ReturnSource());
        }
    }
}
