namespace Winreels;

partial class RegisterInterface
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
        email = new TextBox();
        username = new TextBox();
        password = new TextBox();
        sign_up = new Button();
        has_user = new Button();
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
        // email
        // 
        email.Location = new Point(527, 115);
        email.Name = "email";
        email.PlaceholderText = "Email";
        email.Size = new Size(100, 25);
        email.TabIndex = 2;
        // 
        // username
        // 
        username.Location = new Point(527, 156);
        username.Name = "username";
        username.PlaceholderText = "Username";
        username.Size = new Size(100, 25);
        username.TabIndex = 3;
        // 
        // password
        // 
        password.Location = new Point(527, 200);
        password.Name = "password";
        password.PlaceholderText = "Password";
        password.Size = new Size(100, 25);
        password.TabIndex = 4;
        // 
        // sign_up
        // 
        sign_up.Location = new Point(500, 254);
        sign_up.Name = "sign_up";
        sign_up.Size = new Size(75, 23);
        sign_up.TabIndex = 5;
        sign_up.Text = "Sign Up";
        sign_up.UseVisualStyleBackColor = true;
        sign_up.Click += SignUp;
        // 
        // has_user
        // 
        has_user.Location = new Point(585, 254);
        has_user.Name = "has_user";
        has_user.Size = new Size(75, 23);
        has_user.TabIndex = 6;
        has_user.Text = "Sign In";
        has_user.UseVisualStyleBackColor = true;
        has_user.Click += SignIn;
        // 
        // RegisterInterface
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(1184, 661);
        Controls.Add(has_user);
        Controls.Add(sign_up);
        Controls.Add(password);
        Controls.Add(username);
        Controls.Add(email);
        Controls.Add(label1);
        Font = new Font("Segoe UI", 9.75F);
        ForeColor = Color.Black;
        Name = "RegisterInterface";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Winreels";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox email;
    private TextBox username;
    private TextBox password;
    private Button sign_up;
    private Button has_user;
}
