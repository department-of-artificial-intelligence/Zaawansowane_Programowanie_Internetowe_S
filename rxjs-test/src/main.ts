import "./style.css";
import {
  curry2, greaterThan, suma, sumaLiczbZNapisow, sumaParzystych,
  sumaWiekszychNiz, sumaWiekszychNizZ4, sumaWiekszychNizZ5,
} from "./funkcyjne";

const liczby: readonly number[] = [3, 8, 15, 4, 23, 42, 7, 10];
const prog = 9;
const napisy: readonly string[] = ["Ala", "1", "Ewa", "12.4", "", "12abc", "-3", " 5 "];

interface Wynik {
  zadanie: number;
  opis: string;
  kod: string;
  wynik: unknown;
}

const wyniki: Wynik[] = [
  {
    zadanie: 1,
    opis: "Suma wszystkich elementów",
    kod: "liczby.reduce((acc, x) => acc + x, 0)",
    wynik: suma(liczby),
  },
  {
    zadanie: 2,
    opis: "Suma wartości parzystych",
    kod: "suma(liczby.filter(x => x % 2 === 0))",
    wynik: sumaParzystych(liczby),
  },
  {
    zadanie: 3,
    opis: `Suma wartości większych od ${prog}`,
    kod: `suma(liczby.filter(x => x > ${prog}))`,
    wynik: sumaWiekszychNiz(liczby, prog),
  },
  {
    zadanie: 4,
    opis: `Predykat jako funkcja greaterThan(prog, wartosc), prog = ${prog}`,
    kod: `sumaGdzie(liczby, x => greaterThan(${prog}, x))`,
    wynik: sumaWiekszychNizZ4(liczby, prog),
  },
  {
    zadanie: 5,
    opis: "To samo z curry2: curry2(greaterThan)(prog) to gotowy predykat",
    kod: `sumaGdzie(liczby, curry2(greaterThan)(${prog}))`,
    wynik: sumaWiekszychNizZ5(liczby, prog),
  },
  {
    zadanie: 6,
    opis: `Suma napisów reprezentujących liczby z ${JSON.stringify(napisy)}`,
    kod: "napisy.filter(czyLiczba).map(Number).reduce((acc, x) => acc + x, 0)",
    wynik: sumaLiczbZNapisow(napisy),
  },
];

// dodatkowo dla przykładu z treści zadania 6
const przykladZ6 = sumaLiczbZNapisow(["Ala", "1", "Ewa", "12.4"]);
// curry2 działa dla dowolnej funkcji dwuargumentowej, np. jako "gotowy" predykat
const wiekszeOd20 = curry2(greaterThan)(20);

const app = document.querySelector<HTMLDivElement>("#app")!;
const naglowek = document.createElement("h1");
naglowek.textContent = "Zajęcia 5 – tablice w stylu funkcyjnym";
const dane = document.createElement("p");
dane.textContent = `liczby = [${liczby.join(", ")}]`;
app.append(naglowek, dane);

for (const w of wyniki) {
  const sekcja = document.createElement("section");
  const tytul = document.createElement("h2");
  tytul.textContent = `Zadanie ${w.zadanie}. ${w.opis}`;
  const kod = document.createElement("pre");
  kod.textContent = `${w.kod}\n// wynik: ${String(w.wynik)}`;
  sekcja.append(tytul, kod);
  app.appendChild(sekcja);
  console.log(`Zadanie ${w.zadanie}:`, w.wynik);
}

const dodatki = document.createElement("pre");
dodatki.textContent =
  `sumaLiczbZNapisow(["Ala", "1", "Ewa", "12.4"]) = ${przykladZ6}\n` +
  `[${liczby.join(", ")}].filter(curry2(greaterThan)(20)) = [${liczby.filter(wiekszeOd20).join(", ")}]`;
app.appendChild(dodatki);
