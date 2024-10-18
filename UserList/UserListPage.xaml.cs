using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using OnboardingSystem.Authentication;
using PdfSharpCore;

namespace OnboardingSystem
{
	public partial class UserListPage : ContentPage, IDisposable
	{
		private readonly IAuthenticationService _authService; // user authentication
		private readonly HttpClient _httpClient; // database connection

		private ObservableCollection<JsonObject> Items = new ObservableCollection<JsonObject>(); // list of items for display
		private List<ObservableCollection<JsonObject>> _paginateItemList = new List<ObservableCollection<JsonObject>>(); // paginated list of items
		private ObservableCollection<JsonObject> _allItemList = []; // original list of items

		private int _currentPage = 1; // current page for pagination

		public UserListPage(IAuthenticationService authService)
		{
			InitializeComponent();

			_authService = authService;
			_httpClient = new HttpClient { BaseAddress = new Uri(Constants.API_BASE_URL) };

			BindingContext = this;
		}

		public int CurrentPage
		{
			get => _currentPage;
			set
			{
				if (_currentPage != value)
				{
					_currentPage = value;
					OnPropertyChanged(nameof(CurrentPage));
				}
			}
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await LoadData(); // load table items from database
		}

		private async Task LoadData()
		{
			LoadingOverlay.IsVisible = true; // show loading indicator
			try
			{
				var token = await _authService.GetValidTokenAsync(); // get user token for role based access
				if (string.IsNullOrEmpty(token))
				{
					await DisplayAlert("Error", "Please log in to access this page.", "OK");
					await Shell.Current.GoToAsync("//LoginPage");
					return;
				}

				var userList = await GetUserListAsync(token); // get items from daatabase
				if (userList != null)
				{
					UpdateItemsList(userList); // for paginated list of items
					UpdateAllItemsList(userList); // for search bar list of items
					GenerateTable();

					OnPropertyChanged(nameof(CurrentPage)); // update current page number
				}
				else
				{
					await DisplayAlert("Error", "Failed to retrieve user list.", "OK");
				}
			}
			catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
			{
				LoadingOverlay.IsVisible = false; // remove loading indicator
				await DisplayAlert("Access Denied", "You do not have permission to view this page.", "OK");
				await Shell.Current.GoToAsync("//DashboardPage"); // Or another appropriate page
			}
			catch (Exception ex)
			{
				LoadingOverlay.IsVisible = false; // remove loading indicator
				await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
			}
			LoadingOverlay.IsVisible = false; // remove loading indicator
		}

		private async Task<JsonArray> GetUserListAsync(string token) // get all items from database
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.GetStringAsync($"{Constants.GET_USER_LIST_ENDPOINT}?all=true");
			var allList = JsonNode.Parse(response)?.AsArray();

			return allList;
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

						column++;
					}
				}

				return grid;
			});
		}

		private async void OnAddUserClicked(object sender, EventArgs e) // add user button clicked
		{
			var popup = new AddUserPopup();

			var result = await this.ShowPopupAsync(popup) as dynamic;

			// Call the method to add a user
			if (result != null)
			{
				await RegisterUserAsync(result.username, result.firstName, result.lastName, result.phone, result.role, result.password);
			}
		}

		private async void OnDeleteUserClicked(object sender, EventArgs e) // delete user button clicked
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

		private async Task RegisterUserAsync(string username, string firstName, string lastName, string phone, string role, string password) // add user
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
				var response = await _httpClient.PostAsJsonAsync($"{Constants.API_BASE_URL}{Constants.REGISTER_ENDPOINT}", newUser);
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

		private async Task DeleteUserAsync(int userId) // delete user
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"{Constants.API_BASE_URL}{Constants.GET_USERS_ENDPOINT}" + $"/{userId}");
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

		private void UpdateItemsList(JsonArray userList) // update the list of paginated items
		{
			_paginateItemList.Clear();
			Items.Clear();

			List<JsonObject> currentBatch = new List<JsonObject>();

			foreach (var item in userList)
			{
				currentBatch.Add(item.AsObject());

				// When we have 10 items, add them as a batch to _itemList
				if (currentBatch.Count == 10)
				{
					_paginateItemList.Add(new ObservableCollection<JsonObject>(currentBatch));
					currentBatch.Clear();
				}
			}

			// If there are any remaining items less than 10, add them as a separate batch
			if (currentBatch.Count > 0)
			{
				_paginateItemList.Add(new ObservableCollection<JsonObject>(currentBatch));
			}

			// Load a page of paginated items
			LoadPage(_currentPage);
		}

		private void LoadPage(int pageIndex) // load a page of paginated items
		{
			int zeroIndex = pageIndex - 1;
			if (zeroIndex < 0 || zeroIndex >= _paginateItemList.Count)
			{
				return; // Invalid page index, do nothing
			}

			Items.Clear();

			foreach (var items in _paginateItemList[zeroIndex])
			{
				Items.Add(items.AsObject()); // add items for display
			}
			_currentPage = pageIndex;
			OnPropertyChanged(nameof(CurrentPage));
		}

		private void UpdateAllItemsList(JsonArray allList) // update the list of all items
		{
			_allItemList.Clear();
			foreach (var item in allList)
			{
				_allItemList.Add(item.AsObject());
			}
		}

		private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e) // search bar, triggers when text inside the search bar changes
		{
			// Get the search text
			string searchText = e.NewTextValue.ToLower();

			// Clear the current Items collection
			Items.Clear();


			if (searchText != "") // if not empty searchbar, load all data for searching
			{
				// Filter the _allItems collection and add matching items to Items
				foreach (var item in _allItemList)
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
			else // if empty searchbar, load the pagination data
			{
				LoadPage(_currentPage);
			}

		}

		private void NextButton_Clicked(object? sender, EventArgs e) // clicked next page
		{
			if (_currentPage < _paginateItemList.Count)
			{
				LoadPage(_currentPage + 1);
			}
		}
		private void PrevButton_Clicked(object? sender, EventArgs e) // clicked previous page
		{
			if (_currentPage > 1)
			{
				LoadPage(_currentPage - 1);
			}
		}

		public void Dispose() //prevent memory leak
		{
			_httpClient?.Dispose();
		}
	}
}
