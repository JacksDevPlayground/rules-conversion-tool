using rules_conversion_tool;
using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers
{
    public class YoutubeTests
    {
        [Fact]
        public void Parse_ShouldReturnCorrectYoutubeComponent()
        {
            // Arrange
            var markdownContent = @"Scrum is easier than it seems, we'll explain how in these 8 simple steps.

`youtube: https://youtu.be/xOvFK328C_Q`

  **Video: 8 Steps To Scrum - Scrum Explained (4 min)**";

            var expectedOutput = @"Scrum is easier than it seems, we'll explain how in these 8 simple steps.

{% YoutubeVideoComponent videoId=""xOvFK328C_Q"" figureText=""Video: 8 Steps To Scrum - Scrum Explained (4 min)""  /%}";

            // Act
            var result = Youtube.Parse(markdownContent);

            // Assert
            Assert.Equal(expectedOutput, result.Trim());
        }

        [Fact]
        public void Parse_ShouldExtractYoutubeLinks_WithoutFigureText()
        {
            // Arrange
            var filePath = "../../../Test-rules/rule.md";
                // "C:\\Users\\jpr17\\src\\POC\\rules-conversion-tool\\rules-conversion-tool.tests\\Test-rules/rule.md"; // Replace with the correct path to your file
            var rawContent = File.ReadAllText(filePath);
            var content = ConversionCommand.GetBodyContent(rawContent);

            // Act
            var result = Youtube.Parse(content);

            // Assert
            Assert.NotNull(content);
            Assert.NotEmpty(content);
            Assert.Contains("{% YoutubeVideoComponent videoId=\"qVIQNu00LGo\" figureText=\"\"  /%}", result);
            Assert.Contains("{% YoutubeVideoComponent videoId=\"9HEAm6lpNP8\" figureText=\"\"  /%}", result);
        }

        [Fact]
        public void Parse_WithoutYoutubeComponents_ShouldReturnOriginalMarkdown()
        {
            // Arrange
            var markdownWithoutYoutube = @"This is a sample markdown content.
It doesn't contain any YouTube components.
Just plain text.";

            // Act
            var result = Youtube.Parse(markdownWithoutYoutube);

            // Assert
            Assert.Equal(markdownWithoutYoutube, result);
        }
    }
}
