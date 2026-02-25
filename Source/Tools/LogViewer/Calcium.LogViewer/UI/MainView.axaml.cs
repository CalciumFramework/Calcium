using System;
using System.Collections.Specialized;

using Avalonia.Controls;
using Avalonia.Input;

namespace Calcium.LogViewer.UI
{
	public partial class MainView : UserControl
	{
		const double columnMinimumWidth = 80;
		MainVM? VM => DataContext as MainVM;

		public MainView()
		{
			InitializeComponent();

			DataContext = new MainVM();

			listBox.ItemsView.CollectionChanged += HandleCollectionChanged;

			messageExceptionSplitter.DragDelta += OnMessageExceptionSplitterDragDelta;
			exceptionFillerSplitter.DragDelta  += OnExceptionFillerSplitterDragDelta;
		}

		void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
		{
			/* Scrolls to the end. */
			if (VM?.AutoScroll == true && listBox.ItemsView.Count > 0)
			{
				var item = listBox.ItemsView[^1];
				if (item != null)
				{
					listBox.ScrollIntoView(item);
				}
			}
		}

		void OnMessageExceptionSplitterDragDelta(object? sender, VectorEventArgs e)
		{
			if (DataContext is not MainVM viewModel)
			{
				return;
			}

			/* Dragging this splitter changes Message and Exception widths against each other. */
			double delta = e.Vector.X;

			double messageWidth = GetPixelWidth(viewModel.MessageColumnWidth, headerGrid.ColumnDefinitions[5].ActualWidth);
			double exceptionWidth = GetPixelWidth(viewModel.ExceptionColumnWidth, headerGrid.ColumnDefinitions[7].ActualWidth);

			double newMessage = Math.Max(columnMinimumWidth,   messageWidth   + delta);
			double newException = Math.Max(columnMinimumWidth, exceptionWidth - delta);

			viewModel.MessageColumnWidth   = new Avalonia.Controls.GridLength(newMessage);
			viewModel.ExceptionColumnWidth = new Avalonia.Controls.GridLength(newException);

			e.Handled = true;
		}

		void OnExceptionFillerSplitterDragDelta(object? sender, VectorEventArgs e)
		{
			if (DataContext is not MainVM viewModel)
			{
				return;
			}

			/* Dragging this splitter changes Exception width against the remaining filler space. */
			double delta = e.Vector.X;

			double exceptionWidth = GetPixelWidth(viewModel.ExceptionColumnWidth, headerGrid.ColumnDefinitions[7].ActualWidth);
			double newException = Math.Max(columnMinimumWidth, exceptionWidth + delta);

			viewModel.ExceptionColumnWidth = new Avalonia.Controls.GridLength(newException);

			e.Handled = true;
		}

		static double GetPixelWidth(Avalonia.Controls.GridLength length, double fallbackActualWidth)
		{
			if (length.IsAbsolute)
			{
				return length.Value;
			}

			/* If it was Star/Auto, use current realised size as the starting point. */
			return fallbackActualWidth <= 0 ? 200 : fallbackActualWidth;
		}
	}
}
