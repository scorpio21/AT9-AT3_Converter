using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio;
using System.IO;
using System.Diagnostics;
using at3_at9_Converter.Services;

namespace at3_at9_Converter
{

    public partial class MainForm : Form
    {
        #region Fields and Properties

        private bool isSpanish = true;
        public static string dir = "", appdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), version = "";

        Process playerProcess = new Process();
        ProcessStartInfo playerStartInfo = new ProcessStartInfo();

        private string selectedFilePath = "";
        private string finalFileName = "";
        private string finalFileName2 = "";
        private string fileExtension = "";
        private string filePath = "";
        private string sourceDirectory = "";
        private string newFileName = "";
        private string originalFileName = "";
        private string at3ToolPath = "";
        private string at9ToolPath = "";
        private string selectedAt3Bitrate = "";
        private string selectedAt9Bitrate = "";

        private readonly BitrateConfig _bitrateConfig;
        private readonly LanguageService _languageService;
        private readonly DialogService _dialogService;
        private readonly FileManager _fileManager;
        private readonly AudioConverterService _audioConverterService;

        #endregion

        #region Language Support

        private void UpdateLanguage()
        {
            if (isSpanish)
            {
                this.tabPage1.Text = "Conversión AT9";
                this.tabPage2.Text = "Conversión AT3";
                this.label1.Text = "Arrastra y suelta tu archivo aquí";
                this.label2.Text = "Arrastra y suelta tu archivo aquí";
                this.button2.Text = "Convertir";
                this.button4.Text = "Convertir";
                this.button1.Text = "Detener Reproducción";
                this.button3.Text = "Detener Reproducción";
                this.groupBox1.Text = "Tipo de Conversión";
                this.groupBox2.Text = "Tipo de Conversión";
                this.label3.Text = "Bitrate:";
                this.label4.Text = "Tipo Consola:";
                this.label5.Text = "Bitrate:";
                this.label6.Text = "Tipo Consola:";
                this.toolTip1.SetToolTip(this.label6, "Elige el tipo de consola");
                this.toolTip1.SetToolTip(this.label5 , "Elige el tipo de bitrate");
                this.toolTip1.SetToolTip(this.button2, "Iniciar conversión");
                this.toolTip1.SetToolTip(this.button3, "Detener Reproducción");
                this.toolTip1.SetToolTip(this.button4, "Iniciar conversión");
                this.toolTip1.SetToolTip(this.button1, "Detener Reproducción");
                this.toolTip1.SetToolTip(this.groupBox1, "Elige el tipo de conversión");
                this.toolTip1.SetToolTip(this.groupBox2, "Elige el tipo de conversión");
                chkLanguage.Text = "Idioma: Español";
            }
            else
            {
                this.tabPage1.Text = "AT9Tool PSVita/TV & P4";
                this.tabPage2.Text = "AT3Tool PSP & PS3";
                this.label1.Text = "Drag and drop your file here";
                this.label2.Text = "Drag and drop your file here";
                this.button2.Text = "Convert";
                this.button4.Text = "Convert";
                this.button1.Text = "Stop Playing";
                this.button3.Text = "Stop Playing";
                this.groupBox1.Text = "Select Type Convertion";
                this.groupBox2.Text = "Select Type Convertion";
                this.label3.Text = "BitRate [kbps]:";
                this.label4.Text = "Console Type:";
                this.label5.Text = "BitRate [kbps]:";
                this.label6.Text = "Console Type:";
                chkLanguage.Text = "Language: English";
            }
        }

        private void chkLanguage_CheckedChanged(object sender, EventArgs e)
        {
            isSpanish = !chkLanguage.Checked;
            UpdateLanguage();
        }

        private string GetMsg(string es, string en)
        {
            return isSpanish ? es : en;
        }

        #endregion

        #region Constructor and Initialization

        public MainForm()
        {
            InitializeComponent();

            _bitrateConfig = new BitrateConfig();
            _languageService = new LanguageService();
            _fileManager = new FileManager();
            _dialogService = new DialogService(_languageService);
            _audioConverterService = new AudioConverterService(_fileManager, _bitrateConfig);

            _audioConverterService.StatusUpdate = (status) =>
            {
                toolStripStatusLabel1.Text = status;
                statusStrip1.Refresh();
            };

            _audioConverterService.LogError = (error) =>
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conversion_errors.log"),
                    "\r\n" + error);
            };

            this.tabPage1.AllowDrop = true;
            this.tabPage1.DragEnter += new DragEventHandler(tabPage1_DragEnter);
            this.tabPage1.DragDrop += new DragEventHandler(tabPage1_DragDrop);
            this.tabPage2.AllowDrop = true;
            this.tabPage2.DragEnter += new DragEventHandler(tabPage2_DragEnter);
            this.tabPage2.DragDrop += new DragEventHandler(tabPage2_DragDrop);
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
        }

        #endregion

        #region Drag and Drop Event Handlers

        void tabPage1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void tabPage1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            originalFileName = "";
            sourceDirectory = "";
            textBox1.Text = "";
            fileExtension = "";
            filePath = "";
            foreach (string file in files) {

                originalFileName += Path.GetFileName(file);
                sourceDirectory = Path.GetDirectoryName(file);
                textBox1.Text = file;

            }

                fileExtension = textBox1.Text.Substring(textBox1.Text.LastIndexOf((".")));
                filePath = textBox1.Text;
                VerifExtention_at9();

        }

        void tabPage2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void tabPage2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            originalFileName = "";
            sourceDirectory = "";
            textBox2.Text = "";
            fileExtension = "";
            filePath = "";
            foreach (string file in files)
            {
                originalFileName += Path.GetFileName(file);
                sourceDirectory = Path.GetDirectoryName(file);
                textBox2.Text = file;

            }
                fileExtension = textBox2.Text.Substring(textBox2.Text.LastIndexOf((".")));
                filePath = textBox2.Text;
                VerifExtention_at3();
        }

        #endregion

        #region File Helper Methods

        private static String sReplace(String ffilePath)
        {
           
            ffilePath = ffilePath.Replace(" ", "-");
            return ffilePath;

        }

        private void rRename()
        {
            newFileName = "";
            if (tabControl1.SelectedIndex == 0)
            {
            newFileName = sReplace(originalFileName);
            System.IO.File.Move(dir + "\\" + originalFileName, dir + "\\" + newFileName);
                filePath = dir + "\\" + newFileName;
                originalFileName = newFileName;
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                newFileName = sReplace(originalFileName);
                System.IO.File.Move(textBox2.Text, sourceDirectory + "\\" + newFileName);
                filePath = textBox2.Text;
                originalFileName = newFileName;
            }

        }

        #endregion

        #region File Verification Methods

        private void VerifExtention_at9()
        {
            if (fileExtension.Equals(".wav") || fileExtension.Equals(".WAV"))
            {
                selectedFilePath = textBox1.Text;
                finalFileName = Path.ChangeExtension(selectedFilePath, ".at9");
                
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton1.Enabled = true;
                radioButton1.Checked = true;
                tabPage1ConsoleCombo();
                comboBox3.Items.Clear();
                comboBox4.Enabled = true;
            }
            else if (fileExtension.Equals(".at9") || fileExtension.Equals(".AT9"))
            {
                selectedFilePath = textBox1.Text;
                finalFileName2 = Path.ChangeExtension(selectedFilePath, ".wav");
                finalFileName = Path.ChangeExtension(selectedFilePath, ".mp3");
                InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
            }
            else if (fileExtension.Equals(".mp3") || fileExtension.Equals(".MP3"))
            {
                selectedFilePath = textBox1.Text;
                finalFileName2 = Path.ChangeExtension(selectedFilePath, ".wav");
                finalFileName = Path.ChangeExtension(selectedFilePath, ".at9");
                
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton4.Enabled = false;
                radioButton3.Enabled = true;
                radioButton3.Checked = true;
                tabPage1ConsoleCombo();
                comboBox3.Items.Clear();
                comboBox4.Enabled = true;
            }
            else
            {
                mMessageBox("Informations", "Please select MP3 Or Wav Or at9 File", SystemIcons.Information, true);
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                button2.Enabled = false;                      
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                textBox1.Text = "";
            }
        }

        private void VerifExtention_at3()
        {
            if (fileExtension.Equals(".wav") || fileExtension.Equals(".WAV"))
            {
                selectedFilePath = textBox2.Text;
                finalFileName = Path.ChangeExtension(selectedFilePath, ".at3");
                
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                radioButton5.Enabled = true;
                radioButton5.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                comboBox1.Enabled = true;
            }
            else if (fileExtension.Equals(".at3") || fileExtension.Equals(".AT3"))
            {
                selectedFilePath = textBox2.Text;
                finalFileName2 = Path.ChangeExtension(selectedFilePath, ".wav");
                finalFileName = Path.ChangeExtension(selectedFilePath, ".mp3");
                InputBox("Questions", "what format do you want to convert it ?", SystemIcons.Question, true);
            }
            else if (fileExtension.Equals(".mp3") || fileExtension.Equals(".MP3"))
            {
                selectedFilePath = textBox2.Text;
                finalFileName2 = Path.ChangeExtension(selectedFilePath, ".wav");
                finalFileName = Path.ChangeExtension(selectedFilePath, ".at3");
                
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton8.Enabled = false;
                radioButton7.Enabled = true;
                radioButton7.Checked = true;
                tabPage2ConsoleCombo();
                comboBox2.Items.Clear();
                comboBox1.Enabled = true;
            }
            else
            {
                mMessageBox("Informations", "Please select MP3 Or Wav Or at3 File", SystemIcons.Information, true);
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                button4.Enabled = false;
                comboBox1.Enabled = false;               
                comboBox2.Enabled = false;                
                textBox2.Text = "";
            }
        }

        #endregion

        #region Button Click Event Handlers

        private void button2_Click(object sender, EventArgs e)
        {
  
            if (radioButton3.Checked == true || radioButton4.Checked == true)
            {
            if (System.IO.File.Exists(finalFileName) || System.IO.File.Exists(finalFileName2))
            {
                at9DoProcessFileExist();
            }
            else
            {
                at9DoProcess();
            }
            }
            else if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                if (System.IO.File.Exists(finalFileName))
                {
                    at9DoProcessFileExist();
                }
                else
                {
                    at9DoProcess();
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (radioButton7.Checked == true || radioButton8.Checked == true)
            {
                if (System.IO.File.Exists(finalFileName) || System.IO.File.Exists(finalFileName2))
                {
                    at3DoProcessFileExist();
                }
                else
                {
                    at3DoProcess();
                }
            }
            else if (radioButton5.Checked == true || radioButton6.Checked == true)
            {
                if (System.IO.File.Exists(finalFileName))
                {
                    at3DoProcessFileExist();
                }
                else
                {
                    at3DoProcess();
                }
            }
        }

        #endregion

        #region Conversion Methods

        private void at9DoProcessFileExist()
        {

            mInputBox("Question", "File(s) Exist Do you want to Continue ?", SystemIcons.Question, true, 1); 

        }

        private void at3DoProcessFileExist()
        {

            mInputBox("Question", "File(s) Exist Do you want to Continue ?", SystemIcons.Question, true, 2);

        }

        private void at9DoProcess()
        {
            try
            {
                if (radioButton1.Checked == true && textBox1.Text != "")
                {
                    _audioConverterService.ConvertWavToAt9(selectedFilePath, finalFileName, at9ToolPath, selectedAt9Bitrate);
                }
                else if (radioButton2.Checked == true && textBox1.Text != "")
                {
                    _audioConverterService.ConvertAt9ToWav(selectedFilePath, finalFileName2, at9ToolPath);
                }
                else if (radioButton3.Checked == true && textBox1.Text != "")
                {
                    _audioConverterService.ConvertMp3ToAt9(selectedFilePath, finalFileName, finalFileName2, at9ToolPath, selectedAt9Bitrate);
                    _fileManager.DeleteFile(finalFileName2);
                }
                else if (radioButton4.Checked == true && textBox1.Text != "")
                {
                    _audioConverterService.ConvertAt9ToMp3(selectedFilePath, finalFileName2, finalFileName, at9ToolPath);
                    _fileManager.DeleteFile(finalFileName2);
                }
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void at3DoProcess()
        {
            try
            {
                if (radioButton5.Checked == true && textBox2.Text != "")
                {
                    _audioConverterService.ConvertWavToAt3(selectedFilePath, finalFileName, at3ToolPath, selectedAt3Bitrate, comboBox1.Text);
                    PlayerFile();
                }
                else if (radioButton6.Checked == true && textBox2.Text != "")
                {
                    _audioConverterService.ConvertAt3ToWav(selectedFilePath, finalFileName2, at3ToolPath);
                }
                else if (radioButton7.Checked == true && textBox2.Text != "")
                {
                    _audioConverterService.ConvertMp3ToAt3(selectedFilePath, finalFileName, finalFileName2, at3ToolPath, selectedAt3Bitrate, comboBox1.Text);
                    _fileManager.DeleteFile(finalFileName2);
                    PlayerFile();
                }
                else if (radioButton8.Checked == true && textBox2.Text != "")
                {
                    _audioConverterService.ConvertAt3ToMp3(selectedFilePath, finalFileName2, finalFileName, at3ToolPath);
                    _fileManager.DeleteFile(finalFileName2);
                }
                toolStripStatusLabel1.Text = "Finish!";
                statusStrip1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region File Operations

        private void mMoveFile()
        {
            try
            {
                string fileName = Path.GetFileName(finalFileName);
                string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                string destPath = finalFileName;

                if (_fileManager.FileExists(sourcePath) && sourcePath.ToLower() != destPath.ToLower())
                {
                    _fileManager.MoveFile(sourcePath, destPath);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conversion_errors.log"),
                    "\r\nError en mMoveFile: " + ex.Message);
            }
        }

        private void mMoveFile2()
        {
            try {
                mMoveFile();
                if (!string.IsNullOrEmpty(finalFileName2)) {
                    string fileName2 = Path.GetFileName(finalFileName2);
                    string sourcePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName2);
                    string destPath2 = finalFileName2;
                    if (_fileManager.FileExists(sourcePath2) && sourcePath2.ToLower() != destPath2.ToLower()) {
                        _fileManager.MoveFile(sourcePath2, destPath2);
                    }
                }
            } catch { }
        }

        #endregion

        #region Dialog Methods

        private void DeleteFile()
        {
            mInputBox("Question", "Do you want to delete WAV file ?", SystemIcons.Question, true, 3); 
        }

        private void PlayerFile()
        {
            mInputBox("Question", "Do you want to play file ?", SystemIcons.Question, true, 7);
        }

        #endregion

        #region Tab Control Event Handlers

        private void tabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {

            if (textBox1.Text != "" || textBox2.Text != "" || radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true || radioButton4.Checked == true || radioButton5.Checked == true || radioButton6.Checked == true || radioButton7.Checked == true || radioButton8.Checked == true)
            { 
                
                textBox1.Text = ""; 
                textBox2.Text = ""; 
                button2.Enabled = false;
                button4.Enabled = false; 
                radioButton1.Checked = false; 
                radioButton2.Checked = false; 
                radioButton3.Checked = false; 
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton5.Enabled = false;
                radioButton6.Enabled = false;
                radioButton7.Enabled = false;
                radioButton8.Enabled = false;
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                toolStripStatusLabel1.Text = GetMsg("¡Listo!", "Ready!");
                statusStrip1.Refresh();
            }
        }

        #endregion

        #region Input Dialog Methods

        private DialogResult InputBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonMP3 = new Button();
            Button buttonWAV = new Button();
            PictureBox icon1 = new PictureBox();

            if (isDigit == true)

            form.Text = title;
            label.Text = promptText;

            buttonMP3.Text = "MP3";
            buttonWAV.Text = "WAV";
            buttonMP3.DialogResult = DialogResult.OK;
            buttonWAV.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();

            label.SetBounds(50, 22, 290, 17);
            icon1.SetBounds(15, 15, 35, 35);
            buttonMP3.SetBounds(24, 54, 140, 23);
            buttonWAV.SetBounds(172, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonMP3.ForeColor = Color.Green;
            buttonWAV.ForeColor = Color.Green;
            buttonMP3.Font = new Font("Arial", 8, FontStyle.Bold);
            buttonWAV.Font = new Font("Arial", 8, FontStyle.Bold);

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonMP3, buttonWAV });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonMP3;
            form.CancelButton = buttonWAV;

            DialogResult dialogResult = form.ShowDialog(this);
            switch (dialogResult)
            {
                case DialogResult.OK:
                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        radioButton1.Enabled = false;
                        radioButton2.Enabled = false;
                        radioButton3.Enabled = false;
                        radioButton4.Enabled = true;
                        radioButton4.Checked = true;
                        comboBox3.Items.Clear();
                        comboBox4.Items.Clear();
                        comboBox3.Enabled = false;
                        comboBox4.Enabled = true;
                        tabPage1ConsoleCombo();
                        textBox1.Text = filePath;
                    }
                    else if (tabControl1.SelectedTab == tabPage2)
                    {
                        radioButton5.Enabled = false;
                        radioButton6.Enabled = false;
                        radioButton7.Enabled = false;
                        radioButton8.Enabled = true;
                        radioButton8.Checked = true;
                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = false;
                        tabPage2ConsoleCombo();
                        textBox2.Text = filePath;
                    }
                    break;
                case DialogResult.Cancel:
                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        radioButton1.Enabled = false;
                        radioButton3.Enabled = false;
                        radioButton4.Enabled = false;
                        radioButton2.Enabled = true;
                        radioButton2.Checked = true;
                        comboBox3.Items.Clear();
                        comboBox4.Items.Clear();
                        comboBox3.Enabled = false;
                        comboBox4.Enabled = true;
                        tabPage1ConsoleCombo();
                        textBox1.Text = filePath;
                    }
                    else if (tabControl1.SelectedTab == tabPage2)
                    {
                        radioButton5.Enabled = false;
                        radioButton7.Enabled = false;
                        radioButton8.Enabled = false;
                        radioButton6.Enabled = true;
                        radioButton6.Checked = true;
                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = false;
                        tabPage2ConsoleCombo();
                        textBox2.Text = filePath;
                    }
                    break;
            }
            return dialogResult;

        }

        #endregion

        #region Input Dialog Methods

        private DialogResult mInputBox(string title, string promptText, Icon icon, bool isDigit = false, int i = 0)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonYes = new Button();
            Button buttonNo = new Button();
            PictureBox icon1 = new PictureBox();
            int z = i;

            if (isDigit == true)

                form.Text = title;
            label.Text = promptText;

            buttonYes.Text = "Yes";
            buttonNo.Text = "No";
            buttonYes.DialogResult = DialogResult.OK;
            buttonNo.DialogResult = DialogResult.Cancel;
            icon1.Image = icon.ToBitmap();

            label.SetBounds(50, 22, 290, 17);
            icon1.SetBounds(15, 15, 35, 35);
            buttonYes.SetBounds(24, 54, 140, 23);
            buttonNo.SetBounds(172, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonYes.ForeColor = Color.Green;
            buttonNo.ForeColor = Color.Green;
            buttonYes.Font = new Font("Arial", 8, FontStyle.Bold);
            buttonNo.Font = new Font("Arial", 8, FontStyle.Bold);

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonYes, buttonNo });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonYes;
            form.CancelButton = buttonNo;

            DialogResult dialogResult = form.ShowDialog(this);
            if (z == 1)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:                       
                            at9DoProcess(); 
                        break;
                    case DialogResult.Cancel:

                        break;
                }
               
            }else if (z == 2)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                            at3DoProcess();
                        break;
                    case DialogResult.Cancel:

                        break;
                }
               
            }else if (z == 3)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                            System.IO.File.Delete(finalFileName2);
                            mMoveFile();
                        break;
                    case DialogResult.Cancel:
                        mMoveFile2();
                        break;
                }
                
            }else if (z == 4)
            {
                
                switch (dialogResult)
                {
                    case DialogResult.OK:

                        string path1 = dir + "\\" + originalFileName;
                        string path2 = sourceDirectory + "\\" + originalFileName;
                        if (!sourceDirectory.Equals(dir))
                        {
                            if (File.Exists(path1))
                            {
                                File.Delete(path1);
                                File.Copy(path2, path1);
                                rRename();
                            }
                            else
                            {
                                File.Copy(path2, path1);
                                rRename();
                            }

                        }

                        break;
                    case DialogResult.Cancel:

                        break;
                }
                
            }
            else if (z == 5)
            {
                string path = dir + "\\" + finalFileName;
                string path2 = sourceDirectory + "\\" + finalFileName;
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        File.Delete(path2);
                        File.Move(path, path2);
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            else if (z == 6)
            { 
                string path = dir + "\\" + finalFileName;
                string path1 = dir + "\\" + finalFileName2;
                string path2 = sourceDirectory + "\\" + finalFileName;
                string path3 = sourceDirectory + "\\" + finalFileName2;
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        if (File.Exists(path2))
                        {
                            File.Delete(path2);
                            File.Move(path, path2);
                        }
                        else
                        {
                            File.Move(path, path2);
                        }
                        if (File.Exists(path3))
                        {
                            File.Delete(path3);
                            File.Move(path1, path3);
                        }
                        else
                        {
                            File.Move(path1, path3);
                        }
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            else if (z == 7)
            {
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        mPlayer();
                        break;
                    case DialogResult.Cancel:

                        break;
                }

            }
            return dialogResult;
        }

        #endregion

        #region Message Dialog Methods

        private DialogResult mMessageBox(string title, string promptText, Icon icon, bool isDigit = false)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonOK = new Button();
            PictureBox icon1 = new PictureBox();

            if (isDigit == true)

            form.Text = title;
            label.Text = promptText;

            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            icon1.Image = icon.ToBitmap();

            label.SetBounds(60, 22, 290, 17);
            icon1.SetBounds(15, 15, 35, 35);
            buttonOK.SetBounds(100, 54, 140, 23);

            label.AutoSize = true;
            label.ForeColor = Color.DarkRed;
            label.Font = new Font("Arial", 10, FontStyle.Bold);
            buttonOK.ForeColor = Color.Green;
            buttonOK.Font = new Font("Arial", 8, FontStyle.Bold);

            form.ClientSize = new Size(335, 100);
            form.Controls.AddRange(new Control[] { icon1, label, buttonOK });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOK;

            DialogResult dialogResult = form.ShowDialog(this);
            return dialogResult;
        }

        #endregion

        #region Form Load Event Handler

        private void MainForm_Load(object sender, EventArgs e)
        {
            dir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string[] versionpart = this.ProductVersion.Split('.');
            version = versionpart[0] + "." + versionpart[1];
            this.Text += MainForm.version;
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "http://bmk.hamtek-solutions.com/";
            linkLabel2.Links.Add(link);
            toolStripStatusLabel1.Text = GetMsg("¡Listo!", "Ready!");
        }

        #endregion

        #region Combo Box Event Handlers

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true || radioButton7.Checked == true)
            {
                if (comboBox1.Text == "PSP") 
                { 
                    at3ToolPath = "PSP_at3ToolPath.exe"; button4.Enabled = false; tabPage2PSPCombo(); comboBox2.Enabled = true; 
                }
                else if (comboBox1.Text == "PS3") 
                { 
                    at3ToolPath = "PS3_at3ToolPath.exe"; button4.Enabled = false; tabPage2PS3Combo(); comboBox2.Enabled = true; 
                }
            }
            else
            {
                if (comboBox1.Text == "PSP") 
                { 
                    at3ToolPath = "PSP_at3ToolPath.exe"; button4.Enabled = true; tabPage2PSPCombo();
                }
                else if (comboBox1.Text == "PS3") 
                { 
                    at3ToolPath = "PS3_at3ToolPath.exe"; button4.Enabled = true; tabPage2PS3Combo();
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true || radioButton3.Checked == true)
            {
                if (comboBox4.Text == "PS4") 
                { 
                    at9ToolPath = "PS4_at9ToolPath.exe"; button2.Enabled = false; tabPage1PS4Combo(); comboBox3.Enabled = true; 
                }
                else if (comboBox4.Text == "PSVita")
                { 
                    at9ToolPath = "PSVita_at9ToolPath.exe"; button2.Enabled = false; tabPage1PSVitaCombo(); comboBox3.Enabled = true;
                    MessageBox.Show("If you make theme for PSVita/TV use the BitRate 144 for more compatibility", "Informations", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); 
                }
            }
            else
            {
                if (comboBox4.Text == "PS4") 
                { 
                    at9ToolPath = "PS4_at9ToolPath.exe"; button2.Enabled = true; tabPage1PS4Combo(); 
                }
                else if (comboBox4.Text == "PSVita") 
                { 
                    at9ToolPath = "PSVita_at9ToolPath.exe"; button2.Enabled = true; tabPage1PSVitaCombo();
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAt3Bitrate = comboBox2.Text;
            button4.Enabled = true;

            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                string dirPath = Path.GetDirectoryName(selectedFilePath);
                string fileNameOnly = Path.GetFileNameWithoutExtension(selectedFilePath);
                finalFileName = Path.Combine(dirPath, fileNameOnly + "_" + selectedAt3Bitrate + "bit.at3");
                finalFileName2 = Path.Combine(dirPath, fileNameOnly + ".wav");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAt9Bitrate = comboBox3.Text;
            button2.Enabled = true;

            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                string dirPath = Path.GetDirectoryName(selectedFilePath);
                string fileNameOnly = Path.GetFileNameWithoutExtension(selectedFilePath);
                finalFileName = Path.Combine(dirPath, fileNameOnly + "_" + selectedAt9Bitrate + "bit.at9");
                finalFileName2 = Path.Combine(dirPath, fileNameOnly + ".wav");
            }
        }

        private void tabPage1ConsoleCombo()
        {
            comboBox4.Items.Clear();
            foreach (var console in _bitrateConfig.ConsoleListAt9) { comboBox4.Items.Add(console); }
        }

        private void tabPage1PSVitaCombo()
        {
            comboBox3.Items.Clear();
            foreach (var bitrate in _bitrateConfig.PsvitaList) { comboBox3.Items.Add(bitrate); }
            if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;
        }

        private void tabPage1PS4Combo()
        {
            comboBox3.Items.Clear();
            foreach (var bitrate in _bitrateConfig.Ps4List) { comboBox3.Items.Add(bitrate); }
            if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;
        }

        private void tabPage2ConsoleCombo()
        {
            comboBox1.Items.Clear();
            foreach (var console in _bitrateConfig.ConsoleListAt3) { comboBox1.Items.Add(console); }
        }

        private void tabPage2PSPCombo()
        {
            comboBox2.Items.Clear();
            foreach (var bitrate in _bitrateConfig.PspList) { comboBox2.Items.Add(bitrate); }
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }

        private void tabPage2PS3Combo()
        {
            comboBox2.Items.Clear();
            foreach (var bitrate in _bitrateConfig.Ps3List) { comboBox2.Items.Add(bitrate); }
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }

        #endregion

        #region Picture Box Event Handlers

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (comboBox4.Text == "PS4" && comboBox3.Enabled == true)
            {
                Ps4BitrateInfo Ps4Load = new Ps4BitrateInfo();
                Ps4Load.Show();
            }
            else if (comboBox4.Text == "PSVita" && comboBox3.Enabled == true)
            {
                PsvitaBitrateInfo PsvitaLoad = new PsvitaBitrateInfo();
                PsvitaLoad.Show();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "PSP" && comboBox2.Enabled == true)
            {
                PspBitrateInfo PspLoad = new PspBitrateInfo();
                PspLoad.Show();
            }
            else if (comboBox1.Text == "PS3" && comboBox2.Enabled == true)
            {
                Ps3BitrateInfo Ps3Load = new Ps3BitrateInfo();
                Ps3Load.Show();
            }
        }

        #endregion

        #region Player Methods

        private void mPlayer()
        {
            if (comboBox1.Text == "PSP")
            {
                try
                {
                    playerProcess = new Process();
                    playerStartInfo = new ProcessStartInfo();
                    playerStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    playerStartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PLAYER\MiniPlayer.exe");
                    playerStartInfo.Arguments = "\"" + finalFileName + "\"";
                    playerStartInfo.UseShellExecute = false;
                    playerStartInfo.CreateNoWindow = true;
                    playerStartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    playerProcess.StartInfo = playerStartInfo;
                    playerProcess.Start();

                    toolStripStatusLabel1.Text = "Playing...";
                    statusStrip1.Refresh();
                    button1.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al reproducir: " + ex.Message);
                }
            }
        }

        #endregion

        #region Link Label Event Handlers

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playerProcess.Kill();
            toolStripStatusLabel1.Text = "Stop!";
            statusStrip1.Refresh();
            button1.Enabled = false;
        }

        #endregion
    }
}
