using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialTunningTool
{
    class ConsoleDisplay
    {
        private TextBox _textBox = null;
        private DataAdapter Adapter = null;
        private Timer timer = null;
        public ConsoleDisplay(TextBox textBox, DataAdapter adapter){
            _textBox = textBox;
            Adapter = adapter;
            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            object [] b = Adapter.getData();
            Adapter.clear();
            for(int i = 0; i < b.Length; i++){
                _textBox.Text += "\n" + b[i].ToString();
            }
            _textBox.SelectionStart = _textBox.TextLength;
            _textBox.ScrollToCaret();
        }
    }
}
