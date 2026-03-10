namespace Winreels;

partial class UploadInterface
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
        description = new TextBox();
        tags = new TextBox();
        file = new OpenFileDialog();
        select_file = new Button();
        publish = new Button();
        watch = new Button();
        backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        label2 = new Label();
        SuspendLayout();
        // 
        // description
        // 
        description.Font = new Font("Aharoni", 9.75F);
        description.Location = new Point(143, 78);
        description.Name = "description";
        description.PlaceholderText = "Content";
        description.Size = new Size(135, 20);
        description.TabIndex = 4;
        description.TextAlign = HorizontalAlignment.Center;
        // 
        // tags
        // 
        tags.Font = new Font("Aharoni", 9.75F);
        tags.Location = new Point(128, 115);
        tags.Name = "tags";
        tags.PlaceholderText = "Tags (Serperate by ' ')";
        tags.Size = new Size(160, 20);
        tags.TabIndex = 5;
        tags.TextAlign = HorizontalAlignment.Center;
        // 
        // file
        // 
        file.FileName = "openFileDialog1";
        // 
        // select_file
        // 
        select_file.FlatAppearance.BorderColor = Color.Black;
        select_file.FlatStyle = FlatStyle.Flat;
        select_file.Image = Properties.Resources.file;
        select_file.Location = new Point(156, 152);
        select_file.Name = "select_file";
        select_file.Size = new Size(51, 46);
        select_file.TabIndex = 6;
        select_file.UseVisualStyleBackColor = true;
        select_file.Click += SelectFile;
        // 
        // publish
        // 
        publish.FlatAppearance.BorderColor = Color.Black;
        publish.FlatStyle = FlatStyle.Flat;
        publish.Image = Properties.Resources.publish;
        publish.Location = new Point(213, 143);
        publish.Name = "publish";
        publish.Size = new Size(43, 39);
        publish.TabIndex = 7;
        publish.UseVisualStyleBackColor = true;
        publish.Click += Publish;
        // 
        // watch
        // 
        watch.FlatAppearance.BorderColor = Color.Black;
        watch.FlatStyle = FlatStyle.Flat;
        watch.Image = Properties.Resources.watch;
        watch.Location = new Point(161, 188);
        watch.Name = "watch";
        watch.Size = new Size(95, 102);
        watch.TabIndex = 8;
        watch.UseVisualStyleBackColor = true;
        watch.Click += Watch;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Aharoni", 25F, FontStyle.Bold);
        label2.ForeColor = Color.Gold;
        label2.Location = new Point(128, 19);
        label2.Name = "label2";
        label2.Size = new Size(170, 34);
        label2.TabIndex = 18;
        label2.Text = "My Video";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // UploadInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(403, 293);
        Controls.Add(label2);
        Controls.Add(watch);
        Controls.Add(publish);
        Controls.Add(select_file);
        Controls.Add(tags);
        Controls.Add(description);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        Name = "UploadInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private TextBox description;
    private TextBox tags;
    private OpenFileDialog file;
    private Button select_file;
    private Button publish;
    private Button watch;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private Label label2;
}
