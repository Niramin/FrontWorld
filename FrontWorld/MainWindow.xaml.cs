
using FrontWorld.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace FrontWorld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private World world;
        public MainWindow()
        {
            
            InitializeComponent();
            btn.Click += async (semder, e) =>
            {
                world = new World();
                world.logger.Subscribe(sendProgressUpdate);
                int cycle = Int32.Parse(CyclesCount.Text);
                int recharge = Int32.Parse(RechargeCount.Text);
                await Task.Run(() => { world.conductCycle(cycle,recharge); });

            };

            outdirbutton.Click += (sender, e) =>
            {


                using (var dialog = new FolderBrowserDialog())
{
                    //setup here

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)  //check for OK...they might press cancel, so don't do anything if they did.
                    {
                        var path = dialog.SelectedPath;
                        Properties.Settings.Default.outdir = path;
                        Properties.Settings.Default.Save();
                        //do something with path
                    }
                }

            };
        }

        private void sendProgressUpdate(string message)
        {
            string[] messages = message.Split('/');
            int num = Int32.Parse(messages[0]);
            int dom = Int32.Parse(messages[1]);

            
            this.Dispatcher.BeginInvoke(new Action(() => {

                progressBar.Maximum = dom;
                progressBar.Value = num;
                if (dom == num) progressBar.Value = 0;
            
            }));

        }
    }
}
