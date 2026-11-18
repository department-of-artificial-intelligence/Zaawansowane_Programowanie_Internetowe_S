import "./style.css";
import type { Subscription } from "rxjs";
import { zadania } from "./zadania";

const app = document.querySelector<HTMLDivElement>("#app")!;
const naglowek = document.createElement("header");
const h1 = document.createElement("h1");
h1.textContent = "Zajęcia 5 – programowanie funkcyjne i RxJS";
const menu = document.createElement("nav");
naglowek.append(h1, menu);
const tresc = document.createElement("main");
app.append(naglowek, tresc);

for (const zadanie of zadania) {
  const link = document.createElement("a");
  link.href = `#zadanie-${zadanie.id}`;
  link.textContent = zadanie.id;
  link.title = zadanie.tytul;
  menu.appendChild(link);
}

// subskrypcja aktualnie otwartego zadania - odpinana przy zmianie zakładki
let aktywna: Subscription | void;

function pokaz(): void {
  const id = location.hash.replace("#zadanie-", "");
  const zadanie = zadania.find(z => z.id === id) ?? zadania[0];

  aktywna?.unsubscribe();
  menu.querySelectorAll("a").forEach(a =>
    a.classList.toggle("aktywne", a.getAttribute("href") === `#zadanie-${zadanie.id}`));

  tresc.replaceChildren();
  const tytul = document.createElement("h2");
  tytul.textContent = `Zadanie ${zadanie.id}. ${zadanie.tytul}`;
  const opis = document.createElement("p");
  opis.className = "opis";
  opis.textContent = zadanie.opis;
  tresc.append(tytul, opis);

  aktywna = zadanie.uruchom(tresc);
}

window.addEventListener("hashchange", pokaz);
pokaz();
