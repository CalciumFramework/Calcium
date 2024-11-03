//using BenchmarkDotNet.Attributes;
//using Calcium.ResourcesModel.Experimental;
//using Moq;

//using Calcium.ResourcesModel;
//using BenchmarkDotNet.Running;

//namespace Calcium.Tests.ResourcesModel.AsyncStringParserTests
//{
//	public class BenchmarkEntryPoint
//	{
//		public static void Main(string[] args)
//		{
//			// Run the benchmark class with your test methods
//			BenchmarkRunner.Run<AsyncStringParserPerformanceTests>();
//		}
//	}

//	[MemoryDiagnoser]
//	public class AsyncStringParserPerformanceTests
//	{
//		private readonly AsyncStringParser _parser;
//		private readonly Mock<IStringTokenizer> _tokenizer;
//		private readonly Mock<IConverterRegistry> _converterRegistry;
//		private readonly Mock<ITagsProcessor> _tagsProcessor;

//		public AsyncStringParserPerformanceTests()
//		{
//			_tokenizer = new Mock<IStringTokenizer>();
//			_converterRegistry = new Mock<IConverterRegistry>();
//			_tagsProcessor = new Mock<ITagsProcessor>();

//			_parser = new AsyncStringParser(_converterRegistry.Object, _tokenizer.Object);
//		}

//		[Benchmark]
//		public async Task ParseLargeInputWithManyTags()
//		{
//			string largeInput = string.Concat(Enumerable.Repeat("This is a test ${Tag} ", 10000));
//			_tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
//					  .Returns(Enumerable.Range(0, 10000).Select(i => new TagSegment { Index = (uint)(i * 18), Length = 7, TagName = "Tag" }).ToList());

//			await _parser.ParseAsync(largeInput);
//		}

//		[Benchmark]
//		public async Task ParseWithComplexTagResolution()
//		{
//			string input = "Test ${ComplexTag} example.";
//			_tokenizer.Setup(t => t.Tokenize(It.IsAny<string>(), It.IsAny<TagDelimiters>()))
//					  .Returns(new List<TagSegment> { new TagSegment { Index = 5, Length = 12, TagName = "ComplexTag" } });

//			_tagsProcessor.Setup(t => t.SetTagValuesAsync(It.IsAny<IDictionary<string, ISet<TagSegment>>>(), It.IsAny<CancellationToken>()))
//						  .Returns(async (IDictionary<string, ISet<TagSegment>> segments, CancellationToken token) =>
//						  {
//							  await Task.Delay(100, token); // Simulate complex processing
//							  foreach (var segment in segments["ComplexTag"])
//							  {
//								  segment.TagValue = "ResolvedValue";
//							  }
//						  });

//			await _parser.ParseAsync(input, _tagsProcessor.Object);
//		}

//		[Benchmark]
//		public void ParseWithConcurrentCalls()
//		{
//			Parallel.For(0, 100, async i =>
//			{
//				string input = $"Test {i} ${{Tag}}";
//				await _parser.ParseAsync(input);
//			});
//		}
//	}
//}
