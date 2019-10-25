let tab = [1, 2, 3, 4];
// ZAD 1
let sum = (acc, value) => acc + value;
let zad1Result = tab.reduce(sum, 0);
console.log(zad1Result);
// ZAD 2 
let sumEven = (acc, value) => value % 2 === 0 ? acc + value : acc;
let zad2Result = tab.reduce(sumEven, 0);
console.log(zad2Result);
// ZAD 3
let sumGreatherEven = (acc, value, threshold) => value % 2 === 0 && value > threshold ? acc + value : acc;
let zad3Result = tab.reduce((acc, value) => sumGreatherEven(acc, value, 3), 0);
console.log(zad3Result);
// ZAD 4 
let greatherFunc = (value, threshold) => value > threshold ? true : false;
let sumGreatherEven4 = (acc, value, threshold) => value % 2 === 0 && greatherFunc(value, threshold) ? acc + value : acc;
let zad4Result = tab.reduce((acc, value) => sumGreatherEven4(acc, value, 3), 0);
console.log(zad4Result);
// zad 5
function sumEven5(acc, value) {
    return (threshold) => value % 2 === 0 && value > threshold ? acc + value : acc;
}
let zad5Result = tab.reduce((acc, value) => sumEven5(acc, value)(1), 0);
console.log(zad5Result);
// zad6 
let arrayOfStrings = ["13", "posterunek2", "odc2"];
let sumIfIsNotNaN = (acc, value) => !isNaN(value) ? acc + parseFloat(value) : 0;
let zad6Result = arrayOfStrings.reduce((acc, str) => {
    let sum = 0;
    sum = Array.from(str).reduce(sumIfIsNotNaN, sum);
    // for (var i = 0; i < str.length; ++i) {
    //     sum += sumIfIsNotNaN(acc, str[i])
    // }    
    return sum;
}, 0);
console.log(zad6Result);
