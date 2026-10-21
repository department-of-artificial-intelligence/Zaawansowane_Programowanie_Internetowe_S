# Biblioteka – zadania 9–11

## Zadanie 9 – reguły biznesowe

**Katalog książek**
- ISBN jest wymagany, musi mieć poprawną cyfrę kontrolną (ISBN-10 lub ISBN-13) i jest unikalny w katalogu.
- Tytuł i autor są wymagane, liczba egzemplarzy mieści się w zakresie 1–1000.
- Nie można usunąć książki, której egzemplarze są wypożyczone. Usunięcie książki anuluje jej rezerwacje.

**Czytelnicy**
- Imię i nazwisko są wymagane.
- Nie można usunąć czytelnika, który nie zwrócił wszystkich książek. Usunięcie czytelnika anuluje jego rezerwacje.

**Wypożyczenia**
- Czytelnik może mieć jednocześnie maksymalnie 5 wypożyczonych książek.
- Czytelnik z przeterminowanym wypożyczeniem nie może wypożyczać kolejnych książek.
- Czytelnik nie może wypożyczyć drugiego egzemplarza tej samej książki.
- Książkę można wypożyczyć tylko wtedy, gdy jest wolny egzemplarz, który nie jest „zajęty” przez rezerwację osoby wcześniejszej w kolejce.
- Wypożyczenie trwa 30 dni. Można je przedłużyć raz, o 14 dni, jeżeli nie jest przeterminowane i nikt nie zarezerwował tej książki.
- Książkę zwraca się tylko raz. Wypożyczenie zarezerwowanej książki realizuje rezerwację czytelnika.

**Rezerwacje**
- Rezerwować można tylko książkę, której czytelnik nie może w tej chwili wypożyczyć (brak wolnych egzemplarzy).
- Czytelnik nie może zarezerwować książki, którą już zarezerwował lub wypożyczył.
- Czytelnik może mieć maksymalnie 3 aktywne rezerwacje.
- Rezerwacje są obsługiwane w kolejności ich złożenia. Anulować można tylko aktywną rezerwację.

## Zadanie 10 – przypadki użycia oraz DTO

| Przypadek użycia | Wejście (DTO) | Wynik |
|---|---|---|
| `AddBook` | `AddBookDto(Isbn, Title, Author, Copies)` | `Result<BookResult>` (Id, Isbn, Title, Author, Copies) |
| `RemoveBook` | `Guid bookId` | `Result<Unit>` |
| `RegisterReader` | `RegisterReaderDto(FirstName, LastName)` | `Result<ReaderResult>` (Id, FirstName, LastName) |
| `RemoveReader` | `Guid readerId` | `Result<Unit>` |
| `BorrowBook` | `BorrowBookDto(ReaderId, BookId)` | `Result<LoanResult>` (Id, BookId, ReaderId, LoanDate, DueDate) |
| `ReturnBook` | `Guid loanId` | `Result<Unit>` |
| `ExtendLoan` | `Guid loanId` | `Result<LoanResult>` |
| `ReserveBook` | `ReserveBookDto(ReaderId, BookId)` | `Result<ReservationResult>` (Id, BookId, ReaderId, CreatedAt) |
| `CancelReservation` | `Guid reservationId` | `Result<Unit>` |

Zapytania (`ILibraryQueries`):
- `GetBooks()` zwraca `BookDto` z liczbą dostępnych egzemplarzy.
- `GetActiveLoans(readerId)` zwraca listę `LoanDto`.
- `GetOverdueLoans()` zwraca listę `LoanDto`.

## Zadanie 11 – encje i obiekty wartości

- **Encje (korzenie agregatów):**
  - `Book`
  - `Reader`
  - `Loan` – wypożyczenie ma własną tożsamość i zmienia stan (zwrot, przedłużenie).
  - `Reservation` – ma własną tożsamość i status Active/Fulfilled/Cancelled.
- **Obiekty wartości:**
  - `Isbn` – walidacja formatu i cyfry kontrolnej.
  - `ReservationStatus` – enumeracja.
- Agregaty odwołują się do siebie wyłącznie przez identyfikator (`BookId`, `ReaderId`), więc każdy może być modyfikowany niezależnie.
- Reguły obejmujące kilka agregatów (limity wypożyczeń, kolejka rezerwacji) znajdują się w usłudze domenowej `LendingPolicy`.
- Bieżąca data pochodzi z portu wyjściowego `IClock`, dzięki czemu reguły zależne od czasu są testowalne.
