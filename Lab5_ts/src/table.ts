// Funkcja pomocnicza do zadań 22 i 26: buduje tabelę HTML z tablicy obiektów.
// Nagłówki kolumn są brane z nazw właściwości pierwszego obiektu.
export function createTable(data: Record<string, unknown>[]): HTMLTableElement {
  const table = document.createElement("table");
  if (data.length === 0) {
    table.innerHTML = "<tr><td>Brak danych</td></tr>";
    return table;
  }

  const columns = Object.keys(data[0]);

  const headerRow = table.createTHead().insertRow();
  columns.forEach(col => {
    const th = document.createElement("th");
    th.textContent = col;
    headerRow.appendChild(th);
  });

  const tbody = table.createTBody();
  data.forEach(item => {
    const row = tbody.insertRow();
    columns.forEach(col => row.insertCell().textContent = String(item[col]));
  });

  return table;
}