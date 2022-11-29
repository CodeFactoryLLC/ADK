//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

namespace CodeFactory.ADK
{
    /// <summary>
    /// Enumeration that determines the type of code block being implemented.
    /// </summary>
    public enum CodeBlockType
    {
        /// <summary>
        /// Code block implements a try block.
        /// </summary>
        TryStatement = 0,

        /// <summary>
        /// Code block implements a catch block.
        /// </summary>
        CatchStatement = 1,

        /// <summary>
        /// Code block implements a finally block.
        /// </summary>
        FinallyStatement = 2,

        /// <summary>
        /// Code block type is unknown
        /// </summary>
        Unknown = 9999
    }
}
