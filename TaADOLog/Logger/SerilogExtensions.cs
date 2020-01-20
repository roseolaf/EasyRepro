using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaADOLog.Logger
{
    public static class SerilogExtensions
    {
     
        public static void Step(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "Step")
              .Information(messageTemplate, args);
        }

        public static void ExpectedResult(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "ExpectedResult")
              .Information(messageTemplate, args);
        }
        public static void ExpectedResultFail(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "ExpectedResult")
                .Fatal(messageTemplate, args);
        }

        public static void TestResult(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "TestResult")
              .Information( messageTemplate, args);
        }

        public static void Error(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "Error")
              .Error(messageTemplate, args);
        }

        public static void Fail(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "Fail")
              .Fatal(messageTemplate, args);
        }

        public const string VerboseScreenshot = "VerboseScreenshotBeforeInvoke";
        static Regex classNameRx = new Regex(@"\w+$");
        public static T Log<TDelegate, T>(this LoggerWrapper logger, TDelegate method, params object[] methodParameters) where TDelegate : Delegate
        {
            var paramRawName = method.Method.GetParameters();


            var @params = new Dictionary<ParameterInfo, object>();
            var extendedMethodParameters = methodParameters.ToList();
            for (int i = 0; i < paramRawName.Length; i++)
            {
                if (i < methodParameters.Length)
                {
                    @params.Add(paramRawName[i], methodParameters[i]);
                }
                else
                {
                    @params.Add(paramRawName[i], paramRawName[i].DefaultValue);
                    extendedMethodParameters.Add(paramRawName[i].DefaultValue);
                }
            }
            var logdict = @params.ToDictionary(x => x.Key.Name, y => y.Value);
            try
            {

                logger.Verbose(VerboseScreenshot);
                var retVar = (T)method.DynamicInvoke(extendedMethodParameters.ToArray());
                logger.Step("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue}", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar);
                return retVar;
            }
            catch (Exception)
            {
                logger.Error("{ActionClassName} {ActionMethodName} {@Parameters}", method.Method.DeclaringType.Name, method.Method.Name, logdict);

                throw;
            }
        }
        public static T LogExpectedResult<TDelegate, T>(this LoggerWrapper logger, TDelegate method, object expectedResult, string expectedResultMsg, params object[] methodParameters) where TDelegate : Delegate
        {
            var paramRawName = method.Method.GetParameters();


            var @params = new Dictionary<ParameterInfo, object>();
            var extendedMethodParameters = methodParameters.ToList();
            for (int i = 0; i < paramRawName.Length; i++)
            {
                if (i < methodParameters.Length)
                {
                    @params.Add(paramRawName[i], methodParameters[i]);
                }
                else
                {
                    @params.Add(paramRawName[i], paramRawName[i].DefaultValue);
                    extendedMethodParameters.Add(paramRawName[i].DefaultValue);
                }
            }
            var logdict = @params.ToDictionary(x => x.Key.Name, y => y.Value);
            try
            {
                logger.Verbose(VerboseScreenshot);
                var retVar = (T)method.DynamicInvoke(extendedMethodParameters.ToArray());
                
                logger.ExpectedResult("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);

                return (T)retVar;
            }
            catch (Exception)
            {
                logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} {@Parameters} with expected result {expectedResult} ({expectedResultMessage}) failed", method.Method.DeclaringType.Name, method.Method.Name, logdict, expectedResult, expectedResultMsg);

                throw;
            }
        }

        public static bool LogExpectedResultCheck<TDelegate, T>(this LoggerWrapper logger, TDelegate method, object expectedResult, string expectedResultMsg, params object[] methodParameters) where TDelegate : Delegate
        {
            var paramRawName = method.Method.GetParameters();


            var @params = new Dictionary<ParameterInfo, object>();
            var extendedMethodParameters = methodParameters.ToList();
            for (int i = 0; i < paramRawName.Length; i++)
            {
                if (i < methodParameters.Length)
                {
                    @params.Add(paramRawName[i], methodParameters[i]);
                }
                else
                {
                    @params.Add(paramRawName[i], paramRawName[i].DefaultValue);
                    extendedMethodParameters.Add(paramRawName[i].DefaultValue);
                }
            }
            var logdict = @params.ToDictionary(x => x.Key.Name, y => y.Value);
            try
            {

                logger.Verbose(VerboseScreenshot);
                var retVar = (T)method.DynamicInvoke(extendedMethodParameters.ToArray());
                //logger.ExpectedResult("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);

                if (retVar is string s)
                {
                    if (s.Contains((string)expectedResult))
                        logger.ExpectedResult("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);
                    else
                        logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);

                    return s.Contains((string) expectedResult);
                }

                if (retVar.Equals(expectedResult))
                    logger.ExpectedResult("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);
                else
                    logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);

                return retVar.Equals(expectedResult); 
            }
            catch (Exception)
            {
                logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} {@Parameters} with expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, expectedResult, expectedResultMsg);

                throw;
            }
        }
        public static bool LogExpectedResultCheck<TDelegate, T>(this LoggerWrapper logger, TDelegate method, object expectedResult, string expectedResultMsg) where TDelegate : Delegate
        {  
            try
            {
                logger.Verbose(VerboseScreenshot);
                var retVar = (T)method.DynamicInvoke();
                //logger.ExpectedResult("{ActionClassName} {ActionMethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);

                if (retVar is string s)
                {
                    if (s.Contains((string) expectedResult))
                        logger.ExpectedResult("{ActionClassName} {ActionMethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);
                    else
                        logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);

                    return s.Contains((string) expectedResult);
                }

                if (retVar.Equals(expectedResult))
                    logger.ExpectedResult("{ActionClassName} {ActionMethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);
                else
                    logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);

                return retVar.Equals(expectedResult);
            }
            catch (Exception)
            {
                logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} with expected result {expectedResult} ({expectedResultMessage}) failed", method.Method.DeclaringType.Name, method.Method.Name, expectedResult, expectedResultMsg);

                throw;
            }
        }
        public static T LogExpectedResult<TDelegate, T>(this LoggerWrapper logger, TDelegate method, object expectedResult, string expectedResultMsg) where TDelegate : Delegate
        {
            try
            {
                logger.Verbose(VerboseScreenshot);
                var retVar = (T)method.DynamicInvoke();
                logger.ExpectedResult("{ActionClassName} {ActionMethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);

                return (T)retVar;
            }
            catch (Exception)
            {
                logger.ExpectedResultFail("{ActionClassName} {ActionMethodName} with expected result {expectedResult} ({expectedResultMessage}) failed", method.Method.DeclaringType.Name, method.Method.Name, expectedResult, expectedResultMsg);

                throw;
            }
        }

        public static void Log<T>(this LoggerWrapper logger, T method) where T : Delegate
        {

            try
            {
                logger.Verbose(VerboseScreenshot);
                method.DynamicInvoke(); 
                logger.Step("{ClassNames} {ActionMethodName}", method.Method.DeclaringType.Name, method.Method.Name);

            }
            catch (Exception)
            {
                logger.Error("{ClassNames} {ActionMethodName}", method.Method.DeclaringType.Name, method.Method.Name);

                throw;
            }
        }


        public static void Log<T>(this LoggerWrapper logger, T method, params object[] methodParameters) where T : Delegate
        {
            var paramRawName = method.Method.GetParameters();

            var @params = new Dictionary<ParameterInfo, object>();
            var extendedMethodParameters = methodParameters.ToList();
            for (int i = 0; i < paramRawName.Length; i++)
            {
                if (i < methodParameters.Length)
                {
                    @params.Add(paramRawName[i], methodParameters[i]);
                }
                else
                {
                    @params.Add(paramRawName[i], paramRawName[i].DefaultValue);
                    extendedMethodParameters.Add(paramRawName[i].DefaultValue);
                }
            }
            var logdict = @params.ToDictionary(x => x.Key.Name, y => y.Value);
             try
            {
                logger.Verbose(VerboseScreenshot);
                method.DynamicInvoke(extendedMethodParameters.ToArray());
                logger.Step("{ActionClassName} {ActionMethodName} {@Parameters}", method.Method.DeclaringType.Name, method.Method.Name, logdict);

            }
            catch (Exception)
            {
                logger.Error("{ActionClassName} {ActionMethodName} {@Parameters}", method.Method.DeclaringType.Name, method.Method.Name, logdict);

                throw;
            }
        }

        public static T LogSetExpectedResult<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, T value, string expectedResultMsg)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();
            try
            {
                logger.Verbose(VerboseScreenshot);
                propertyInfo.SetValue(propertyOwner, value, null);
                logger.ExpectedResult("Set {ActionClassName} {@Properties} to {Value} with expected result ({expectedResultMessage})", propertyInfo.DeclaringType.Name, propertyInfo.Name, value, expectedResultMsg);

                return value;
            }
            catch (Exception)
            {
                logger.ExpectedResultFail("Set {ActionClassName} {@Properties} to {Value} with expected result ({expectedResultMessage}) failed", propertyInfo.DeclaringType.Name, propertyInfo.Name, value, expectedResultMsg);

                throw;
            }

        }

        public static T LogSet<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, T value)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();
            try
            {
                logger.Verbose(VerboseScreenshot);
                propertyInfo.SetValue(propertyOwner, value, null);
                logger.Step("Set {ActionClassName} {@Properties} to {Value}", propertyInfo.DeclaringType.Name, propertyInfo.Name, value);

                return value;
            }
            catch (Exception)
            {
                logger.Error("Set {ActionClassName} {@Properties} to {Value}", propertyInfo.DeclaringType.Name, propertyInfo.Name, value);

                throw;
            }
             
        }

        public static T LogGet<T>(this LoggerWrapper logger, Expression<Func<T>> lambda)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();

            try
            {
                logger.Verbose(VerboseScreenshot);
                var value = propertyInfo.GetValue(propertyOwner, null);
                logger.Step("Get {ActionClassName} {@Properties} with value {Value}", propertyInfo.DeclaringType.Name, propertyInfo.Name, value);
                return (T)value;
            }
            catch (Exception)
            {
                logger.Error("Get {ActionClassName} {@Properties} failed", propertyInfo.DeclaringType.Name, propertyInfo.Name);

                throw;
            }
          
        }

        public static T LogGetExpectedResult<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, object expectedResult, string expectedResultMsg)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();

            try
            {
                logger.Verbose(VerboseScreenshot);
                var value = propertyInfo.GetValue(propertyOwner, null);
                logger.ExpectedResult("Get {ActionClassName} {@Properties} with value {@Value} expected result {@expectedResult} ({expectedResultMessage})", propertyInfo.DeclaringType.Name, propertyInfo.Name, value, expectedResult, expectedResultMsg);

                return (T)value;
            }
            catch (Exception)
            {
                logger.ExpectedResultFail("Get {ActionClassName} {@Properties} and expected result {@expectedResult} ({expectedResultMessage}) failed", propertyInfo.DeclaringType.Name, propertyInfo.Name, expectedResult, expectedResultMsg);

                throw;
            }
       
        }



        public static bool LogGetExpectedResultCheck<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, object expectedResult, string expectedResultMsg)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();

            try
            {
                logger.Verbose(VerboseScreenshot);
                var value = propertyInfo.GetValue(propertyOwner, null);
                logger.ExpectedResult("Get {ActionClassName} {@Properties} with value {@Value} expected result {@expectedResult} ({expectedResultMessage})", propertyInfo.DeclaringType.Name, propertyInfo.Name, value, expectedResult, expectedResultMsg);

                return value.Equals(expectedResult);

            }
            catch (Exception e)
            {
                logger.ExpectedResultFail("Get {ActionClassName} {@Properties} and expected result {@expectedResult} ({expectedResultMessage}) failed", propertyInfo.DeclaringType.Name, propertyInfo.Name, expectedResult, expectedResultMsg);

                throw;
            }
         
        }

    }
}
