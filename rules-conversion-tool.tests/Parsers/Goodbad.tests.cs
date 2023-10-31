using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers;

public class GoodbadTests
{
    [Fact]
    public void Parse_ShouldReturnCorrectGoodBadComponent_WithFigureText()
    {
        // Arrange
        var markdownContent = @"::: good  
![Figure: Caption for good images](https://images.unsplash.com/photo-1491472253230-a044054ca35f?ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80)
:::";

        var expectedOutput =
            @"{% GoodBadImageComponent type=""good"" image=""https://images.unsplash.com/photo-1491472253230-a044054ca35f?ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80"" figureText=""Figure: Caption for good images"" /%}";

        // Act
        var result = GoodBad.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldReturnCorrectGoodBadComponent_WithoutFigureText()
    {
        // Arrange
        var markdownContent = @"::: bad  
Some content without a figure text.
:::";

        var expectedOutput = @"{% GoodBadComponent type=""bad"" figureText="""" %}
Some content without a figure text.
{% /GoodBadComponent %}";
        expectedOutput = expectedOutput.Replace("\r\n", "\n");
        // Act
        var result = GoodBad.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }

    [Fact]
    public void Parse_ShouldHandleMultipleGoodBadComponents()
    {
        // Arrange
        var markdownContent = @"::: ok  
![Figure: Caption for OK images](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
:::

::: bad  
Some content for bad.
:::";

        string expectedOutput =
            @"{% GoodBadImageComponent type=""ok"" image=""https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80"" figureText=""Figure: Caption for OK images"" /%}

{% GoodBadComponent type=""bad"" figureText="""" %}
Some content for bad.
{% /GoodBadComponent %}".Replace("\r\n", "\n");

        // Act - Weirdness with newlines Windows vs Linux?!?!!?
        var result = GoodBad.Parse(markdownContent).Replace("\r\n", "\n");


        // Assert
        Assert.Equal(expectedOutput, result.Trim());
    }
}