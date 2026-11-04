// Zamiana znaków specjalnych, żeby dane (np. z serwera) nie zostały zinterpretowane jako HTML
export function escapeHtml(tekst: string): string {
  return tekst
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}
