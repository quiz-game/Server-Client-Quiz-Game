﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {
        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> clientSockets = new List<Socket>();
        static List<string> names = new List<string>();
        static bool terminating = false, accept = true;
        static string question, answer, guess;
        static int clients = 0;
        static readonly object _game = new object(), _accept = new object();


        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Visible = false;
            int turn = int.Parse(turnNum.Text);

            play();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Visible = false;
            int portnum = int.Parse(port.Text);
            try
            {
                //Opening the server 
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, portnum);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(2);
                feed.Text = "Server is listening... \n";
                //Accepting messages
                Thread acceptThread = new Thread(Accept);
                acceptThread.Start();
            }
            catch
            {
                feed.Text = "There is a problem";
            }
        }

        //Initiates the game
        private void play()
        {
            /*string command = "Ask a question.";
            Byte[] buffer = Encoding.Default.GetBytes(command);
            Byte[] a_q_buffer = new Byte[64];
            Byte[] g_buffer = new Byte[64];
            //turn = true for the first client's turn, false for second client's turn
            if (turn)
            {
                if (!terminating)
                {
                    try
                    {
                        //Sending the question to first client, waiting for the answer and comparing it to second client's 
                        clientSockets[0].Send(buffer);
                        feed.Text += "Waiting for the question." + '\n';
                        clientSockets[0].Receive(a_q_buffer);
                        string a_q = Encoding.Default.GetString(a_q_buffer);
                        if (a_q.Substring(0, a_q.IndexOf('?') + 1) != "")
                        {
                            question = a_q.Substring(0, a_q.IndexOf('?') + 1);
                            feed.Text += "Question: " + question + '\n';
                            answer = a_q.Substring(a_q.IndexOf('?') + 1, a_q.IndexOf('.'));
                            clientSockets[1].Send(Encoding.Default.GetBytes(question));
                            feed.Text += "Waiting for a guess. \n";
                            clientSockets[1].Receive(g_buffer);
                            guess = Encoding.Default.GetString(g_buffer);
                            if (guess.Substring(0, guess.IndexOf('.') + 1) != "")
                            {
                                feed.Text += "Guess: " + guess.Substring(0, guess.IndexOf('.') + 1) + '\n';
                                feed.Text += "Answer: " + answer.Substring(0, answer.IndexOf('.') + 1) + '\n';
                                //if answer is correct, inform every part, both server and clients
                                if (guess.Substring(0, guess.IndexOf('.') + 1) == answer.Substring(0, answer.IndexOf('.') + 1))
                                {
                                    feed.Text += "Correct! \n";
                                    clientSockets[0].Send(Encoding.Default.GetBytes("1"));
                                    clientSockets[1].Send(Encoding.Default.GetBytes("1"));
                                }
                                //if false
                                else
                                {
                                    feed.Text += "Wrong! \n";

                                    clientSockets[0].Send(Encoding.Default.GetBytes("0"));
                                    clientSockets[1].Send(Encoding.Default.GetBytes("0"));
                                }
                                turn = false;
                            }
                        }

                    }
                    //if a clients is disconected from the server, game stops and all sockets are closed
                    catch
                    {
                        if (!terminating)
                        {
                            feed.Text += "Client 1 has disconnected. Game will stop. \n";
                            terminating = true;
                        }

                    }
                }


            }
            else
            {
                if (!terminating)
                {
                    try
                    {
                        //turn of second client, question is sent to first client and compared with first one's answer


                        clientSockets[1].Send(buffer);
                        feed.Text += "Waiting for the question" + '\n';
                        clientSockets[1].Receive(a_q_buffer);
                        string a_q = Encoding.Default.GetString(a_q_buffer);
                        if (a_q.Substring(0, a_q.IndexOf('?') + 1) != "")
                        {
                            question = a_q.Substring(0, a_q.IndexOf('?') + 1);
                            feed.Text += "Question: " + question + '\n';
                            answer = a_q.Substring(a_q.IndexOf('?') + 1, a_q.IndexOf('.'));
                            clientSockets[0].Send(Encoding.Default.GetBytes(question));
                            feed.Text += "Waiting for a guess. \n";
                            clientSockets[0].Receive(g_buffer);
                            if (guess.Substring(0, guess.IndexOf('.') + 1) != "")
                            {
                                guess = Encoding.Default.GetString(g_buffer);
                                feed.Text += "Guess: " + guess.Substring(0, guess.IndexOf('.') + 1) + '\n';
                                feed.Text += "Answer: " + answer.Substring(0, answer.IndexOf('.') + 1) + '\n';
                                if (guess.Substring(0, guess.IndexOf('.') + 1) == answer.Substring(0, answer.IndexOf('.') + 1))
                                {
                                    feed.Text += "Correct! \n";
                                    clientSockets[0].Send(Encoding.Default.GetBytes("1"));
                                    clientSockets[1].Send(Encoding.Default.GetBytes("1"));
                                }
                                // if false
                                else
                                {
                                    feed.Text += "Wrong! \n";

                                    clientSockets[0].Send(Encoding.Default.GetBytes("0"));
                                    clientSockets[1].Send(Encoding.Default.GetBytes("0"));
                                }
                                turn = true;
                            }
                        }
                        //if answer is true

                    }
                    catch
                    {
                        //if one of the clients disconnects from the server, the game stops and everything is closed down.
                        if (!terminating)
                        {
                            feed.Text += "Client 2 has disconnected. Game will stop. \n";
                            terminating = true;
                        }

                    }
                }

            }
            */
        }
        //Accepting clients
        private void Accept(object obj)
        {
            //Accepting client
            while (accept)
            {
                try
                {
                    Socket newClient = serverSocket.Accept();
                    clientSockets.Add(newClient);
                    Thread receiveThread = new Thread(Receive);
                    receiveThread.Start();

                }
                catch
                {
                    //if the server encounters a problem
                    if (terminating)
                    {
                        feed.Text = "Server stopped working...";
                        accept = false;
                    }
                    else
                    {
                        feed.Text = "Problem occured in accept function...";
                    }
                }
            }
        }

        //Receiving messages
        private void Receive(object obj)
        {
            bool connected = true;

            while (connected && !terminating)
            {
                int lenClientSoc = clientSockets.Count();

                Socket thisClient = clientSockets[lenClientSoc - 1];
                try
                {
                    Byte[] buffer = new Byte[500];
                    thisClient.Receive(buffer);
                    string name = Encoding.Default.GetString(buffer).Replace("\0", string.Empty);
                    while (names.Any(item => item == name))
                    {
                        buffer = new Byte[500];
                        thisClient.Send(Encoding.Default.GetBytes("Please enter a different name."));
                        thisClient.Receive(buffer);
                        name = Encoding.Default.GetString(buffer).Replace("\0", string.Empty);
                    }
                    feed.Text += name;
                    feed.Text += " has connected. \n";
                    names.Add(name);

                }
                catch
                {
                    connected = false;
                    if (!terminating)
                    {
                        feed.Text += names[lenClientSoc - 1];
                        feed.Text += " has disconnected. \n";
                    }
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                }


            }
        }

    }
}
