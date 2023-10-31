using rules_conversion_tool;

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.SetApplicationName("MarkdownConverter");

            // Register the Conversion command
            config.AddCommand<ConversionCommand>("convert");
        });
        return app.Run(args);
    }
}