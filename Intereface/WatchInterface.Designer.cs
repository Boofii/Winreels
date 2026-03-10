namespace Winreels;

partial class WatchInterface
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
        backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        upload = new Button();
        view = new LibVLCSharp.WinForms.VideoView();
        next = new Button();
        description = new Label();
        tags = new Label();
        likes = new Button();
        previous = new Button();
        label2 = new Label();
        ((System.ComponentModel.ISupportInitialize)view).BeginInit();
        SuspendLayout();
        // 
        // upload
        // 
        upload.FlatAppearance.BorderColor = Color.Black;
        upload.FlatStyle = FlatStyle.Flat;
        upload.Image = Properties.Resources.upload2;
        upload.Location = new Point(513, 556);
        upload.Name = "upload";
        upload.Size = new Size(140, 132);
        upload.TabIndex = 9;
        upload.UseVisualStyleBackColor = true;
        upload.Click += Upload;
        // 
        // view
        // 
        view.BackColor = Color.Black;
        view.Location = new Point(321, 73);
        view.MediaPlayer = null;
        view.Name = "view";
        view.Size = new Size(526, 477);
        view.TabIndex = 10;
        view.Text = "videoView1";
        // 
        // next
        // 
        next.FlatAppearance.BorderColor = Color.Black;
        next.FlatStyle = FlatStyle.Flat;
        next.ForeColor = Color.White;
        next.Image = Properties.Resources.previous;
        next.Location = new Point(969, 287);
        next.Name = "next";
        next.Size = new Size(88, 127);
        next.TabIndex = 11;
        next.UseVisualStyleBackColor = true;
        next.Click += next_Click;
        // 
        // description
        // 
        description.BackColor = Color.Linen;
        description.BorderStyle = BorderStyle.FixedSingle;
        description.Font = new Font("Aharoni", 25F, FontStyle.Bold);
        description.ForeColor = Color.Black;
        description.Location = new Point(63, 73);
        description.Name = "description";
        description.Size = new Size(192, 340);
        description.TabIndex = 14;
        description.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // tags
        // 
        tags.BackColor = Color.Linen;
        tags.BorderStyle = BorderStyle.FixedSingle;
        tags.Font = new Font("Aharoni", 10F, FontStyle.Bold);
        tags.Location = new Point(63, 413);
        tags.Name = "tags";
        tags.Size = new Size(192, 43);
        tags.TabIndex = 15;
        tags.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // likes
        // 
        likes.BackColor = Color.Black;
        likes.BackgroundImageLayout = ImageLayout.None;
        likes.FlatAppearance.BorderColor = Color.Black;
        likes.FlatStyle = FlatStyle.Flat;
        likes.Font = new Font("Aharoni", 20F, FontStyle.Bold);
        likes.ForeColor = Color.White;
        likes.Image = Properties.Resources.heart;
        likes.Location = new Point(969, 73);
        likes.Name = "likes";
        likes.Size = new Size(88, 94);
        likes.TabIndex = 13;
        likes.Text = "(0)";
        likes.TextAlign = ContentAlignment.BottomCenter;
        likes.UseVisualStyleBackColor = false;
        likes.Click += likes_Click;
        // 
        // previous
        // 
        previous.FlatAppearance.BorderColor = Color.Black;
        previous.FlatStyle = FlatStyle.Flat;
        previous.ForeColor = Color.White;
        previous.Image = Properties.Resources.previous1;
        previous.Location = new Point(969, 173);
        previous.Name = "previous";
        previous.Size = new Size(88, 124);
        previous.TabIndex = 16;
        previous.UseVisualStyleBackColor = true;
        previous.Click += previous_Click;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Aharoni", 25F, FontStyle.Bold);
        label2.ForeColor = Color.Gold;
        label2.Location = new Point(501, 23);
        label2.Name = "label2";
        label2.Size = new Size(152, 34);
        label2.TabIndex = 17;
        label2.Text = "My Feed";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // WatchInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(1184, 661);
        Controls.Add(label2);
        Controls.Add(previous);
        Controls.Add(tags);
        Controls.Add(description);
        Controls.Add(likes);
        Controls.Add(next);
        Controls.Add(view);
        Controls.Add(upload);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        KeyPreview = true;
        Name = "WatchInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        Load += FormLoad;
        ((System.ComponentModel.ISupportInitialize)view).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private Button upload;
    private LibVLCSharp.WinForms.VideoView view;
    private Button next;
    private Label description;
    private Label tags;
    private Button likes;
    private Button previous;
    private Label label2;
}
