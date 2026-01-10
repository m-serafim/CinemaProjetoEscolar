/**
 * NEXOR CINEMA - Loading Animations Controller
 * Manages loading states, page transitions, and dynamic animations
 */

(function() {
    'use strict';

    // ========================================
    // PAGE LOADING OVERLAY
    // ========================================
    
    const LoadingOverlay = {
        overlay: null,
        
        init() {
            // Create overlay element
            this.overlay = document.createElement('div');
            this.overlay.className = 'page-loading-overlay';
            this.overlay.innerHTML = '<div class="loading-spinner loading-spinner-lg"></div>';
            document.body.appendChild(this.overlay);
            
            // Hide overlay when page is loaded
            window.addEventListener('load', () => {
                setTimeout(() => this.hide(), 300);
            });
        },
        
        show() {
            if (this.overlay) {
                this.overlay.classList.remove('fade-out');
                this.overlay.style.display = 'flex';
            }
        },
        
        hide() {
            if (this.overlay) {
                this.overlay.classList.add('fade-out');
                setTimeout(() => {
                    this.overlay.style.display = 'none';
                }, 600);
            }
        }
    };

    // ========================================
    // LOADING BAR (Top of Page)
    // ========================================
    
    const LoadingBar = {
        bar: null,
        
        init() {
            this.bar = document.createElement('div');
            this.bar.className = 'loading-bar';
            this.bar.innerHTML = '<div class="loading-bar-progress"></div>';
            this.bar.style.display = 'none';
            document.body.appendChild(this.bar);
        },
        
        show() {
            if (this.bar) {
                this.bar.style.display = 'block';
            }
        },
        
        hide() {
            if (this.bar) {
                setTimeout(() => {
                    this.bar.style.display = 'none';
                }, 400);
            }
        }
    };

    // ========================================
    // INTERSECTION OBSERVER - Scroll Animations
    // ========================================
    
    const ScrollAnimations = {
        observer: null,
        
        init() {
            this.observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('visible');
                        
                        // Add animation class based on data attribute
                        const animClass = entry.target.getAttribute('data-animation');
                        if (animClass) {
                            entry.target.classList.add(animClass);
                        }
                    }
                });
            }, {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            });
            
            // Observe elements with animation classes
            this.observeElements();
        },
        
        observeElements() {
            const elements = document.querySelectorAll('[data-animation]');
            elements.forEach(el => this.observer.observe(el));
        },
        
        refresh() {
            if (this.observer) {
                this.observer.disconnect();
                this.observeElements();
            }
        }
    };

    // ========================================
    // SKELETON LOADER Manager
    // ========================================
    
    const SkeletonLoader = {
        create(type = 'card', count = 1) {
            const container = document.createElement('div');
            container.className = 'skeleton-container';
            
            for (let i = 0; i < count; i++) {
                const skeleton = document.createElement('div');
                
                switch(type) {
                    case 'card':
                        skeleton.className = 'skeleton skeleton-card';
                        break;
                    case 'text':
                        skeleton.className = 'skeleton skeleton-text';
                        break;
                    case 'title':
                        skeleton.className = 'skeleton skeleton-title';
                        break;
                    case 'circle':
                        skeleton.className = 'skeleton skeleton-circle';
                        break;
                    default:
                        skeleton.className = 'skeleton';
                }
                
                container.appendChild(skeleton);
            }
            
            return container;
        },
        
        replace(skeletonElement, actualContent) {
            if (skeletonElement && actualContent) {
                // Fade out skeleton
                skeletonElement.style.opacity = '0';
                skeletonElement.style.transition = 'opacity 0.3s ease';
                
                setTimeout(() => {
                    skeletonElement.replaceWith(actualContent);
                    // Trigger fade in animation
                    actualContent.classList.add('fade-in');
                }, 300);
            }
        }
    };

    // ========================================
    // STAGGER ANIMATION Helper
    // ========================================
    
    const StaggerAnimation = {
        apply(elements, baseDelay = 100) {
            elements.forEach((element, index) => {
                const delay = index * baseDelay;
                element.style.animationDelay = `${delay}ms`;
                element.classList.add('fade-in-up');
            });
        },
        
        applyToRows(selector, baseDelay = 50) {
            const rows = document.querySelectorAll(selector);
            rows.forEach((row, index) => {
                row.className += ' table-row-animated';
                row.style.animationDelay = `${index * baseDelay}ms`;
            });
        },
        
        applyToFormGroups(selector) {
            const groups = document.querySelectorAll(selector);
            groups.forEach((group, index) => {
                group.classList.add('form-group-animated');
                group.style.animationDelay = `${index * 50}ms`;
            });
        }
    };

    // ========================================
    // PAGE TRANSITION Manager
    // ======================================== 
    
    const PageTransition = {
        transitionOut(callback) {
            document.body.style.opacity = '0';
            document.body.style.transition = 'opacity 0.3s ease';
            
            setTimeout(() => {
                if (callback) callback();
            }, 300);
        },
        
        transitionIn() {
            setTimeout(() => {
                document.body.style.opacity = '1';
            }, 50);
        },
        
        applyToLinks() {
            const links = document.querySelectorAll('a:not([target="_blank"]):not([data-no-transition])');
            links.forEach(link => {
                link.addEventListener('click', (e) => {
                    const href = link.getAttribute('href');
                    
                    // Skip if it's just a hash link or javascript:void
                    if (!href || href.startsWith('#') || href.startsWith('javascript:')) {
                        return;
                    }
                    
                    // Skip external links
                    if (link.hostname !== window.location.hostname) {
                        return;
                    }
                    
                    e.preventDefault();
                    LoadingBar.show();
                    
                    this.transitionOut(() => {
                        window.location.href = href;
                    });
                });
            });
        }
    };

    // ========================================
    // BUTTON LOADING STATE
    // ========================================
    
    const ButtonLoading = {
        set(button, loading = true) {
            if (loading) {
                button.disabled = true;
                button.dataset.originalText = button.innerHTML;
                button.innerHTML = '<div class="loading-dots"><div class="loading-dot"></div><div class="loading-dot"></div><div class="loading-dot"></div></div>';
                button.classList.add('btn-loading');
            } else {
                button.disabled = false;
                if (button.dataset.originalText) {
                    button.innerHTML = button.dataset.originalText;
                    delete button.dataset.originalText;
                }
                button.classList.remove('btn-loading');
            }
        },
        
        success(button, message = 'Success!', duration = 2000) {
            button.innerHTML = `<i class="bi bi-check-circle-fill"></i> ${message}`;
            button.classList.add('btn-success');
            
            setTimeout(() => {
                if (button.dataset.originalText) {
                    button.innerHTML = button.dataset.originalText;
                }
                button.classList.remove('btn-success');
            }, duration);
        },
        
        error(button, message = 'Error!', duration = 2000) {
            button.innerHTML = `<i class="bi bi-x-circle-fill"></i> ${message}`;
            button.classList.add('btn-danger', 'shake');
            
            setTimeout(() => {
                if (button.dataset.originalText) {
                    button.innerHTML = button.dataset.originalText;
                }
                button.classList.remove('btn-danger', 'shake');
            }, duration);
        }
    };

    // ========================================
    // RIPPLE EFFECT for Buttons
    // ========================================
    
    const RippleEffect = {
        init() {
            document.addEventListener('click', (e) => {
                const button = e.target.closest('.btn, .btn-modern-primary, .btn-modern-secondary');
                if (button && !button.classList.contains('btn-ripple')) {
                    button.classList.add('btn-ripple');
                }
            });
        }
    };

    // ========================================
    // SMOOTH SCROLL to Anchor Links
    // ========================================
    
    const SmoothScroll = {
        init() {
            document.querySelectorAll('a[href^="#"]').forEach(anchor => {
                anchor.addEventListener('click', function(e) {
                    const href = this.getAttribute('href');
                    if (href === '#' || href === '#!') return;
                    
                    const target = document.querySelector(href);
                    if (target) {
                        e.preventDefault();
                        target.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                });
            });
        }
    };

    // ========================================
    // ALERT ANIMATIONS
    // ========================================
    
    const AlertAnimations = {
        init() {
            const alerts = document.querySelectorAll('.alert');
            alerts.forEach(alert => {
                alert.classList.add('alert-animated');
            });
        },
        
        dismiss(alertElement, duration = 300) {
            alertElement.style.opacity = '0';
            alertElement.style.transform = 'translateY(-20px)';
            alertElement.style.transition = `all ${duration}ms ease`;
            
            setTimeout(() => {
                alertElement.remove();
            }, duration);
        }
    };

    // ========================================
    // INITIALIZATION
    // ========================================
    
    function init() {
        console.log('Nexor Cinema - Loading Animations System Initialized');
        
        // Initialize components only if DOM is ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initializeComponents);
        } else {
            initializeComponents();
        }
    }
    
    function initializeComponents() {
        LoadingOverlay.init();
        LoadingBar.init();
        ScrollAnimations.init();
        RippleEffect.init();
        SmoothScroll.init();
        AlertAnimations.init();
        
        // Apply page transition
        PageTransition.transitionIn();
        
        // Apply stagger animations to common elements
        applyCommonAnimations();
        
        console.log('All animation components loaded');
    }
    
    function applyCommonAnimations() {
        // Animate table rows if present
        const tableRows = document.querySelectorAll('table tbody tr:not(.table-row-animated)');
        if (tableRows.length > 0) {
            StaggerAnimation.applyToRows('table tbody tr');
        }
        
        // Animate form groups if present
        const formGroups = document.querySelectorAll('.mb-3:not(.form-group-animated), .form-group:not(.form-group-animated)');
        if (formGroups.length > 0) {
            formGroups.forEach((group, index) => {
                group.classList.add('form-group-animated');
                group.style.animationDelay = `${index * 50}ms`;
            });
        }
        
        // Animate cards
        const cards = document.querySelectorAll('.card:not(.movie-card):not(.feature-card)');
        cards.forEach((card, index) => {
            card.classList.add('fade-in-up');
            card.style.animationDelay = `${index * 100}ms`;
        });
    }

    // ========================================
    // EXPORT PUBLIC API
    // ========================================
    
    window.NexorAnimations = {
        LoadingOverlay,
        LoadingBar,
        SkeletonLoader,
        StaggerAnimation,
        PageTransition,
        ButtonLoading,
        ScrollAnimations,
        init: initializeComponents
    };

    // Auto-initialize
    init();

})();
