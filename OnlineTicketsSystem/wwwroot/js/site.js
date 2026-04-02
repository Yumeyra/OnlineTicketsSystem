// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function toggleDarkMode() {
//    document.body.classList.toggle("dark-mode");
//}
//function toggleDarkMode() {
//    const html = document.documentElement;
//    const current = html.getAttribute("data-bs-theme");

//    const newTheme = current === "dark" ? "light" : "dark";

//    html.setAttribute("data-bs-theme", newTheme);
//    localStorage.setItem("theme", newTheme);

//    updateThemeIcon(newTheme);
//}

//function updateThemeIcon(theme) {
//    const icon = document.getElementById("theme-btn-icon");
//    if (!icon) return;

//    icon.textContent = theme === "dark" ? "☀️" : "🌙";
//}

//// 🔥 ВАЖНО – изпълнява се веднага
//document.addEventListener("DOMContentLoaded", () => {
//    const savedTheme = localStorage.getItem("theme") || "light";

//    document.documentElement.setAttribute("data-bs-theme", savedTheme);
//    updateThemeIcon(savedTheme);
//});

function toggleDarkMode() {
    const html = document.documentElement;
    const current = html.getAttribute("data-bs-theme");
    const newTheme = current === "dark" ? "light" : "dark";

    html.setAttribute("data-bs-theme", newTheme);
    localStorage.setItem("theme", newTheme);

    updateThemeIcon(newTheme);
}

function updateThemeIcon(theme) {
    const icon = document.getElementById("theme-btn-icon");
    if (!icon) return;

    icon.textContent = theme === "dark" ? "☀️" : "🌙";
}

document.addEventListener("DOMContentLoaded", () => {
    const savedTheme = localStorage.getItem("theme") || "light";
    document.documentElement.setAttribute("data-bs-theme", savedTheme);
    updateThemeIcon(savedTheme);
});
