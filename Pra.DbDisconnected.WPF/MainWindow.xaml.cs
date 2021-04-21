using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.ComponentModel;

namespace Pra.DbDisconnected.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        DataSet dsBooks = new DataSet("Bibliotheek");
        string xmlDirectory = Directory.GetCurrentDirectory() + "/XMLBestanden";
        string xmlFile = Directory.GetCurrentDirectory() + "/XMLBestanden/boeken.xml";
        
        private bool ReadXml()
        {
            bool read = false;
            if (Directory.Exists(xmlDirectory))
            {
                if (File.Exists(xmlFile))
                {
                    dsBooks.ReadXml(xmlFile, XmlReadMode.ReadSchema);
                    read = true;
                }
            }
            return read;
        }

        private void CreateTables()
        {
            // creatie dtAuteur
            DataTable dtAuthor;
            dtAuthor = new DataTable();
            dsBooks.Tables.Add(dtAuthor);

            dtAuthor.TableName = "Auteur";

            DataColumn dcAuthorId = new DataColumn();
            dcAuthorId.ColumnName = "auteurID";
            dcAuthorId.DataType = typeof(int);
            dcAuthorId.AutoIncrement = true;
            dcAuthorId.AutoIncrementSeed = 1;
            dcAuthorId.AutoIncrementStep = 1;
            dcAuthorId.Unique = true;
            dtAuthor.Columns.Add(dcAuthorId);
            dtAuthor.PrimaryKey = new DataColumn[] { dcAuthorId };

            DataColumn dcAuthorName = new DataColumn();
            dcAuthorName.ColumnName = "auteurNaam";
            dcAuthorName.DataType = typeof(string);
            dcAuthorName.MaxLength = 50;
            dtAuthor.Columns.Add(dcAuthorName);

            DataColumn dcDisplayAuthor = new DataColumn("DisplayAuteur");
            dcDisplayAuthor.Expression = "auteurNaam + ' (' + auteurID + ')'";
            dtAuthor.Columns.Add(dcDisplayAuthor);

            // creatie dtUitgever
            DataTable dtPublisher = new DataTable();
            dtPublisher.TableName = "Uitgever";
            dsBooks.Tables.Add(dtPublisher);

            DataColumn dcPublisherId = new DataColumn();
            dcPublisherId.ColumnName = "uitgeverID";
            dcPublisherId.DataType = typeof(int);
            dcPublisherId.AutoIncrement = true;
            dcPublisherId.AutoIncrementSeed = 1;
            dcPublisherId.AutoIncrementStep = 1;
            dcPublisherId.Unique = true;

            DataColumn dcPUblisherName = new DataColumn();
            dcPUblisherName.ColumnName = "uitgeverNaam";
            dcPUblisherName.DataType = typeof(string);
            dcPUblisherName.MaxLength = 50;

            dtPublisher.Columns.Add(dcPublisherId);
            dtPublisher.Columns.Add(dcPUblisherName);
            dtPublisher.PrimaryKey = new DataColumn[] { dcPublisherId };

            // creatie boeken
            DataTable dtBooks = new DataTable();
            dtBooks.TableName = "Boeken";
            dsBooks.Tables.Add(dtBooks);

            DataColumn dcBookId = new DataColumn();
            dcBookId.ColumnName = "boekID";
            dcBookId.DataType = typeof(int);
            dcBookId.AutoIncrement = true;
            dcBookId.AutoIncrementSeed = 1;
            dcBookId.AutoIncrementStep = 1;
            dcBookId.Unique = true;
            dtBooks.Columns.Add(dcBookId);
            dtBooks.PrimaryKey = new DataColumn[] { dcBookId };

            // de overige velden voegen we op een kortere manier toe
            dtBooks.Columns.Add("Titel", typeof(string));
            dtBooks.Columns.Add("AuteurID", typeof(int));
            dtBooks.Columns.Add("UitgeverID", typeof(int));
            dtBooks.Columns.Add("Jaartal", typeof(int));

            dsBooks.Relations.Add(dtAuthor.Columns["auteurID"], dtBooks.Columns["AuteurID"]);
        }

        private void FillTables()
        {
            AddAuthor("Boon Louis");
            AddAuthor("Tuchman Barbara");
            AddAuthor("Cook Robin");
            AddPublisher("AW Bruna");
            AddPublisher("Luttingh");
        }

        private void AddAuthor(string name)
        {
            DataTable authors = dsBooks.Tables["Auteur"];
            DataRow newAuthor = authors.NewRow();
            newAuthor["auteurNaam"] = name;
            authors.Rows.Add(newAuthor);
        }

        private void RemoveAuthor(string authorId)
        {
            DataTable authors = dsBooks.Tables["Auteur"];
            foreach (DataRow row in authors.Rows)
            {
                if (row["auteurID"].ToString() == authorId)
                {
                    authors.Rows.Remove(row);
                    break;
                }
            }
        }
        private void AddPublisher(string name)
        {
            DataTable publishers = dsBooks.Tables["Uitgever"];
            DataRow newPublisher = publishers.NewRow();
            newPublisher["UitgeverNaam"] = name;
            publishers.Rows.Add(newPublisher);
        }

        private void AddBook(string title, int authorId, int publisherId, int year)
        {
            DataTable books = dsBooks.Tables["Boeken"];
            DataRow newBook = books.NewRow();
            newBook["Titel"] = title;
            newBook["AuteurID"] = authorId;
            newBook["UitgeverID"] = publisherId;
            newBook["Jaartal"] = year;
            books.Rows.Add(newBook);
        }

        private void UpdateAuthorAndPublisherComboBoxes()
        {
            ComboBoxItem itm;
            
            cmbAuthors.Items.Clear();
            cmbPublishers.Items.Clear();

            DataTable authors = dsBooks.Tables["Auteur"];
            for (int counter = 0; counter < authors.Rows.Count; counter++)
            {
                itm = new ComboBoxItem();
                itm.Content = authors.Rows[counter]["auteurNaam"];
                itm.Tag = authors.Rows[counter]["auteurID"];
                cmbAuthors.Items.Add(itm);
            }
            cmbAuthors.SelectedIndex = 0;

            DataTable publishers = dsBooks.Tables["Uitgever"];
            for (int counter = 0; counter < publishers.Rows.Count; counter++)
            {
                itm = new ComboBoxItem();
                itm.Content = publishers.Rows[counter]["uitgeverNaam"];
                itm.Tag = publishers.Rows[counter]["uitgeverID"];
                cmbPublishers.Items.Add(itm);
            }
            cmbPublishers.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ReadXml())
            {
                CreateTables();
                FillTables();
            }
            dgAuthors.ItemsSource = dsBooks.Tables["Auteur"].DefaultView;
            dgBooks.ItemsSource = dsBooks.Tables["Boeken"].DefaultView;
            UpdateAuthorAndPublisherComboBoxes();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Directory.Exists(xmlDirectory))
                Directory.CreateDirectory(xmlDirectory);
            if (File.Exists(xmlFile))
                File.Delete(xmlFile);
            dsBooks.WriteXml(xmlFile, XmlWriteMode.WriteSchema);
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            DataView sortedTable = new DataView();
            sortedTable.Table = dsBooks.Tables["Auteur"];
            sortedTable.Sort = "AuteurNaam desc, AuteurID desc";
            dgAuthors.ItemsSource = sortedTable;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            DataView filteredTable = new DataView(dsBooks.Tables["Auteur"]);
            filteredTable.RowFilter = "AuteurNaam like 'T%'";
            dgAuthors.ItemsSource = filteredTable;
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            string author = txtAuthor.Text.Trim();
            AddAuthor(author);
            dgAuthors.ItemsSource = dsBooks.Tables["Auteur"].DefaultView;
            UpdateAuthorAndPublisherComboBoxes();
            txtAuthor.Clear();
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title = txtTitle.Text;
                int year = int.Parse(txtYear.Text);
                ComboBoxItem itm;
                itm = (ComboBoxItem)cmbAuthors.SelectedItem;
                int authorId = int.Parse(itm.Tag.ToString());
                itm = (ComboBoxItem)cmbPublishers.SelectedItem;
                int publisherId = int.Parse(itm.Tag.ToString());

                AddBook(title, authorId, publisherId, year);

                dgBooks.ItemsSource = dsBooks.Tables["Boeken"].DefaultView;
                txtTitle.Clear();
                txtYear.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Foute ingave \nReden :" + ex.Message);
            }
        }

        private void RemoveAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (dgAuthors.SelectedIndex > -1)
            {
                string authorId = dgAuthors.SelectedValue.ToString();
                RemoveAuthor(authorId);
                UpdateAuthorAndPublisherComboBoxes();
                dgAuthors.ItemsSource = dsBooks.Tables["Auteur"].DefaultView;
                dgBooks.ItemsSource = dsBooks.Tables["Boeken"].DefaultView;
            }
        }
    }
}
