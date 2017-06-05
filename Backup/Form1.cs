using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simplex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //DataTable matrix = new DataTable();
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            lstVariavelBase.Items.Add("F" + (lstVariavelBase.Items.Count + 1).ToString());
        }

        private void btnVarNaoBase_Click(object sender, EventArgs e)
        {
            lstNaoBase.Items.Add("X" + (lstNaoBase.Items.Count + 1).ToString());
        }
        
        private void btnMontarMatriz_Click(object sender, EventArgs e)
        {
            DataTable m = new DataTable();
            m = new DataTable();
            m.Columns.Add("Base", typeof(string));
            for (int i = 0;i < lstNaoBase.Items.Count; i++)
                m.Columns.Add(lstNaoBase.Items[i].ToString(), typeof(float));
            for (int i = 0; i < lstVariavelBase.Items.Count; i++)
                m.Columns.Add(lstVariavelBase.Items[i].ToString(), typeof(float));
            m.Columns.Add("B", typeof(float));
            object[] Linha = new object[m.Columns.Count];
            for (int i = 0; i < lstVariavelBase.Items.Count; i++)
            {                
                Linha[0] = lstVariavelBase.Items[i].ToString();
                for (int j = 1; j < m.Columns.Count; j++)
                {
                    if (m.Columns[j].ColumnName.ToString() == Linha[0].ToString())
                        Linha[j] = 1.0;
                    else
                        Linha[j] = 0.0;                   
                }
                m.Rows.Add(Linha);
            }
            Linha[0] = "Z";
            Linha[Linha.Length - 2] = 0;
            m.Rows.Add(Linha);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.DataSource = m;
            dataGridView1.Update();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (results != null)
            {
                results.Close();
                results.Dispose();
                results = null;
            }
            if (results == null)
                results = new frmResults();

            results.Show();
            DataGridView dr1 = new DataGridView();
            dr1.DataSource = ((DataTable)dataGridView1.DataSource).Copy();
            dr1.Dock = DockStyle.Top;
            dr1.Update();
            results.AdicionarControle(dr1);

            dataGridView1.DataSource =  CalcularSimplex((DataTable)dataGridView1.DataSource);
            dataGridView1.Update();
            


            foreach (DataRow dr in ((DataTable)dataGridView1.DataSource).Rows)
            {
                if (dr[0].ToString().Substring(0, 1) == "X")
                    lstResultado.Items.Add(dr[0].ToString() + " = " + dr[dr.ItemArray.Length - 1].ToString());
            }
        }

        private DataTable CalcularSimplex(DataTable m)
        {
            int iPivo = 0;
            int jPivo = 0;
            int iMax = m.Rows.Count - 1;
            int jMax = m.Columns.Count - 2;            
            float[,] z = new float[m.Rows.Count, m.Columns.Count - 1];
            for (int i = 0; i < m.Rows.Count; i++)
                for (int j = 0; j < m.Columns.Count - 1; j++)
                    z[i, j] = float.Parse(m.Rows[i][j + 1].ToString());
            float MenorValor;
            float Pivo;
            // Encontrar a coluna de resultados com o menor valor.
            // jMax-1 ==> Nao olha a coluna B (resultados).
            MenorValor = z[iMax, 0];
            for (int j = 0; j < jMax - 1; j++)
            {
                if (z[iMax, j] < MenorValor)
                {
                    MenorValor = z[iMax, j];
                    jPivo = j;
                }
            }

            // Localizada a coluna do pivo, identificar a linha do Pivo;
            bool primeiroValor = true;            
            for (int i = 0; i < iMax; i++)
            {
                if (z[i, jPivo] > 0 && ((z[i, jMax] / z[i, jPivo]) > 0 && (z[i, jMax] / z[i, jPivo]) < MenorValor || primeiroValor))
                {
                    MenorValor = z[i, jMax] / z[i, jPivo];
                    iPivo = i;
                    primeiroValor = false;
                }
            }

            // INICIAR TRANSFORMACAO DAS LINHAS -- recalcula linha do pivo.
            Pivo = z[iPivo, jPivo];
            for (int j = 0; j <= jMax; j++)
                z[iPivo, j] = z[iPivo, j] / Pivo;
            // CALCULA OUTRAS LINHAS
            float fCoef;
            for (int i = 0; i <= iMax; i++)
                if (i != iPivo)
                {
                    fCoef = z[i, jPivo];
                    for (int j = 0; j <= jMax; j++)
                    {
                        z[i, j] = z[i, j] - fCoef * z[iPivo, j];
                    }
                }

            // EXIBE NA TELA
            m.Rows[iPivo][0] = m.Columns[jPivo + 1].ColumnName;
            for (int i = 0; i <= iMax; i++)
                for (int j = 0; j <= jMax; j++)
                    m.Rows[i][j + 1] = z[i, j];

            
            bool PossuiNegativo = false;
            for (int j = 0; j < jMax; j++)
            {
                if (z[iMax, j] < 0) PossuiNegativo = true;
            }
            if (results == null)
                results = new frmResults();
            results.Show();
            DataGridView dr = new DataGridView();
            dr.DataSource = m.Copy();
            dr.Dock = DockStyle.Top;
            dr.Update();            
            results.AdicionarControle(dr);
            results.WindowState = FormWindowState.Maximized;
            results.Refresh();
            if (PossuiNegativo)
            {                   
                m = CalcularSimplex(m);                
            }
            

            return m;

        }

        frmResults results;

        private void button2_Click(object sender, EventArgs e)
        {
            lstNaoBase.Items.Clear();
            lstVariavelBase.Items.Clear();
            lstResultado.Items.Clear();
            dataGridView1.Columns.Clear();
        }
    }
}
