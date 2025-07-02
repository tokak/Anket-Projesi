document.addEventListener("DOMContentLoaded", function () {
    // Mobil menü toggle
    const burgerBtn = document.querySelector('.burger-btn');
    const sidebar = document.querySelector('#sidebar');
    if (burgerBtn && sidebar) {
        burgerBtn.addEventListener('click', function (event) {
            event.preventDefault();
            sidebar.classList.toggle('active');
        });
    }

    // Sidebar'ı kapatma (mobil görünümde)
    const sidebarHide = document.querySelector('.sidebar-hide');
    if (sidebarHide && sidebar) {
        sidebarHide.addEventListener('click', function (event) {
            event.preventDefault();
            sidebar.classList.remove('active');
        });
    }
    const submenus = document.querySelectorAll('.sidebar-item.has-sub');
    submenus.forEach(menu => {
        const link = menu.querySelector('a.sidebar-link');
        link.addEventListener('click', (e) => {
            e.preventDefault();

            // Tıklanan menünün durumunu değiştir (active sınıfı ekle/kaldır)
            menu.classList.toggle('active');
        });
    });
    // ===============================================================


    // Aktif menü elemanını ve üst menüsünü işaretleme
    const currentUrl = window.location.href;
    const sidebarLinks = document.querySelectorAll('.sidebar-item a, .submenu-item a');
    sidebarLinks.forEach(link => {
        if (link.href === currentUrl) {
            // Aktif linkin kendisi
            link.parentElement.classList.add('active');

            // Eğer bu bir alt menü elemanı ise, üst menüyü de aktif yap
            const parentMenu = link.closest('.sidebar-item.has-sub');
            if (parentMenu) {
                parentMenu.classList.add('active');
            }
        }
    });
});