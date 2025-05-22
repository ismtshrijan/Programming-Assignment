using System;
using System.Drawing;
using System.Windows.Forms;

namespace TurboMartPOS
{
    public enum Theme
    {
        Light,
        Dark
    }

    public static class ThemeManager
    {
        public static void ApplyTheme(Form form, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                form.BackColor = Color.FromArgb(32, 32, 32);
                ApplyThemeToControls(form.Controls, theme);
            }
            else
            {
                form.BackColor = Color.WhiteSmoke;
                ApplyThemeToControls(form.Controls, theme);
            }
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls, Theme theme)
        {
            foreach (Control control in controls)
            {
                if (theme == Theme.Dark)
                {
                    if (control is Panel || control is GroupBox)
                        control.BackColor = Color.FromArgb(40, 40, 40);
                    else if (control is DataGridView dgv)
                    {
                        dgv.BackgroundColor = Color.FromArgb(40, 40, 40);
                        dgv.DefaultCellStyle.BackColor = Color.FromArgb(48, 48, 48);
                        dgv.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
                    }
                    else if (control is TextBox || control is RichTextBox)
                    {
                        control.BackColor = Color.FromArgb(48, 48, 48);
                        control.ForeColor = Color.WhiteSmoke;
                    }
                    else if (control is Button btn)
                    {
                        btn.BackColor = Color.FromArgb(56, 142, 60);
                        btn.ForeColor = Color.WhiteSmoke;
                    }
                    else if (control is Label lbl)
                    {
                        lbl.ForeColor = Color.WhiteSmoke;
                    }
                }
                else
                {
                    // Light theme
                    if (control is Panel || control is GroupBox)
                        control.BackColor = Color.WhiteSmoke;
                    else if (control is DataGridView dgv)
                    {
                        dgv.BackgroundColor = Color.White;
                        dgv.DefaultCellStyle.BackColor = Color.White;
                        dgv.DefaultCellStyle.ForeColor = Color.Black;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    }
                    else if (control is TextBox || control is RichTextBox)
                    {
                        control.BackColor = Color.White;
                        control.ForeColor = Color.Black;
                    }
                    else if (control is Button btn)
                    {
                        btn.BackColor = Color.FromArgb(33, 150, 243);
                        btn.ForeColor = Color.White;
                    }
                    else if (control is Label lbl)
                    {
                        lbl.ForeColor = Color.Black;
                    }
                }
                // Recursively apply to child controls
                if (control.HasChildren)
                    ApplyThemeToControls(control.Controls, theme);
            }
        }
    }
} 