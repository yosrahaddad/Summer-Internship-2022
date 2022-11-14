using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Diagnostics;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        
       SqlDataAdapter  da1, da2;
        SqlConnection cn,cn2;
        DataRow  dro1,dro2;
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();
        DataTable dt3 = new DataTable();
        string path = @"C:\WORK\form.csv";


        public Form1()
        {
              InitializeComponent();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //pour ne pas réecrire les donnees  a chaque ouverture 
             //si le fichier exist et n'est pas vide vous pouvez démarrer vos projet en donnant le chemin de projet
        try { 
               var info = new FileInfo(path);
               if ((info.Exists) & info.Length != 0)
                  {
                    button6.Enabled = true;   
                  }
            }
            catch
            { 
                //
            }

        }


        //boutton serveur prod connecter1
        private void button3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) | String.IsNullOrEmpty(textBox3.Text) | String.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Il vous manque un champ obligatoire pour connecter au serveur PROD");
            }
            try
            {
                //connecter sur serveur prod
                String connectionString = string.Format(@"Data source={0};user Id={1} ;Password={2} ;", textBox1.Text, textBox3.Text, textBox4.Text);
                cn = new SqlConnection(connectionString);
                cn.Open();
                MessageBox.Show(textBox1.Text, "Vous êtes connecté sur ce serveur");

                //afficher les databases du serveur prod
                dt1.Clear();
                da1 = new SqlDataAdapter(" SELECT name FROM sys.databases  where name NOT IN ('master', 'tempdb', 'model', 'msdb')  ", cn);
                da1.Fill(dt1);
                comboBox1.Items.Clear();
                cn.Close();

                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    dro1 = dt1.Rows[j];
                    comboBox1.Items.Add(dro1[0].ToString());
                }
                
            }
            catch
            {
                MessageBox.Show(textBox1.Text, "Problème de connexion sur ce serveur  ");

            }
           
        }


        //boutton connecter2 serveur bi
        private void button4_Click(object sender, EventArgs e)
        {
            
            //nom de serveur ,nom d'utilisateur, pwd obligatoire non null
            if (String.IsNullOrEmpty(textBox2.Text)| String.IsNullOrEmpty(textBox5.Text)| String.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Il vous manque un champ obigatoire pour connecter au serveur BI");
            }

            try
            {
                //connecter sur serveur BI
                String connectionBI = string.Format(@"Data source={0}; User ID={1} ;Password={2};", textBox2.Text, textBox5.Text, textBox6.Text);
                cn2 = new SqlConnection(connectionBI);
                cn2.Open();
                MessageBox.Show(textBox2.Text, "Vous êtes connecté sur ce serveur");
                dt2.Clear();
                da2 = new SqlDataAdapter(" SELECT name FROM sys.databases  where name NOT IN ('master', 'tempdb', 'model', 'msdb')  ", cn2);
                comboBox3.Items.Clear();
                comboBox2.Items.Clear();
                da2.Fill(dt2);
                cn2.Close();

                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    dro2 = dt2.Rows[i];
                    comboBox3.Items.Add(dro2[0].ToString());
                    comboBox2.Items.Add(dro2[0].ToString());
                }
                
            }
            catch
            {
                MessageBox.Show(textBox2.Text, "Problème de connexion sur ce serveur  ");
               
            }

        }


        //button valider pour enregistrer les donnees dans le fichier csv
        private void button1_Click(object sender, EventArgs e)
        {
           
                //creer ou ouvrir fichier pour enregister les données
                //path de fichier
                

                FileStream csvPath = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using (StreamWriter writer = new StreamWriter(csvPath))
                {
                //les noms de serveurs, base prod , base dwh :non null obligatoirement
                    if (!String.IsNullOrEmpty(textBox1.Text) & !String.IsNullOrEmpty(textBox2.Text) & !String.IsNullOrEmpty(comboBox1.Text) & !String.IsNullOrEmpty(comboBox2.Text))
                    {

                        writer.WriteLine(textBox1.Text + "  |   " + textBox2.Text + "    |   " + comboBox1.Text + "   |   " + comboBox2.Text + "  |   " + comboBox3.Text + "  |     ");
                        MessageBox.Show("Les Données Sont Bien Ajoutées", "Succes");
                        button6.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Verifier les donnèes à insérer ", "Erreur");
                    //si l'un des champs est null
                    }
                }
        }


        //boutton quitter
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //excution du projet, des scripts
        private void button6_Click(object sender, EventArgs e)
        {
            
            ExcutePackage();

        }

        //fct d'execution
        private void ExcutePackage()
        {
           
            Microsoft.SqlServer.Dts.Runtime.DTSExecResult pkgResults;

            if (String.IsNullOrEmpty(textBox8.Text))
            {
                MessageBox.Show("Donner le chemin de votre projet.");

            }
            else
            {
                string[] filePaths = Directory.GetFiles(textBox8.Text);
                foreach (string file in filePaths)
                {

                    Microsoft.SqlServer.Dts.Runtime.Application app1 = new Microsoft.SqlServer.Dts.Runtime.Application();
                    pkgResults = app1.LoadPackage(file, null).Execute();

                    if (pkgResults == Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success)
                    {
                        MessageBox.Show(file, "SUCCES");
                    }
                    else
                    {
                        MessageBox.Show(file,"ERREUR");
                    }

                }
            }

        }

        //boutton Réinitialiser
        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            textBox3.Text = String.Empty;
            textBox4.Text = String.Empty;
            textBox5.Text = String.Empty;
            textBox6.Text = String.Empty;
            textBox8.Text = String.Empty;
            comboBox1.Text = String.Empty;
            comboBox2.Text = String.Empty;
            comboBox3.Text = String.Empty;
        }

        //show or hide password 1
        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if (textBox4.PasswordChar == '*')
            {
                textBox4.PasswordChar = '\0';
            }
            else
            {
                textBox4.PasswordChar = '*';
            }

        }

        //show or hide password 2
        private void pictureBox2_Click(object sender, EventArgs e)
        {

            if (textBox6.PasswordChar == '*')
            {
                textBox6.PasswordChar = '\0';
            }
            else
            {
                textBox6.PasswordChar = '*';
            }

        }


        private void Instance_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
          
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox5_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.PasswordChar = '*';

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            textBox6.PasswordChar = '*';
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

       
        private void label1_Click(object sender, EventArgs e)
        {

        }
        
        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void label7_Click_1(object sender, EventArgs e)
        {

        }
        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void label8_Click_1(object sender, EventArgs e)
        {
            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
        private void label9_Click_1(object sender, EventArgs e)
        {

        }
        private void label10_Click(object sender, EventArgs e)
        {

        }

    }
}





    
        

    






