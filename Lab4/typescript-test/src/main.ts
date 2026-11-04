import "./style.css";
import { zadania } from "./zadania";

const app = document.querySelector<HTMLDivElement>("#app")!;

const naglowek = document.createElement("header");
naglowek.innerHTML = "<h1>Zajęcia 4 – TypeScript</h1>";
const menu = document.createElement("nav");
const tresc = document.createElement("main");
naglowek.appendChild(menu);
app.append(naglowek, tresc);

for (const zadanie of zadania) {
  const link = document.createElement("a");
  link.href = `#zadanie-${zadanie.numer}`;
  link.textContent = String(zadanie.numer);
  link.title = zadanie.tytul;
  menu.appendChild(link);
}

async function pokaz(): Promise<void> {
  const numer = Number(location.hash.replace("#zadanie-", "")) || 1;
  const zadanie = zadania.find(z => z.numer === numer) ?? zadania[0];

  menu.querySelectorAll("a").forEach(a =>
    a.classList.toggle("aktywne", a.getAttribute("href") === `#zadanie-${zadanie.numer}`));

  tresc.replaceChildren();
  const tytul = document.createElement("h2");
  tytul.textContent = `Zadanie ${zadanie.numer}. ${zadanie.tytul}`;
  tresc.appendChild(tytul);

  try {
    await zadanie.uruchom(tresc);
  } catch (blad) {
    console.error(blad);
    const p = document.createElement("p");
    p.className = "blad";
    p.textContent = String(blad);
    tresc.appendChild(p);
  }
}

window.addEventListener("hashchange", () => void pokaz());
void pokaz();
