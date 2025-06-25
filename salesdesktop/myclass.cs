using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Globalization;

namespace salesdesktop
{
    class myclass
    {

        static String FileConf = AppDomain.CurrentDomain.BaseDirectory + "\\autocon.dll";

        static String serverUrlprofil = "https://salesmobile.grandwater.co.id/storage/foto_toko";
        public String geturlapiprofil() { return serverUrlprofil; }

       //String urlapi = "http://192.168.3.223:8000/api";

       String urlapi = "https://salesmobile.grandwater.co.id/api";


        String KeyFcprogram = "AAAAFSQWRpA:APA91bHS2t9WU4Fo4oRslCYAW6g8IPkxcdQsIN65jZ7BM5Yq7gmwF5HBBZjIwkJ2ufJao9RuvwxA-DZ24rJ8GRXo6I55vuVVgbrH19Dh84RB7Re6mc-7DF0hEvZNNsRG-rOAA8Bg4sOr";

        public String geturlapi() { return urlapi; }

        public String geturlKeyFc() { return KeyFcprogram; }

        public static OleDbConnection open_conn()
        {

            String mloc = "";
            String mdbase = "";
            String muser = "";
            String mpwd = "";

            OleDbConnection cn = null;

            try
            {

                using (StreamReader reader = new StreamReader(new FileStream(FileConf, FileMode.Open)))
                {
                    string line;
                    int i = 0;

                    while ((line = reader.ReadLine()) != null)
                    {

                        if (i == 0)
                        {
                            mloc = line;
                        }

                        if (i == 1)
                        {
                            mdbase = line;
                        }

                        if (i == 2)
                        {
                            muser = line;
                        }

                        if (i == 3)
                        {
                            mpwd = line;
                        }

                        i++;
                    }
                }

                String myconnectionstring = String.Format("Provider=SQLOLEDB;Server={0};Database={1};Uid={2};Pwd={3};", mloc, mdbase, muser, mpwd);

                cn = new OleDbConnection(myconnectionstring);
                cn.Open();

            }
            catch (OleDbException e)
            {
                Console.WriteLine(e.Message);
            }


            return cn;


        }

        public static DataSet GetdataSet(String sql, OleDbConnection cn)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, cn);
            DataSet myData = new DataSet();
            adapter.Fill(myData);

            adapter.Dispose();

            return myData;

        }

        public static String ConvertDateToString(String valdate, String convertformat)
        {

            String hasilconvert = "";

            if (convertformat.Equals("ymdhms"))
            {
                hasilconvert = Convert.ToDateTime(valdate).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (convertformat.Equals("ymd"))
            {
                hasilconvert = Convert.ToDateTime(valdate).ToString("yyyy-MM-dd");
            }
            else if (convertformat.Equals("dmy"))
            {
                hasilconvert = Convert.ToDateTime(valdate).ToString("dd-MM-yyyy");
            }
            else if (convertformat.Equals("dmyhms"))
            {
                hasilconvert = Convert.ToDateTime(valdate).ToString("dd-MM-yyyy HH:mm");
            }
            else if (convertformat.Equals("dmmy"))
            {
                hasilconvert = Convert.ToDateTime(valdate).ToString("dd MMMM yyyy");
            }

            return hasilconvert;

        }


    }

}
