namespace Winreels;

partial class LoginInterface
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
        label1 = new Label();
        username = new TextBox();
        password = new TextBox();
        sign_in = new Button();
        colorDialog1 = new ColorDialog();
        new_user = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
        label1.ForeColor = Color.Gold;
        label1.Location = new Point(500, 10);
        label1.Name = "label1";
        label1.Size = new Size(160, 46);
        label1.TabIndex = 0;
        label1.Text = "Winreels";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // username
        // 
        username.Location = new Point(528, 114);
        username.Name = "username";
        username.PlaceholderText = "Username";
        username.Size = new Size(100, 25);
        username.TabIndex = 2;
        // 
        // password
        // 
        password.Location = new Point(528, 163);
        password.Name = "password";
        password.PlaceholderText = "Password";
        password.Size = new Size(100, 25);
        password.TabIndex = 3;
        // 
        // sign_in
        // 
        sign_in.Location = new Point(500, 223);
        sign_in.Name = "sign_in";
        sign_in.Size = new Size(75, 23);
        sign_in.TabIndex = 4;
        sign_in.Text = "Sign In";
        sign_in.UseVisualStyleBackColor = true;
        sign_in.Click += SignIn;
        // 
        // new_user
        // 
        new_user.Location = new Point(585, 223);
        new_user.Name = "new_user";
        new_user.Size = new Size(75, 23);
        new_user.TabIndex = 5;
        new_user.Text = "Sign Up";
        new_user.UseVisualStyleBackColor = true;
        new_user.Click += SignUp;
        // 
        // LoginInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(1184, 661);
        Controls.Add(new_user);
        Controls.Add(sign_in);
        Controls.Add(password);
        Controls.Add(username);
        Controls.Add(label1);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        Name = "LoginInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox username;
    private TextBox password;
    private Button sign_in;
    private ColorDialog colorDialog1;
    private Button new_user;
}
