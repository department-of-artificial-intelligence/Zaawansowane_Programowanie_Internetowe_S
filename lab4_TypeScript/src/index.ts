import { App } from "./App";
import { Counter } from "./Counter";
import { TreeNode } from "./TreeNode";

document.addEventListener("DOMContentLoaded", () => {

    // --- Uruchomienie Zadania 4 (Licznik) ---
    try {
        new Counter("#value-input", "#plus-btn", "#minus-btn");
        console.log("Licznik (Zad 4) pomyślnie utworzony!");
    } catch (error: any) {
        console.error("Błąd inicjalizacji licznika:", error.message);
    }

    // --- Uruchomienie Zadania 5, 6 i 7 (Aplikacja pobierająca dane) ---
    try {
        // Przekazujemy ID kontenera ORAZ ID listy <select>
        const app = new App("#author-list-container", "#view-mode-select");
        app.start();

    } catch (error: any) {
        console.error("Błąd inicjalizacji aplikacji (Zad 5/6/7):", error.message);
    }

    // ===================================================
    // --- START: Zadanie 8 (Drzewo Generyczne) ---
    // ===================================================
    console.log("\n--- Zadanie 8: Drzewo Generyczne ---");

    // Definiujemy prosty interfejs dla naszych danych
    interface IPerson {
        name: string;
        position: string;
    }

    // 1. Tworzymy drzewo z typem generycznym <IPerson>
    const root = new TreeNode<IPerson>({ name: "Anna", position: "Prezes" });

    // Dodajemy dzieci
    const directorIT = root.addChild({ name: "Jan", position: "Dyrektor IT" });
    const directorFin = root.addChild({ name: "Ewa", position: "Dyrektor Finansów" });

    directorIT.addChild({ name: "Piotr", position: "Programista" });
    directorIT.addChild({ name: "Maria", position: "Tester" });

    const accountant = directorFin.addChild({ name: "Karol", position: "Księgowy" });
    accountant.addChild({ name: "Tomasz", position: "Młodszy Księgowy" });

    // 2. Używamy metody traverseDFS (w głąb)
    console.log("\n--- Przechodzenie w głąb (DFS) ---");
    // Nasza "akcja" (Wizytator) to funkcja strzałkowa,
    // która loguje wartość węzła.
    root.traverseDFS((node) => {
        // Proste wcięcie dla wizualizacji hierarchii
        let indent = "";
        let current = node.parent;
        while (current) {
            indent += "  ";
            current = current.parent;
        }
        console.log(`${indent}- ${node.value.name} (${node.value.position})`);
    });

    // 3. Używamy metody traverseBFS (wszerz)
    console.log("\n--- Przechodzenie wszerz (BFS) ---");
    // Tutaj akcja jest prostsza - po prostu listujemy węzły
    // w kolejności ich odwiedzin.
    let bfsResult = "Kolejność BFS: ";
    root.traverseBFS((node) => {
        bfsResult += `${node.value.name}, `;
    });
    console.log(bfsResult.slice(0, -2)); // Usuwamy ostatni przecinek i spację
});