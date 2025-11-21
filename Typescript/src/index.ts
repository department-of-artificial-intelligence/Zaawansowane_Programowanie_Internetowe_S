import { from, filter, reduce } from "rxjs";

const thresholdValue = 2; 
var arrayNumber = [1, 2, 3, 4, 5, 6];
var arrayNumberStream = from(arrayNumber);

function greaterThanThreshold(threshold: number) {
    return (currentValue: number): boolean => {
        return currentValue > threshold;
    };
}
const curriedReduceOperator = curry2(
    (reducerFn: (acc: number, val: number) => number, initialValue: number) => {
        return reduce(reducerFn, initialValue);
    }
);

const sumReducer = curriedReduceOperator((accumulator: number, currentValue: number) => accumulator + currentValue);
const arrayNumberObservable = arrayNumberStream.pipe(
    filter(greaterThanThreshold(thresholdValue)),
    sumReducer(0) 
);

arrayNumberObservable.subscribe({
    next: (result) => {
        console.log(`The sum of values greater than ${thresholdValue} is:`, result);
    },
    error: (err) => console.error(err),
    complete: () => console.log('Summation Complete.')
});

function curry2<T1, T2, T3>(fn:(arg1:T1, arg2:T2)=>T3) {
    return (a1:T1)=>(a2:T2)=>fn(a1,a2);
}

const stringArray = ["Ala", "1", "Ewa", "12.4"];

const isNumeric = (str: string): boolean => {
    const num = Number(str);
    return !isNaN(num);
};

const totalSum = stringArray.reduce((accumulator, currentValue) => {
    
    if (isNumeric(currentValue)) {
        const numValue = Number(currentValue);
        return accumulator + numValue;
    }
    
    return accumulator;
}, 0);

console.log("Oryginalna tablica:", stringArray);
console.log("Suma wartości numerycznych:", totalSum);