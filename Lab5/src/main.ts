const liczby: number[] = [3, 8, 1, 12, 5, 6];

const sumaParzystych = liczby
  .filter(x => x % 2 === 0)        
  .reduce((acc, x) => acc + x, 0);

console.log(`Suma parzystych: ${sumaParzystych}`);