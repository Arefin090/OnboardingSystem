using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using OnboardingSystem.Authentication;
using OnboardingSystem.Helpers;


namespace OnboardingSystem
{
    public partial class UserListPage : ContentPage, IDisposable
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;
        private readonly HttpClient _httpClient;
        private readonly Authorization _authHelper;

        public ObservableCollection<JsonObject> Items { get; set; }

		private ObservableCollection<JsonObject> _itemList;

        public UserListPage(IAuthenticationService authService, IUserService userService)
        {
            InitializeComponent();

            _authService = authService;
            _userService = userService;
            _httpClient = new HttpClient { BaseAddress = new Uri(Constants.API_BASE_URL) };
            _authHelper = new Authorization(_authService);

            Items = new ObservableCollection<JsonObject>();
			_itemList = new ObservableCollection<JsonObject>();
            BindingContext = this;

            LoadData();
        }

        private async void LoadData()
        {
            if (!await _authHelper.IsUserAuthorized(Constants.ROLE_ADMIN, Constants.ROLE_STAFF))
            {
                return;
            }

            try
            {
                var token = await _authHelper.GetValidTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    await DisplayAlert("Error", "Failed to obtain a valid token. Please log in again.", "OK");
                    return;
                }

                var userList = await FetchUserListAsync(token);
                if (userList != null)
                {
                    UpdateItemsList(userList);
                    GenerateTable();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to parse user list.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
            }
        }

        private async Task<JsonArray> FetchUserListAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetStringAsync(Constants.GET_USER_LIST_ENDPOINT);
            return JsonNode.Parse(response)?.AsArray();
        }

        private void UpdateItemsList(JsonArray userList)
        {
			_itemList.Clear();
			Items.Clear();
            foreach (var item in userList)
            {
				_itemList.Add(item.AsObject());
				Items.Add(item.AsObject());
            }
        }

	private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
	{
		// Get the search text
		string searchText = e.NewTextValue.ToLower();

		// Clear the current Items collection
		Items.Clear();

		// Filter the _allItems collection and add matching items to Items
		foreach (var item in _itemList)
		{
			// Assuming "Username", "FirstName", "LastName" are fields you want to search
			if (item["username"]?.ToString().ToLower().Contains(searchText) == true ||
				item["firstName"]?.ToString().ToLower().Contains(searchText) == true ||
				item["lastName"]?.ToString().ToLower().Contains(searchText) == true)
			{
				Items.Add(item); // Add matching item to filtered Items collection
			}
		}
	}

	private async void GenerateTable()
	{
		var headers = new List<string> { "ID", "Username", "First Name", "Last Name", "Phone Number", "Role" };

		// Clear any existing column definitions
		HeaderGrid.ColumnDefinitions.Clear();
		var columnWidths = GetColumnDefinitions(headers.Count);
		foreach (var width in columnWidths)
		{
			HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
		}

		// Add labels to the grid
		for (int i = 0; i < headers.Count; i++)
		{
			var label = new Label
			{
				Text = headers[i],
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
			HeaderGrid.Children.Add(label);
			HeaderGrid.SetColumn(label, i);
		}
		AddCollectionView(headers.Count());
	}

	private List<GridLength> GetColumnDefinitions(int numOfColumns)
	{
		var columnWidths = new List<GridLength>();
		for (int i = 0; i < numOfColumns; i++)
		{
			columnWidths.Add(new GridLength(1, GridUnitType.Star)); // Defines Column Size
		}
		return columnWidths;
	}

	private void AddCollectionView(int numOfColumns)
	{
		GridCollection.ItemsSource = Items;
		var columnWidths = GetColumnDefinitions(numOfColumns);
		GridCollection.ItemTemplate = new DataTemplate(() =>
		{
			var grid = new Grid { Padding = 10 };
			foreach (var width in columnWidths)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
			}
			foreach (var row in Items)
			{
				int column = 0;
				foreach (var key in row)
				{
					var item = new Label();
					item.SetBinding(Label.TextProperty, $"[{key.Key}]");
					item.Margin = new Thickness(2, 0, 2, 0);
					grid.Children.Add(item);
					grid.SetColumn(item, column);

					var separator = new BoxView
					{
						HeightRequest = 1, // Line thickness
						BackgroundColor = Colors.Gray, // Line color
						HorizontalOptions = LayoutOptions.Fill, // Expand to fill width
						Margin = new Thickness(0, 40, 0, 0) // Add some spacing above and below
					};
					grid.Children.Add(separator);
					grid.SetColumn(separator, column);

					column++;
				}
			}

			return grid;
		});
	}


	private async void OnAddUserClicked(object sender, EventArgs e)
	{
		string username = await DisplayPromptAsync("Add User", "Enter username:");
		string firstName = await DisplayPromptAsync("Add User", "Enter first name:");
		string lastName = await DisplayPromptAsync("Add User", "Enter last name:");
		string phone = await DisplayPromptAsync("Add User", "Enter phone number:");
		string role = await DisplayPromptAsync("Add User", "Enter role:");
		string password = await DisplayPromptAsync("Add User", "Enter password:");

		// Call the method to register a user
		await RegisterUserAsync(username, firstName, lastName, phone, role, password);
	}

	private async void OnDeleteUserClicked(object sender, EventArgs e)
	{
		string delete = await DisplayPromptAsync("Delete User", "Enter user id:");

		// Call the method to delete a use
		try
		{
			await DeleteUserAsync(Int32.Parse(delete));
		}
		catch (Exception ex)
		{}
	}


	private async Task RegisterUserAsync(string username, string firstName, string lastName, string phone, string role, string password)
	{
		var newUser = new
		{
			Username = username,
			FirstName = firstName,
			LastName = lastName,
			Phone = phone,
			Role = role,
			Password = password
		};

		try
		{
			var response = await _httpClient.PostAsJsonAsync("https://localhost:44339/api/User/register", newUser);
			if (response.IsSuccessStatusCode)
			{
				await DisplayAlert("Success", "User registered successfully.", "OK");
				LoadData(); // Reload the user list after registration
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				await DisplayAlert("Error", $"Registration failed: {error}", "OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
		}
	}

	private async Task DeleteUserAsync(int userId)
	{
		try
		{
			var response = await _httpClient.DeleteAsync($"https://localhost:44339/api/User/{userId}");
			if (response.IsSuccessStatusCode)
			{
				await DisplayAlert("Success", "User deleted successfully.", "OK");
				LoadData(); // Reload the user list after deletion
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				await DisplayAlert("Error", $"Deletion failed: {error}", "OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
		}
	}

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}