//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using Microsoft.Extensions.Logging;


namespace CodeFactory.ADK.NDF
{
    /// <summary>
    /// CodeBlock that builds a catch block for SqlExceptions and returns a NDF managed exception.
    /// </summary>
    public class CatchBlockSqlException:CodeBlockSyntaxBase
    {
        /// <summary>
        /// Implementation of a catch block for the SqlException type. That will throw the target managed exception type.
        /// </summary>
        /// <param name="loggingFormatter">Optional the log formatter to use with the code block.</param>
        public CatchBlockSqlException(ILoggingFormatter loggingFormatter = null) : base("NDFCatchBlockSQLException",CodeBlockType.CatchStatement,loggingFormatter)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Generates the syntax for the code block and returns it.
        /// </summary>
        /// <returns>Formatted syntax from the code block.</returns>
        public override string GenerateSyntax()
        {
            SourceFormatter formatter = new SourceFormatter();

            formatter.AppendCodeLine(0,"catch (SqlException sqlDataException)");
            formatter.AppendCodeLine(0,"{");
            if (LoggingFormatter != null)
            {
                formatter.AppendCodeLine(1, LoggingFormatter.InjectLoggingSyntax(LogLevel.Error, "The following SQL exception occurred.","sqlDataException"));
                formatter.AppendCodeLine(1, LoggingFormatter.InjectExitLoggingSyntax(LogLevel.Error));
            }
            formatter.AppendCodeLine(1,"sqlDataException.ThrowManagedException();");
            formatter.AppendCodeLine(0,"}");

            return formatter.ReturnSource();

        }
    }
}
