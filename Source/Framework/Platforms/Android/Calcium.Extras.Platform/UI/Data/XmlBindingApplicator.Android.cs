//#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Calcium.InversionOfControl;
using Java.Lang;

using Calcium.Logging;
using Calcium.MissingTypes.System.Windows.Data;
using Enum = System.Enum;
using Exception = System.Exception;

namespace Calcium.UI.Data
{
	public class XmlBindingApplicator
	{
		static readonly XName bindingXmlNamespace
			= XNamespace.Get("http://schemas.android.com/apk/res-auto") + "Binding";

		static readonly XNamespace androidXmlNamespace
			= XNamespace.Get("http://schemas.android.com/apk/res/android");

		static readonly XName androidIdXName
			= XNamespace.Get("http://schemas.android.com/apk/res/android") + "id";

		static List<Type> valueConverterTypes;

		readonly List<Action> unbindActions = new List<Action>();

		static readonly Dictionary<int, List<XElement>> layoutCache = new Dictionary<int, List<XElement>>();

		MarkupTypeResolver typeResolver = new MarkupTypeResolver();
		readonly MarkupExtensionUtil markupExtensionUtil = new MarkupExtensionUtil();
		static IContainer iocContainer;

		public void RemoveBindings()
		{
			foreach (var unbindAction in unbindActions)
			{
				try
				{
					unbindAction();
				}
				catch (Exception ex)
				{
					Log?.Warn("Unable to remove binding.", ex);
					
					if (Debugger.IsAttached)
					{
						throw;
					}
				}
			}

			unbindActions.Clear();
		}

		public void ApplyBindings(Activity activity, string viewModelPropertyName, int layoutResourceId)
		{
			if (ApplicationContextHolder.Context == null)
			{
				ApplicationContextHolder.Context = activity.ApplicationContext;
			}

			List<XElement> elements = null;

			if (layoutResourceId > -1)
			{
				/* Load the XML elements of the view. */
				elements = GetLayoutAsXmlElements(activity, layoutResourceId);
			}

			if (elements != null && elements.Any())
			{
				var rootView = activity.Window.DecorView.FindViewById(Android.Resource.Id.Content);

				/* Get all the binding inside the XML file. */
				var bindingInfos = ExtractBindingsFromLayoutFile(elements, rootView);
				if (bindingInfos == null || !bindingInfos.Any())
				{
					return;
				}

				/* Load all the converters if there is a binding using a converter. */
				if (bindingInfos.Any(info => !string.IsNullOrWhiteSpace(info.Converter)))
				{
					if (valueConverterTypes == null)
					{
						valueConverterTypes = new List<Type>();
						var converters = TypeUtility.GetTypes<IValueConverter>().ToList();
						if (converters.Any())
						{
							valueConverterTypes.AddRange(converters);
						}
						else if (Debugger.IsAttached)
						{
							Debugger.Break();
						}
					}
				}

				InternalBindingApplicator binder = new InternalBindingApplicator();
				
				foreach (var bindingInfo in bindingInfos)
				{
					IValueConverter valueConverter = null;
					string valueConverterName = bindingInfo.Converter;

					if (!string.IsNullOrWhiteSpace(valueConverterName))
					{
						var markupExtensionInfo = markupExtensionUtil.CreateMarkupExtensionInfo(valueConverterName);
						if (markupExtensionInfo != null)
						{
							IMarkupExtension extension = binder.RetrieveExtension(markupExtensionInfo);

							if (iocContainer == null)
							{
								iocContainer = Dependency.Resolve<IContainer>();
							}

							valueConverter = (IValueConverter)extension.ProvideValue(iocContainer);
						}
						else
						{ 
							Type valueConverterType;

							if (typeResolver.TryResolve(valueConverterName, out valueConverterType))
							{
								//var valueConverterType = valueConverterTypes.FirstOrDefault(t => t.Name == valueConverterName);
								if (valueConverterType != null)
								{
									var valueConverterConstructor = valueConverterType.GetConstructor(Type.EmptyTypes);
									if (valueConverterConstructor != null)
									{
										valueConverter = valueConverterConstructor.Invoke(null) as IValueConverter;
									}
									else
									{
										throw new InvalidOperationException(
											$"Value converter {valueConverterName} needs "
											+ "an empty constructor to be instanciated.");
									}
								}
								else
								{
									throw new InvalidOperationException(
										$"There is no converter named {valueConverterName}.");
								}
							}
						}
					}

					binder.ApplyBinding(bindingInfo, activity, viewModelPropertyName, valueConverter, unbindActions);
				}
			}
		}

		public void ApplyBindings(View view, object dataContext, int layoutResourceId)
		{
			Context context = view.Context;

			if (ApplicationContextHolder.Context == null)
			{
				ApplicationContextHolder.Context = context.ApplicationContext;
			}

			List<XElement> elements = null;

			if (layoutResourceId > -1)
			{
				/* Load the XML elements of the view. */
				elements = GetLayoutAsXmlElements(context, layoutResourceId);
			}

			if (elements != null && elements.Any())
			{
				/* Get all the binding inside the XML file. */
				var bindingInfos = ExtractBindingsFromLayoutFile(elements, view);
				if (bindingInfos == null || !bindingInfos.Any())
				{
					return;
				}

				/* Load all the converters if there is a binding using a converter. */
				if (bindingInfos.Any(bo => !string.IsNullOrWhiteSpace(bo.Converter)))
				{
					if (valueConverterTypes == null)
					{
						valueConverterTypes = new List<Type>();
						var converters = TypeUtility.GetTypes<IValueConverter>();
						valueConverterTypes.AddRange(converters);
					}
				}

				var binder = new InternalBindingApplicator();

				foreach (var bindingInfo in bindingInfos)
				{
					IValueConverter valueConverter = null;
					string valueConverterName = bindingInfo.Converter;

					if (!string.IsNullOrWhiteSpace(valueConverterName))
					{
						var markupExtensionInfo = markupExtensionUtil.CreateMarkupExtensionInfo(valueConverterName);
						if (markupExtensionInfo != null)
						{
							IMarkupExtension extension = binder.RetrieveExtension(markupExtensionInfo);

							if (iocContainer == null)
							{
								iocContainer = Dependency.Resolve<IContainer>();
							}

							valueConverter = (IValueConverter)extension.ProvideValue(iocContainer);
						}
						else
						{ 
							var converterType = valueConverterTypes.FirstOrDefault(t => t.Name == valueConverterName);
							if (converterType != null)
							{
								var constructor = converterType.GetConstructor(Type.EmptyTypes);
								if (constructor != null)
								{
									valueConverter = constructor.Invoke(null) as IValueConverter;
								}
								else
								{
									throw new InvalidOperationException(
										$"Value converter {valueConverterName} needs "
										+ "an empty constructor to be instanciated.");
								}
							}
							else
							{
								throw new InvalidOperationException(
									$"There is no converter named {valueConverterName}.");
							}
						}
					}

					binder.ApplyBinding(bindingInfo, dataContext, valueConverter, unbindActions);
				}
			}
		}

		public static void SetViewBinder(Type viewType, string propertyName, IViewBinder binder)
		{
			InternalBindingApplicator.ViewBinderRegistry.SetViewBinder(viewType, propertyName, binder);
		}

		/// <summary>
		/// Returns the current view (activity) as a list of XML element.
		/// Based on code by Thomas Lebrun http://bit.ly/1OQsD8L
		/// </summary>
		/// <param name="activity">The current activity we want 
		/// to get as a list of XML elements.</param>
		/// <param name="layoutResourceId">The id corresponding to the layout.</param>
		/// <returns>A list of XML elements which represent the XML layout of the view.</returns>
		static List<XElement> GetLayoutAsXmlElements(Context activity, int layoutResourceId)
		{
			if (layoutCache.TryGetValue(layoutResourceId, out List<XElement> result))
			{
				return result;
			}

//			result = new List<XElement>();

			using (XmlReader viewAsXmlReader = activity.Resources.GetLayout(layoutResourceId))
			{
				//viewAsXmlReader.MoveToContent();
//				XDocument xRoot = XDocument.Load(viewAsXmlReader);
//
//				//var elements = xRoot.Elements();
//				var rootElement = xRoot.Root;
//				result.Add(rootElement);
//				GetChildElements(rootElement, result);


				using (var sb = new StringBuilder())
				{
					while (viewAsXmlReader.Read())
					{
						sb.Append(viewAsXmlReader.ReadOuterXml());
					}

					var viewAsXDocument = XDocument.Parse(sb.ToString());
					result = viewAsXDocument.Descendants().ToList();
				}
			}

			layoutCache[layoutResourceId] = result;

			return result;
		}

		static void GetChildElements(XElement element, IList<XElement> accumlator)
		{
			var childElements = element.Elements();
			foreach (var childElement in childElements)
			{
				accumlator.Add(childElement);

				GetChildElements(childElement, accumlator);
			}
		}

		static List<View> GetAllChildrenInView(View rootView)
		{
			List<View> result = new List<View>();
			GetAllChildrenInView(rootView, result);

			return result;
		}

		/// <summary>
		/// Recursive method which returns the list of children contains in a view.
		/// </summary>
		/// <param name="rootView">The start view from which the children will be retrieved.</param>
		/// <param name="children">A cumulative collection of child views.</param>
		static void GetAllChildrenInView(View rootView, List<View> children)
		{
			if (!(rootView is ViewGroup))
			{
				children.Add(rootView);
				return;
			}

			children.Add(rootView);

			var viewGroup = (ViewGroup)rootView;

			for (var i = 0; i < viewGroup.ChildCount; i++)
			{
				View child = viewGroup.GetChildAt(i);

				GetAllChildrenInView(child, children);
			}
		}

		const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace;
		readonly Regex sourceRegex = new Regex(@"Source=\s*(?: (?<Path> \w+(\.\w+)*) | (?<MarkupExtension> ( { [^{}\s]+ \s+ ( [^{}]+ )? } )* ))", regexOptions);
		readonly Regex pathRegex = new Regex(@"(?: Path\s*=\s*(?<Path> (?: \w+(?:\.\w+)*)) | (?:,|;|\A) \s* (?<! =) \s* (?<Path> \w+(?:\.\w+)*) (?!=)\s*(?:,|;|\z))", regexOptions | RegexOptions.Multiline);
		readonly Regex targetRegex = new Regex(@"Target=(\w+)", regexOptions);
		readonly Regex converterRegex = new Regex(@"Converter=\s*(?: (?<Path> \w+(\.\w+)*) | (?<MarkupExtension> ( { [^{}\s]+ \s+ ( [^{}]+ )? } )* ))", regexOptions);
		//readonly Regex converterParameterRegex = new Regex(@"ConverterParameter=(\s*[\w\@]+\s*(.\w+)*)", regexOptions);
		readonly Regex converterParameterRegex = new Regex(@"ConverterParameter=\s*((?:\\\\|\\,|\\;|[^,;])*)\s*[,;]?.*", regexOptions);
		readonly Regex modeRegex = new Regex(@"Mode=(\w+)", regexOptions);
		readonly Regex changedEventRegex = new Regex(@"ChangedEvent=(\w+)", regexOptions);
		//readonly Regex markupExtensionRegex = new Regex(@"{ ( [^{}\s]+ \s+ ( [^{}]+ )? )* }", regexOptions);

		/// <summary>
		/// Extracts the binding information represented 
		/// as the Binding="" attribute in the XML file.
		/// Based on code by Thomas Lebrun http://bit.ly/1OQsD8L
		/// </summary>
		/// <param name="xmlElements">The list of XML elements from 
		/// which to extract the binding information.</param>
		/// <param name="rootView">The root view that is used 
		/// to retrieve child views by ID.</param>
		/// <returns>
		/// A list containing all the binding info objects.
		/// </returns>
		List<BindingExpression> ExtractBindingsFromLayoutFile(
			List<XElement> xmlElements, View rootView)
		{
			var result = new List<BindingExpression>();

			//Dictionary<int, View> viewDictionary = new Dictionary<int, View>();
			//foreach (var view in viewElements)
			//{
			//	int id = view.Id;
			//	if (id != -1)
			//	{
			//		viewDictionary[id] = view;
			//	}
			//}

			var idList = new List<string>();

			for (var i = 0; i < xmlElements.Count; i++)
			{
				var xmlElement = xmlElements.ElementAt(i);

				/* An id attribute is required. */
				var idAttribute = xmlElement.Attribute(androidIdXName);
				string idValue = idAttribute?.Value;

				if (!xmlElement.Attributes(bindingXmlNamespace).Any())
				{
					goto AddIdToList;
				}

				var attribute = xmlElement.Attribute(bindingXmlNamespace);

				if (attribute == null)
				{
					goto AddIdToList;
				}

				string bindingString = attribute.Value;

				if (string.IsNullOrWhiteSpace(bindingString))
				{
					goto AddIdToList;
				}

				if (!string.IsNullOrEmpty(idValue) && idList.Contains(idValue))
				{
					Log.Warn("Binding Error: Duplicate android:id '" 
						+ idValue + "'. Elements with bindings must use unique IDs within the layout.");
				}

				//				if (!bindingString.StartsWith("{") || !bindingString.EndsWith("}"))
				//				{
				//					throw new InvalidOperationException(
				//						"The following XML binding operation is not well formatted, it should start "
				//						+ $"with '{{' and end with '}}:'{Environment.NewLine}{bindingString}");
				//				}

				string[] bindingStrings = bindingString.Split(';');

				foreach (var bindingText in bindingStrings)
				{
					//					if (!bindingText.Contains(","))
					//					{
					//						throw new InvalidOperationException(
					//							"The following XML binding operation is not well formatted, "
					//							+ "it should contains at least one \',\' "
					//							+ $"between Source and Target:{Environment.NewLine}{bindingString}");
					//					}

					//bool sourceIsMarkupExtension = false;
					string sourceValue = null;
					var sourceMatch = sourceRegex.Match(bindingText);
					if (sourceMatch.Success)
					{
						var groups = sourceMatch.Groups;
						var path = groups["Path"];
						sourceValue = path?.Value;
						if (string.IsNullOrEmpty(sourceValue))
						{
							sourceValue = groups["MarkupExtension"]?.Value;

							//sourceIsMarkupExtension = true;
						}
					}

					string pathValue = pathRegex.Match(bindingText).Groups["Path"].Value;

					string targetValue = targetRegex.Match(bindingText).Groups[1].Value;

					string converterValue = null;
					var converterMatch = converterRegex.Match(bindingText);
					if (converterMatch.Success)
					{
						var groups = converterMatch.Groups;
						var path = groups["Path"];
						converterValue = path?.Value;
						if (string.IsNullOrEmpty(converterValue))
						{
							converterValue = groups["MarkupExtension"]?.Value;
						}
					}
					//string converterValue = converterRegex.Match(bindingText).Groups[1].Value;

					string converterParameterValue = converterParameterRegex.Match(bindingText).Groups[1].Value;

					if (!string.IsNullOrEmpty(sourceValue))
					{
						sourceValue = sourceValue.Replace("\\,", ",");
					}

					if (!string.IsNullOrEmpty(targetValue))
					{
						targetValue = targetValue.Replace("\\,", ",");
					}

					if (!string.IsNullOrEmpty(converterValue))
					{
						converterValue = converterValue.Replace("\\,", ",");
					}
					
					if (!string.IsNullOrEmpty(converterParameterValue))
					{
						converterParameterValue = converterParameterValue.Replace("\\,", ",");
					}

					var bindingMode = BindingMode.OneWay;

					var modeRegexMatch = modeRegex.Match(bindingText);

					if (modeRegexMatch.Success)
					{
						if (!Enum.TryParse(modeRegexMatch.Groups[1].Value, true, out bindingMode))
						{
							throw new InvalidOperationException(
								"The Mode property of the following XML binding operation "
								+ "is not well formatted, it should be \'OneWay\' "
								+ $"or 'TwoWay':{Environment.NewLine}{bindingString}");
						}
					}

					bool idValid = false;
					View view = null;
					
					if (!string.IsNullOrWhiteSpace(idValue))
					{
						string intPart = idValue.Substring(1);

						if (intPart.Length > 0)
						{
							if (int.TryParse(intPart, out int id))
							{
								view = rootView.FindViewById(id);

								if (view == null)
								{
									throw new Exception("Unable to find view with ID: " + id
														+ " Root view is " + rootView);
								}

								idValid = true;
							}
						}
					}

					if (!idValid)
					{
						throw new Exception("Element with binding expression lacks an id attribute. "
							+ "An id attribute is required for elements that have a binding expression. "
							+ Environment.NewLine + "Compiled Element: " + Environment.NewLine + xmlElement);
					}

					var viewValueChangedEvent = changedEventRegex.Match(bindingText).Groups[1].Value;

					result.Add(new BindingExpression
					{
						View = view,
						Path = pathValue,
						Source = sourceValue,
						Target = targetValue,
						Converter = converterValue,
						ConverterParameter = converterParameterValue,
						Mode = bindingMode,
						ViewValueChangedEvent = viewValueChangedEvent,
					});
				}

				AddIdToList:
				idList.Add(idValue);
			}

			return result;
		}

		ILog log_UseProperty;
		bool tryRetrieveLog = true;

		ILog Log
		{
			get
			{
				if (tryRetrieveLog && log_UseProperty == null)
				{
					Dependency.TryResolve<ILog>(out log_UseProperty);
					tryRetrieveLog = false;
				}

				return log_UseProperty;
			}
		}

		public bool HasBindings => unbindActions.Any();
	}
}
//#endif
