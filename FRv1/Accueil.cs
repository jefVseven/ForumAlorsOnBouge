﻿using ClassesMetiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FRv1
{
    public partial class Accueil : Form
    {
        //static internal int IsRmodo;
        //static internal Users CurrentUsers;
        //static internal bool Isconnected;

        public Accueil()
        {
            InitializeComponent();
        }

        private void btConsul_Click(object sender, EventArgs e)
        {
            using (RubricSujPost frmMain = new RubricSujPost())
            {
                frmMain.ShowDialog();
            }
        }

        private void btQuitter_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btConnect_Click(object sender, EventArgs e)
        {

            using (Identification frmIdentity = new Identification())
            {
                frmIdentity.ShowDialog();
            }
        }
    }
}