#if WINDOWS_UWP || NETFX_CORE

#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2018, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2018-09-07 20:45:33Z</CreationDate>
</File>
*/
# endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Codon.DialogModel
{
	public class SelectableItem
	{
		public SelectableItem(string text, object item)
		{
			Text = text;
			Item = item;
		}

		public string Text { get; }
		public object Item { get; }

		public override string ToString()
		{
			return Text;
		}
	}

	// public class TextFuncConverter : IValueConverter
	// {
	// 	public Func<object, string> TextFunc { get; set; }
	//
	// 	internal static Func<object, string> TextFuncInternal { get; set; } = o => o?.ToString();
	//
	// 	public TextFuncConverter()
	// 	{
	// 		TextFunc = TextFuncInternal;
	// 	}
	//
	// 	public object Convert(object value, Type targetType, object parameter, string language)
	// 	{
	// 		return TextFunc(value);
	// 	}
	//
	// 	public object ConvertBack(object value, Type targetType, object parameter, string language)
	// 	{
	// 		throw new NotImplementedException();
	// 	}
	// }

	partial class DialogService
	{
		protected virtual async Task<MultipleChoiceResponse<T>> AskMultipleChoiceAsync<T>(MultipleChoiceQuestion<T> question)
		{
			if (!question.Items.Any())
			{
				throw new ArgumentException("The Question's item count is zero.");
			}

			DataTemplate template = null;
			bool useCustomTemplate = question.ItemTemplateName != null;

			if (useCustomTemplate)
			{
				if (Application.Current.Resources.TryGetValue(question.ItemTemplateName, out object templateObject))
				{
					template = (DataTemplate)templateObject;
				}
				else
				{
					throw new ArgumentException("Unable to locate template with name " + question.ItemTemplateName);
				}
			}
			else if (Application.Current.Resources.TryGetValue(TemplateNames.MultipleChoiceItem, out object templateObject))
			{
				template = (DataTemplate)templateObject;
			}

			IEnumerable items = question.Items;
			bool usingSelectableItems = false;
			object selectedItem = question.SelectedItem;
			bool multiSelect = question.MultiSelect;
			var selectedItems = new List<object>();

			if (template == null)
			{
				//Application.Current.Resources["TextFuncConverter"] = new TextFuncConverter();
				/* When a new instance of the converter is created,
				 * it takes the static TextFuncInternal value as a field value.
				 * That way, each new instance gets the right func. */
				//TextFuncConverter.TextFuncInternal = question.TextFunc;

				/* I wish I could make this work, but unfortunately the xmlns:dialogModel only resolves
				 * if present somewhere in the host app. */

				// var xaml = $@"<DataTemplate 
				// 				xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
				// 				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
				// 				xmlns:dialogModel=""using:Codon.DialogModel""> 
				// 				<Border>
				// 					<Border.Resources>
				// 						<dialogModel:TextFuncConverter x:Name=""TextFuncConverter""/>
				// 					</Border.Resources>
				// 					<TextBlock Text=""{{Binding Converter={{StaticResource TextFuncConverter}}}}"" /> 
				// 				</Border>
				// 			</DataTemplate>";
				//
				// template = (DataTemplate)XamlReader.Load(xaml);
				// var rd = new ResourceDictionary();
				// rd.Source = new Uri("ms-appx:///Codon.Platform/DialogResources.xaml");
				// template = (DataTemplate)rd["DialogService_MultipleChoiceItem"];

				var xaml = $@"<DataTemplate 
								xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
								xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""> 
								<Border>
									<TextBlock Text=""{{Binding Text}}"" /> 
								</Border>
							</DataTemplate>";
				
				template = (DataTemplate)XamlReader.Load(xaml);

				var tempList = new List<SelectableItem>();
				
				var questionSelectedItems = question.SelectedItems?.ToList();
				
				foreach (T o in question.Items)
				{
					var item = new SelectableItem(question.TextFunc(o), o);
					tempList.Add(item);
					
					if (multiSelect)
					{
						if (questionSelectedItems != null && questionSelectedItems.Contains(o))
						{
							selectedItems.Add(item);
						}
					}
					else
					{
						if (Equals(question.SelectedItem, o))
						{
							selectedItem = item;
						}
					}
				}

				items = tempList;

				usingSelectableItems = true;
			}
			
			ListBox box = new ListBox
			{
				ItemsSource = items,
				ItemTemplate = template,
				SelectionMode = question.MultiSelect ? SelectionMode.Multiple : SelectionMode.Single
			};

			if (multiSelect)
			{
				foreach (var item in selectedItems)
				{
					box.SelectedItems.Add(item);
				}
			}
			else
			{
				box.SelectedItem = selectedItem;
			}
			
			ContentDialog dialog = new ContentDialog
			{
				Content = box,
				Title = question.Caption,
				IsPrimaryButtonEnabled = true,
				IsSecondaryButtonEnabled = true,
				PrimaryButtonText = Strings.Okay,
				SecondaryButtonText = Strings.Cancel
			};

			ContentDialogResult dialogResult = await dialog.ShowAsync();

			if (dialogResult == ContentDialogResult.None || dialogResult == ContentDialogResult.Secondary)
			{
				return new MultipleChoiceResponse<T>();
			}

			if (usingSelectableItems)
			{
				var objects = box.SelectedItems.Cast<SelectableItem>().Select(x => (T)x.Item).ToList();
				return new MultipleChoiceResponse<T>(objects);
			}

			return new MultipleChoiceResponse<T>(box.SelectedItems?.Cast<T>());
		}
	}
}

#endif