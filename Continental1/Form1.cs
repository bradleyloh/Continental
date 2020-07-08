using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;         //Usage of FireSharp Plugins to connect to Firebase
using FireSharp.Interfaces;
using FireSharp.Response;
using Tulpep.NotificationWindow;     //For windows notification popup

namespace Continental1
{
    public partial class Form1 : Form
    {

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "VAZ9i5Nhzn8elUoVOHXi9VDaeyVLlXLIRpxmP8QN",             //Connection requirements to firebase
            BasePath = "https://continental1.firebaseio.com/"
        };
        IFirebaseClient client;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);

            if (client != null)
            {
                MessageBox.Show("Database connected");
            }
            dt.Columns.Add("PID");                           //For DataGridView display
            dt.Columns.Add("bn");
            dt.Columns.Add("bpm");
            dt.Columns.Add("cmd");
            dt.Columns.Add("pip");
            dt.Columns.Add("spo2");


            dataGridView1.DataSource = dt;
        }

        private async void btn_extract_Click(object sender, EventArgs e)
        {
            int i = 1;
            dt.Rows.Clear();        //Clear before extracting to prevent re-read
            while (true)            //To loop infinite
            {

                try
                {
                    FirebaseResponse resp2 = await client.GetTaskAsync("Patient" + i);          //Extracting command from Firebase
                    Data obj2 = resp2.ResultAs<Data>();


                    DataRow row = dt.NewRow();         //Initialising the Data into DataGridView

                    row["PID"] = obj2.PID;
                    row["bn"] = obj2.bn;
                    row["bpm"] = obj2.bpm;
                    row["cmd"] = obj2.cmd;
                    row["pip"] = obj2.pip;
                    row["spo2"] = obj2.spo2;

                    dt.Rows.Add(row);                 //Execution of Data into DataGridView


                    
                    var popupNotifier = new PopupNotifier();                               //Popup notification 
                    popupNotifier.TitleText = "Patient PID" + obj2.PID;
                    popupNotifier.ContentText = "Issues" + obj2.bn+ '\n' + obj2.bpm + '\n' + obj2.cmd+ '\n' + obj2.pip + '\n' + obj2.spo2;
                    popupNotifier.IsRightToLeft = false;
                    popupNotifier.Popup();



                    i++;

                }
                catch                                //If error occurs
                {
                    await Task.Delay(2000);          //Delay 2seconds 
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        DataTable dt = new DataTable();

        
    }

    internal class Data             //Class declare to store data into variable
    {
        public string PID{ get; set; }
        public string bn { get; set; }
        public string bpm { get; set; }
        public string cmd { get; set; }
        public string pip{get; set; }
        public string spo2 { get; set; }
    }
}
