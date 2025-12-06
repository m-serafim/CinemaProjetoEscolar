// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Live Search Functionality
(function() {
    const searchInput = document.getElementById('liveSearchInput');
    const searchResults = document.getElementById('searchResults');
    const clearBtn = document.getElementById('clearSearchBtn');
    let searchTimeout;

    if (searchInput && searchResults) {
        // Debounce search input
        searchInput.addEventListener('input', function(e) {
            clearTimeout(searchTimeout);
            const query = e.target.value.trim();

            // Show/hide clear button
            if (clearBtn) {
                clearBtn.style.display = query.length > 0 ? 'block' : 'none';
            }

            if (query.length < 2) {
                searchResults.style.display = 'none';
                return;
            }

            searchTimeout = setTimeout(() => {
                performSearch(query);
            }, 300);
        });

        // Clear button click handler
        if (clearBtn) {
            clearBtn.addEventListener('click', function() {
                searchInput.value = '';
                searchResults.style.display = 'none';
                clearBtn.style.display = 'none';
                searchInput.focus();
            });
        }

        // Close search results when clicking outside
        document.addEventListener('click', function(e) {
            if (!searchInput.contains(e.target) && !searchResults.contains(e.target) && (!clearBtn || !clearBtn.contains(e.target))) {
                searchResults.style.display = 'none';
            }
        });

        // Show search results when focusing input with existing value
        searchInput.addEventListener('focus', function(e) {
            if (e.target.value.trim().length >= 2 && searchResults.innerHTML) {
                searchResults.style.display = 'block';
            }
        });

        // Keyboard navigation for search results
        searchInput.addEventListener('keydown', function(e) {
            const items = searchResults.querySelectorAll('.search-result-item');
            if (items.length === 0) return;

            let currentIndex = -1;
            items.forEach((item, index) => {
                if (item.classList.contains('keyboard-focused')) {
                    currentIndex = index;
                }
            });

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                items.forEach(item => item.classList.remove('keyboard-focused'));
                const nextIndex = currentIndex < items.length - 1 ? currentIndex + 1 : 0;
                items[nextIndex].classList.add('keyboard-focused');
                items[nextIndex].scrollIntoView({ block: 'nearest' });
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                items.forEach(item => item.classList.remove('keyboard-focused'));
                const prevIndex = currentIndex > 0 ? currentIndex - 1 : items.length - 1;
                items[prevIndex].classList.add('keyboard-focused');
                items[prevIndex].scrollIntoView({ block: 'nearest' });
            } else if (e.key === 'Enter' && currentIndex >= 0) {
                e.preventDefault();
                items[currentIndex].click();
            } else if (e.key === 'Escape') {
                searchResults.style.display = 'none';
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
            const thumbnail = escapeHtml(movie.thumbnail || '/images/movies/placeholder.svg');
            html += `
                <a href="/Catalogo/Details/${movie.id}" class="search-result-item">
                    <img src="${thumbnail}" alt="${escapeHtml(movie.title)}" class="search-result-thumbnail" onerror="this.src='/images/movies/placeholder.svg'">
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

// Rotating Movie Display - NO ANIMATIONS (instant switching)
(function() {
    const rotatingContainer = document.getElementById('rotatingMovieDisplay');
    if (!rotatingContainer) {
        console.log('Rotating container not found!');
        return;
    }

    console.log('🎬 Initializing movie rotation (instant switching - no animations)...');
    let movies = [];
    let currentIndex = 0;
    let rotationInterval;

    // Fetch movies
    fetch('/Home/GetRandomMovies')
        .then(response => {
            console.log('📡 Fetch response:', response);
            return response.json();
        })
        .then(data => {
            console.log('🎥 Movies data:', data);
            movies = data.movies;
            if (movies && movies.length > 0) {
                console.log(`✅ Found ${movies.length} movies, initializing slideshow...`);
                initializeSlideshow();
                startRotation();
            } else {
                console.log('⚠️ No movies found');
                rotatingContainer.innerHTML = '<div class="no-movies-message">No movies available</div>';
            }
        })
        .catch(error => {
            console.error('❌ Error fetching movies:', error);
            rotatingContainer.innerHTML = '<div class="no-movies-message">Unable to load movies</div>';
        });

    function initializeSlideshow() {
        if (!movies || movies.length === 0) {
            console.log('⚠️ No movies to initialize');
            return;
        }

        console.log('🎨 Creating movie slides...');
        
        // Create all movie slides
        rotatingContainer.innerHTML = '';
        movies.forEach((movie, index) => {
            const thumbnail = escapeHtml(movie.thumbnail || '/images/movies/placeholder.svg');
            const slideDiv = document.createElement('div');
            slideDiv.className = `rotating-movie-slide${index === 0 ? ' active' : ''}`;
            slideDiv.innerHTML = `
                <div class="rotating-movie-card">
                    <a href="/Catalogo/Details/${movie.id}">
                        <img src="${thumbnail}" alt="${escapeHtml(movie.title)}" class="rotating-movie-image" onerror="this.src='/images/movies/placeholder.svg'" loading="lazy">
                        <div class="rotating-movie-title">${escapeHtml(movie.title)}</div>
                    </a>
                </div>
            `;
            rotatingContainer.appendChild(slideDiv);
            console.log(`✔️ Created slide ${index}: "${movie.title}"`);
        });
        console.log('🎉 All slides created!');
    }

    function startRotation() {
        if (movies.length <= 1) {
            console.log('⚠️ Only one or no movies, skipping rotation');
            return;
        }
        
        console.log(`🔄 Starting instant rotation with ${movies.length} movies`);
        
        rotationInterval = setInterval(() => {
            const slides = rotatingContainer.querySelectorAll('.rotating-movie-slide');
            const currentSlide = slides[currentIndex];
            const nextIndex = (currentIndex + 1) % movies.length;
            const nextSlide = slides[nextIndex];

            console.log(`➡️ Switching from "${movies[currentIndex].title}" to "${movies[nextIndex].title}"`);

            // Instant switch - no animation
            currentSlide.classList.remove('active');
            nextSlide.classList.add('active');
            
            currentIndex = nextIndex;
            console.log(`✅ Now showing: ${movies[nextIndex].title}`);
        }, 4500); // 4.5 seconds per movie
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    console.log('✅ Movie rotation ready - instant switching enabled');
})();

// Fade-in sections on scroll
(function() {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });

    document.querySelectorAll('.fade-in-section').forEach(section => {
        observer.observe(section);
    });
})();
