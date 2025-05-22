using System.Windows.Forms;

namespace TurboMartPOS
{
    public class ErrorMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.Error;

        public ErrorMessage(string message, string title = "Error", string details = null, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            Message = message;
            Title = title;
            Details = details;
            Icon = icon;
        }

        public void Show()
        {
            string fullMessage = Message;
            if (!string.IsNullOrWhiteSpace(Details))
            {
                fullMessage += "\n\nDetails: " + Details;
            }
            MessageBox.Show(fullMessage, Title, MessageBoxButtons.OK, Icon);
        }

        public static void Show(string message, string title = "Error", string details = null, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            new ErrorMessage(message, title, details, icon).Show();
        }
    }
} 