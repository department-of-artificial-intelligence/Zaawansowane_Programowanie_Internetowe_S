import { Autor } from './Autor';

// Przykładowa lista autorów
const autorzy: Autor[] = [
  new Autor("Jan", "Kowalski", "jan.kowalski@example.com"),
  new Autor("Anna", "Nowak", "anna.nowak@example.com"),
  new Autor("Piotr", "Zieliński", "piotr.zielinski@example.com")
];

// Funkcja generująca tabelę HTML
function generujTabeleAutora(autorzy: Autor[]): string {
  let tabelaHTML = `
    <table border="1">
      <thead>
        <tr>
          <th>Imię</th>
          <th>Nazwisko</th>
          <th>Email</th>
        </tr>
      </thead>
      <tbody>`;

  // Generowanie wierszy tabeli na podstawie autorów
  autorzy.forEach((autor) => {
    tabelaHTML += ` 
      <tr>
        <td>${autor.imie}</td>
        <td>${autor.nazwisko}</td>
        <td>${autor.email}</td>
      </tr>`;
  });

  tabelaHTML += `
      </tbody>
    </table>`;
  
  return tabelaHTML;
}

// Dodanie tabeli HTML do strony (przykład dla przeglądarki)
document.getElementById("tabelaAutorow")!.innerHTML = generujTabeleAutora(autorzy);
