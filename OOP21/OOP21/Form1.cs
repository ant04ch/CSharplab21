using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace OOP21
{
    public partial class Form1 : Form
    {
        private string currentFilePath;
        private ToolStripStatusLabel statusLabel;
        private int openDocuments = 0;
        public string DocName { get; set; }
        private SyntaxHighlighter syntaxHighlighter;
        public Form2 fr2;

        public Form1()
        {
            InitializeComponent();
            InitializeStatusLabel();
            FormClosing += Form1_FormClosing;
            syntaxHighlighter = new SyntaxHighlighter();

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsFileModified() && currentFilePath != null)
            {
                DialogResult result = MessageBox.Show("Файл був змінений. Бажаєте зберегти зміни?", "Зберегти зміни", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    зберегтиToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
        private void InitializeStatusLabel()
        {
            statusLabel = new ToolStripStatusLabel();
            statusStrip1.Items.Add(statusLabel);
        }

        private void створитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(richTextBox1.Text) && IsFileModified())
            {
                var result = MessageBox.Show("Бажаєте зберегти зміни у поточному файлі?", "Попередження", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    зберегтиToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                richTextBox1.Text = string.Empty;
                currentFilePath = null;
                statusLabel.Text = string.Empty;
            }

            richTextBox1.Text = string.Empty;
            currentFilePath = null;

            openDocuments++;
            string docName = "Новий файл " + openDocuments;

            Form1 newForm = new Form1();
            newForm.DocName = docName;
            newForm.Text = docName;
            newForm.menuStrip1.Visible = false; // Приховати меню
            newForm.Show();
        }

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = openFileDialog.FileName;
                    richTextBox1.Text = File.ReadAllText(currentFilePath);
                    statusLabel.Text = Path.GetFileName(currentFilePath);
                }
            }
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFilePath != null)
            {
                File.WriteAllText(currentFilePath, richTextBox1.Text);
                statusLabel.Text = Path.GetFileName(currentFilePath);
            }
            else
            {
                зберегтиякToolStripMenuItem_Click_1(sender, e);
            }
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFileModified() && currentFilePath != null)
            {
                DialogResult result = MessageBox.Show("Файл був змінений. Бажаєте зберегти зміни?", "Зберегти зміни", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    зберегтиToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            System.Windows.Forms.Application.Exit();
        }

        private bool IsFileModified()
        {
            if (currentFilePath != null)
            {
                string fileContent = File.ReadAllText(currentFilePath);
                return fileContent != richTextBox1.Text;
            }

            return false;
        }

        private void зберегтиякToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*";

                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    saveFileDialog.FileName = Path.GetFileName(currentFilePath);
                }
                else if (this.Text != "Form1")
                {
                    saveFileDialog.FileName = this.Text;
                }
                else
                {
                    saveFileDialog.FileName = "Untitled";
                }

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = saveFileDialog.FileName;
                    richTextBox1.SaveFile(currentFilePath, RichTextBoxStreamType.RichText);
                    statusLabel.Text = Path.GetFileName(currentFilePath);
                }
            }
        }

        private void новевікноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 newForm = new Form1();
            newForm.Show();
        }

        private void друкToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    PrintDocument printDocument = new PrintDocument();
                    printDocument.PrintPage += PrintDocument_PrintPage;

                    printDocument.Print();
                }
            }
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            string textToPrint = richTextBox1.Text;
            Font printFont = new Font("Arial", 12);
            Brush printBrush = Brushes.Black;
            float printX = e.MarginBounds.Left;
            float printY = e.MarginBounds.Top;
            float linesPerPage = e.MarginBounds.Height / printFont.GetHeight(e.Graphics);

            int printedLines = 0;

            while (printedLines < linesPerPage && !string.IsNullOrEmpty(textToPrint))
            {
                e.Graphics.DrawString(textToPrint, printFont, printBrush, printX, printY);

                printedLines++;
                printY += printFont.GetHeight(e.Graphics);
                textToPrint = textToPrint.Substring(textToPrint.IndexOf('\n') + 1);
            }

            if (!string.IsNullOrEmpty(textToPrint))
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }
        }

        private void вирізатиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void скасуватидіюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void копіюватиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void вставкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void виділитивсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }
        private void знайтиToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void видалитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "";
        }

        private void налаштуванняToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void моваToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void українськаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            українськаToolStripMenuItem.Checked = true;
            англійськаToolStripMenuItem.Checked=false;
            зберегтиякToolStripMenuItem.Text = "Зберегти як";
            новевікноToolStripMenuItem.Text = "Нове вікно";
            друкToolStripMenuItem.Text = "Друк";
            вирізатиToolStripMenuItem.Text = "Вирізати";
            скасуватидіюToolStripMenuItem.Text = "Скасувати дію";
            копіюватиToolStripMenuItem.Text = "Копіювати";
            вставкаToolStripMenuItem.Text = "Вставка";
            виділитивсеToolStripMenuItem.Text = "Виділити все";
            знайтиToolStripMenuItem.Text = "Знайти...";
            видалитиToolStripMenuItem.Text = "Видалити";
            українськаToolStripMenuItem.Text = "Українська";
            англійськаToolStripMenuItem.Text = "Англійська";
            вирівнятиТекстToolStripMenuItem.Text = "Вирівняти текст";
            заЛівимКраємToolStripMenuItem.Text = "За лівим краєм";
            поЦентруToolStripMenuItem.Text = "По центру";
            заПравимКраємToolStripMenuItem.Text = "За правим краєм";
            шрифтToolStripMenuItem.Text = "Шрифт";
            підкреслюватиToolStripMenuItem.Text = "Підкреслювати";
            додатиЗображенняToolStripMenuItem.Text = "Додати зображення";
            опрограммеToolStripMenuItem.Text = "Про програму";
            файлToolStripMenuItem.Text = "Файл";
            створитиToolStripMenuItem.Text = "Створити";
            відкритиToolStripMenuItem.Text = "Відкрити";
            зберегтиToolStripMenuItem.Text = "Зберегти";
            вихідToolStripMenuItem.Text = "Вихід";
            сервисToolStripMenuItem.Text = "Сервіс";
            моваToolStripMenuItem.Text = "Мова";
            налаштуванняToolStripMenuItem.Text = "Налаштування";
            опрограммеToolStripMenuItem.Text = "Про програму";
            правкаToolStripMenuItem.Text = "Редагування";
            справкаToolStripMenuItem.Text = "Довідка";
        }

        private void англійськаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            українськаToolStripMenuItem.Checked = false;
            англійськаToolStripMenuItem.Checked = true;
            зберегтиякToolStripMenuItem.Text = "Save As";
            новевікноToolStripMenuItem.Text = "New Window";
            друкToolStripMenuItem.Text = "Print";
            вирізатиToolStripMenuItem.Text = "Cut";
            скасуватидіюToolStripMenuItem.Text = "Undo";
            копіюватиToolStripMenuItem.Text = "Copy";
            вставкаToolStripMenuItem.Text = "Paste";
            виділитивсеToolStripMenuItem.Text = "Select All";
            знайтиToolStripMenuItem.Text = "Find";
            видалитиToolStripMenuItem.Text = "Delete";
            українськаToolStripMenuItem.Text = "Ukrainian";
            англійськаToolStripMenuItem.Text = "English";
            вирівнятиТекстToolStripMenuItem.Text = "Align Text";
            заЛівимКраємToolStripMenuItem.Text = "Align Left";
            поЦентруToolStripMenuItem.Text = "Align Center";
            заПравимКраємToolStripMenuItem.Text = "Align Right";
            шрифтToolStripMenuItem.Text = "Font";
            підкреслюватиToolStripMenuItem.Text = "Underline";
            додатиЗображенняToolStripMenuItem.Text = "Add Image";
            опрограммеToolStripMenuItem.Text = "About";
            файлToolStripMenuItem.Text = "File";
            створитиToolStripMenuItem.Text = "Create";
            відкритиToolStripMenuItem.Text = "Open";
            зберегтиToolStripMenuItem.Text = "Save";
            вихідToolStripMenuItem.Text = "Exit";
            сервисToolStripMenuItem.Text = "Service";
            моваToolStripMenuItem.Text = "Language";
            налаштуванняToolStripMenuItem.Text = "Settings";
            опрограммеToolStripMenuItem.Text = "About";
            правкаToolStripMenuItem.Text = "Edit";
            справкаToolStripMenuItem.Text = "Help";
        }

        private void вирівнятиТекстToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void заЛівимКраємToolStripMenuItem_Click(object sender, EventArgs e)
        {
                richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void поЦентруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void заПравимКраємToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void шрифтToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.SelectionFont = fontDialog.Font;
                }
            }
        }

        private void підкреслюватиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            підкреслюватиToolStripMenuItem.Checked = !підкреслюватиToolStripMenuItem.Checked;

            if (підкреслюватиToolStripMenuItem.Checked)
            {
                підкреслюватиToolStripMenuItem.Text = "Вимкнути підкреслення";
                syntaxHighlighter.HighlightSyntax(richTextBox1);
            }
            else
            {
                підкреслюватиToolStripMenuItem.Text = "Підкреслити синтаксис";
                syntaxHighlighter.ResetSyntaxHighlight(richTextBox1);
            }
        }
        public class SyntaxHighlighter
        {
            private readonly List<string> keywords;

            public SyntaxHighlighter()
            {
                keywords = new List<string>
        {
            "int ", "float ", "double ", "char ", "void ", "if ", "else ", "for ", "while ", "do ", "switch ",
            "case ", "break ", "continue ", "return ", "string ", "cin ", "cout "
            // Add more keywords as needed
        };
            }

            public void HighlightSyntax(RichTextBox richTextBox)
            {
                string text = richTextBox.Text;
                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = text.Length;
                richTextBox.SelectionColor = Color.Black;

                foreach (string keyword in keywords)
                {
                    int index = 0;
                    while (index < text.Length)
                    {
                        index = text.IndexOf(keyword, index);
                        if (index == -1)
                            break;

                        richTextBox.SelectionStart = index;
                        richTextBox.SelectionLength = keyword.Length;
                        richTextBox.SelectionColor = Color.Blue;

                        index += keyword.Length;
                    }
                }
            }
            public void ResetSyntaxHighlight(RichTextBox richTextBox)
            {
                string text = richTextBox.Text;
                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = text.Length;
                richTextBox.SelectionColor = Color.Black;

                // Додайте наступні рядки для зміни кольору виділення на чорний
                richTextBox.SelectionStart = 0;
                richTextBox.SelectionLength = text.Length;
                richTextBox.SelectionColor = Color.Black;
            }
        }

        private void додатиЗображенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;
                    InsertImage(imagePath);
                }
            }
        }
        private void InsertImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
                Clipboard.SetImage(image);
                richTextBox1.Paste(DataFormats.GetFormat(DataFormats.Bitmap));
            }
        }

        private void опрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void правкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
