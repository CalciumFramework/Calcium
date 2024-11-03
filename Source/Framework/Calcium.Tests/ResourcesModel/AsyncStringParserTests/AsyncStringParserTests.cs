using Calcium.ResourcesModel.Experimental;

using FluentAssertions;

using Moq;

namespace Calcium.ResourcesModel
{
	public class AsyncStringParserTests
	{
		#region ParseAsync Method Tests

		[Fact]
		public async Task ParseAsync_SingleTagReplacement_ReturnsReplacedString()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
						 { new() { Index = 5, Length = 6, TagName = "Tag", TagValue = "Hello" } });

			string input = "Text ${Tag} here.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Text Hello here.");
		}

		[Fact]
		public async Task ParseAsync_MultipleTagsReplacement_ReplacesAllTags()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length  = 11, TagName = "FirstTag", TagValue  = "FirstValue" },
						 new() { Index = 21, Length = 12, TagName = "SecondTag", TagValue = "SecondValue" }
					 });

			string input = "Test ${FirstTag} and ${SecondTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Test FirstValue and SecondValue.");
		}

		[Fact]
		public async Task ParseAsync_WithTagsProcessor_UsesResolvedTagValues()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();

			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 9, Length = 7, TagName = "Date" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Callback((IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								   {
									   // Ensure the key "Date" is added before accessing it
									   if (segments.ContainsKey("Date"))
									   {
										   foreach (TagSegment segment in segments["Date"])
										   {
											   segment.TagValue = "2024-11-03";
										   }
									   }
								   });

			string input = "Today is ${Date}.";

			// Act
			string result = await asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			result.Should().Be("Today is 2024-11-03.");
		}

		[Fact]
		public async Task ParseAsync_CancellationRequested_ThrowsOperationCanceledException()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);
			CancellationTokenSource cts = new();
			cts.Cancel();

			string input = "Some input with ${Tag}.";

			// Act & Assert
			await Assert.ThrowsAsync<OperationCanceledException>(async () =>
																	 await asyncParser.ParseAsync(input, null, null,
																		 null, cts.Token));
		}

		#endregion

		#region Multiple Tags Replacement Tests

		[Fact]
		public async Task ParseAsync_MultipleTagsReplacement_ReplacesAllTagsInOrder()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length  = 11, TagName = "FirstTag", TagValue  = "FirstValue" },
						 new() { Index = 21, Length = 12, TagName = "SecondTag", TagValue = "SecondValue" }
					 });

			string input = "Test ${FirstTag} and ${SecondTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Test FirstValue and SecondValue.");
		}

		[Fact]
		public async Task ParseAsync_MultipleTagsWithOverriddenValues_UsesOverriddenValues()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length  = 11, TagName = "FirstTag", Tag  = "FirstTag" },
						 new() { Index = 21, Length = 12, TagName = "SecondTag", Tag = "SecondTag" }
					 });

			Dictionary<string, string> overriddenValues = new()
			{
				{ "FirstTag", "OverriddenFirst" },
				{ "SecondTag", "OverriddenSecond" }
			};

			string input = "Test ${FirstTag} and ${SecondTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input, null, overriddenValues);

			// Assert
			result.Should().Be("Test OverriddenFirst and OverriddenSecond.");
		}

		[Fact]
		public async Task ParseAsync_MultipleTagsWithMixedSources_ReplacesCorrectly()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();

			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length  = 11, TagName = "FirstTag" },
						 new() { Index = 21, Length = 12, TagName = "SecondTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Callback((IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								   {
									   if (segments.ContainsKey("FirstTag"))
									   {
										   foreach (TagSegment segment in segments["FirstTag"])
										   {
											   segment.TagValue = "ProcessedFirst";
										   }
									   }

									   if (segments.ContainsKey("SecondTag"))
									   {
										   foreach (TagSegment segment in segments["SecondTag"])
										   {
											   segment.TagValue = "ProcessedSecond";
										   }
									   }
								   });

			string input = "Test ${FirstTag} and ${SecondTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			result.Should().Be("Test ProcessedFirst and ProcessedSecond.");
		}

		#endregion

		#region Asynchronous Processing Tests

		[Fact]
		public async Task ParseAsync_ExecutesAsynchronously_VerifiesAsyncExecution()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 11, TagName = "AsyncTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Returns((IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								  {
									  TaskCompletionSource tcs = new();
									  token.Register(() => tcs.TrySetCanceled(), false);

									  if (segments.ContainsKey("AsyncTag"))
									  {
										  foreach (TagSegment segment in segments["AsyncTag"])
										  {
											  segment.TagValue = "AsyncProcessedValue";
										  }
									  }

									  tcs.SetResult();
									  return tcs.Task;
								  });

			string input = "Test ${AsyncTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			result.Should().Be("Test AsyncProcessedValue.");
		}

		[Fact]
		public async Task ParseAsync_WithAsyncTagsProcessor_DoesNotBlock()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 17, TagName = "NonBlockingTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Returns(async (IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								  {
									  await Task.Yield(); // Simulate yielding execution to ensure async context
									  foreach (TagSegment segment in segments["NonBlockingTag"])
									  {
										  segment.TagValue = "NonBlockingValue";
									  }
								  });

			string input = "Test ${NonBlockingTag}.";

			// Act
			Task<string> task = asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			task.Status.Should().Be(TaskStatus.WaitingForActivation); // Ensures it runs asynchronously
			string result = await task;
			result.Should().Be("Test NonBlockingValue.");
		}

		#endregion

		#region Handling Missing Tag Values Tests

		[Fact]
		public async Task ParseAsync_TagWithoutProvidedValue_RetainsOriginalTag()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 8, TagName = "MissingTag" }
					 });

			string input = "Test ${MissingTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Test ${MissingTag}.");
		}

		[Fact]
		public async Task ParseAsync_TagWithoutResolvedValueFromTagsProcessor_RetainsOriginalTag()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 8, TagName = "UnresolvedTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Returns(Task.CompletedTask); // Simulate no tag values resolved

			string input = "Example ${UnresolvedTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			result.Should().Be("Example ${UnresolvedTag}.");
		}

		[Fact]
		public async Task ParseAsync_TagWithoutValueOrOverride_RetainsOriginalTag()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 10, Length = 7, TagName = "NoValueTag" }
					 });

			string input = "Here is ${NoValueTag} in the text.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Here is ${NoValueTag} in the text.");
		}

		#endregion

		#region Use of tagsProcessor for Dynamic Tag Resolution Tests

		[Fact]
		public async Task ParseAsync_CallsTagsProcessorToResolveTags()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 10, Length = 13, TagName = "DynamicTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Callback((IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								   {
									   if (segments.ContainsKey("DynamicTag"))
									   {
										   foreach (TagSegment segment in segments["DynamicTag"])
										   {
											   segment.TagValue = "ResolvedValue";
										   }
									   }
								   })
						 .Returns(Task.CompletedTask);

			string input = "This is a ${DynamicTag} test.";

			// Act
			string result = await asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			result.Should().Be("This is a ResolvedValue test.");
			tagsProcessor.Verify(
				t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
					It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ParseAsync_TagsProcessorCalledWithCorrectSegments()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 8, Length  = 11, TagName = "FirstTag" },
						 new() { Index = 24, Length = 12, TagName = "SecondTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Callback((IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								   {
									   segments.Should().ContainKey("FirstTag");
									   segments.Should().ContainKey("SecondTag");

									   foreach (TagSegment segment in segments["FirstTag"])
									   {
										   segment.TagValue = "Value1";
									   }

									   foreach (TagSegment segment in segments["SecondTag"])
									   {
										   segment.TagValue = "Value2";
									   }
								   })
						 .Returns(Task.CompletedTask);

			string input = "Testing ${FirstTag} and ${SecondTag}.";

			// Act
			string result = await asyncParser.ParseAsync(input, tagsProcessor.Object);

			// Assert
			result.Should().Be("Testing Value1 and Value2.");
			tagsProcessor.Verify(
				t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
					It.IsAny<CancellationToken>()), Times.Once);
		}

		#endregion

		#region Cancellation Support Tests

		[Fact]
		public async Task ParseAsync_WhenCancellationRequested_ThrowsOperationCanceledException()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);
			CancellationTokenSource cts = new();

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 16, TagName = "CancelableTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Returns(async (IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								  {
									  await Task.Yield(); // Simulate async operation
									  token.ThrowIfCancellationRequested();
								  });

			string input = "Test ${CancelableTag}.";

			// Act
			cts.Cancel();
			Func<Task> act
				= async () => await asyncParser.ParseAsync(input, tagsProcessor.Object, null, null, cts.Token);

			// Assert
			await act.Should().ThrowAsync<OperationCanceledException>();
		}

		[Fact]
		public async Task ParseAsync_TokenPassedToTagsProcessor_CanBeCancelledMidExecution()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			Mock<ITagsProcessor> tagsProcessor = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);
			CancellationTokenSource cts = new();

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 15, TagName = "InterruptTag" }
					 });

			tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(),
									It.IsAny<CancellationToken>()))
						 .Returns(async (IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
								  {
									  await Task.Delay(1000,
										  token); // Simulate a long-running operation that respects cancellation
									  foreach (TagSegment segment in segments["InterruptTag"])
									  {
										  segment.TagValue = "ShouldNotReachHere";
									  }
								  });

			string input = "Test ${InterruptTag}.";

			// Act
			cts.CancelAfter(100); // Cancel after 100 ms
			Func<Task> act
				= async () => await asyncParser.ParseAsync(input, tagsProcessor.Object, null, null, cts.Token);

			// Assert
			await act.Should().ThrowAsync<OperationCanceledException>();
		}

		#endregion

		#region Edge Cases Tests

		[Fact]
		public async Task ParseAsync_EmptyInput_ReturnsEmptyString()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>());

			string input = string.Empty;

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().BeEmpty();
		}

		[Fact]
		public async Task ParseAsync_NullInput_ThrowsArgumentNullException()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			// Act
			Func<Task> act = async () => await asyncParser.ParseAsync(null!);

			// Assert
			await act.Should().ThrowAsync<ArgumentNullException>();
		}

		[Fact]
		public async Task ParseAsync_InputWithOnlyTagsWithoutValues_ReturnsOriginalTags()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 8, TagName = "OnlyTag" }
					 });

			string input = "Text ${OnlyTag} with no resolved value.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Text ${OnlyTag} with no resolved value.");
		}

		[Fact]
		public async Task ParseAsync_InputWithNestedTags_ReturnsOriginalInput()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 18, TagName = "OuterTag" }
					 });

			string input = "Text ${Outer ${InnerTag}}.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Text ${Outer ${InnerTag}}.");
		}

		//[Fact]
		//public async Task ParseAsync_InputWithOverlappingTags_ReturnsOriginalInput()
		//{
		//	// Arrange
		//	var tokenizer = new Mock<IStringTokenizer>();
		//	var converterRegistry = new Mock<IConverterRegistry>();
		//	var asyncParser = new AsyncStringParser(converterRegistry.Object, tokenizer.Object);

		//	tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
		//			 .Returns(new List<TagSegment>
		//			 {
		//		 new TagSegment { Index = 5, Length = 25, TagName = "OverlapStart" },
		//		 new TagSegment { Index = 8, Length = 10, TagName = "OverlapInner" }
		//			 });

		//	string input = "Text ${Overlap ${OverlapInner}} here.";

		//	// Act
		//	string result = await asyncParser.ParseAsync(input);

		//	// Assert
		//	result.Should().Be("Text ${Overlap ${OverlapInner}} here.");
		//}

		[Fact]
		public async Task ParseAsync_InputWithIncompleteTags_ReturnsOriginalInput()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length = 5, TagName = "OpenTag" }
					 });

			string input = "Text ${OpenTag with missing end.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Text ${OpenTag with missing end.");
		}

		[Fact]
		public async Task ParseAsync_TagAtEndOfString_ReplacesTagCorrectly()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 12, Length = 9, TagName = "EndTag", Tag = "EndTag" }
					 });

			Dictionary<string, string> tagValues = new()
			{
				{ "EndTag", "EndValue" }
			};

			string input = "This is the ${EndTag}";

			// Act
			string result = await asyncParser.ParseAsync(input, null, tagValues);

			// Assert
			result.Should().Be("This is the EndValue");
		}

		[Fact]
		public async Task ParseAsync_TagAtEndOfStringWithoutResolvedValue_RetainsOriginalTag()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 12, Length = 9, TagName = "EndTag" }
					 });

			string input = "This is the ${EndTag}";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("This is the ${EndTag}");
		}

		#endregion

		#region Multi-line and Argument Parsing Tests

		[Fact]
		public async Task ParseAsync_MultiLineInput_ReplacesTagsAcrossLines()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 5, Length  = 8, TagName = "Tag1" },
						 new() { Index = 25, Length = 8, TagName = "Tag2" }
					 });

			string input = "Line 1 ${Tag1}\nLine 2 with ${Tag2}.";

			// Act
			string result = await asyncParser.ParseAsync(input);

			// Assert
			result.Should().Be("Line 1 ${Tag1}\nLine 2 with ${Tag2}.");
		}

		[Fact]
		public async Task ParseAsync_MultiLineInputWithResolvedTags_ReplacesTagsCorrectly()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new() { Index = 6, Length  = 12, TagName = "MultiTag1", Tag = "MultiTag1" },
						 new() { Index = 37, Length = 12, TagName = "MultiTag2", Tag = "MultiTag2" }
					 });

			Dictionary<string, string> tagValues = new()
			{
				{ "MultiTag1", "Value1" },
				{ "MultiTag2", "Value2" }
			};

			string input = "Start ${MultiTag1}\nAnother line with ${MultiTag2}.";

			// Act
			string result = await asyncParser.ParseAsync(input, null, tagValues);

			// Assert
			result.Should().Be("Start Value1\nAnother line with Value2.");
		}

		[Fact]
		public async Task ParseAsync_TagWithArguments_ParsesAndReplacesArguments()
		{
			// Arrange
			Mock<IStringTokenizer> tokenizer = new();
			Mock<IConverterRegistry> converterRegistry = new();
			AsyncStringParser asyncParser = new(converterRegistry.Object, tokenizer.Object);

			tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
					 .Returns(new List<TagSegment>
					 {
						 new()
						 {
							 Index = 6, Length = 20, TagName = "ArgTag", TagArg = "Arg1=Value",
							 Tag   = "ArgTag:Arg1=Value"
						 }
					 });

			Dictionary<string, string> tagValues = new()
			{
				{ "ArgTag:Arg1=Value", "ProcessedValue" }
			};

			string input = "Input ${ArgTag:Arg1=Value} with text.";

			// Act
			string result = await asyncParser.ParseAsync(input, null, tagValues);

			// Assert
			result.Should().Be("Input ProcessedValue with text.");
		}

		#endregion
	}
}