using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Net;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace salesdesktop
{
    public partial class fcaritoko : DevExpress.XtraEditors.XtraForm
    {

        DataSet datas;
        public int Getokmode { get; set; } = 0;
        public String Getkode { get; set; }="";

        String mlatorigin = "";
        String mlongorigin = "";

        myclass classapi = new myclass();

        public fcaritoko(String mlat,String mlong)
        {

            mlatorigin = mlat;
            mlongorigin = mlong;

            InitializeComponent();
        }

        async Task<bool> Loadgrid(bool modefilter)
        {


            OleDbConnection cn = new OleDbConnection();

            try
            {
                cn = myclass.open_conn();

                if (cn != null && cn.State != ConnectionState.Closed)
                {

                    String sqlcari = @"select convert(int,SUBSTRING(a.BusinessPartnerId,2,len(a.BusinessPartnerId))) as xnum,a.BusinessPartnerId as kode,c.Name as nama,b.Address as alamat,pr.FirstName as sales
                        from WTL2020_m1company.Shared.Customers a inner join WTL2020_m1company.Shared.BusinessAddresses b
                        on a.BusinessPartnerId=b.BusinessPartnerId inner join WTL2020_m1company.Shared.BusinessPartners c
                        on a.BusinessPartnerId=c.BusinessPartnerId inner join WTL2020_m1company.Shared.Persons pr
                        on a.SalesmanId=pr.PersonId where len(a.RowId)>0 and SUBSTRING(a.BusinessPartnerId,2,1) in ('1','2','3','4','5','6','7','8','9','0') AND SUBSTRING(a.BusinessPartnerId,LEN(a.BusinessPartnerId),1) in ('1','2','3','4','5','6','7','8','9','0') ";

                    if (modefilter)
                    {

                        if (tkode.EditValue!=null)
                        {
                            if (tkode.EditValue.ToString().Length > 0)
                            {
                                sqlcari = String.Format(" {0} and a.BusinessPartnerId like '{1}%'", sqlcari, tkode.EditValue.ToString().ToUpper());
                            }
                        }


                        if (tnama.EditValue != null)
                        {
                            if (tnama.EditValue.ToString().Length > 0)
                            {
                                sqlcari = String.Format(" {0} and c.Name like '%{1}%'", sqlcari, tnama.EditValue.ToString().ToUpper());
                            }
                        }


                        if (talamat.EditValue != null)
                        {
                            if (talamat.EditValue.ToString().Length > 0)
                            {
                                sqlcari = String.Format(" {0} and b.Address like '%{1}%'", sqlcari, talamat.EditValue.ToString().ToUpper());
                            }
                        }

                        if (tsales.EditValue != null)
                        {
                            if (tsales.EditValue.ToString().Length > 0)
                            {
                                sqlcari = String.Format(" {0} and pr.FirstName like '%{1}%'", sqlcari, tsales.EditValue.ToString().ToUpper());
                            }
                        }
                            

                    }else
                    {
                        sqlcari = String.Format(" {0} a.BusinessPartnerId= 'xa2330kf'", sqlcari);
                    }

                    

                    sqlcari = String.Format(" {0} order by CONVERT(numeric,SUBSTRING(a.BusinessPartnerId,2,len(a.BusinessPartnerId))) desc", sqlcari);

                    datas = new DataSet();
                    datas = myclass.GetdataSet(sqlcari, cn);


                    grid1.Invoke(new Action(() => grid1.DataSource = datas.Tables[0]));

                    //ErrorCode = -2147217913
                    //Message = "Conversion failed when converting the nvarchar value '\\8847' to data type int."
                }
            }

            catch (Exception ex)
            {

               

                if (cn != null)
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }

                Console.WriteLine(String.Format("Something went wrong {0}", ex.Message.ToString()));
            }


            await Task.Delay(200);

            return true;


        }


        private async void simpleButton1_Click(object sender, EventArgs e)
        {

            if (tkode.EditValue== null &&
                    tnama.EditValue == null &&
                        talamat.EditValue == null &&
                            tsales.EditValue == null)
            {
                MessageBox.Show("Kriteria filter harus diisi...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            await Task.WhenAll(Loadgrid(true));

        }

        private async void fcaritoko_Shown(object sender, EventArgs e)
        {
            await Task.WhenAll(Loadgrid(false));
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {

            if (datas.Tables[0].Rows.Count <= 0) { return; }

            string noid = datas.Tables[0].Rows[this.BindingContext[this.datas.Tables[0]].Position]["kode"].ToString();

            fmodenoo objfrmHome = new fmodenoo();
            objfrmHome.ShowDialog();

            int ookks = objfrmHome.Getokmode;

            if (ookks != 0)
            {
                if (ookks==1)
                {
                    String kodeawal = noid.Substring(0, 1);
                    String kodeselanjutnya = noid.Substring(1, (noid.Length - 1));
                    int kodeakhir = int.Parse(kodeselanjutnya) + 1;

                    String kodebaru = String.Format("{0}{1}", kodeawal, kodeakhir);

                    Getokmode = ookks;
                    Getkode = kodebaru;

                }
                else
                {
                    Getokmode = ookks;
                    Getkode = noid;
                }

                this.Close();

            }

        }

        private async void tkode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                await Task.WhenAll(Loadgrid(true));
            }
        }

        private async void tnama_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await Task.WhenAll(Loadgrid(true));
            }
        }

        private async void talamat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await Task.WhenAll(Loadgrid(true));
            }
        }

        private async void tsales_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await Task.WhenAll(Loadgrid(true));
            }
        }

        private async void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            if (datas.Tables[0].Rows.Count <= 0) { return; }

            splashScreenManager1.ShowWaitForm();

            string noid = datas.Tables[0].Rows[this.BindingContext[this.datas.Tables[0]].Position]["kode"].ToString();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            String apikeysimpen = Properties.Settings.Default.apikey;

            //  get dan save token
            if (apikeysimpen.Length == 0)
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var urllogin = string.Format("{0}/loginsink", classapi.geturlapi());

                dynamic dynamicJsonlogin = new ExpandoObject();
                dynamicJsonlogin.email = "admsinkserver";
                dynamicJsonlogin.password = "rahasiasinkserver123*";

                string jsonlogin = "";
                jsonlogin = JsonConvert.SerializeObject(dynamicJsonlogin);
                var objClintlogin = new HttpClient();
                HttpResponseMessage responselogin = await objClintlogin.PostAsync(urllogin, new StringContent(jsonlogin, System.Text.Encoding.UTF8, "application/json"));

                if (responselogin.Content != null)
                {

                    var dataloginnol = await responselogin.Content.ReadAsStringAsync();

                    var tokenlogin = JsonConvert.DeserializeObject<Clstoken>(dataloginnol);

                    Properties.Settings.Default.apikey = tokenlogin.token;
                    Properties.Settings.Default.Save();

                }
                else
                {
                    return;
                }


            }
            // akhir get dan save token

            var urlgetdatakoordinat = string.Format("{0}/getrequest_coordtoko", classapi.geturlapi());

            var jsonString = JsonConvert.SerializeObject(new { kodetoko = noid });

            var clientHandler_alltoko = new HttpClientHandler();
            var objClint_alltoko = new HttpClient(clientHandler_alltoko);

            objClint_alltoko.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikeysimpen);

            HttpResponseMessage response_alltoko = await objClint_alltoko.PostAsync(urlgetdatakoordinat, new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"));

            if (response_alltoko.Content != null)
            {

                var datanoo = await response_alltoko.Content.ReadAsStringAsync();
                var detailnoo = JsonConvert.DeserializeObject<List<Clskoordinat>>(datanoo);

                if (detailnoo.Count > 0)
                {

                    String newlat="";
                    String newlong="";

                    foreach (var datas in detailnoo)
                    {
                        newlat = datas.mlat.ToString();
                        newlong = datas.mlong.ToString();
                    }

                    if (newlat.Length==0)
                    {
                        String messgextra = "Coordinat belum ada";
                        XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                        args.AutoCloseOptions.Delay = 3000;
                        args.Caption = "Get coordinat..";
                        args.Text = messgextra;
                        args.Buttons = new DialogResult[] { DialogResult.OK };
                        args.DefaultButtonIndex = 0;
                        args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                        XtraMessageBox.Show(args).ToString();
                        return;
                    }

                    newlat = newlat.Replace(",", ".");
                    newlong = newlong.Replace(",", ".");

                    mlatorigin = mlatorigin.Replace(",", ".");
                    mlongorigin = mlongorigin.Replace(",", ".");

                    String urlmaps = String.Format("https://www.google.com/maps/dir/?api=1&origin={0}%2C{1}&destination={2}%2C{3}&travelmode=driving", newlat, newlong,mlatorigin, mlongorigin);

                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = urlmaps,
                        UseShellExecute = true
                    });

                    splashScreenManager1.CloseWaitForm();

                }
                else
                {
                    splashScreenManager1.CloseWaitForm();

                    
                    String messgextra = "Toko belum ada di server";
                    XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                    args.AutoCloseOptions.Delay = 3000;
                    args.Caption = "Get coordinat..";
                    args.Text = messgextra;
                    args.Buttons = new DialogResult[] { DialogResult.OK };
                    args.DefaultButtonIndex = 0;
                    args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                    XtraMessageBox.Show(args).ToString();

                }
            }


        }

        public class Clstoken { public string token { get; set; } }

        public class Clskoordinat
        {
            public string mlat { get; set; }
            public string mlong { get; set; }
        }

        private void grid1_Click(object sender, EventArgs e)
        {

        }
    }

    
    
}