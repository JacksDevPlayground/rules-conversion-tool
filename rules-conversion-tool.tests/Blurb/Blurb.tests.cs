using rules_conversion_tool;

namespace Rules.Conversion.Tests.Blurb;

public class BlurbTests
{
    [Fact]
    public void GetBlurbContent_ShouldReturnContentBeforeEndIntro()
    {
        // Arrange
        var markdown = @"Scrum is easier than it seems, we'll explain how in these 8 simple steps.

`youtube: https://youtu.be/xOvFK328C_Q`

**Video: 8 Steps To Scrum - Scrum Explained (4 min)**

<!--endintro-->

::: good

![Figure: This Scrum image includes all the important steps from the Initial Meeting to the Sprint Review and Retro](8stepstoscrum-v5-2.jpg)

:::";

        var expectedBlurb = @"Scrum is easier than it seems, we'll explain how in these 8 simple steps.

`youtube: https://youtu.be/xOvFK328C_Q`

**Video: 8 Steps To Scrum - Scrum Explained (4 min)**";

        // Act
        var actualBlurb = ConversionCommand.GetBlurbContent(markdown);

        // Assert
        Assert.Equal(expectedBlurb, actualBlurb);
    }

    [Fact]
    public void GetBlurbContent_ShouldReturnEmptyString_WhenNoEndIntro()
    {
        // Arrange
        var markdown = @"This is some content without the endintro marker.";

        var expectedBlurb = string.Empty;

        // Act
        var actualBlurb = ConversionCommand.GetBlurbContent(markdown);

        // Assert
        Assert.Equal(expectedBlurb, actualBlurb);
    }

    [Fact]
    public void GetBlurbContent_ShouldHandleEmptyMarkdown()
    {
        // Arrange
        var markdown = string.Empty;

        var expectedBlurb = string.Empty;

        // Act
        var actualBlurb = ConversionCommand.GetBlurbContent(markdown);

        // Assert
        Assert.Equal(expectedBlurb, actualBlurb);
    }
}