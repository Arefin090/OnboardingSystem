using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using OnboardingSystem.Authentication;

namespace OnboardingSystem
{
    public partial class UserListPage : ContentPage, IDisposable
    {
        private readonly IAuthenticationService _authService;
        private readonly HttpClient _httpClient;

        public ObservableCollection<JsonObject> Items { get; private set; }
        private ObservableCollection<JsonObject> _itemList;

        public UserListPage(IAuthenticationService authService)
        {
            InitializeComponent();

            _authService = authService;
            _httpClient = new HttpClient { BaseAddress = new Uri(Constants.API_BASE_URL) };

            Items = new ObservableCollection<JsonObject>();
            _itemList = new ObservableCollection<JsonObject>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                Console.WriteLine("Starting LoadData method");

                var token = await _authService.GetValidTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Failed to obtain a valid token. Navigating to login page.");
                    await DisplayAlert("Error", "Please log in to access this page.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                Console.WriteLine("Valid token obtained. Fetching user list.");

                var userList = await GetUserListAsync(token);
                if (userList != null)
                {
                    Console.WriteLine($"User list fetched successfully. Count: {userList.Count}");
                    UpdateItemsList(userList);
                    GenerateTable();
                }
                else
                {
                    Console.WriteLine("Failed to parse user list.");
                    await DisplayAlert("Error", "Failed to retrieve user list.", "OK");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.WriteLine("User is not authorized to access this resource.");
                await DisplayAlert("Access Denied", "You do not have permission to view this page.", "OK");
                await Shell.Current.GoToAsync("//DashboardPage"); // Or another appropriate page
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadData: {ex.Message}");
                await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
            }
        }

        private async Task<JsonArray> GetUserListAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
			var popup = new AddUserPopup();

			var result = await this.ShowPopupAsync(popup) as dynamic;

			// Call the method to add a user
			if (result != null)
			{
				await RegisterUserAsync(result.username, result.firstName, result.lastName, result.phone, result.role, result.password);
			}
		}

		private async void OnDeleteUserClicked(object sender, EventArgs e)
		{
			string delete = await DisplayPromptAsync("Delete User", "Enter user id:");

			// Call the method to delete a user
			if (!string.IsNullOrWhiteSpace(delete))
			{
				try
				{
					await DeleteUserAsync(Int32.Parse(delete));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
				}
			}
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