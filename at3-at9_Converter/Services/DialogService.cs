using System;
using System.Windows.Forms;

namespace at3_at9_Converter.Services
{
    public class DialogService
    {
        private readonly LanguageService _languageService;

        public DialogService(LanguageService languageService)
        {
            _languageService = languageService;
        }

        public DialogResult ShowMessage(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }

        public string ShowInputBox(string prompt, string title, string defaultValue = "")
        {
            Form inputForm = new Form();
            inputForm.Text = title;
            inputForm.ClientSize = new System.Drawing.Size(300, 100);
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.StartPosition = FormStartPosition.CenterScreen;
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;

            Label label = new Label();
            label.Text = prompt;
            label.Location = new System.Drawing.Point(10, 10);
            label.Size = new System.Drawing.Size(280, 20);

            TextBox textBox = new TextBox();
            textBox.Text = defaultValue;
            textBox.Location = new System.Drawing.Point(10, 35);
            textBox.Size = new System.Drawing.Size(280, 20);

            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new System.Drawing.Point(10, 65);
            okButton.Size = new System.Drawing.Size(75, 25);

            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(100, 65);
            cancelButton.Size = new System.Drawing.Size(75, 25);

            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                return textBox.Text;
            }
            return null;
        }

        public DialogResult ShowOverwriteConfirmation(bool isSpanish, string fileName)
        {
            string message = _languageService.GetOverwriteConfirmation(isSpanish, fileName);
            return MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public string ShowRenameDialog(bool isSpanish, string originalName)
        {
            string prompt = _languageService.GetRenamePrompt(isSpanish, originalName);
            string title = isSpanish ? "Renombrar Archivo" : "Rename File";
            return ShowInputBox(prompt, title, originalName);
        }

        public DialogResult ShowFormatSelectionDialog(bool isSpanish, out string selectedFormat)
        {
            selectedFormat = null;
            Form formatForm = new Form();
            formatForm.Text = isSpanish ? "Seleccionar Formato" : "Select Format";
            formatForm.ClientSize = new System.Drawing.Size(250, 150);
            formatForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            formatForm.StartPosition = FormStartPosition.CenterScreen;

            Label label = new Label();
            label.Text = isSpanish ? "Selecciona el formato de salida:" : "Select output format:";
            label.Location = new System.Drawing.Point(10, 10);
            label.Size = new System.Drawing.Size(230, 20);

            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("WAV");
            comboBox.Items.Add("MP3");
            comboBox.SelectedIndex = 0;
            comboBox.Location = new System.Drawing.Point(10, 35);
            comboBox.Size = new System.Drawing.Size(230, 25);

            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new System.Drawing.Point(10, 80);
            okButton.Size = new System.Drawing.Size(75, 25);

            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(100, 80);
            cancelButton.Size = new System.Drawing.Size(75, 25);

            formatForm.Controls.Add(label);
            formatForm.Controls.Add(comboBox);
            formatForm.Controls.Add(okButton);
            formatForm.Controls.Add(cancelButton);

            formatForm.AcceptButton = okButton;
            formatForm.CancelButton = cancelButton;

            if (formatForm.ShowDialog() == DialogResult.OK)
            {
                selectedFormat = comboBox.SelectedItem.ToString();
                return DialogResult.OK;
            }
            return DialogResult.Cancel;
        }
    }
}
