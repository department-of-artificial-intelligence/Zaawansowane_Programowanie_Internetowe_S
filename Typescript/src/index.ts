
// Zadanie 01 
const tab1: number[] = [1, 2, 3, 4, 5];
const sum: number = tab1.reduce((prev, current) => prev + current);
const sumEven: number = (tab1.filter(x => x % 2 == 0)).reduce((prev, current) => prev + current);

function greaterThan(value: number): number {
    return (tab1.filter(x => x > value)).reduce((prev, current) => prev + current, 0);
}



function hello() {
    console.log("Suma " + sum);
    console.log("Suma parzystych " + sumEven);
    var valueStr = prompt("Podaj liczbe");
    if (valueStr === null) {
        alert("Nieprawidlowa wartosc");
    } else {
        let value: number = parseInt(valueStr);
        if (Number.isNaN(value)) {
            alert("Nieprawidlowa wartosc");
            return;
        }
        console.log("Suma powyzej wpisanej liczby " + greaterThan(value));
    }
}
hello();
