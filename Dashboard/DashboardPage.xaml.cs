using Microcharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json; // For serialization

namespace OnboardingSystem;

public partial class DashboardPage : ContentPage
{
    private readonly Dictionary<string, List<Dictionary<string, object>>> staticDataTables;

    public DashboardPage()
    {
        InitializeComponent();
        staticDataTables = InitializeStaticData();
        LoadTables();
        LoadConfig();
    }

    private Dictionary<string, List<Dictionary<string, object>>> InitializeStaticData()
    {
        Random random = new Random();
        DateTime startDate = new DateTime(2023, 8, 1);
        DateTime endDate = new DateTime(2024, 8, 31);

        return new Dictionary<string, List<Dictionary<string, object>>>
        {
            {
                "Staff", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "Name", "John Doe" }, { "Role", "Manager" }, { "PhoneNumber", "0412345678" }, { "Branch", "Dandenong" } },
                    new Dictionary<string, object> { { "Name", "Jane Smith" }, { "Role", "Sales Associate" }, { "PhoneNumber", "0412345679" }, { "Branch", "Springvale" } },
                    new Dictionary<string, object> { { "Name", "Sam Brown" }, { "Role", "Sales Associate" }, { "PhoneNumber", "0412345680" }, { "Branch", "Frankston" } },
                    new Dictionary<string, object> { { "Name", "Emily White" }, { "Role", "Cashier" }, { "PhoneNumber", "0412345681" }, { "Branch", "Melbourne" } },
                    new Dictionary<string, object> { { "Name", "Michael Green" }, { "Role", "Sales Associate" }, { "PhoneNumber", "0412345682" }, { "Branch", "Geelong" } },
                    new Dictionary<string, object> { { "Name", "Sarah Blue" }, { "Role", "Manager" }, { "PhoneNumber", "0412345683" }, { "Branch", "Ballarat" } },
                    new Dictionary<string, object> { { "Name", "Chris Black" }, { "Role", "Stock Clerk" }, { "PhoneNumber", "0412345684" }, { "Branch", "Bendigo" } },
                    new Dictionary<string, object> { { "Name", "Anna Grey" }, { "Role", "Sales Associate" }, { "PhoneNumber", "0412345685" }, { "Branch", "Warrnambool" } },
                    new Dictionary<string, object> { { "Name", "Tom Yellow" }, { "Role", "Sales Associate" }, { "PhoneNumber", "0412345686" }, { "Branch", "Horsham" } },
                    new Dictionary<string, object> { { "Name", "Lucy Pink" }, { "Role", "Cashier" }, { "PhoneNumber", "0412345687" }, { "Branch", "Mildura" } },
                }
            },
            {
                "Products", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "ProductName", "Running Shoes" }, { "Details", "Lightweight running shoes" }, { "Gender", "Unisex" }, { "Price", 99.99 }, { "Stock", 50 } },
                    new Dictionary<string, object> { { "ProductName", "Soccer Ball" }, { "Details", "Official size 5" }, { "Gender", "Unisex" }, { "Price", 29.99 }, { "Stock", 100 } },
                    new Dictionary<string, object> { { "ProductName", "Basketball Jersey" }, { "Details", "Breathable fabric" }, { "Gender", "Male" }, { "Price", 59.99 }, { "Stock", 75 } },
                    new Dictionary<string, object> { { "ProductName", "Tennis Racket" }, { "Details", "Graphite composite" }, { "Gender", "Unisex" }, { "Price", 149.99 }, { "Stock", 30 } },
                    new Dictionary<string, object> { { "ProductName", "Yoga Pants" }, { "Details", "Comfortable stretch" }, { "Gender", "Female" }, { "Price", 39.99 }, { "Stock", 60 } },
                    new Dictionary<string, object> { { "ProductName", "Swimming Goggles" }, { "Details", "Anti-fog lenses" }, { "Gender", "Unisex" }, { "Price", 19.99 }, { "Stock", 200 } },
                    new Dictionary<string, object> { { "ProductName", "Badminton Racket" }, { "Details", "Lightweight design" }, { "Gender", "Unisex" }, { "Price", 89.99 }, { "Stock", 40 } },
                    new Dictionary<string, object> { { "ProductName", "Cycling Helmet" }, { "Details", "Ventilated shell" }, { "Gender", "Unisex" }, { "Price", 79.99 }, { "Stock", 25 } },
                    new Dictionary<string, object> { { "ProductName", "Football Cleats" }, { "Details", "Firm ground" }, { "Gender", "Male" }, { "Price", 109.99 }, { "Stock", 45 } },
                    new Dictionary<string, object> { { "ProductName", "Sports Bra" }, { "Details", "High impact support" }, { "Gender", "Female" }, { "Price", 49.99 }, { "Stock", 80 } },
                    new Dictionary<string, object> { { "ProductName", "Baseball Cap" }, { "Details", "Adjustable fit" }, { "Gender", "Unisex" }, { "Price", 15.99 }, { "Stock", 150 } },
                    new Dictionary<string, object> { { "ProductName", "Gym Bag" }, { "Details", "Water-resistant" }, { "Gender", "Unisex" }, { "Price", 35.99 }, { "Stock", 70 } },
                    new Dictionary<string, object> { { "ProductName", "Track Jacket" }, { "Details", "Wind-resistant" }, { "Gender", "Female" }, { "Price", 69.99 }, { "Stock", 55 } },
                    new Dictionary<string, object> { { "ProductName", "Golf Clubs Set" }, { "Details", "Complete set" }, { "Gender", "Male" }, { "Price", 499.99 }, { "Stock", 10 } },
                    new Dictionary<string, object> { { "ProductName", "Boxing Gloves" }, { "Details", "Leather, durable" }, { "Gender", "Unisex" }, { "Price", 39.99 }, { "Stock", 90 } },
                    new Dictionary<string, object> { { "ProductName", "Tennis Balls" }, { "Details", "Pack of 3" }, { "Gender", "Unisex" }, { "Price", 12.99 }, { "Stock", 120 } },
                    new Dictionary<string, object> { { "ProductName", "Compression Shorts" }, { "Details", "Breathable material" }, { "Gender", "Male" }, { "Price", 24.99 }, { "Stock", 95 } },
                    new Dictionary<string, object> { { "ProductName", "Ski Gloves" }, { "Details", "Insulated" }, { "Gender", "Unisex" }, { "Price", 59.99 }, { "Stock", 35 } },
                    new Dictionary<string, object> { { "ProductName", "Running Shorts" }, { "Details", "Quick-dry fabric" }, { "Gender", "Female" }, { "Price", 29.99 }, { "Stock", 65 } },
                    new Dictionary<string, object> { { "ProductName", "Badminton Shuttlecocks" }, { "Details", "Pack of 12" }, { "Gender", "Unisex" }, { "Price", 14.99 }, { "Stock", 150 } },
                }
            },
            {
                "Sales", new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "ProductId", 1 }, { "Qty", 10 }, { "Branch", "Dandenong" }, { "Date", new DateTime(2023, 8, 5) } },
                    new Dictionary<string, object> { { "ProductId", 2 }, { "Qty", 15 }, { "Branch", "Springvale" }, { "Date", new DateTime(2021, 6, 14) } },
                    new Dictionary<string, object> { { "ProductId", 3 }, { "Qty", 5 }, { "Branch", "Frankston" }, { "Date", new DateTime(2020, 9, 28) } },
                    new Dictionary<string, object> { { "ProductId", 4 }, { "Qty", 8 }, { "Branch", "Melbourne" }, { "Date", new DateTime(2022, 3, 19) } },
                    new Dictionary<string, object> { { "ProductId", 5 }, { "Qty", 12 }, { "Branch", "Geelong" }, { "Date", new DateTime(2021, 11, 2) } },
                    new Dictionary<string, object> { { "ProductId", 6 }, { "Qty", 20 }, { "Branch", "Ballarat" }, { "Date", new DateTime(2024, 2, 22) } },
                    new Dictionary<string, object> { { "ProductId", 7 }, { "Qty", 7 }, { "Branch", "Bendigo" }, { "Date", new DateTime(2023, 7, 18) } },
                    new Dictionary<string, object> { { "ProductId", 8 }, { "Qty", 9 }, { "Branch", "Warrnambool" }, { "Date", new DateTime(2020, 12, 15) } },
                    new Dictionary<string, object> { { "ProductId", 9 }, { "Qty", 11 }, { "Branch", "Horsham" }, { "Date", new DateTime(2021, 5, 30) } },
                    new Dictionary<string, object> { { "ProductId", 10 }, { "Qty", 14 }, { "Branch", "Mildura" }, { "Date", new DateTime(2022, 8, 7) } },
                    new Dictionary<string, object> { { "ProductId", 11 }, { "Qty", 6 }, { "Branch", "Dandenong" }, { "Date", new DateTime(2023, 4, 12) } },
                    new Dictionary<string, object> { { "ProductId", 12 }, { "Qty", 13 }, { "Branch", "Springvale" }, { "Date", new DateTime(2024, 1, 3) } },
                    new Dictionary<string, object> { { "ProductId", 13 }, { "Qty", 11 }, { "Branch", "Frankston" }, { "Date", new DateTime(2021, 10, 24) } },
                    new Dictionary<string, object> { { "ProductId", 14 }, { "Qty", 17 }, { "Branch", "Melbourne" }, { "Date", new DateTime(2022, 9, 11) } },
                    new Dictionary<string, object> { { "ProductId", 15 }, { "Qty", 18 }, { "Branch", "Geelong" }, { "Date", new DateTime(2023, 3, 6) } },
                    new Dictionary<string, object> { { "ProductId", 16 }, { "Qty", 16 }, { "Branch", "Ballarat" }, { "Date", new DateTime(2020, 2, 13) } },
                    new Dictionary<string, object> { { "ProductId", 17 }, { "Qty", 19 }, { "Branch", "Bendigo" }, { "Date", new DateTime(2023, 11, 26) } },
                    new Dictionary<string, object> { { "ProductId", 18 }, { "Qty", 21 }, { "Branch", "Warrnambool" }, { "Date", new DateTime(2024, 5, 17) } },
                    new Dictionary<string, object> { { "ProductId", 19 }, { "Qty", 22 }, { "Branch", "Horsham" }, { "Date", new DateTime(2022, 10, 9) } },
                    new Dictionary<string, object> { { "ProductId", 20 }, { "Qty", 23 }, { "Branch", "Mildura" }, { "Date", new DateTime(2020, 4, 20) } }
                }
            }
        };
    }

    //
    //
    // Load available tables into the dropdowns for the charts
    //
    //
    private void LoadTables()
    {
        var tables = staticDataTables.Keys.ToList();

        // Reset selected index before changing ItemsSource
        lineChartTablePicker.SelectedIndex = -1;
        barChartTablePicker.SelectedIndex = -1;
        pieChartTablePicker.SelectedIndex = -1;

        lineChartTablePicker.ItemsSource = tables;
        barChartTablePicker.ItemsSource = tables;
        pieChartTablePicker.ItemsSource = tables;

        // Detach previous event handlers to avoid duplicate firing
        lineChartTablePicker.SelectedIndexChanged -= LineChartTablePicker_SelectedIndexChanged;
        barChartTablePicker.SelectedIndexChanged -= BarChartTablePicker_SelectedIndexChanged;
        pieChartTablePicker.SelectedIndexChanged -= PieChartTablePicker_SelectedIndexChanged;

        // Re-attach the event handlers
        lineChartTablePicker.SelectedIndexChanged += LineChartTablePicker_SelectedIndexChanged;
        barChartTablePicker.SelectedIndexChanged += BarChartTablePicker_SelectedIndexChanged;
        pieChartTablePicker.SelectedIndexChanged += PieChartTablePicker_SelectedIndexChanged;
    }

    // Loads the column names for line chart when a table is selected
    private void LineChartTablePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedTable = lineChartTablePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedTable)) return;

        LoadColumnsForLineChartTable(selectedTable);
    }

    // Loads the column names for bar chart when a table is selected
    private void BarChartTablePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedTable = barChartTablePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedTable)) return;

        LoadColumnsForBarChartTable(selectedTable);
    }

    // Loads column names for the pie chart when a table is selected
    private void PieChartTablePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedTable = pieChartTablePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedTable)) return;

        LoadColumnsForPieChartTable(selectedTable);
    }

    // Loads column names from the selected table for the line chart
    private void LoadColumnsForLineChartTable(string tableName)
    {
        if (!staticDataTables.ContainsKey(tableName)) return;

        var columns = staticDataTables[tableName].FirstOrDefault()?.Keys.ToList();

        // Check if columns list is valid
        if (columns == null || columns.Count == 0)
        {
            xAxisPicker.ItemsSource = null;
            yAxisPicker.ItemsSource = null;
            xAxisPicker.SelectedIndex = -1;
            yAxisPicker.SelectedIndex = -1;
            return;
        }

        // Reset the selected index before updating ItemsSource
        xAxisPicker.SelectedIndex = -1;
        yAxisPicker.SelectedIndex = -1;

        // Set the items source for the pickers
        xAxisPicker.ItemsSource = columns;
        yAxisPicker.ItemsSource = columns;
    }

    // Loads column names from the selected table for the bar chart
    private void LoadColumnsForBarChartTable(string tableName)
    {
        if (!staticDataTables.ContainsKey(tableName)) return;

        var columns = staticDataTables[tableName].FirstOrDefault()?.Keys.ToList();

        // Check if columns list is valid
        if (columns == null || columns.Count == 0)
        {
            barChartXAxisPicker.ItemsSource = null;
            barChartYAxisPicker.ItemsSource = null;
            barChartXAxisPicker.SelectedIndex = -1;
            barChartYAxisPicker.SelectedIndex = -1;
            return;
        }

        // Reset the selected index before updating ItemsSource
        barChartXAxisPicker.SelectedIndex = -1;
        barChartYAxisPicker.SelectedIndex = -1;

        // Set the items source for the pickers
        barChartXAxisPicker.ItemsSource = columns;
        barChartYAxisPicker.ItemsSource = columns;
        aggregateFunctionPicker.ItemsSource = new List<string> { "Sum", "Average", "Max", "Min" };
    }

    //Loads column names from the selected table for the pie chart
    private void LoadColumnsForPieChartTable(string tableName)
    {
        if (!staticDataTables.ContainsKey(tableName)) return;

        var columns = staticDataTables[tableName].FirstOrDefault()?.Keys.ToList();

        // Check if columns list is valid
        if (columns == null || columns.Count == 0)
        {
            pieChartXAxisPicker.ItemsSource = null;
            pieChartYAxisPicker.ItemsSource = null;
            pieChartXAxisPicker.SelectedIndex = -1;
            pieChartYAxisPicker.SelectedIndex = -1;
            return;
        }

        // Reset the selected index before updating ItemsSource
        pieChartXAxisPicker.SelectedIndex = -1;
        pieChartYAxisPicker.SelectedIndex = -1;

        // Set the items source for the pickers
        pieChartXAxisPicker.ItemsSource = columns;
        pieChartYAxisPicker.ItemsSource = columns;
    }



    //
    //

    //
    //
    //Load and render line chart when button is clicked
    //
    //
    private void OnLoadLineChartClicked(object sender, EventArgs e)
    {
        errorMessageLabel.IsVisible = false;
        lineChartView.IsVisible = false;

        // Gets table, X, Y, and date grouping
        string? selectedTable = lineChartTablePicker.SelectedItem?.ToString();
        string? selectedXAxis = xAxisPicker.SelectedItem?.ToString();
        string? selectedYAxis = yAxisPicker.SelectedItem?.ToString();
        string? selectedDateGrouping = dateGroupingPicker.SelectedItem?.ToString(); // New date grouping picker

        // Data validation
        if (string.IsNullOrEmpty(selectedTable) || string.IsNullOrEmpty(selectedXAxis) || string.IsNullOrEmpty(selectedYAxis))
        {
            ShowErrorMessage("Please select both X and Y axes.");
            return;
        }

        if (!staticDataTables.ContainsKey(selectedTable)) return;

        var dataTable = staticDataTables[selectedTable];

        if (!IsNumericField(dataTable.First()[selectedYAxis].GetType()))
        {
            ShowErrorMessage("The selected Y-Axis field must be numeric.");
            return;
        }

        if (!IsDateField(dataTable.First()[selectedXAxis].GetType()))
        {
            ShowErrorMessage("The selected X-Axis field must be a date or date-convertible field.");
            return;
        }

        // Convert data for line chart
        var trendData = dataTable.Select(row => new TrendData
        {
            Date = Convert.ToDateTime(row[selectedXAxis]),
            MetricValue = Convert.ToSingle(row[selectedYAxis])
        }).ToList();

        // Group trend data based on the selected grouping option (None/Day/Month/Year)
        trendData = GroupTrendData(trendData, selectedDateGrouping);

        // Render chart with grouped data
        SetStaticLineChart(trendData, selectedXAxis, selectedYAxis, selectedDateGrouping);
        lineChartView.IsVisible = true;
    }



    // Group data by Day, Month, or Year
    private List<TrendData> GroupTrendData(List<TrendData> trendData, string selectedGrouping)
    {
        switch (selectedGrouping)
        {
            case "Day":
                return trendData.GroupBy(d => d.Date.Date)
                                .Select(g => new TrendData { Date = g.Key, MetricValue = g.Average(d => d.MetricValue) })
                                .ToList();
            case "Month":
                return trendData.GroupBy(d => new DateTime(d.Date.Year, d.Date.Month, 1))
                                .Select(g => new TrendData { Date = g.Key, MetricValue = g.Average(d => d.MetricValue) })
                                .ToList();
            case "Year":
                return trendData.GroupBy(d => new DateTime(d.Date.Year, 1, 1))
                                .Select(g => new TrendData { Date = g.Key, MetricValue = g.Average(d => d.MetricValue) })
                                .ToList();
            default:
                return trendData; // No grouping
        }
    }
        
    private void SetStaticLineChart(List<TrendData> trendData, string xAxis, string yAxis, string dateGrouping)
    {
        var sortedTrendData = trendData.OrderBy(d => d.Date).ToList();

        var trendEntries = sortedTrendData.Select(d => new ChartEntry(d.MetricValue)
        {
            Label = dateGrouping switch
            {
                "None" => d.Date.ToString("dd/MM/yyyy"), // Default: Full date
                "Day" => d.Date.ToString("dd/MM/yyyy"),  // Group by day
                "Month" => d.Date.ToString("MM/yyyy"),   // Group by month
                "Year" => d.Date.ToString("yyyy"),       // Group by year
                _ => d.Date.ToString("dd/MM/yyyy")       // Fallback to None (dd/MM/yyyy)
            },
            ValueLabel = d.MetricValue.ToString(),
            Color = SKColor.Parse("#8e44ad")
        }).ToArray();

        lineChartView.Chart = new LineChart
        {
            Entries = trendEntries,
            LabelTextSize = 15,
            LabelColor = SKColors.White,
            BackgroundColor = SKColors.Transparent
        };
    }

    private class TrendData
    {
        public DateTime Date { get; set; }
        public float MetricValue { get; set; }
    }

    //
    //

    //
    //
    //Load and render bar chart when button is clicked
    //
    //
    private void OnLoadBarChartClicked(object sender, EventArgs e)
    {
        errorMessageLabel.IsVisible = false;

        //Gets table, columns, and aggregation
        string? selectedTable = barChartTablePicker.SelectedItem?.ToString();
        string? xAxisColumn = barChartXAxisPicker.SelectedItem?.ToString();
        string? yAxisColumn = barChartYAxisPicker.SelectedItem?.ToString();
        string? aggregationFunction = aggregateFunctionPicker.SelectedItem?.ToString();

        //Data validation
        if (string.IsNullOrEmpty(selectedTable) || string.IsNullOrEmpty(xAxisColumn) || string.IsNullOrEmpty(yAxisColumn) || string.IsNullOrEmpty(aggregationFunction))
        {
            ShowErrorMessage("Please select all fields to load the bar chart.");
            return;
        }

        if (!staticDataTables.ContainsKey(selectedTable)) return;

        var tableData = staticDataTables[selectedTable];

        if (!IsStringField(tableData.First()[xAxisColumn].GetType()) && !IsDateField(tableData.First()[xAxisColumn].GetType()))
        {
            ShowErrorMessage("The selected X-Axis field must be a string or date.");
            return;
        }

        if (!IsNumericField(tableData.First()[yAxisColumn].GetType()))
        {
            ShowErrorMessage("The selected Y-Axis field must be numeric.");
            return;
        }

        //Aggregate data, convert aggregate data in chart entries
        var aggregatedData = AggregateData(tableData, xAxisColumn, yAxisColumn, aggregationFunction);

        var barChartEntries = aggregatedData.Select(row => new ChartEntry((float)Convert.ToDouble(row["Value"]))
        {
            Label = row["Label"].ToString(),
            ValueLabel = row["Value"].ToString(),
            Color = SKColor.Parse("#8e44ad") 
        }).ToList();

        //Render bar chart
        barChartView.Chart = new BarChart { 
            Entries = barChartEntries, 
            LabelTextSize = 15,
            LabelColor = SKColors.White,
            BackgroundColor = SKColors.Transparent,
            Margin = 5
        };
        barChartView.IsVisible = true;
    }

    //Aggregate data function 
    private List<Dictionary<string, object>> AggregateData(List<Dictionary<string, object>> data, string xAxisColumn, string yAxisColumn, string aggregationFunction)
    {
        return data
            .GroupBy(row => row[xAxisColumn])
            .Select(group => new Dictionary<string, object>
            {
                { "Label", group.Key },
                { "Value", aggregationFunction switch
                    {
                        "Sum" => group.Sum(row => Convert.ToSingle(row[yAxisColumn])),
                        "Average" => group.Average(row => Convert.ToSingle(row[yAxisColumn])),
                        "Max" => group.Max(row => Convert.ToSingle(row[yAxisColumn])),
                        "Min" => group.Min(row => Convert.ToSingle(row[yAxisColumn])),
                        _ => 0
                    }
                }
            })
            .ToList();
    }

    //
    //
    // Load and render pie chart when button is clicked
    //
    //
    private void OnLoadPieChartClicked(object sender, EventArgs e)
    {
        errorMessageLabel.IsVisible = false;
        pieChartView.IsVisible = false;

        // Get table and columns for the pie chart
        string? selectedTable = pieChartTablePicker.SelectedItem?.ToString();
        string? xAxisColumn = pieChartXAxisPicker.SelectedItem?.ToString();
        string? yAxisColumn = pieChartYAxisPicker.SelectedItem?.ToString();

        // Data validation
        if (string.IsNullOrEmpty(selectedTable) || string.IsNullOrEmpty(xAxisColumn) || string.IsNullOrEmpty(yAxisColumn))
        {
            ShowErrorMessage("Please select all fields to load the pie chart.");
            return;
        }

        if (!staticDataTables.ContainsKey(selectedTable)) return;

        var tableData = staticDataTables[selectedTable];

        if (!IsStringField(tableData.First()[xAxisColumn].GetType()) && !IsDateField(tableData.First()[xAxisColumn].GetType()))
        {
            ShowErrorMessage("The selected X-Axis field must be a string or date.");
            return;
        }

        if (!IsNumericField(tableData.First()[yAxisColumn].GetType()))
        {
            ShowErrorMessage("The selected Y-Axis field must be numeric.");
            return;
        }

        // Aggregate data and convert to chart entries
        var aggregatedData = AggregateData(tableData, xAxisColumn, yAxisColumn, "Sum"); // or other aggregation if needed

        // var pieChartEntries = aggregatedData.Select(row => new ChartEntry((float)Convert.ToDouble(row["Value"]))
        // {
        //     Label = row["Label"].ToString(),
        //     ValueLabel = row["Value"].ToString(),
        //     Color = GetRandomColor() // Assign random color here
        // }).ToArray();

            var pieChartEntries = aggregatedData.Select(row =>
        {
            var color = GetRandomColor(); // Assign random color here
            return new ChartEntry((float)Convert.ToDouble(row["Value"]))
            {
                Label = row["Label"].ToString(),
                ValueLabel = row["Value"].ToString(),
                Color = color,          // Assign the color to the pie segment
                TextColor = color       // Assign the same color to the label
            };
        }).ToArray();

        // Render pie chart
        pieChartView.Chart = new PieChart
        {
            Entries = pieChartEntries,
            LabelTextSize = 15,
            LabelColor = SKColors.White,
            BackgroundColor = SKColors.Transparent
        };

        // Ensure the chart view is updated and visible
        pieChartView.IsVisible = true;
    }

    private SKColor GetRandomColor()
    {
        // Define the palette colors
        var palette = new List<SKColor>
        {
            new SKColor(247, 239, 229),   // #F7EFE5 (light beige)
            new SKColor(226, 191, 217),   // #E2BFD9 (soft pink)
            new SKColor(200, 161, 224),   // #C8A1E0 (lavender)
            new SKColor(103, 65, 136)     // #674188 (dark purple)
        };

        // Randomly select two colors from the palette to interpolate between
        var random = new Random();
        var color1 = palette[random.Next(palette.Count)];
        var color2 = palette[random.Next(palette.Count)];

        // Ensure color1 and color2 are not the same
        while (color1 == color2)
        {
            color2 = palette[random.Next(palette.Count)];
        }

        // Generate a random ratio to interpolate between color1 and color2
        var ratio = (float)random.NextDouble();
        
        // Interpolate between the two colors
        byte r = (byte)(color1.Red + ratio * (color2.Red - color1.Red));
        byte g = (byte)(color1.Green + ratio * (color2.Green - color1.Green));
        byte b = (byte)(color1.Blue + ratio * (color2.Blue - color1.Blue));

        return new SKColor(r, g, b);
    }



    //
    //

    //
    //
    //Shows an error message if faced with invalid user input or data issues
    //
    //
    private void ShowErrorMessage(string message)
    {
        errorMessageLabel.Text = message;
        errorMessageLabel.IsVisible = true;
    }

    //
    //
    //Data validation functions
    //
    //
    private bool IsStringField(Type type)
    {
        return type == typeof(string);
    }
    
    private bool IsDateField(Type type)
    {
        return type == typeof(DateTime) || type == typeof(DateTimeOffset);
    }

    private bool IsNumericField(Type type)
    {
        return type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
    }
    
    //
    //
    //Save State functions - Save to C:\Users\<YourUserName>\AppData\Local\<YourAppName>\chartConfig.json
    //
    //
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Create a new ChartConfig object and assign picker values
        var config = new ChartConfig
        {
            // Bar Chart Config
            BarChartTable = barChartTablePicker.SelectedItem?.ToString(),
            BarChartXAxis = barChartXAxisPicker.SelectedItem?.ToString(),
            BarChartYAxis = barChartYAxisPicker.SelectedItem?.ToString(),
            AggregateFunction = aggregateFunctionPicker.SelectedItem?.ToString(),

            // Pie Chart Config
            PieChartTable = pieChartTablePicker.SelectedItem?.ToString(),
            PieChartXAxis = pieChartXAxisPicker.SelectedItem?.ToString(),
            PieChartYAxis = pieChartYAxisPicker.SelectedItem?.ToString(),

            // Line Chart Config
            LineChartTable = lineChartTablePicker.SelectedItem?.ToString(),
            LineChartXAxis = xAxisPicker.SelectedItem?.ToString(),
            LineChartYAxis = yAxisPicker.SelectedItem?.ToString(),
            DateGrouping = dateGroupingPicker.SelectedItem?.ToString()
        };

        // Serialize the config to JSON format
        var jsonConfig = JsonSerializer.Serialize(config);

        // Define the file path (platform-specific)
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "chartConfig.json");

        // Save to file
        await File.WriteAllTextAsync(filePath, jsonConfig);

        // Optionally display a message
        await DisplayAlert("Save", "Configuration saved successfully!", "OK");
    }

    private async void LoadConfig()
    {
        try
        {
            // Define the file path (platform-specific)
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "chartConfig.json");

            if (File.Exists(filePath))
            {
                // Read the file
                var jsonConfig = await File.ReadAllTextAsync(filePath);

                // Deserialize the configuration
                var config = JsonSerializer.Deserialize<ChartConfig>(jsonConfig);

                // Restore the picker values
                barChartTablePicker.SelectedItem = config.BarChartTable;
                barChartXAxisPicker.SelectedItem = config.BarChartXAxis;
                barChartYAxisPicker.SelectedItem = config.BarChartYAxis;
                aggregateFunctionPicker.SelectedItem = config.AggregateFunction;

                pieChartTablePicker.SelectedItem = config.PieChartTable;
                pieChartXAxisPicker.SelectedItem = config.PieChartXAxis;
                pieChartYAxisPicker.SelectedItem = config.PieChartYAxis;

                lineChartTablePicker.SelectedItem = config.LineChartTable;
                xAxisPicker.SelectedItem = config.LineChartXAxis;
                yAxisPicker.SelectedItem = config.LineChartYAxis;
                dateGroupingPicker.SelectedItem = config.DateGrouping;

                await DisplayAlert("Load", "Configuration loaded successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load configuration: {ex.Message}", "OK");
        }
    }

    private void OnLoadClicked(object sender, EventArgs e)
    {
        LoadConfig();
    }

    public class ChartConfig
    {
        public string? BarChartTable { get; set; }
        public string? BarChartXAxis { get; set; }
        public string? BarChartYAxis { get; set; }
        public string? AggregateFunction { get; set; }

        public string? PieChartTable { get; set; }
        public string? PieChartXAxis { get; set; }
        public string? PieChartYAxis { get; set; }

        public string? LineChartTable { get; set; }
        public string? LineChartXAxis { get; set; }
        public string? LineChartYAxis { get; set; }
        public string? DateGrouping { get; set; }
    }
}
