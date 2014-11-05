using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KSPAdvancedFlyByWire;

namespace ControllerTestTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ControllersListbox.Items.Clear();
            var controllers = IController.EnumerateAllControllers();
            foreach (var controller in controllers)
            {
                ControllersListbox.Items.Add(controller.Value.Value);
            }
        }
    }
}
