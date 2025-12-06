// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Live Search Functionality
(function() {
    const searchInput = document.getElementById('liveSearchInput');
    const searchResults = document.getElementById('searchResults');
    let searchTimeout;

    if (searchInput && searchResults) {
        // Debounce search input
        searchInput.addEventListener('input', function(e) {
            clearTimeout(searchTimeout);
            const query = e.target.value.trim();

            if (query.length < 2) {
                searchResults.style.display = 'none';
                return;
            }

            searchTimeout = setTimeout(() => {
                performSearch(query);
            }, 300);
        });

        // Close search results when clicking outside
        document.addEventListener('click', function(e) {
            if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
                searchResults.style.display = 'none';
            }
        });

        // Show search results when focusing input with existing value
        searchInput.addEventListener('focus', function(e) {
            if (e.target.value.trim().length >= 2 && searchResults.innerHTML) {
                searchResults.style.display = 'block';
            }
        });
    }

    function performSearch(query) {
        // Show loading state
        searchResults.innerHTML = '<div class="search-loading"><div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Loading...</span></div> Searching...</div>';
        searchResults.style.display = 'block';

        fetch(`/Home/SearchMovies?query=${encodeURIComponent(query)}`)
            .then(response => response.json())
            .then(data => {
                displaySearchResults(data.results);
            })
            .catch(error => {
                console.error('Search error:', error);
                searchResults.innerHTML = '<div class="search-no-results">Error performing search</div>';
                searchResults.style.display = 'block';
            });
    }

    function displaySearchResults(results) {
        if (!results || results.length === 0) {
            searchResults.innerHTML = '<div class="search-no-results">No movies found</div>';
            searchResults.style.display = 'block';
            return;
        }

        let html = '';
        results.forEach(movie => {
            const thumbnail = movie.thumbnail || '/images/movies/placeholder.svg';
            html += `
                <a href="/Catalogo/Details/${movie.id}" class="search-result-item">
                    <img src="${thumbnail}" alt="${movie.title}" class="search-result-thumbnail" onerror="this.src='/images/movies/placeholder.svg'">
                    <div class="search-result-info">
                        <div class="search-result-title">${escapeHtml(movie.title)}</div>
                        <div class="search-result-genre">${escapeHtml(movie.genre)}</div>
                    </div>
                </a>
            `;
        });

        searchResults.innerHTML = html;
        searchResults.style.display = 'block';
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
})();

// Rotating Movie Display
(function() {
    const rotatingContainer = document.getElementById('rotatingMovieDisplay');
    if (!rotatingContainer) return;

    let movies = [];
    let currentIndex = 0;
    let rotationInterval;

    // Fetch movies
    fetch('/Home/GetRandomMovies')
        .then(response => response.json())
        .then(data => {
            movies = data.movies;
            if (movies && movies.length > 0) {
                displayMovie(0);
                startRotation();
            } else {
                rotatingContainer.innerHTML = '<div class="no-movies-message">No movies available</div>';
            }
        })
        .catch(error => {
            console.error('Error fetching movies:', error);
            rotatingContainer.innerHTML = '<div class="no-movies-message">Unable to load movies</div>';
        });

    function displayMovie(index) {
        if (!movies || movies.length === 0) return;

        const movie = movies[index];
        const thumbnail = movie.thumbnail || '/images/movies/placeholder.svg';

        rotatingContainer.innerHTML = `
            <div class="rotating-movie-card fade-in">
                <a href="/Catalogo/Details/${movie.id}">
                    <img src="${thumbnail}" alt="${escapeHtml(movie.title)}" class="rotating-movie-image" onerror="this.src='/images/movies/placeholder.svg'">
                    <div class="rotating-movie-title">${escapeHtml(movie.title)}</div>
                </a>
            </div>
        `;
    }

    function startRotation() {
        rotationInterval = setInterval(() => {
            // Fade out
            rotatingContainer.classList.add('fade-out');

            setTimeout(() => {
                currentIndex = (currentIndex + 1) % movies.length;
                displayMovie(currentIndex);
                rotatingContainer.classList.remove('fade-out');
            }, 500);
        }, 5000);
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
})();
