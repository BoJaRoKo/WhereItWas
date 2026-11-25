using Markdig;

namespace WhereItWas.Gui;

public partial class HelpForm : Form
{
    private readonly string _html;

    public HelpForm(string markdown)
    {
        InitializeComponent();

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .DisableHtml()
            .Build();
        var body = Markdown.ToHtml(markdown, pipeline);
        _html = $$"""
            <!DOCTYPE html>
            <html lang="pl">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <meta name="color-scheme" content="light">
                <style>
                    html { background: #ffffff; color-scheme: light; }
                    body { background: #ffffff; color: #242424; font-family: "Segoe UI", sans-serif; line-height: 1.55; margin: 2rem auto; max-width: 900px; padding: 0 1.5rem; }
                    h1, h2, h3 { line-height: 1.25; color: #242424; }
                    table { border-collapse: collapse; width: 100%; margin: 1rem 0; }
                    th, td { border: 1px solid #ccc; padding: .45rem .65rem; text-align: left; color: #242424; }
                    th { background: #f2f2f2; }
                    code { background: #f2f2f2; color: #242424; padding: .1rem .25rem; }
                    pre { background: #f2f2f2; color: #242424; padding: 1rem; overflow-x: auto; }
                    pre code { padding: 0; }
                    blockquote { border-left: 3px solid #888; margin-left: 0; padding-left: 1rem; color: #555; }
                </style>
            </head>
            <body>{{body}}</body>
            </html>
            """;

        Shown += HelpForm_Shown;
    }

    private async void HelpForm_Shown(object? sender, EventArgs e)
    {
        try
        {
            await webViewHelp.EnsureCoreWebView2Async();
            webViewHelp.NavigateToString(_html);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Nie można wyświetlić pomocy. Sprawdź, czy środowisko uruchomieniowe Microsoft Edge WebView2 jest zainstalowane.\n\n{ex.Message}",
                "WhereItWas — pomoc",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Close();
        }
    }
}
