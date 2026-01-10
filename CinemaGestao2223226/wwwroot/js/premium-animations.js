/**
 * NEXOR CINEMA - Premium Animation System
 * Ultra-smooth, cinematic animations controller
 * Version 2.0
 */

(function() {
    'use strict';

    // ========================================
    // CONFIGURATION
    // ========================================
    
    const CONFIG = {
        // Animation timings
        durations: {
            instant: 100,
            fast: 200,
            normal: 400,
            slow: 600,
            slower: 800,
            slowest: 1200
        },
        // Intersection Observer thresholds
        observerThreshold: 0.15,
        observerRootMargin: '0px 0px -50px 0px',
        // Stagger delay between elements
        staggerDelay: 80,
        // Debounce delay
        debounceDelay: 16
    };

    // ========================================
    // UTILITY FUNCTIONS
    // ========================================
    
    const Utils = {
        // Debounce function for performance
        debounce(func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        },

        // Throttle function for scroll events
        throttle(func, limit) {
            let inThrottle;
            return function(...args) {
                if (!inThrottle) {
                    func.apply(this, args);
                    inThrottle = true;
                    setTimeout(() => inThrottle = false, limit);
                }
            };
        },

        // Check if element is in viewport
        isInViewport(element, offset = 0) {
            const rect = element.getBoundingClientRect();
            return (
                rect.top <= (window.innerHeight || document.documentElement.clientHeight) - offset &&
                rect.bottom >= 0
            );
        },

        // Add animation class with cleanup
        animateElement(element, animationClass, duration = CONFIG.durations.normal) {
            return new Promise(resolve => {
                element.classList.add(animationClass);
                setTimeout(() => {
                    element.classList.remove(animationClass);
                    resolve();
                }, duration);
            });
        },

        // Create ripple effect
        createRipple(event, element) {
            const ripple = document.createElement('span');
            ripple.className = 'ripple';
            
            const rect = element.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = event.clientX - rect.left - size / 2;
            const y = event.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = `${size}px`;
            ripple.style.left = `${x}px`;
            ripple.style.top = `${y}px`;
            
            element.appendChild(ripple);
            
            setTimeout(() => ripple.remove(), 600);
        },

        // Lerp function for smooth animations
        lerp(start, end, factor) {
            return start + (end - start) * factor;
        },

        // Clamp function
        clamp(value, min, max) {
            return Math.min(Math.max(value, min), max);
        },

        // RAF wrapper
        raf(callback) {
            return window.requestAnimationFrame(callback);
        }
    };

    // ========================================
    // PAGE LOADER
    // ========================================
    
    const PageLoader = {
        loader: null,
        progressBar: null,
        isLoaded: false,

        init() {
            this.createLoader();
            this.createProgressBar();
            this.bindEvents();
        },

        createLoader() {
            this.loader = document.createElement('div');
            this.loader.className = 'cinema-page-loader';
            this.loader.innerHTML = `
                <div class="film-reel-loader">
                    <div class="film-reel"></div>
                </div>
                <span class="loader-text">Loading</span>
            `;
            document.body.prepend(this.loader);
        },

        createProgressBar() {
            this.progressBar = document.createElement('div');
            this.progressBar.className = 'progress-loader';
            this.progressBar.innerHTML = '<div class="progress-loader-bar"></div>';
            document.body.prepend(this.progressBar);
        },

        bindEvents() {
            // Hide loader when page is fully loaded
            window.addEventListener('load', () => {
                setTimeout(() => this.hide(), 300);
            });

            // Fallback timeout
            setTimeout(() => {
                if (!this.isLoaded) this.hide();
            }, 3000);
        },

        show() {
            if (this.loader) {
                this.loader.classList.remove('loaded');
            }
            if (this.progressBar) {
                this.progressBar.style.display = 'block';
            }
        },

        hide() {
            this.isLoaded = true;
            
            if (this.loader) {
                this.loader.classList.add('loaded');
                setTimeout(() => {
                    if (this.loader && this.loader.parentNode) {
                        this.loader.remove();
                    }
                }, 800);
            }
            
            if (this.progressBar) {
                setTimeout(() => {
                    if (this.progressBar && this.progressBar.parentNode) {
                        this.progressBar.remove();
                    }
                }, 500);
            }

            // Trigger page entrance animations
            ScrollAnimations.revealAll();
        }
    };

    // ========================================
    // SCROLL ANIMATIONS
    // ========================================
    
    const ScrollAnimations = {
        observer: null,
        animatedElements: new Set(),

        init() {
            this.setupObserver();
            this.observeElements();
            this.setupParallax();
        },

        setupObserver() {
            this.observer = new IntersectionObserver(
                (entries) => {
                    entries.forEach(entry => {
                        if (entry.isIntersecting && !this.animatedElements.has(entry.target)) {
                            this.animateElement(entry.target);
                            this.animatedElements.add(entry.target);
                        }
                    });
                },
                {
                    threshold: CONFIG.observerThreshold,
                    rootMargin: CONFIG.observerRootMargin
                }
            );
        },

        observeElements() {
            // Elements with data-animate attribute
            document.querySelectorAll('[data-animate]').forEach(el => {
                this.observer.observe(el);
            });

            // Auto-observe common elements
            const autoAnimateSelectors = [
                '.card:not(.no-animate)',
                '.movie-poster-card',
                '.feature-card',
                '.quick-access-card',
                '.info-card',
                'table tbody tr',
                '.form-group, .mb-3',
                '.section-header',
                '.hero-content > *'
            ];

            autoAnimateSelectors.forEach(selector => {
                document.querySelectorAll(selector).forEach((el, index) => {
                    if (!el.hasAttribute('data-animate')) {
                        el.setAttribute('data-animate', 'fade-up');
                        el.style.animationDelay = `${index * CONFIG.staggerDelay}ms`;
                    }
                    this.observer.observe(el);
                });
            });
        },

        animateElement(element) {
            const animationType = element.getAttribute('data-animate') || 'fade-up';
            element.classList.add('is-visible');
            
            // Add specific animation class
            element.classList.add(`animate-${animationType.replace('fade-', 'slide-')}`);
        },

        revealAll() {
            // Reveal all visible elements on page load
            document.querySelectorAll('[data-animate]').forEach(el => {
                if (Utils.isInViewport(el, 50)) {
                    setTimeout(() => {
                        this.animateElement(el);
                        this.animatedElements.add(el);
                    }, 100);
                }
            });
        },

        setupParallax() {
            const parallaxElements = document.querySelectorAll('[data-parallax]');
            if (parallaxElements.length === 0) return;

            let ticking = false;

            window.addEventListener('scroll', () => {
                if (!ticking) {
                    Utils.raf(() => {
                        parallaxElements.forEach(el => {
                            const speed = parseFloat(el.getAttribute('data-parallax')) || 0.5;
                            const yPos = -(window.scrollY * speed);
                            el.style.transform = `translate3d(0, ${yPos}px, 0)`;
                        });
                        ticking = false;
                    });
                    ticking = true;
                }
            });
        },

        refresh() {
            this.animatedElements.clear();
            if (this.observer) {
                this.observer.disconnect();
            }
            this.setupObserver();
            this.observeElements();
        }
    };

    // ========================================
    // HOVER ANIMATIONS
    // ========================================
    
    const HoverAnimations = {
        init() {
            this.setupMagneticButtons();
            this.setupTiltCards();
            this.setupRippleEffect();
            this.setup3DCards();
        },

        setupMagneticButtons() {
            document.querySelectorAll('.btn-magnetic, .hover-magnetic').forEach(btn => {
                btn.addEventListener('mousemove', (e) => {
                    const rect = btn.getBoundingClientRect();
                    const x = e.clientX - rect.left - rect.width / 2;
                    const y = e.clientY - rect.top - rect.height / 2;
                    
                    btn.style.transform = `translate(${x * 0.3}px, ${y * 0.3}px)`;
                });

                btn.addEventListener('mouseleave', () => {
                    btn.style.transform = 'translate(0, 0)';
                });
            });
        },

        setupTiltCards() {
            document.querySelectorAll('.hover-tilt-3d, .card-tilt').forEach(card => {
                card.addEventListener('mousemove', (e) => {
                    const rect = card.getBoundingClientRect();
                    const x = e.clientX - rect.left;
                    const y = e.clientY - rect.top;
                    
                    const centerX = rect.width / 2;
                    const centerY = rect.height / 2;
                    
                    const rotateX = (y - centerY) / 20;
                    const rotateY = (centerX - x) / 20;
                    
                    card.style.transform = `
                        perspective(1000px) 
                        rotateX(${rotateX}deg) 
                        rotateY(${rotateY}deg) 
                        scale3d(1.02, 1.02, 1.02)
                    `;
                });

                card.addEventListener('mouseleave', () => {
                    card.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) scale3d(1, 1, 1)';
                });
            });
        },

        setupRippleEffect() {
            document.addEventListener('click', (e) => {
                const target = e.target.closest('.btn, .btn-ripple-effect, button[type="submit"]');
                if (target && !target.classList.contains('no-ripple')) {
                    Utils.createRipple(e, target);
                }
            });
        },

        setup3DCards() {
            document.querySelectorAll('.card-3d, .movie-poster-card').forEach(card => {
                card.addEventListener('mouseenter', function() {
                    this.style.transition = 'transform 0.1s ease-out';
                });

                card.addEventListener('mouseleave', function() {
                    this.style.transition = 'transform 0.4s cubic-bezier(0.4, 0, 0.2, 1)';
                    this.style.transform = '';
                });
            });
        }
    };

    // ========================================
    // SMOOTH SCROLL
    // ========================================
    
    const SmoothScroll = {
        init() {
            this.setupAnchorLinks();
            this.setupScrollToTop();
        },

        setupAnchorLinks() {
            document.querySelectorAll('a[href^="#"]').forEach(anchor => {
                anchor.addEventListener('click', (e) => {
                    const href = anchor.getAttribute('href');
                    if (href === '#' || href === '#!') return;

                    const target = document.querySelector(href);
                    if (target) {
                        e.preventDefault();
                        this.scrollTo(target);
                    }
                });
            });
        },

        scrollTo(element, offset = 100) {
            const top = element.getBoundingClientRect().top + window.scrollY - offset;
            window.scrollTo({
                top,
                behavior: 'smooth'
            });
        },

        setupScrollToTop() {
            const scrollBtn = document.querySelector('.scroll-to-top');
            if (!scrollBtn) return;

            window.addEventListener('scroll', Utils.throttle(() => {
                if (window.scrollY > 500) {
                    scrollBtn.classList.add('visible');
                } else {
                    scrollBtn.classList.remove('visible');
                }
            }, 100));

            scrollBtn.addEventListener('click', () => {
                window.scrollTo({ top: 0, behavior: 'smooth' });
            });
        }
    };

    // ========================================
    // PAGE TRANSITIONS
    // ========================================
    
    const PageTransitions = {
        init() {
            this.setupLinkTransitions();
        },

        setupLinkTransitions() {
            document.querySelectorAll('a:not([target="_blank"]):not([data-no-transition]):not([href^="#"]):not([href^="javascript"])').forEach(link => {
                link.addEventListener('click', (e) => {
                    const href = link.getAttribute('href');
                    if (!href || link.hostname !== window.location.hostname) return;

                    e.preventDefault();
                    this.transitionTo(href);
                });
            });
        },

        transitionTo(url) {
            // Add exit animation
            document.body.classList.add('page-transitioning');
            
            const mainContent = document.querySelector('main') || document.body;
            mainContent.style.opacity = '0';
            mainContent.style.transform = 'translateY(20px)';
            mainContent.style.transition = 'opacity 0.3s ease, transform 0.3s ease';

            setTimeout(() => {
                window.location.href = url;
            }, 300);
        }
    };

    // ========================================
    // ALERT ANIMATIONS
    // ========================================
    
    const AlertAnimations = {
        init() {
            this.animateExistingAlerts();
            this.setupDismissAnimations();
        },

        animateExistingAlerts() {
            document.querySelectorAll('.alert').forEach((alert, index) => {
                alert.classList.add('alert-premium');
                alert.style.animationDelay = `${index * 100}ms`;
            });
        },

        setupDismissAnimations() {
            document.querySelectorAll('.alert .btn-close').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.preventDefault();
                    const alert = btn.closest('.alert');
                    if (alert) {
                        alert.classList.add('alert-dismiss');
                        setTimeout(() => alert.remove(), 400);
                    }
                });
            });
        },

        show(message, type = 'info', duration = 5000) {
            const alertContainer = document.querySelector('.alert-container') || this.createAlertContainer();
            
            const alert = document.createElement('div');
            alert.className = `alert alert-${type} alert-dismissible alert-premium toast-animated`;
            alert.innerHTML = `
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            `;
            
            alertContainer.appendChild(alert);

            // Setup dismiss
            const dismissBtn = alert.querySelector('.btn-close');
            dismissBtn.addEventListener('click', () => {
                alert.classList.add('alert-dismiss');
                setTimeout(() => alert.remove(), 400);
            });

            // Auto dismiss
            if (duration > 0) {
                setTimeout(() => {
                    if (alert.parentNode) {
                        alert.classList.add('alert-dismiss');
                        setTimeout(() => alert.remove(), 400);
                    }
                }, duration);
            }

            return alert;
        },

        createAlertContainer() {
            const container = document.createElement('div');
            container.className = 'alert-container';
            container.style.cssText = 'position: fixed; top: 100px; right: 20px; z-index: 9999; max-width: 400px;';
            document.body.appendChild(container);
            return container;
        }
    };

    // ========================================
    // FORM ANIMATIONS
    // ========================================
    
    const FormAnimations = {
        init() {
            this.setupInputAnimations();
            this.setupValidationAnimations();
            this.setupFormGroupStagger();
        },

        setupInputAnimations() {
            document.querySelectorAll('.form-control, .form-select').forEach(input => {
                input.classList.add('input-animated');

                input.addEventListener('focus', () => {
                    input.parentElement?.classList.add('input-focused');
                });

                input.addEventListener('blur', () => {
                    input.parentElement?.classList.remove('input-focused');
                });
            });
        },

        setupValidationAnimations() {
            document.querySelectorAll('form').forEach(form => {
                form.addEventListener('submit', (e) => {
                    const invalidInputs = form.querySelectorAll(':invalid');
                    invalidInputs.forEach(input => {
                        input.classList.add('input-invalid');
                        setTimeout(() => input.classList.remove('input-invalid'), 500);
                    });
                });
            });
        },

        setupFormGroupStagger() {
            document.querySelectorAll('form .mb-3, form .form-group').forEach((group, index) => {
                group.style.animationDelay = `${index * 50}ms`;
                group.classList.add('animate-slide-up');
            });
        },

        showSuccess(input) {
            input.classList.remove('input-invalid');
            input.classList.add('input-valid');
            setTimeout(() => input.classList.remove('input-valid'), 2000);
        },

        showError(input) {
            input.classList.remove('input-valid');
            input.classList.add('input-invalid');
        }
    };

    // ========================================
    // TABLE ANIMATIONS
    // ========================================
    
    const TableAnimations = {
        init() {
            this.animateTableRows();
            this.setupRowHoverEffects();
        },

        animateTableRows() {
            document.querySelectorAll('table tbody tr').forEach((row, index) => {
                row.classList.add('table-row-premium', 'table-row-enter');
                row.style.animationDelay = `${index * 50}ms`;
            });
        },

        setupRowHoverEffects() {
            document.querySelectorAll('table tbody tr').forEach(row => {
                row.addEventListener('mouseenter', () => {
                    row.style.backgroundColor = 'rgba(56, 189, 248, 0.08)';
                });

                row.addEventListener('mouseleave', () => {
                    row.style.backgroundColor = '';
                });
            });
        }
    };

    // ========================================
    // NAVIGATION ANIMATIONS
    // ========================================
    
    const NavAnimations = {
        init() {
            this.setupNavLinkAnimations();
            this.setupDropdownAnimations();
            this.setupScrollHeader();
        },

        setupNavLinkAnimations() {
            document.querySelectorAll('.nav-link').forEach(link => {
                link.classList.add('nav-link-animated');
            });
        },

        setupDropdownAnimations() {
            document.querySelectorAll('.dropdown').forEach(dropdown => {
                const menu = dropdown.querySelector('.dropdown-menu');
                if (!menu) return;

                menu.classList.add('dropdown-animated');

                dropdown.addEventListener('show.bs.dropdown', () => {
                    menu.classList.add('show');
                });

                dropdown.addEventListener('hide.bs.dropdown', () => {
                    menu.classList.remove('show');
                });
            });
        },

        setupScrollHeader() {
            const header = document.querySelector('.site-header, header');
            if (!header) return;

            let lastScroll = 0;

            window.addEventListener('scroll', Utils.throttle(() => {
                const currentScroll = window.scrollY;

                if (currentScroll > 100) {
                    header.classList.add('header-scrolled');
                    
                    if (currentScroll > lastScroll && currentScroll > 200) {
                        header.classList.add('header-hidden');
                    } else {
                        header.classList.remove('header-hidden');
                    }
                } else {
                    header.classList.remove('header-scrolled');
                }

                lastScroll = currentScroll;
            }, 100));
        }
    };

    // ========================================
    // BUTTON LOADING STATES
    // ========================================
    
    const ButtonLoading = {
        set(button, isLoading = true) {
            if (isLoading) {
                button.disabled = true;
                button.dataset.originalContent = button.innerHTML;
                button.innerHTML = `
                    <div class="spinner-dots">
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                `;
                button.classList.add('btn-loading');
            } else {
                button.disabled = false;
                if (button.dataset.originalContent) {
                    button.innerHTML = button.dataset.originalContent;
                    delete button.dataset.originalContent;
                }
                button.classList.remove('btn-loading');
            }
        },

        success(button, message = 'Success!', duration = 2000) {
            const originalContent = button.innerHTML;
            button.innerHTML = `<i class="bi bi-check-circle-fill"></i> ${message}`;
            button.classList.add('btn-success');
            
            setTimeout(() => {
                button.innerHTML = originalContent;
                button.classList.remove('btn-success');
            }, duration);
        },

        error(button, message = 'Error!', duration = 2000) {
            const originalContent = button.innerHTML;
            button.innerHTML = `<i class="bi bi-x-circle-fill"></i> ${message}`;
            button.classList.add('btn-danger', 'error-x');
            
            setTimeout(() => {
                button.innerHTML = originalContent;
                button.classList.remove('btn-danger', 'error-x');
            }, duration);
        }
    };

    // ========================================
    // SKELETON LOADER
    // ========================================
    
    const SkeletonLoader = {
        create(type = 'card', count = 1) {
            const container = document.createElement('div');
            container.className = 'skeleton-container';

            for (let i = 0; i < count; i++) {
                const skeleton = document.createElement('div');
                skeleton.className = `skeleton-premium skeleton-${type}`;
                container.appendChild(skeleton);
            }

            return container;
        },

        replace(skeletonElement, actualContent) {
            if (!skeletonElement || !actualContent) return;

            skeletonElement.style.opacity = '0';
            skeletonElement.style.transform = 'scale(0.95)';
            skeletonElement.style.transition = 'all 0.3s ease';

            setTimeout(() => {
                actualContent.classList.add('animate-fade-in');
                skeletonElement.replaceWith(actualContent);
            }, 300);
        }
    };

    // ========================================
    // COUNTER ANIMATION
    // ========================================
    
    const CounterAnimation = {
        animate(element, target, duration = 2000) {
            const start = 0;
            const startTime = performance.now();

            const update = (currentTime) => {
                const elapsed = currentTime - startTime;
                const progress = Math.min(elapsed / duration, 1);
                
                // Easing function (ease-out-expo)
                const eased = 1 - Math.pow(2, -10 * progress);
                const current = Math.floor(start + (target - start) * eased);
                
                element.textContent = current.toLocaleString();

                if (progress < 1) {
                    Utils.raf(update);
                } else {
                    element.textContent = target.toLocaleString();
                    element.classList.add('counter-animated');
                }
            };

            Utils.raf(update);
        },

        init() {
            const counters = document.querySelectorAll('[data-counter]');
            
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const target = parseInt(entry.target.getAttribute('data-counter'), 10);
                        this.animate(entry.target, target);
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.5 });

            counters.forEach(counter => observer.observe(counter));
        }
    };

    // ========================================
    // PARTICLES EFFECT
    // ========================================
    
    const ParticlesEffect = {
        init() {
            const container = document.querySelector('.particles-container');
            if (!container) return;

            this.createParticles(container, 30);
        },

        createParticles(container, count) {
            for (let i = 0; i < count; i++) {
                const particle = document.createElement('div');
                particle.className = 'particle';
                particle.style.left = `${Math.random() * 100}%`;
                particle.style.animationDelay = `${Math.random() * 15}s`;
                particle.style.animationDuration = `${15 + Math.random() * 10}s`;
                container.appendChild(particle);
            }
        }
    };

    // ========================================
    // HERO ANIMATIONS
    // ========================================
    
    const HeroAnimations = {
        init() {
            const hero = document.querySelector('.cinema-hero, .modern-hero');
            if (!hero) return;

            this.animateHeroContent();
            this.setupHeroParallax();
        },

        animateHeroContent() {
            const heroContent = document.querySelector('.hero-content, .hero-movie-info');
            if (heroContent) {
                heroContent.classList.add('hero-stagger', 'active');
            }
        },

        setupHeroParallax() {
            const heroBackground = document.querySelector('.hero-bg-image');
            if (!heroBackground) return;

            window.addEventListener('scroll', Utils.throttle(() => {
                const scrolled = window.scrollY;
                heroBackground.style.transform = `translateY(${scrolled * 0.3}px) scale(1.1)`;
            }, 16));
        }
    };

    // ========================================
    // TAB ANIMATIONS
    // ========================================
    
    const TabAnimations = {
        init() {
            this.setupTabTransitions();
        },

        setupTabTransitions() {
            document.querySelectorAll('.tab-btn').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const tabId = btn.getAttribute('data-tab');
                    const tabContent = document.getElementById(tabId);
                    
                    if (!tabContent) return;

                    // Animate out current content
                    document.querySelectorAll('.tab-content.active').forEach(content => {
                        content.style.opacity = '0';
                        content.style.transform = 'translateY(20px)';
                        setTimeout(() => {
                            content.classList.remove('active');
                            content.style.opacity = '';
                            content.style.transform = '';
                        }, 300);
                    });

                    // Animate in new content
                    setTimeout(() => {
                        tabContent.classList.add('active');
                        tabContent.style.opacity = '0';
                        tabContent.style.transform = 'translateY(20px)';
                        
                        Utils.raf(() => {
                            tabContent.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
                            tabContent.style.opacity = '1';
                            tabContent.style.transform = 'translateY(0)';
                        });
                    }, 300);
                });
            });
        }
    };

    // ========================================
    // MODAL ANIMATIONS
    // ========================================
    
    const ModalAnimations = {
        init() {
            this.setupModalTransitions();
        },

        setupModalTransitions() {
            document.querySelectorAll('.modal').forEach(modal => {
                modal.addEventListener('show.bs.modal', () => {
                    const dialog = modal.querySelector('.modal-dialog');
                    const content = modal.querySelector('.modal-content');
                    
                    if (content) {
                        content.classList.add('modal-content-animated');
                        setTimeout(() => content.classList.add('show'), 10);
                    }
                });

                modal.addEventListener('hide.bs.modal', () => {
                    const content = modal.querySelector('.modal-content');
                    if (content) {
                        content.classList.remove('show');
                    }
                });
            });
        }
    };

    // ========================================
    // INITIALIZATION
    // ========================================
    
    function init() {
        console.log('Nexor Cinema - Premium Animation System v2.0');
        
        // Initialize all animation modules
        PageLoader.init();
        
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initializeModules);
        } else {
            initializeModules();
        }
    }

    function initializeModules() {
        ScrollAnimations.init();
        HoverAnimations.init();
        SmoothScroll.init();
        PageTransitions.init();
        AlertAnimations.init();
        FormAnimations.init();
        TableAnimations.init();
        NavAnimations.init();
        CounterAnimation.init();
        ParticlesEffect.init();
        HeroAnimations.init();
        TabAnimations.init();
        ModalAnimations.init();

        console.log('All premium animation modules loaded');
    }

    // ========================================
    // PUBLIC API
    // ========================================
    
    window.NexorPremiumAnimations = {
        Utils,
        PageLoader,
        ScrollAnimations,
        HoverAnimations,
        SmoothScroll,
        PageTransitions,
        AlertAnimations,
        FormAnimations,
        TableAnimations,
        NavAnimations,
        ButtonLoading,
        SkeletonLoader,
        CounterAnimation,
        ParticlesEffect,
        HeroAnimations,
        TabAnimations,
        ModalAnimations,
        refresh: () => {
            ScrollAnimations.refresh();
            TableAnimations.animateTableRows();
            FormAnimations.setupFormGroupStagger();
        }
    };

    // Auto-initialize
    init();

})();
