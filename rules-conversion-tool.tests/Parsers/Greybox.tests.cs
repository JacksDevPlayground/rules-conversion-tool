using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers;

public class GreyboxTests
{
    [Fact]
    public void Parse_ShouldReturnCorrectGreyboxComponent()
    {
        // Arrange
        var markdownContent = @"::: greybox  
Hello, this is a greybox content!
:::";

        var expectedOutput = @"{% GreyboxComponent %}
Hello, this is a greybox content!
{% /GreyboxComponent %}";
expectedOutput = expectedOutput.Replace("\r\n", "\n");
        // Act
        var result = Greybox.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldHandleMultipleGreyboxComponents()
    {
        // Arrange
        var markdownContent = @"::: greybox  
First greybox content.
:::

Some other content here.

::: greybox  
Second greybox content.
:::";

        var expectedOutput = @"{% GreyboxComponent %}
First greybox content.
{% /GreyboxComponent %}

Some other content here.

{% GreyboxComponent %}
Second greybox content.
{% /GreyboxComponent %}";
expectedOutput = expectedOutput.Replace("\r\n", "\n");
        // Act
        var result = Greybox.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldNotModifyContentOutsideGreybox()
    {
        // Arrange
        var markdownContent = @"This is a regular content.

::: greybox  
Greybox content.
:::

Another regular content.";

        var expectedOutput = @"This is a regular content.

{% GreyboxComponent %}
Greybox content.
{% /GreyboxComponent %}

Another regular content.";
        // Act
        var result = Greybox.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }
}