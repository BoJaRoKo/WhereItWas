namespace WhereItWas.Gui;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tableLayoutPanel1 = new TableLayoutPanel();
        button1 = new Button();
        textBox1 = new TextBox();
        tabControl1 = new TabControl();
        tabPage1 = new TabPage();
        textBox2 = new TextBox();
        tabPage2 = new TabPage();
        dataGridView1 = new DataGridView();
        bindingSource1 = new BindingSource(components);
        button2 = new Button();
        button3 = new Button();
        labelExecutionTimer = new Label();
        comboBox1 = new ComboBox();
        queryTimer = new System.Windows.Forms.Timer(components);
        tableLayoutPanel1.SuspendLayout();
        tabControl1.SuspendLayout();
        tabPage1.SuspendLayout();
        tabPage2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
        SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 12;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.333332F));
        tableLayoutPanel1.Controls.Add(button1, 11, 0);
        tableLayoutPanel1.Controls.Add(textBox1, 0, 0);
        tableLayoutPanel1.Controls.Add(tabControl1, 0, 4);
        tableLayoutPanel1.Controls.Add(button2, 11, 13);
        tableLayoutPanel1.Controls.Add(button3, 11, 1);
        tableLayoutPanel1.Controls.Add(labelExecutionTimer, 11, 2);
        tableLayoutPanel1.Controls.Add(comboBox1, 0, 1);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 14;
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.Size = new Size(2082, 1061);
        tableLayoutPanel1.TabIndex = 2;
        // 
        // button1
        // 
        button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        button1.Location = new Point(1906, 3);
        button1.Name = "button1";
        button1.Size = new Size(173, 34);
        button1.TabIndex = 0;
        button1.Text = "Generate";
        button1.UseVisualStyleBackColor = true;
        button1.Click += Button1_Click;
        // 
        // textBox1
        // 
        tableLayoutPanel1.SetColumnSpan(textBox1, 11);
        textBox1.Dock = DockStyle.Fill;
        textBox1.Location = new Point(3, 3);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(1897, 31);
        textBox1.TabIndex = 1;
        // 
        // tabControl1
        // 
        tableLayoutPanel1.SetColumnSpan(tabControl1, 12);
        tabControl1.Controls.Add(tabPage1);
        tabControl1.Controls.Add(tabPage2);
        tabControl1.Dock = DockStyle.Fill;
        tabControl1.Location = new Point(3, 199);
        tabControl1.Name = "tabControl1";
        tableLayoutPanel1.SetRowSpan(tabControl1, 9);
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(2076, 813);
        tabControl1.TabIndex = 2;
        // 
        // tabPage1
        // 
        tabPage1.Controls.Add(textBox2);
        tabPage1.Location = new Point(4, 34);
        tabPage1.Name = "tabPage1";
        tabPage1.Padding = new Padding(3);
        tabPage1.Size = new Size(2068, 775);
        tabPage1.TabIndex = 0;
        tabPage1.Text = "SQL";
        tabPage1.UseVisualStyleBackColor = true;
        // 
        // textBox2
        // 
        textBox2.Dock = DockStyle.Fill;
        textBox2.Location = new Point(3, 3);
        textBox2.Multiline = true;
        textBox2.Name = "textBox2";
        textBox2.ScrollBars = ScrollBars.Both;
        textBox2.Size = new Size(2062, 769);
        textBox2.TabIndex = 0;
        // 
        // tabPage2
        // 
        tabPage2.Controls.Add(dataGridView1);
        tabPage2.Location = new Point(4, 34);
        tabPage2.Name = "tabPage2";
        tabPage2.Padding = new Padding(3);
        tabPage2.Size = new Size(2068, 775);
        tabPage2.TabIndex = 1;
        tabPage2.Text = "Dane";
        tabPage2.UseVisualStyleBackColor = true;
        // 
        // dataGridView1
        // 
        dataGridView1.AllowUserToAddRows = false;
        dataGridView1.AllowUserToDeleteRows = false;
        dataGridView1.AutoGenerateColumns = false;
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView1.DataSource = bindingSource1;
        dataGridView1.Dock = DockStyle.Fill;
        dataGridView1.Location = new Point(3, 3);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.ReadOnly = true;
        dataGridView1.RowHeadersVisible = false;
        dataGridView1.RowHeadersWidth = 62;
        dataGridView1.Size = new Size(2062, 769);
        dataGridView1.TabIndex = 0;
        // 
        // button2
        // 
        button2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        button2.Location = new Point(1906, 1018);
        button2.Name = "button2";
        button2.Size = new Size(173, 34);
        button2.TabIndex = 3;
        button2.Text = "Exit";
        button2.UseVisualStyleBackColor = true;
        button2.Click += Button2_Click;
        // 
        // button3
        // 
        button3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        button3.Location = new Point(1906, 43);
        button3.Name = "button3";
        button3.Size = new Size(173, 34);
        button3.TabIndex = 4;
        button3.Text = "Execute";
        button3.UseVisualStyleBackColor = true;
        button3.Click += Button3_Click;
        // 
        // labelExecutionTimer
        // 
        labelExecutionTimer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        labelExecutionTimer.AutoSize = true;
        labelExecutionTimer.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
        labelExecutionTimer.Location = new Point(1906, 80);
        labelExecutionTimer.Name = "labelExecutionTimer";
        labelExecutionTimer.Size = new Size(173, 25);
        labelExecutionTimer.TabIndex = 6;
        labelExecutionTimer.Text = "Czas: 00:00:00";
        labelExecutionTimer.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // comboBox1
        // 
        comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel1.SetColumnSpan(comboBox1, 2);
        comboBox1.FormattingEnabled = true;
        comboBox1.Items.AddRange(new object[] { "Tylko liczba znalezionych rekordów", "Tylko ścieżka", "Pola danych bez ścieżki", "Pola danych i ścieżka" });
        comboBox1.Location = new Point(3, 43);
        comboBox1.Name = "comboBox1";
        comboBox1.Size = new Size(340, 33);
        comboBox1.TabIndex = 7;
        // 
        // queryTimer
        // 
        queryTimer.Interval = 250;
        queryTimer.Tick += QueryTimer_Tick;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(2082, 1061);
        Controls.Add(tableLayoutPanel1);
        HelpButton = true;
        Name = "Form1";
        Text = "Gdzie to było ?";
        Load += Form1_Load;
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        tabControl1.ResumeLayout(false);
        tabPage1.ResumeLayout(false);
        tabPage1.PerformLayout();
        tabPage2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
        ResumeLayout(false);
    }

    #endregion
    private TableLayoutPanel tableLayoutPanel1;
    private Button button1;
    private TextBox textBox1;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private TextBox textBox2;
    private TabPage tabPage2;
    private Button button2;
    private DataGridView dataGridView1;
    private BindingSource bindingSource1;
    private Button button3;
    private Label labelExecutionTimer;
    private System.Windows.Forms.Timer queryTimer;
    private ComboBox comboBox1;
}
