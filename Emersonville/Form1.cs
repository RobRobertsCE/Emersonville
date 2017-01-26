using Emersonville.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emersonville
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                int frameIdx = 0;
                var h = new Horse();

                h.DirectionX = 12;
                h.DirectionY = 44;

                h.Load = 225; // load 'er up!

                h.Rate += 17; // giddy-up!

                while (frameIdx < 100)
                {
                    if (frameIdx < 10)
                        h.Rate += 1.1M;

                    if (frameIdx > 70)
                        h.Rate -= .1M;

                    h.UpdatePosition(1000);
                    Console.WriteLine("frame:{0} Health:{1:N2} Energy:{2:N2} EffectiveEffort:{3:N2} EffectiveRate:{4:N2} ", frameIdx, h.Health, h.Energy, h.EffectiveEffort, h.EffectiveRate);

                    frameIdx++;

                    if (h.Health == 0)
                    {
                        frameIdx = 100;
                        Console.WriteLine("You are dead.");
                    }
                    else if (h.Energy == 0)
                    {
                        frameIdx = 100;
                        Console.WriteLine("You are exhausted.");
                    }
                    else if (h.Rate == 0)
                    {
                        frameIdx = 100;
                        Console.WriteLine("You are still.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
