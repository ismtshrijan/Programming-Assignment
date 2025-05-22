using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using TurboMartPOS;

namespace TurboMartPOS
{
    public partial class MainForm : Form
    {
        private CartManager cartManager;
        private decimal taxRate;
        private DataGridView dgvCart;
        private TextBox txtProductName;
        private TextBox txtPrice;
        private TextBox txtQuantity;
        private TextBox txtCash;
        private Button btnAddProduct;
        private Button btnCalculate;
        private Button btnPrint;
        private Button btnClear;
        private RichTextBox txtReceipt;
        private Label lblSubtotal;
        private Label lblTax;
        private Label lblTotal;
        private Label lblCash;
        private Label lblChange;
        private BindingSource cartBindingSource;
        private Theme currentTheme = Theme.Light;
        private Button btnExportTxt;
        private Button btnExportCsv;

        public MainForm()
        {
            try
            {
                cartManager = new CartManager();
                taxRate = GetConfiguredTaxRate();
                InitializeComponent();
                InitializeUI();
            }
            catch (Exception ex)
            {
                ErrorMessage.Show($"Error initializing form: {ex.Message}", "Initialization Error", ex.StackTrace);
            }
        }

        private decimal GetConfiguredTaxRate()
        {
            string rateStr = ConfigurationManager.AppSettings["TaxRate"];
            if (decimal.TryParse(rateStr, out decimal rate) && rate >= 0 && rate <= 1)
                return rate;
            return 0.13m; // fallback
        }

        private void InitializeUI()
        {
            try
            {
                // Modern color palette
                Color primaryColor = Color.FromArgb(33, 150, 243); // Blue
                Color accentColor = Color.FromArgb(76, 175, 80);   // Green
                Color dangerColor = Color.FromArgb(244, 67, 54);   // Red
                Color panelBg = Color.FromArgb(245, 245, 250);     // Light gray
                Color formBg = Color.WhiteSmoke;
                Color labelHighlight = Color.FromArgb(56, 142, 60); // Dark green
                Font mainFont = new Font("Segoe UI", 11F, FontStyle.Regular);
                Font boldFont = new Font("Segoe UI", 12F, FontStyle.Bold);

                // Form setup
                this.Text = "Turbo Mart POS System";
                this.Size = new Size(1200, 800);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = formBg;
                this.MinimumSize = new Size(1000, 600);

                // Create main layout panels
                var leftPanel = new Panel 
                { 
                    Name = "leftPanel",
                    Dock = DockStyle.Left, 
                    Width = 500, 
                    BackColor = panelBg, // Restore normal color
                    Padding = new Padding(16, 16, 8, 16)
                };
                var rightPanel = new Panel 
                { 
                    Name = "rightPanel",
                    Dock = DockStyle.Fill, 
                    BackColor = formBg, // Restore normal color
                    Padding = new Padding(8, 16, 16, 16)
                };

                // Initialize all controls first
                InitializeControls(primaryColor, accentColor, dangerColor, panelBg, formBg, labelHighlight, mainFont, boldFont);

                // Add theme toggle button
                var btnTheme = new Button
                {
                    Text = "Toggle Theme",
                    Width = 120,
                    Height = 32,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Location = new Point(16, 8)
                };
                btnTheme.Click += (s, e) =>
                {
                    currentTheme = currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
                    ThemeManager.ApplyTheme(this, currentTheme);
                };
                leftPanel.Controls.Add(btnTheme);

                // Create groups with specific heights
                var productGroup = CreateProductGroup(primaryColor, panelBg, boldFont, mainFont);
                productGroup.Name = "productGroup";
                productGroup.Height = 200;
                productGroup.Dock = DockStyle.Top;

                var cartGroup = CreateCartGroup(primaryColor, panelBg, boldFont);
                cartGroup.Name = "cartGroup";
                cartGroup.Dock = DockStyle.Fill;
                cartGroup.Height = 300;

                var paymentGroup = CreatePaymentGroup(primaryColor, accentColor, panelBg, boldFont, mainFont, labelHighlight);
                paymentGroup.Name = "paymentGroup";
                paymentGroup.Height = 220;
                paymentGroup.Dock = DockStyle.Bottom;

                var receiptGroup = CreateReceiptGroup(primaryColor, panelBg, boldFont);
                receiptGroup.Name = "receiptGroup";
                receiptGroup.Width = this.Width / 2; // 50% of the form width
                receiptGroup.Dock = DockStyle.Right;

                // Add controls to panels with proper order
                leftPanel.Controls.Clear();
                leftPanel.Controls.Add(productGroup);
                leftPanel.Controls.Add(cartGroup);
                leftPanel.Controls.Add(paymentGroup);
                rightPanel.Controls.Clear();
                rightPanel.Controls.Add(receiptGroup);
                this.Controls.Clear();
                this.Controls.Add(leftPanel);
                this.Controls.Add(rightPanel);

                // Debug information
                Debug.WriteLine("Form initialization complete");
                Debug.WriteLine($"Left panel size: {leftPanel.Size}, Visible: {leftPanel.Visible}");
                Debug.WriteLine($"Right panel size: {rightPanel.Size}, Visible: {rightPanel.Visible}");
                Debug.WriteLine($"Receipt textbox size: {txtReceipt.Size}, Visible: {txtReceipt.Visible}");

                // Force initial cart display
                UpdateCartDisplay();

                // Apply initial theme
                ThemeManager.ApplyTheme(this, currentTheme);
            }
            catch (Exception ex)
            {
                ErrorMessage.Show($"Error in InitializeUI: {ex.Message}", "UI Initialization Error", ex.StackTrace);
            }
        }

        private void InitializeControls(Color primaryColor, Color accentColor, Color dangerColor, 
            Color panelBg, Color formBg, Color labelHighlight, Font mainFont, Font boldFont)
        {
            // Initialize DataGridView
            dgvCart = new DataGridView
            {
                Name = "dgvCart",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = mainFont,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = primaryColor, 
                    ForeColor = Color.White, 
                    Font = boldFont,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                },
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = Color.White, 
                    ForeColor = Color.Black, 
                    Font = mainFont,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5),
                    SelectionBackColor = Color.FromArgb(230, 240, 255),
                    SelectionForeColor = Color.Black
                },
                RowTemplate = { Height = 40 },
                GridColor = Color.LightGray,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 248, 248)
                },
                Visible = true,
                AutoGenerateColumns = false,
                MinimumSize = new Size(350, 200)
            };

            // Add columns
            dgvCart.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "ProductName", 
                    HeaderText = "Product Name", 
                    Width = 160,
                    DataPropertyName = "ProductName",
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "UnitPrice", 
                    HeaderText = "Unit Price", 
                    Width = 90,
                    DataPropertyName = "UnitPrice",
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Quantity", 
                    HeaderText = "Quantity", 
                    Width = 80,
                    DataPropertyName = "Quantity",
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Total", 
                    HeaderText = "Total", 
                    Width = 90,
                    DataPropertyName = "Total",
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                }
            });

            // Set up data binding
            cartBindingSource = new BindingSource();
            dgvCart.DataSource = cartBindingSource;

            // Initialize other controls
            txtProductName = new TextBox { Location = new Point(140, 27), Width = 200, Font = mainFont, BorderStyle = BorderStyle.FixedSingle };
            txtPrice = new TextBox { Location = new Point(140, 67), Width = 200, Font = mainFont, BorderStyle = BorderStyle.FixedSingle };
            txtQuantity = new TextBox { Location = new Point(140, 107), Width = 200, Font = mainFont, BorderStyle = BorderStyle.FixedSingle };
            txtCash = new TextBox { Location = new Point(140, 132), Width = 200, Font = mainFont, BorderStyle = BorderStyle.FixedSingle };
            txtReceipt = new RichTextBox
            {
                Name = "txtReceipt",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 11F, FontStyle.Regular),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = true,
                MinimumSize = new Size(300, 400)
            };

            // Initialize labels
            lblSubtotal = new Label 
            { 
                Text = "Subtotal:", 
                Location = new Point(10, 30), 
                Font = mainFont,
                AutoSize = true
            };
            lblTax = new Label 
            { 
                Text = "Tax (13%):", 
                Location = new Point(10, 65), 
                Font = mainFont,
                AutoSize = true
            };
            lblTotal = new Label 
            { 
                Text = "Total:", 
                Location = new Point(10, 100), 
                Font = boldFont, 
                ForeColor = labelHighlight,
                AutoSize = true
            };
            lblCash = new Label 
            { 
                Text = "Cash Given:", 
                Location = new Point(10, 135), 
                Font = mainFont,
                AutoSize = true
            };
            lblChange = new Label 
            { 
                Text = "Change:", 
                Location = new Point(10, 170), 
                Font = boldFont, 
                ForeColor = accentColor,
                AutoSize = true
            };

            // Initialize buttons
            btnAddProduct = CreateButton("Add to Cart", new Point(140, 147), 200, 36, primaryColor, boldFont);
            btnCalculate = CreateButton("Calculate Change", new Point(140, 170), 200, 36, accentColor, boldFont);
            btnPrint = CreateButton("Print Receipt", DockStyle.Bottom, 40, primaryColor, boldFont);
            btnPrint.Enabled = false;
            btnPrint.Visible = true;
            btnClear = CreateButton("Clear Cart", DockStyle.Bottom, 40, dangerColor, boldFont);
            btnClear.Visible = true;

            // Add event handlers
            btnAddProduct.Click += BtnAddProduct_Click;
            btnCalculate.Click += BtnCalculate_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
        }

        private Button CreateButton(string text, Point location, int width, int height, Color color, Font font)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Width = width,
                Height = height,
                Font = font,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(color.R - 20, color.G - 20, color.B - 20);
            button.MouseLeave += (s, e) => button.BackColor = color;
            return button;
        }

        private Button CreateButton(string text, DockStyle dock, int height, Color color, Font font)
        {
            var button = new Button
            {
                Text = text,
                Dock = dock,
                Height = height,
                Font = font,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(color.R - 20, color.G - 20, color.B - 20);
            button.MouseLeave += (s, e) => button.BackColor = color;
            return button;
        }

        private GroupBox CreateProductGroup(Color primaryColor, Color panelBg, Font boldFont, Font mainFont)
        {
            var group = new GroupBox
            {
                Text = "Product Entry",
                Dock = DockStyle.Top,
                Height = 200,
                Font = boldFont,
                BackColor = panelBg,
                ForeColor = primaryColor,
                Padding = new Padding(12)
            };

            var lblProductName = new Label { Text = "Product Name:", Location = new Point(10, 30), Font = mainFont };
            var lblPrice = new Label { Text = "Price ($):", Location = new Point(10, 70), Font = mainFont };
            var lblQuantity = new Label { Text = "Quantity:", Location = new Point(10, 110), Font = mainFont };

            group.Controls.AddRange(new Control[] { lblProductName, txtProductName, lblPrice, txtPrice, lblQuantity, txtQuantity, btnAddProduct });
            return group;
        }

        private GroupBox CreateCartGroup(Color primaryColor, Color panelBg, Font boldFont)
        {
            var group = new GroupBox
            {
                Name = "cartGroup",
                Text = "Shopping Cart",
                Dock = DockStyle.Fill,
                Font = boldFont,
                BackColor = panelBg,
                ForeColor = primaryColor,
                Padding = new Padding(12),
                MinimumSize = new Size(350, 200),
                Visible = true
            };

            // Ensure DataGridView is properly added to the group
            if (dgvCart != null)
            {
                dgvCart.Name = "dgvCart";
                dgvCart.Dock = DockStyle.Fill;
                dgvCart.Visible = true;
                dgvCart.Height = group.Height - 40; // Account for group header
                group.Controls.Add(dgvCart);
                Debug.WriteLine($"DataGridView added to {group.Name}");
            }

            return group;
        }

        private GroupBox CreatePaymentGroup(Color primaryColor, Color accentColor, Color panelBg, Font boldFont, Font mainFont, Color labelHighlight)
        {
            var group = new GroupBox
            {
                Text = "Payment",
                Dock = DockStyle.Bottom,
                Height = 220,
                Font = boldFont,
                BackColor = panelBg,
                ForeColor = primaryColor,
                Padding = new Padding(12)
            };

            group.Controls.AddRange(new Control[] { lblSubtotal, lblTax, lblTotal, lblCash, txtCash, lblChange, btnCalculate });
            return group;
        }

        private GroupBox CreateReceiptGroup(Color primaryColor, Color panelBg, Font boldFont)
        {
            var group = new GroupBox
            {
                Text = "Receipt",
                Dock = DockStyle.Fill,
                Font = boldFont,
                BackColor = panelBg,
                ForeColor = primaryColor,
                Padding = new Padding(12),
                MinimumSize = new Size(300, 400),
                Visible = true
            };

            // Create a panel to hold the receipt and buttons
            var receiptPanel = new Panel
            {
                Name = "receiptPanel",
                Dock = DockStyle.Fill,
                Padding = new Padding(8),
                BackColor = Color.White,
                Visible = true
            };

            // Export buttons
            btnExportTxt = new Button
            {
                Text = "Export TXT",
                Dock = DockStyle.Bottom,
                Height = 36,
                Font = boldFont,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Visible = true,
                Enabled = false
            };
            btnExportTxt.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtReceipt.Text))
                    ReceiptExporter.SaveReceiptAsTxt(txtReceipt.Text);
            };

            btnExportCsv = new Button
            {
                Text = "Export CSV",
                Dock = DockStyle.Bottom,
                Height = 36,
                Font = boldFont,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Visible = true,
                Enabled = false
            };
            btnExportCsv.Click += (s, e) =>
            {
                if (cartManager.Items.Count > 0)
                    ReceiptExporter.SaveReceiptAsCsv(cartManager.Items);
            };

            // Add buttons first, then txtReceipt last so it is not covered
            if (btnPrint != null)
            {
                btnPrint.Dock = DockStyle.Bottom;
                btnPrint.Visible = true;
                receiptPanel.Controls.Add(btnPrint);
            }
            if (btnClear != null)
            {
                btnClear.Dock = DockStyle.Bottom;
                btnClear.Visible = true;
                receiptPanel.Controls.Add(btnClear);
            }
            receiptPanel.Controls.Add(btnExportTxt);
            receiptPanel.Controls.Add(btnExportCsv);
            if (txtReceipt != null)
            {
                txtReceipt.Dock = DockStyle.Fill;
                txtReceipt.Visible = true;
                receiptPanel.Controls.Add(txtReceipt);
            }

            group.Controls.Add(receiptPanel);

            // Debug information
            Debug.WriteLine($"Receipt group created - Size: {group.Size}, Visible: {group.Visible}");
            Debug.WriteLine($"Receipt panel - Size: {receiptPanel.Size}, Visible: {receiptPanel.Visible}");
            Debug.WriteLine($"Receipt textbox - Size: {txtReceipt?.Size}, Visible: {txtReceipt?.Visible}");
            Debug.WriteLine($"Print button - Size: {btnPrint?.Size}, Visible: {btnPrint?.Visible}");
            Debug.WriteLine($"Clear button - Size: {btnClear?.Size}, Visible: {btnClear?.Visible}");

            return group;
        }

        private void UpdateTotalsDisplay()
        {
            decimal subtotal = cartManager.GetSubtotal();
            decimal tax = cartManager.GetTax(taxRate);
            decimal total = cartManager.GetTotal(taxRate);
            lblSubtotal.Text = $"Subtotal: {subtotal:C2}";
            lblTax.Text = $"Tax ({taxRate:P0}): {tax:C2}";
            lblTotal.Text = $"Total: {total:C2}";
        }

        private void UpdateCartDisplay()
        {
            try
            {
                if (dgvCart == null)
                {
                    ErrorMessage.Show("DataGridView is not initialized!", "Error");
                    return;
                }

                // Use data binding
                var items = cartManager.Items.Select(item => new
                {
                    ProductName = item.Product.Name,
                    UnitPrice = item.Product.Price.ToString("C2"),
                    Quantity = item.Quantity,
                    Total = item.GetSubtotal().ToString("C2")
                }).ToList();
                cartBindingSource.DataSource = items;
                dgvCart.Refresh();
                dgvCart.Visible = true;
                dgvCart.BringToFront();

                // Ensure the cart group is visible
                if (dgvCart.Parent is GroupBox cartGroup)
                {
                    cartGroup.Visible = true;
                    cartGroup.BringToFront();
                    Debug.WriteLine($"Cart group visibility: {cartGroup.Visible}, Size: {cartGroup.Size}");
                }

                // Update totals display automatically
                UpdateTotalsDisplay();
            }
            catch (Exception ex)
            {
                ErrorMessage.Show($"Error updating cart display: {ex.Message}", "Error", ex.StackTrace);
            }
        }

        private void ClearProductInputs()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            txtProductName.Focus();
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtCash.Text, out decimal cashGiven) || cashGiven <= 0)
                {
                    ErrorMessage.Show("Please enter a valid cash amount.", "Validation Error", null, MessageBoxIcon.Warning);
                    return;
                }

                decimal subtotal = cartManager.GetSubtotal();
                decimal tax = cartManager.GetTax(taxRate);
                decimal total = cartManager.GetTotal(taxRate);

                if (cashGiven < total)
                {
                    ErrorMessage.Show("Insufficient cash provided.", "Error");
                    return;
                }

                decimal change = cashGiven - total;

                // Update labels with proper formatting
                lblSubtotal.Text = $"Subtotal: {subtotal:C2}";
                lblTax.Text = $"Tax ({taxRate:P0}): {tax:C2}";
                lblTotal.Text = $"Total: {total:C2}";
                lblChange.Text = $"Change: {change:C2}";

                // Enable print button
                btnPrint.Enabled = true;

                // Debug information
                Debug.WriteLine($"Payment calculated - Subtotal: {subtotal:C2}, Tax: {tax:C2}, Total: {total:C2}");
                Debug.WriteLine($"Cash given: {cashGiven:C2}, Change: {change:C2}");
            }
            catch (Exception ex)
            {
                ErrorMessage.Show($"Error calculating total: {ex.Message}", "Error");
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtReceipt == null)
                {
                    ErrorMessage.Show("Receipt control is not initialized!", "Error");
                    return;
                }

                // Clear existing content
                txtReceipt.Clear();

                // Use ReceiptGenerator to generate the receipt text
                if (!decimal.TryParse(txtCash.Text, out decimal cashGiven))
                {
                    ErrorMessage.Show("Invalid cash amount for receipt.", "Error");
                    return;
                }
                string receiptText = ReceiptGenerator.GenerateReceipt(cartManager.Items, taxRate, cashGiven);
                txtReceipt.Text = receiptText;
                txtReceipt.SelectionStart = 0;
                txtReceipt.SelectionLength = 0;
                txtReceipt.ScrollToCaret();
                txtReceipt.Refresh();
                txtReceipt.Visible = true;
                txtReceipt.BringToFront();

                // Enable export buttons
                btnExportTxt.Enabled = !string.IsNullOrWhiteSpace(txtReceipt.Text);
                btnExportCsv.Enabled = cartManager.Items.Count > 0;

                // Ensure parent controls are visible
                if (txtReceipt.Parent != null)
                {
                    txtReceipt.Parent.Visible = true;
                    txtReceipt.Parent.BringToFront();
                }

                // Debug information after generating receipt
                Debug.WriteLine($"After generating receipt - Text length: {txtReceipt.TextLength}");
                Debug.WriteLine($"Textbox visible: {txtReceipt.Visible}");
                Debug.WriteLine($"Textbox parent: {txtReceipt.Parent?.Name ?? "No Parent"}");
                Debug.WriteLine($"Textbox size: {txtReceipt.Size}");
                Debug.WriteLine($"Receipt content:\n{txtReceipt.Text}");
            }
            catch (Exception ex)
            {
                ErrorMessage.Show($"Error generating receipt: {ex.Message}", "Error");
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateProductInputs())
                    return;

                var product = new Product(
                    txtProductName.Text.Trim(),
                    decimal.Parse(txtPrice.Text),
                    int.Parse(txtQuantity.Text)
                );

                cartManager.AddItem(product, product.Quantity);

                // Force immediate update
                UpdateCartDisplay();
                UpdateTotalsDisplay();
                ClearProductInputs();
                btnCalculate.Enabled = true;

                // Debug information
                Debug.WriteLine($"Added to cart: {product.Name}, Quantity: {product.Quantity}, Price: {product.Price:C2}");
                Debug.WriteLine($"Total items in cart: {cartManager.Items.Count}");
            }
            catch (Exception ex)
            {
                ErrorMessage.Show($"Error adding product: {ex.Message}", "Error", ex.StackTrace);
            }
        }

        private bool ValidateProductInputs()
        {
            if (!ValidationHelper.ValidateProductName(txtProductName.Text, out string nameError))
            {
                ErrorMessage.Show(nameError, "Validation Error", null, MessageBoxIcon.Warning);
                return false;
            }
            if (!ValidationHelper.ValidatePrice(txtPrice.Text, out decimal price, out string priceError))
            {
                ErrorMessage.Show(priceError, "Validation Error", null, MessageBoxIcon.Warning);
                return false;
            }
            if (!ValidationHelper.ValidateQuantity(txtQuantity.Text, out int quantity, out string quantityError))
            {
                ErrorMessage.Show(quantityError, "Validation Error", null, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            cartManager.Clear();
            UpdateCartDisplay();
            UpdateTotalsDisplay();
            ClearProductInputs();
            txtCash.Clear();
            txtReceipt.Clear();
            lblSubtotal.Text = "Subtotal:";
            lblTax.Text = "Tax (13%):";
            lblTotal.Text = "Total:";
            lblChange.Text = "Change:";
            btnCalculate.Enabled = false;
            btnPrint.Enabled = false;
            if (btnExportTxt != null) btnExportTxt.Enabled = false;
            if (btnExportCsv != null) btnExportCsv.Enabled = false;
        }
    }
} 