/* [DV] I have been able to get this working. callExpression is null is .NET8.0 project. */
//using System.Diagnostics.CodeAnalysis;

//#nullable enable

//namespace Calcium.MissingTypes
//{
//	public static class TestCallerExpression
//	{
//		// This method returns the string representing the expression passed as "value"
//		public static string? GetExpression<T>(T value, [CallerArgumentExpression("value")] string? callerExpression = null)
//		{
//			return callerExpression;
//		}
//	}
//}
