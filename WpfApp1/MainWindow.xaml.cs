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
using System.Timers;
using System.Threading;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            Btn1.Click += new System.Windows.RoutedEventHandler(buttonpick);
            Btn2.Click += new System.Windows.RoutedEventHandler(canceltime);
            Btn3.Click += new System.Windows.RoutedEventHandler(buttonpick2);

   

            //TaskName.PreviewMouseDown += RemoveText;
            //TaskName.PreviewMouseDown +=  RemoveDescText;
            //TaskDescription.PreviewMouseDown += AddText;
            //TaskDescription.PreviewMouseDown += AddDescText;

            TaskName.Text = "Task Name...";
            TaskDescription.Text = "Task Description...";
            TaskName.GotFocus += RemoveText;
            TaskName.LostFocus += AddText;
            TaskDescription.GotFocus += RemoveDescText;
            TaskDescription.LostFocus += AddDescText;

            CombinationTaskName.GotFocus += RemoveText2;
            CombinationTaskName.LostFocus += AddText2;
            CombinationTaskDesc.GotFocus += RemoveDescText2;
            CombinationTaskDesc.LostFocus += AddDescText2;

            textBox7.GotFocus += RemoveFilename;
            textBox7.LostFocus += AddFilename;
            textBox8.GotFocus += RemoveFilepath;
            textBox8.LostFocus += AddFilePath;

            Grid1.KeyDown += PreviewKeyDown;
            Grid1.PreviewKeyDown += PreviewKeyDown;
            Grid1.TargetUpdated += deselectgrid;
            Grid1.CellEditEnding += deselectgrid;


            combining = true;

            timercommitted = true;


            holdingtime = 0;
            timestarted2 = null;
            bankedtime = 0;
            timespaused = 0;

            running = false;

            Binding bs1 = new Binding();
            bs1.Source = dt;
            bs1.Mode = BindingMode.TwoWay;
            bs1.BindingGroupName = "Grp1";



            if (bs1.Source != null)
            {
                Grid1.DataContext = dt.DefaultView;
            }

            rownumber = 0;

            editmode = false;

            editing = false;

            Grid1.IsEnabled = true;


            textBox7.IsEnabled = false;
            textBox8.IsEnabled = false;
            Btn4_Copy.IsEnabled = false;
            Btn4_Copy1.IsEnabled = false;
            radioButton1.IsEnabled = false;
            radioButton2.IsEnabled = false;
            radioButton3.IsEnabled = false;
            radioButton4.IsEnabled = false;
            checkBox2.IsEnabled = false;
            checkBox2_Copy.IsEnabled = false;
            checkBox3.IsEnabled = false;

            radioButton1.IsChecked = true;
            radioButton3.IsChecked = true;


            Btn2.IsEnabled = false;
            Btn3.IsEnabled = false;
            Btn4.IsEnabled = false;
            Btn4_Copy.IsEnabled = false;

            TaskName.IsEnabled = false;
            TaskDescription.IsEnabled = false;

            CombinationCanvas.Visibility = Visibility.Collapsed;

            gridplaceholder.Visibility = Visibility.Visible;

        }


        // Public stuff

        private static System.Timers.Timer timer;
        public static int timedseconds = 0;
        public static int timedminutes = 0;
        public static int timedhours = 0;
        public static bool timerstate;
        public static bool paused;
        public static bool holdingtimetaken;
        public static bool combined;
        public static bool editmode;
        public static bool columnsedited;
        public static bool editing;
        public static bool previewing;
        public static bool combinationmode;
        public static bool combining;
        //public static bool errorshown;

        public static DateTime? timestarted;
        public static DateTime timefinished;
        public static DateTime startofprocess;
        public static DateTime? timestarted2;
        public static DateTime? timestarted3;
        public static DateTime timefinished2;
        public static DateTime? waitingendtime;

        public static int timeaccuml;
        public static int timeaccuml2;
        public static int holdingtime;
        public static int pausetime;
        public static int bankedtime;
        public static int timespaused;
        public static int diffint2;
        public static int rownumber;
        public static int restarttime;

        public static int linenumber;
        public static int filenumber;

        public static string timestoaddup;
        public static string detailstocombine;
        public static string initialdesc;
        public static string delimiter;
        public static string header1 = "Task";
        public static string header2 = "Description";
        public static string header3 = "Time Elapsed";
        public static string headers;
        public static string alldetails;
        public static string shortened;
        public static string allnames;
        public static string heldtimevalue;
        public static string invalidvalue;

        public static string stringforfile;
        public static string filetype;
        public static string dateappend;
        public static string diff3;

        public static string AmalgamatedTask;
        public static string Spacer;
        public static string timedtemp;
        public static string totalsplittemp;
        public static string rownsame2temp;
        public static string allnamestemp;

        public static string txt2;


        public string taskname;
        public string taskdesc;

        public static DataTable dt;
        public static DataTable dt2;


        // BINDINGS



        // METHODS



        public bool timercommitted = false;
        public bool running;


        //STOP OR GO
        void buttonpick(object sender, RoutedEventArgs e)

        {

            Btn4.FontSize = 12;
            Btn4.Content = "Combine Tasks";
            Btn4.IsEnabled = false;

            if (Grid1.Items.Count > 0)
            {
                Grid1.CommitEdit();
                dt.AcceptChanges();
            }

            if (running == false)
            {
                StartClicked(sender, e);
                return;

            }
            if (running == true)
            {
                canceltime(sender, e);
                return;
            }



        }



        void buttonpick2(object sender, RoutedEventArgs e)
        {

            if (editing == false)
            {
                editmodeon(sender, e);
                return;
            }



            if (editing == true)
            {
                canceledit(sender, e);
                return;
            }


        }

        // GO
        async void StartClicked(object sender, RoutedEventArgs e)
        {
            var ind = Grid1.SelectedItem as DataRowView;


            if (ind is null)
            {
                try
                {
                    {
                        editmode = false;
                        running = true;
                        TaskName.IsEnabled = true;
                        TaskDescription.IsEnabled = true;

                        if (TaskName.Text == "")
                        {
                            TaskName.Text = "Task Name...";
                        }

                        if (TaskDescription.Text == "")
                        {
                            TaskDescription.Text = "Task Description...";
                        }
                        await Task.Run(() => updatetime());
                    }
                }

                catch (OperationCanceledException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

           
                //Btn2.IsEnabled = true;
                
        }

        // SET TIMER AND CREATE TIME VALUES

        async void updatetime()
        {



            if (running == true)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    var bgcolour = new SolidColorBrush(Color.FromArgb(255, 228, 74, 28));
                    Btn1.Opacity = 1;
                    Btn1.Content = "Stop";
                    Btn1.Background = bgcolour;
                    Btn3.IsEnabled = false;
                    Grid1.Visibility = Visibility.Visible;
                    CombinationCanvas.Visibility = Visibility.Collapsed;


                }));
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 100;
                timer.AutoReset = true;
                timer.Elapsed += applytime;
                timer.Enabled = true;

                timestarted = DateTime.Now;
                timestarted2 = DateTime.Now.AddSeconds(-holdingtime);

            }
            if (running == false)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    var bgcolour = new SolidColorBrush(Color.FromArgb(255, 60, 162, 60));
                    Btn1.Content = "Start Timer";
                    Btn1.Background = bgcolour;
                    if ((string)Lab1.Content != "00:00:00")
                    {
                        Btn2.IsEnabled = true;
                    }
                }));

                return;
            }
        }


        // APPLY TIME

        async void applytime(object sender, ElapsedEventArgs e)
        {
            if (running == true && timestarted2 != null)
            {
                DateTime timenow = (DateTime)timestarted2;
                timenow.AddSeconds(-holdingtime);
                DateTime timerecorded = e.SignalTime;
                waitingendtime = DateTime.Now;
                TimeSpan span = timerecorded - timenow;
                int roundedseconds = (int)span.TotalSeconds;
                TimeSpan timepassed = TimeSpan.FromSeconds(roundedseconds);
                holdingtime = ((int)timepassed.TotalSeconds);
                diff3 = timepassed.ToString(@"hh\:mm\:ss");
                Dispatcher.Invoke((Action)(() =>
            {
                Lab1.Content = timepassed.ToString(@"hh\:mm\:ss");
            }));
            }
        }

        //STOP
        void canceltime(object sender, RoutedEventArgs e)
        {

            Dispatcher.Invoke((Action)(() =>
            {
                var bgcolour = new SolidColorBrush(Color.FromArgb(255, 60, 162, 60));
                Btn1.Content = "Start Timer";
                Btn1.Background = bgcolour;
                Btn2.IsEnabled = false;

            }));
            running = false;
            return;

        }




        void committime(object sender, RoutedEventArgs e)
        {

            //if ((string)Lab1.Content == "00:00:00")
            //{
            //    return;
            //}

            if ((string)Lab1.Content != "00:00:00")
            {

                if (Btn4_Copy.IsEnabled == false)
                {
                    Btn4_Copy.IsEnabled = true;
                }

                Btn3.IsEnabled = true;
                var brush1 = new SolidColorBrush(Color.FromArgb(255, 212, 185, 2));
                Btn3.Content = "Edit Mode";
                Btn3.Background = brush1;


                editing = false;

                editmode = false;

                DataTable data1 = new DataTable();

                if (data1.Columns.Count == 0)
                {


                    data1.Columns.Add("shortName");
                    data1.Columns.Add("Spacer");
                    data1.Columns.Add("Time Elapsed");
                    data1.Columns.Add("Description");
                    data1.Columns.Add("Task");
                    data1.Columns.Add("Hide");
                    data1.Columns.Add("ShortDescription");

                    //data1.AcceptChanges();
                }


                if (dt is null)

                {
                    dt = new DataTable();
                    dt.Columns.Add("ShortName", typeof(string));
                    dt.Columns.Add("Spacer", typeof(string));
                    dt.Columns.Add("Time Elapsed", typeof(string));
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("Task", typeof(string));
                    dt.Columns.Add("Hide", typeof(string));
                    dt.Columns.Add("ShortDescription", typeof(string));

                }


                if (dt is not null)
                {


                    if (TaskName.Text == "Task Name...")
                    {
                        taskname = "[No Task Name]";
                    }

                    if (TaskName.Text != "Task Name...")
                    {
                        taskname = TaskName.Text;
                    }

                    if (TaskDescription.Text == "Task Description...")
                    {
                        taskdesc = "[No Task Description]";
                    }

                    if (TaskDescription.Text != "Task Description...")
                    {
                        taskdesc = TaskDescription.Text;

                    }

                    if (taskname.Length > 20)
                        shortened = (taskname.Substring(0, 20) + "...");

                    if (taskname.Length < 20)
                        shortened = taskname;

                    DataRow dr = data1.Rows.Add(shortened, "", diff3, taskdesc, taskname, taskdesc);

                    foreach (DataRow row in data1.Rows)

                    {
                        rownumber = rownumber + 1;
                        dt.Rows.Add(shortened, "", diff3, taskdesc, taskname, taskdesc);
                    }
                }




                Binding bs1 = new Binding();
                bs1.Source = dt;
                bs1.Mode = BindingMode.TwoWay;
                bs1.BindingGroupName = "Grp1";
                bs1.BindsDirectlyToSource = true;

                DataView dv1 = dt.DefaultView;

                dv1.AllowEdit = false;
                dv1.AllowDelete = false;
                dv1.AllowNew = false;
                dv1.RowFilter = "NOT Spacer = 'Hide'";

                dt = dv1.ToTable(false, "ShortName", "Spacer", "Time Elapsed", "Description", "Task", "ShortDescription");
                Grid1.ItemsSource = dt.AsDataView();
                Grid1.Items.Refresh();
                Grid1.IsReadOnly = true;
                Grid1.IsEnabled = true;
                Grid1.RowHeight = 20;

                SolidColorBrush col = new SolidColorBrush(Color.FromArgb(255, 212, 185, 2));
                SolidColorBrush col2 = new SolidColorBrush(Color.FromArgb(255, 94, 114, 125));

                var wdth = Grid1.ColumnWidth.IsSizeToCells;



                if (Grid1.SelectedItems.Count > 0)
                {
                    Grid1.IsReadOnly = true;
                    Grid1.IsEnabled = true;

                    Style st = new Style(typeof(DataGridCell));
                    st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
                    st.Setters.Add(new Setter(BorderBrushProperty, value: null));
                    st.Setters.Add(new Setter(BackgroundProperty, col));
                    st.Setters.Add(new Setter(ForegroundProperty, col2));

                    Style st2 = new Style(typeof(DataGridCell));
                    st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
                    st2.Setters.Add(new Setter(BorderBrushProperty, value: null));
                    st2.Setters.Add(new Setter(BackgroundProperty, col));
                    st2.Setters.Add(new Setter(ForegroundProperty, col2));


                    Style st3 = new Style(typeof(DataGridCell));
                    st3.Setters.Add(new Setter(BackgroundProperty, value: null));
                    st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

                    Grid1.Columns[0].Width = 140;
                    Grid1.Columns[0].CellStyle = st;
                    Grid1.Columns[1].Width = 10;
                    Grid1.Columns[1].CellStyle = st3;
                    Grid1.Columns[2].Width = 55;
                    Grid1.Columns[2].CellStyle = st2;
                    Grid1.Columns[3].MaxWidth = 0;
                    Grid1.Columns[4].MaxWidth = 0;
                    Grid1.Columns[5].MaxWidth = 0;


                }


                else if (Grid1.SelectedItems.Count < 1)
                {
                    Grid1.IsReadOnly = true;
                    Grid1.IsEnabled = true;



                    Style st = new Style(typeof(DataGridCell));
                    st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
                    st.Setters.Add(new Setter(BorderBrushProperty, value: null));
                    st.Setters.Add(new Setter(BackgroundProperty, col2));
                    st.Setters.Add(new Setter(ForegroundProperty, col));

                    Style st2 = new Style(typeof(DataGridCell));
                    st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
                    st2.Setters.Add(new Setter(BorderBrushProperty, value: null));
                    st2.Setters.Add(new Setter(BackgroundProperty, col2));
                    st2.Setters.Add(new Setter(ForegroundProperty, col));

                    Style st3 = new Style(typeof(DataGridCell));
                    st3.Setters.Add(new Setter(BackgroundProperty, value: null));
                    st3.Setters.Add(new Setter(BorderBrushProperty, value: null));



                    Grid1.Columns[0].Width = 140;
                    Grid1.Columns[0].CellStyle = st;
                    Grid1.Columns[1].Width = 10;
                    Grid1.Columns[1].CellStyle = st3;
                    Grid1.Columns[2].Width = 55;
                    Grid1.Columns[2].CellStyle = st2;
                    Grid1.Columns[3].MaxWidth = 0;
                    Grid1.Columns[4].MaxWidth = 0;
                    Grid1.Columns[5].MaxWidth = 0;



                }


                timestarted = null;
                timestarted2 = null;
                holdingtime = 0;
                holdingtimetaken = false;
                Lab1.Content = "00:00:00";

                TaskName.Text = "Task Name...";
                TaskDescription.Text = "Task Description...";

                var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

                TaskName.Foreground = brush;
                TaskDescription.Foreground = brush;

                taskname = "";
                taskdesc = "";

                Btn2.IsEnabled = false;

                if (Grid1.Items.Count > 0)
                {
                    Btn3.IsEnabled = true;


                }
                TaskName.IsEnabled = false;
                TaskDescription.IsEnabled = false;

                Lab1.Content = "00:00:00";

                textBox7.IsEnabled = true;
                textBox8.IsEnabled = true;
                Btn4_Copy.IsEnabled = true;
                Btn4_Copy1.IsEnabled = true;
                radioButton1.IsEnabled = true;
                radioButton2.IsEnabled = true;
                radioButton3.IsEnabled = true;
                radioButton4.IsEnabled = true;
                checkBox2.IsEnabled = true;
                checkBox2_Copy.IsEnabled = true;
                checkBox3.IsEnabled = true;



            }

        }

        void StartClicked2(object sender, RoutedEventArgs e)
        {

        }

        //public void BlankTaskNameText(object sender, MouseButtonEventArgs e)
        //{

        //    TaskName.Text = "Task Name...";
        //    TaskDescription.Text = "Task Description...";
        //    TaskName.GotFocus += RemoveText;
        //    TaskName.LostFocus += AddText;
        //    TaskDescription.GotFocus += RemoveDescText;
        //    TaskDescription.LostFocus += AddDescText;


        //}


        public void RemoveText(object sender, EventArgs e)
        {



            if (TaskName.Text == "Task Name..." || TaskName.Text == taskname)
            {
                TaskName.Text = "";
                TaskName.Foreground = Brushes.Black;

            }
        }

        public void AddText(object sender, EventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));


            if (string.IsNullOrWhiteSpace(TaskName.Text))
            {
                TaskName.Text = "Task Name...";
                TaskName.Foreground = brush;
            }
        }


        public void RemoveDescText(object sender, EventArgs e)
        {

            if (TaskDescription.Text == "Task Description..." || TaskDescription.Text == taskdesc)
            {
                TaskDescription.Text = "";
                TaskDescription.Foreground = Brushes.Black;
            }
        }

        public void AddDescText(object sender, EventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

            if (string.IsNullOrWhiteSpace(TaskDescription.Text))
            {
                TaskDescription.Text = "Task Description...";
                TaskDescription.Foreground = brush;
            }
        }


        void deselectgrid(object sender, EventArgs e)
        {

            Grid1.SelectedIndex = -1;
            Grid1.FontWeight = FontWeights.Regular;
            Grid1.FontStretch = FontStretches.Normal;
        

        }


        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (editing == false)
            //{

            //    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            //    {
            //        e.Handled = true;
            //        Btn4.IsEnabled = false;
            //    }
            //}
        }




        void canceledit(object sender, EventArgs e)
        {
            

            Dispatcher.Invoke((Action)(() =>
            {
                var brush = new SolidColorBrush(Color.FromArgb(255, 212, 185, 2));
                Btn3.Content = "Edit Mode";
                Btn3.Background = brush;

            }));


            Btn4.IsEnabled = false;
            Btn4.FontSize = 12;
            Btn4.Content = "Combine Tasks";

            Btn4_Copy1.IsEnabled = true;

            Grid1.IsReadOnly = true;

            var wdth = Grid1.ColumnWidth.IsSizeToCells;

            SolidColorBrush col = new SolidColorBrush(Color.FromArgb(255, 212, 185, 2));
            SolidColorBrush col2 = new SolidColorBrush(Color.FromArgb(255, 94, 114, 125));

            Style st = new Style(typeof(DataGridCell));
            st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            st.Setters.Add(new Setter(BorderBrushProperty, value: null));
            st.Setters.Add(new Setter(BackgroundProperty, col2));
            st.Setters.Add(new Setter(ForegroundProperty, col));

            Style st2 = new Style(typeof(DataGridCell));
            st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            st2.Setters.Add(new Setter(BorderBrushProperty, value: null));
            st2.Setters.Add(new Setter(BackgroundProperty, col2));
            st2.Setters.Add(new Setter(ForegroundProperty, col));

            Style st3 = new Style(typeof(DataGridCell));
            st3.Setters.Add(new Setter(BackgroundProperty, value: null));
            st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

            Grid1.Columns[0].Width = 140;
            Grid1.Columns[0].CellStyle = st;
            Grid1.Columns[1].Width = 10;
            Grid1.Columns[1].CellStyle = st3;
            Grid1.Columns[2].Width = 55;
            Grid1.Columns[2].CellStyle = st2;
            Grid1.Columns[3].MaxWidth = 0;
            Grid1.Columns[4].MaxWidth = 0;
            Grid1.Columns[5].MaxWidth = 0;

            editing = false;
            editmode = false;

            Grid1.Visibility = Visibility.Visible;
            CombinationCanvas.Visibility = Visibility.Collapsed;

            combining = false;

        }





        private void deselectall(object sender, RoutedEventArgs e)
        {

            SolidColorBrush col = new SolidColorBrush(Color.FromArgb(255, 212, 185, 2));
            SolidColorBrush col2 = new SolidColorBrush(Color.FromArgb(255, 94, 114, 125));





            if (Grid1.SelectedItems.Count > 0 && editmode == false)
            {
                Grid1.IsReadOnly = true;

                var wdth = Grid1.ColumnWidth.IsSizeToCells;

                Style st = new Style(typeof(DataGridCell));
                st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
                st.Setters.Add(new Setter(BorderBrushProperty, value: null));
                st.Setters.Add(new Setter(BackgroundProperty, col2));
                st.Setters.Add(new Setter(ForegroundProperty, col));

                Style st2 = new Style(typeof(DataGridCell));
                st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
                st2.Setters.Add(new Setter(BorderBrushProperty, value: null));
                st2.Setters.Add(new Setter(BackgroundProperty, col2));
                st2.Setters.Add(new Setter(ForegroundProperty, col));


                Style st3 = new Style(typeof(DataGridCell));
                st3.Setters.Add(new Setter(BackgroundProperty, value: null));
                st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

                Grid1.Columns[0].Width = 140;
                Grid1.Columns[0].CellStyle = st;
                Grid1.Columns[1].Width = 10;
                Grid1.Columns[1].CellStyle = st3;
                Grid1.Columns[2].Width = 55;
                Grid1.Columns[2].CellStyle = st2;
                Grid1.Columns[3].MaxWidth = 0;
                Grid1.Columns[4].MaxWidth = 0;
                Grid1.Columns[5].MaxWidth = 0;
            }


            else if (Grid1.SelectedItems.Count < 1 && editmode == false)
            {
                Grid1.IsReadOnly = true;

                var wdth = Grid1.ColumnWidth.IsSizeToCells;

                Style st = new Style(typeof(DataGridCell));
                st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
                st.Setters.Add(new Setter(BorderBrushProperty, value: null));
                st.Setters.Add(new Setter(BackgroundProperty, col2));
                st.Setters.Add(new Setter(ForegroundProperty, col));

                Style st2 = new Style(typeof(DataGridCell));
                st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
                st2.Setters.Add(new Setter(BorderBrushProperty, value: null));
                st2.Setters.Add(new Setter(BackgroundProperty, col2));
                st2.Setters.Add(new Setter(ForegroundProperty, col));


                Style st3 = new Style(typeof(DataGridCell));
                st3.Setters.Add(new Setter(BackgroundProperty, value: null));
                st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

                Grid1.Columns[0].Width = 140;
                Grid1.Columns[0].CellStyle = st;
                Grid1.Columns[1].Width = 10;
                Grid1.Columns[1].CellStyle = st3;
                Grid1.Columns[2].Width = 55;
                Grid1.Columns[2].CellStyle = st2;
                Grid1.Columns[3].MaxWidth = 0;
                Grid1.Columns[4].MaxWidth = 0;
                Grid1.Columns[5].MaxWidth = 0;
            }





        }

        public void editmodeon(object sender, EventArgs e)


        {
            Binding bs1 = new Binding();
            bs1.Source = null;

            bs1.Source = dt;

            if (Grid1.Items.Count == 0)
            {
                return;
            }

            Btn4.IsEnabled = true;
            Btn4_Copy1.IsEnabled = false;

            Grid1.SelectedIndex = 0;

            Grid1.IsReadOnly = false;
            Grid1.CanUserDeleteRows = true;
            editmode = true;

            editing = true;


            Dispatcher.Invoke((Action)(() =>
            {
                var bgcolour = new SolidColorBrush(Color.FromArgb(255, 228, 74, 28));
                Btn3.Content = "Stop Editing";
                Btn3.Background = bgcolour;

            }));


            Grid1.BeginEdit();

            Style st = new Style(typeof(DataGridCell));
            st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            st.Setters.Add(new Setter(BorderBrushProperty, value: null));


            Style st2 = new Style(typeof(DataGridCell));
            st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            st2.Setters.Add(new Setter(BorderBrushProperty, value: null));


            Style st3 = new Style(typeof(DataGridCell));
            st3.Setters.Add(new Setter(BackgroundProperty, value: null));
            st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

            if (Grid1.Items.Count > 0)
            {
                Grid1.Columns[0].Width = 140;
                Grid1.Columns[0].CellStyle = st;
                Grid1.Columns[1].Width = 10;
                Grid1.Columns[1].CellStyle = st3;
                Grid1.Columns[2].Width = 55;
                Grid1.Columns[2].CellStyle = st2;
                Grid1.Columns[3].MaxWidth = 0;
            }


            UpdateLayout();



        }




        //private void changedcell(object sender, EventArgs e)
        //{
        //    dt.AcceptChanges();
        //    string val = (Grid1.Columns[0].GetCellContent(Grid1.Items[0]) as TextBlock).Text;
        //    TaskName.Text = val;
        //}



        private void SelectedRow(object sender, SelectionChangedEventArgs e)
        {

            var ind = Grid1.SelectedItem as DataRowView;
            string txt = "";
            string txt2 = "";
            string time = "";


            if (ind != null)
            {
                txt = ind.Row["ShortName"] as string;
                txt = txt.Trim('\n');

                if (ind.Row["ShortDescription"].ToString().Length > 0)
                {
                    txt2 = ind.Row["ShortDescription"] as string;
                    txt2 = txt2.TrimEnd('\u002C');
                }

                else
                {
                    txt2 = ind.Row["Description"] as string;
                    txt2 = txt2.TrimEnd('\u002C');
                }



                //ind.Row["ShortDescription"] as string;
                TaskName.Text = txt;
                TaskDescription.Text = txt2;

            }

        }

        //private void SelectedRow(object sender, RoutedEventArgs e)
        //{


        //    var ind = Grid1.SelectedItem as DataRowView;
        //    string txt = "";
        //    string txt2 = "";

        //    if (ind != null)
        //    {

        //        txt = ind.Row["Task"] as string;
        //        txt2 = ind.Row["ShortDescription"] as string;
        //        TaskName.Text = txt;
        //        TaskDescription.Text = txt2;

        //    }



        //}

        //private void SelectedRow(object sender, SelectedCellsChangedEventArgs e)
        //{

        //    var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

        //    var ind = Grid1.SelectedItem as DataRowView;
        //    string txt = "";
        //    string txt2 = "";


        //    if (ind != null)
        //    {

        //        txt = ind.Row["ShortName"] as string;

        //        if (ind.Row["ShortDescription"].ToString().Length > 0)
        //        {
        //            txt2 = ind.Row["ShortDescription"] as string;
        //        }

        //        else
        //        {
        //            txt2 = ind.Row["Description"] as string;
        //        }
        //        TaskName.Text = txt;
        //        TaskDescription.Text = txt2;
        //        taskname = txt;
        //        taskdesc = txt2;

        //        TaskName.Foreground = brush;
        //        TaskDescription.Foreground = brush;

        //    }



        //    if (taskname != txt || taskdesc != txt2)
        //    {
        //        TaskName.Foreground = brush;
        //        TaskDescription.Foreground = brush;

        //    }


        //}



        //private void validaterowselected()
        //{
        //    if (Grid1.SelectedIndex == -1 && Grid1.Visibility == Visibility.Visible)
        //    {
        //        MessageBox.Show("Please select a row");
        //        return;
        //    }

        //}




        public void buttonvalidate()
        {
            Task runbutton = new Task(changebutton);
            runbutton.Start();

        }


        public void changebutton()
        {

            Dispatcher.Invoke((Action)(() =>
            {
                var bgcolour = new SolidColorBrush(Color.FromArgb(255, 228, 74, 28));
                Btn4.Opacity = 1;
                Btn4.Background = bgcolour;
                Btn4.Content = "Please combine" + Environment.NewLine + "    valid times.";
                
            }));


            Thread.Sleep(200);

            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = false;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = true;
            }));
            Thread.Sleep(75);

            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = false;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = true;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = false;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = true;
            }));
            Thread.Sleep(75);

            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = false;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = true;
            }));
            Thread.Sleep(75);

            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = false;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = true;
            }));
            Thread.Sleep(75);

            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = false;
            }));
            Thread.Sleep(75);
            Dispatcher.Invoke((Action)(() =>
            {
                Btn4.IsEnabled = true;
            }));
            Thread.Sleep(100);
            Dispatcher.Invoke((Action)(() =>
            {
                var bgcolour = new SolidColorBrush(Color.FromArgb(255, 60, 162, 60));
                Btn4.IsEnabled = true;
                Btn4.Background = bgcolour;
                Btn4.FontSize = 12;
                Btn4.Content = "Combine Tasks";
                Btn3.IsEnabled = true;
            }));
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {

            if (combining == true)
            {
                combinetasks(sender, e);
 
            }

            else
            {
                if (Grid1.SelectedItems.Count > 1)
                {

                    editmode = false;
                    editing = false;
                    Grid1.IsEnabled = false;
                    Grid1.IsEnabled = true;

                    Btn3.IsEnabled = false;


                    int totaltime = 0;
                    string totalname = "";
                    string totaldesc = "";
                    string totalsplit = "";
                    string rownsame2 = "";
                    string replacement = "";

                    Grid1.CurrentItem = Grid1.SelectedItem;

                    foreach (DataRowView dr in Grid1.Items)
                    {

                        string rowsadd = dr["Time Elapsed"].ToString();
                        string rowsdesc = dr["Description"].ToString();
                        string rownsame = dr["Task"].ToString() + ";";
                        

                        Btn4.IsEnabled = true;
                        Btn4.FontSize = 12;
                        Btn4.Content = "Combine Tasks";

                        try
                        {

                            DateTime.ParseExact(rowsadd,"hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        }
                        catch (Exception ex)
                        {

                            buttonvalidate();
                            return;

                        }


                        TimeSpan time = TimeSpan.Parse(rowsadd);
                        int totalseconds = (int)time.TotalSeconds;
                        totaltime = totaltime + totalseconds;
                        totalname = rownsame.Replace("Amalgamated Task", "-");
                        rownsame2 = totalname.TrimStart(',');
                        totaldesc = rowsdesc + "-";
                        totalsplit = totalsplit + (rownsame2 + Environment.NewLine + totaldesc + Environment.NewLine + Environment.NewLine);
                        allnames += rownsame2.Replace("\n", "-");
                        allnames = allnames.Replace(Environment.NewLine, "-");
                        allnames = (CombinationTaskDesc.Text ?? allnames);

                        dr.BeginEdit();
                        dr["Spacer"] = "Hide";
                        dr.EndEdit();

                    }

                    TimeSpan elapsed = TimeSpan.FromSeconds(totaltime);
                    string timed = elapsed.ToString(@"hh\:mm\:ss");


                    timestoaddup = timed;
                    detailstocombine = rownsame2 + Environment.NewLine;
                    initialdesc = totaldesc.TrimEnd('-');
                    alldetails = totalsplit + "Total Combined Time: " + timed;

                    rownsame2 = rownsame2.Replace(",,", ",");


                    //dt2 = new DataTable();
                    //dt2.Columns.Add("ShortName", typeof(string));
                    //dt2.Columns.Add("Spacer", typeof(string));
                    //dt2.Columns.Add("Time Elapsed", typeof(string));
                    //dt2.Columns.Add("Description", typeof(string));
                    //dt2.Columns.Add("Task", typeof(string));
                    //dt2.Columns.Add("Hide", typeof(string));
                    //dt2.Columns.Add("ShortDescription", typeof(string));


                    //dt2 = dt.Copy();
                    //dt2.Rows.Add("Amalgamated Task", "", timed, totalsplit, rownsame2.TrimEnd(','), allnames);

                    AmalgamatedTask = "Amalgamated Task";
                    Spacer = "";
                    timedtemp = timed;
                    totalsplittemp = totalsplit.TrimEnd('-');
                    rownsame2temp = rownsame2.TrimEnd(',');
                    rownsame2temp = rownsame2temp.TrimEnd('-');
                    allnamestemp = allnames.TrimEnd('-');

                    combining = true;


                    DataView dv1 = dt.DefaultView;

                    dv1.AllowEdit = false;
                    dv1.AllowDelete = false;
                    dv1.AllowNew = false;
                    dv1.RowFilter = "NOT Spacer = 'Hide'";

                    //Grid1.ItemsSource = dt.AsDataView();
                    //Grid1.Items.Refresh();
                    //Grid1.IsReadOnly = true;
                    //Grid1.IsEnabled = true;
                    //Grid1.RowHeight = 20;

                    Style st = new Style(typeof(DataGridCell));
                    st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
                    st.Setters.Add(new Setter(BorderBrushProperty, value: null));

                    Style st2 = new Style(typeof(DataGridCell));
                    st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
                    st2.Setters.Add(new Setter(BorderBrushProperty, value: null));


                    Style st3 = new Style(typeof(DataGridCell));
                    st3.Setters.Add(new Setter(BackgroundProperty, value: null));
                    st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

                    Grid1.Columns[0].Width = 140;
                    Grid1.Columns[0].CellStyle = st;
                    Grid1.Columns[1].Width = 10;
                    Grid1.Columns[1].CellStyle = st3;
                    Grid1.Columns[2].Width = 55;
                    Grid1.Columns[2].CellStyle = st2;
                    Grid1.Columns[3].MaxWidth = 0;
                    Grid1.Columns[4].MaxWidth = 0;
                    Grid1.Columns[5].MaxWidth = 0;


                    CombinationCanvas.Visibility = Visibility.Visible;
                    Grid1.Visibility = Visibility.Collapsed;
                    Btn4.Content = "Confirm";

                    Btn1.IsEnabled = false;
                    Btn2.IsEnabled = false;


                }

            }
        }



      


            public void RemoveText2(object sender, EventArgs e)
        {



            if (CombinationTaskName.Text == "Task Name..." || CombinationTaskName.Text == taskname)
            {
                CombinationTaskName.Text = "";
                CombinationTaskName.Foreground = Brushes.Black;

            }
        }

        public void AddText2(object sender, EventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));


            if (string.IsNullOrWhiteSpace(CombinationTaskName.Text))
            {
                CombinationTaskName.Text = "Task Name...";
                CombinationTaskName.Foreground = brush;
            }
        }


        public void RemoveDescText2(object sender, EventArgs e)
        {

            if (CombinationTaskDesc.Text == "Task Description..." || CombinationTaskDesc.Text == taskdesc)
            {
                CombinationTaskDesc.Text = "";
                CombinationTaskDesc.Foreground = Brushes.Black;
            }
        }

        public void AddDescText2(object sender, EventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

            if (string.IsNullOrWhiteSpace(CombinationTaskDesc.Text))
            {
                CombinationTaskDesc.Text = "Task Description...";
                CombinationTaskDesc.Foreground = brush;
            }
        }



        public void combinetasks(object sender, EventArgs e)
        {



            if (CombinationTaskName.Text != "Task Name...")
            {
                AmalgamatedTask = CombinationTaskName.Text;
            }
            if (CombinationTaskName.Text == "Task Name...")
            {
                AmalgamatedTask = "Amalgamated Task";
            }
            if (CombinationTaskDesc.Text != "Task Description...")
            {
                allnamestemp = CombinationTaskDesc.Text;
            }
            if (CombinationTaskDesc.Text == "Task Description...")
            {
                allnamestemp = totalsplittemp;
            }


            dt.Rows.Add(AmalgamatedTask, "", timedtemp, allnamestemp, AmalgamatedTask, allnamestemp);


            AmalgamatedTask = "";
            //Spacer = "";
            timedtemp = "";
            totalsplittemp = "";
            rownsame2temp = "";
            allnamestemp = "";

            //dt.Rows.Add();
            //dt.AcceptChanges();

            int totaltime = 0;
            string textvalue = CombinationTaskName.Text;
            string newdescription = CombinationTaskDesc.Text;
            string combineddescription = "Event:" + Environment.NewLine + CombinationTaskName.Text + Environment.NewLine + Environment.NewLine + "Description: " + Environment.NewLine + CombinationTaskDesc.Text;

            TaskName.Text = "Task Name...";
            TaskDescription.Text = "Task Description...";
            Grid1.SelectedIndex = -1;

            Binding bs1 = new Binding();
            bs1.Source = null;

            bs1.Source = dt;
            bs1.Mode = BindingMode.TwoWay;
            bs1.BindingGroupName = "Grp1";
            bs1.BindsDirectlyToSource = true;

            DataView dv1 = dt.DefaultView;

            dv1.AllowEdit = false;
            dv1.AllowDelete = false;
            dv1.AllowNew = false;
            dv1.RowFilter = "NOT Spacer = 'Hide'";


            dt = dv1.ToTable(false, "ShortName", "Spacer", "Time Elapsed", "Description", "Task", "ShortDescription");
            Grid1.ItemsSource = null;
            Grid1.ItemsSource = dt.AsDataView();
            Grid1.Items.Refresh();
            Grid1.IsReadOnly = true;
            Grid1.IsEnabled = true;
            Grid1.RowHeight = 20;

            Style st = new Style(typeof(DataGridCell));
            st.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            st.Setters.Add(new Setter(BorderBrushProperty, value: null));

            Style st2 = new Style(typeof(DataGridCell));
            st2.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            st2.Setters.Add(new Setter(BorderBrushProperty, value: null));


            Style st3 = new Style(typeof(DataGridCell));
            st3.Setters.Add(new Setter(BackgroundProperty, value: null));
            st3.Setters.Add(new Setter(BorderBrushProperty, value: null));

            Grid1.Columns[0].Width = 140;
            Grid1.Columns[0].CellStyle = st;
            Grid1.Columns[1].Width = 10;
            Grid1.Columns[1].CellStyle = st3;
            Grid1.Columns[2].Width = 55;
            Grid1.Columns[2].CellStyle = st2;
            Grid1.Columns[3].MaxWidth = 0;
            Grid1.Columns[4].MaxWidth = 0;
            Grid1.Columns[5].MaxWidth = 0;

            TaskName.Text = "Task Name...";
            TaskDescription.Text = "Task Description...";
            Grid1.SelectedIndex = -1;

            CombinationCanvas.Visibility = Visibility.Collapsed;
            Grid1.Visibility = Visibility.Visible;
            Btn4.Content = "Combine Tasks";
            Btn4.IsEnabled = false;
            //combining = false;

            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

            CombinationTaskDesc.Text = "Task Description...";
            CombinationTaskDesc.Foreground = brush;
            CombinationTaskName.Text = "Task Name...";
            CombinationTaskName.Foreground = brush;

            Btn1.IsEnabled = true;
            Btn2.IsEnabled = false;
            Btn4_Copy1.IsEnabled = true;

        }


        private void RestrictToNumbers(object sender, KeyEventArgs e)
        {



            var col2 = Grid1.Columns[2];


            if (Grid1.CurrentColumn == col2)
            {


                string[] nums = {
                            "NumPad1",
                            "NumPad2",
                            "NumPad3",
                            "NumPad4",
                            "NumPad5",
                            "NumPad6",
                            "NumPad7",
                            "NumPad8",
                            "NumPad9",
                            "NumPad0",
                            "D1",
                            "D2",
                            "D3",
                            "D4",
                            "D5",
                            "D6",
                            "D7",
                            "D8",
                            "D9",
                            "D0",
                            "RightShift",
                            "LeftShift",
                            "Oem1",
                            "Back",
                            "Delete",
                            "Return",
                            "Enter",
                            "Left",
                            "Right"
                             };


                var ind = Grid1.SelectedItem as DataRowView;

                if (Grid1.SelectedIndex > -1)

                {
                    string txt = ind.Row["Time Elapsed"] as string;

                    if (!nums.Contains(e.Key.ToString()))
                    {
                        e.Handled = true;


                        Binding bs1 = new Binding();

                        bs1.Source = null;

                        bs1.Source = dt;
                    };

                    try
                    {
                        if (editing == true)
                        {

                            DateTime.ParseExact(txt, "hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            Btn4.IsEnabled = true;
                            //errorshown = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //

                    }
                }


            }



        }



        private void LeaveCell(object sender, DataGridCellEditEndingEventArgs e)
        {

            //{

            //    var ind = Grid1.SelectedItem as DataRowView;

            //    if (ind == null)
            //    {
            //        return;
            //    }

            //    if (ind != null)
            //    {

            //        Grid1.Focus();
            //        string txt = ind.Row["Time Elapsed"] as string;

            //        string error = "Please enter a valid time." + Environment.NewLine + "This must be in a format of hh:mm:ss.";

            //        heldtimevalue = txt;

            //        try
            //        {
            //            Grid1.Focus();
            //            DateTime.ParseExact(txt, "hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            //            //errorshown = false;
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(error,"Time is invalid",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            //            //errorshown = true;
            //            Grid1.Visibility = Visibility.Visible;
            //            CombinationCanvas.Visibility = Visibility.Collapsed;
            //            return;

            //        }
            //    }

            //}
        }

        private void VaildateEnter(object sender, TextCompositionEventArgs e)
        {


            var ind = Grid1.SelectedItem as DataRowView;


            string txt = ind.Row["Time Elapsed"] as string;




        }



        private void RetrictToNumbersTyped(object sender, DataTransferEventArgs e)
        {


            Binding bs1 = new Binding();
            bs1.Source = dt;
            bs1.Mode = BindingMode.TwoWay;
            bs1.BindingGroupName = "Grp1";
            bs1.BindsDirectlyToSource = true;

            DataView dv1 = dt.DefaultView;

            dv1.AllowEdit = false;
            dv1.AllowDelete = false;
            dv1.AllowNew = false;
            dv1.RowFilter = "NOT Spacer = 'Hide'";

            dt = dv1.ToTable(false, "ShortName", "Spacer", "Time Elapsed", "Description", "Task", "ShortDescription");
            Grid1.ItemsSource = dt.AsDataView();
            Grid1.Items.Refresh();
            Grid1.IsReadOnly = true;
            Grid1.IsEnabled = true;
            Grid1.RowHeight = 20;

        }

        private void ClearAll(object sender, RoutedEventArgs e)
        {
            if (dt != null)
            {
                dt.Clear();


                Binding bs1 = new Binding();
                bs1.Source = dt;
                bs1.Mode = BindingMode.TwoWay;
                bs1.BindingGroupName = "Grp1";
                bs1.BindsDirectlyToSource = true;

                DataView dv1 = dt.DefaultView;

                dv1.AllowEdit = false;
                dv1.AllowDelete = false;
                dv1.AllowNew = false;
                dv1.RowFilter = "NOT Spacer = 'Hide'";

                dt = dv1.ToTable(false, "ShortName", "Spacer", "Time Elapsed", "Description", "Task", "ShortDescription");
                Grid1.ItemsSource = dt.AsDataView();
                Grid1.Items.Refresh();
                Grid1.IsReadOnly = true;
                Grid1.IsEnabled = true;
                Grid1.RowHeight = 20;

                Lab1.Content = "00:00:00";
                Btn2.IsEnabled = false;
                running = false;


                var bgcolour = new SolidColorBrush(Color.FromArgb(255, 60, 162, 60));
                Btn1.Content = "Start Timer";
                Btn1.Background = bgcolour;
                holdingtime = 0;

                Btn4_Copy.IsEnabled = false;    

                textBox7.IsEnabled = false;
                textBox8.IsEnabled = false;
                Btn4_Copy.IsEnabled = false;
                Btn4_Copy1.IsEnabled = false;
                radioButton1.IsEnabled = false;
                radioButton2.IsEnabled = false;
                radioButton3.IsEnabled = false;
                radioButton4.IsEnabled = false;
                checkBox2.IsEnabled = false;
                checkBox2_Copy.IsEnabled = false;
                checkBox3.IsEnabled = false;

                var brush = new SolidColorBrush(Color.FromArgb(255, 212, 185, 2));
                Btn3.Content = "Edit Mode";
                Btn3.Background = brush;

                Btn3.IsEnabled = false;
                Btn4.IsEnabled = false;

                Btn1.IsEnabled = true;

                Grid1.Visibility = Visibility.Visible;
                CombinationCanvas.Visibility = Visibility.Collapsed;

                var brush2 = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

                textBox7.Text = "Filename...";
                textBox7.Foreground = brush2;
                textBox8.Text = "Filepath...";
                textBox8.Foreground = brush2;

            }

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {



        }



        public void RemoveFilename(object sender, EventArgs e)
        {



            if (textBox7.Text == "Filename...")
            {
                textBox7.Text = "";
                textBox7.Foreground = Brushes.Black;

            }
        }

        public void AddFilename(object sender, EventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));


            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                textBox7.Text = "Filename...";
                textBox7.Foreground = brush;
            }
        }


        public void RemoveFilepath(object sender, EventArgs e)
        {

            if (textBox8.Text == "Filepath...")
            {
                textBox8.Text = "";
                textBox8.Foreground = Brushes.Black;
            }
        }

        public void AddFilePath(object sender, EventArgs e)
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 146, 144, 130));

            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                textBox8.Text = "Filepath...";
                textBox8.Foreground = brush;
            }
        }





        public void buildpreview(object sender, RoutedEventArgs e)
        {


            if (textBox7.Text == "")  
            {
                MessageBox.Show("Please enter a Filename.","Problem",MessageBoxButton.OK,MessageBoxImage.Stop);
            }

            


            if (textBox7.Text == "Filename...")
            {
                MessageBox.Show("Please enter a Filename.", "Problem", MessageBoxButton.OK, MessageBoxImage.Stop);
            }



            delimiter = "";
            headers = "";

            StringBuilder sb = new StringBuilder();


            if (radioButton1.IsChecked == true)
            {
                delimiter = "|";
            }
            if (radioButton2.IsChecked == true)
            {
                delimiter = ",";
            }
            if (radioButton3.IsChecked == true)
            {
                filetype = ".csv";
            }
            if (radioButton4.IsChecked == true)
            {
                filetype = ".txt";
            }
            if (checkBox2.IsChecked == true)
            {
                dateappend = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            if (checkBox2.IsChecked == false)
            {
                dateappend = "";
            }

            if (dt != null)
            {
              
                    sb.Append(header1);
                    sb.Append(delimiter);
                    sb.Append(header2);
                    sb.Append(delimiter);
                    sb.Append(header3);
                    sb.Append(Environment.NewLine);

                    headers = sb.ToString();

                    sb.Clear();


                if (checkBox2_Copy.IsChecked == true)
                {
                    sb.Append(headers);
                }

                

                DataView dv1 = new DataView(dt);
                dv1.RowFilter = "NOT Spacer = 'Hide'";

                //dt = dv1.ToTable();

                int i = 1;

                

                    if (checkBox3.IsChecked == true)
                    {
                    foreach (DataRow row in dt.Rows)

                    {
                            string olddesc = row["Task"].ToString();
                            string formatteddetails = "Task" + i + " Name: " + Regex.Replace(olddesc, @"\r\n?|\n", "");
                            string formatteddesc = "Task " + i + " Details: " + Regex.Replace(row["Description"].ToString(), @"\r\n?|\n", "");
                            string formattedtime = "Task " + i + " Time Elapsed: " + row["Time Elapsed"].ToString();
                            sb.Append(formatteddetails);
                            sb.Append(delimiter);
                            sb.Append(formatteddesc.TrimEnd('-'));
                            sb.Append(delimiter);
                            sb.Append(formattedtime);
                            sb.Append(Environment.NewLine);

                            i++;


                        }
                    }



                else if (checkBox2_Copy.IsChecked == true)
                {
                    foreach (DataRow row in dt.Rows)

                    {
                        string formatteddesc = Regex.Replace(row["Description"].ToString(), @"\r\n?|\n", "");
                        string olddesc = row["Task"].ToString();
                        sb.Append(olddesc);
                        sb.Append(delimiter);
                        sb.Append(formatteddesc.TrimEnd('-'));
                        sb.Append(delimiter);
                        sb.Append(row["Time Elapsed"].ToString());
                        sb.Append(Environment.NewLine);

                        i++;


                    }
                }


                else if (checkBox2_Copy.IsChecked == false && checkBox3.IsChecked == false)
                {
                    foreach (DataRow row in dt.Rows)

                    {
                        string formatteddesc = Regex.Replace(row["Description"].ToString(), @"\r\n?|\n", "");
                        string olddesc = row["Task"].ToString();
                        sb.Append(olddesc);
                        sb.Append(delimiter);
                        sb.Append(formatteddesc.TrimEnd('-'));
                        sb.Append(delimiter);
                        sb.Append(row["Time Elapsed"].ToString());
                        sb.Append(Environment.NewLine);

                        i++;


                    }
                }


                stringforfile = sb.ToString();

                createFile(sender, e);

                sb.Clear();

                
            

            }
          
        }


        private void createFile(object sender, EventArgs e)
        {


            if (!(textBox7.Text == null | textBox7.Text == "Filename..." ) | (textBox8.Text == null | textBox8.Text == "Filepath..."))
            {


                    try
                    {

                    string filename = textBox7.Text + dateappend;
                    string newpath = System.IO.Path.Combine(textBox8.Text.ToString(), (filename + filetype));
                    var tw = new StreamWriter((newpath), false);
                    using (tw)

                    {
                        tw.Flush();
                        tw.WriteLine(stringforfile);
                        tw.Flush();
                        filenumber = filenumber + 1;
                    }


                }

                catch (Exception ex)
                {
                   
                   var cause = ex.Message;
                  
                   MessageBox.Show(cause.ToString()+Environment.NewLine+Environment.NewLine + "Please ensure that the Filename and Filepath are correct.", "Problem", MessageBoxButton.OK, MessageBoxImage.Stop);
                  

                }

                

            }

           

        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}








