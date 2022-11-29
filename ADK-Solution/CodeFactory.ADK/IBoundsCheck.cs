//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using CodeFactory.DotNet.CSharp;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Contract all bounds check types must implement.
    /// </summary>
    public interface IBoundsCheck
    {
        /// <summary>
        /// Unique name assigned to identify the type of bounds check being performed.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Flag that determines if a bounds check should be ignored if the parameter has a default value set.
        /// </summary>
        bool IgnoreWhenDefaultValueIsSet { get; }

        /// <summary>
        /// The log formatter assigned to this bounds check.
        /// </summary>
        ILoggingFormatter LoggingFormatter { get; }


        /// <summary>
        /// Generates the bounds check syntax if the parameter meets the criteria for a bounds check.
        /// </summary>
        /// <param name="sourceMethod">The target method the parameter belongs to.</param>
        /// <param name="checkParameter">The parameter to build the bounds check for.</param>
        /// <returns>Returns a tuple that contains a boolean that determines if the bounds check syntax was created for the parameter.</returns>
        (bool hasBoundsCheck, string boundsCheckSyntax) GenerateSyntax(CsMethod sourceMethod,
            CsParameter checkParameter);
    }
}
