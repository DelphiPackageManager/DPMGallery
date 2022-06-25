//todo : can we replace this with alpine js in the views?
var btn = document.querySelector("button.mobile-menu-button");
var menu = document.querySelector(".mobile-menu");
// add event listeners
btn.addEventListener("click", function () {
    menu.classList.toggle("hidden");
});
function clearOnEscape(evt, input) {
    var code = evt.charCode || evt.keyCode;
    if (code == 27) {
        input.value = '';
        event.preventDefault();
        document.querySelector('form').submit();
    }
}
function closeMobileMenu() {
    var v = document.getElementById("mobile-menu");
    v.classList.add("hidden");
}
window.addEventListener('resize', closeMobileMenu);
//# sourceMappingURL=site.js.map