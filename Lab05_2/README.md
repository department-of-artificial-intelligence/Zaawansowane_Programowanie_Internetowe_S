# Lab 05 — RxJS i komunikacja z serwerem

Projekt korzysta z Vite, TypeScript i RxJS. Nie wymaga Webpacka.

## Uruchomienie

```sh
npm install
npm start
```

Otwórz `http://localhost:9000`. Przyciski na stronie wywołują przykładowe
żądania do serwera.

## Endpointy

- `GET /dane.json` — zwraca przykładowe dane z katalogu `public`;
- `POST /proceed` — przyjmuje JSON i odsyła go bez zmian;
- `/error` — zwraca status `404`;
- `/error1` — dwa pierwsze żądania zwracają `500`, a trzecie `200`; cykl
  powtarza się co trzy żądania.

Middleware tych endpointów znajduje się w `vite.config.js`.

## Budowanie

```sh
npm run build
```
