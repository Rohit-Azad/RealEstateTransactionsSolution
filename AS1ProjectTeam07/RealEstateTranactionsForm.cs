using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AS1ProjectTeam07
{
    public partial class RealEstateTranactionsForm : Form
    {
        List<House> realEstateTransactions;

        public RealEstateTranactionsForm()
        {
            InitializeComponent();

            InitializeDataGridViewRealEstateTransactions();

            GetRealEstateTransactionsFromXML();

            DisplayRealEstateTransactions();

            InitializeAllOtherFormControls();

            ResetControlsToDefault();
        }

        private void ResetControlsToDefault()
        {
            //clear all textboxes
            textBoxMaximumArea.Clear();
            textBoxMaximumPrice.Clear();
            textBoxMinimumArea.Clear();
            textBoxMinimumPrice.Clear();

            //unregister all listboxes' event
            listBoxBathroom.SelectedIndexChanged -= ListBox_SelectedIndexChanged;
            listBoxBedroom.SelectedIndexChanged -= ListBox_SelectedIndexChanged;
            listBoxHouseType.SelectedIndexChanged -= ListBox_SelectedIndexChanged;
            listBoxCities.SelectedIndexChanged -= ListBox_SelectedIndexChanged;

            //select all items in the listboxes
            for (int i = 0; i < listBoxBathroom.Items.Count; i++)
                listBoxBathroom.SetSelected(i, true);

            for (int i = 0; i < listBoxBedroom.Items.Count; i++)
                listBoxBedroom.SetSelected(i, true);

            for (int i = 0; i < listBoxHouseType.Items.Count; i++)
                listBoxHouseType.SetSelected(i, true);

            for (int i = 0; i < listBoxCities.Items.Count; i++)
                listBoxCities.SetSelected(i, true);

            //clear checkboxes
            checkBoxAreaSearch.Checked = false;
            checkBoxPriceSearch.Checked = false;

            //redisplay all the transactions
            DisplayRealEstateTransactions();

            // register all listboxes' event handler again
            listBoxBathroom.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBoxBedroom.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBoxHouseType.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBoxCities.SelectedIndexChanged += ListBox_SelectedIndexChanged;

        }

        public void InitializeAllOtherFormControls()
        {
            //clear anything that was set up previously
            listBoxCities.Items.Clear();
            listBoxBedroom.Items.Clear();
            listBoxBathroom.Items.Clear();
            listBoxHouseType.Items.Clear();

            //ensure listbox selection can be multiple and can use shift, ctl-a, etc
            listBoxCities.SelectionMode = SelectionMode.MultiExtended;            
            listBoxBedroom.SelectionMode = SelectionMode.MultiExtended;            
            listBoxBathroom.SelectionMode = SelectionMode.MultiExtended;            
            listBoxHouseType.SelectionMode = SelectionMode.MultiExtended;

            //adding distinct cities to listbox
            var cities = from houses in realEstateTransactions
                                 orderby houses.City
                                 select houses.City;
            listBoxCities.Items.AddRange(cities.Distinct().ToArray());
            for (int i = 0; i < listBoxCities.Items.Count; i++)
                listBoxCities.SetSelected(i, true);

            //adding number of bedrooms to listbox
            var bedrooms = from houses in realEstateTransactions
                         orderby houses.Bedrooms
                         orderby houses.Bathrooms
                         select  houses.Bedrooms.ToString();
            listBoxBedroom.Items.AddRange(bedrooms.Distinct().ToArray());
            for (int i = 0; i < listBoxBedroom.Items.Count; i++)
                listBoxBedroom.SetSelected(i, true);

            //adding bathrooms to listbox
            var bathrooms = from houses in realEstateTransactions
                         orderby houses.Bathrooms
                         select houses.Bathrooms.ToString();
            listBoxBathroom.Items.AddRange(bathrooms.Distinct().ToArray());
            for (int i = 0; i < listBoxBathroom.Items.Count; i++)
                listBoxBathroom.SetSelected(i, true);

            //adding distinct housetypes to listbox
            var houseTypes = from houses in realEstateTransactions
                         orderby houses.HouseType
                         select houses.HouseType;
            listBoxHouseType.Items.AddRange(houseTypes.Distinct().ToArray());
            for (int i = 0; i < listBoxHouseType.Items.Count; i++)
                listBoxHouseType.SetSelected(i, true);

            //register all listboxes' event 
            listBoxBathroom.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBoxBedroom.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBoxHouseType.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBoxCities.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            //register checkboxes and textboxes' events
            checkBoxPriceSearch.CheckedChanged += CheckBoxPriceSearch_CheckedChanged;
            textBoxMaximumPrice.TextChanged += TextBoxPrice_TextChanged;
            textBoxMinimumPrice.TextChanged += TextBoxPrice_TextChanged;

            checkBoxAreaSearch.CheckedChanged += CheckBoxAreaSearch_CheckedChanged;
            textBoxMinimumArea.TextChanged += TextBoxPrice_TextChanged;
            textBoxMaximumArea.TextChanged += TextBoxPrice_TextChanged;

            //register button event
            buttonReset.Click += ButtonReset_Click;
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            ResetControlsToDefault();
        }

        private void CheckBoxAreaSearch_CheckedChanged(object sender, EventArgs e)
        {
            DisplayRealEstateTransactions();
        }

        private void TextBoxPrice_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxPriceSearch.Checked == true || checkBoxAreaSearch.Checked == true)
                DisplayRealEstateTransactions();
        }
       
        private void CheckBoxPriceSearch_CheckedChanged(object sender, EventArgs e)
        {
            DisplayRealEstateTransactions();
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayRealEstateTransactions();
        }


        public void InitializeDataGridViewRealEstateTransactions()
        {
            //First Gridiew Control
            dataGridViewRealEstate.ReadOnly = true;
            dataGridViewRealEstate.AllowUserToAddRows = false;
            dataGridViewRealEstate.RowHeadersVisible = false;
            dataGridViewRealEstate.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewRealEstate.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewRealEstate.AllowUserToDeleteRows = false;
            dataGridViewRealEstate.RowHeadersWidth = 30;
            dataGridViewRealEstate.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewRealEstate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //Second Gridiew Control
            dataGridViewRealEstateSelected.ReadOnly = true;
            dataGridViewRealEstateSelected.AllowUserToAddRows = false;
            dataGridViewRealEstateSelected.RowHeadersVisible = false;
            dataGridViewRealEstateSelected.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewRealEstateSelected.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewRealEstateSelected.AllowUserToDeleteRows = false;
            dataGridViewRealEstateSelected.RowHeadersWidth = 30;
            dataGridViewRealEstateSelected.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewRealEstateSelected.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewRealEstate.Columns.Clear();
            dataGridViewRealEstateSelected.Columns.Clear();

            //create an array of column names for both DataGridView
            DataGridViewTextBoxColumn[] columns1 = new DataGridViewTextBoxColumn[] {
                    new DataGridViewTextBoxColumn() { Name = "City" },
                    new DataGridViewTextBoxColumn() { Name = "Address" },
                    new DataGridViewTextBoxColumn() { Name = "Bedrooms" },
                    new DataGridViewTextBoxColumn() { Name = "Bathrooms" },
                    new DataGridViewTextBoxColumn() { Name = "SurfaceArea" },
                    new DataGridViewTextBoxColumn() { Name = "HouseType" },
                    new DataGridViewTextBoxColumn() { Name = "Price" }
                    };
            DataGridViewTextBoxColumn[] columns2 = new DataGridViewTextBoxColumn[] {
                    new DataGridViewTextBoxColumn() { Name = "City" },
                    new DataGridViewTextBoxColumn() { Name = "Address" },
                    new DataGridViewTextBoxColumn() { Name = "Bedrooms" },
                    new DataGridViewTextBoxColumn() { Name = "Bathrooms" },
                    new DataGridViewTextBoxColumn() { Name = "SurfaceArea" },
                    new DataGridViewTextBoxColumn() { Name = "HouseType" },
                    new DataGridViewTextBoxColumn() { Name = "Price" }
                    };

            //add the array to both DataGridView
            dataGridViewRealEstate.Columns.AddRange(columns1);
            dataGridViewRealEstateSelected.Columns.AddRange(columns2);
        }

        /// <summary>
        /// Use Deserialize to get all RentalHousing from XML file
        /// </summary>
        private void GetRealEstateTransactionsFromXML()
        {
            //open xml file
            try
            {
                OpenFileDialog openFileDialogXML = new OpenFileDialog
                {
                    InitialDirectory = Path.GetFullPath(Application.StartupPath + "\\..\\.."),
                    Filter = "XML files|*.XML"
                };

                StreamReader rentalFile;

                if (openFileDialogXML.ShowDialog() == DialogResult.OK)
                {
                    rentalFile = File.OpenText(openFileDialogXML.FileName);
                }
                else return;

                // create the serializer
                XmlSerializer realEstateSerializer = new XmlSerializer(typeof(List<House>));

                // deserializing to the list
                realEstateTransactions = realEstateSerializer.Deserialize(rentalFile) as List<House>;
                rentalFile.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        public void DisplayRealEstateTransactions()
        {
            //query to select everthing in realEstateTransactions list
            var topView = from house in realEstateTransactions
                             orderby house.Price
                             orderby house.HouseType
                             orderby house.City
                             select house;

            //display everthing in the top DataGridView
            foreach (House house in topView)
            {
                dataGridViewRealEstate.Rows.Add(new String[]
                {   house.City,
                    house.Address,
                    house.Bedrooms.ToString(),
                    house.Bathrooms.ToString(),
                    house.SurfaceArea.ToString(),
                    house.HouseType,
                    house.Price.ToString()
                });
            }
            //count total transactions
            labelCount.Text = topView.Count().ToString();
           
            //query to find the average price
            var averagePrice = from housePrice in realEstateTransactions
                               select housePrice.Price;
            labelAveragePrice.Text = averagePrice.Average().ToString("C2");

            bool isNumMin;
            bool isNumMax;

            //check if any prices is entered in the price textboxes or is numeric and then parse them
            int minPrice = 0;
            int maxPrice = 0;            
            if (checkBoxPriceSearch.Checked == true)
            {
                isNumMin = int.TryParse(textBoxMinimumPrice.Text, out minPrice);
                isNumMax = int.TryParse(textBoxMaximumPrice.Text, out maxPrice);

                if (isNumMin == false || isNumMax == false)
                {
                    MessageBox.Show("Price is missing or not an integer");
                    checkBoxPriceSearch.Checked = false;
                }
                    
            }
            //check if any prices is entered in the area textboxes or is numeric and then parse them
            int minArea = 0;
            int maxArea = 0;
            if (checkBoxAreaSearch.Checked == true)
            {
                isNumMax = int.TryParse(textBoxMaximumArea.Text, out maxArea);
                isNumMin = int.TryParse(textBoxMinimumArea.Text, out minArea);

                if (isNumMin == false || isNumMax == false)
                {
                    MessageBox.Show("Area is missing or not an integer");
                    checkBoxAreaSearch.Checked = false;
                }
            }

            //create lists of chosen items from all the listBoxs
            List<string> selectedCities = new List<string>();
            List<string> selectedBedrooms = new List<string>();
            List<string> selectedBathrooms = new List<string>();
            List<string> selectedHouseTypes = new List<string>();

            for (int i = 0; i < listBoxCities.SelectedItems.Count; i++)
                selectedCities.Add(listBoxCities.SelectedItems[i].ToString());

            for (int i = 0; i < listBoxBedroom.SelectedItems.Count; i++)
                selectedBedrooms.Add(listBoxBedroom.SelectedItems[i].ToString());

            for (int i = 0; i < listBoxBathroom.SelectedItems.Count; i++)
                selectedBathrooms.Add(listBoxBathroom.SelectedItems[i].ToString());

            for (int i = 0; i < listBoxHouseType.SelectedItems.Count; i++)
                selectedHouseTypes.Add(listBoxHouseType.SelectedItems[i].ToString());

            //query to create a selectedHousing list of realEstateTransactions
            var selectedHousing = from housing in realEstateTransactions
                                  join cities in selectedCities on housing.City equals cities
                                  join bedrooms in selectedBedrooms on housing.Bedrooms.ToString() equals bedrooms
                                  join bathrooms in selectedBathrooms on housing.Bathrooms.ToString() equals bathrooms
                                  join types in selectedHouseTypes on housing.HouseType equals types
                                  where (checkBoxPriceSearch.Checked == true &&  housing.Price > minPrice && housing.Price < maxPrice || checkBoxPriceSearch.Checked == false)
                                  where (checkBoxAreaSearch.Checked == true && housing.SurfaceArea > minArea && housing.SurfaceArea < maxArea || checkBoxAreaSearch.Checked == false)
                                  orderby housing.City, housing.HouseType, housing.Price
                                  select housing;

            dataGridViewRealEstateSelected.Rows.Clear();

            //display query result in datagridview
            foreach (House housing in selectedHousing)
                dataGridViewRealEstateSelected.Rows.Add(housing.City, housing.Address,
                    housing.Bedrooms, housing.Bathrooms, housing.SurfaceArea, housing.HouseType, housing.Price);

            //count and show the average price and the total number of transactions
            int averagePriceSelected = selectedHousing.Count();
            labelAveragePriceSelected.Text = averagePriceSelected.ToString();

            labelCountSelected.Text = selectedHousing.Count().ToString();

            if (averagePriceSelected > 0)
                labelAveragePriceSelected.Text = selectedHousing.Average(s => s.Price).ToString("C2");
            else
                labelAveragePriceSelected.Text = "0";
    
        }

        
    }
    [Serializable]
    public class House
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string HouseType { get; set; }
        public int SurfaceArea { get; set; }
        public int Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public override string ToString()
        {
            return $"{City},{Address},{Bedrooms},{Bathrooms},{SurfaceArea},{HouseType},{Price}";
        }
    }
}
