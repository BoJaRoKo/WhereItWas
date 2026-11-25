namespace WhereItWas.Gui;

partial class HelpForm
{
    private System.ComponentModel.IContainer components = null;
    private Microsoft.Web.WebView2.WinForms.WebView2 webViewHelp;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        webViewHelp = new Microsoft.Web.WebView2.WinForms.WebView2();
        ((System.ComponentModel.ISupportInitialize)webViewHelp).BeginInit();
        SuspendLayout();
        // 
        // webViewHelp
        // 
        webViewHelp.AllowExternalDrop = true;
        webViewHelp.CreationProperties = null;
        webViewHelp.DefaultBackgroundColor = Color.White;
        webViewHelp.Dock = DockStyle.Fill;
        webViewHelp.Location = new Point(0, 0);
        webViewHelp.Name = "webViewHelp";
        webViewHelp.Size = new Size(1000, 700);
        webViewHelp.TabIndex = 0;
        webViewHelp.ZoomFactor = 1D;
        // 
        // HelpForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 700);
        Controls.Add(webViewHelp);
        MinimumSize = new Size(640, 480);
        Name = "HelpForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "WhereItWas — pomoc";
        ((System.ComponentModel.ISupportInitialize)webViewHelp).EndInit();
        ResumeLayout(false);
    }
}
