//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using CodeFactory.DotNet.CSharp;

namespace CodeFactory.ADK
{
    /// <summary>
    /// Base class implementation for the contract interface <see cref="IBoundsCheck"/>
    /// </summary>
    public abstract class BoundsCheckBase:IBoundsCheck
    {
        private readonly string _name;
        private readonly bool _ignoreWhenDefaultValueIsSet;
        private readonly ILoggingFormatter _loggingFormatter;

        /// <summary>
        /// Initializes the base class for the bounds check.
        /// </summary>
        /// <param name="name">The unique name that identifies the type of bounds check being implemented.</param>
        /// <param name="ignoreWhenDefaultValueIsSet">Flag that determines if the bounds checking should be ignored if a default value is set.</param>
        /// <param name="loggingFormatter">Log formatter used with bounds check logic.</param>
        protected BoundsCheckBase(string name, bool ignoreWhenDefaultValueIsSet, ILoggingFormatter loggingFormatter)
        {
            _name = name;
            _ignoreWhenDefaultValueIsSet = ignoreWhenDefaultValueIsSet;
            _loggingFormatter = loggingFormatter;
        }

        /// <summary>
        /// Unique name assigned to identify the type of bounds check being performed.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Flag that determines if a bounds check should be ignored if the parameter has a default value set.
        /// </summary>
        public bool IgnoreWhenDefaultValueIsSet => _ignoreWhenDefaultValueIsSet;

        /// <summary>
        /// The log formatter assigned to this bounds check.
        /// </summary>
        public ILoggingFormatter LoggingFormatter => _loggingFormatter;

        /// <summary>
        /// Generates the bounds check syntax if the parameter meets the criteria for a bounds check.
        /// </summary>
        /// <param name="sourceMethod">The target method the parameter belongs to.</param>
        /// <param name="checkParameter">The parameter to build the bounds check for.</param>
        /// <returns>Returns a tuple that contains a boolean that determines if the bounds check syntax was created for the parameter.</returns>
        public abstract (bool hasBoundsCheck, string boundsCheckSyntax) GenerateSyntax(CsMethod sourceMethod,
            CsParameter checkParameter);

    }
}
