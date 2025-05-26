using ODEliteTracker.Database;
using ODEliteTracker.Models.Bookmarks;
using ODEliteTracker.ViewModels.ModelViews.Bookmarks;
using ODJournalDatabase.Database.Interfaces;

namespace ODEliteTracker.Stores
{
    public sealed class BookmarkDataStore
    {
        public BookmarkDataStore(IODDatabaseProvider databaseProvider)
        {
             this.databaseProvider = (ODEliteTrackerDatabaseProvider)databaseProvider;

            _initializeLazy = new Lazy<Task>(Initialise);

            _ = Load();
        }
        private readonly ODEliteTrackerDatabaseProvider databaseProvider;
        private Lazy<Task> _initializeLazy;

        public List<SystemBookmark> Bookmarks { get; private set; } = [];

        public event EventHandler<bool>? StoreLive;
        public event EventHandler? BookmarksUpdated;

        private bool isLive = false;
        public bool IsLive 
        {
            get => isLive;
            private set
            {
                if (isLive != value)
                {
                    isLive = value;
                    StoreLive?.Invoke(this, value);
                }
            }
        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialise);
                throw;
            }
        }

        private async Task Initialise()
        {
            await UpdateBookmarks();

            IsLive = true;
        }

        private async Task UpdateBookmarks()
        {
            Bookmarks.Clear();
            var bookmarks = await databaseProvider.GetAllBookmarks();

            if (bookmarks.Count > 0)
            {
                Bookmarks = bookmarks;
            }
        }

        public async Task<int> SaveBookMark(SystemBookmarkVM system, BookmarkVM bookmark)
        {
            var ret = await databaseProvider.AddBookmark(system, bookmark);

            await UpdateBookmarks();

            if (IsLive)
            {
                BookmarksUpdated?.Invoke(this, EventArgs.Empty);
            }
            return ret;
        }

        public async Task DeleteBookmark(long systemAddress, int bookmarkId)
        {
            await databaseProvider.DeleteBookmark(systemAddress, bookmarkId);

            await UpdateBookmarks();

            if (IsLive)
            {
                BookmarksUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
