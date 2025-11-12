function greaterThan(limit: number, num: number): boolean {
  return num > limit;
}



const tablica = [1, 2, 3, 4, 6, 8, 2, 3, 4, 5, 5, 5, 5];

// użycie curryfikowanej funkcji
const evenSum: number = tablica
  .filter(greaterThan)           // liczby > 2
  .filter(num => num % 2 === 0)   // liczby parzyste
  .reduce((acc, current) => acc + current, 0);

document.getElementById('test')!.innerText = String(evenSum);
console.log('Suma liczb parzystych większych od limitu:', evenSum);

function curry2<T1, T2, T3>(fn:(arg1:T1, arg2:T2)=>T3) {
return (a1:T1)=>(a2:T2)=>fn(a1,a2);}

