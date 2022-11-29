using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFactory.ADK
{
    public abstract class CodeBlockSyntaxBase:ICodeBlockSyntax
    {   //Backing fields for properties 
        private readonly string _name;
        private readonly CodeBlockType _blockType;

        /// <summary>
        /// Formatter to add logging to the code block.
        /// </summary>
        protected readonly ILoggingFormatter LoggingFormatter;

        /// <summary>
        /// Base constructor that implements the base class properties and fields to be inherited by the implementation.
        /// </summary>
        /// <param name="name">The unique name of the code block.</param>
        /// <param name="blockType">The type of code block that is implemented in syntax.</param>
        /// <param name="loggingFormatter">Optional the log formatter to use with the code block.</param>
        protected CodeBlockSyntaxBase(string name, CodeBlockType blockType, ILoggingFormatter loggingFormatter = null)
        {
            _name = name;
            _blockType = blockType;
            LoggingFormatter = loggingFormatter;
        }

        /// <summary>
        /// Unique name assigned to the code block syntax.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The type of code block that is to be generated.
        /// </summary>
        public CodeBlockType BlockType => _blockType;

        /// <summary>
        /// Generates the syntax for the code block and returns it.
        /// </summary>
        /// <returns>Formatted syntax from the code block.</returns>
        public abstract string GenerateSyntax();

    }
}
