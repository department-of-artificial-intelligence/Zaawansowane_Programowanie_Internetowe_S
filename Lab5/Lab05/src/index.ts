import { section } from "./dom";
import { task01 } from "./tasks/task01";
import { task02 } from "./tasks/task02";
import { task03 } from "./tasks/task03";
import { task04 } from "./tasks/task04";
import { task05 } from "./tasks/task05";
import { task06 } from "./tasks/task06";
import { task07 } from "./tasks/task07";
import { task08 } from "./tasks/task08";
import { task09 } from "./tasks/task09";
import { task10 } from "./tasks/task10";
import { task11 } from "./tasks/task11";
import { task12 } from "./tasks/task12";
import { task13 } from "./tasks/task13";
import { task14 } from "./tasks/task14";
import { task15 } from "./tasks/task15";
import { task16 } from "./tasks/task16";
import { task17 } from "./tasks/task17";
import { task18 } from "./tasks/task18";
import { task19 } from "./tasks/task19";
import { task20 } from "./tasks/task20";
import { task21 } from "./tasks/task21";
import { task22 } from "./tasks/task22";
import { task23 } from "./tasks/task23";
import { task24 } from "./tasks/task24";
import { task25 } from "./tasks/task25";
import { task26 } from "./tasks/task26";
import { task27 } from "./tasks/task27";

const tasks = [
    task01, task02, task03, task04, task05, task06, task07, task08, task09, task10,
    task11, task12, task13, task14, task15, task16, task17, task18, task19, task20,
    task21, task22, task23, task24, task25, task26, task27
];

tasks.forEach((task, index) => task(section(`Task ${index + 1}`)));
