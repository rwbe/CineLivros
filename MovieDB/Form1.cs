using System;
using System.Data;
using System.Data.OleDb; 
using System.Windows.Forms;

namespace MovieDB
{
    public partial class Form1 : Form
    {
        public OleDbConnection database;
        DataGridViewButtonColumn editButton;
        DataGridViewButtonColumn deleteButton;

        int movieIDInt;

        #region constructor formul�rio
        public Form1()
        {

            InitializeComponent();
            // inicializa string de conex�o
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=moviedb.mdb";
            try
            {
                database = new OleDbConnection(connectionString);
                database.Open();
                //SQL consulta para lista de itens
                string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
                carregaDataGrid(queryString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        #endregion

        #region carrega datagridview
        public void carregaDataGrid(string sqlQueryString) {

            OleDbCommand SQLQuery = new OleDbCommand();
            DataTable data = null;
            dgvItens.DataSource = null;
            SQLQuery.Connection = null;
            OleDbDataAdapter dataAdapter = null;
            dgvItens.Columns.Clear(); // <-- limpa colunas
            //---------------------------------
            SQLQuery.CommandText = sqlQueryString;
            SQLQuery.Connection = database;
            data = new DataTable();
            dataAdapter = new OleDbDataAdapter(SQLQuery);
            dataAdapter.Fill(data);
            dgvItens.DataSource = data;
            dgvItens.AllowUserToAddRows = false; // remove linha nula
            dgvItens.ReadOnly = true;
            dgvItens.Columns[0].Visible = false;
            dgvItens.Columns[1].Width = 340;
            dgvItens.Columns[3].Width = 55;
            dgvItens.Columns[4].Width = 50;
            dgvItens.Columns[5].Width = 80;
            // insere bot�o de editar no datagridview
            editButton = new DataGridViewButtonColumn();
            editButton.HeaderText = "Editar";
            editButton.Text = "Editar";
            editButton.UseColumnTextForButtonValue = true;
            editButton.Width = 80;
            dgvItens.Columns.Add(editButton);
            // insere bot�o de deletar no datagridview
            deleteButton = new DataGridViewButtonColumn();
            deleteButton.HeaderText = "Deletar";
            deleteButton.Text = "Deletar";
            deleteButton.UseColumnTextForButtonValue = true;
            deleteButton.Width = 80;
            dgvItens.Columns.Add(deleteButton);
        }
        #endregion

        private void izlazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        #region fecha conex�o database
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            database.Close();
        }
        #endregion

        #region bot�o atualiza
        private void button2_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
            carregaDataGrid(queryString);
        }
        #endregion

        #region Input
        private void button6_Click(object sender, EventArgs e)
        {
            string categoriaString;
            try
            {
                categoriaString = cboCategoria.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Informe a Categoria\nErro: " + ex.Message + "");
                return;
            }

            int categoria = 0;

            string nome = txtTitulo.Text.ToString();
            string publicadora = txtPublicadora.Text.ToString();
            string ano = txtAno.Text.ToString();

            int _ano = 0;
            if (ano != "")
            {
                _ano = VerificaAno(ano);
            }
            string assistido;
            if (rdbSim.Checked == true)
            {
                assistido = "Sim";
            }
            else
            {
                assistido = "N�o";
            }
            if (_ano != 1)
            {
                if (categoriaString == "Aventura") categoria = 1;
                if (categoriaString == "Com�dia") categoria = 2;
                if (categoriaString == "A��o") categoria = 3;
                if (categoriaString == "Desenho") categoria = 4;
                if (categoriaString == "Romance") categoria = 5;
                if (categoriaString == "Fantasia") categoria = 6;
                if (categoriaString == "Suspense") categoria = 7;
                if (categoriaString == "Hist�rico") categoria = 8;
                if (categoriaString == "Drama") categoria = 9;
                if (categoriaString == "Horror") categoria = 10;
                if (categoriaString == "Fic��o") categoria = 11;
                if (categoriaString == "Crime") categoria = 12;
                if (categoriaString == "Biografia") categoria = 13;
                if (categoriaString == "Document�rio") categoria = 14;
                if (categoriaString == "Livro") categoria = 15;

                string SQLString ="";
     
                    if (ano == "")
                    {
                        SQLString = "INSERT INTO movie(Title, Publisher, Previewed, typeID) VALUES('" + nome.Replace("'", "''") + "','" + publicadora + "','" + assistido + "'," + categoria + ");";
                    }
                    else
                    {
                        //MessageBox.Show(_ano.ToString());
                        SQLString = "INSERT INTO movie(Title, Publisher, Previewed, typeID) VALUES('" + nome.Replace("'", "''") + "','" + publicadora + "','" + assistido + "'," + _ano + "," + categoria + ");";
                    }


                OleDbCommand SQLCommand = new OleDbCommand();
                SQLCommand.CommandText = SQLString;
                SQLCommand.Connection = database;
                int resposta = -1;
                try
                {
                    resposta = SQLCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (resposta >= 1)
                    MessageBox.Show("Item foi inclu�do no banco de dados","Sucesso",MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtTitulo.Clear();
                txtPublicadora.Clear();
                txtAno.Clear();
                cboCategoria.ResetText();
                rdbSim.Checked = rdbNao.Checked = false;
            }
            else
            {
                MessageBox.Show("O formato do ano n�o esta correto!\nInforme um ano v�lido.", "Alerta",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAno.Clear();
                txtAno.Focus();
            }
        }

        public int VerificaAno(string ano)
        {
                int _ano = int.Parse(ano);
                if (_ano >= 2100 || _ano <= 1900)
                {
                    return 1;
                }
                else
                {
                    return _ano;
                }
        }
        #endregion

        #region trata o bot�o Deleta/Edita 
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";

            int linhaAtual = int.Parse(e.RowIndex.ToString());

            if (linhaAtual < 0)
                return;
            
            try
            {
                string movieIDString = dgvItens[0, linhaAtual].Value.ToString();
                movieIDInt = int.Parse(movieIDString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            // editar 
            if (dgvItens.Columns[e.ColumnIndex] == editButton && linhaAtual >= 0)
            {
                try
                {
                    string titulo = dgvItens[1, linhaAtual].Value.ToString();
                    string publicadora = dgvItens[2, linhaAtual].Value.ToString();
                    string assistido = dgvItens[3, linhaAtual].Value.ToString();
                    string ano = dgvItens[4, linhaAtual].Value.ToString();
                    string categoria = dgvItens[5, linhaAtual].Value.ToString();

                    //executa o form2 para edi��o
                    Form2 f2 = new Form2();
                    f2.titulo = titulo;
                    f2.publicadora = publicadora;
                    f2.assistido = assistido;
                    f2.ano = ano;
                    f2.categoria = categoria;
                    f2.itemID = movieIDInt;
                    f2.Show();
                    dgvItens.Update();
                }
                catch { }
            }
            // bot�o deletar
            else if (dgvItens.Columns[e.ColumnIndex] == deleteButton && linhaAtual >= 0)
            {
                if (MessageBox.Show("Confirma ?", "Deletar", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    // consulta sql delete sql
                    string queryDeleteString = "DELETE FROM movie where movieID = " + movieIDInt + "";
                    OleDbCommand sqlDelete = new OleDbCommand();
                    sqlDelete.CommandText = queryDeleteString;
                    sqlDelete.Connection = database;
                    sqlDelete.ExecuteNonQuery();
                    carregaDataGrid(queryString);
                }
            }
         }
        #endregion
         
        #region procurar por titulo
        private void button1_Click(object sender, EventArgs e)
        {
            string titulo = textBox4.Text.ToString();
            if (titulo != "")
            {
                string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID = movie.typeID AND movie.titulo LIKE '" + titulo + "%'";
                carregaDataGrid(queryString);
            }
            else
            {
                MessageBox.Show("Informe o t�tulo do item","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region procurar por Categoria
        private void button5_Click(object sender, EventArgs e)
        {
            int categoria = 0;
            string categoriaString = comboBox2.SelectedItem.ToString();
            if (categoriaString == "Aventura") categoria = 1;
            if (categoriaString == "Com�dia") categoria = 2;
            if (categoriaString == "A��o") categoria = 3;
            if (categoriaString == "Desenho") categoria = 4;
            if (categoriaString == "Romance") categoria = 5;
            if (categoriaString == "Fantasia") categoria = 6;
            if (categoriaString == "Suspense") categoria = 7;
            if (categoriaString == "Hist�rico") categoria = 8;
            if (categoriaString == "Drama") categoria = 9;
            if (categoriaString == "Horror") categoria = 10;
            if (categoriaString == "Fic��o") categoria = 11;
            if (categoriaString == "Crime") categoria = 12;
            if (categoriaString == "Biografia") categoria = 13;
            if (categoriaString == "Document�rio") categoria = 14;
            if (categoriaString == "Livro") categoria = 15;
            
            string queryString = "SELECT movieID, Titulo, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND movie.typeID = " + categoria + "";
            carregaDataGrid(queryString);
        }
        #endregion

        #region procurar por ano
        private void button4_Click(object sender, EventArgs e)
        {
            string primeiroAno = textBox5.Text.ToString();
            string segundoAno = textBox6.Text.ToString();;
            int ano1 = VerificaAno(primeiroAno);
            int ano2 = VerificaAno(segundoAno);

            if ((ano1 != 1 && ano2 != 1) && ano1 <= ano2)
            {
                string queryString = "SELECT movieID, Titulo, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND movie.MovieYear BETWEEN " + ano1 + " AND " + ano2 + "";
                carregaDataGrid(queryString);
            }
            else
            {
                MessageBox.Show("O formato do ano n�o esta correto, inclua um ano v�lido.","Alerta",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Clear();
                textBox5.Focus();
                textBox6.Clear();
            }
        }
        #endregion

        #region procurar itens lidos/assistidos
        private void button3_Click(object sender, EventArgs e)
        {
            string assistido;

            if (radioButton3.Checked == true)
                assistido = "Sim";
            else
                assistido = "N�o";

            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND Previewed ='" + assistido + "'";
            carregaDataGrid(queryString);
        }
        #endregion

        private void button6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(null, null);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
            carregaDataGrid(queryString);
        }

        private void txtAno_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnprod_Click(object sender, EventArgs e)
        {

        }

        private void btnlogoInicio_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            string categoriaString;
            try
            {
                categoriaString = cboCategoria.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Informe a Categoria\nErro: " + ex.Message + "");
                return;
            }

            int categoria = 0;

            string nome = txtTitulo.Text.ToString();
            string publicadora = txtPublicadora.Text.ToString();
            string ano = txtAno.Text.ToString();

            int _ano = 0;
            if (ano != "")
            {
                _ano = VerificaAno(ano);
            }
            string assistido;
            if (rdbSim.Checked == true)
            {
                assistido = "Sim";
            }
            else
            {
                assistido = "N�o";
            }
            if (_ano != 1)
            {
                if (categoriaString == "Aventura") categoria = 1;
                if (categoriaString == "Com�dia") categoria = 2;
                if (categoriaString == "A��o") categoria = 3;
                if (categoriaString == "Desenho") categoria = 4;
                if (categoriaString == "Romance") categoria = 5;
                if (categoriaString == "Fantasia") categoria = 6;
                if (categoriaString == "Suspense") categoria = 7;
                if (categoriaString == "Hist�rico") categoria = 8;
                if (categoriaString == "Drama") categoria = 9;
                if (categoriaString == "Horror") categoria = 10;
                if (categoriaString == "Fic��o") categoria = 11;
                if (categoriaString == "Crime") categoria = 12;
                if (categoriaString == "Biografia") categoria = 13;
                if (categoriaString == "Document�rio") categoria = 14;
                if (categoriaString == "Livro") categoria = 15;

                string SQLString = "";

                if (ano == "")
                {
                    SQLString = "INSERT INTO movie(Title, Publisher, Previewed, typeID) VALUES('" + nome.Replace("'", "''") + "','" + publicadora + "','" + assistido + "'," + categoria + ");";
                }
                else
                {
                    //MessageBox.Show(_ano.ToString());
                    SQLString = "INSERT INTO movie(Title, Publisher, Previewed, typeID) VALUES('" + nome.Replace("'", "''") + "','" + publicadora + "','" + assistido + "'," + _ano + "," + categoria + ");";
                }


                OleDbCommand SQLCommand = new OleDbCommand();
                SQLCommand.CommandText = SQLString;
                SQLCommand.Connection = database;
                int resposta = -1;
                try
                {
                    resposta = SQLCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (resposta >= 1)
                    MessageBox.Show("Item foi inclu�do no banco de dados", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtTitulo.Clear();
                txtPublicadora.Clear();
                txtAno.Clear();
                cboCategoria.ResetText();
                rdbSim.Checked = rdbNao.Checked = false;
            }
            else
            {
                MessageBox.Show("O formato do ano n�o esta correto!\nInforme um ano v�lido.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAno.Clear();
                txtAno.Focus();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
            carregaDataGrid(queryString);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string titulo = textBox4.Text.ToString();
            if (titulo != "")
            {
                string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID AND movie.title LIKE '" + titulo + "%'";
                carregaDataGrid(queryString);
            }
            else
            {
                MessageBox.Show("Informe o t�tulo do item", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.TabStop = false;
        }
    }
}