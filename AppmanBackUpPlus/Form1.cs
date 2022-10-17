using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.IO;

namespace AppmanBackUpPlus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            groupBox2.Visible = false;
            dataGridView1.Visible = false;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_server.Text != "")
                {
                    if (cb_windowsauth.Checked == true)
                    {
                        string connectionString;
                        if (cb_windowsauth.Checked == true)
                        {
                            connectionString = "Data Source=" + txt_server.Text + ";Trusted_Connection=true";
                        }
                        else
                        {
                            connectionString = "Data Source=" + txt_server.Text + "; user id=" + txt_username.Text + ";password=" + txt_password.Text;
                        }
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            con.Open();
                            using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases where name not in('tempdb', 'master','model','msdb')", con))
                            {

                                using (SqlDataReader dr = cmd.ExecuteReader())
                                {
                                    DataTable dtCustomers = new DataTable();
                                    dtCustomers.Load(dr);
                                    dataGridView1.DataSource = dtCustomers;
                                    groupBox2.Visible = true;
                                    txt_server.Enabled = false;
                                    txt_username.Enabled = false;
                                    txt_password.Enabled = false;
                                    cb_windowsauth.Enabled = false;
                                    dataGridView1.Visible = true;

                                }
                            }
                        }
                    }
                    else
                    {
                        if(txt_username.Text !="" && txt_password.Text != "")
                        {
                            string connectionString;
                            if (cb_windowsauth.Checked == true)
                            {
                                connectionString = "Data Source=" + txt_server.Text + ";Trusted_Connection=true";
                            }
                            else
                            {
                                connectionString = "Data Source=" + txt_server.Text + "; user id=" + txt_username.Text + ";password=" + txt_password.Text;
                            }
                            using (SqlConnection con = new SqlConnection(connectionString))
                            {
                                con.Open();
                                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases where name not in('tempdb', 'master','model','msdb')", con))
                                {

                                    using (SqlDataReader dr = cmd.ExecuteReader())
                                    {
                                        DataTable dtCustomers = new DataTable();
                                        dtCustomers.Load(dr);
                                        dataGridView1.DataSource = dtCustomers;
                                        groupBox2.Visible = true;
                                        txt_server.Enabled = false;
                                        txt_username.Enabled = false;
                                        txt_password.Enabled = false;
                                        cb_windowsauth.Enabled = false;
                                        dataGridView1.Visible = true;

                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please Enter User Name & Password.");
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please Enter Server.");
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void cb_windowsauth_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_windowsauth.Checked)
            {
                txt_username.Text = "";
                txt_password.Text = "";
                txt_username.Enabled = false;
                txt_password.Enabled = false;
            }
            else
            {
                txt_username.Enabled = true;
                txt_password.Enabled = true;
            }
        }

        private void btn_backup_Click(object sender, EventArgs e)
        {
            

            try
            {
                if (txt_backuplocation.Text != "")
                {
                    string destinationPath = txt_backuplocation.Text;

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string databaseName = row.Cells[0].Value.ToString();//dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();    

                        //Define a Backup object variable.    
                        Backup sqlBackup = new Backup();

                        ////Specify the type of backup, the description, the name, and the database to be backed up.    
                        sqlBackup.Action = BackupActionType.Database;
                        sqlBackup.BackupSetDescription = "BackUp of:" + databaseName + "on" + DateTime.Now.ToShortDateString();
                        sqlBackup.BackupSetName = "FullBackUp";
                        sqlBackup.Database = databaseName;

                        ////Declare a BackupDeviceItem    
                        //string destinationPath = txt_backuplocation.Text + @"\" + System.DateTime.Now;
                        string backupfileName = databaseName + "_" + System.DateTime.Now.ToShortDateString() + ".bak";
                        BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath + "\\" + backupfileName, DeviceType.File);
                        ////Define Server connection    

                        //ServerConnection connection = new ServerConnection(frm.serverName, frm.userName, frm.password);    
                        ServerConnection connection;
                        if (cb_windowsauth.Checked)
                        {
                            connection = new ServerConnection(txt_server.Text);
                        }
                        else
                        {
                            connection = new ServerConnection(txt_server.Text, txt_username.Text, txt_password.Text);

                        }
                        ////To Avoid TimeOut Exception    
                        Server sqlServer = new Server(connection);
                        sqlServer.ConnectionContext.StatementTimeout = 60 * 60;
                        Database db = sqlServer.Databases[databaseName];

                        sqlBackup.Initialize = true;
                        sqlBackup.Checksum = true;
                        sqlBackup.ContinueAfterError = true;

                        ////Add the device to the Backup object.    
                        sqlBackup.Devices.Add(deviceItem);
                        ////Set the Incremental property to False to specify that this is a full database backup.    
                        sqlBackup.Incremental = false;

                        sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);
                        ////Specify that the log must be truncated after the backup is complete.    
                        sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

                        sqlBackup.FormatMedia = false;
                        ////Run SqlBackup to perform the full database backup on the instance of SQL Server.    
                        sqlBackup.SqlBackup(sqlServer);
                        ////Remove the backup device from the Backup object.    
                        sqlBackup.Devices.Remove(deviceItem);
                        // toolStripStatusLabel1.Text = "Successful backup is created!";


                    }
                    MessageBox.Show("All User Database BackedUp Successfully");

                }
                else
                {
                    MessageBox.Show("Please Enter BackUp Location.");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            txt_server.Enabled = true;
            txt_username.Enabled = true;
            txt_password.Enabled = true;
            cb_windowsauth.Enabled = true;
            groupBox2.Visible = false;
            dataGridView1.Visible = false;
        }
    }
}
