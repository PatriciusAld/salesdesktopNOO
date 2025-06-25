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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Diagnostics;
using System.Data.OleDb;
using System.Net;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

using System.IO;

namespace salesdesktop
{

    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {


        string setnopengajuan = "";
        String mlat;
        String mlong;
        String idkab = "";
        String idprop = "";

        bool fotoktp;
        bool fototoko;
        bool fotopemilik;
        String kdsales;
        bool noomodenew = true;
        String jenissales;

        String fotoktp_url;
        String fototoko_url;
        String fotopemilik_url;

        public bool Getok { get; set; } = false;



        private async void loaddata0()
        {
            await Task.WhenAll(loaddata());
        }

        async Task<bool> loaddata()
        {

            String latnow = mlat.Replace(".", ",");
            String longnow = mlong.Replace(".", ",");

            PointLatLng points = new PointLatLng(Convert.ToDouble(latnow), Convert.ToDouble(longnow));

            tmaps.Position = points;


            GMapMarker marker = new GMarkerGoogle(points, GMarkerGoogleType.red_dot);

            GMapOverlay markers = new GMapOverlay("markers");
            markers.Markers.Add(marker);
            tmaps.Overlays.Add(markers);

            OleDbConnection cn = new OleDbConnection();

            try
            {
                cn = myclass.open_conn();

                if (cn != null && cn.State != ConnectionState.Closed)
                {


                    String sqlarea = @"select idkec,namakec + ' ( ' + namakab + ' - ' + namaprop + ' ) ' as areagabung 
                            from db_penghubung.dbo.v_areas";

                    DataSet dsarea = new DataSet();
                    dsarea = myclass.GetdataSet(sqlarea, cn);
                    tkelurahan.Invoke(new Action(() => tkelurahan.Properties.DataSource = dsarea.Tables[0]));


                    String sqlklas = @"SELECT CustomerCategoryId as noid,Description as descid
                            FROM [WTL2020_m1company].[Shared].[CustomerCategory]
                            where (IsLowestLevel=1 and Depth=1) or CustomerCategoryId in ('MM INDO','MM ALFA')
                            order by noid";

                    DataSet dsklas = new DataSet();
                    dsklas = myclass.GetdataSet(sqlklas, cn);
                    tklasifikasi.Invoke(new Action(() => tklasifikasi.Properties.DataSource = dsklas.Tables[0]));


                    String sql = String.Format(@"select a.*,p.FirstName as sales from db_penghubung.dbo.tbnoo_sync a inner join 
                        WTL2020_m1company.Shared.Persons p on a.kdsales=p.PersonId
                        where nopengajuan='{0}'", setnopengajuan);

                    using (OleDbCommand cmdd = new OleDbCommand(sql, cn))
                    {
                        using (OleDbDataReader drdd = cmdd.ExecuteReader())
                        {

                            while (drdd.Read())
                            {
                                ttglpengajuan.EditValue = myclass.ConvertDateToString(drdd["tglpengajuan"].ToString(), "dmyhms");
                                tnopengajuan.EditValue = drdd["nopengajuan"].ToString();
                                tsales.EditValue = drdd["sales"].ToString();
                                tnamatoko.EditValue = drdd["namatoko"].ToString();
                                talamattoko.EditValue = drdd["alamattoko"].ToString();
                                tnotelp.EditValue = drdd["notelp"].ToString();
                                tnoktp.EditValue = drdd["noktp"].ToString();
                                tnamaktp.EditValue = drdd["namaktp"].ToString();
                                talamatktp.EditValue = drdd["alamatktp"].ToString();
                                tpribadi.EditValue = drdd["pakaisendiri"].ToString();
                                tpemilik.EditValue = drdd["namapemilik"].ToString();
                                tklasifikasi.EditValue = drdd["klastoko"].ToString();
                                tlimit.EditValue = drdd["limittoko"].ToString();
                                tcall.EditValue = drdd["targetcall"].ToString();
                                thari.EditValue = drdd["harikunjungan"].ToString();
                                talamatmap.EditValue = drdd["alamatsesuaikoor"].ToString();

                                kdsales = drdd["kdsales"].ToString();

                                string rootPath = AppDomain.CurrentDomain.BaseDirectory;
                                myclass classapi = new myclass();
                                String serverUrlprofil = classapi.geturlapiprofil();

                                if (drdd["fotoktp"].ToString().Length > 0)
                                {
                                    fotoktp_url = drdd["fotoktp"].ToString();
                                    pictureEdit1.LoadAsync(String.Format(@"{0}/{1}", serverUrlprofil, drdd["fotoktp"].ToString()));


                                }
                                else
                                {
                                    fotoktp_url = "";
                                    pictureEdit1.LoadAsync(String.Format(@"{0}/tidakadafoto.jpg", rootPath));
                                }


                                if (drdd["fototoko"].ToString().Length > 0)
                                {
                                    fototoko_url = drdd["fototoko"].ToString();
                                    pictureEdit2.LoadAsync(String.Format(@"{0}/{1}", serverUrlprofil, drdd["fototoko"].ToString()));
                                }
                                else
                                {
                                    fototoko_url = "";
                                    pictureEdit2.LoadAsync(String.Format(@"{0}/tidakadafoto.jpg", rootPath));
                                }

                                if (drdd["fotopemilik"].ToString().Length > 0)
                                {
                                    fotopemilik_url = drdd["fotopemilik"].ToString();
                                    pictureEdit3.LoadAsync(String.Format(@"{0}/{1}", serverUrlprofil, drdd["fotopemilik"].ToString()));
                                }
                                else
                                {
                                    fotopemilik_url = "";
                                    pictureEdit3.LoadAsync(String.Format(@"{0}/tidakadafoto.jpg", rootPath));
                                }

                                /*if (fotoktp)
                                {
                                   pictureEdit1.LoadAsync(String.Format(@"{0}\ktp.jpg", rootPath));
                                }
                                else
                                {
                                    pictureEdit1.LoadAsync(String.Format(@"{0}\tidakadafoto.jpg", rootPath));
                                }

                                if (fototoko)
                                {
                                    pictureEdit2.LoadAsync(String.Format(@"{0}\toko.jpg", rootPath));
                                }
                                else
                                {
                                    pictureEdit2.LoadAsync(String.Format(@"{0}\tidakadafoto.jpg", rootPath));
                                }


                                if (fotopemilik)
                                {
                                    pictureEdit3.LoadAsync(String.Format(@"{0}\pemilik.jpg", rootPath));
                                }
                                else
                                {
                                    pictureEdit3.LoadAsync(String.Format(@"{0}\tidakadafoto.jpg", rootPath));
                                } */


                            }

                        }
                    }



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
            finally
            {


                if (cn != null)
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }

                Console.WriteLine("The 'try catch' is finished.");




            }


            await Task.Delay(100);
            return true;


        }

        

        async Task<bool> uploadData()
        {


            if (noomodenew == false)
            {
                if (MessageBox.Show("Yakin akan update toko yang sudah ada ?", "Konfirmasi Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    await Task.Delay(100);
                    return false;

                }
            }

            splashScreenManager1.ShowWaitForm();

            OleDbConnection cn = new OleDbConnection();

            try
            {
                cn = myclass.open_conn();

                if (cn != null && cn.State != ConnectionState.Closed)
                {

                    if (noomodenew)
                    {
                        String hasilcekkode = "";
                        String salesmanId = "";
                        String sqlcekkode = String.Format("SELECT BusinessPartnerId, SalesmanId FROM WTL2020_m1company.Shared.Customers WHERE BusinessPartnerId = '{0}'", tkodetoko.EditValue.ToString().ToUpper());
                        using (OleDbCommand cmdcek = new OleDbCommand(sqlcekkode, cn))
                        {
                            using (OleDbDataReader drd = cmdcek.ExecuteReader())
                            {
                                while (drd.Read())
                                {
                                    hasilcekkode = drd["BusinessPartnerId"].ToString();
                                    salesmanId = drd["SalesmanId"].ToString();
                                }
                            }
                        }
                        if (hasilcekkode.ToString().Length > 0)
                        {

                            splashScreenManager1.CloseWaitForm();

                            MessageBox.Show("Kode toko sudah ada dimobiz...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await Task.Delay(100);
                            return false;
                        }

                    }

                    

                    myclass classapi = new myclass();

                    int unlimit = 0;
                    if (cekunlimited.Checked) { unlimit = 1; }

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

                            apikeysimpen = tokenlogin.token;

                            Properties.Settings.Default.apikey = tokenlogin.token;
                            Properties.Settings.Default.Save();

                        }
                        else
                        {
                            splashScreenManager1.CloseWaitForm();
                            MessageBox.Show("Error get key", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            await Task.Delay(100);
                            return false;
                        }


                    }



                   

                    var url = string.Format("{0}/updatenooupload", classapi.geturlapi());


                        int noorealbaru = 0;
                        if (tmode.EditValue.ToString() != "NOO BARU")
                        {
                            noorealbaru = 1;
                        }

                    string serverUrlProfil = classapi.geturlapiprofil();

                    dynamic dynamicJson = new ExpandoObject();
                    dynamicJson.nopengajuan = tnopengajuan.EditValue.ToString().Trim();
                    dynamicJson.kodetoko = tkodetoko.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.fotoktp_url =  fotoktp_url;
                    dynamicJson.nama = tnamatoko.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.alamat = talamattoko.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.notelp = tnotelp.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.noktp = tnoktp.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.namaktp = tnamaktp.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.alamatktp = talamatktp.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.pakaisendiri = tpribadi.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.klastoko = tklasifikasi.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.limit = tlimit.EditValue.ToString().Replace(",", ".");
                    dynamicJson.tcall = tcall.EditValue.ToString().Replace(",", ".");
                    dynamicJson.harikunjungan = thari.EditValue.ToString().ToUpper().Trim();
                    dynamicJson.adadi_mobiz = noorealbaru;
                    dynamicJson.jenis_sales = jenissales;
                    dynamicJson.kdsales = kdsales;
                    dynamicJson.unlimit = unlimit;


                    // Cek nama toko di tbnoo_sync
                    string querySync = $@"
                        SELECT TOP 1 NamaToko 
                        FROM db_penghubung.dbo.tbnoo_sync 
                        WHERE NamaToko = '{dynamicJson.nama}'";

                    string kdPajakTemp = ""; // Variabel sementara untuk KD_PAJAK

                    using (OleDbCommand cmdSync = new OleDbCommand(querySync, cn))
                    {
                        using (OleDbDataReader readerSync = cmdSync.ExecuteReader())
                        {
                            if (readerSync.Read())
                            {
                                string namaToko = readerSync["NamaToko"].ToString().ToUpper().Trim();
                                if (namaToko == "ALFAMIDI")
                                {
                                    kdPajakTemp = "ALFAMIDI"; // Gunakan kdPajakTemp
                                }
                                else if (dynamicJson.klastoko == "MM INDO")
                                {
                                    kdPajakTemp = "INDO";
                                }
                                else if (dynamicJson.klastoko == "MM ALFA")
                                {
                                    kdPajakTemp = "ALFA";
                                }
                            }
                        }
                    }

                    // Jika KD_PAJAK ditemukan, ambil data dari ms_cust
                    if (!string.IsNullOrEmpty(kdPajakTemp))
                    {
                        string queryCust = $@"
                         SELECT NPWP, NAMA, ALAMAT 
                        FROM db_penghubung.dbo.ms_cust 
                            WHERE KD_PAJAK = '{kdPajakTemp}'";

                        using (OleDbCommand cmdCust = new OleDbCommand(queryCust, cn))
                        {
                            using (OleDbDataReader readerCust = cmdCust.ExecuteReader())
                            {
                                if (readerCust.Read())
                                {
                                    dynamicJson.noktp = readerCust["NPWP"].ToString().ToUpper().Trim();
                                    dynamicJson.namaktp = readerCust["NAMA"].ToString().ToUpper().Trim();
                                    dynamicJson.alamatktp = readerCust["ALAMAT"].ToString().ToUpper().Trim();
                                }
                            }
                        }
                    }


                    string json = "";
                    json = JsonConvert.SerializeObject(dynamicJson);

                    var clientHandler_update = new HttpClientHandler();
                    var objClint_update = new HttpClient(clientHandler_update);

                    objClint_update.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikeysimpen);

                    HttpResponseMessage response_update = await objClint_update.PostAsync(url, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

                    if (response_update.Content != null)
                    {
                        var dataget = await response_update.Content.ReadAsStringAsync();
                        var hasilnon = JsonConvert.DeserializeObject<Statgetapisuccess>(dataget);

                        if (hasilnon.pesan == "ok")
                        {

                            String custcategori1 = "";
                            String custcategori2 = "";
                            String custcategori3 = "";

                            custcategori1 = tklasifikasi.EditValue.ToString().Trim();

                            if (tklasifikasi.EditValue.Equals("MM INDO"))
                            {
                                custcategori1 = "MDRN MKT";
                                custcategori2 = "MM";
                                custcategori3 = "MM INDO";
                            }

                            if (tklasifikasi.EditValue.Equals("MM ALFA"))
                            {
                                custcategori1 = "MDRN MKT";
                                custcategori2 = "MM";
                                custcategori3 = "MM ALFA";
                            }

                            double limit;
                            int unlimited = 0;

                            limit = double.Parse(tlimit.EditValue.ToString());

                            if (cekunlimited.Checked)
                            {
                                unlimited = 1;
                                limit = 0;
                            }



                            if (noomodenew)
                            {

                                String sql = String.Format(@"insert into WTL2020_m1company.Shared.Customers (BusinessPartnerId,SalesmanId,
                                OriginalSalesmanId,CustomerCategoryId,CustCategoryId1,
                                CustCategoryId2,CustCategoryId3,Area1,Area2,Area3,AreaId,CreditLimitUnlimited,CreditLimit,
                                CreditWarningPercentage,IsActive,TaxID,DepartmentId,BillingAddressID,ShippingAddressID,CreditUsed)
                                values('{0}','{1}','{1}','{2}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},80,1,'PPN OUT 11%','UMUM','ADDRESS','ADDRESS',0)",
                                tkodetoko.EditValue.ToString().ToUpper().Trim(), kdsales, custcategori1, custcategori2, custcategori3,
                                idprop, idkab, tkelurahan.EditValue.ToString().Trim(), tkelurahan.EditValue.ToString().Trim(), unlimited, limit.ToString().Replace(",", "."));

                                String sqladdres = String.Format(@"insert into WTL2020_m1company.Shared.BusinessAddresses 
                                ([BusinessPartnerId],[Address],[PersonToContact1],
                                [Phone1],[AliasName],[IsPrimary],[AddressId],[IsActive],[City],[Country])
                                values ('{0}','{1}','{2}','{3}','{4}',1,'ADDRESS',0,'-','{5}')",
                                        tkodetoko.EditValue.ToString().ToUpper().Trim(), talamattoko.EditValue.ToString().ToUpper().Trim(),
                                        tpemilik.EditValue.ToString().ToUpper().Trim(), tnotelp.EditValue.ToString().ToUpper().Trim(),
                                        tnamatoko.EditValue.ToString().ToUpper().Trim(), ttglpengajuan.EditValue.ToString());

                                String sqlper = String.Format(@" insert into WTL2020_m1company.Shared.BusinessPartners (BusinessPartnerId,
                                Name,PaymentTermId,IsActive)
                                values('{0}','{1}','KREDIT30',1)",
                                    tkodetoko.EditValue.ToString().ToUpper().Trim(), tnamatoko.EditValue.ToString().ToUpper().Trim());


                                using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                                {
                                    cmd.ExecuteNonQuery();
                                }

                                using (OleDbCommand cmdadres = new OleDbCommand(sqladdres, cn))
                                {
                                    cmdadres.ExecuteNonQuery();
                                }

                                using (OleDbCommand cmdper = new OleDbCommand(sqlper, cn))
                                {
                                    cmdper.ExecuteNonQuery();

                                }
                                if (dynamicJson.pakaisendiri == "0" && tnoktp.EditValue.ToString().Trim().Length == 16)
                                {
                                  

                                    // Cek apakah KD_PAJAK sudah ada di tabel dbo.ms_cust
                                    String sqlCheckCust = String.Format(@"SELECT COUNT(*) FROM db_penghubung.dbo.ms_cust WHERE KD_PAJAK = '{0}'",
                                        tnoktp.EditValue.ToString().ToUpper().Trim());

                                    int countCust = 0;

                                    using (OleDbCommand cmdCheckCust = new OleDbCommand(sqlCheckCust, cn))
                                    {
                                        countCust = Convert.ToInt32(cmdCheckCust.ExecuteScalar());
                                    }

                                    string alamatCust = talamatktp.EditValue.ToString().ToUpper().Trim();

                                    if (countCust > 0)
                                    {
                                        // Jika KD_PAJAK sudah ada, ambil ALAMAT dari dbo.ms_cust
                                        String sqlGetAlamat = String.Format(@"SELECT ALAMAT FROM db_penghubung.dbo.ms_cust WHERE KD_PAJAK = '{0}'",
                                            tnoktp.EditValue.ToString().ToUpper().Trim());

                                        using (OleDbCommand cmdGetAlamat = new OleDbCommand(sqlGetAlamat, cn))
                                        {
                                            var result = cmdGetAlamat.ExecuteScalar();
                                            if (result != null)
                                            {
                                                alamatCust = result.ToString().ToUpper().Trim(); // Gunakan alamat dari dbo.ms_cust
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Jika KD_PAJAK tidak ada, insert ke dbo.ms_cust
                                        String sqlCustHub = String.Format(@"INSERT INTO db_penghubung.dbo.ms_cust 
                                        (KD_PAJAK, NAMA, ALAMAT, NPWP, SAKTIF, SGABUNG, FROMSFA)
                                          VALUES ('{0}', '{1}', '{2}', '{0}', 1, 0, 1)",
                                            tnoktp.EditValue.ToString().ToUpper().Trim(),
                                            tnamaktp.EditValue.ToString().ToUpper().Trim(),
                                            talamatktp.EditValue.ToString().ToUpper().Trim());

                                        using (OleDbCommand cmdCustHub = new OleDbCommand(sqlCustHub, cn))
                                        {
                                            cmdCustHub.ExecuteNonQuery();
                                        }
                                    }

                                    // Insert ke dbo.ms_cust2 dengan alamat yang sama dari dbo.ms_cust
                                    String sqlCustHub2 = String.Format(@"INSERT INTO db_penghubung.dbo.ms_cust2 
                                        (KD_PAJAK, KD_PROG, ALAMAT, sudahupload)
                                        VALUES ('{0}', '{1}', '{2}', 1)",
                                        tnoktp.EditValue.ToString().ToUpper().Trim(),
                                        tkodetoko.EditValue.ToString().ToUpper().Trim(),
                                        alamatCust);

                                    using (OleDbCommand cmdCustHub2 = new OleDbCommand(sqlCustHub2, cn))
                                    {
                                        cmdCustHub2.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Jika pakaisendiri bukan "0"
                                    if (dynamicJson.klastoko == "MM INDO" || dynamicJson.klastoko == "MM ALFA")
                                    {
                                        string kdPajakSearch = dynamicJson.klastoko == "MM INDO" ? "INDO" : "ALFA";
                                        string kdPajak = "";
                                        string alamatCust = "";

                                        // Tambahan pengecekan khusus untuk Alfamidi
                                        if (dynamicJson.nama == "ALFAMIDI")
                                        {
                                            // Cek data dari tbnoo_sync berdasarkan nama toko
                                            String sqlCheckAlfamidi = String.Format(@"SELECT NamaToko FROM db_penghubung.dbo.tbnoo_sync WHERE NamaToko = '{0}'",
                                                dynamicJson.nama);

                                            using (OleDbCommand cmdCheckAlfamidi = new OleDbCommand(sqlCheckAlfamidi, cn))
                                            {
                                                var result = cmdCheckAlfamidi.ExecuteScalar();
                                                if (result != null) // Jika ditemukan data Alfamidi
                                                {
                                                    // Cari KD_PAJAK = ALFAMIDI di tabel ms_cust
                                                    String sqlGetAlamatAlfamidi = @"SELECT KD_PAJAK, ALAMAT FROM db_penghubung.dbo.ms_cust WHERE KD_PAJAK = 'ALFAMIDI'";

                                                    using (OleDbCommand cmdGetAlamatAlfamidi = new OleDbCommand(sqlGetAlamatAlfamidi, cn))
                                                    {
                                                        using (OleDbDataReader reader = cmdGetAlamatAlfamidi.ExecuteReader())
                                                        {
                                                            if (reader.Read())
                                                            {
                                                                kdPajak = reader["KD_PAJAK"].ToString().ToUpper().Trim(); // Harus ALFAMIDI
                                                                alamatCust = reader["ALAMAT"].ToString().ToUpper().Trim();
                                                            }
                                                        }
                                                    }

                                                    // Insert ke ms_cust2 dengan KD_PAJAK ALFAMIDI
                                                    if (!string.IsNullOrEmpty(kdPajak))
                                                    {
                                                        String sqlInsertAlfamidi = String.Format(@"INSERT INTO db_penghubung.dbo.ms_cust2 
                                                            (KD_PAJAK, KD_PROG, ALAMAT, sudahupload)
                                                            VALUES ('{0}', '{1}', '{2}', 1)",
                                                            kdPajak,
                                                            tkodetoko.EditValue.ToString().ToUpper().Trim(),
                                                            alamatCust);

                                                        using (OleDbCommand cmdInsertAlfamidi = new OleDbCommand(sqlInsertAlfamidi, cn))
                                                        {
                                                            cmdInsertAlfamidi.ExecuteNonQuery();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // Logika default untuk MM INDO dan MM ALFA selain Alfamidi
                                            String sqlCheckPajak = String.Format(@"SELECT KD_PAJAK, ALAMAT FROM db_penghubung.dbo.ms_cust WHERE KD_PAJAK LIKE '%{0}%'",
                                                kdPajakSearch);

                                            using (OleDbCommand cmdCheckPajak = new OleDbCommand(sqlCheckPajak, cn))
                                            {
                                                using (OleDbDataReader reader = cmdCheckPajak.ExecuteReader())
                                                {
                                                    if (reader.Read())
                                                    {
                                                        kdPajak = reader["KD_PAJAK"].ToString().ToUpper().Trim();
                                                        alamatCust = reader["ALAMAT"].ToString().ToUpper().Trim();
                                                    }
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(kdPajak))
                                            {
                                                // Insert ke ms_cust2 dengan KD_PAJAK yang sesuai
                                                String sqlInsertCust2 = String.Format(@"INSERT INTO db_penghubung.dbo.ms_cust2 
                                                    (KD_PAJAK, KD_PROG, ALAMAT, sudahupload)
                                                    VALUES ('{0}', '{1}', '{2}', 1)",
                                                    kdPajak,
                                                    tkodetoko.EditValue.ToString().ToUpper().Trim(),
                                                    alamatCust);

                                                using (OleDbCommand cmdInsertCust2 = new OleDbCommand(sqlInsertCust2, cn))
                                                {
                                                    cmdInsertCust2.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                String sqlcekkode = String.Format("SELECT BusinessPartnerId, SalesmanId FROM WTL2020_m1company.Shared.Customers WHERE BusinessPartnerId = '{0}'", tkodetoko.EditValue.ToString().ToUpper());

                                using (OleDbCommand cmdcekkode = new OleDbCommand(sqlcekkode, cn))
                                {
                                    OleDbDataReader reader = cmdcekkode.ExecuteReader();

                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            string businessPartnerId = reader["BusinessPartnerId"].ToString();
                                            string salesmanId = reader["SalesmanId"].ToString();

                                            if (salesmanId == "K001.SS" || salesmanId == "K002.SS")
                                            {
                                                String sqlupkantor = String.Format(@"update WTL2020_m1company.Shared.Customers
                                                        set  Area1 = '{4}', Area2 = '{5}', Area3 = '{6}', AreaId = '{6}'
                                                        where BusinessPartnerId = '{7}'",
                                                           kdsales, custcategori1, custcategori2, custcategori3,
                                                           idprop, idkab, tkelurahan.EditValue.ToString().Trim(),
                                                           tkodetoko.EditValue.ToString().ToUpper().Trim());

                                                // Update data di tabel Customers
                                                using (OleDbCommand cmd = new OleDbCommand(sqlupkantor, cn))
                                                {
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                            else
                                            {

                                                // Setelah mendapatkan SalesmanId dari database, lanjutkan dengan pengecekan API sales
                                                var urlcheck = string.Format("{0}/sales/{1}", classapi.geturlapi(), salesmanId);
                                                var clientHandler_check = new HttpClientHandler();
                                                HttpClient client = new HttpClient(clientHandler_check);
                                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikeysimpen);

                                                HttpResponseMessage response = await client.GetAsync(urlcheck);
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    var responseData = await response.Content.ReadAsStringAsync();
                                                    var salesResponse = JsonConvert.DeserializeObject<SalesResponse>(responseData);

                                                    if (salesResponse != null && salesResponse.Success)
                                                    {
                                                        // Mendapatkan jenis sales dan salesmanId dari API response
                                                        string jenissales = salesResponse.jnssales;
                                                        string kdsales = salesResponse.kdsales;

                                                        // Query UPDATE berdasarkan jenis sales
                                                        String sqlup = "";

                                                        if (jenissales == "TO")
                                                        {


                                                            sqlup = String.Format(@"update WTL2020_m1company.Shared.Customers
                                                        set Area1 = '{4}', Area2 = '{5}', Area3 = '{6}', AreaId = '{6}'
                                                        where BusinessPartnerId = '{7}'",
                                                               kdsales, custcategori1, custcategori2, custcategori3,
                                                               idprop, idkab, tkelurahan.EditValue.ToString().Trim(),
                                                               tkodetoko.EditValue.ToString().ToUpper().Trim());

                                                            // Update data di tabel Customers
                                                            using (OleDbCommand cmd = new OleDbCommand(sqlup, cn))
                                                            {
                                                                cmd.ExecuteNonQuery();
                                                            }


                                                        }
                                                        else
                                                        {
                                                            sqlup = String.Format(@"update WTL2020_m1company.Shared.Customers
                                                        set SalesmanId = '{0}', OriginalSalesmanId = '{0}',
                                                        CustomerCategoryId = '{1}', CustCategoryId1 = '{1}',
                                                        CustCategoryId2 = '{2}', CustCategoryId3 = '{3}',
                                                        Area1 = '{4}', Area2 = '{5}', Area3 = '{6}', AreaId = '{6}',
                                                        CreditLimitUnlimited ={7},CreditLimit ={8}
                                                        where BusinessPartnerId = '{9}'",
                                                                kdsales, custcategori1, custcategori2, custcategori3,
                                                                idprop, idkab, tkelurahan.EditValue.ToString().Trim(),
                                                                unlimited, limit.ToString().Replace(",", "."),
                                                                tkodetoko.EditValue.ToString().ToUpper().Trim());

                                                            // Update data di tabel Customers
                                                            using (OleDbCommand cmd = new OleDbCommand(sqlup, cn))
                                                            {
                                                                cmd.ExecuteNonQuery();
                                                            }

                                                            // Lakukan update pada tabel BusinessAddresses
                                                            String sqladdresup = String.Format(@"update WTL2020_m1company.Shared.BusinessAddresses 
                                                                set [Address]='{0}',[PersonToContact1]='{1}',[Phone1]='{2}',
                                                                [AliasName]='{3}',[Country]='{4}' where [BusinessPartnerId]='{5}'",
                                                                    talamattoko.EditValue.ToString().ToUpper().Trim(),
                                                                    tpemilik.EditValue.ToString().ToUpper().Trim(), tnotelp.EditValue.ToString().ToUpper().Trim(),
                                                                    tnamatoko.EditValue.ToString().ToUpper().Trim(), ttglpengajuan.EditValue.ToString(),
                                                                    tkodetoko.EditValue.ToString().ToUpper().Trim());

                                                            using (OleDbCommand cmdadres = new OleDbCommand(sqladdresup, cn))
                                                            {
                                                                cmdadres.ExecuteNonQuery();
                                                            }

                                                            // Lakukan update pada tabel BusinessPartners
                                                            String sqlperup = String.Format(@"update WTL2020_m1company.Shared.BusinessPartners 
                                                            set Name='{0}' where BusinessPartnerId='{1}'",
                                                            tnamatoko.EditValue.ToString().ToUpper().Trim(), tkodetoko.EditValue.ToString().ToUpper().Trim());

                                                            using (OleDbCommand cmdper = new OleDbCommand(sqlperup, cn))
                                                            {
                                                                cmdper.ExecuteNonQuery();
                                                            }

                                                        }



                                                    }
                                                    else
                                                    {
                                                        splashScreenManager1.CloseWaitForm();
                                                        MessageBox.Show("Sales tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    splashScreenManager1.CloseWaitForm();
                                                    MessageBox.Show("Gagal memanggil API sales.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    return false;
                                                }

                                            }



                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Data BusinessPartnerId tidak ditemukan di database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }
                                }


                            }


                            String sqlupdatenoo = String.Format(@"
                            UPDATE db_penghubung.dbo.tbnoo_sync 
                                SET kodetoko = '{0}', namatoko = '{1}', alamattoko = '{2}', notelp = '{3}', klastoko = '{4}', limittoko = {5}, targetcall = {6}, harikunjungan = '{7}',noktp = '{8}', 
                                namaktp = '{9}', alamatktp = '{10}', pakaisendiri = '{11}' WHERE nopengajuan = '{12}'", tkodetoko.EditValue.ToString().ToUpper().Trim(),tnamatoko.EditValue.ToString().ToUpper().Trim(),
                                talamattoko.EditValue.ToString().ToUpper().Trim(),tnotelp.EditValue.ToString().ToUpper().Trim(), tklasifikasi.EditValue.ToString().ToUpper().Trim(), tlimit.EditValue.ToString().Replace(",", "."),
                                tcall.EditValue.ToString().Replace(",", "."), thari.EditValue.ToString().ToUpper().Trim(), tnoktp.EditValue.ToString().ToUpper().Trim(),   
                                tnamaktp.EditValue.ToString().ToUpper().Trim(), talamatktp.EditValue.ToString().ToUpper().Trim(),tpribadi.EditValue.ToString().ToUpper().Trim(),  
                                tnopengajuan.EditValue.ToString()
                             );

                            // Eksekusi query
                            using (OleDbCommand cmdnoo = new OleDbCommand(sqlupdatenoo, cn))
                            {
                                cmdnoo.ExecuteNonQuery();
                            }


                            // kirim notif

                            var urlmessage = string.Format("{0}/getfcmkey_sales", classapi.geturlapi());

                            dynamic dynamicJsonmessage = new ExpandoObject();
                            dynamicJsonmessage.kdsales = kdsales;
                            dynamicJsonmessage.namatoko = tnamatoko.EditValue.ToString().ToUpper().Trim();

                            string jsonmessage = "";
                            jsonmessage = JsonConvert.SerializeObject(dynamicJsonmessage);

                            var objClintmessage = new HttpClientHandler();
                            var objClintmessageup = new HttpClient(objClintmessage);

                            objClintmessageup.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikeysimpen);

                            HttpResponseMessage responsemessage = await objClintmessageup.PostAsync(urlmessage, new StringContent(jsonmessage, System.Text.Encoding.UTF8, "application/json"));


                            if (responsemessage.Content != null)
                            {

                                var datamessage = await responsemessage.Content.ReadAsStringAsync();

                                var tokenmessage = JsonConvert.DeserializeObject<ClsSuccessapi>(datamessage);

                                if (tokenmessage.success == false)
                                {
                                    splashScreenManager1.CloseWaitForm();
                                    MessageBox.Show("Send Message Error", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    await Task.Delay(100);
                                    return false;
                                }

                            }

                            // akhir kirim notif

                            splashScreenManager1.CloseWaitForm();

                            Getok = true;

                            String messgextra = "";
                            if (noomodenew)
                            {
                                messgextra = "Toko berhasil diupload dan ditambahkan ke Mobiz..";
                            }
                            else
                            {
                                messgextra = "Toko berhasil diupload dan ditambahkan, perubahan diMobiz telah dilakukan..";
                            }

                            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                            args.AutoCloseOptions.Delay = 3000;
                            args.Caption = "New Toko..";
                            args.Text = messgextra;
                            args.Buttons = new DialogResult[] { DialogResult.OK };
                            args.DefaultButtonIndex = 0;
                            args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                            XtraMessageBox.Show(args).ToString();

                            this.Close();


                        }
                        else
                        {
                            splashScreenManager1.CloseWaitForm();
                            MessageBox.Show(hasilnon.pesan, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            await Task.Delay(100);
                            return false;
                        }

                    }


                }

            }
            catch (Exception ex)
            {

                splashScreenManager1.CloseWaitForm();

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

                if (cn != null)
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }

                Console.WriteLine("The 'try catch' is finished.");




            }


            await Task.Delay(100);
            return true;
        }

        public XtraForm1(string nopengajuan, String mlats, String mlongs, bool fotoktps, bool fototokos, bool fotopemiliks, String jenissaless)
        {
            setnopengajuan = nopengajuan;
            mlat = mlats;
            mlong = mlongs;
            fotoktp = fotoktps;
            fototoko = fototokos;
            fotopemilik = fotopemiliks;
            jenissales = jenissaless;

            InitializeComponent();
        }

        private void XtraForm1_Shown(object sender, EventArgs e)
        {

            noomodenew = true;

            GMapProviders.GoogleMap.ApiKey = "AIzaSyD1R7mHtx3FcfIKPzJqL666Rdrx_0W2Yec";
            tmaps.MapProvider = GMapProviders.GoogleMap;
            tmaps.MinZoom = 5;
            tmaps.MaxZoom = 100;
            tmaps.Zoom = 15;


            //tfoto1.ImageLocation = fotoktp;
            //tfoto2.ImageLocation = fototoko;
            //tfoto3.ImageLocation = fotopemilik;

            //  tfoto1.LoadAsync(String.Format(@"{0}", fotoktp));
            //  tfoto2.LoadAsync(String.Format(@"{0}", fototoko));
            //  tfoto3.LoadAsync(String.Format(@"{0}", fotopemilik));

            loaddata0();

        }

        private void tmaps_DoubleClick(object sender, EventArgs e)
        {

            if (setnopengajuan.Length == 0)
            {
                return;
            }

            String newlat = mlat.ToString();
            String newlong = mlong.ToString();

            newlat = newlat.Replace(",", ".");
            newlong = newlong.Replace(",", ".");

            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = String.Format("https://www.google.com/maps/search/?api=1&query={0}%2C{1}", newlat, newlong),
                UseShellExecute = true
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (setnopengajuan.Length == 0)
            {
                return;
            }

            // Mendapatkan nilai latitude dan longitude
            String newlat = mlat.ToString();
            String newlong = mlong.ToString();

            // Mengganti koma dengan titik untuk kompatibilitas URL
            newlat = newlat.Replace(",", ".");
            newlong = newlong.Replace(",", ".");

            // Membuka URL dengan latitude dan longitude sebagai parameter query string
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = String.Format("https://grandwater.co.id/merchant-maps/?long={0}&lat={1}", newlong, newlat),
                UseShellExecute = true
            });
        }


        private void tfoto1_DoubleClick(object sender, EventArgs e)
        {

            if (setnopengajuan.Length == 0)
            {
                return;
            }

            XtraForm2 objfrmHome = new XtraForm2("ktp", fotoktp_url);
            objfrmHome.ShowDialog();
        }

        private void tfoto2_DoubleClick(object sender, EventArgs e)
        {

            if (setnopengajuan.Length == 0)
            {
                return;
            }

            XtraForm2 objfrmHome = new XtraForm2("toko", fototoko_url);
            objfrmHome.ShowDialog();
        }

        private void tfoto3_DoubleClick(object sender, EventArgs e)
        {

            if (setnopengajuan.Length == 0)
            {
                return;
            }

            XtraForm2 objfrmHome = new XtraForm2("pemilik", fotopemilik_url);
            objfrmHome.ShowDialog();
        }

        private void cekunlimited_CheckedChanged(object sender, EventArgs e)
        {
            if (cekunlimited.Checked)
            {
                cekunlimited.Text = "Unlimited Limit ?, IYA";
                tlimit.EditValue = 0;
                tlimit.Properties.ReadOnly = true;
            }
            else
            {
                cekunlimited.Text = "Unlimited Limit ?, Tidak";
                tlimit.Properties.ReadOnly = false;
            }
        }

        private async void btsimpan_Click(object sender, EventArgs e)
        {

            if (pictureEdit1.LoadInProgress)
            {
                MessageBox.Show("Foto pelanggan [ktp] sedang diproses...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (pictureEdit2.LoadInProgress)
            {
                MessageBox.Show("Foto pelanggan [toko] sedang diproses...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (pictureEdit3.LoadInProgress)
            {
                MessageBox.Show("Foto pelanggan [pemilik] sedang diproses...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (tkodetoko.EditValue == null || tkodetoko.EditValue.ToString().Length == 0)
            {
                MessageBox.Show("Kode harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (idprop.Length == 0)
            {
                MessageBox.Show("Area harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (tnamatoko.EditValue == null || tnamatoko.EditValue.ToString().Length == 0)
            {
                MessageBox.Show("Nama toko harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (talamattoko.EditValue == null || talamattoko.EditValue.ToString().Length == 0)
            {
                MessageBox.Show("Alamat toko harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            if (tnotelp.EditValue == null || tnotelp.EditValue.ToString().Length == 0)
            {
                MessageBox.Show("Telp toko harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

           




            if (cekunlimited.Checked == false)
            {
                if (tlimit.EditValue == null || double.Parse(tlimit.EditValue.ToString()) == 0 || double.Parse(tlimit.EditValue.ToString()) < 0)
                {
                    MessageBox.Show("Limit harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (tcall.EditValue == null || double.Parse(tcall.EditValue.ToString()) == 0 || double.Parse(tcall.EditValue.ToString()) < 0)
            {
                MessageBox.Show("Target call harus diisi", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }



            await Task.WhenAll(uploadData());
        }

        private void tkelurahan_EditValueChanged(object sender, EventArgs e)
        {
            if (tkelurahan.EditValue.ToString().Length > 0)
            {

                OleDbConnection cn = new OleDbConnection();

                try
                {
                    cn = myclass.open_conn();

                    if (cn != null && cn.State != ConnectionState.Closed)
                    {

                        String sqlquery = String.Format("select idkab,namakab,idprop,namaprop from db_penghubung.dbo.v_areas where idkec='{0}'", tkelurahan.EditValue);
                        using (OleDbCommand cmq = new OleDbCommand(sqlquery, cn))
                        {
                            using (OleDbDataReader drdread = cmq.ExecuteReader())
                            {
                                while (drdread.Read())
                                {
                                    idkab = drdread["idkab"].ToString();
                                    idprop = drdread["idprop"].ToString();

                                    tkabupaten.EditValue = drdread["namakab"].ToString();
                                    tpropinsi.EditValue = drdread["namaprop"].ToString();


                                }
                            }


                        }

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
                finally
                {


                    if (cn != null)
                    {
                        if (cn.State == ConnectionState.Open)
                        {
                            cn.Close();
                        }
                    }

                    Console.WriteLine("The 'try catch' is finished.");




                }


            }

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            fcaritoko objfrmHome = new fcaritoko(mlat, mlong);
            objfrmHome.ShowDialog();

            int ookks = objfrmHome.Getokmode;
            String kodebaru = objfrmHome.Getkode;

            if (ookks != 0)
            {

                tkodetoko.EditValue = kodebaru;

                if (ookks == 1)
                {
                    noomodenew = true;
                    tmode.EditValue = "NOO BARU";

                }
                else
                {
                    noomodenew = false;
                    tmode.EditValue = "EDIT/ALIHKAN TOKO";
                }
            }
            else
            {
                noomodenew = true;
                tmode.EditValue = "NOO BARU";
                tkodetoko.EditValue = "";
            }

        }

        private void tkodetoko_EditValueChanged(object sender, EventArgs e)
        {

            if (tkodetoko.EditValue.ToString().Length > 0)
            {

                OleDbConnection cn = new OleDbConnection();

                try
                {
                    cn = myclass.open_conn();

                    if (cn != null && cn.State != ConnectionState.Closed)
                    {

                        String sqlquery = String.Format("select BusinessPartnerId as kode from WTL2020_m1company.Shared.Customers where BusinessPartnerId='{0}'", tkodetoko.EditValue);
                        using (OleDbCommand cmq = new OleDbCommand(sqlquery, cn))
                        {
                            using (OleDbDataReader drdread = cmq.ExecuteReader())
                            {

                                String kodetoko = "";

                                while (drdread.Read())
                                {
                                    kodetoko = drdread["kode"].ToString();
                                }

                                if (kodetoko.Length == 0)
                                {
                                    noomodenew = true;
                                    tmode.EditValue = "NOO BARU";
                                }
                                else
                                {
                                    noomodenew = false;
                                    tmode.EditValue = "EDIT/ALIHKAN TOKO";
                                }

                            }


                        }

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
                finally
                {


                    if (cn != null)
                    {
                        if (cn.State == ConnectionState.Open)
                        {
                            cn.Close();
                        }
                    }

                    Console.WriteLine("The 'try catch' is finished.");




                }


            }

        }

        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void textEdit2_EditValueChanged_1(object sender, EventArgs e)
        {

        }
    }

    public class Clstoken { public string token { get; set; } }

    public class ClsSuccessapi { public bool success { get; set; } }

    public class Statgetapisuccess
    {
        public String pesan { get; set; }
    }

    public class SalesApiClient
    {
        private readonly HttpClient _httpClient;

        public SalesApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<SalesResponse> GetSalesTypeAsync(string salesmanId)
        {
            var url = $"http://localhost:8000/api/sales/{salesmanId}"; // URL API
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SalesResponse>(content);
            }
            return new SalesResponse { Success = false };
        }
    }

    // Response dari API
    public class SalesResponse
    {
        public bool Success { get; set; }
        public string kdsales { get; set; }
        public string jnssales { get; set; }
    }

}

