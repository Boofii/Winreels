namespace Winreels;

partial class UserInterface
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
        l_username = new TextBox();
        l_password = new TextBox();
        r_password = new TextBox();
        r_username = new TextBox();
        r_email = new TextBox();
        sign_in = new Button();
        sign_up = new Button();
        label1 = new Label();
        label2 = new Label();
        SuspendLayout();
        // 
        // l_username
        // 
        l_username.BackColor = SystemColors.Window;
        l_username.Font = new Font("Aharoni", 9.75F);
        l_username.ForeColor = SystemColors.WindowText;
        l_username.Location = new Point(66, 77);
        l_username.Name = "l_username";
        l_username.PlaceholderText = "Username";
        l_username.Size = new Size(100, 20);
        l_username.TabIndex = 2;
        l_username.TextAlign = HorizontalAlignment.Center;
        // 
        // l_password
        // 
        l_password.BackColor = SystemColors.Window;
        l_password.Font = new Font("Aharoni", 9.75F);
        l_password.ForeColor = SystemColors.WindowText;
        l_password.Location = new Point(66, 114);
        l_password.Name = "l_password";
        l_password.PasswordChar = '*';
        l_password.PlaceholderText = "Password";
        l_password.Size = new Size(100, 20);
        l_password.TabIndex = 3;
        l_password.TextAlign = HorizontalAlignment.Center;
        // 
        // r_password
        // 
        r_password.BackColor = SystemColors.Window;
        r_password.Font = new Font("Aharoni", 9.75F);
        r_password.ForeColor = SystemColors.WindowText;
        r_password.Location = new Point(240, 151);
        r_password.Name = "r_password";
        r_password.PasswordChar = '*';
        r_password.PlaceholderText = "Password";
        r_password.Size = new Size(100, 20);
        r_password.TabIndex = 5;
        r_password.TextAlign = HorizontalAlignment.Center;
        // 
        // r_username
        // 
        r_username.BackColor = SystemColors.Window;
        r_username.Font = new Font("Aharoni", 9.75F);
        r_username.ForeColor = SystemColors.WindowText;
        r_username.Location = new Point(240, 114);
        r_username.Name = "r_username";
        r_username.PlaceholderText = "Username";
        r_username.Size = new Size(100, 20);
        r_username.TabIndex = 4;
        r_username.TextAlign = HorizontalAlignment.Center;
        // 
        // r_email
        // 
        r_email.BackColor = SystemColors.Window;
        r_email.Font = new Font("Aharoni", 9.75F);
        r_email.ForeColor = SystemColors.WindowText;
        r_email.Location = new Point(240, 77);
        r_email.Name = "r_email";
        r_email.PlaceholderText = "Email";
        r_email.Size = new Size(100, 20);
        r_email.TabIndex = 6;
        r_email.TextAlign = HorizontalAlignment.Center;
        // 
        // sign_in
        // 
        sign_in.BackgroundImageLayout = ImageLayout.None;
        sign_in.FlatAppearance.BorderColor = Color.Black;
        sign_in.FlatStyle = FlatStyle.Flat;
        sign_in.Image = Properties.Resources.join1;
        sign_in.Location = new Point(86, 196);
        sign_in.Name = "sign_in";
        sign_in.Size = new Size(56, 52);
        sign_in.TabIndex = 7;
        sign_in.UseVisualStyleBackColor = true;
        sign_in.Click += SignIn;
        // 
        // sign_up
        // 
        sign_up.BackgroundImageLayout = ImageLayout.None;
        sign_up.FlatAppearance.BorderColor = Color.Black;
        sign_up.FlatStyle = FlatStyle.Flat;
        sign_up.Image = Properties.Resources.add;
        sign_up.Location = new Point(265, 196);
        sign_up.Name = "sign_up";
        sign_up.Size = new Size(53, 52);
        sign_up.TabIndex = 8;
        sign_up.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Aharoni", 25F, FontStyle.Bold);
        label1.ForeColor = Color.Gold;
        label1.Location = new Point(64, 18);
        label1.Name = "label1";
        label1.Size = new Size(102, 34);
        label1.TabIndex = 9;
        label1.Text = "Login";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Aharoni", 25F, FontStyle.Bold);
        label2.ForeColor = Color.Gold;
        label2.Location = new Point(219, 18);
        label2.Name = "label2";
        label2.Size = new Size(144, 34);
        label2.TabIndex = 10;
        label2.Text = "Register";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // UserInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(403, 293);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(sign_up);
        Controls.Add(sign_in);
        Controls.Add(r_email);
        Controls.Add(r_password);
        Controls.Add(r_username);
        Controls.Add(l_password);
        Controls.Add(l_username);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        Name = "UserInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private TextBox l_username;
    private TextBox l_password;
    private TextBox r_password;
    private TextBox r_username;
    private TextBox r_email;
    private Button sign_in;
    private Button sign_up;
    private Label label1;
    private Label label2;
}
