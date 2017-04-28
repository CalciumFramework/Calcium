#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-07-21 09:41:27Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Codon.Logging;
using Codon.Reflection;

namespace Codon.UIModel
{
	/// <summary>
	/// This class is analagous to the Knockout JS style computed observables. 
	/// It is used to wrap a value and provides change notification for bindings 
	/// without adding plumbing to a viewmodel. 
	/// This class allows you to provide an expression that is parsed to locate associated objects 
	/// that implement <c>INotifyPropertyChanged</c>. When a change notification is received 
	/// from any such objects, the value of the expression is recomputed.
	/// Disposing of this object removes subscriptions to all associated objects.
	/// <see cref="http://danielvaughan.org/post/Knockout-Style-Observables-in-XAML.aspx" />
	/// <seealso cref="Observable{T}"/>
	/// </summary>
	/// <example>
	/// class MainWindowViewModel
	///{
	///    public MainWindowViewModel()
	///	   {
	///			fullName = new ComputedObservable&lt;string&gt;(() => FirstName.Value + " " + ToUpper(LastName.Value));
	///	   }
	/// ...
	/// }
	/// Bind to the value in XAML using:
	/// &lt;TextBlock Text="{Binding Path=FullName.Value}" /&gt;
	/// </example>
	/// <typeparam name="T">The type of result returned from executing the specified expression.</typeparam>
	public class ComputedObservable<T> : INotifyPropertyChanged, IDisposable
	{
		readonly Dictionary<PropertyChangedEventHandler, WeakReference> handlers
					= new Dictionary<PropertyChangedEventHandler, WeakReference>();

		readonly Func<T> valueFunc;

		/// <summary>
		/// Gets the value by executing the Lambda Expression that was supplied to the constructor.
		/// </summary>
		public T Value => valueFunc();

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="ComputedObservable{T}"/> class.
		/// </summary>
		/// <param name="expression">
		/// The expression that is evaluated when the value 
		/// of this object is requested. Any associated objects 
		/// that implement <c>INotifyPropertyChanged</c> 
		/// are automatically subscribed to.
		/// </param>
		public ComputedObservable(Expression<Func<T>> expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			Expression body = expression.Body;

			ProcessDependents(body);

			valueFunc = expression.Compile();
		}

		void ProcessDependents(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					ProcessUnaryExpression((UnaryExpression)expression);
					break;
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
				case ExpressionType.RightShift:
				case ExpressionType.LeftShift:
				case ExpressionType.ExclusiveOr:
					ProcessBinaryExpression((BinaryExpression)expression);
					break;
				//				case ExpressionType.TypeIs:
				//					return this.ProcessTypeIs((TypeBinaryExpression)expression);
				//				case ExpressionType.Conditional:
				//					return this.ProcessConditional((ConditionalExpression)expression);
				//				case ExpressionType.Constant:
				//					return this.ProcessConstant((ConstantExpression)expression);
				//				case ExpressionType.Parameter:
				//					return this.ProcessParameter((ParameterExpression)expression);
				case ExpressionType.MemberAccess:
					ProcessMemberAccessExpression((MemberExpression)expression);
					break;
				case ExpressionType.Call:
					ProcessMethodCallExpression((MethodCallExpression)expression);
					break;
				//				case ExpressionType.Lambda:
				//					return this.ProcessLambda((LambdaExpression)expression);
				//				case ExpressionType.New:
				//					return this.ProcessNew((NewExpression)expression);
				//				case ExpressionType.NewArrayInit:
				//				case ExpressionType.NewArrayBounds:
				//					return this.ProcessNewArray((NewArrayExpression)expression);
				//				case ExpressionType.Invoke:
				//					return this.ProcessInvocation((InvocationExpression)expression);
				//				case ExpressionType.MemberInit:
				//					return this.ProcessMemberInit((MemberInitExpression)expression);
				//				case ExpressionType.ListInit:
				//					return this.ProcessListInit((ListInitExpression)expression);
				default:
					return;
			}
		}

		void ProcessMethodCallExpression(MethodCallExpression expression)
		{
			foreach (Expression argumentExpression in expression.Arguments)
			{
				ProcessDependents(argumentExpression);
			}
		}

		void ProcessUnaryExpression(UnaryExpression expression)
		{
			ProcessDependents(expression.Operand);
		}

		void ProcessBinaryExpression(BinaryExpression binaryExpression)
		{
			Expression left = binaryExpression.Left;
			Expression right = binaryExpression.Right;
			ProcessDependents(left);
			ProcessDependents(right);
		}

		void ProcessMemberAccessExpression(MemberExpression expression)
		{
			Expression ownerExpression = expression.Expression;
			Type ownerExpressionType = ownerExpression.Type;

			if (typeof(INotifyPropertyChanged).IsAssignableFromEx(ownerExpressionType))
			{
				try
				{
					string memberName = expression.Member.Name;
					PropertyChangedEventHandler handler = delegate(object sender, PropertyChangedEventArgs args)
					{
						if (args.PropertyName == memberName)
						{
							OnValueChanged();
						}
					};

					var owner = (INotifyPropertyChanged)GetValue(ownerExpression);
					owner.PropertyChanged += handler;

					handlers[handler] = new WeakReference(owner);
				}
				catch (Exception ex)
				{
					var log = Dependency.Resolve<ILog>();
					log.Info($"ComputedValue failed to resolve INotifyPropertyChanged value for property {expression.Member} {ex}");
				}
			}
		}

		object GetValue(Expression expression)
		{
			UnaryExpression unaryExpression = Expression.Convert(expression, typeof(object));
			Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(unaryExpression);
			Func<object> getter = getterLambda.Compile();

			return getter();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			var temp = PropertyChanged;

			if (temp == null)
			{
				return;
			}

			UIContext.Instance.Send(() =>
			{
				temp.Invoke(this, new PropertyChangedEventArgs(propertyName));
			});
		}

		void OnValueChanged()
		{
			OnPropertyChanged(nameof(Value));
		}

		bool disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					foreach (KeyValuePair<PropertyChangedEventHandler, WeakReference> pair in handlers)
					{
						INotifyPropertyChanged target = pair.Value.Target as INotifyPropertyChanged;
						if (target != null)
						{
							target.PropertyChanged -= pair.Key;
						}
					}

					handlers.Clear();
				}

				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}