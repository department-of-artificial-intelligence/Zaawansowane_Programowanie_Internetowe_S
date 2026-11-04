// Zadanie 2: wspólne funkcje walidujące. Rzucają wyjątek, więc obiektu
// z niepoprawnymi danymi nie da się utworzyć ani później zepsuć przez setter.

export class BladWalidacji extends Error {
  constructor(message: string) {
    super(message);
    this.name = "BladWalidacji";
  }
}

// Składniowa poprawność adresu: coś@coś.domena (bez spacji, jedna małpa)
const WZORZEC_EMAIL = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;

export function wymagany(wartosc: string | null | undefined, nazwaPola: string): string {
  if (wartosc === null || wartosc === undefined || wartosc.trim().length === 0) {
    throw new BladWalidacji(`Pole "${nazwaPola}" jest wymagane`);
  }
  return wartosc.trim();
}

export function poprawnyEmail(wartosc: string | null | undefined, nazwaPola = "email"): string {
  const email = wymagany(wartosc, nazwaPola);
  if (!WZORZEC_EMAIL.test(email)) {
    throw new BladWalidacji(`Pole "${nazwaPola}" musi zawierać poprawny adres e-mail`);
  }
  return email;
}

export function wymaganyObiekt<T>(wartosc: T | null | undefined, nazwaPola: string): T {
  if (wartosc === null || wartosc === undefined) {
    throw new BladWalidacji(`Pole "${nazwaPola}" musi być podane`);
  }
  return wartosc;
}
