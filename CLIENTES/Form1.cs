using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace ClienteManager
{
    public partial class Form1 : Form
    {
        private List<Cliente> clientes = new List<Cliente>();
        private string dataFilePath = "clientes.json";

        public Form1()
        {
            InitializeComponent();
            LoadClientesFromFile();

            // Definir o contexto de licença do EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Adicionar o evento TextChanged para txt_nome
            txt_nome.TextChanged += txt_nome_TextChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AtualizarListaClientes();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string nome = txt_nome.Text;
            string numero = txt_numero.Text;
            DateTime dataAtivacao = dateTimePicker1.Value;
            string nomeTecnico = comboBoxTecnico.SelectedItem.ToString();

            Cliente cliente = new Cliente
            {
                Nome = nome,
                Numero = numero,
                DataAtivacao = dataAtivacao,
                NomeTecnico = nomeTecnico
            };

            clientes.Add(cliente);
            AtualizarListaClientes();
            SaveClientesToFile();

            txt_nome.Clear();
            txt_numero.Clear();
            dateTimePicker1.Value = DateTime.Now;
            comboBoxTecnico.SelectedIndex = -1;
        }

        private void AtualizarListaClientes()
        {
            listBox1.Items.Clear();
            foreach (var cliente in clientes)
            {
                listBox1.Items.Add(cliente.ToString());
            }
        }

        private void AtualizarListaClientesPorData()
        {
            listBox1.Items.Clear();
            DateTime dataSelecionada = dateTimePicker2.Value.Date;
            var clientesFiltrados = clientes.Where(c => c.DataAtivacao.Date == dataSelecionada).ToList();

            foreach (var cliente in clientesFiltrados)
            {
                listBox1.Items.Add(cliente.ToString());
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            AtualizarListaClientesPorData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DateTime dataSelecionada = dateTimePicker2.Value.Date;
            var clientesFiltrados = clientes.Where(c => c.DataAtivacao.Date == dataSelecionada).ToList();

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog.Title = "Salvar Lista de Clientes";
                saveFileDialog.FileName = "clientes.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo file = new FileInfo(saveFileDialog.FileName);
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Clientes");

                        // Escrever cabeçalho
                        worksheet.Cells[1, 1].Value = "Nome";
                        worksheet.Cells[1, 2].Value = "Telefone";
                        worksheet.Cells[1, 3].Value = "Técnico";

                        // Escrever dados dos clientes
                        int row = 2;
                        foreach (var cliente in clientesFiltrados)
                        {
                            worksheet.Cells[row, 1].Value = cliente.Nome;
                            worksheet.Cells[row, 2].Value = cliente.Numero;
                            worksheet.Cells[row, 3].Value = cliente.NomeTecnico;
                            row++;
                        }

                        package.Save();
                    }

                    MessageBox.Show("Lista de clientes baixada com sucesso!");
                }
            }
        }

        private void SaveClientesToFile()
        {
            string json = JsonConvert.SerializeObject(clientes, Formatting.Indented);
            File.WriteAllText(dataFilePath, json);
        }

        private void LoadClientesFromFile()
        {
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                clientes = JsonConvert.DeserializeObject<List<Cliente>>(json) ?? new List<Cliente>();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveClientesToFile();
        }

        private void txt_nome_TextChanged(object sender, EventArgs e)
        {
            txt_nome.Text = txt_nome.Text.ToUpper();
            txt_nome.SelectionStart = txt_nome.Text.Length;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxTecnico_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            LimparClientesPorData();
        }

        private void LimparClientesPorData()
        {
            DateTime dataSelecionada = dateTimePicker2.Value.Date;
            var clientesFiltrados = clientes.Where(c => c.DataAtivacao.Date == dataSelecionada).ToList();

            foreach (var cliente in clientesFiltrados)
            {
                clientes.Remove(cliente);
            }

            AtualizarListaClientes();
            SaveClientesToFile();
        }

    }


}


    public class Cliente
    {
        public string Nome { get; set; }
        public string Numero { get; set; }
        public DateTime DataAtivacao { get; set; }
        public string NomeTecnico { get; set; }

        public override string ToString()
        {
            return $"{Nome} - {Numero} - {DataAtivacao.ToShortDateString()} - {NomeTecnico}";
        }
    }

