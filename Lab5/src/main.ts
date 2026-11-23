const teksty: string[] = ["Ala", "1", "Ewa", "12.4"];

const suma = teksty
  .map(s => Number(s))               // [NaN, 1, NaN, 12.4]  string[] -> number[]
  .filter(x => !Number.isNaN(x))     // [1, 12.4]            zostają tylko liczby
  .reduce((acc, x) => acc + x, 0);   // 1 + 12.4

console.log(`Suma liczb: ${suma}`);  // Suma liczb: 13.4