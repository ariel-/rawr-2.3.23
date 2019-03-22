using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.DPSDK
{
    public partial class Graph : Form
    {
        private Bitmap bitGraph;
        public Graph(Bitmap bitmap)
        {
            this.bitGraph = bitmap;
            InitializeComponent();
        }

       
        private void Graph_Load(object sender, EventArgs e)
        {
            pictureBoxGraph.Image = bitGraph;
            pictureBoxGraph.SizeMode = PictureBoxSizeMode.Zoom;
        }

    }
}
