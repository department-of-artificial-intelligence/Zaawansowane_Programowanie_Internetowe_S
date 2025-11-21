/**
 * Definiujemy typ dla naszej "akcji" (callbacka),
 * którą będziemy wykonywać na każdym węźle.
 * Jest to esencja wzorca Wizytator.
 */
export type VisitorAction<T> = (node: TreeNode<T>) => void;

/**
 * Generyczna klasa reprezentująca węzeł drzewa.
 * @template T Typ wartości przechowywanej w węźle.
 */
export class TreeNode<T> {

    // Wartość węzła (dowolnego typu T)
    public value: T;
    // Lista dzieci (tego samego typu)
    public children: TreeNode<T>[] = [];
    // Opcjonalna referencja do rodzica
    public parent: TreeNode<T> | null = null;

    constructor(value: T) {
        this.value = value;
    }

    /**
     * Dodaje nowy węzeł potomny do bieżącego węzła.
     * @param value Wartość dla nowego węzła potomnego.
     * @returns Nowo utworzony węzeł (TreeNode<T>).
     */
    public addChild(value: T): TreeNode<T> {
        const newNode = new TreeNode(value);
        newNode.parent = this; // Ustawiamy referencję do rodzica
        this.children.push(newNode);
        return newNode;
    }

    // --- Metody przechodzenia drzewa (Implementacja wzorca) ---

    /**
     * Przechodzi drzewo metodą w głąb (Depth-First Search - DFS).
     * Wykonuje podaną akcję na każdym węźle (pre-order).
     * @param action Funkcja (wizytator), która ma być wykonana na każdym węźle.
     */
    public traverseDFS(action: VisitorAction<T>): void {
        // 1. Wykonaj akcję na bieżącym węźle (to jest "wizyta")
        action(this);

        // 2. Odwiedź rekurencyjnie wszystkie dzieci
        for (const child of this.children) {
            child.traverseDFS(action);
        }
    }

    /**
     * Przechodzi drzewo metodą wszerz (Breadth-First Search - BFS).
     * Wykonuje podaną akcję na każdym węźle.
     * @param action Funkcja (wizytator), która ma być wykonana na każdym węźle.
     */
    public traverseBFS(action: VisitorAction<T>): void {
        // Używamy kolejki (Queue) do śledzenia węzłów do odwiedzenia
        const queue: TreeNode<T>[] = [this];

        // Dopóki kolejka nie jest pusta
        while (queue.length > 0) {
            // 1. Pobierz pierwszy węzeł z kolejki
            const currentNode = queue.shift()!; // '!' mówi TS, że wiemy, że nie jest 'undefined'

            // 2. Wykonaj akcję na bieżącym węźle ("wizyta")
            action(currentNode);

            // 3. Dodaj wszystkie dzieci bieżącego węzła na koniec kolejki
            for (const child of currentNode.children) {
                queue.push(child);
            }
        }
    }
}