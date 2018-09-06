using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace notatnik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }



        private void zamknijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBox1.Text=="")
            {
                Application.Exit();
            }
            else
            {
              DialogResult dr = MessageBox.Show("Czy zapisać zmiany w edytownym dokumencie ?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);

            switch (dr)
            {
                case DialogResult.Yes:
                    zapiszJakoToolStripMenuItem_Click(null, null);
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }    
            }
        }

        private void pasekstanuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = pasekstanuToolStripMenuItem.Checked;
        }

        public static string[] CzytajPlikTesktowy(string nazwaPliku)
        {
            List<string> teskt = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(nazwaPliku))
                {
                    string wiersz;
                    while ((wiersz = sr.ReadLine()) != null)
                    {
                        teskt.Add(wiersz);
                    }
                    return teskt.ToArray();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Błąd odczytu pliku " + nazwaPliku + "\nOpis wyjątku: " + e.Message, "Notatnik -Błą przy wczytywaniu pliku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;

            }
        }

        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string nazwaPliku = openFileDialog1.FileName;
                textBox1.Lines = CzytajPlikTesktowy(nazwaPliku);
                int ostatniSlash = nazwaPliku.LastIndexOf('\\');
                toolStripStatusLabel1.Text = nazwaPliku.Substring(ostatniSlash + 1, nazwaPliku.Length - ostatniSlash - 1);
            }
        }

        public static void ZapiszDoPlikuTekstowego(string nazwapliku, string[] tekst)
        {
            using (StreamWriter sw = new StreamWriter(nazwapliku))
            {
                foreach (string wiersz in tekst)
                {
                    sw.WriteLine(wiersz);
                }
            }
        }

        private void zapiszJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string nazwaPliku = openFileDialog1.FileName;
            if (nazwaPliku.Length > 0)
            {
                saveFileDialog1.FileName = nazwaPliku;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                nazwaPliku = saveFileDialog1.FileName;
                ZapiszDoPlikuTekstowego(nazwaPliku, textBox1.Lines);
                int ostatniSlash = nazwaPliku.LastIndexOf('\\');
                toolStripStatusLabel1.Text = nazwaPliku.Substring(ostatniSlash + 1, nazwaPliku.Length - ostatniSlash - 1);
            }
        }

        private void tłoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = textBox1.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.BackColor = colorDialog1.Color;
            }
        }

        private void czcionkaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = textBox1.Font;
            fontDialog1.Color = textBox1.ForeColor;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Font = fontDialog1.Font;
                textBox1.ForeColor = fontDialog1.Color;
            }
        }

        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Undo();

        }

        private void wytnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void kopiujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void wklejToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        private void usuńToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.SelectedText = "";
        }

        private void zaznaczWszystkoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

         private StringReader sr = null;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
             
        Font czcionka = textBox1.Font;
            int wysokosWiersza = (int)czcionka.GetHeight(e.Graphics);
            int iloscLinii=e.MarginBounds.Height / wysokosWiersza;
            if (sr==null)
            {
                string tekst = " ";
                foreach (string wiersz in textBox1.Lines)
                {
                    float szerokosc = e.Graphics.MeasureString(wiersz, czcionka).Width;
                    if (szerokosc<e.MarginBounds.Width)
                    {
                        tekst += wiersz + "\n";
                    }
                    else
                    {
                        float sredniaSzerokoscLitery = szerokosc / wiersz.Length;
                        int ileLiterWWierszu = (int)(e.MarginBounds.Width / sredniaSzerokoscLitery);
                        string skracanyWiersz = wiersz;
                        do
                        {
                            int ostatniaSpacja = skracanyWiersz.Substring(0, ileLiterWWierszu).LastIndexOf(' ');
                            int iloscLiter = ostatniaSpacja != -1 ? Math.Min(ostatniaSpacja, ileLiterWWierszu) : ileLiterWWierszu;
                            tekst += skracanyWiersz.Substring(0, iloscLiter) + "\n";
                            skracanyWiersz = skracanyWiersz.Substring(iloscLiter).TrimStart(' ');
                        } while (skracanyWiersz.Length>ileLiterWWierszu);
                        tekst += skracanyWiersz + "\n";
                    }    
                }    
                  sr = new StringReader(tekst);
            }
                  
            e.HasMorePages = true;
            for (int i = 0; i < iloscLinii; i++)
            {
                string wiersz = sr.ReadLine();
                if (wiersz==null)
                {
                    e.HasMorePages = false;
                    sr = null;
                    break;
                }
                e.Graphics.DrawString(wiersz, czcionka, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top + i * wysokosWiersza);
            }
        }

        private void toolStripMenuItemDrukuj_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog()==DialogResult.OK)
            {
                printDocument1.DocumentName = "Notatnik.NET - " + toolStripStatusLabel1.Text;
                //  printDocument1.Print();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void toolStripMenuItemUstawieniaStrony_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.ShowDialog();
        }

        private void toolStripMenuItemPodgladWydruku_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            printDocument1.Print();
        }

        private void textBox1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] nameDropFile = e.Data.GetData(DataFormats.FileDrop) as string[];
                string nameFile = nameDropFile[0];
                textBox1.Lines = CzytajPlikTesktowy(nameFile);
                int lastSlash = nameFile.LastIndexOf('\\');
                toolStripStatusLabel1.Text = nameFile.Substring(lastSlash + 1, nameFile.Length - lastSlash - 1);
            }
        }
    }
}
                                         