import { ajax } from "rxjs/ajax";
import { fromEvent, retry, map,
    debounceTime,
    distinctUntilChanged,
    switchMap,
    catchError,
    of,
    delay,
    mergeMap,
    concatMap } from "rxjs";

// zad 21
/*
ajax("dane.json").subscribe({
    next: odpowiedz => {
        console.log(odpowiedz.response);
    },
    error: blad => {
        console.log("Wystąpił błąd:", blad);
    }
});
*/

//zad 2
/*
interface Wpis {
    id: number;
    nazwa: string;
}

const tabela = document.querySelector<HTMLTableElement>("#tabela")!;

ajax<Wpis[]>("dane.json").subscribe({
    next: odpowiedz => {
        odpowiedz.response.forEach(wpis => {
            const wiersz = tabela.insertRow();

            const komorkaId = wiersz.insertCell();
            const komorkaNazwa = wiersz.insertCell();

            komorkaId.textContent = wpis.id.toString();
            komorkaNazwa.textContent = wpis.nazwa;
        });
    },
    error: blad => {
        console.log("Wystąpił błąd:", blad);
    }
});
*/
// zadanie 23

/*

const formularz = document.querySelector<HTMLFormElement>("#formularz")!;

const inputImie = document.querySelector<HTMLInputElement>("#imie")!;
const inputNazwisko = document.querySelector<HTMLInputElement>("#nazwisko")!;
const inputEmail = document.querySelector<HTMLInputElement>("#email")!;

fromEvent<SubmitEvent>(formularz, "submit")
    .subscribe(event => {
        event.preventDefault();

        const dane = {
            imie: inputImie.value,
            nazwisko: inputNazwisko.value,
            email: inputEmail.value
        };

        ajax({
            url: "http://localhost:9000/proceed",
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: dane
        }).subscribe({
            next: odpowiedz => {
                console.log("Odpowiedź serwera:", odpowiedz.response);
            },
            error: blad => {
                console.log("Wystąpił błąd:", blad);
            }
        });
    });
*/

// zadanie 24

/*
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

ajax("http://localhost:9000/error").subscribe({
    next: odpowiedz => {
        console.log(odpowiedz.response);
    },
    error: blad => {
        console.log("Wystąpił błąd:", blad);

        paragraf.textContent = "Błąd 404: Nie znaleziono żądanego zasobu!";
    }
});
*/
// zadanie 25

/*
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

ajax("http://localhost:9000/error1")
    .pipe(
        retry(2)
    )
    .subscribe({
        next: odpowiedz => {
            console.log("Odpowiedź serwera:", odpowiedz.response);
            paragraf.textContent = "Połączenie z serwerem zakończone sukcesem.";
        },
        error: blad => {
            console.log("Wystąpił błąd:", blad);
            paragraf.textContent = "Nie udało się połączyć z serwerem po 3 próbach.";
        }
    });
*/

// zadanie 26
/*

const input = document.querySelector<HTMLInputElement>("#szukaj")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

fromEvent(input, "input")
    .pipe(
        map(() => input.value),

        debounceTime(500),

        distinctUntilChanged(),

        switchMap(tekst =>
            ajax("http://localhost:9000/dane.json").pipe(
                map(odpowiedz => ({
                    tekst: tekst,
                    dane: odpowiedz.response
                })),
                catchError(blad => {
                    console.log("Błąd:", blad);
                    paragraf.textContent = "Wystąpił błąd podczas pobierania danych!";
                    return of(null);
                })
            )
        )
    )
    .subscribe(wynik => {
        if (wynik !== null) {
            paragraf.textContent =
                JSON.stringify(wynik.dane, null, 2);
        }
    });
*/

// zadanie 27


const button = document.querySelector<HTMLButtonElement>("#przycisk")!;
const paragraf = document.querySelector<HTMLParagraphElement>("#info")!;

let licznik = 0;

fromEvent(button, "click")
    .pipe(
        // wariant 1
        // switchMap(() => {
        // jeśli kliknie się szybko pare razy, wyświetlony zostanie tylko ostatni wynik a reszta zostanie anulowana

        // wariant 2
        //mergeMap(() => {
        // wszystkie zadania zostana obsluzone zawsze

        // wariant 3
        concatMap(() => {
        // jeśli kliknie sie szybko parerazy wszystkie zostana obsluzone, ale jedno po drugim

            licznik++;

            const numer = licznik;

            console.log("Wysłano żądanie:", numer);

            return of(numer).pipe(
                delay(2000)
            );
        })
    )
    .subscribe(wynik => {
        console.log("Odpowiedź:", wynik);
        paragraf.textContent += `Otrzymano odpowiedź nr ${wynik}\n`;
    });