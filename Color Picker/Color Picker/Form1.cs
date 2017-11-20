using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.IO.Pipes;
using System.Media;

namespace Color_Picker
{
    public partial class Form1 : Form
    {
        string myColours;
        int count = 0;

        public Form1()
        {
            InitializeComponent();

            // Plays and loops smooth Django Reinhardt music while you pick colours, might as well make it interesting!
            System.Media.SoundPlayer mPlayer = new System.Media.SoundPlayer();
            mPlayer.SoundLocation = "Vamp.wav";
            mPlayer.PlayLooping();
        }

        public class myColor
        {
            public string colourCodes { get; set; }
        }

        private void jsonSerialize()
        {
            // Create serialization object 
            myColor colour1 = new myColor();
            colour1.colourCodes = myColours;
            
            // Convert colour1 to json data and save as txt file in the root of the C: drive
            string jsonData = JsonConvert.SerializeObject(colour1);
            File.WriteAllText(@"c:\mycolours.txt", jsonData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void changeColour_Click(object sender, EventArgs e)
        {
            count++;

            // Show colorDialog window
            DialogResult colourPick = colorDialog1.ShowDialog();
            
            // If color is picked and "ok" is clicked within colorDialog
            if (colourPick == DialogResult.OK)
            {
                // Change the colour of the panel
                panel1.BackColor = colorDialog1.Color;

                // Get the hexidecimal code from colorDialog and convert to string
                string hexCode = "#" + (colorDialog1.Color.ToArgb() & 0x00FFFFFF).ToString("X6");

                // convert hexidecimal code to ARGB color, then convert to RGBA
                Color hexColour = ColorTranslator.FromHtml(hexCode);
                string rgbaCode = String.Format("{0},{1},{2},{3}", hexColour.R, hexColour.G, hexColour.B, hexColour.A);
                if (count > 1)
                {
                    myColours += ", ";
                }

                // Add hexCode and rgbaCode to myColours string
                myColours += "Colour " + count.ToString() + ": " + rgbaCode + "/" + hexCode;

                // Display hexidecimal and rgba codes in list boxes
                listBox1.Items.Add(count.ToString() + " - " + hexCode);
                listBox3.Items.Add(count.ToString() + " - " + rgbaCode);
            }
        }

        private void serializeColor_Click(object sender, EventArgs e)
        {
            // Initiate serialization on click
            jsonSerialize();
            label4.Text = "File saved to C:/mycolours.txt";      
        }
        
        private void exitButton_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }
        
        private void clientSend_Click(object sender, EventArgs e)
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("colourpipe", PipeDirection.Out))
            {
                // Wait for a client to connect
                listBox2.Items.Add("Waiting for client connection...");
                pipeServer.WaitForConnection();
                listBox2.Items.Add("Client connected!");
                try
                {
                    // Reads myColours string and sends to client
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        sw.WriteLine(myColours);
                        listBox2.Items.Add("Colour codes sent to client :)");
                    }
                }
                catch (IOException x) 
                {
                    // Catch if the pipe is broken or disconnected.
                    listBox2.Items.Add(x.Message);
                }
            }
        }
    }
}
