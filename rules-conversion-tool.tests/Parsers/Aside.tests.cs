using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers;

public class AsideTests
{
    [Fact]
    public void Parse_ShouldReturnCorrectAsideComponent_WithoutContent()
    {
        // Arrange
        var markdownContent = @"::: info

:::";
        var expectedOutput = @"{% AsideComponent type=""info"" /%}";
        expectedOutput = expectedOutput.Replace("\r\n", "\n");
        // Act
        var result = Aside.Parse(markdownContent); // Assuming you have a Parse method in Asides

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldReturnCorrectAsideComponent_WithContent()
    {
        // Arrange
        var markdownContent = @"::: warning
This is a warning message.
:::";

        var expectedOutput = @"{% AsideComponent type=""warning"" %}
This is a warning message.
{% /AsideComponent %}";
        expectedOutput = expectedOutput.Replace("\r\n", "\n");
        // Act
        var result = Aside.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldHandleHtmlContentInsideAside()
    {
        // Arrange
        var markdownContent = @"::: codeauditor
This is a <div> using the class ""codeauditor"". Works the same as using a <p>.
:::";

        var expectedOutput = @"{% AsideComponent type=""codeauditor"" %}
This is a <div> using the class ""codeauditor"". Works the same as using a <p>.
{% /AsideComponent %}";
        expectedOutput = expectedOutput.Replace("\r\n", "\n");
        // Act
        var result = Aside.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldIgnoreUnknownAsideTypes()
    {
        // Arrange
        var markdownContent = @"::: unknownType
This is an unknown type.
:::";

        // Act
        var result = Aside.Parse(markdownContent);

        // Assert
        Assert.Equal(markdownContent, result.Trim()); // Expecting the original content to be unchanged
    }
}