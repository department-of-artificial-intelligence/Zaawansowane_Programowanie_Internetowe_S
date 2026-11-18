// Drobne funkcje do budowania elementów strony (żeby nie powtarzać createElement)

export function przycisk(tekst: string): HTMLButtonElement {
  const b = document.createElement("button");
  b.type = "button";
  b.textContent = tekst;
  return b;
}

export function pole(typ: "text" | "number" = "text", placeholder = ""): HTMLInputElement {
  const i = document.createElement("input");
  i.type = typ;
  i.placeholder = placeholder;
  return i;
}

export function akapit(tekst = ""): HTMLParagraphElement {
  const p = document.createElement("p");
  p.className = "wynik";
  p.textContent = tekst;
  return p;
}

export function wiersz(...elementy: (HTMLElement | string)[]): HTMLDivElement {
  const d = document.createElement("div");
  d.className = "wiersz";
  d.append(...elementy);
  return d;
}

export function etykieta(tekst: string, element: HTMLElement): HTMLLabelElement {
  const l = document.createElement("label");
  l.append(tekst, element);
  return l;
}
