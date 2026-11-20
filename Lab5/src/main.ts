import { getRandom } from "./utils/random";
import { greaterThan } from "./utils/logical";
import { curry2 } from "./utils/curry";
import { Counter } from "./components/counter";
import { InputTracker } from "./components/inputTracker";
import { InputMultiplier } from "./components/multiplier";
import { DivFollower } from "./components/divFollower";

/* Zadnia 1-6 */
const stringArray = ["Ala", "1", "Ewa", "12.4"];

const randomNumbers: number[] = [];
for (let i = 0; i < 10; i++) {
    randomNumbers.push(getRandom(10, 20));
}

const greaterThan11 = curry2(greaterThan)(11);

const tapLog = <T>(format: (value: T) => string) => (value: T): T => {
    console.log(format(value));
    return value;
};

const sum = randomNumbers
    .filter((value) => value % 2 === 0 && greaterThan11(value))
    .map(tapLog((value) => `Passed filter: ${value}`))
    .reduce((acc, value) => acc + value, 0);

console.log(`Sum is: ${sum}`);

const stringSum = stringArray
    .map((value) => parseFloat(value))
    .filter((value) => !Number.isNaN(value))
    .reduce((acc, value) => acc + value, 0);

console.log(`Sum of numbers in string array is: ${stringSum}`);
/* ================================================================= */

const appRoot = document.getElementById("app");

const counter = new Counter();
counter.mount(appRoot);

const inputTracker = new InputTracker();
inputTracker.mount(appRoot);

const inputMultiplier = new InputMultiplier();
inputMultiplier.mount(appRoot);

const divFollower = new DivFollower(100);
divFollower.mount(appRoot);