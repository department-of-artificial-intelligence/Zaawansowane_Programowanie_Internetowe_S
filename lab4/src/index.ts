// import { Autor } from "./Autor";
// const autorzy: Autor[] = [
//     new Autor("Adam", "Kowalski", "adam.kowalski@example.com"),
//     new Autor("Ewa", "Nowak", "ewa.nowak@example.com"),
//     new Autor("Piotr", "Zieliński", "piotr.zielinski@example.com"),
// ];

// // funkcja generująca tabelę HTML
// function generujTabeleAutorow(autorzy: Autor[]): string {
//     let html = `
//         <table border="1" cellpadding="5" cellspacing="0">
//             <thead>
//                 <tr>
//                     <th>ID</th>
//                     <th>Imię</th>
//                     <th>Nazwisko</th>
//                     <th>Email</th>
//                 </tr>
//             </thead>
//             <tbody>
//     `;

//     for (const a of autorzy) {
//         html += `
//             <tr>
//                 <td>${a.id}</td>
//                 <td>${a.imie}</td>
//                 <td>${a.nazwisko}</td>
//                 <td>${a.email}</td>
//             </tr>
//         `;
//     }

//     html += `</tbody></table>`;
//     return html;
// }

// // --- wyświetlenie w przeglądarce ---
// document.body.innerHTML = `
//     <h1>Lista autorów</h1>
//     ${generujTabeleAutorow(autorzy)}
// `;