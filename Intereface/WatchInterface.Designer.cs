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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WatchInterface));
        label2 = new Label();
        backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        upload = new Button();
        axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
        ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).BeginInit();
        SuspendLayout();
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
        label2.ForeColor = Color.Gold;
        label2.Location = new Point(467, 9);
        label2.Name = "label2";
        label2.Size = new Size(222, 46);
        label2.TabIndex = 1;
        label2.Text = "Video Watch";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // upload
        // 
        upload.Location = new Point(541, 496);
        upload.Name = "upload";
        upload.Size = new Size(75, 23);
        upload.TabIndex = 9;
        upload.Text = "Upload";
        upload.UseVisualStyleBackColor = true;
        upload.Click += Upload;
        // 
        // axWindowsMediaPlayer1
        // 
        axWindowsMediaPlayer1.Enabled = true;
        axWindowsMediaPlayer1.Location = new Point(232, 70);
        axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
        axWindowsMediaPlayer1.OcxState = (AxHost.State)resources.GetObject("axWindowsMediaPlayer1.OcxState");
        axWindowsMediaPlayer1.Size = new Size(734, 405);
        axWindowsMediaPlayer1.TabIndex = 10;
        // 
        // WatchInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(1184, 661);
        Controls.Add(axWindowsMediaPlayer1);
        Controls.Add(upload);
        Controls.Add(label2);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        Name = "WatchInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        Load += FormLoad;
        ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label2;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private Button upload;
    private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
}
