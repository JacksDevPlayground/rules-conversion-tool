using rules_conversion_tool;

namespace Rules.Conversion.Tests.Body;

public class BodyTests
{
    [Fact]
    public void GetBodyContent_ShouldHandleEmptyMarkdown()
    {
        // Arrange
        var markdown = "";

        // Act
        var result = ConversionCommand.GetBodyContent(markdown);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetBodyContent_ShouldReturnContentAfterEndIntro()
    {
        // Arrange
        var markdown = "This is the blurb.\n<!--endintro-->\nThis is the body.";

        // Act
        var result = ConversionCommand.GetBodyContent(markdown);

        // Assert
        Assert.Equal("This is the body.", result.Trim());
    }

    [Fact]
    public void GetBodyContent_ShouldReturnEmptyWhenNoEndIntro()
    {
        // Arrange
        var markdown = "This is just some content without an endintro marker.";

        // Act
        var result = ConversionCommand.GetBodyContent(markdown);

        // Assert
        Assert.Equal(string.Empty, result.Trim());
    }

    [Fact]
    public void GetBodyContent_ShouldHandleMultipleEndIntroMarkers()
    {
        // Arrange
        var markdown = "Blurb 1.\n<!--endintro-->\nBody 1.\n<!--endintro-->\nBody 2.";

        // Act
        var result = ConversionCommand.GetBodyContent(markdown);

        // Assert
        Assert.Equal("Body 1.\n<!--endintro-->\nBody 2.", result.Trim());
    }
}