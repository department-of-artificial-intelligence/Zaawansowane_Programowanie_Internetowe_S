import { fromEvent, map, type Observable } from "rxjs";

export interface Punkt {
  x: number;
  y: number;
}

// Położenie kursora w oknie przeglądarki
export const ruchMyszy$: Observable<Punkt> = fromEvent<MouseEvent>(document, "mousemove").pipe(
  map(e => ({ x: e.clientX, y: e.clientY })),
);

export function utworzKropke(kontener: HTMLElement, kolor: string): HTMLDivElement {
  const kropka = document.createElement("div");
  kropka.className = "kropka";
  kropka.style.background = kolor;
  kontener.appendChild(kropka);
  return kropka;
}

export function przesun(kropka: HTMLElement, p: Punkt): void {
  kropka.style.transform = `translate(${p.x}px, ${p.y}px)`;
  kropka.style.opacity = "1";
}
