/******/ (() => { // webpackBootstrap
/*!**********************!*\
  !*** ./src/index.ts ***!
  \**********************/
const numbers = [1, 2, 3, 4, 5];
document.addEventListener("DOMContentLoaded", () => {
    const suma = document.getElementById("Suma");
    const parzyste = document.getElementById("Parzyste");
    const zadaneWynik = document.getElementById("Zadane");
    const zadaneWynik1 = document.getElementById("currying");
    const input = document.getElementById("liczba");
    const button = document.getElementById("oblicz");
    const sum = numbers.reduce((acc, val) => acc + val, 0);
    const parzysta = numbers
        .filter(num => num % 2 === 0)
        .reduce((acc, val) => acc + val, 0);
    if (suma)
        suma.textContent = `Suma wszystkich: ${sum}`;
    if (parzyste)
        parzyste.textContent = `Suma parzystych: ${parzysta}`;
    function greaterThan(limit) {
        return (value) => value > limit;
    }
    function curry2(fn) {
        return (a) => (b) => fn(a, b);
    }
    const greaterThanC = curry2((limit, value) => value > limit);
    button?.addEventListener("click", () => {
        const liczba2 = parseInt(input.value, 10);
        if (isNaN(liczba2)) {
            zadaneWynik.textContent = "Podaj poprawną liczbę!";
            return;
        }
        const zadane = numbers
            .filter(greaterThan(liczba2))
            .reduce((acc, val) => acc + val, 0);
        const sumWithCurry = numbers
            .filter(greaterThan(liczba2))
            .reduce((acc, val) => acc + val, 0);
        zadaneWynik.textContent = `Suma od liczby ${liczba2}: ${zadane}`;
        zadaneWynik1.textContent = `Suma od liczby ${liczba2}: ${sumWithCurry}`;
    });
});

/******/ })()
;
//# sourceMappingURL=bundle.js.map