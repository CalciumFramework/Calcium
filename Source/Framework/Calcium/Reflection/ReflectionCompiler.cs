#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-11 14:21:36Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calcium.Reflection
{
	/// <summary>
	/// This class creates delegates to call methods
	/// and retrieve and set property values.
	/// Its purpose is to improve an applications 
	/// performance by reducing the use of reflection.
	/// </summary>
	static class ReflectionCompiler
	{
		#region Method Callers
		/// <summary>
		/// Create an action that can be used
		/// to call a method on a object.
		/// </summary>
		/// <param name="methodInfo">
		/// The method info for the method you wish to call.
		/// </param>
		/// <returns>An action that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.</returns>
		public static Action<object, object[]> CreateMethodAction(
			MethodInfo methodInfo)
		{
			var parameters = methodInfo.GetParameters();
			var parametersLength = parameters.Length;

			Delegate compiledExpression 
				= CreateCompiledExpression<object>(
					methodInfo, parametersLength, parameters);

			var voidReturnType = methodInfo.ReturnType == typeof(void);

			Action<object, object[]> result = null;

			if (voidReturnType)
			{
				switch (parametersLength)
				{
					case 0:
						{
							var action = (Action<object>)compiledExpression;
							result = (o, args) => { action(o); };
							break;
						}
					case 1:
						{
							var action = (Action<object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0]); };
							break;
						}
					case 2:
						{
							var action = (Action<object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1]); };
							break;
						}
					case 3:
						{
							var action = (Action<object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2]); };
							break;
						}
					case 4:
						{
							var action = (Action<object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3]); };
							break;
						}
					case 5:
						{
							var action = (Action<object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4]); };
							break;
						}
					case 6:
						{
							var action = (Action<object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4], args[5]); };
							break;
						}
					case 7:
						{
							var action = (Action<object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6]); };
							break;
						}
					case 8:
						{
							var action = (Action<object, object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); };
							break;
						}
				}
			}
			else
			{
				switch (parametersLength)
				{
					case 0:
						{
							var func = (Func<object, object>)compiledExpression;
							result = (o, args) => func(o);
							break;
						}
					case 1:
						{
							var func = (Func<object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0]);
							break;
						}
					case 2:
						{
							var func = (Func<object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1]);
							break;
						}
					case 3:
						{
							var func = (Func<object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2]);
							break;
						}
					case 4:
						{
							var func = (Func<object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3]);
							break;
						}
					case 5:
						{
							var func = (Func<object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4]);
							break;
						}
					case 6:
						{
							var func = (Func<object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5]);
							break;
						}
					case 7:
						{
							var func = (Func<object, object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
							break;
						}
					case 8:
						{
							var func = (Func<object, object, object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
							break;
						}
				}
			}

			return result;
		}

		/// <summary>
		/// Create a func that can be used
		/// to call a method on a object.
		/// </summary>
		/// <param name="methodInfo">
		/// The method info of the method you wish to call.
		/// </param>
		/// <returns>A func that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.
		/// The last return argument is the result of the method call.
		/// If the method has a void return type, then <c>null</c>
		/// is always returned.</returns>
		public static Func<object, object[], object> CreateMethodFunc(
			MethodInfo methodInfo)
		{
			var parameters = methodInfo.GetParameters();
			var parametersLength = parameters.Length;

			var compiledExpression = CreateCompiledExpression<object>(methodInfo, parametersLength, parameters);

			Func<object, object[], object> result = null;

			var voidMethod = methodInfo.ReturnType == typeof(void);

			if (voidMethod)
			{
				switch (parametersLength)
				{
					case 0:
						{
							var action = (Action<object>)compiledExpression;
							result = (o, args) => { action(o); return null; };
							break;
						}
					case 1:
						{
							var action = (Action<object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0]); return null; };
							break;
						}
					case 2:
						{
							var action = (Action<object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1]); return null; };
							break;
						}
					case 3:
						{
							var action = (Action<object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2]); return null; };
							break;
						}
					case 4:
						{
							var action = (Action<object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3]); return null; };
							break;
						}
					case 5:
						{
							var action = (Action<object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4]); return null; };
							break;
						}
					case 6:
						{
							var action = (Action<object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4], args[5]); return null; };
							break;
						}
					case 7:
						{
							var action = (Action<object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6]); return null; };
							break;
						}
					case 8:
						{
							var action = (Action<object, object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => { action(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); return null; };
							break;
						}
				}
			}
			else
			{
				switch (parametersLength)
				{
					case 0:
						{
							var func = (Func<object, object>)compiledExpression;
							result = (o, args) => func(o);
							break;
						}
					case 1:
						{
							var func = (Func<object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0]);
							break;
						}
					case 2:
						{
							var func = (Func<object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1]);
							break;
						}
					case 3:
						{
							var func = (Func<object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2]);
							break;
						}
					case 4:
						{
							var func = (Func<object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3]);
							break;
						}
					case 5:
						{
							var func = (Func<object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4]);
							break;
						}
					case 6:
						{
							var func = (Func<object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5]);
							break;
						}
					case 7:
						{
							var func = (Func<object, object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
							break;
						}
					case 8:
						{
							var func = (Func<object, object, object, object, object, object, object, object, object, object>)compiledExpression;
							result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
							break;
						}
				}
			}

			return result;
		}

		/// <summary>
		/// Create a func that can be used
		/// to call a method on a object.
		/// </summary>
		/// <param name="methodInfo">
		/// The method info of the method you wish to call.
		/// </param>
		/// <typeparam name="TReturn">
		/// The return type of the method to invoke.</typeparam>
		/// <returns>A func that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.
		/// The last return argument is the result of the method call.
		/// If the method has a void return type, then <c>null</c>
		/// is always returned.</returns>
		internal static Func<object, object[], TReturn> CreateMethodFunc<TReturn>(
			MethodInfo methodInfo)
		{
			var parameters = methodInfo.GetParameters();
			var parametersLength = parameters.Length;

			var compiledExpression = CreateCompiledExpression<TReturn>(methodInfo, parametersLength, parameters);

			Func<object, object[], TReturn> result = null;

			switch (parametersLength)
			{
				case 0:
				{
					var func = (Func<object, TReturn>)compiledExpression;
					result = (o, args) => func(o);
					break;
				}
				case 1:
				{
					var func = (Func<object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0]);
					break;
				}
				case 2:
				{
					var func = (Func<object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1]);
					break;
				}
				case 3:
				{
					var func = (Func<object, object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1], args[2]);
					break;
				}
				case 4:
				{
					var func = (Func<object, object, object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1], args[2], args[3]);
					break;
				}
				case 5:
				{
					var func = (Func<object, object, object, object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4]);
					break;
				}
				case 6:
				{
					var func = (Func<object, object, object, object, object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5]);
					break;
				}
				case 7:
				{
					var func = (Func<object, object, object, object, object, object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
					break;
				}
				case 8:
				{
					var func = (Func<object, object, object, object, object, object, object, object, object, TReturn>)compiledExpression;
					result = (o, args) => func(o, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
					break;
				}
			}

			return result;
		}

		public static Func<object[], object> CreateConstructorFunc(
			ConstructorInfo method)
		{
			Func<object[], object> result = null;
			var parameters = method.GetParameters();

			Delegate compiledExpression = CreateCompiledExpression(method, parameters);
		
			int parametersLength = parameters.Length;

			switch (parametersLength)
			{
				case 0:
					{
						var func = (Func<object>)compiledExpression;
						result = o => func();
						break;
					}
				case 1:
					{
						var func = (Func<object, object>)compiledExpression;
						result = args => func(args[0]);
						break;
					}
				case 2:
					{
						var func = (Func<object, object, object>)compiledExpression;
						result = args => func(args[0], args[1]);
						break;
					}
				case 3:
					{
						var func = (Func<object, object, object, object>)compiledExpression;
						result = args => func(args[0], args[1], args[2]);
						break;
					}
				case 4:
					{
						var func = (Func<object, object, object, object, object>)compiledExpression;
						result = args => func(args[0], args[1], args[2], args[3]);
						break;
					}
				case 5:
					{
						var func = (Func<object, object, object, object, object, object>)compiledExpression;
						result = args => func(args[0], args[1], args[2], args[3], args[4]);
						break;
					}
				case 6:
					{
						var func = (Func<object, object, object, object, object, object, object>)compiledExpression;
						result = args => func(args[0], args[1], args[2], args[3], args[4], args[5]);
						break;
					}
				case 7:
					{
						var func = (Func<object, object, object, object, object, object, object, object>)compiledExpression;
						result = args => func(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
						break;
					}
				case 8:
					{
						var func = (Func<object, object, object, object, object, object, object, object, object>)compiledExpression;
						result = args => func(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
						break;
					}
			}

			return result;
		}

		static Delegate CreateCompiledExpression(ConstructorInfo method, ParameterInfo[] parameters)
		{
			int parametersLength = parameters.Length;
			var parameterExpressions = new ParameterExpression[parametersLength];
			var paramTypes = new Expression[parametersLength];

			for (int i = 0; i < parametersLength; i++)
			{
				ParameterExpression objectParameter = Expression.Parameter(typeof(object));
				parameterExpressions[i] = objectParameter;

				var parameterInfo = parameters[i];
				var expression = Expression.Convert(objectParameter, parameterInfo.ParameterType);
				paramTypes[i] = expression;
			}

			Expression newExpression = Expression.New(method, paramTypes);

			var compiledExpression = Expression.Lambda(newExpression, parameterExpressions).Compile();
			return compiledExpression;
		}

		static Delegate CreateCompiledExpression<TReturn>(
			MethodInfo method, 
			int parametersLength, 
			ParameterInfo[] parameters)
		{
			var parameterExpressions = new ParameterExpression[parametersLength + 1];
			parameterExpressions[0] = Expression.Parameter(typeof(object), "obj");

			var paramTypes = new Expression[parametersLength];

			for (int i = 0; i < parametersLength; i++)
			{
				var parameter = Expression.Parameter(typeof(object));
				/* Skip the first item as that is the object 
				 * on which the method is called. */
				parameterExpressions[i + 1] = parameter;

				var info = parameters[i];
				//string typeName = info.ParameterType.FullName.Replace("&", string.Empty);
				//var type = Type.GetType(typeName);
				var expression = Expression.Convert(parameter, info.ParameterType);
				paramTypes[i] = expression;
			}

			var instanceExpression = Expression.Convert(parameterExpressions[0], method.DeclaringType);

			var voidMethod = method.ReturnType == typeof(void);

			Expression callExpression;

			if (voidMethod)
			{
				callExpression = Expression.Call(instanceExpression, method, paramTypes);
			}
			else
			{
				callExpression = Expression.Convert(Expression.Call(instanceExpression, method, paramTypes), typeof(TReturn));
			}

			var compiledExpression = Expression.Lambda(callExpression, parameterExpressions).Compile();
			return compiledExpression;
		}

		#endregion

		#region Property Accessors
		public static Action<object, object> CreatePropertySetter(PropertyInfo property)
		{
			MethodInfo setMethod = property.SetMethod;

			if (setMethod == null 
				|| setMethod.GetParameters().Length != 1)
			{
				throw new ArgumentException(
					$"Property {property.DeclaringType}.{property.Name} " +
					$"has no setter or parameters Length not equal to 1.");
			}

			var obj = Expression.Parameter(typeof(object), "o");
			var value = Expression.Parameter(typeof(object));

			Expression<Action<object, object>> expr =
				Expression.Lambda<Action<object, object>>(
					Expression.Call(
						Expression.Convert(obj, setMethod.DeclaringType),
						setMethod,
						Expression.Convert(value, setMethod.GetParameters()[0].ParameterType)),
					obj,
					value);

			return expr.Compile();
		}

		internal static Action<object, TProperty> CreatePropertySetter<TProperty>(
			PropertyInfo propertyInfo)
		{
			MethodInfo setMethod = propertyInfo.SetMethod;

			if (setMethod == null || setMethod.GetParameters().Length != 1)
			{
				throw new ArgumentException(
					$"Property {propertyInfo.DeclaringType}.{propertyInfo.Name} " +
					"has no setter or parameters Length not equal to 1.");
			}

			var propertyType = typeof(TProperty);

			var obj = Expression.Parameter(typeof(object), "o");
			var value = Expression.Parameter(propertyType);

			Expression<Action<object, TProperty>> expr =
				Expression.Lambda<Action<object, TProperty>>(
					Expression.Call(
						Expression.Convert(obj, setMethod.DeclaringType),
						setMethod,
						Expression.Convert(value, setMethod.GetParameters()[0].ParameterType)),
					obj,
					value);

			return expr.Compile();
		}

		public static Func<object, object> CreatePropertyGetter(PropertyInfo property)
		{
			MethodInfo getMethod = property.GetMethod;

			if (getMethod == null || getMethod.GetParameters().Length != 0)
			{
				throw new ArgumentException(
					$"Property {property.DeclaringType}.{property.Name} " +
					"has no getter or parameters Length not equal to 0.");
			}

			var returnType = getMethod.ReturnType;

#if NETFX_CORE
			if (!returnType.GetTypeInfo().IsValueType)
			{
				return Compile<object>(getMethod);
			}
#else
			if (!returnType.IsValueType())
			{
				return Compile<object>(getMethod);
			}
#endif

			MethodInfo method = typeof(ReflectionCompiler).GetTypeInfo().GetDeclaredMethods(nameof(CoerceCompiled)).SingleOrDefault(x => x.IsStatic || x.IsPrivate);
			MethodInfo genericMethod = method.MakeGenericMethod(returnType);

			var compiled = (Func<object, object>)genericMethod.Invoke(null, new object[] { getMethod });
			return compiled;
		}

		public static Func<object, TProperty> CreatePropertyGetter<TProperty>(
			PropertyInfo property)
		{
			MethodInfo getMethod = property.GetMethod;

			if (getMethod == null || getMethod.GetParameters().Length != 0)
			{
				throw new ArgumentException(
					$"Property {property.DeclaringType}.{property.Name} " +
					$"has no getter or parameters Length not equal to 0.");
			}

			var returnType = getMethod.ReturnType;

			if (!typeof(TProperty).IsAssignableFromEx(returnType))
			{
				throw new ArgumentException(
					"TProperty " + typeof(TProperty) + 
					" is not assignable to property " + property.Name +
					" on class " + property.DeclaringType);
			}

#if NETFX_CORE
			if (!returnType.GetTypeInfo().IsValueType)
			{
				return Compile<object>(getMethod);
			}
#else
			if (!returnType.IsValueType())
			{
				return Compile<TProperty>(getMethod);
			}
#endif

			MethodInfo method = typeof(ReflectionCompiler).GetTypeInfo()
				.GetDeclaredMethods(nameof(CoerceCompiled))
				.Single(x => x.IsStatic || x.IsPrivate);
			MethodInfo genericMethod = method.MakeGenericMethod(returnType);

			var compiled = (Func<object, TProperty>)genericMethod.Invoke(null, new object[] { getMethod });
			return compiled;
		}

		#region NEW

		static PropertyInfo GetPropertyInfo<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			var member = GetMemberExpression(expression).Member;
			var property = member as PropertyInfo;
			if (property == null)
			{
				throw new InvalidOperationException(string.Format("Member with Name '{0}' is not a property.", member.Name));
			}
			return property;
		}

		static MemberExpression GetMemberExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			MemberExpression memberExpression = null;
			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				var body = (UnaryExpression)expression.Body;
				memberExpression = body.Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new ArgumentException("Not a member access", "expression");
			}

			return memberExpression;
		}

		public static Func<TEntity, TProperty> CreatePropertyGetter<TEntity, TProperty>(
													Expression<Func<TEntity, TProperty>> property)
		{
			PropertyInfo propertyInfo = GetPropertyInfo(property);

			ParameterExpression instance = Expression.Parameter(typeof(TEntity), "instance");

			var body = Expression.Call(instance, propertyInfo.GetMethod);
			var parameters = new ParameterExpression[] { instance };

			return Expression.Lambda<Func<TEntity, TProperty>>(body, parameters).Compile();
		}

		public static Func<TEntity, TProperty> CreatePropertyGetter<TEntity, TProperty>(PropertyInfo propertyInfo)
		{
			ParameterExpression instance = Expression.Parameter(typeof(TEntity), "instance");

			var body = Expression.Call(instance, propertyInfo.GetMethod);
			var parameters = new ParameterExpression[] { instance };

			return Expression.Lambda<Func<TEntity, TProperty>>(body, parameters).Compile();
		}

		#endregion

		static Func<object, object> CoerceCompiled<T>(
			MethodInfo getMethod)
		{
			var compiled = Compile<T>(getMethod);
			Func<object, object> result = o => compiled(o);
			return result;
		}

		static Func<object, T> Compile<T>(MethodInfo getMethod)
		{
			var obj = Expression.Parameter(typeof(object), "o");

			Expression<Func<object, T>> expr =
							Expression.Lambda<Func<object, T>>(
								Expression.Call(
									Expression.Convert(obj, getMethod.DeclaringType),
									getMethod),
								obj);

			return expr.Compile();
		}
		#endregion

		/// <summary>
		/// Creates a getter func for the specified 
		/// property info instance.
		/// Using a func to get the value of the property
		/// is faster than using reflection each time.
		/// </summary>
		/// <typeparam name="TProperty">
		/// The type of the property.</typeparam>
		/// <param name="propertyInfo">
		/// The property info instance for the property.
		/// </param>
		/// <param name="owner">
		/// The object that owns the property.</param>
		/// <returns>A func that can be called get
		/// the value of the property.</returns>
		public static Func<TProperty> CreateGetter<TProperty>(
			this PropertyInfo propertyInfo, object owner)
		{
			AssertArg.IsNotNull(propertyInfo, nameof(propertyInfo));
			AssertArg.IsNotNull(owner, nameof(owner));

			Type getterType = Expression.GetFuncType(propertyInfo.PropertyType);
			//Type getterType = typeof(Func<>).MakeGenericType(propertyInfo.PropertyType);

#if NETSTANDARD || NETFX_CORE
			MethodInfo methodInfo = propertyInfo.GetMethod;
			Func<TProperty> getter = (Func<TProperty>)methodInfo.CreateDelegate(
					getterType, owner);
#else
			Func<TProperty> getter = (Func<TProperty>)Delegate.CreateDelegate(
								getterType, owner, propertyInfo.GetGetMethod());
#endif
			return getter;
		}

		/// <summary>
		/// Creates a delegate that can be used to retrieve
		/// the value of a property.
		/// </summary>
		/// <typeparam name="TDelegate">
		/// The delgate type.</typeparam>
		/// <param name="propertyInfo">
		/// A property info instance.</param>
		/// <param name="owner">The owner of the property.</param>
		/// <returns>A delegate that can be invoked
		/// to retrieve the value of the property.</returns>
		public static TDelegate CreateGetterDelegate<TDelegate>(
			this PropertyInfo propertyInfo,
			object owner)
		{
			AssertArg.IsNotNull(propertyInfo, nameof(propertyInfo));
			AssertArg.IsNotNull(owner, nameof(owner));

#if NETSTANDARD || NETFX_CORE
			MethodInfo getMethodInfo = propertyInfo.GetMethod;
			object getter = getMethodInfo.CreateDelegate(
					typeof(TDelegate), owner);
#else
			object getter = Delegate.CreateDelegate(
								typeof(TDelegate), owner, propertyInfo.GetGetMethod());
#endif
			return (TDelegate)getter;
		}

		/// <summary>
		/// Creates a setter func for the specified 
		/// property info instance.
		/// Using a func to set the value of the property
		/// is faster than using reflection each time.
		/// </summary>
		/// <typeparam name="TProperty">
		/// The type of the property.</typeparam>
		/// <param name="propertyInfo">
		/// The property info instance for the property.
		/// </param>
		/// <param name="owner">
		/// The object that owns the property.</param>
		/// <returns>A func that can be called set
		/// the value of the property.</returns>
		public static Action<TProperty> CreateSetter<TProperty>(
			this PropertyInfo propertyInfo, object owner)
		{
			AssertArg.IsNotNull(propertyInfo, nameof(propertyInfo));
			AssertArg.IsNotNull(owner, nameof(owner));

			var propertyType = propertyInfo.PropertyType;
			var setterType = Expression.GetActionType(new[] { propertyType });

#if NETSTANDARD || NETFX_CORE
			MethodInfo setMethodInfo = propertyInfo.SetMethod;
			Delegate setter = setMethodInfo.CreateDelegate(setterType, owner);
#else
			Delegate setter = Delegate.CreateDelegate(
						setterType, owner, propertyInfo.GetSetMethod());
#endif
			return (Action<TProperty>)setter;
		}

		/// <summary>
		/// Retrieves the property info object for the specified expression.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="expression">An expression that 
		/// retrieves the property value. For example: () => MyProperty</param>
		/// <returns>The property info instance for the property.</returns>
		public static PropertyInfo GetPropertyInfo<T>(
			Expression<Func<T>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException(
					"MemberExpression expected.", nameof(expression));
			}

			if (memberExpression.Member == null)
			{
				throw new ArgumentException("Member should not be null.");
			}

#if NETSTANDARD || NETFX_CORE
			if (!(memberExpression.Member is PropertyInfo))
			{
				throw new ArgumentException("Property expected.", nameof(expression));
			}
#else
			if (memberExpression.Member.MemberType != MemberTypes.Property)
			{
				throw new ArgumentException("Property expected.", nameof(expression));
			}
#endif
			PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
			return propertyInfo;
		}

		#region Events
		/// <summary>
		/// Creates a handler for the specified event,
		/// so that when the event is raised, the specified
		/// action is invoked.
		/// </summary>
		/// <param name="eventInfo">The event.</param>
		/// <param name="action">
		/// The action to invoke when the event is raised.</param>
		/// <returns>A delegate that can be used 
		/// to subscribe to the event.</returns>
		/// <example>
		/// var handler = ReflectionCompiler.CreateEventHandler(eventInfo, ExecuteCommand);
		/// eventInfo.AddEventHandler(element, handler);
		/// </example>
		public static Delegate CreateEventHandler(
			EventInfo eventInfo, Action action)
		{
			AssertArg.IsNotNull(eventInfo, nameof(eventInfo));
			AssertArg.IsNotNull(action, nameof(action));

			/* Source: http://stackoverflow.com/questions/3478218/using-reflection-emit-to-implement-a-interface */
			Type handlerType = eventInfo.EventHandlerType;
			MethodInfo invokeMethodInfo = handlerType.GetTypeInfo().GetDeclaredMethod("Invoke");
			ParameterInfo[] eventParams = invokeMethodInfo.GetParameters();

			//lambda: (object x0, EventArgs x1) => d()
			IEnumerable<ParameterExpression> parameters
				= eventParams.Select(p => Expression.Parameter(p.ParameterType, p.Name));
			// - assumes void method with no arguments but can be
			//   changed to accomodate any supplied method
			MethodCallExpression body = Expression.Call(
				Expression.Constant(action), action.GetType().GetTypeInfo().GetDeclaredMethod("Invoke"));
			LambdaExpression lambda = Expression.Lambda(body, parameters.ToArray());
			
			Delegate compiledLambda = lambda.Compile();
			Delegate result = invokeMethodInfo.CreateDelegate(handlerType, compiledLambda);

			return result;
		}

		public static Delegate CreateEventHandlerUntyped(
			EventInfo eventInfo, Action action)
		{
			AssertArg.IsNotNull(eventInfo, nameof(eventInfo));
			AssertArg.IsNotNull(action, nameof(action));

			/* Source: http://stackoverflow.com/questions/3478218/using-reflection-emit-to-implement-a-interface */
			Type handlerType = eventInfo.EventHandlerType;
			MethodInfo invokeMethodInfo = handlerType.GetTypeInfo().GetDeclaredMethod("Invoke");
			ParameterInfo[] eventParams = invokeMethodInfo.GetParameters();

			//lambda: (object x0, EventArgs x1) => d()
			IEnumerable<ParameterExpression> parameters
				= eventParams.Select(p => Expression.Parameter(p.ParameterType, p.Name));
			// - assumes void method with no arguments but can be
			//   changed to accomodate any supplied method
			MethodCallExpression body = Expression.Call(
				Expression.Constant(action), action.GetType().GetTypeInfo().GetDeclaredMethod("Invoke"));
			LambdaExpression lambda = Expression.Lambda(body, parameters.ToArray());

			Delegate compiledLambda = lambda.Compile();
			
			return compiledLambda;
		}

		#endregion
	}
}
