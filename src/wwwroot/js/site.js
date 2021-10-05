
const btn = document.querySelector("button.mobile-menu-button");
const menu = document.querySelector(".mobile-menu");

// add event listeners
btn.addEventListener("click", () => {
    menu.classList.toggle("hidden");
});


function closeMobileMenu() {
    var v = document.getElementById("mobile-menu");
    v.classList.add("hidden");
}

window.addEventListener('resize', closeMobileMenu);