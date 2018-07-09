#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Codon.InversionOfControl;
using Codon.MissingTypes.System.Windows.Data;

namespace Codon.UI.Data
{
	/// <summary>
	/// This class is not intended to be consumed publicly.
	/// This class should be internal. 
	/// The assembly's strong name, however, prevents Xamarin 
	/// based libraries, namely Codon.Extras.Platform.Android, 
	/// from opening up visibility with the InternalsVisibleTo. 
	/// Xamarin based libraries can't be strong named, 
	/// and InternalsVisibleTo requires a strong named assembly 
	/// name if the assembly where the attribute is declared 
	/// is strong named.</summary>
	public partial class InternalBindingApplicator
	{
		readonly string[] dotContextPath = { "." };

#if __ANDROID__
		const string viewEnabledPropertyName = nameof(Android.Views.View.Enabled);
#else
		/* The Enabled property name assumes that there is a property on the view 
		 * that can be used to set the enabled state of the view. */
		const string viewEnabledPropertyName = "Enabled";
#endif
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ViewBinderRegistry ViewBinderRegistry { get; } = new ViewBinderRegistry();

		readonly MarkupExtensionUtil markupExtensionUtil = new MarkupExtensionUtil();

		public void ApplyBinding(
			BindingExpression bindingExpression,
			object activity,
			string dataContextPropertyOnActivity,
			IValueConverter converter,
			List<Action> unbindActions)
		{
#if NETSTANDARD
			PropertyInfo targetProperty = bindingExpression.View.GetType().GetRuntimeProperty(bindingExpression.Target);
#else
			PropertyInfo targetProperty = bindingExpression.View.GetType().GetProperty(bindingExpression.Target);
#endif
			var localRemoveActions = new List<Action>();
			
			string sourcePath = bindingExpression.Source?.Trim();
			string path = bindingExpression.Path?.Trim();
			
			object dataContext;
			string[] pathSplit;

			MarkupExtensionInfo extensionInfo 
				= markupExtensionUtil.CreateMarkupExtensionInfo(sourcePath);
			if (extensionInfo == null)
			{
				pathSplit = GetPathSplit(sourcePath, path, dataContextPropertyOnActivity);
				dataContext = activity;
			}
			else
			{
				dataContext = RetrieveExtensionValue(extensionInfo);
				pathSplit = GetPathSplit(null, path, null);
			}
			
			Bind(bindingExpression, dataContext, dataContext, pathSplit, converter, targetProperty, localRemoveActions, unbindActions, 0);
		}

		public void ApplyBinding(
			BindingExpression bindingExpression,
			object dataContext,
			IValueConverter converter,
			List<Action> unbindActions)
		{
#if NETSTANDARD
			var properties = bindingExpression.View.GetType().GetRuntimeProperties();
#else
			var properties = bindingExpression.View.GetType().GetProperties( 
				BindingFlags.Public 
				| BindingFlags.NonPublic 
				| BindingFlags.Instance);
#endif
			string targetName = bindingExpression.Target;
			PropertyInfo targetProperty = properties.FirstOrDefault(x => x.Name == targetName);
			string sourcePath = bindingExpression.Source;
			string path = bindingExpression.Path?.Trim();

			string[] pathSplit;
			
			var extensionInfo = markupExtensionUtil.CreateMarkupExtensionInfo(sourcePath);
			var originalDataContext = dataContext;

			if (extensionInfo == null)
			{
				pathSplit = GetPathSplit(sourcePath, path, null);
			}
			else
			{
				dataContext = RetrieveExtensionValue(extensionInfo);
				pathSplit = GetPathSplit(null, path, null);
			}

			var localRemoveActions = new List<Action>();

			Bind(bindingExpression, dataContext, originalDataContext, pathSplit, converter, targetProperty, localRemoveActions, unbindActions, 0);
		}
		
		public IMarkupExtension RetrieveExtension(MarkupExtensionInfo extensionInfo)
		{
			var extensionRegistry = Dependency.Resolve<IMarkupExtensionRegistry>();

			var retrievedItem = extensionRegistry.TryGetExtensionCreationFunc(
									extensionInfo.ExtensionTypeName, 
									out Func<object[], IMarkupExtension> creationFunc) 
									&& creationFunc != null;
			if (!retrievedItem)
			{
				throw new BindingException("Unable to resolve MarkupExtension. " + extensionInfo.ExtensionTypeName);
			}

			try
			{
				var result = creationFunc(extensionInfo.Parameters);
				return result;
			}
			catch (Exception ex)
			{
				throw new BindingException("Unable to create MarkupExtension. " + extensionInfo.ExtensionTypeName, ex);
			}
		}

		static IContainer iocContainer;

		object RetrieveExtensionValue(MarkupExtensionInfo extensionInfo)
		{
			if (iocContainer == null)
			{
				iocContainer = Dependency.Resolve<IContainer>();
			}

			var extension = RetrieveExtension(extensionInfo);
			var result = extension.ProvideValue(iocContainer);
			return result;
		}

		string[] GetPathSplit(string sourcePath, string path, string dataContextPropertyOnActivity)
		{
			string resultPath = sourcePath;

			if (!string.IsNullOrWhiteSpace(dataContextPropertyOnActivity))
			{
				if (string.IsNullOrEmpty(sourcePath))
				{
					resultPath = dataContextPropertyOnActivity;
				}
				else
				{
					resultPath = dataContextPropertyOnActivity + "." + sourcePath;
				}

				if (!string.IsNullOrEmpty(path))
				{
					resultPath = resultPath + "." + path;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(path))
				{
					if (!string.IsNullOrEmpty(sourcePath))
					{
						resultPath = sourcePath + "." + path;
					}
					else
					{
						resultPath = path;
					}
				}
			}

			if (resultPath == null)
			{
				/* When neither a Source nor Path are supplied 
				 * it implies binding directly to the data context. */
				return dotContextPath;
			}

			var pathSplit = resultPath.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			
			return pathSplit;
		}
	}

	public class MarkupExtensionUtil
	{
		const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;
		readonly Regex markupExtensionRegex = new Regex(@"{ (  (?<Extension> [^{}\s]+ ) \s+ (?<Parameters> [^{}]+ )? )* }", regexOptions);

		public MarkupExtensionInfo CreateMarkupExtensionInfo(string text)
		{
			if (text == null)
			{
				return null;
			}

			var match = markupExtensionRegex.Match(text);
			if (match.Success)
			{
				var groups = match.Groups;
				string extension = groups["Extension"]?.Value;
				string parameters = groups["Parameters"]?.Value ?? string.Empty;
				var parameterList = parameters.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				var result = new MarkupExtensionInfo(extension, parameterList);

				return result;
			}

			return null;
		}
	}

	public class MarkupExtensionInfo
	{
		public string ExtensionTypeName { get; private set; }
		public object[] Parameters { get; private set; }

		internal MarkupExtensionInfo(string extensionTypeName, object[] parameters)
		{
			ExtensionTypeName = extensionTypeName;
			Parameters = parameters;
		}
	}
}