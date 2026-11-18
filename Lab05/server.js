const http = require("http");

// Sztuczne opóźnienie odpowiedzi dla /dane.json (w milisekundach),
// ułatwiające sprawdzenie działania operatora switchMap w Zadaniu 26
const DELAY_MS = 1500;

const sampleData = [
    { id: 1, nazwa: "TypeScript", kategoria: "Język programowania" },
    { id: 2, nazwa: "RxJS", kategoria: "Biblioteka reaktywna" },
    { id: 3, nazwa: "Webpack", kategoria: "Bundler" }
];

const server = http.createServer((req, res) => {
    // Nagłówki CORS wymagane, ponieważ aplikacja Webpacka działa na innym porcie (np. 8080)
    res.setHeader("Access-Control-Allow-Origin", "*");
    res.setHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
    res.setHeader("Access-Control-Allow-Headers", "Content-Type, X-Requested-With");

    // Obsługa zapytania wstępnego (preflight) przeglądarki
    if (req.method === "OPTIONS") {
        res.writeHead(204);
        res.end();
        return;
    }

    // ZADANIE 26: GET http://localhost:9000/dane.json (z opóźnieniem)
    if (req.url === "/dane.json" && req.method === "GET") {
        console.log(`[${new Date().toLocaleTimeString()}] Otrzymano żądanie GET /dane.json`);

        const timer = setTimeout(() => {
            res.writeHead(200, { "Content-Type": "application/json; charset=utf-8" });
            res.end(JSON.stringify(sampleData));
            console.log(`[${new Date().toLocaleTimeString()}] Wysłano odpowiedź /dane.json`);
        }, DELAY_MS);

        // Jeśli switchMap anuluje żądanie przed upływem DELAY_MS, przerywamy timer
        req.on("close", () => {
            if (!res.writableEnded) {
                clearTimeout(timer);
                console.log(`[${new Date().toLocaleTimeString()}] Żądanie /dane.json zostało anulowane przez przeglądarkę!`);
            }
        });
        return;
    }

    // ZADANIE 23: POST http://localhost:9000/proceed
    if (req.url === "/proceed" && req.method === "POST") {
        let body = "";
        req.on("data", (chunk) => {
            body += chunk.toString();
        });
        req.on("end", () => {
            res.writeHead(200, { "Content-Type": "application/json; charset=utf-8" });
            res.end(JSON.stringify({ status: "OK", odebrano: JSON.parse(body || "{}") }));
        });
        return;
    }

    // ZADANIE 24: http://localhost:9000/error -> 404
    if (req.url === "/error") {
        res.writeHead(404, { "Content-Type": "application/json; charset=utf-8" });
        res.end(JSON.stringify({ message: "Nie znaleziono zasobu" }));
        return;
    }

    // ZADANIE 25: http://localhost:9000/error1 -> losowo 500 lub 200
    if (req.url === "/error1") {
        if (Math.random() < 0.7) {
            res.writeHead(500, { "Content-Type": "application/json; charset=utf-8" });
            res.end(JSON.stringify({ message: "Internal Server Error" }));
        } else {
            res.writeHead(200, { "Content-Type": "application/json; charset=utf-8" });
            res.end(JSON.stringify({ status: "Połączenie udane!" }));
        }
        return;
    }

    res.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" });
    res.end("Nieznany adres");
});

server.listen(9000, () => {
    console.log("Serwer testowy działa pod adresem http://localhost:9000");
});