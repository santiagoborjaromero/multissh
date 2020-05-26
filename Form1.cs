using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Renci.SshNet;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MTSSH
{
    public partial class frmMain : Form
    {
        [DllImport("user32.dll")] static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")] static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);
        
        StreamReader r;
        string config_file = "";
        string DBConfig;
        int activateOneTime = 0;
        int workspaces_selected = 0;

        //Settings set = new Settings();
        List<Settings> set = new List<Settings>();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public frmMain()
        {
            InitializeComponent();
            InternalConfig.path = System.Windows.Forms.Application.UserAppDataPath;

            config_file = @"" + InternalConfig.path + @"\" + InternalConfig.file_config;
        }


        private void frmMain_Resize(object sender, EventArgs e)
        {
            workspace(workspaces_selected);
        }

        private  void joinObj(int x)
        {
            string s = string.Empty;
            string host = string.Empty;
            string port = string.Empty;
            string user = string.Empty;
            string pass = string.Empty;
            try
            {
                switch (x)
                {
                    case 1:
                        //s = string.Format("{0}@{1} {2} -pw {3}", txtUser1.Text, txtHost1.Text, txtPort1.Text, txtPass1.Text);
                        host = txtHost1.Text;
                        port = txtPort1.Text;
                        user = txtUser1.Text;
                        pass = txtPass1.Text;
                        break;
                    case 2:
                        //s = string.Format("{0}@{1} {2} -pw {3}", txtUser2.Text, txtHost2.Text, txtPort2.Text, txtPass2.Text);
                        //s= txtUser2.Text + "@" + txtHost2.Text + " " + txtPort2.Text + " -pw " + txtPass2.Text;
                        host = txtHost2.Text;
                        port = txtPort2.Text;
                        user = txtUser2.Text;
                        pass = txtPass2.Text;
                        break;
                    case 3:
                        //s = string.Format("{0}@{1} {2} -pw {3}", txtUser3.Text, txtHost3.Text, txtPort3.Text, txtPass3.Text);
                        //s= txtUser3.Text + "@" + txtHost3.Text + " " + txtPort3.Text + " -pw " + txtPass3.Text;
                        host = txtHost3.Text;
                        port = txtPort3.Text;
                        user = txtUser3.Text;
                        pass = txtPass3.Text;
                        break;
                    case 4:
                        //s = string.Format("{0}@{1} {2} -pw {3}", txtUser4.Text, txtHost4.Text, txtPort4.Text, txtPass4.Text);
                        //s= txtUser4.Text + "@" + txtHost4.Text + " " + txtPort4.Text + " -pw " + txtPass4.Text;
                        host = txtHost4.Text;
                        port = txtPort4.Text;
                        user = txtUser4.Text;
                        pass = txtPass4.Text;
                        break;
                }

                //Validations
                string msgError = string.Empty;
                if (host == "" && port=="" && user=="" && pass == ""){
                    msgError = "You must select any option to continue";
                    Util.Log("ERROR", msgError);
                    MessageBox.Show(msgError, "MTSSH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (host == "Server" || host == "")
                {
                    msgError = "The Host can't be Server or empty";
                    Util.Log("ERROR", msgError);
                    MessageBox.Show(msgError, "MTSSH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (port == "Port" || port == "")
                {
                    msgError = "The Port can't be Port or empty";
                    Util.Log("ERROR", msgError);
                    MessageBox.Show(msgError, "MTSSH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (user == "User" || user == "")
                {
                    msgError = "The User can't be User or empty";
                    Util.Log("ERROR", msgError);
                    MessageBox.Show(msgError, "MTSSH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                s = string.Format("{0}@{1} {2} -pw {3}", user, host, port, pass);

                Util.Log("INFO", "P" + x.ToString() + " | " +  s);
                SshConn(x, s);
            }
            catch(Exception e)
            {
                Util.Log("ERROR", e.ToString());
                MessageBox.Show("There is a problem with the configuration of each server, please check it and try again", "MTSSH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

            joinObj(1);

        }

        private void btm2_Click(object sender, EventArgs e)
        { 
            joinObj(2);
        }

        private void btm3_Click(object sender, EventArgs e)
        {

            joinObj(3);
        }

        private void btm4_Click(object sender, EventArgs e)
        {

            joinObj(4);
        }

        private void btmAll_Click(object sender, EventArgs e)
        {
            joinObj(1);
            joinObj(2);
            joinObj(3);
            joinObj(4);
        }

        private void btmSave_Click(object sender, EventArgs e)
        {
            
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            if (activateOneTime == 0)
            {
                activateOneTime = 1;
                reloadConfig();
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
            }
        }

        private void SshConn(int area, string text)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "putty.exe";
                info.Arguments = string.Format("-ssh " + text);
                info.UseShellExecute = true;
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Maximized;
                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = false;
                info.RedirectStandardError = false;
                Process p = Process.Start(info);
                p.WaitForInputIdle();

                int minus = 10;
                switch (area)
                {
                    case 1:
                        SetParent(p.MainWindowHandle, panel1.Handle);
                        MoveWindow(p.MainWindowHandle, 0, 0, panel1.Width - minus, panel1.Height - minus, true);
                        break;
                    case 2:
                        SetParent(p.MainWindowHandle, panel2.Handle);
                        MoveWindow(p.MainWindowHandle, 0, 0, panel2.Width - minus, panel2.Height - minus, true);
                        break;
                    case 3:
                        SetParent(p.MainWindowHandle, panel3.Handle);
                        MoveWindow(p.MainWindowHandle, 0, 0, panel3.Width - minus, panel3.Height - minus, true);
                        break;
                    case 4:
                        SetParent(p.MainWindowHandle, panel4.Handle);
                        MoveWindow(p.MainWindowHandle, 0, 0, panel4.Width - minus, panel4.Height - minus, true);
                        break;
                }
                
            }catch(Exception e)
            {
                Util.Log("ERROR", e.ToString());
                MessageBox.Show("There is a problem with the configuration of each server, please check it and try again", "MTSSH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btmClose_Click(object sender, EventArgs e)
        {
            pLog.Visible = false;
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {
            
        }

        private void pictureBox3_Click_2(object sender, EventArgs e)
        {
            
            txtLog.Text = Util.ReadLog();
            pLog.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            txt_Shost.Text = string.Empty;
            txt_SPort.Text = string.Empty;
            txt_SUser.Text = string.Empty;
            txt_SPass.Text = string.Empty;


            txtSettings.Text = Util.ReadConfig();
            pSetting.Visible = true;

            txt_Shost.Focus();
        }

        public void reloadConfig()
        {
            dynamic config_array = null;

            txtHost1.Text = "";
            txtPort1.Text = "";
            txtUser1.Text = "";
            txtPass1.Text = "";

            txtHost2.Text = "";
            txtPort2.Text = "";
            txtUser2.Text = "";
            txtPass2.Text = "";

            txtHost3.Text = "";
            txtPort3.Text = "";
            txtUser3.Text = "";
            txtPass3.Text = "";

            txtHost4.Text = "";
            txtPort4.Text = "";
            txtUser4.Text = "";
            txtPass4.Text = "";


            if (File.Exists(config_file))
            {
                using (r = new StreamReader(config_file)) { DBConfig = r.ReadToEnd(); }
                config_array = (Object)JsonConvert.DeserializeObject(DBConfig);
            }

            if (!File.Exists(config_file) || config_array == null)
            {
                set.Clear();
            }
            else
            {
                set.Clear();

                foreach(dynamic c in config_array)
                {
                    Settings s = new Settings();
                    s.host = c.host;
                    s.port = c.port;
                    s.user = c.user;
                    s.pass = c.pass;
                    set.Add(s);
                }

                foreach (Settings c in set)
                {
                    cmb1.Items.Add(c.host);
                    cmb2.Items.Add(c.host);
                    cmb3.Items.Add(c.host);
                    cmb4.Items.Add(c.host);
                }
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
           
        }

        private void workspace(int ws)
        {
            dynamic constLeft = 5;
            dynamic constTop = 10;
            dynamic top = 60;
            dynamic minus = 40;

            dynamic h;
            dynamic w;

            workspaces_selected = ws;

            switch (ws)
            {
                case 1:
                    h = this.Height - minus - top;
                    w = this.Width - minus;

                    panel1.Top = top;
                    panel1.Left = constLeft;
                    panel1.Width = w;
                    panel1.Height = h - 10;

                    panel1.Visible = true;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel4.Visible = false;
                    break;
                case 21:
                    h = this.Height - minus - top;
                    w = (((this.Width - minus) - (constLeft * 3)) / 2);

                    panel1.Top = top;
                    panel1.Left = constLeft;
                    panel1.Width = w;
                    panel1.Height = h - 10;

                    panel2.Top = top;
                    panel2.Left = w + (constLeft * 2);
                    panel2.Width = w;
                    panel2.Height = h - 10;


                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = false;
                    panel4.Visible = false;
                    break;
                case 22:
                    h = (((this.Height - minus - top) - (constTop * 3)) / 2);
                    w = this.Width - minus;

                    panel1.Top = top;
                    panel1.Left = constLeft;
                    panel1.Width = w;
                    panel1.Height = h;

                    panel2.Top = top + (constTop * 2) + h; 
                    panel2.Left = constLeft;
                    panel2.Width = w;
                    panel2.Height = h;

                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = false;
                    panel4.Visible = false;
                    break;
                case 31:
                    h = (((this.Height - minus - top) - (constTop * 3)) / 2);
                    w = (((this.Width - minus) - (constLeft * 3)) / 2);

                    panel1.Top = top;
                    panel1.Left = constLeft;
                    panel1.Width = w;
                    panel1.Height = h;

                    panel2.Top = top;
                    panel2.Left = w + (constLeft * 2);
                    panel2.Width = w;
                    panel2.Height = h;

                    w = this.Width - minus;

                    panel3.Top = top + (constTop * 2) + h;
                    panel3.Left = constLeft;
                    panel3.Width = w;
                    panel3.Height = h;

                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = false;
                    break;
                case 32:
                    h = (((this.Height - minus - top) - (constTop * 3)) / 2);
                    w = this.Width - minus;

                    panel1.Top = top;
                    panel1.Left = constLeft;
                    panel1.Width = w;
                    panel1.Height = h;


                    w = (((this.Width - minus) - (constLeft * 3)) / 2);

                    panel2.Top = top + (constTop * 2) + h;
                    panel2.Left = constLeft;
                    panel2.Width = w;
                    panel2.Height = h;

                    panel3.Top = top + (constTop * 2) + h;
                    panel3.Left = w + (constLeft * 2);
                    panel3.Width = w;
                    panel3.Height = h;

                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = false;
                    break;
                case 4:
                    h = (((this.Height - minus - top) - (constTop * 3)) / 2);
                    w = (((this.Width - minus) - (constLeft * 3)) / 2);

                    panel1.Height = h;
                    panel2.Height = h;
                    panel3.Height = h;
                    panel4.Height = h;

                    panel1.Width = w;
                    panel2.Width = w;
                    panel3.Width = w;
                    panel4.Width = w;

                    panel1.Top = top;
                    panel2.Top = panel1.Top;
                    panel3.Top = top + (constTop * 2) + h;
                    panel4.Top = panel3.Top;

                    panel1.Left = constLeft;
                    panel2.Left = w + (constLeft * 2);
                    panel3.Left = panel1.Left;
                    panel4.Left = panel2.Left;


                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = true;
                    break;
            }
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void btnWorkspaceDown_Click(object sender, EventArgs e)
        {
            panelWorkSpace.Visible = true;
            btnWorkspaceDown.Visible = false;
            btnWorkspaceUp.Visible = true;
        }

        private void pictureBox7_Click_1(object sender, EventArgs e)
        {

        }

        private void w1_Click(object sender, EventArgs e)
        {
            workspace(1);
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void w21_Click(object sender, EventArgs e)
        {
            workspace(21);
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void w22_Click(object sender, EventArgs e)
        {
            workspace(22);
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void w31_Click(object sender, EventArgs e)
        {
            workspace(31);
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void w32_Click(object sender, EventArgs e)
        {
            workspace(32);
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void w4_Click(object sender, EventArgs e)
        {
            workspace(4);
            panelWorkSpace.Visible = false;
            btnWorkspaceDown.Visible = true;
            btnWorkspaceUp.Visible = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            joinObj(1);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            joinObj(3);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            joinObj(2);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            joinObj(4);
        }

        private void pictureBox9_Click_1(object sender, EventArgs e)
        {
            pSetting.Visible = false;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string json = string.Empty;
            System.IO.File.WriteAllText(config_file, json);
            txtSettings.Text = Util.ReadConfig();

            txt_Shost.Focus();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            txtSettings.Text = Util.ReadConfig();
        }

        private void pictureBox7_Click_2(object sender, EventArgs e)
        {
            if (txt_Shost.Text != string.Empty && txt_SPort.Text != string.Empty && txt_SUser.Text != string.Empty && txt_SPass.Text != string.Empty)
            {
                if (searchData(txt_Shost.Text, 0))
                {
                    modifyData(txt_Shost.Text,txt_SPort.Text,txt_SUser.Text,txt_SPass.Text);
                }
                else
                {
                    Settings s = new Settings();
                    s.host = txt_Shost.Text;
                    s.port = txt_SPort.Text;
                    s.user = txt_SUser.Text;
                    s.pass = txt_SPass.Text;

                    set.Add(s);
                }

                dynamic json = JsonConvert.SerializeObject(set, Formatting.Indented);
                System.IO.File.WriteAllText(config_file, json);

                reloadConfig();
            }

            txtSettings.Text = Util.ReadConfig();

            txt_Shost.Text = string.Empty;
            txt_SPort.Text = string.Empty;
            txt_SUser.Text = string.Empty;
            txt_SPass.Text = string.Empty;
            txt_Shost.Focus();
        }

        private void txt_Shost_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_Shost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txt_SPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txt_SUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txt_SPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                //SendKeys.Send("{TAB}");


            }
        }

        private void cmb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchData(cmb2.Text, 2);
        }

        private void cmb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchData(cmb1.Text,1);
        }

        private void cmb4_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchData(cmb4.Text, 4);
        }

        private void cmb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchData(cmb3.Text, 3);
        }


        private Boolean searchData(string hostObject, int panelObject)
        {
            string host = "";
            string port = "";
            string user = "";
            string pass = "";

            Boolean search = false;

            foreach (Settings s in set)
            {
                if (s.host == hostObject)
                {
                    host = s.host;
                    port = s.port;
                    user = s.user;
                    pass = s.pass;
                    search = true;
                }
            }

            if (search && panelObject>0)
            {
                switch (panelObject)
                {
                    case 1:
                        txtHost1.Text = host;
                        txtPort1.Text = port;
                        txtUser1.Text = user;
                        txtPass1.Text = pass;
                        break;
                    case 2:
                        txtHost2.Text = host;
                        txtPort2.Text = port;
                        txtUser2.Text = user;
                        txtPass2.Text = pass;
                        break;
                    case 3:
                        txtHost3.Text = host;
                        txtPort3.Text = port;
                        txtUser3.Text = user;
                        txtPass3.Text = pass;
                        break;
                    case 4:
                        txtHost4.Text = host;
                        txtPort4.Text = port;
                        txtUser4.Text = user;
                        txtPass4.Text = pass;
                        break;
                }
            }
            
            return search;

        }

        private void modifyData(string oHost, string oPort, string oUser, string oPass)
        {
            foreach (Settings s in set)
            {
                if (s.host == oHost)
                {
                    s.host = oHost;
                    s.port = oPort;
                    s.user = oUser;
                    s.pass = oPass;
                }
            }
        }
    }
}
