using rules_conversion_tool.Parsers;

namespace Rules.Conversion.Tests.Parsers;

public class EmailTests
{
    [Fact]
    public void Parse_ShouldReturnCorrectEmailComponent()
    {
        // Arrange
        string markdownContent = @"::: email-template
|          |     |
| -------- | --- |
| To:      | Bob Northwind |
| Cc:      | {{ ANYONE YOU'RE WORKING WITH }} |
| Subject: | {{ YOUR NAME / TEAM NAME }} - Daily Scrum |
::: email-content  

### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX

:::
:::";

        string expectedOutput = @"{% EmailTemplateComponent to=""Bob Northwind"" cc=""{{ ANYONE YOU'RE WORKING WITH }}"" bcc="""" subject=""{{ YOUR NAME / TEAM NAME }} - Daily Scrum"" %}
### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX
{% /EmailTemplateComponent %}";

        // Act
        string result = Email.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result);
    }
    
    [Fact]
    public void Parse_ShouldNotModifyContentOutsideEmail()
    {
        // Arrange
        string markdownContent = @"This is regular content

::: email-template
|          |     |
| -------- | --- |
| To:      | Bob Northwind |
| Cc:      | {{ ANYONE YOU'RE WORKING WITH }} |
| Subject: | {{ YOUR NAME / TEAM NAME }} - Daily Scrum |
::: email-content  

### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX

:::
:::

Wow! more content.";

        string expectedOutput = @"This is regular content

{% EmailTemplateComponent to=""Bob Northwind"" cc=""{{ ANYONE YOU'RE WORKING WITH }}"" bcc="""" subject=""{{ YOUR NAME / TEAM NAME }} - Daily Scrum"" %}
### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX
{% /EmailTemplateComponent %}

Wow! more content.";

        // Act
        string result = Email.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result);
    }
    
    
    [Fact]
    public void Parse_ShouldHandleMultipleEmailComponents()
    {
        // Arrange
        string markdownContent = @"This is regular content

::: email-template
|          |     |
| -------- | --- |
| To:      | Bob Northwind |
| Cc:      | {{ ANYONE YOU'RE WORKING WITH }} |
| Subject: | {{ YOUR NAME / TEAM NAME }} - Daily Scrum |
::: email-content  

### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX

:::
:::

Wow! more content.

::: email-template
|          |     |
| -------- | --- |
| To:      | Bob Northwind |
| Cc:      | {{ ANYONE YOU'RE WORKING WITH }} |
| Subject: | {{ YOUR NAME / TEAM NAME }} - Daily Scrum |
::: email-content  

### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX

:::
:::";

        string expectedOutput = @"This is regular content

{% EmailTemplateComponent to=""Bob Northwind"" cc=""{{ ANYONE YOU'RE WORKING WITH }}"" bcc="""" subject=""{{ YOUR NAME / TEAM NAME }} - Daily Scrum"" %}
### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX
{% /EmailTemplateComponent %}

Wow! more content.

{% EmailTemplateComponent to=""Bob Northwind"" cc=""{{ ANYONE YOU'RE WORKING WITH }}"" bcc="""" subject=""{{ YOUR NAME / TEAM NAME }} - Daily Scrum"" %}
### Hi Bob,

Yesterday I worked on:

* ✅ Done - XXX
* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ❌ Blocked - XXX

Today I'm working on:

* ⏳ In Progress - XXX
* ⬜ PBI - XXX
* ⬜ Email - XXX
* ❌ Blocked - XXX
{% /EmailTemplateComponent %}";

        // Act
        string result = Email.Parse(markdownContent);

        // Assert
        Assert.Equal(expectedOutput, result);
    }
}
