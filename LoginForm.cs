using System;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;

namespace TurboMartPOS
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblUsername;
        private Label lblPassword;

        public LoginForm()
        {
            InitializeLoginControls();
        }

        private void InitializeLoginControls()
        {
            this.Text = "Login";
            this.Size = new System.Drawing.Size(300, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblUsername = new Label
            {
                Text = "Username:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new System.Drawing.Point(100, 20),
                Width = 150
            };

            lblPassword = new Label
            {
                Text = "Password:",
                Location = new System.Drawing.Point(20, 50),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(100, 50),
                Width = 150,
                PasswordChar = '*'
            };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new System.Drawing.Point(100, 90),
                Width = 100
            };
            btnLogin.Click += BtnLogin_Click;

            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string configUsername = ConfigurationManager.AppSettings["LoginUsername"];
            string configPassword = ConfigurationManager.AppSettings["LoginPassword"];
            string salesUsername = ConfigurationManager.AppSettings["SalesUsername"];
            string salesPassword = ConfigurationManager.AppSettings["SalesPassword"];

            if ((txtUsername.Text == configUsername && txtPassword.Text == configPassword) ||
                (txtUsername.Text == salesUsername && txtPassword.Text == salesPassword))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}