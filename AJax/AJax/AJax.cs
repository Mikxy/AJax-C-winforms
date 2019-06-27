using sxlib;
using sxlib.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AJax
{
    public partial class AJax : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void formdrag_mousedown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        public AJax()
        {
            InitializeComponent();
        }

        bool a = false;

        private async void Form4_Load(object sender, EventArgs e)
        {
            TUpdate("Loading");
            Tools.Enabled = false;
            Scriptbox.Enabled = false;
            Scriptlist.Enabled = false;

            try
            {
                foreach (string path in Directory.GetFiles("./Scripts"))
                {
                    this.Scriptlist.Items.Add(Path.GetFileName(path));
                }
            }
            catch (Exception)
            {
                Directory.CreateDirectory("./Scripts");
                foreach (string path2 in Directory.GetFiles("./Scripts"))
                {
                    this.Scriptlist.Items.Add(Path.GetFileName(path2));
                }

            }

            try
            {
                a = Convert.ToBoolean(new Settings().SettingsConfig.Read("ScriptsFolder", "Configuration"));

                if (a == false)
                {
                    this.Size = new Size(500, 300);
                }
                else
                {
                    this.Size = new Size(658, 300);
                }
                
            }
            catch (Exception)
            {

            }

            toolStripMenuItem3.Text = "Scripts Folder = " + a.ToString();

            if (File.Exists("Settings.AJax"))
            {
                string tm = new Settings().SettingsConfig.Read("Topmost", "Configuration");
                TopMost = Convert.ToBoolean(tm);
                toolStripMenuItem4.Text = "Topmost = " + TopMost.ToString();

                string wr = new Settings().SettingsConfig.Read("WordWrap", "Configuration");
                Scriptbox.WordWrap = Convert.ToBoolean(wr);
                toolStripMenuItem2.Text = "WordWrap = " + Scriptbox.WordWrap.ToString();
                Scriptbox.Text = File.ReadAllText(@"bin\\AJax.lua");

            }
            else
            {
                toolStripMenuItem4.Text = "Topmost = True";
                toolStripMenuItem2.Text = "WordWrap = False";
            }

            if (File.Exists(@"bin\\AJax.lua"))
            {
                await Task.Delay(500);
                Scriptbox.Text = File.ReadAllText(@"bin\\AJax.lua");
            }

            Synapse = SxLib.InitializeOffscreen(directory);
            Synapse.LoadEvent += Loader;
            Synapse.AttachEvent += Attacher;
            Synapse.Load();

        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure?", "AJax - Close Confirmation", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                TUpdate("Closing");
                Environment.Exit(-1);
            }
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

            a = !a;
            if (a == false)
            {
                this.Size = new Size(500, 300);
            }
            else
            {
                this.Size = new Size(658, 300);
            }

            new Settings().SettingsConfig.Write("ScriptsFolder", a.ToString(), "Configuration");
            toolStripMenuItem3.Text = "Scripts Folder = " + a.ToString();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {

            TopMost = !TopMost;
            new Settings().SettingsConfig.Write("Topmost", TopMost.ToString(), "Configuration");
            toolStripMenuItem4.Text = "Topmost = " + TopMost.ToString();
        }

        void TUpdate(string text)
        {
            this.Text = "Ajax - " + text;
        }

        async void RUpdate()
        {
            await Task.Delay(3000);
            this.Text = "Ajax - Idle";
        }


        private void toolStripMenuItem2_Click_2(object sender, EventArgs e)
        {
            Scriptbox.WordWrap = !Scriptbox.WordWrap;
            new Settings().SettingsConfig.Write("WordWrap", Scriptbox.WordWrap.ToString(), "Configuration");
            toolStripMenuItem2.Text = "WordWrap = " + Scriptbox.WordWrap.ToString();
        }

        public static SxLibOffscreen Synapse;
        
        void Attacher(SxLibBase.SynAttachEvents Event)
        {
            switch (Event)
            {
                case SxLibBase.SynAttachEvents.READY:
                    {
                        TUpdate("Injected");
                        Tools.Enabled = true;
                        RUpdate();
                        break;
                    }

                case SxLibBase.SynAttachEvents.FAILED_TO_FIND:
                    {
                        TUpdate("Roblox not found");
                        Tools.Enabled = true;
                        RUpdate();
                        break;
                    }

                case SxLibBase.SynAttachEvents.SCANNING:
                    {
                        TUpdate("Scanning");
                        break;
                    }

                case SxLibBase.SynAttachEvents.INJECTING:
                    {
                        TUpdate("Injecting");
                        break;
                    }

                case SxLibBase.SynAttachEvents.CHECKING_WHITELIST:
                    {
                        TUpdate("Checking whitelist");
                        break;
                    }

                case SxLibBase.SynAttachEvents.CHECKING:
                    {
                        TUpdate("Checking Roblox");
                        break;
                    }

                case SxLibBase.SynAttachEvents.NOT_RUNNING_LATEST_VER:
                    {
                        TUpdate("SynX UI is outdated");
                        Tools.Enabled = true;
                        RUpdate();
                        break;
                    }

                case SxLibBase.SynAttachEvents.ALREADY_INJECTED:
                    {
                        TUpdate("Already Injected");
                        Tools.Enabled = true;
                        RUpdate();
                        break;
                    }

                case SxLibBase.SynAttachEvents.NOT_INJECTED:
                    {
                        TUpdate("Inject first");
                        Tools.Enabled = true;
                        RUpdate();
                        break;
                    }

                case SxLibBase.SynAttachEvents.PROC_DELETION:
                    {
                        TUpdate("Disconnected from Roblox");
                        RUpdate();
                        break;
                    }
            }
        }

        void Loader(SxLibBase.SynLoadEvents Event)
        {
            switch (Event)
            {
                case SxLibBase.SynLoadEvents.READY:
                    {

                        TUpdate("Idle");
                        Tools.Enabled = true;
                        Scriptbox.Enabled = true;
                        Scriptlist.Enabled = true;
                        //Synapse.Attach();
                        break;
                    }

                case SxLibBase.SynLoadEvents.NOT_UPDATED:
                    {
                        TUpdate("Wait for Roblox Update");
                        break;
                    }
            }
        }

        public static string directory = Directory.GetCurrentDirectory();

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Synapse.Execute(Scriptbox.Text);
            File.WriteAllText(@"bin\\AJax.lua", Scriptbox.Text);
        }

        private void Scriptbox_Load(object sender, EventArgs e)
        {

        }

        private void Scriptbox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Scriptbox.Text = "";
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "AJax Save Script";
            sfd.Filter = "Lua Script (*.lua*)|*.lua*";
            sfd.ShowDialog();
            try
            {
                string script = Scriptbox.Text;
                File.WriteAllText(sfd.FileName, Scriptbox.Text);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "AJax Open Script";
            ofd.Filter = "Lua Script (*.lua*)|*.lua*";
            ofd.ShowDialog();
            try
            {
                string file = File.ReadAllText(ofd.FileName);
                Scriptbox.Text = file;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            TUpdate("Loading");
            Tools.Enabled = false;
            try
            {
                Synapse.Attach();
            }
            catch (Exception)
            {
                Synapse.Load();
            }
        }

        private void Scriptlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string text = File.ReadAllText("./Scripts/" + this.Scriptlist.SelectedItems[0]);
                this.Scriptbox.Text = text;
            }
            catch (Exception)
            {
                try
                {
                    string text2 = File.ReadAllText("./Scripts/" + this.Scriptlist.SelectedItems[0]);
                    this.Scriptbox.Text = text2;
                }
                catch (Exception)
                {

                }
            }
        }

        private void Tools_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
