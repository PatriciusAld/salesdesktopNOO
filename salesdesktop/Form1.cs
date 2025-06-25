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
using System.Timers;
using System.Data.OleDb;
using Microsoft.Win32;
using System.Net;
using System.Drawing.Imaging;

namespace salesdesktop
{
    public partial class Form1 : Form
    {
      

        DataSet datas;
        BindingSource bs1;
        myclass classapi = new myclass();
        System.Timers.Timer _mytimer;

        bool hasilimagektp;
        bool hasilimagetoko;
        bool hasilimagepemilik;
        //  bool moderefresh = false;

        private static List<Task> TaskList = new List<Task>();

        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private async void ceknewnoo(object sender, EventArgs e)
        {

            if (_mytimer.Enabled)
            {
                _mytimer.Stop();
            }

            toolStripLabel1.Visible = true;
            OleDbConnection cn = new OleDbConnection();

            try
            {
                
                cn = myclass.open_conn();

                if (cn != null && cn.State != ConnectionState.Closed)
                {

                    String sql = "select count(nopengajuan) as jml from db_penghubung.dbo.tbnoo_sync where kodetoko='@BARU' or kodetoko = '-' or kodetoko is null or len(kodetoko) = 0";

                    using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                    {
                        OleDbDataReader drd = cmd.ExecuteReader();

                        int adabaru = 0;
                        while (drd.Read())
                        {
                            adabaru = int.Parse(drd[0].ToString()) ;
                        }

                        if (adabaru >0)
                        {
                            notifyIcon1.ShowBalloonTip(5000, "NOTED", string.Format("[ {0} ] Noo baru telah diterima..", adabaru), ToolTipIcon.Info);
                        }

                        await Task.WhenAll(Loadgrid(cn));


                    }

                }

            


            }
            catch (Exception ex)
            {
               

                toolStripLabel1.Visible = false;

                if (cn != null)
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }

                Console.WriteLine(String.Format("Something went wrong {0}", ex.Message.ToString()));
            }
            finally
            {
               

                toolStripLabel1.Visible = false;

                if (cn != null)
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }

                Console.WriteLine("The 'try catch' is finished.");

                _mytimer.Start();

            }

            
        }

        async Task<bool> Loadgrid(OleDbConnection cn)
        {


                string sql = @"select  a.tglpengajuan as tglpengajuan,p.FirstName as namasales,a.namatoko,a.alamattoko,a.nopengajuan,a.mlat,a.mlong,a.fotoktp,a.fototoko,a.fotopemilik,a.jenis_sales
                from db_penghubung.dbo.tbnoo_sync a inner
                join WTL2020_m1company.Shared.Persons p on a.kdsales = p.PersonId
                where kodetoko='@BARU' or kodetoko = '-' or kodetoko is null or len(kodetoko) = 0 order by a.jenis_sales,a.tglpengajuan";

                datas = new DataSet();
                datas = myclass.GetdataSet(sql, cn);

                bs1 = new BindingSource();
                bs1.DataSource = datas.Tables[0];
                bn1.Invoke(new Action(() => bn1.BindingSource = bs1 ));

                grid1.Invoke(new Action(() => grid1.DataSource = bs1 ));

                await Task.Delay(200);


            return true;


        }

        async Task<bool> Loadimages(String fotoktp,String fototoko,String fotopemilik)
        {

            String serverUrlprofil = "https://salesmobile.grandwater.co.id/storage/foto_toko";

            String linkfotoktp = String.Format("{0}/{1}", serverUrlprofil, fotoktp);
            String linkfototoko = String.Format("{0}/{1}", serverUrlprofil, fototoko);
            String linkfotopemilik = String.Format("{0}/{1}", serverUrlprofil, fotopemilik);

        //    savefotofromurl(linkfotoktp, "ktp");
        //    savefotofromurl(linkfototoko, "toko");
        //    savefotofromurl(linkfotopemilik, "pemilik");

            
            System.Drawing.Image imagektp = DownloadImageFromUrl(linkfotoktp);
            System.Drawing.Image imagetoko = DownloadImageFromUrl(linkfototoko);
            System.Drawing.Image imagepemilik = DownloadImageFromUrl(linkfotopemilik);


            string rootPath = AppDomain.CurrentDomain.BaseDirectory;

            if (imagektp == null)
            {
                hasilimagektp = false;
            }
            else
            {
                string fileNamektp = System.IO.Path.Combine(String.Format(@"{0}",rootPath), "ktp.jpg");
                imagektp.Save(fileNamektp);

                hasilimagektp = true;

            }

            if (imagetoko == null)
            {
                hasilimagetoko = false;
            }
            else
            {

                string fileNametoko = System.IO.Path.Combine(String.Format(@"{0}", rootPath), "toko.jpg");
                imagetoko.Save(fileNametoko);

                hasilimagetoko = true;

            }


            if (imagepemilik == null)
            {
                hasilimagepemilik = false;
            }
            else
            {

                string fileNamepemilik = System.IO.Path.Combine(String.Format(@"{0}", rootPath), "pemilik.jpg");
                imagepemilik.Save(fileNamepemilik);

                hasilimagepemilik = true;

            }
            

            await Task.Delay(10);


            return true;


        }

        private void setTimers(String stat, object sender)
        {

            if (stat == "stop")
            {
                if (_mytimer.Enabled)
                {
                    _mytimer.Stop();
                }
            }
            else
            {
                if (!_mytimer.Enabled)
                {
                    _mytimer.Start();
                }
            }

            
        }

        public System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image imagess = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 8000;  //30.000

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                imagess = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception except)
            {
                Console.WriteLine(String.Format("Something went wrong {0}", except.Message.ToString()));
                return null;
            }

            return imagess;
        }

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Click += notifyIcon1_Click;

            //    if (rkApp.GetValue("salesdesktop") == null)
            //   {
            rkApp.DeleteValue("salesdesktop", false);
            rkApp.SetValue("salesdesktop", Application.ExecutablePath);
        //    }
            

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            e.Cancel = true;
            notifyIcon1.Visible = true;
            Hide();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            
            Show();
            notifyIcon1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            notifyIcon1.Visible = false;
            toolStripLabel1.Visible = false;

            _mytimer = new System.Timers.Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            _mytimer.AutoReset = true;
            _mytimer.Elapsed += new System.Timers.ElapsedEventHandler(ceknewnoo);
            _mytimer.Start();

        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
           
            if (datas.Tables[0].Rows.Count <= 0) { return; }

            setTimers("stop", sender);
            splashScreenManager1.ShowWaitForm();

            string noid = datas.Tables[0].Rows[bs1.Position]["nopengajuan"].ToString();
            String mlat = datas.Tables[0].Rows[bs1.Position]["mlat"].ToString();
            String mlong = datas.Tables[0].Rows[bs1.Position]["mlong"].ToString();

            String fotoktp = datas.Tables[0].Rows[bs1.Position]["fotoktp"].ToString();
            String fototoko = datas.Tables[0].Rows[bs1.Position]["fototoko"].ToString();
            String fotopemilik = datas.Tables[0].Rows[bs1.Position]["fotopemilik"].ToString();
            String jenissales = datas.Tables[0].Rows[bs1.Position]["jenis_sales"].ToString();

           // await Task.WhenAll(Loadimages(fotoktp,fototoko,fotopemilik));

            splashScreenManager1.CloseWaitForm();

            XtraForm1 objfrmHome = new XtraForm1(noid,mlat,mlong,hasilimagektp,hasilimagetoko,hasilimagepemilik,jenissales);
            objfrmHome.ShowDialog();


            bool ookks = objfrmHome.Getok;

            if (ookks)
            {
                ceknewnoo(sender, null);
            }
            else
            {
                setTimers("start", sender);
            }

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
           
            ceknewnoo(sender,e);
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            
            Show();
            notifyIcon1.Visible = false;
        }

        private void grid1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
