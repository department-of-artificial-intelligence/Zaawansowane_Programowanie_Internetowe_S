// Eksportujemy klasę, aby można ją było importować w index.ts
export class Counter {

    // Prywatne pola przechowujące stan i referencje do DOM
    private _value: number;

    // Elementy DOM, którymi będziemy zarządzać
    private inputElement: HTMLInputElement;
    private plusButton: HTMLButtonElement;
    private minusButton: HTMLButtonElement;

    /**
     * Konstruktor klasy Counter.
     * Używamy "wstrzykiwania zależności" - przekazujemy ID elementów,
     * którymi klasa ma zarządzać.
     */
    constructor(inputSelector: string, plusSelector: string, minusSelector: string) {

        // Znajdź elementy w dokumencie
        this.inputElement = document.querySelector(inputSelector)!;
        this.plusButton = document.querySelector(plusSelector)!;
        this.minusButton = document.querySelector(minusSelector)!;

        // Sprawdzenie, czy elementy istnieją (dla bezpieczeństwa)
        if (!this.inputElement || !this.plusButton || !this.minusButton) {
            throw new Error("Nie można znaleźć elementów licznika. Sprawdź selektory.");
        }

        // Inicjalizacja wartości - pobieramy ją z pola input lub ustawiamy na 0
        this._value = parseInt(this.inputElement.value) || 0;

        // Podpięcie "nasłuchiwaczy" zdarzeń (event listeners)
        this.attachListeners();
    }

    /**
     * Prywatna metoda do podpinania zdarzeń.
     * Używamy funkcji strzałkowych dla zachowania kontekstu 'this'.
     */
    private attachListeners(): void {
        this.plusButton.addEventListener('click', this.increment);
        this.minusButton.addEventListener('click', this.decrement);
    }

    /**
     * Prywatna metoda do aktualizowania widoku (pola input).
     */
    private updateView(): void {
        this.inputElement.value = this._value.toString();
    }

    // --- Metody publiczne (API naszej klasy) ---

    /**
     * Metoda publiczna zwiększająca wartość licznika.
     * Używamy funkcji strzałkowej, aby 'this' zawsze odnosiło się do instancji klasy.
     */
    public increment = (): void => {
        this._value++;
        this.updateView();
    }

    /**
     * Metoda publiczna zmniejszająca wartość licznika.
     */
    public decrement = (): void => {
        this._value--;
        this.updateView();
    }

    /**
     * Publiczny getter do odczytu aktualnej wartości.
     */
    public get value(): number {
        return this._value;
    }
}