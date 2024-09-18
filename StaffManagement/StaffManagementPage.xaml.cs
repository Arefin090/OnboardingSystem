using System.Collections.ObjectModel;
using System.Data.Common;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace OnboardingSystem;
public partial class StaffManagementPage : ContentPage
{
	// URL of the API endpoint for staff management
	private static readonly string ApiUrl = "https://localhost:44339/api/User";
	private readonly HttpClient _httpClient;
	public ObservableCollection<JsonObject> Items { get; set; }

	public StaffManagementPage()
	{
		InitializeComponent();
		//CreateDynamicTable();
		_httpClient = new HttpClient { BaseAddress = new Uri(ApiUrl) };

		Items = new ObservableCollection<JsonObject>();

		BindingContext = this;

		LoadData();
	}

	private async void LoadData()
	{
		var auth = new UserAuthenticator();

		// Check if the user is authorized
		bool isAuthorized = await auth.IsUserAuthorizedAsync();

		if (!isAuthorized)
		{
			await DisplayAlert("Unauthorized", "You are not authorized to access this data.", "OK");
			return;
		}

		try
		{
			var token = await auth.GetTokenAsync();
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Bearer " + token);

			var response = await _httpClient.GetStringAsync(""); // GET request
			System.Diagnostics.Debug.WriteLine("API Response: " + response);

			var itemsList = JsonNode.Parse(response)?.AsArray();

			if (itemsList != null)
			{
				Items.Clear();
				foreach (var item in itemsList)
				{

					System.Diagnostics.Debug.WriteLine(item.AsObject());
					var jsonObject = item.AsObject();
					Items.Add(jsonObject); // Add to the ObservableCollection
				}

				// Setup dynamic template after loading data
				Setup();
			}
			var sampleItem = Items.FirstOrDefault();
			System.Diagnostics.Debug.WriteLine(sampleItem);
			foreach (var key in sampleItem)
			{
				System.Diagnostics.Debug.WriteLine(key.Key);
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
		}
	}

	private void SetupDynamicItemTemplate()
	{
		var collectionView = new CollectionView
		{
			ItemsSource = Items,
			ItemTemplate = new DataTemplate(() =>
			{
				var stackLayout = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					Padding = 10,
					Spacing = 5
				};

				var sampleItem = Items.FirstOrDefault();
				if (sampleItem != null)
				{
					foreach (var key in sampleItem)
					{
						var label = new Label
						{
							FontSize = 14,
							TextColor = Color.FromRgb(100,100,100),
						};

						// Dynamically create the binding
						label.SetBinding(Label.TextProperty, new Binding(key.Key));
						stackLayout.Children.Add(label);
					}
				}

				return new ViewCell { View = stackLayout };
			})
		};

		// Replace existing CollectionView with the dynamically created one
		Content = collectionView;
	}

	private void Setup()
	{
		CollectionView collectionView = new CollectionView();
		collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Items");
		collectionView.ItemTemplate = new DataTemplate(() =>
		{
			Grid grid = new Grid { Padding = 10 };
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			Label idLabel = new Label { FontAttributes = FontAttributes.Bold };
			idLabel.SetBinding(Label.TextProperty, "[id]");

			Label usernameLabel = new Label { FontAttributes = FontAttributes.Bold };
			usernameLabel.SetBinding(Label.TextProperty, "[username]");

			Label firstnameLabel = new Label { FontAttributes = FontAttributes.Bold };
			firstnameLabel.SetBinding(Label.TextProperty, "[firstName]");

			Label lastnameLabel = new Label { FontAttributes = FontAttributes.Bold };
			lastnameLabel.SetBinding(Label.TextProperty, "[lastName]");

			Label phoneLabel = new Label { FontAttributes = FontAttributes.Bold };
			phoneLabel.SetBinding(Label.TextProperty, "[phone]");

			Label roleLabel = new Label { FontAttributes = FontAttributes.Bold };
			roleLabel.SetBinding(Label.TextProperty, "[role]");

			//Label locationLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
			//locationLabel.SetBinding(Label.TextProperty, "[role]");

			grid.Add(idLabel);
			grid.Add(usernameLabel, 1, 0);
			grid.Add(firstnameLabel, 2, 0);
			grid.Add(lastnameLabel, 3, 0);
			grid.Add(phoneLabel, 4, 0);
			grid.Add(roleLabel, 5, 0);

			return grid;
		});
		Content = collectionView;
		//Content = new ScrollView
		//{
		//	Content = new Grid
		//	{
		//		RowDefinitions = Rows.Define((Auto), (Auto)),

		//		Children =
		//		{
		//			new Grid
		//			{
		//				RowDefinitions = Rows.Define(Auto),

		//				ColumnDefinitions = Columns.Define((Stars(.35)), (Stars(.2)), (Stars(.15)), (Stars(.15)), (Stars(.15))),

		//				Children =
		//				{
		//					new Entry
		//					{
		//						Keyboard = Keyboard.Numeric,
		//						BackgroundColor = Colors.AliceBlue,
		//					}.Column(0)
		//						.FontSize(15)
		//						.Placeholder("Search")
		//						.TextColor(Colors.Black)
		//						//.Height(44)
		//						//.Margin(6, 6)
		//						.Bind(Entry.TextProperty, "Apples", BindingMode.TwoWay),

		//					new Button
		//					{
		//					}.Text("Add")
		//					.Column(2),
		//					new Button
		//					{
		//					}.Text("Delete")
		//					.Column(3),
		//					new Button
		//					{
		//					}.Text("Filter")
		//					.Column(4)
		//				}
		//			},

		//			new Grid
		//			{
		//				RowDefinitions = Rows.Define(Auto),

		//				ColumnDefinitions = Columns.Define((Star), (Star), (Star), (Star), (Star)),

		//				Children =
		//				{
		//					new Label()
		//						.Text("Customer name:")
		//						.Column(0),
		//					new Label()
		//						.Text("Customer name:")
		//						.Column(1),
		//					new Label()
		//						.Text("Customer name:")
		//						.Column(2),
		//					new Label()
		//						.Text("Customer name:")
		//						.Column(3),
		//					new Label()
		//						.Text("Customer name:")
		//						.Column(4),
		//					new Label()
		//						.Text("Customer name:")
		//						.Column(5),
		//				}
		//			}.Row(1),
		//		}
		//	}
		//};
	}
}
