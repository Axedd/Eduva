const toggleButton = document.querySelector('.toggle-sidebar');
const sidebar = document.querySelector('.sidebar');
const container = document.querySelector('.app-container');

toggleButton.addEventListener('click', () => {
    sidebar.classList.toggle('closed');
    container.classList.toggle('full-width');
});

// For mobile responsiveness
if (window.innerWidth <= 768) {
    sidebar.classList.add('closed');
    container.classList.add('full-width');
}

window.addEventListener('resize', () => {
    if (window.innerWidth <= 768) {
        sidebar.classList.add('closed');
        container.classList.add('full-width');
    } else {
        sidebar.classList.remove('closed');
        container.classList.remove('full-width');
    }
});