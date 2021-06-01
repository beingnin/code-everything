using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Canvas.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        protected static IEnumerable<Event> LoadEvents()
        {
            var result = new List<Event>();
            for (int i = 0; i < 50; i++)
            {
               yield return new Event
                {
                    SlNo = i+1,
                    Message = "Some message",
                    Title = "Some title"
                };
            }
        }
        protected static List<Mapping> LoadMappings()
        {
            return new List<Mapping>
            {
                new Mapping
                {
                    SlNo=1,
                    Collection="Collection",
                    Schema="Schema",
                    Status="Active",
                    Table="Table"
                },
                new Mapping
                {
                    SlNo=1,
                    Collection="Collection",
                    Schema="Schema",
                    Status="Active",
                    Table="Table"
                },
                new Mapping
                {
                    SlNo=1,
                    Collection="Collection",
                    Schema="Schema",
                    Status="Active",
                    Table="Table"
                }
            };
        }
        public MainWindow()
        {
            InitializeComponent();
            Event_Grid.ItemsSource = LoadEvents();
            Mappings_Grid.ItemsSource = LoadMappings();
        }


    }
    public class Event
    {
        public int SlNo { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
    public class Mapping
    {
        public int SlNo { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }
        public string Collection { get; set; }
        public string Status { get; set; }
    }
}

