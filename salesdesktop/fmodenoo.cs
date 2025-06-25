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
    public partial class fmodenoo : DevExpress.XtraEditors.XtraForm
    {

        public int Getokmode { get; set; } = 0;

        public fmodenoo()
        {
            InitializeComponent();
        }

        private void fmodenoo_Shown(object sender, EventArgs e)
        {
            radioGroup1.EditValue = 1;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int hasilok = int.Parse(radioGroup1.EditValue.ToString());

            Getokmode = hasilok;
            this.Close();

        }

    }
}