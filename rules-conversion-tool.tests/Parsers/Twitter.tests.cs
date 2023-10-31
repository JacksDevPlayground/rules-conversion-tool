using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers;

public class TwitterTests
{
    [Fact]
    public void Parse_ShouldReturnCorrectTweetComponent()
    {
        // Arrange
        var markdownContent = @"Here's a tweet for you to check out:

`oembed: https://twitter.com/user/status/1234567890`

Isn't it interesting?";

        var expectedOutput = @"Here's a tweet for you to check out:

{% TweetComponent #1234567890  /%}

Isn't it interesting?";

        // Act
        var result = Twitter.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void Parse_WithoutTwitterLinks_ShouldReturnOriginalMarkdown()
    {
        // Arrange
        var markdownWithoutTwitter = @"This is a sample markdown content.
It doesn't contain any Twitter links.
Just plain text.";

        // Act
        var result = Twitter.Parse(markdownWithoutTwitter);

        // Assert
        Assert.Equal(markdownWithoutTwitter, result);
    }

    [Fact]
    public void Parse_WithInvalidTwitterLink_ShouldReturnOriginalMarkdown()
    {
        // Arrange
        var markdownWithInvalidLink = @"Here's a tweet for you to check out:

`oembed: https://example.com/user/status/1234567890`

Isn't it interesting?";

        // Act
        var result = Twitter.Parse(markdownWithInvalidLink);

        // Assert
        Assert.Equal(markdownWithInvalidLink, result);
    }

    [Fact]
    public void ExtractTweetId_ShouldReturnCorrectId()
    {
        // Arrange
        var tweetLink = "oembed: https://twitter.com/user/status/9876543210";

        // Act
        var tweetId = Twitter.ExtractTweetId(tweetLink);

        // Assert
        Assert.Equal("9876543210", tweetId);
    }

    [Fact]
    public void ExtractTweetId_WithInvalidLink_ShouldReturnNull()
    {
        // Arrange
        var invalidLink = "oembed: https://example.com/user/status/9876543210";

        // Act
        var tweetId = Twitter.ExtractTweetId(invalidLink);

        // Assert
        Assert.Null(tweetId);
    }
}