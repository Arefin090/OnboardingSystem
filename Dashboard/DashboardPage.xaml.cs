using Microcharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Net.Http;
using System.Text.Json; // For serialization
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace OnboardingSystem;

public partial class DashboardPage : ContentPage
{
    private readonly HttpClient _httpClient;
    
    public DashboardPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{Constants.API_BASE_URL}/api/") // Base URL
        };

        LoadTables();
        LoadConfig();
    }

    //
    //

    //
    // Load available tables into the dropdowns for the charts
    //
    private void LoadTables()
    {
        try
        {
            // Retrieve table names from MenuInitializer instead of API
            var tables = MenuInitializer.menuItems.Select(item => item.TableName).ToList();

            if (tables == null || !tables.Any())
            {
                DisplayAlert("Error", "No tables available", "OK");
                return;
            }

            // Reset selected index before changing ItemsSource
            lineChartTablePicker.SelectedIndex = -1;
            barChartTablePicker.SelectedIndex = -1;
            pieChartTablePicker.SelectedIndex = -1;

            // Assign tables to the pickers' ItemsSource
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
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load tables: {ex.Message}", "OK");
        }
    }

    //
    // Loads the column names for chart pickers when a table is selected
    //
    private void LineChartTablePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedTable = lineChartTablePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedTable)) return;

        LoadColumnsForLineChartTable(selectedTable);
    }

    private void BarChartTablePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedTable = barChartTablePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedTable)) return;

        LoadColumnsForBarChartTable(selectedTable);
    }

    private void PieChartTablePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedTable = pieChartTablePicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedTable)) return;

        LoadColumnsForPieChartTable(selectedTable);
    }

    // Loads columns
    private void LoadColumnsForLineChartTable(string tableName)
    {
        var columns = MenuInitializer.menuItems
                                    .Where(item => item.TableName == tableName)
                                    .SelectMany(item => item.ColumnDefinitions.Select(col => col.Name))
                                    .ToList();

        if (columns == null || columns.Count == 0)
        {
            xAxisPicker.ItemsSource = null;
            yAxisPicker.ItemsSource = null;
            xAxisPicker.SelectedIndex = -1;
            yAxisPicker.SelectedIndex = -1;
            return;
        }

        xAxisPicker.SelectedIndex = -1;
        yAxisPicker.SelectedIndex = -1;
        xAxisPicker.ItemsSource = columns;
        yAxisPicker.ItemsSource = columns;
    }

    private void LoadColumnsForBarChartTable(string tableName)
    {
        var columns = MenuInitializer.menuItems
                                    .Where(item => item.TableName == tableName)
                                    .SelectMany(item => item.ColumnDefinitions.Select(col => col.Name))
                                    .ToList();

        if (columns == null || columns.Count == 0)
        {
            barChartXAxisPicker.ItemsSource = null;
            barChartYAxisPicker.ItemsSource = null;
            barChartXAxisPicker.SelectedIndex = -1;
            barChartYAxisPicker.SelectedIndex = -1;
            return;
        }

        barChartXAxisPicker.SelectedIndex = -1;
        barChartYAxisPicker.SelectedIndex = -1;
        barChartXAxisPicker.ItemsSource = columns;
        barChartYAxisPicker.ItemsSource = columns;
        aggregateFunctionPicker.ItemsSource = new List<string> { "Sum", "Count", "AVG", "Max", "Min" };
    }

    private void LoadColumnsForPieChartTable(string tableName)
    {
        var columns = MenuInitializer.menuItems
                                    .Where(item => item.TableName == tableName)
                                    .SelectMany(item => item.ColumnDefinitions.Select(col => col.Name))
                                    .ToList();

        if (columns == null || columns.Count == 0)
        {
            pieChartXAxisPicker.ItemsSource = null;
            pieChartYAxisPicker.ItemsSource = null;
            pieChartXAxisPicker.SelectedIndex = -1;
            pieChartYAxisPicker.SelectedIndex = -1;
            return;
        }

        pieChartXAxisPicker.SelectedIndex = -1;
        pieChartYAxisPicker.SelectedIndex = -1;
        pieChartXAxisPicker.ItemsSource = columns;
        pieChartYAxisPicker.ItemsSource = columns;
    }

    //
    //
        
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

    //
    //

    //
    //
    //Load and render line chart when button is clicked
    //
    //
    private async void OnLoadLineChartClicked(object sender, EventArgs e)
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

        // Check if the selected columns are valid
        if (!IsColumnTypeValidFor(selectedTable, selectedXAxis, "DATE", "DATETIME", "DATETIMEOFFSET"))
        {
            ShowErrorMessage("The selected X-Axis field must be a date.");
            return;
        }

        if (!IsColumnTypeValidFor(selectedTable, selectedYAxis, "INT", "DECIMAL", "FLOAT", "DOUBLE"))
        {
            ShowErrorMessage("The selected Y-Axis field must be numeric.");
            return;
        }

        // Fetch line chart data from the API
        var trendData = await FetchLineChartData(selectedTable, selectedXAxis, selectedYAxis);

        if (trendData == null || !trendData.Any())
        {
            ShowErrorMessage("No data available for the selected options.");
            return;
        }

        // Group trend data based on the selected grouping option (None/Day/Month/Year)
        trendData = GroupTrendData(trendData, selectedDateGrouping);

        // Render chart with grouped data
        SetStaticLineChart(trendData, selectedXAxis, selectedYAxis, selectedDateGrouping);
        lineChartView.IsVisible = true;
    }

    private async Task<List<TrendData>> FetchLineChartData(string table, string columnX, string columnY)
    {
        var trendData = new List<TrendData>();

        try
        {
            var response = await _httpClient.GetAsync($"{Constants.API_BASE_URL}{Constants.DASHBOARD_ENDPOINT}?table={table}&x={columnX}&y={columnY}&aggregationType=0");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + jsonResponse);

                var jsonDocument = JsonDocument.Parse(jsonResponse);

                // Navigate directly to the "data" array
                var dataArray = jsonDocument.RootElement.GetProperty("data");

                // Populate trendData with dynamic keys
                foreach (var item in dataArray.EnumerateArray())
                {
                    var trendEntry = new TrendData
                    {
                        Date = Convert.ToDateTime(item.GetProperty(columnX).GetString()), // Use dynamic X
                        MetricValue = item.GetProperty($"SUM({columnY})").GetInt32() // Use the summed value directly
                    };
                    trendData.Add(trendEntry);
                }
            }
            else
            {
                ShowErrorMessage("Failed to load data from the server.");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error fetching chart data: {ex.Message}");
        }

        return trendData;
    }

    // Group data by Day, Month, or Year
    private List<TrendData> GroupTrendData(List<TrendData> trendData, string selectedGrouping)
    {
        switch (selectedGrouping)
        {
            case "Day":
                return trendData.GroupBy(d => d.Date.Date)
                                .Select(g => new TrendData { Date = g.Key, MetricValue = g.Sum(d => d.MetricValue) })
                                .ToList();
            case "Month":
                return trendData.GroupBy(d => new DateTime(d.Date.Year, d.Date.Month, 1))
                                .Select(g => new TrendData { Date = g.Key, MetricValue = g.Sum(d => d.MetricValue) })
                                .ToList();
            case "Year":
                return trendData.GroupBy(d => new DateTime(d.Date.Year, 1, 1))
                                .Select(g => new TrendData { Date = g.Key, MetricValue = g.Sum(d => d.MetricValue) })
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
    private async void OnLoadBarChartClicked(object sender, EventArgs e)
    {
        errorMessageLabel.IsVisible = false;

        // Gets table, columns, and aggregation
        string? selectedTable = barChartTablePicker.SelectedItem?.ToString();
        string? xAxisColumn = barChartXAxisPicker.SelectedItem?.ToString();
        string? yAxisColumn = barChartYAxisPicker.SelectedItem?.ToString();
        string? aggregationFunction = aggregateFunctionPicker.SelectedItem?.ToString();

        // Data validation
        if (string.IsNullOrEmpty(selectedTable) || string.IsNullOrEmpty(xAxisColumn) || 
            string.IsNullOrEmpty(yAxisColumn) || string.IsNullOrEmpty(aggregationFunction))
        {
            ShowErrorMessage("Please select all fields to load the bar chart.");
            return;
        }

        // Check if the selected columns are valid
        if (!IsColumnTypeValidFor(selectedTable, xAxisColumn, "VARCHAR", "STRING", "DATE", "DATETIME", "DATETIMEOFFSET"))
        {
            ShowErrorMessage("The selected X-Axis field must be a string or date.");
            return;
        }

        if (!IsColumnTypeValidFor(selectedTable, yAxisColumn, "INT", "DECIMAL", "FLOAT", "DOUBLE"))
        {
            ShowErrorMessage("The selected Y-Axis field must be numeric.");
            return;
        }

        // Fetch data from the API
        var barChartData = await FetchBarChartData(selectedTable, xAxisColumn, yAxisColumn, aggregationFunction);

        if (barChartData == null || !barChartData.Any())
        {
            ShowErrorMessage("No data available for the selected options.");
            return;
        }

        // Convert API data into chart entries
        var barChartEntries = barChartData.Select(row => new ChartEntry((float)Convert.ToDouble(row["Value"]))
        {
            Label = row["Label"].ToString(),
            ValueLabel = row["Value"].ToString(),
            Color = SKColor.Parse("#8e44ad")
        }).ToList();

        // Render bar chart
        barChartView.Chart = new BarChart
        {
            Entries = barChartEntries,
            LabelTextSize = 15,
            LabelColor = SKColors.White,
            BackgroundColor = SKColors.Transparent,
            Margin = 5
        };
        barChartView.IsVisible = true;
    }

    private async Task<List<Dictionary<string, object>>> FetchBarChartData(string table, string xAxis, string yAxis, string aggregationFunction)
    {
        var barChartData = new List<Dictionary<string, object>>();
        int aggregationType = -1;

        // Map aggregationFunction to its respective enum integer value
        switch (aggregationFunction)
        {
            case "Sum":
                aggregationType = 0;
                break;
            case "Count":
                aggregationType = 1;
                break;
            case "AVG":
                aggregationType = 2;
                break;
            case "Max":
                aggregationType = 3;
                break;
            case "Min":
                aggregationType = 4;
                break;
            default:
                ShowErrorMessage("Invalid aggregation function.");
                return barChartData;
        }

        try
        {
            // Make API call with the appropriate aggregation type
            var response = await _httpClient.GetAsync($"{Constants.API_BASE_URL}{Constants.DASHBOARD_ENDPOINT}?table={table}&x={xAxis}&y={yAxis}&aggregationType={aggregationType}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + jsonResponse);

                var jsonDocument = JsonDocument.Parse(jsonResponse);

                // Navigate directly to the "data" array
                var dataArray = jsonDocument.RootElement.GetProperty("data");

                // Populate the barChartData list
                foreach (var item in dataArray.EnumerateArray())
                {
                    var dataEntry = new Dictionary<string, object>
                    {
                        { "Label", item.GetProperty(xAxis).GetString() }, // Dynamically get X value
                        { "Value", item.GetProperty($"{aggregationFunction.ToUpper()}({yAxis})").GetDouble() } // Get aggregated value dynamically
                    };
                    barChartData.Add(dataEntry);
                }
            }
            else
            {
                ShowErrorMessage("Failed to load data from the server.");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error fetching chart data: {ex.Message}");
        }

        return barChartData; // Return the list of dictionaries
    }

    //
    //

    //
    //
    // Load and render pie chart when button is clicked
    //
    //
    private async void OnLoadPieChartClicked(object sender, EventArgs e)
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

        // Check if the selected columns are valid
        if (!IsColumnTypeValidFor(selectedTable, xAxisColumn, "VARCHAR", "STRING", "DATE", "DATETIME", "DATETIMEOFFSET"))
        {
            ShowErrorMessage("The selected X-Axis field must be a string or date.");
            return;
        }

        if (!IsColumnTypeValidFor(selectedTable, yAxisColumn, "INT", "DECIMAL", "FLOAT", "DOUBLE"))
        {
            ShowErrorMessage("The selected Y-Axis field must be numeric.");
            return;
        }

        // Fetch pie chart data from the API
        var pieChartData = await FetchPieChartData(selectedTable, xAxisColumn, yAxisColumn);

        if (pieChartData == null || !pieChartData.Any())
        {
            ShowErrorMessage("No data available for the selected options.");
            return;
        }

        // Convert API data into pie chart entries
        var pieChartEntries = pieChartData.Select(row =>
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

    // Fetch data from the API for pie chart
    private async Task<List<Dictionary<string, object>>> FetchPieChartData(string table, string xAxis, string yAxis)
    {
        var pieChartData = new List<Dictionary<string, object>>();

        try
        {
            var response = await _httpClient.GetAsync($"{Constants.API_BASE_URL}{Constants.DASHBOARD_ENDPOINT}?table={table}&x={xAxis}&y={yAxis}&aggregationType=SUM");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + jsonResponse);

                var jsonDocument = JsonDocument.Parse(jsonResponse);

                // Navigate directly to the "data" array
                var dataArray = jsonDocument.RootElement.GetProperty("data");

                // Populate the pieChartData list
                foreach (var item in dataArray.EnumerateArray())
                {
                    var dataEntry = new Dictionary<string, object>
                    {
                        { "Label", item.GetProperty(xAxis).GetString() }, // Dynamically get X value
                        { "Value", item.GetProperty($"SUM({yAxis})").GetDouble() } // Use the summed value directly
                    };
                    pieChartData.Add(dataEntry);
                }
            }
            else
            {
                ShowErrorMessage("Failed to load data from the server.");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error fetching chart data: {ex.Message}");
        }

        return pieChartData; // Return the list of dictionaries
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
    private bool IsColumnTypeValidFor(string tableName, string columnName, params string[] expectedTypes)
    {
        var table = MenuInitializer.menuItems.FirstOrDefault(item => item.TableName == tableName);
        
        if (table != null)
        {
            var column = table.ColumnDefinitions.FirstOrDefault(c => c.Name == columnName);
            if (column != null)
            {
                return expectedTypes.Any(expectedType => 
                    column.Type.StartsWith(expectedType, StringComparison.OrdinalIgnoreCase));
            }
        }
        
        return false; // Column or table not found
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
    }

	// method for PDF generation
	private async void OnGenerateReportClicked(object sender, EventArgs e)
	{
		// Replace button text with loading indicator
		var generateReportButtonText = GenerateReportButton.Text; //save button text
		var originalButtonWidth = GenerateReportButton.Width; // save original width

		// Ensure the button width is fixed when changing text
		GenerateReportButton.Text = "";
		GenerateReportButton.WidthRequest = originalButtonWidth;
		GenerateReportButton.IsEnabled = false;
		LoadingIndicator.IsVisible = true;
		LoadingIndicator.IsRunning = true;
        await Task.Delay(800);
		try
		{
            // Create a new PDF document
            var pdfDocument = new PdfDocument();
			var pdfPage = pdfDocument.AddPage();
			var gfx = XGraphics.FromPdfPage(pdfPage);

			// Set up fonts and formatting
			var titleFont = new XFont("Verdana", 20, XFontStyle.Bold);
			var tableTitleFont = new XFont("Verdana", 14, XFontStyle.Bold);
			var tableDataFont = new XFont("Verdana", 10, XFontStyle.Regular);
			var errorFont = new XFont("Verdana", 10, XFontStyle.Italic);

			// Set up pen for drawing table borders
			var tablePen = new XPen(XColors.Black, 1);

			// Starting coordinates for the PDF layout
			double yPoint = 40;
			double pageHeightLimit = pdfPage.Height - 40;

			// Add Title for the PDF
			gfx.DrawString("Database Tables Report", titleFont, XBrushes.Black,
				new XRect(0, yPoint, pdfPage.Width, pdfPage.Height), XStringFormats.TopCenter);
			yPoint += 40;

			// Loop through the tables in MenuInitializer and generate content
			foreach (var menuItem in MenuInitializer.menuItems)
			{
				// Check if table is null or inaccessible
				if (menuItem.ColumnDefinitions == null)
				{
					gfx.DrawString($"Table {menuItem.TableName} is inaccessible or not defined.", errorFont, XBrushes.Red,
						new XRect(20, yPoint, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
					yPoint += 40;
					CheckPageHeight(ref yPoint, ref pdfPage, ref gfx, pageHeightLimit, 40);
					continue; // Skip to the next table if inaccessible
				}

				// Ensure the table has columns defined
				if (!menuItem.ColumnDefinitions.Any())
				{
					gfx.DrawString($"Table {menuItem.TableName} has no columns defined.", errorFont, XBrushes.Red,
						new XRect(20, yPoint, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
					yPoint += 40;
					CheckPageHeight(ref yPoint, ref pdfPage, ref gfx, pageHeightLimit, 40);
					continue; // Skip to the next table if no columns are defined
				}

				// Table Title
				gfx.DrawString($"Table: {menuItem.TableName}", tableTitleFont, XBrushes.Black,
					new XRect(20, yPoint, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
				yPoint += 20;

				// Define variables to calculate dynamic widths
				double rowHeight = 20;
				double padding = 10;  // Padding inside each cell
				double leftPadding = 5; // Horizontal padding between text and the cell border

				// Initial column widths based on headers
				double column1Width = gfx.MeasureString("Column Name", tableDataFont).Width + padding;
				double column2Width = gfx.MeasureString("Type", tableDataFont).Width + padding;
				double column3Width = gfx.MeasureString("Key", tableDataFont).Width + padding;
				double column4Width = gfx.MeasureString("Constraint", tableDataFont).Width + padding;

				// Calculate the max width of data for each column
				foreach (var column in menuItem.ColumnDefinitions)
				{
					// Column Name
					double nameWidth = gfx.MeasureString(column.Name ?? "None", tableDataFont).Width + padding;
					column1Width = Math.Max(column1Width, nameWidth);

					// Type
					double typeWidth = gfx.MeasureString(column.Type ?? "None", tableDataFont).Width + padding;
					column2Width = Math.Max(column2Width, typeWidth);

					// Key
					double keyWidth = gfx.MeasureString(column.Key ? "Yes" : "No", tableDataFont).Width + padding;
					column3Width = Math.Max(column3Width, keyWidth);

					// Constraint
					double constraintWidth = gfx.MeasureString(column.Constraint ?? "None", tableDataFont).Width + padding;
					column4Width = Math.Max(column4Width, constraintWidth);
				}

				// Draw column headers with dynamic widths
				double xStart = 20;

				gfx.DrawRectangle(tablePen, xStart, yPoint, column1Width, rowHeight);
				gfx.DrawString("Column Name", tableDataFont, XBrushes.Black,
					new XRect(xStart + leftPadding, yPoint, column1Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

				gfx.DrawRectangle(tablePen, xStart + column1Width, yPoint, column2Width, rowHeight);
				gfx.DrawString("Type", tableDataFont, XBrushes.Black,
					new XRect(xStart + column1Width + leftPadding, yPoint, column2Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

				gfx.DrawRectangle(tablePen, xStart + column1Width + column2Width, yPoint, column3Width, rowHeight);
				gfx.DrawString("Key", tableDataFont, XBrushes.Black,
					new XRect(xStart + column1Width + column2Width + leftPadding, yPoint, column3Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

				gfx.DrawRectangle(tablePen, xStart + column1Width + column2Width + column3Width, yPoint, column4Width, rowHeight);
				gfx.DrawString("Constraint", tableDataFont, XBrushes.Black,
					new XRect(xStart + column1Width + column2Width + column3Width + leftPadding, yPoint, column4Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

				yPoint += rowHeight;

				// Loop through each column and print data with dynamic widths
				foreach (var column in menuItem.ColumnDefinitions)
				{
					// Check if new page is required before drawing each row
					CheckPageHeight(ref yPoint, ref pdfPage, ref gfx, pageHeightLimit, rowHeight);

					// Draw data row with borders
					gfx.DrawRectangle(tablePen, xStart, yPoint, column1Width, rowHeight);
					gfx.DrawString(column.Name ?? "None", tableDataFont, XBrushes.Black,
						new XRect(xStart + leftPadding, yPoint, column1Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

					gfx.DrawRectangle(tablePen, xStart + column1Width, yPoint, column2Width, rowHeight);
					gfx.DrawString(column.Type ?? "None", tableDataFont, XBrushes.Black,
						new XRect(xStart + column1Width + leftPadding, yPoint, column2Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

					gfx.DrawRectangle(tablePen, xStart + column1Width + column2Width, yPoint, column3Width, rowHeight);
					gfx.DrawString(column.Key ? "Yes" : "No", tableDataFont, XBrushes.Black,
						new XRect(xStart + column1Width + column2Width + leftPadding, yPoint, column3Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

					gfx.DrawRectangle(tablePen, xStart + column1Width + column2Width + column3Width, yPoint, column4Width, rowHeight);
					gfx.DrawString(column.Constraint ?? "None", tableDataFont, XBrushes.Black,
						new XRect(xStart + column1Width + column2Width + column3Width + leftPadding, yPoint, column4Width - 2 * leftPadding, rowHeight), XStringFormats.CenterLeft);

					yPoint += rowHeight;
				}

				yPoint += 40; // Add more space between tables for readability
			}

			// Save the PDF to a file
			var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DatabaseReport.pdf");
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				pdfDocument.Save(stream);
			}

			// Notify user that the PDF was generated
			await DisplayAlert("PDF Report", $"PDF report generated successfully at {filePath}", "OK");
		}
		catch (Exception ex)
		{
			// Handle errors and show a message to the user
			await DisplayAlert("Error", $"Failed to generate PDF report: {ex.Message}", "OK");
		}

		// After PDF generation is complete, replace the loading indicator with the saved button text
		GenerateReportButton.Text = generateReportButtonText; //revert to original button text
		GenerateReportButton.IsEnabled = true;
		LoadingIndicator.IsRunning = false;
		LoadingIndicator.IsVisible = false;
		GenerateReportButton.WidthRequest = -1; //reset width request
	}

	// Helper function to check if a new page is required
	private void CheckPageHeight(ref double yPoint, ref PdfPage pdfPage, ref XGraphics gfx, double pageHeightLimit, double rowHeight)
	{
		// Check if there is enough space for the next row
		if (yPoint + rowHeight > pageHeightLimit)
		{
			// Add new page
			pdfPage = pdfPage.Owner.AddPage();
			gfx = XGraphics.FromPdfPage(pdfPage);
			yPoint = 40; // Reset yPoint for new page (with some margin)
		}
	}
}