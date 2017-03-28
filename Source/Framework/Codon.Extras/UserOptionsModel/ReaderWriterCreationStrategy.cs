#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-21 19:10:29Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.ObjectModel;

using Codon.ComponentModel;
using Codon.UserOptionsModel.UserOptions;

namespace Codon.UserOptionsModel
{
	/// <summary>
	/// Provides an <see cref="IUserOptionReaderWriter"/>.
	/// </summary>
	public interface IReaderWriterCreationStrategy
	{
		/// <summary>
		/// Provides an <see cref="IUserOptionReaderWriter"/>
		/// that is able to retrieve or persist a user option.
		/// </summary>
		/// <param name="userOption">
		/// The user option that this reader writer will
		/// attend to.</param>
		/// <returns>A user option reader writer.</returns>
		IUserOptionReaderWriter Create(IUserOption userOption);
	}

	public class ReaderWriterCreationStrategy : IReaderWriterCreationStrategy
	{
		public IUserOptionReaderWriter Create(IUserOption userOption)
		{
			AssertArg.IsNotNull(userOption, nameof(userOption));

			BooleanUserOption booleanUserOption = userOption as BooleanUserOption;
			if (booleanUserOption != null)
			{
				return new UserOptionReaderWriter<bool>(userOption);
			}

			DoubleUserOption doubleUserOption = userOption as DoubleUserOption;
			if (doubleUserOption != null)
			{
				return new UserOptionReaderWriter<double>(userOption);
			}

			IntUserOption intUserOption = userOption as IntUserOption;
			if (intUserOption != null)
			{
				return new UserOptionReaderWriter<int>(userOption);
			}

			StringUserOption stringUserOption = userOption as StringUserOption;
			if (stringUserOption != null)
			{
				return new UserOptionReaderWriter<string>(userOption);
			}

			var provider = userOption as IProvider<IUserOptionReaderWriter>;
			if (provider != null)
			{
				return provider.ProvidedItem;
			}

			var optionBase = userOption as UserOptionBase;
			if (optionBase != null)
			{
				Type templateType = typeof(UserOptionReaderWriter<>);
				Type genericType = templateType.MakeGenericType(optionBase.SettingType);
				try
				{
					var readerWriter = Activator.CreateInstance(genericType, userOption);
					return (IUserOptionReaderWriter)readerWriter;
				}
				catch (Exception)
				{
					/* Ignore and return outside this block. */
				}
			}

			return new UserOptionReaderWriter<object>(userOption);
//			Type template = typeof(UserOptionBase<>);
//			var b = userOption.GetType().GetGenericArguments();
//			if (b != null && b.Any())
//			{
//				Type genericType = template.MakeGenericType(b.First());
//				var genericUserOption = userOption as UserOptionBase<>;
//			}

			//return new UserOptionReaderWriter<object>(userOption);
		}
	}

	public class ListOptionReaderWriter<TSetting> : UserOptionReaderWriter<TSetting>
	{
		public ListOptionReaderWriter(IUserOption userOption)
			: base(userOption)
		{
			AssertArg.IsNotNullAndOfType<IListOption<TSetting>>(
				userOption, nameof(userOption));
		}

		public ObservableCollection<TSetting> Options => 
			((IListOption<TSetting>)UserOption).Options;
	}

	public interface IListOption<TSetting>
	{
		ObservableCollection<TSetting> Options { get; }
	}
}