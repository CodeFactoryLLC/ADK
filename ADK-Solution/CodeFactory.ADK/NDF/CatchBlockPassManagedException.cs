using System;
using Microsoft.Extensions.Logging;


namespace CodeFactory.ADK.NDF
{
    public class CatchBlockPassManagedException:CodeBlockSyntaxBase
    {
        /// <summary>
        /// Implementation of a catch block for the <see cref="Exception"/> type. That will throw a UnhandledException.
        /// </summary>
        /// <param name="loggingFormatter">Optional the log formatter to use with the code block.</param>
        public CatchBlockPassManagedException(ILoggingFormatter loggingFormatter = null) : base("NDFCatchBlockPassManagedException",CodeBlockType.CatchStatement,loggingFormatter)
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

            formatter.AppendCodeLine(0,"catch (ManagedException)");
            formatter.AppendCodeLine(0,"{");
            if (LoggingFormatter != null)
            {
                formatter.AppendCodeLine(1, LoggingFormatter.InjectExitLoggingSyntax(LogLevel.Information));
            }
            formatter.AppendCodeLine(1,"throw;");
            formatter.AppendCodeLine(0,"}");

            return formatter.ReturnSource();

        }
    }
}
