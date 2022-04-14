using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APIWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Book? CurrentBook { get; private set; }

        private List<Book> _books = new();


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void AuthorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var query = HttpUtility.ParseQueryString(string.Empty);
            //if (!string.IsNullOrWhiteSpace(AuthorTextbox.Text))
            //{
            //    query["Author"] = AuthorTextbox.Text;
            //}
            //var builder = new UriBuilder("https://localhost:7295/api/Books");
            //builder.Query = query.ToString();
            FilterBooksByAuthor();
        }

        private void FilterBooksByAuthor()
        {
            var finalBooks = new List<Book>();
            if (string.IsNullOrWhiteSpace(AuthorTextbox.Text))
            {
                finalBooks = _books;
            }
            else
            {
                finalBooks = _books.Where(x => x.Author.Contains(AuthorTextbox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            BooksListview.ItemsSource = finalBooks;
            if (CurrentBook != null && finalBooks.Contains(CurrentBook))
            {
                BooksListview.SelectedItem = CurrentBook;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoaderBooksAsync();
        }

        private async Task LoaderBooksAsync()
        {
            await LoadAsync(async () =>
            {
                await LoadBooksAsync();
            });
        }

        private async Task LoadBooksAsync()
        {
            var response = await App.httpClient.GetAsync("https://localhost:7295/api/Books");
            if (response.IsSuccessStatusCode)
            {
                _books = await response.Content.ReadFromJsonAsync<List<Book>>(new JsonSerializerOptions(JsonSerializerDefaults.Web));
                Dispatcher.Invoke(() => FilterBooksByAuthor());
            }
            else
                MessageBox.Show("Соси хуй");
        }

        private async Task LoadAsync(Func<Task> asyncFunc)
        {
            Dispatcher.Invoke(() => AllContentGrid.Visibility = Visibility.Hidden);
            Dispatcher.Invoke(() => { LoadingMediaElement.Position = TimeSpan.FromTicks(1); LoadingMediaElement.Play(); LoadingMediaElement.Visibility = Visibility.Visible; });
            await Task.Delay(1000);
            await asyncFunc();
            Dispatcher.Invoke(() => AllContentGrid.Visibility = Visibility.Visible);
            Dispatcher.Invoke(() => { LoadingMediaElement.Stop(); LoadingMediaElement.Visibility = Visibility.Hidden; });
        }

        private async void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBook is null)
            {
                await AddAsync();
            }
            else
            {
                await UpdateAsync();
            }
        }

        private async Task UpdateAsync()
        {
            if (!string.IsNullOrEmpty(NewAuthorTextbox.Text) && !string.IsNullOrEmpty(NewNameTextbox.Text) && int.TryParse(NewCountOfPagesTextbox.Text, out var countOfPages))
            {
                await LoadAsync(async () =>
                {
                    var book = CurrentBook;
                    book.Author = NewAuthorTextbox.Text;
                    book.Name = NewNameTextbox.Text;
                    book.CountOfPages = countOfPages;
                    var client = App.httpClient;
                    var putResponse = await client.PutAsJsonAsync($"https://localhost:7295/api/Books/{book.Id}", book);
                    if (putResponse.IsSuccessStatusCode)
                    {
                        IsEnabled = false;
                        var response = await client.GetAsync("https://localhost:7295/api/Books");
                        if (response.IsSuccessStatusCode)
                        {
                            _books = await response.Content.ReadFromJsonAsync<List<Book>>(new JsonSerializerOptions(JsonSerializerDefaults.Web));
                            FilterBooksByAuthor();
                            BooksListview.SelectedItem = _books.FirstOrDefault(x => x.Id == book.Id);
                        }
                        else
                            MessageBox.Show("Соси хуй");
                        IsEnabled = true;
                    }
                    else
                        MessageBox.Show("Что-то пошло по пизде");
                });
            }
            else MessageBox.Show("Поздравляю, Вы долбаеб!");
        }

        private async Task AddAsync()
        {
            if (!string.IsNullOrEmpty(NewAuthorTextbox.Text) && !string.IsNullOrEmpty(NewNameTextbox.Text) && int.TryParse(NewCountOfPagesTextbox.Text, out var countOfPages))
            {
                await LoadAsync(async () =>
                {
                    Book newBook = new Book
                    {
                        Name = NewNameTextbox.Text,
                        CountOfPages = int.Parse(NewCountOfPagesTextbox.Text),
                        Author = NewAuthorTextbox.Text
                    };
                    var content = JsonContent.Create(newBook);
                    var client = App.httpClient;
                    var postResponse = await client.PostAsync("https://localhost:7295/api/Books", content);
                    if (postResponse.IsSuccessStatusCode)
                    {
                        await LoadBooksAsync();
                    }
                    else
                        MessageBox.Show("Что-то пошло по пизде");
                });
            }
            else MessageBox.Show("Поздравляю, Вы долбаеб!");
        }

        private async void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            await DeleteAsync();
        }

        private async Task DeleteAsync()
        {
            if (BooksListview.SelectedItems.Count > 0)
            {
                await LoadAsync(async () =>
                {
                    var element = (Book)BooksListview.SelectedItem;
                    var client = App.httpClient;
                    var deleteResponse = await client.DeleteAsync($"https://localhost:7295/api/Books/{element.Id}");
                    if (deleteResponse.IsSuccessStatusCode)
                    {
                        await LoadBooksAsync();
                    }
                    else
                        MessageBox.Show("ОШИБКА!");
                });
            }
            else
                MessageBox.Show("Выдели объект(ы) мудак!");
        }

        private void BooksListview_SelectionChanged(object sender, EventArgs e)
        {
            var book = BooksListview.SelectedItem as Book;
            if (book?.Equals(CurrentBook) ?? false)
            {
                NewAuthorTextbox.Text = string.Empty;
                NewNameTextbox.Text = string.Empty;
                NewCountOfPagesTextbox.Text = string.Empty;
                CurrentBook = null;
                BooksListview.SelectedItems.Clear();
            }
            else if (book != null)
            {
                NewAuthorTextbox.Text = book.Author;
                NewNameTextbox.Text = book.Name;
                NewCountOfPagesTextbox.Text = book.CountOfPages.ToString();
                CurrentBook = book;
            }
        }

        private void Unselect_Click(object sender, RoutedEventArgs e)
        {
            BooksListview.SelectedItems.Clear();
            NewAuthorTextbox.Text = string.Empty;
            NewNameTextbox.Text = string.Empty;
            NewCountOfPagesTextbox.Text = string.Empty;
            CurrentBook = null;
        }
    }
}
