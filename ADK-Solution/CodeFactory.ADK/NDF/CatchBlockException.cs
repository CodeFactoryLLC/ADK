using System;
using Microsoft.Extensions.Logging;


namespace CodeFactory.ADK.NDF
{
    public class CatchBlockException:CodeBlockSyntaxBase
    {
        /// <summary>
        /// Implementation of a catch block for the <see cref="Exception"/> type. That will throw a UnhandledException.
        /// </summary>
        /// <param name="loggingFormatter">Optional the log formatter to use with the code block.</param>
        public CatchBlockException(ILoggingFormatter loggingFormatter = null) : base("NDFCatchBlockException",CodeBlockType.CatchStatement,loggingFormatter)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Generates the syntax for the code block and returns it.
        /// </summary>
        /// <param name="memberName">Optional parameter that passes the hosting member name for the code block.</param>
        /// <returns>Formatted syntax from the code block.</returns>
        public override string GenerateSyntax(string memberName = null)
        {
            SourceFormatter formatter = new SourceFormatter();

            formatter.AppendCodeLine(0,"catch (Exception unhandledException)");
            formatter.AppendCodeLine(0,"{");
            if (LoggingFormatter != null)
            {
                formatter.AppendCodeLine(1, LoggingFormatter.InjectLoggingSyntax(LogLevel.Error, "The following unhandled exception occurred, see exception details. Throwing a unhandled managed exception.",false,"unhandledException"));
                formatter.AppendCodeLine(1, LoggingFormatter.InjectExitLoggingSyntax(LogLevel.Error, memberName));
            }
            formatter.AppendCodeLine(1,"throw new UnhandledException();");
            formatter.AppendCodeLine(0,"}");

            return formatter.ReturnSource();

        }
    }
}
