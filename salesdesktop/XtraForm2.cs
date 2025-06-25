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

namespace salesdesktop
{
    public partial class XtraForm2 : DevExpress.XtraEditors.XtraForm
    {

        string setpathphoto;
        String stattoko;

        public XtraForm2(String pathfoto,String stattokos)
        {
            setpathphoto = pathfoto;
            stattoko = stattokos;
            InitializeComponent();
        }

        private void XtraForm2_Load(object sender, EventArgs e)
        {

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;

            myclass classapi = new myclass();
            String urlprof = classapi.geturlapiprofil();

            if (stattoko.Length > 0)
            {
                pictureEdit1.LoadAsync(String.Format(@"{0}\{1}", urlprof, stattoko));
            }
            else
            {
                pictureEdit1.LoadAsync(String.Format(@"{0}\tidakadafoto.jpg", rootPath));
            }
        }

    }
}