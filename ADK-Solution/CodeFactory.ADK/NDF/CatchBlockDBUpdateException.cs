//*****************************************************************************
//* Automation Delivery Kit
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using Microsoft.Extensions.Logging;


namespace CodeFactory.ADK.NDF
{
    /// <summary>
    /// CodeBlock that builds a catch block for DBUpdateException and returns a NDF managed exception.
    /// </summary>
    public class CatchBlockDbUpdateException:CodeBlockSyntaxBase
    {
        /// <summary>
        /// Implementation of a catch block for the DBUpdateException type. That will throw a the targeted managed exception type.
        /// </summary>
        /// <param name="loggingFormatter">Optional the log formatter to use with the code block.</param>
        public CatchBlockDbUpdateException(ILoggingFormatter loggingFormatter = null) : base("NDFCatchBlockDbUpdateException",CodeBlockType.CatchStatement,loggingFormatter)
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

            formatter.AppendCodeLine(0,"catch (DbUpdateException updateDataException)");
            formatter.AppendCodeLine(0,"{");
            formatter.AppendCodeLine(1,"var sqlError = updateException.InnerException as SqlException;");
            formatter.AppendCodeLine(1);
            formatter.AppendCodeLine(1,"if (sqlError == null");
            formatter.AppendCodeLine(1,"{");
            if (LoggingFormatter != null)
            {
                formatter.AppendCodeLine(2, LoggingFormatter.InjectLoggingSyntax(LogLevel.Error, "The following database error occurred.","updateDataException"));
                formatter.AppendCodeLine(2, LoggingFormatter.InjectExitLoggingSyntax(LogLevel.Error));
            }
            formatter.AppendCodeLine(2, "throw new DataException();");
            formatter.AppendCodeLine(1,"}");


            if (LoggingFormatter != null)
            {
                formatter.AppendCodeLine(1, LoggingFormatter.InjectLoggingSyntax(LogLevel.Error, "The following SQL exception occurred.","sqlError"));
                formatter.AppendCodeLine(1, LoggingFormatter.InjectExitLoggingSyntax(LogLevel.Error));
            }
            formatter.AppendCodeLine(1,"sqlError.ThrowManagedException();");
            formatter.AppendCodeLine(0,"}");

            return formatter.ReturnSource();

        }
    }
}
