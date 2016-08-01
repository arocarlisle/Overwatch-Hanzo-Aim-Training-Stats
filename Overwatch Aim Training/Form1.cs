using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RamGecTools;
using ScreenShotDemo;

namespace Overwatch_Aim_Traning
{
    public partial class Form1 : Form
    {
        //MouseHook
        MouseHook mouseHook = new MouseHook();

        //Add hero 'Hanzo' with fire delay of 500
        Hero hero = new Hero("Hanzo", 500);

        //Resolution Vars
        int resWidth = 1920;
        int resHeigth = 1080;

        //Stuff
        int scanCycles = 0;
        float kills = 0;
        float shots = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Set Log Box
            textBox1.ReadOnly = true;

            //Add Hook functions
            mouseHook.LeftButtonUp += new MouseHook.MouseHookCallback(hookLeftUp);

            //Install Hook
            log("Installing Mouse Hook...\n");
            mouseHook.Install();
            log("Mouse Hook installed\n");

            //Add weapon ready timer
            timer1.Interval = hero.fireDelay;

        }

        public void hookLeftUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (hero.weaponReady && isInGame())
            {
                log("Tracked Shot\n");
                shots++;
                updateStats();

                //Start "Kill Scanner Timer"
                timer2.Start();

                //Make weapon unready
                hero.weaponReady = false;
                timer1.Start();
            }
        }

        public bool isKill()
        {
            //Take screen shot
            ScreenCapture sc = new ScreenCapture();
            Image img = sc.CaptureScreen();

            using (Bitmap bmp = new Bitmap(img))
            {
                //Get color of hitmarker location (20 pixels above)
                //Red Skull means you killed some one
                Color skullClr = bmp.GetPixel(resWidth / 2, resHeigth / 2 - 20);
                int skullRed = skullClr.R;
                int skullGreen = skullClr.G;
                int skullBlue = skullClr.B;

                //Clear img`s memory
                //I dont know how to do it using 'using'
                img.Dispose();

                //Check whether it`s red (enough)
                //Red Skull means Kill
                if (skullRed >= 230 && skullGreen >= 70 && skullBlue >= 100)
                {
                    return true;
                }

            }        
            return false;
        }

        public bool isInGame()
        {
            //Take screen shot
            ScreenCapture sc = new ScreenCapture();
            Image img = sc.CaptureScreen();

            using (Bitmap bmp = new Bitmap(img))
            {
                //Check if there is a green dot (crosshair)
                //No Crosshair means not in Game
                Color crossClr = bmp.GetPixel(resWidth / 2, resHeigth / 2);
                int crossRed = crossClr.R;
                int crossGreen = crossClr.G;
                int crossBlue = crossClr.B;

                //Clear img`s memory
                //I dont know how to do it using 'using'
                img.Dispose();

                if (crossRed == 0 && crossGreen == 255 && crossBlue == 0)
                {
                    return true;
                }

            }

            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Check if heros weapon can be used
            if (!hero.weaponReady)
            {
                hero.weaponReady = true;
                timer1.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Check if you killed someone 3 times after click
            scanCycles++;

            if (isKill())
            {
                log("Tracked Kill\n");
                scanCycles = 0;
                kills++;
                updateStats();
                timer2.Stop();

            }
            else if(scanCycles >= 3)
            {
                scanCycles = 0;
                timer2.Stop();
            }

        }

        private void textBox1_VisibleChanged(object sender, EventArgs e)
        {
            //Do AutoScroll on Text Change
            if (textBox1.Visible)
            {
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.ScrollToCaret();
            }
        }

        public void log(string s)
        {
            DateTime time = DateTime.Now;
            textBox1.AppendText("[" + time.ToString("H:mm:ss")  + "] " + s);
        }

        public void updateStats()
        {
            label1.Text = "Shots: " + shots;
            label2.Text = "Kills: " + kills;
            label4.Text = "Accuracy: " + (kills / shots) * 100 + "%";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex > 0)
            {
                //I will add support for other displays later
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Uninstall
            mouseHook.Uninstall();

            //Install Hook
            log("Installing Mouse Hook...\n");        
            mouseHook.Install();
            log("Mouse Hook installed\n");
        }
    }
}
