//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

namespace CodeFactory.ADK
{
    /// <summary>
    /// Contract definition that all code block syntax items must emit.
    /// </summary>
    public interface ICodeBlockSyntax
    {
        /// <summary>
        /// Unique name assigned to the code block syntax.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of code block that is to be generated.
        /// </summary>
        CodeBlockType BlockType { get; }

        /// <summary>
        /// Generates the syntax for the code block and returns it.
        /// </summary>
        /// <param name="memberName">Optional parameter that passes the hosting member name for the code block.</param>
        /// <returns>Formatted syntax from the code block.</returns>
        string GenerateSyntax(string memberName = null);
    }
}
