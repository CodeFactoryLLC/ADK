//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

namespace CodeFactory.ADK
{
    /// <summary>
    /// Determines the format the property will take when it is backed by a field.
    /// </summary>
    public enum PropertyGenerationType
    {
        /// <summary>
        /// Standard notation.
        /// </summary>
        PropertyStandard = 0,

        /// <summary>
        /// Expression based notation.
        /// </summary>
        PropertyExpression = 1,
    }
}
