using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace Draeger.Dynamics365.Testautomation.Common
{
    static class SerilogExtensions
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

        public static void TestResult(
            this LoggerWrapper logger,
            string messageTemplate,
            params object[] args)
        {
            logger.ForContext("MessageType", "TestResult")
              .Information(messageTemplate, args);
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

                var retVar = (T)method.DynamicInvoke(extendedMethodParameters.ToArray());
                logger.Step("{ClassName} {MethodName} {@Parameters} with value {@ReturnValue}", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar);
                return retVar;
            }
            catch (Exception)
            {

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

                var retVar = (T)method.DynamicInvoke(extendedMethodParameters.ToArray());
                logger.ExpectedResult("{ClassName} {MethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);

                return (T)retVar;
            }
            catch (Exception)
            {

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

                var retVar = (T)method.DynamicInvoke(extendedMethodParameters.ToArray());
                logger.ExpectedResult("{ClassName} {MethodName} {@Parameters} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, logdict, retVar, expectedResult, expectedResultMsg);

                if (retVar is string s)
                    return s.Contains((string)expectedResult);

                return retVar.Equals(expectedResult);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static bool LogExpectedResultCheck<TDelegate, T>(this LoggerWrapper logger, TDelegate method, object expectedResult, string expectedResultMsg) where TDelegate : Delegate
        {  
            try
            {
                var retVar = (T)method.DynamicInvoke();
                logger.ExpectedResult("{ClassName} {MethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name, method.Method.Name, retVar, expectedResult, expectedResultMsg);

                if (retVar is string s)
                    return s.Contains((string) expectedResult);
                return retVar.Equals(expectedResult);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static T LogExpectedResult<TDelegate, T>(this LoggerWrapper logger, TDelegate method, object expectedResult, string expectedResultMsg) where TDelegate : Delegate
        {
            try
            {

                var retVar = (T)method.DynamicInvoke();
                logger.ExpectedResult("{ClassName} {MethodName} with value {@ReturnValue} expected result {expectedResult} ({expectedResultMessage})", method.Method.DeclaringType.Name,method.Method.Name, retVar, expectedResult, expectedResultMsg);

                return (T)retVar;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static void Log<T>(this LoggerWrapper logger, T method) where T : Delegate
        {
            logger.Step("{ClassName} {MethodName}",method.Method.DeclaringType.Name ,method.Method.Name);
            try
            {
                method.DynamicInvoke();
            }
            catch (Exception)
            {

                throw;
            }
        }


        internal static void Log<T>(this LoggerWrapper logger, T method, params object[] methodParameters) where T : Delegate
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
            logger.Step("{ClassName} {MethodName} {@Parameters}",  method.Method.DeclaringType.Name, method.Method.Name, logdict);
            try
            {
                method.DynamicInvoke(extendedMethodParameters.ToArray());
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static T LogSet<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, T value)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();

            logger.Step("Set {ClassName} {Property} to {Value}", propertyInfo.DeclaringType.Name,propertyInfo.Name, value);
            propertyInfo.SetValue(propertyOwner, value, null);
            return value;
        }

        public static T LogGet<T>(this LoggerWrapper logger, Expression<Func<T>> lambda)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();

            var value = propertyInfo.GetValue(propertyOwner, null);
            logger.Step("Get {ClassName} {Property} with value {Value}", propertyInfo.DeclaringType.Name,propertyInfo.Name, value);
            return (T)value;
        }

        public static T LogGetExpectedResult<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, object expectedResult, string expectedResultMsg)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();

            var value = propertyInfo.GetValue(propertyOwner, null);
            logger.ExpectedResult("Get {ClassName} {Property} with value {@Value} expected result {@expectedResult} ({expectedResultMessage})", propertyInfo.DeclaringType.Name, propertyInfo.Name, value, expectedResult, expectedResultMsg);
            
            return (T)value;
        }



        public static bool LogGetExpectedResultCheck<T>(this LoggerWrapper logger, Expression<Func<T>> lambda, object expectedResult, string expectedResultMsg)
        {
            var memberExpression = (MemberExpression)lambda.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyOwnerExpression = (MemberExpression)memberExpression.Expression;
            var propertyOwner = Expression.Lambda(propertyOwnerExpression).Compile().DynamicInvoke();
            

            var value = propertyInfo.GetValue(propertyOwner, null);
            logger.ExpectedResult("Get {ClassName} {Property} with value {@Value} expected result {@expectedResult} ({expectedResultMessage})", propertyInfo.DeclaringType.Name, propertyInfo.Name, value, expectedResult, expectedResultMsg);

            return value.Equals(expectedResult);
        }

    }
}
