const liczby: number[] = [3, 8, 1, 12, 5, 6];
const suma = liczby.reduce((acc, x) => acc + x, 0);
console.log(`Suma: ${suma}`)