namespace Winreels;

partial class VideoInterface
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
        label2 = new Label();
        description = new TextBox();
        tags = new TextBox();
        file = new OpenFileDialog();
        select_file = new Button();
        publish = new Button();
        watch = new Button();
        backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        SuspendLayout();
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
        label2.ForeColor = Color.Gold;
        label2.Location = new Point(467, 9);
        label2.Name = "label2";
        label2.Size = new Size(238, 46);
        label2.TabIndex = 1;
        label2.Text = "Video Upload";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // description
        // 
        description.Location = new Point(513, 143);
        description.Name = "description";
        description.PlaceholderText = "Description";
        description.Size = new Size(135, 25);
        description.TabIndex = 4;
        // 
        // tags
        // 
        tags.Location = new Point(513, 174);
        tags.Name = "tags";
        tags.PlaceholderText = "Tags (Serperate by ' ')";
        tags.Size = new Size(135, 25);
        tags.TabIndex = 5;
        // 
        // file
        // 
        file.FileName = "openFileDialog1";
        // 
        // select_file
        // 
        select_file.Location = new Point(501, 222);
        select_file.Name = "select_file";
        select_file.Size = new Size(75, 23);
        select_file.TabIndex = 6;
        select_file.Text = "Select File";
        select_file.UseVisualStyleBackColor = true;
        select_file.Click += SelectFile;
        // 
        // publish
        // 
        publish.Location = new Point(582, 222);
        publish.Name = "publish";
        publish.Size = new Size(75, 23);
        publish.TabIndex = 7;
        publish.Text = "Publish";
        publish.UseVisualStyleBackColor = true;
        publish.Click += Publish;
        // 
        // watch
        // 
        watch.Location = new Point(540, 260);
        watch.Name = "watch";
        watch.Size = new Size(75, 23);
        watch.TabIndex = 8;
        watch.Text = "Watch";
        watch.UseVisualStyleBackColor = true;
        watch.Click += Watch;
        // 
        // VideoInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(1184, 661);
        Controls.Add(watch);
        Controls.Add(publish);
        Controls.Add(select_file);
        Controls.Add(tags);
        Controls.Add(description);
        Controls.Add(label2);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        Name = "VideoInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label2;
    private TextBox description;
    private TextBox tags;
    private OpenFileDialog file;
    private Button select_file;
    private Button publish;
    private Button watch;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
}
