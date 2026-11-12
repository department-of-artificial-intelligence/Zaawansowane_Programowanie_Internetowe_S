const liczby: number[] =[1,2,3,4,5,6,7,8,9];
const prog: number = 4;

function curry2<T1, T2, T3>(fn:(arg1:T1, arg2:T2)=>T3) {
return (a1:T1)=>(a2:T2)=>fn(a1,a2);
}

const greaterThan = (minwartosc: number) => {
  return (liczba : number): boolean => liczba > minwartosc;
}
const GT1 = (minwartosc: number,liczba:number) :boolean => {
  return liczba > minwartosc;
}
const GT2 = curry2(GT1);
const suma = (tablica: number[], minWartosc: number): number => {
  return tablica
    .filter((liczba) => liczba % 2 === 0) // && liczba > minWartosc;
    .filter(greaterThan(minWartosc))
    .reduce((acc, curr) => acc + curr, 0);
};

const wynik: number = suma(liczby, prog);
console.log(`Suma parzystych większych od ${prog}: ${wynik}`); 

const dane: string[] = ["Ala", "1", "Ewa", "12.4"];

const sumaLiczb: number = dane
  .map((el: string) => Number(el))
  .filter((liczba: number) => !Number.isNaN(liczba))
  .reduce((acc: number, curr: number) => acc + curr, 0);

console.log(`Suma liczb z tablicy: ${sumaLiczb}`);
