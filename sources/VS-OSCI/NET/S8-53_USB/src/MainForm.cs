﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ControlLibraryS8_53;

namespace S8_53_USB {

    public partial class MainForm : Form {

        private bool needForDisconnect = false;

        // Этот порт используется для соединения по USB
        private LibraryS8_53.ComPort port = new LibraryS8_53.ComPort();
        // Этот сокет используется для соединения по LAN
        private LibraryS8_53.SocketTCP socket = new LibraryS8_53.SocketTCP();

        private Dictionary<Button, string> mapButtons = new Dictionary<Button, string>();

        private Queue<string> commands = new Queue<string>();

        public MainForm() {
            InitializeComponent();

            mapButtons.Add(btnChannel0,    "CHAN1");
            mapButtons.Add(btnService,     "SERVICE");
            mapButtons.Add(btnChannel1,    "CHAN2");
            mapButtons.Add(btnDisplay,     "DISPLAY");
            mapButtons.Add(btnTime,        "TIME");
            mapButtons.Add(btnMemory,      "MEMORY");
            mapButtons.Add(btnTrig,        "TRIG");
            mapButtons.Add(btnStart,       "START");
            mapButtons.Add(btnCursors,     "CURSORS");
            mapButtons.Add(btnMeasures,    "MEASURES");
            mapButtons.Add(btnHelp,        "HELP");
            mapButtons.Add(btnMenu,        "MENU");
            mapButtons.Add(btnF1,          "1");
            mapButtons.Add(btnF2,          "2");
            mapButtons.Add(btnF3,          "3");
            mapButtons.Add(btnF4,          "4");
            mapButtons.Add(btnF5,          "5");

            Display.EndFrameEvent += OnEndFrameEvent;

            btnUpdatePorts_Click(null, null);
        }

        private void button_MouseDown(object sender, MouseEventArgs e) {
            commands.Enqueue("KEY:" + StringToSendForButton(sender) + " DOWN");
        }

        private void button_MouseUp(object sender, MouseEventArgs e) {
            commands.Enqueue("KEY:" + StringToSendForButton(sender) + " UP");
        }

        private void governor_RotateLeft(object sender, EventArgs e) {
            commands.Enqueue("GOV:" + ((Governor)sender).ValueToSend + " LEFT");
        }

        private void governor_RotateRight(object sender, EventArgs e) {
            commands.Enqueue("GOV:" + ((Governor)sender).ValueToSend + " RIGHT");
        }

        private string StringToSendForButton(object btn) {
            return mapButtons[(Button)btn];
        }

        private void btnUpdatePorts_Click(object sender, EventArgs e) {
            string[] ports = port.GetPorts();
            comboBoxPorts.Items.Clear();
            comboBoxPorts.Items.AddRange(ports);
            comboBoxPorts.SelectedIndex = ports.Length - 1;
        }

        private void btnConnectUSB_Click(object sender, EventArgs e)
        {
            if (port.IsOpen())                                  // Если порт открыт - идёт обмен с прибором. Будем отключать
            {
                buttonConnectUSB.Text = "Подкл";
                comboBoxPorts.Enabled = true;
                buttonUpdatePorts.Enabled = true;
                needForDisconnect = true;                       // сообщаем прибору, что нужно отключиться при первой возможности

                EnableControlLAN(true);
            }
            else
            {
                if (port.Connect(comboBoxPorts.SelectedIndex, false)) // иначе делаем попыткую подключиться
                {
                    needForDisconnect = false;
                    EnableControlLAN(false);
                    comboBoxPorts.Enabled = false;
                    buttonUpdatePorts.Enabled = false;

                    buttonConnectUSB.Text = "Откл";
                    port.SendString("DISPLAY:AUTOSEND 1");
                    display.StartDrawing(port.GetSerialPort());
                }
            }
        }

        private void buttonConnectLAN_Click(object sender, EventArgs e)
        {
            try
            {
                if (socket.IsConnected())
                {
                    buttonConnectLAN.Text = "Подкл";
                    textBoxIP.Enabled = true;
                    buttonUpdatePorts.Enabled = true;
                    needForDisconnect = true;                       // сообщаем прибору, что нужно отключиться при первой возможности

                    EnableControlsUSB(true);
                }
                else
                {
                    if (socket.Connect(textBoxIP.Text, Int32.Parse(textBoxPort.Text)))
                    {

                    }
                }
            }
            catch(Exception)
            {

            }
        }

        private void cbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonConnectUSB.Enabled = port.DeviceConnectToPort(comboBoxPorts.SelectedIndex);
        }

        private void OnEndFrameEvent(object sender, EventArgs e)
        {
            if (port.IsOpen())                                      // Если идёт обмен по USB
            {
                if (needForDisconnect)
                {
                    port.Stop();
                }
                else
                {
                    while (commands.Count != 0)
                    {
                        port.SendString(commands.Dequeue());
                    }
                    port.SendString("DISPLAY:AUTOSEND 2");
                    display.StartDrawing(port.GetSerialPort());
                }
            }
            else                                                    // Если обмен идёт по LAN
            {
                if(needForDisconnect)
                {
                    socket.Disconnect();
                    EnableControlsUSB(true);
                }
                else
                {
                    while(commands.Count != 0)
                    {
                        socket.SendString(commands.Dequeue());
                    }
                    socket.SendString("DISPLAY:AUTOSEND 2");
                    //display.StartDrawing();
                }
            }
        }

        private void MainForm_Closed(object sender, EventArgs e)
        {
            // Закрывать порт непосредственно по закрытии формы нельзя, чтобы поток не завис.
            needForDisconnect = true;           // Поэтому устанавливаем признако того, что порт надо закрыть
            while(port.IsOpen()) { }            // И ждём пока это произойдёт
        }

        // Активировать/деактивировать элементы управления, отвечающие за связь по USB
       private void EnableControlsUSB(bool enable)
        {
            cbPorts_SelectedIndexChanged(null, null);
            buttonUpdatePorts.Enabled = enable;
            buttonConnectUSB.Enabled = enable;
        }

        // Активировать/деактивировать элементы управления, отвечающие за свять по LAN
        private void EnableControlLAN(bool enable)
        {
            textBoxIP.Enabled = enable;
            textBoxPort.Enabled = enable;
            OnChangedAddressIP();
        }

        private void OnChangedAddressIP()
        {

        }

        private void textBoxIP_TextChanged(object sender, EventArgs e)
        {
            OnChangedAddressIP();
        }

        private void textBoxPort_TextChanged(object sender, EventArgs e)
        {
            OnChangedAddressIP();
        }
    }
}