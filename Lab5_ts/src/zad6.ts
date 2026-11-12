// Zadanie 6 - suma tych stringów, które reprezentują liczby
const strings: string[] = ["Ala", "1", "Ewa", "12.4", "", "7kot", "5"];

const isNumeric = (s: string): boolean => s.trim() !== "" && !isNaN(Number(s));

export function zad6(): void {
  const sum = strings
    .filter(isNumeric)
    .map(Number)
    .reduce((acc, v) => acc + v, 0);
  console.log("Zad 6:", sum);
}