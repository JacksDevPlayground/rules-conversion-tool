using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers;

public class ImageTests
{
    [Fact]
    public void Parse_WithInvalidTwitterLink_ShouldReturnOriginalMarkdown()
    {
        // Arrange
        const string markdown = """
                                ::: info
                                This is a <div> using the class "info". Works the same as using a <p> . Lorem ipsum dolor sit amet, consectetur
                                adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis
                                nostrud exercitation.
                                :::

                                ::: img-small
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::

                                ::: img-medium
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::

                                ::: img-large
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::

                                ::: no-border
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::
                                """;

        // Act
        var result = Image.Parse(markdown);

        // Assert
        Assert.Equal(markdown, result);
    }
    
    [Fact]
    public void Parse_WithSmallImageMarkdown_ShouldReturnCustomImageComponentSmall()
    {
        // Arrange
        const string markdown = """
                                ::: img-small
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::
                                """;
        const string expected = "{% CustomImageComponent type=\"small\" image=\"/rules/testttt/404-good.jpg\" /%}";

        // Act
        var result = Image.Parse(markdown);

        // Assert
        Assert.Equal(expected, result.Trim()); // Trim is used to remove any leading/trailing whitespace
    }
    
    [Fact]
    public void Parse_WithMediumImageMarkdown_ShouldReturnCustomImageComponentMedium()
    {
        // Arrange
        const string markdown = """
                                ::: img-medium
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::
                                """;
        const string expected = "{% CustomImageComponent type=\"medium\" image=\"/rules/testttt/404-good.jpg\" /%}";

        // Act
        var result = Image.Parse(markdown);

        // Assert
        Assert.Equal(expected, result.Trim());
    }

    [Fact]
    public void Parse_WithLargeImageMarkdown_ShouldReturnCustomImageComponentLarge()
    {
        // Arrange
        const string markdown = """
                                ::: img-large
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::
                                """;
        const string expected = "{% CustomImageComponent type=\"large\" image=\"/rules/testttt/404-good.jpg\" /%}";

        // Act
        var result = Image.Parse(markdown);

        // Assert
        Assert.Equal(expected, result.Trim());
    }

    [Fact]
    public void Parse_WithNoBorderImageMarkdown_ShouldReturnCustomImageComponentNoBorder()
    {
        // Arrange
        const string markdown = """
                                ::: no-border
                                ![caption](https://images.unsplash.com/photo-1478998674531-cb7d22e769df?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80)
                                :::
                                """;
        const string expected = "{% CustomImageComponent type=\"noborder\" image=\"/rules/testttt/404-good.jpg\" /%}";

        // Act
        var result = Image.Parse(markdown);

        // Assert
        Assert.Equal(expected, result.Trim());
    }
}
