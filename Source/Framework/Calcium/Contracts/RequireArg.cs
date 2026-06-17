#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-06-05 13:50:01Z</CreationDate>
</File>
*/

#endregion

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calcium
{
	/// <summary>
	/// Utility class for validating method arguments.
	/// </summary>
	public static class RequireArg
	{
		/// <summary>
		/// This value is used to detect when the C# 10+
		/// caller argument expression is unavailable.
		/// </summary>
		const string missingExpression
			= "\u001fCallerArgumentExpression unavailable\u001f";

		static void RequireParameterName(string? parameterName,
										 string? memberName,
										 string? filePath,
										 int     lineNumber)
		{
			if (parameterName == null)
			{
				throw new ArgumentNullException(
					nameof(parameterName),
					"Parameter name or expression, must not be null. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			if (parameterName == missingExpression)
			{
				throw new ArgumentException(
					"Argument name was not supplied by the caller. "
					+ "Pass the argument name explicitly, or compile the caller with C# 10 or later.",
					nameof(parameterName));
			}
		}

		static string FormatCallerParts(string? memberName,
										string? filePath,
										int     lineNumber)
		{
			return $"Method '{memberName}', File '{filePath}', Line '{lineNumber}'";
		}

		// Instruct Resharper not to reformat this code because it makes the code less readable.
		// @formatter:off

		/// <summary>
		/// Throws an exception if the specified value is null.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value
		/// is <code>null</code>.
		/// </exception>
		/// <example>
		/// public UIElementAdapter(UIElement uiElement)
		/// {
		/// this.uiElement = AssertArg.IsNotNull(uiElement, nameof(uiElement));
		/// }
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static T IsNotNull<T>(
			[NotNull] T? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
			where T : class
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == null)
			{
				throw new ArgumentNullException(parameterName,
					"Argument must not be null. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is null.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value
		/// is <code>null</code>.
		/// </exception>
		/// <example>
		/// public void RejectIfNull(UIElement? uiElement)
		/// {
		/// this.uiElement = AssertArg.IsNotNull(uiElement?, nameof(uiElement));
		/// }
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static T IsNotNull<T>(
			[NotNull] T? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0) where T : struct
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == null)
			{
				throw new ArgumentNullException(parameterName,
					"Argument must not be null. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value.Value;
		}

		/// <summary>
		/// Throws an exception if the specified value is equal
		/// to its type's default value.
		/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/default-values" />
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="memberName">
		/// Compiler populated parameter that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified value
		/// is its default value.
		/// For example, if <c>value</c> is 0 for an <c>int</c>,
		/// or <c>null</c> for an <c>object</c>.
		/// </exception>
		/// <example>
		/// public UIElementAdapter(UIElement uiElement)
		/// {
		/// this.uiElement = AssertArg.IsNotNull(uiElement, nameof(uiElement));
		/// }
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		[return: NotNull]
		public static T IsNotDefault<T>(
			[NotNull] T value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value is null)
			{
				throw new ArgumentNullException(parameterName,
					"Argument must not be null. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			if (EqualityComparer<T>.Default.Equals(value, default!))
			{
				throw new ArgumentException(
					$"{parameterName} of type {typeof(T)} must not be the default value. "
					+ FormatCallerParts(memberName, filePath, lineNumber),
					parameterName);
			}

			return value;
		}

		/// <summary>Throws an exception if the specified value
		/// is <code>null</code> or empty (a zero length string)
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="memberName">Compiler populated parameter
		/// that provides the caller member name.</param>
		/// <param name="filePath">Compiler populated parameter
		/// that provides the file path to the caller.</param>
		/// <param name="lineNumber">Compiler populated parameter that provides
		/// the line number of where the method was called.</param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value is <code>null</code>
		/// or empty (a zero length string)</exception>
		/// <example>
		/// public DoSomething(string message)
		/// {
		///   this.message = AssertArg.IsNotNullOrEmpty(message, nameof(message));
		/// }
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static string IsNotNullOrEmpty(
			[NotNull] string? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == null)
			{
				throw new ArgumentNullException(parameterName, 
					"Argument must not be null or empty. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(
					"Argument must not be an empty string. "
					+ FormatCallerParts(memberName, filePath, lineNumber),
					parameterName);
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value
		/// is <code>null</code> or white space.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value
		/// is <code>null</code> or consists of only white space.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static string IsNotNullOrWhiteSpace(
			[NotNull] string? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == null)
			{
				throw new ArgumentNullException(parameterName,
					"Argument must not be null or white space. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException(
					"Argument must not be null or white space. "
					+ FormatCallerParts(memberName, filePath, lineNumber),
					parameterName);
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is an empty guid.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value is an empty guid.
		/// That is, if <c>value</c> equals <c>Guid.Empty</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static Guid IsNotEmpty(
			Guid value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == Guid.Empty)
			{
				throw new ArgumentException(
					"Guid argument must not be an empty guid. "
					+ FormatCallerParts(memberName, filePath, lineNumber),
					parameterName);
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is null or an empty guid.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified value is an empty Guid.
		/// That is, if <c>value</c> equals <c>Guid.Empty</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static Guid IsNotNullOrEmpty(
			[NotNull] Guid? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			Guid g = IsNotNull(value, parameterName,
							   memberName, filePath, lineNumber);

			return IsNotEmpty(g, parameterName,
							  memberName, filePath, lineNumber);
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be greater than the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsGreaterThan(
			int expected,
			int value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value > expected)
			{
				return value;
			}

			throw new ArgumentOutOfRangeException(parameterName,
				$"Argument must be greater than {expected} but was {value}. "
				+ FormatCallerParts(memberName, filePath, lineNumber));
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than zero.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsGreaterThanZero(
			int value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			return IsGreaterThan(0, value, parameterName,
								 memberName, filePath, lineNumber);
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater than zero.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static long IsGreaterThanZero(
			long value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			return IsGreaterThan(0, value, parameterName,
								 memberName, filePath, lineNumber);
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater than zero.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsGreaterThanZero(
			[NotNull] int? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			int nonNullable = IsNotNull(value, parameterName,
										memberName, filePath, lineNumber);

			return IsGreaterThan(0, nonNullable, parameterName,
								 memberName, filePath, lineNumber);
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be greater than the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter that provides
		/// the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static double IsGreaterThan(
			double expected,
			double value,

			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value > expected)
			{
				return value;
			}

			throw new ArgumentOutOfRangeException(parameterName,
				$"Argument must be greater than {expected} but was {value}. "
				+ FormatCallerParts(memberName, filePath, lineNumber));
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be greater than the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static long IsGreaterThan(
			long expected,
			long value,

			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value > expected)
			{
				return value;
			}

			throw new ArgumentOutOfRangeException(parameterName,
				$"Argument must be greater than {expected} but was {value}. "
				+ FormatCallerParts(memberName, filePath, lineNumber));
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than or equal to the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be greater than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than or equal to the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsGreaterThanOrEqual(
			int expected,
			int value,

			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,
			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value >= expected)
			{
				return value;
			}

			throw new ArgumentOutOfRangeException(parameterName,
				$"Argument must be greater than or equal to {expected} but was {value}. "
				+ FormatCallerParts(memberName, filePath, lineNumber));
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than or equal to the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be greater than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than or equal to the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static double IsGreaterThanOrEqual(
			double expected,
			double value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value >= expected)
			{
				return value;
			}

			throw new ArgumentOutOfRangeException(parameterName,
				$"Argument must be greater than or equal to {expected} but was {value}. "
				+ FormatCallerParts(memberName, filePath, lineNumber));
		}

		/// <summary>
		/// Throws an exception if the specified value is not greater
		/// than or equal to the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be greater than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not greater
		/// than or equal to the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static long IsGreaterThanOrEqual(
			long expected,
			long value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value >= expected)
			{
				return value;
			}

			throw new ArgumentOutOfRangeException(parameterName,
				$"Argument must be greater than or equal to {expected} but was {value}. "
				+ FormatCallerParts(memberName, filePath, lineNumber));
		}

		/// <summary>
		/// Throws an exception if the specified value is not less
		/// than the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be less than the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not less
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsLessThan(
			int expected,
			int value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value >= expected)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be less than {expected} but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is not less
		/// than the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be less than the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not less
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static double IsLessThan(
			double expected,
			double value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value >= expected)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be less than {expected} but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is not less
		/// than the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be less than the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not less
		/// than the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static long IsLessThan(
			long expected,
			long value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value >= expected)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be less than {expected} but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is not less
		/// than or equal to the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be less than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not less
		/// than or equal to the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsLessThanOrEqual(
			int expected,
			int value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value > expected)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be less than or equal to {expected} but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is not less
		/// than or equal to the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be less than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not less
		/// than or equal to the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static double IsLessThanOrEqual(
			double expected,
			double value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value > expected)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be less than or equal to {expected} but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is not less
		/// than or equal to the specified expected value.
		/// </summary>
		/// <param name="expected">
		/// The number which must be less than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is not less
		/// than or equal to the expected value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static long IsLessThanOrEqual(
			long expected,
			long value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value > expected)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be less than or equal to {expected} but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value does not fall
		/// between the expected low and expected high parameters.
		/// </summary>
		/// <param name="expectedLow">
		/// The number which must be greater than or equal to the value.
		/// </param>
		/// <param name="expectedHigh">
		/// The number which must be less than or equal to the value.
		/// </param>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the member.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The specified value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Occurs if the specified value is less
		/// than or equal to the expected low value or greater than the expected high value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static int IsBetweenInclusive(
			int expectedLow,
			int expectedHigh,
			int value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value < expectedLow || value > expectedHigh)
			{
				throw new ArgumentOutOfRangeException(parameterName,
					$"Argument must be greater than or equal to {expectedLow} "
					+ $"and less than or equal to {expectedHigh}, but was {value}. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			return value;
		}

		/// <summary>
		/// Throws an exception if the specified value is <code>null</code>
		/// or if the value is not of the specified type.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The value to test.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified value is <code>null</code>
		/// or of a type not assignable from the specified type.
		/// </exception>
		/// <example>
		/// public DoSomething(object message)
		/// {
		/// this.message = AssertArg.IsNotNullAndOfType&lt;string&gt;(message, nameof(message));
		/// }
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		public static T IsNotNullAndOfType<T>(
			[NotNull] object? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0) where T : class
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == null)
			{
				throw new ArgumentNullException(parameterName,
					"Argument must not be null. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			var result = value as T;
			if (result == null)
			{
				throw new ArgumentException(
					$"Argument must be of type {typeof(T)}, but was {value.GetType()}. "
					+ FormatCallerParts(memberName, filePath, lineNumber),
					parameterName);
			}

			return result;
		}

		/// <summary>
		/// Throws an exception if the specified value is not of the type
		/// denoted by the type parameter <c>T</c>.
		/// </summary>
		/// <typeparam name="T">The expected type of the value.</typeparam>
		/// <param name="value">
		/// The parameter value passed to the calling method.
		/// </param>
		/// <param name="parameterName">
		/// The name of the parameter from the calling method.
		/// </param>
		/// <param name="memberName">
		/// Compiler populated parameter
		/// that provides the caller member name.
		/// </param>
		/// <param name="filePath">
		/// Compiler populated parameter
		/// that provides the file path to the caller.
		/// </param>
		/// <param name="lineNumber">
		/// Compiler populated parameter that provides
		/// the line number of where the method was called.
		/// </param>
		/// <returns>The value passed to the method.</returns>
		/// <exception cref="ArgumentException">
		/// If the specified value is not of the type
		/// denoted by the type parameter <c>T</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		[return: NotNull]
		public static T IsOfType<T>(
			[NotNull] object? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			if (value == null)
			{
				throw new ArgumentNullException(parameterName,
					"Argument must not be null. "
					+ FormatCallerParts(memberName, filePath, lineNumber));
			}

			if (value is T result)
			{
				return result;
			}

			throw new ArgumentException(
				$"Argument must be of type {typeof(T)}, but was {value.GetType()}. "
				+ FormatCallerParts(memberName, filePath, lineNumber),
				parameterName);
		}

		public static void AreNotNull(
			object? arg1, string arg1Name,
			object? arg2, string arg2Name,
			object? arg3 = null, string? arg3Name = null,
			object? arg4 = null, string? arg4Name = null,
			object? arg5 = null, string? arg5Name = null,
			object? arg6 = null, string? arg6Name = null,
			object? arg7 = null, string? arg7Name = null)
		{
			StringBuilder? nulls = null;

			void AddIfNull(object? obj, string? name)
			{
				if (name != null && obj == null)
				{
					if (nulls == null)
					{
						nulls = new StringBuilder();
					}
					else
					{
						nulls.Append(", ");
					}

					nulls.Append(name);
				}
			}

			AddIfNull(arg1, arg1Name);
			AddIfNull(arg2, arg2Name);
			AddIfNull(arg3, arg3Name);
			AddIfNull(arg4, arg4Name);
			AddIfNull(arg5, arg5Name);
			AddIfNull(arg6, arg6Name);
			AddIfNull(arg7, arg7Name);

			if (nulls != null)
			{
				throw new ArgumentNullException(
					nulls.ToString(),
					"The arguments cannot be null");
			}
		}

		public static ICollection<TItem> IsNotNullOrEmpty<TItem>(
			[NotNull] ICollection<TItem>? value, 
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			RequireParameterName(parameterName, memberName, filePath, lineNumber);

			return IsNotNullOrEmpty<ICollection<TItem>, TItem>(
				value,
				parameterName,
				memberName, filePath, lineNumber);
		}

		[return: NotNull]
		public static TCollection IsNotNullOrEmpty<TCollection, TItem>(
			[NotNull] TCollection? value,
			[CallerArgumentExpression(nameof(value))]
			string? parameterName = missingExpression,

			[CallerMemberName] string? memberName = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
			where TCollection : class, ICollection<TItem>
		{
			RequireParameterName(parameterName,
								 memberName, filePath, lineNumber);

			IsNotNull(value, parameterName,
					  memberName, filePath, lineNumber);

			if (value.Count < 1)
			{
				throw new ArgumentException(
					$"{parameterName} cannot be empty.",
					parameterName);
			}

			return value;
		}

		// Re-enable Resharper's formatting.
		// @formatter:on
	}
}