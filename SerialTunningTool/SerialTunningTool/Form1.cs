using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace SerialTunningTool  
{
    public partial class Form1 : Form
    {
        private SerialPort com = null;
        private DataRecevier dataRecevier = null;
        private DataAdapter[] adapter = null;
        private PlotGraph plotGraph = null;
        private Communication communication = null;
        private Controller controller = null;
        private ConsoleDisplay consoleDisplay = null;
        private Timer watchDogTimer = null;
        private float[] Kp = { 0.0f, 0.0f, 0.0f };//{800.0f, 800.0f, 600.0f};
        private float[] Ki = {0.0f, 0.0f, 0.0f};
        private float[] Kd = { 0.0f, 0.0f, 0.0f };//{0.85f,0.85f,0.1f};
        private int rpy = 0;
        private float RPM = 0;
        private float initRPM = 3500;
        private float MotorKp = 0.0f;
        private float MotorKd = 0.0f;
        private float kalmanQ = 0.0f;
        private float kalmanR1 = 0.0f;
        private float kalmanR2 = 0.0f;
        private float[] offset = {0.0f,0.0f,0.0f};
        private float driftKp = 0;
        private Timer printTimer = null;

        void SendAll()
        {
            controller.RollKp(Kp[0]);
            System.Threading.Thread.Sleep(20);
            controller.PitchKp(Kp[1]);
            System.Threading.Thread.Sleep(20);
            controller.YawKp(Kp[2]);
            System.Threading.Thread.Sleep(20);
            controller.RollKi(Ki[0]);
            System.Threading.Thread.Sleep(20);
            controller.PitchKi(Ki[1]);
            System.Threading.Thread.Sleep(20);
            controller.YawKi(Ki[2]);
            System.Threading.Thread.Sleep(20);
            controller.RollKd(Kd[0]);
            System.Threading.Thread.Sleep(20);
            controller.PitchKd(Kd[1]);
            System.Threading.Thread.Sleep(20);
            controller.YawKd(Kd[2]);
            System.Threading.Thread.Sleep(20);
            controller.YawKd(Kd[2]);
            System.Threading.Thread.Sleep(20);
            controller.RollOffset(offset[0]);
            System.Threading.Thread.Sleep(20);
            controller.PitchOffset(offset[1]);
            System.Threading.Thread.Sleep(20);
            controller.YawOffset(offset[2]);
            System.Threading.Thread.Sleep(20);
            controller.KalmanQ(kalmanQ);
            System.Threading.Thread.Sleep(20);
            controller.KalmanQ(kalmanR1);
            System.Threading.Thread.Sleep(20);
            controller.KalmanQ(kalmanR2);
            System.Threading.Thread.Sleep(20);
        }

        void PrintData() {

            textBoxLocalDisplay.Text = Kp[0].ToString() + "," + Kp[1].ToString() + "," + Kp[2].ToString() + "\r\n" +
                                    Ki[0].ToString() + "," + Ki[1].ToString() + "," + Ki[2].ToString() + "\r\n" +
                                    Kd[0].ToString() + "," + Kd[1].ToString() + "," + Kd[2].ToString() + "\r\n" +
                                    offset[0].ToString() + "," + offset[1].ToString() + "," + offset[2].ToString() + "\r\n" +
                                    kalmanQ.ToString() + "\r\n" +
                                    kalmanR1.ToString() + "," + kalmanR2.ToString();
        }

        public Form1()
        {
            InitializeComponent();
            PortComboBox.Click += PortComboBox_Click;
            PortComboBox.SelectedIndexChanged += PortComboBox_SelectedIndexChanged;
            radioButtonRoll.Checked = true;
            this.FormClosing += Form1_FormClosing;
            printTimer = new Timer();
            printTimer.Interval = 1000;
            printTimer.Tick += printTimer_Tick;
            printTimer.Start();
            try
            {
                StreamReader file = new StreamReader("parameter.txt");
                String s = file.ReadLine();
                String[] str = s.Split(',');
                for(int i = 0; i < 3; i++){
                    Kp[i] = float.Parse(str[i]);
                }
                for (int i = 3; i < 6; i++)
                {
                    Ki[i-3] = float.Parse(str[i]);
                }
                for (int i = 6; i < 9; i++)
                {
                    Kd[i-6] = float.Parse(str[i]);
                }
                //MotorKp = float.Parse(str[9]);
                //MotorKd = float.Parse(str[10]);
                //kalmanQ = float.Parse(str[11]);
                //kalmanR1 = float.Parse(str[12]);
                //kalmanR2 = float.Parse(str[13]);
                offset[0] = float.Parse(str[9]);
                offset[1] = float.Parse(str[10]);
                offset[2] = float.Parse(str[11]);
                kalmanQ = float.Parse(str[12]);
                kalmanR1 = float.Parse(str[13]);
                kalmanR2 = float.Parse(str[14]);
                file.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);                
            }
        }

        void printTimer_Tick(object sender, EventArgs e)
        {
            PrintData();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StreamWriter file = new StreamWriter("parameter.txt");
                String s = "";
                for (int i = 0; i < 3; i++)
                {
                    s += Kp[i].ToString() + ",";
                }
                for (int i = 0; i < 3; i++)
                {
                    s += Ki[i].ToString() + ",";
                }
                for (int i = 0; i < 3; i++)
                {
                    s += Kd[i].ToString() + ",";
                }
                //s += MotorKp + ",";
                //s += MotorKd + ",";
                //s += kalmanQ + ",";
                //s += kalmanR1 + ",";
                //s += kalmanR2 + ",";
                s += offset[0] + ",";
                s += offset[1] + ",";
                s += offset[2] + ",";
                s += kalmanQ + ",";
                s += kalmanR1 + ",";
                s += kalmanR2;
                file.WriteLine(s);
                file.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);     
            }
        }

        void PortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            com = new SerialPort(PortComboBox.SelectedItem.ToString());
            com.BaudRate = int.Parse(BaudrateComboBox.SelectedItem.ToString());
            com.Encoding = Encoding.GetEncoding("ISO-8859-1");
            try
            {
                com.Open();
                com.DiscardInBuffer();
                com.DiscardOutBuffer();
                adapter = new DataAdapter[5];
                for (int i = 0; i < adapter.Length; i++ )
                {
                    adapter[i] = new DataAdapter();
                }
                dataRecevier = new DataRecevier(com, adapter);
                plotGraph = new PlotGraph(Scope, adapter, textBoxCurvesValue);
                controller = new Controller();
                consoleDisplay = new ConsoleDisplay(textBoxDisplay, adapter[4]);
                communication = new Communication(com);
                watchDogTimer = new Timer();
                watchDogTimer.Interval = 200;
                watchDogTimer.Tick += watchDogTimer_Tick;
                //watchDogTimer.Start();
                //SendAll();

                PrintData();
            }
            catch (Exception err) {
                MessageBox.Show(err.Message);
            }
        }

        void watchDogTimer_Tick(object sender, EventArgs e)
        {
            controller.ClearWatchDog();
        }

        void PortComboBox_Click(object sender, EventArgs e)
        {
            String[] str = SerialPort.GetPortNames();
            PortComboBox.Items.Clear();
            foreach (String s in str)
            {
                PortComboBox.Items.Add(s);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            plotGraph.clear();
            textBoxLocalDisplay.Text = "";
            textBoxDisplay.Text = "";
        }

        private void buttonPowerIncrease_Click(object sender, EventArgs e)
        {

            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                RPM += value;
                controller.MotorContorl(RPM);
            }        
        }

        private void buttonPowerDecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                RPM -= value;
                if (RPM < 0.0f)
                {
                    RPM = 0.0f;
                }
                controller.MotorContorl(RPM);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            controller.MotorStop();
            //System.Threading.Thread.Sleep(20);
            //controller.Reset();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //controller.Reset();
            //System.Threading.Thread.Sleep(20);
            //SendAll();
            //System.Threading.Thread.Sleep(20);
            controller.MotorSetInitPower(initRPM);
            System.Threading.Thread.Sleep(20);
            controller.MotorStart();
        }

        private void buttonInitPowerIncrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value)){
                initRPM += value;
                controller.MotorSetInitPower(initRPM);
            }
        }

        private void buttonInitPowerdecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                initRPM -= value;
                if (initRPM < 0.0f)
                {
                    initRPM = 0.0f;
                }
                controller.MotorSetInitPower(initRPM);
            }
        }

        private void buttonKpIncrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                Kp[rpy] += value;
                switch (rpy)
                {
                    case 0:
                        controller.RollKp(Kp[rpy]);
                        break;
                    case 1:
                        controller.PitchKp(Kp[rpy]);
                        break;
                    case 2:
                        controller.YawKp(Kp[rpy]);
                        break;
                }
            }
        }

        private void buttonKpDecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            float.TryParse(textBoxInput.Text, out value);
            Kp[rpy] -= value;
            if (Kp[rpy] < 0)
            {
                Kp[rpy] = 0;
            }
            switch (rpy)
            {
                case 0:
                    controller.RollKp(Kp[rpy]);
                    break;
                case 1:
                    controller.PitchKp(Kp[rpy]);
                    break;
                case 2:
                    controller.YawKp(Kp[rpy]);
                    break;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            controller.Reset();
            System.Threading.Thread.Sleep(20);
            //SendAll();
        }

        private void buttonKdIncrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                Kd[rpy] += value;
                switch (rpy)
                {
                    case 0:
                        controller.RollKd(Kd[rpy]);
                        break;
                    case 1:
                        controller.PitchKd(Kd[rpy]);
                        break;
                    case 2:
                        controller.YawKd(Kd[rpy]);
                        break;
                }
            }
        }

        private void buttonKdDecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                Kd[rpy] -= value;
                if (Kd[rpy] < 0)
                {
                    Kd[rpy] = 0;
                }
                switch (rpy)
                {
                    case 0:
                        controller.RollKd(Kd[rpy]);
                        break;
                    case 1:
                        controller.PitchKd(Kd[rpy]);
                        break;
                    case 2:
                        controller.YawKd(Kd[rpy]);
                        break;
                }
            }
        }

        private void buttonTogglePrint_Click(object sender, EventArgs e)
        {
            controller.TogglePrint();
        }

        private void buttonMotorKpIncrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                MotorKp += value;
                controller.MotorKp(MotorKp);
            }
        }

        private void buttonMotorKpDecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                MotorKp -= value;
                if (MotorKp < 0)
                {
                    MotorKp = 0;
                }
                controller.MotorKp(MotorKp);
            }
        }

        private void buttonMotorKdIncrease_Click(object sender, EventArgs e)
        {

            float value = 0;
            float.TryParse(textBoxInput.Text, out value);
            MotorKd += value;
            controller.MotorKd(MotorKd);
        }

        private void buttonMotorKdDecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            if (float.TryParse(textBoxInput.Text, out value))
            {
                MotorKd -= value;
                if (MotorKd < 0)
                {
                    MotorKd = 0;
                }
                controller.MotorKd(MotorKd);
            }
        }

        private void buttonKiIncrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            float.TryParse(textBoxInput.Text, out value);
            Ki[rpy] += value;
            switch(rpy){
                case 0:
                    controller.RollKi(Ki[rpy]);
                    break;
                case 1:
                    controller.PitchKi(Ki[rpy]);
                    break;
                case 2:
                    controller.YawKi(Ki[rpy]);
                    break;
            }
         }

        private void buttonKiDecrease_Click(object sender, EventArgs e)
        {
            float value = 0;
            float.TryParse(textBoxInput.Text, out value);
            Ki[rpy] -= value;
            if (Ki[rpy] < 0)
            {
                Ki[rpy] = 0;
            }
            switch (rpy)
            {
                case 0:
                    controller.RollKi(Ki[rpy]);
                    break;
                case 1:
                    controller.PitchKi(Ki[rpy]);
                    break;
                case 2:
                    controller.YawKi(Ki[rpy]);
                    break;
            }
        }

        private void radioButtonRoll_CheckedChanged(object sender, EventArgs e)
        {
            rpy = 0;
        }

        private void radioButtonPitch_CheckedChanged(object sender, EventArgs e)
        {
            rpy = 1;
        }

        private void radioButtonYaw_CheckedChanged(object sender, EventArgs e)
        {
            rpy = 2;
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                watchDogTimer.Stop();
                adapter = null;
                dataRecevier = null;
                plotGraph = null;
                controller = null;
                consoleDisplay = null;
                communication = null;
                watchDogTimer = null;
                Scope.Channels.Clear();
                com.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void buttonQIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                kalmanQ += value;
                controller.KalmanQ(kalmanQ);
            }
            catch{}
        }

        private void buttonQDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                kalmanQ -= value;
                if (kalmanQ < 0)
                {
                    kalmanQ = 0.000001f;
                }
                controller.KalmanQ(kalmanQ);
            }
            catch { }
        }

        private void buttonR1Increase_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                kalmanR1 += value;
                controller.KalmanR1(kalmanR1);
            }
            catch { }
        }

        private void buttonR1Decrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                kalmanR1 -= value;
                if (kalmanR1 < 0)
                {
                    kalmanR1 = 0.000001f;
                }
                controller.KalmanR1(kalmanR1);
            }
            catch { }
        }

        private void buttonR2Increase_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                kalmanR2 += value;
                controller.KalmanR2(kalmanR2);
            }
            catch { }
        }

        private void buttonR2Decrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                kalmanR2 -= value;
                if (kalmanR2 < 0)
                {
                    kalmanR2 = 0.000001f;
                }
                controller.KalmanR2(kalmanR2);
            }
            catch { }

        }

        private void buttonRollOffsetIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                offset[0] += value;
                controller.RollOffset(offset[0]);
            }
            catch { }
        }

        private void buttonRollOffsetDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                offset[0] -= value;
                controller.RollOffset(offset[0]);
            }
            catch { }
        }

        private void buttonPitchOffsetIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                offset[1] += value;
                controller.PitchOffset(offset[1]);
            }
            catch { }
        }

        private void buttonPitchOffsetDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                offset[1] -= value;
                controller.PitchOffset(offset[1]);
            }
            catch { }
        }

        private void buttonYawOffsetIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                offset[2] += value;
                controller.YawOffset(offset[2]);
            }
            catch { }
        }

        private void buttonYawOffsetDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                offset[2] -= value;
                controller.YawOffset(offset[2]);
            }
            catch { }
        }

        private void buttonDriftKpIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                driftKp += value;
                controller.DriftKp(driftKp);
            }
            catch { }            
        }

        private void buttonDriftKpDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                float value = 0;
                float.TryParse(textBoxInput.Text, out value);
                driftKp -= value;
                if (driftKp < 0)
                {
                    driftKp = 0;
                }
                controller.DriftKp(driftKp);
            }
            catch { }  

        }

        private void buttonHigh_Click(object sender, EventArgs e)
        {
            controller.High();
        }

        private void buttonLow_Click(object sender, EventArgs e)
        {
            controller.Low();
        }

        private void buttonSendAll_Click(object sender, EventArgs e)
        {
            SendAll();
        }
    }
}
