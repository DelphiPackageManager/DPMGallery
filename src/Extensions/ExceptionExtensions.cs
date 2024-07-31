using System.Reflection;
using System.Text;
using System;

namespace DPMGallery.Extensions
{
    public static  class ExceptionExtensions
    {
        public static string ToDetailedString(this Exception exception)
        {
            var result = new StringBuilder();
            try
            {
                Exception currentException = exception;

                while (currentException != null)
                {
                    result.AppendFormat("Exception: {0}\n\n", currentException.GetType().Name);
                    result.AppendFormat("Message: {0}\n\n", currentException.Message);
                    result.AppendFormat("Stack Trace: {0}\n\n", currentException.StackTrace);

                    if (currentException is ReflectionTypeLoadException reflectionException && reflectionException.LoaderExceptions.Length != 0)
                    {
                        result.Append("Loader Exceptions: ");
                        foreach (Exception loaderException in reflectionException.LoaderExceptions)
                        {
                            if (loaderException != null)
                            {
                                result.AppendLine();
                                result.Append(loaderException.Message);
                            }
                        }
                        result.AppendLine();
                    }

                    if (currentException is TypeInitializationException typeEx)
                        result.AppendLine($"\nType name is: {typeEx.TypeName}.");

                    currentException = currentException.InnerException;
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                string resultSoFar = string.Empty;
                try
                {
                    resultSoFar = result.ToString();
                }
                catch { }
                return $"An error occurred while getting exception details for exception with message {ex.Message}. Details so far: {resultSoFar}";
            }
        }

    }
}
